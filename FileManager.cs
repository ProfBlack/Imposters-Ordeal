﻿using AssetsTools.NET;
using AssetsTools.NET.Extra;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static ImpostersOrdeal.GlobalData;
using SmartPoint.AssetAssistant;
using System.Configuration;
using System.Text;
using Newtonsoft.Json;
using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    /// <summary>
    ///  Responsible for handling all files.
    /// </summary>
    public class FileManager
    {
        public static readonly string outputModName = "Output";
        public static readonly string outputYAMLModName = "OutputYAML";
        public static readonly string tempLocationName = "Temp";
        public static readonly string[] assetAssistantRelevantFiles = new string[]
        {
            "\\Contest\\md\\contest_masterdatas",
            "\\Battle\\battle_masterdatas",
            "\\Dpr\\ev_script",
            "\\Dpr\\masterdatas",
            "\\Dpr\\scriptableobjects\\gamesettings",
            "\\Message\\common_msbt",
            "\\Message\\english",
            "\\Message\\french",
            "\\Message\\german",
            "\\Message\\italian",
            "\\Message\\jpn",
            "\\Message\\jpn_kanji",
            "\\Message\\korean",
            "\\Message\\simp_chinese",
            "\\Message\\spanish",
            "\\Message\\trad_chinese",
            "\\Pml\\personal_masterdatas",
            "\\UIs\\masterdatas\\uimasterdatas",
            "\\UnderGround\\data\\ugdata"
        };
        private static readonly string delphisMainPath = "romfs\\Data\\StreamingAssets\\Audio\\GeneratedSoundBanks\\Switch\\Delphis_Main.bnk";
        private static readonly string globalMetadataPath = "romfs\\Data\\Managed\\Metadata\\global-metadata.dat";
        private static readonly string dprBinPath = "romfs\\Data\\StreamingAssets\\AssetAssistant\\Dpr.bin";
        private static readonly string externalJsonGamePath = "romfs\\Data\\ExtraData";
        private static readonly string externalUnityJsonGamePath = "ExtraData";
        private static readonly string modArgsPath = "ImpostersOrdealArgs.json";

        private string assetAssistantPath;
        private string audioPath;
        private string metadataPath;
        private string externalJsonPath;
        private Dictionary<string, FileData> fileArchive;
        private Dictionary<string, YAMLFileData> yamlArchive;
        private AssetsManager am = new();
        private int fileIndex = 0;

        private static SharpYaml.Serialization.Serializer serializer;
        private static SharpYaml.Serialization.Serializer Serializer
        {
            get
            {
                if (serializer == null)
                {
                    var settings = new SerializerSettings
                    {
                        SortKeyForMapping = false,
                        IgnoreNulls = true,
                        EmitAlias = false,
                        IndentLess = true,
                    };
                    settings.RegisterSerializer(typeof(UnityFile), new UnityFileSerializer());
                    settings.RegisterSerializer(typeof(float), new FloatSerializer());
                    settings.RegisterSerializer(typeof(string), new StringSerializer());
                    serializer = new SharpYaml.Serialization.Serializer(settings);
                }

                return serializer;
            }
        }

        private class FileData
        {
            public string fileLocation;
            public string gamePath;
            public FileSource fileSource;
            public BundleFileInstance bundle;
            public bool tempLocation;

            public bool IsBundle()
            {
                return bundle != null;
            }
        }

        private class YAMLFileData
        {
            public string fileLocation;
            public string assetPath;
            public string unityPrefix;
            public PathEnum bundleOrigin;
            public bool tempLocation;
            public bool unknownMono;
            public YamlMonoContainer loadedData;

            public bool IsLoaded()
            {
                return loadedData != null && loadedData.MonoBehaviour != null;
            }

            public YamlMonoContainer GetLoadedData()
            {
                if (IsLoaded() || unknownMono)
                    return loadedData;

                var yamlLines = File.ReadAllLines(fileLocation);
                string yaml = string.Join("\n", yamlLines.Skip(4));
                unityPrefix = string.Join("\n", yamlLines.Take(3)) + "\n";

                var mono = new YamlMonoContainer() { MonoBehaviour = FromYAML(bundleOrigin, yaml) };
                loadedData = mono;
                unknownMono = mono.MonoBehaviour == null;

                return mono;
            }
        }

        private enum FileSource
        {
            Dump,
            Mod,
            UnrelatedMod,
            App
        }

        public AssetsManager GetAssetsManager()
        {
            return am;
        }

        /// <summary>
        ///  Gets dump from user and opens all necessary files.
        /// </summary>
        public bool InitializeFromInput()
        {
            //Get the dump path from user.
            FolderBrowserDialog fbd = new();
            fbd.Description = "Select the folder containing the romfs/exefs.";
            if (fbd.ShowDialog() != DialogResult.OK)
                return false;

            //Check that it's a game directory
            if (!IsGameDirectory(fbd.SelectedPath, true))
            {
                MessageBox.Show("Path does not contain a romfs folder.",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            //Get the AssetAssistant path
            assetAssistantPath = GetAssetAssistantPath(fbd.SelectedPath);
            if (assetAssistantPath == "")
            {
                MessageBox.Show("Path does not contain path:\n\\romfs\\Data\\StreamingAssets\\AssetAssistant",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            audioPath = Directory.GetParent(assetAssistantPath).FullName + "\\Audio\\GeneratedSoundBanks\\Switch";
            metadataPath = GetDataPath(fbd.SelectedPath) + "\\Managed\\Metadata";
            externalJsonPath = GetDataPath(fbd.SelectedPath) + "\\ExtraData";

            //Setup fileArchive
            fileArchive = new();
            for (int i = 0; i < assetAssistantRelevantFiles.Length; i++)
            {
                string absolutePath = assetAssistantPath + assetAssistantRelevantFiles[i];
                string gamePath = "romfs\\Data\\StreamingAssets\\AssetAssistant" + assetAssistantRelevantFiles[i];
                if (!File.Exists(absolutePath))
                {
                    MessageBox.Show("File not found:\n" + gamePath + "\nIncomplete dump.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    fileArchive = null;
                    return false;
                }

                FileData fd = new();
                fd.fileLocation = absolutePath;
                fd.gamePath = gamePath;
                fd.fileSource = FileSource.Dump;
                fd.bundle = am.LoadBundleFile(absolutePath, false);
                DecompressBundle(fd.bundle);
                fileArchive[gamePath] = fd;
            }

            Configuration c = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            c.AppSettings.Settings["dumpPath"].Value = fbd.SelectedPath;
            c.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(c.AppSettings.SectionInformation.Name);

            //Setup yamlArchive
            yamlArchive = new();

            return true;
        }

        /// <summary>
        ///  Reads dump path from config and attemts to open all necessary files.
        /// </summary>
        public bool InitializeFromConfig()
        {
            //Get the dump path from config.
            string dumpPath = System.Configuration.ConfigurationManager.AppSettings["dumpPath"];

            //Check that it's a game directory
            if (!IsGameDirectory(dumpPath, true))
                return false;

            //Get the AssetAssistant path
            assetAssistantPath = GetAssetAssistantPath(dumpPath);
            if (assetAssistantPath == "")
                return false;
            audioPath = Directory.GetParent(assetAssistantPath).FullName + "\\Audio\\GeneratedSoundBanks\\Switch";
            metadataPath = GetDataPath(dumpPath) + "\\Managed\\Metadata";
            externalJsonPath = GetDataPath(dumpPath) + "\\ExtraData";

            //Setup fileArchive
            fileArchive = new();
            for (int i = 0; i < assetAssistantRelevantFiles.Length; i++)
            {
                string absolutePath = assetAssistantPath + assetAssistantRelevantFiles[i];
                string gamePath = "romfs\\Data\\StreamingAssets\\AssetAssistant" + assetAssistantRelevantFiles[i];
                if (!File.Exists(absolutePath))
                    return false;

                FileData fd = new();
                fd.fileLocation = absolutePath;
                fd.gamePath = gamePath;
                fd.fileSource = FileSource.Dump;
                fd.bundle = am.LoadBundleFile(absolutePath, false);
                DecompressBundle(fd.bundle);
                fileArchive[gamePath] = fd;
            }

            //setup yamlArchive
            yamlArchive = new();

            return true;
        }

        /// <summary>
        ///  Gets a mod directory from user and loads all the files it contains into fileArchive.
        /// </summary>
        public bool AddMod()
        {
            bool reanalysisNecessary = false;

            //Get the dump path from user
            FolderBrowserDialog fbd = new();
            fbd.Description = "Select a mod folder containing a romfs and/or exefs.";
            if (fbd.ShowDialog() != DialogResult.OK)
                return reanalysisNecessary;

            //Check that it's a game directory
            if (!IsGameDirectory(fbd.SelectedPath))
            {
                MessageBox.Show("Path does not contain a romfs or exefs folder.",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return reanalysisNecessary;
            }

            //Loads all files
            string[] modFilePaths = Directory.GetFiles(fbd.SelectedPath, "*", SearchOption.AllDirectories);
            List<(int, string)> conflicts = new();
            for (int fileIdx = 0; fileIdx < modFilePaths.Length; fileIdx++)
            {
                string absolutePath = fbd.SelectedPath;
                string gamePath = modFilePaths[fileIdx].Substring(absolutePath.Length + 1, modFilePaths[fileIdx].Length - absolutePath.Length - 1);
                if (!fileArchive.ContainsKey(gamePath))
                {
                    FileData fd = new();
                    fd.fileLocation = modFilePaths[fileIdx];
                    fd.gamePath = gamePath;
                    fd.fileSource = FileSource.UnrelatedMod;
                    fileArchive[gamePath] = fd;
                    continue;
                }

                if (fileArchive[gamePath].fileSource == FileSource.Dump)
                {
                    fileArchive[gamePath].fileLocation = modFilePaths[fileIdx];
                    fileArchive[gamePath].fileSource = FileSource.Mod;
                    if (fileArchive[gamePath].IsBundle())
                    {
                        fileArchive[gamePath].bundle = am.LoadBundleFile(modFilePaths[fileIdx], false);
                        DecompressBundle(fileArchive[gamePath].bundle);
                    }
                    reanalysisNecessary = true;
                    continue;
                }

                if (fileArchive[gamePath].fileSource == FileSource.Mod || fileArchive[gamePath].fileSource == FileSource.App)
                {
                    BundleFileInstance bfi = am.LoadBundleFile(modFilePaths[fileIdx], false);
                    DecompressBundle(bfi);
                    if (!Merge(fileArchive[gamePath], bfi))
                    {
                        MainForm.ShowParserError("Unable to merge instances of:\n" +
                            gamePath + "\n" +
                            "Asset count mismatch.");
                        continue;
                    }
                    fileArchive[gamePath].fileSource = FileSource.Mod;
                    reanalysisNecessary = true;
                    continue;
                }

                if (fileArchive[gamePath].fileSource == FileSource.UnrelatedMod)
                {
                    //Loads unrelated bundle if possible
                    if (!fileArchive[gamePath].IsBundle() && gamePath.Contains("AssetAssistant") && Path.GetExtension(gamePath) == "")
                        try
                        {
                            fileArchive[gamePath].bundle = am.LoadBundleFile(fileArchive[gamePath].fileLocation, false);
                            DecompressBundle(fileArchive[gamePath].bundle);
                        }
                        catch (Exception) { }

                    if (fileArchive[gamePath].IsBundle())
                    {
                        BundleFileInstance bfi = am.LoadBundleFile(modFilePaths[fileIdx], false);
                        DecompressBundle(bfi);
                        if (!Merge(fileArchive[gamePath], bfi))
                            MainForm.ShowParserError("Unable to merge instances of:\n" +
                                gamePath + "\n" +
                                "Asset count mismatch.");
                    }
                    else
                        conflicts.Add((fileIdx, Path.GetFileName(fileArchive[gamePath].fileLocation)));
                    continue;
                }
            }

            if (conflicts.Count == 0)
                return reanalysisNecessary;

            //Resolve file conflicts
            List<int> overwrites = new();
            FileSelectForm fsf = new(conflicts, overwrites);
            fsf.ShowDialog();
            for (int i = 0; i < overwrites.Count; i++)
            {
                int fileIdx = overwrites[i];
                string absolutePath = fbd.SelectedPath;
                string gamePath = modFilePaths[fileIdx].Substring(absolutePath.Length + 1, modFilePaths[fileIdx].Length - absolutePath.Length - 1);
                fileArchive[gamePath].fileLocation = modFilePaths[fileIdx];
            }
            return reanalysisNecessary;
        }

        /// <summary>
        ///  Gets a mod directory from the user and loads all the needed files it contains into yamlArchive.
        /// </summary>
        public bool AddYAMLMod()
        {
            //Get the dump path from user
            FolderBrowserDialog fbd = new();
            fbd.Description = "Select a mod folder containing an Assets folder.";
            if (fbd.ShowDialog() != DialogResult.OK)
                return false;

            //Check that it's a game directory
            if (!IsUnityDirectory(fbd.SelectedPath))
            {
                MessageBox.Show("Path does not contain an Assets folder.",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            string assetsPath = Path.Combine(fbd.SelectedPath, "Assets");

            //Loads all files
            string[] modFilePaths = Directory.GetFiles(assetsPath, "*.asset", SearchOption.AllDirectories)
                             .Union(Directory.GetFiles(assetsPath, "*.json", SearchOption.AllDirectories))
                             .ToArray();
            for (int fileIdx = 0; fileIdx < modFilePaths.Length; fileIdx++)
            {
                string gamePath = modFilePaths[fileIdx].Substring(assetsPath.Length + 1, modFilePaths[fileIdx].Length - assetsPath.Length - 1);
                if (!yamlArchive.ContainsKey(gamePath))
                {
                    var bundleOrigin = FindPathEnumForYAML(gamePath);
                    if (bundleOrigin.HasValue)
                        yamlArchive[gamePath] = GenerateFileDataFromYAMLFile(modFilePaths[fileIdx], gamePath, bundleOrigin.Value);
                } 
            }

            return true;
        }

        /// <summary>
        ///  Finds what path in the PathEnum a yaml file is for.
        /// </summary>
        private YAMLFileData GenerateFileDataFromYAMLFile(string fileLocation, string assetPath, PathEnum origin)
        {
            YAMLFileData fd = new();
            fd.fileLocation = fileLocation;
            fd.assetPath = assetPath;
            fd.tempLocation = false;
            fd.bundleOrigin = origin;
            fd.unknownMono = false;

            return fd;
        }

        /// <summary>
        ///  Exports current file archive into local directory.
        /// </summary>
        public void ExportMod()
        {
            string outputDirectory = Path.Combine(Environment.CurrentDirectory, outputModName);
            if (Directory.Exists(outputDirectory))
                Directory.Delete(outputDirectory, true);
            Directory.CreateDirectory(outputDirectory);
            for (int fileDataIdx = 0; fileDataIdx < fileArchive.Count; fileDataIdx++)
                if (fileArchive.Values.ToList()[fileDataIdx].fileSource != FileSource.Dump)
                    ExportFile(fileArchive.Values.ToList()[fileDataIdx], outputDirectory);
        }

        /// <summary>
        ///  Exports current yaml archive into local directory.
        /// </summary>
        public void ExportModToYAML()
        {
            string outputDirectory = Path.Combine(Environment.CurrentDirectory, outputYAMLModName);
            if (Directory.Exists(outputDirectory))
                Directory.Delete(outputDirectory, true);
            Directory.CreateDirectory(outputDirectory);
            for (int fileDataIdx = 0; fileDataIdx < yamlArchive.Count; fileDataIdx++)
                ExportYAMLFile(yamlArchive.Values.ToList()[fileDataIdx], outputDirectory);
        }

        public BundleFileInstance GetPokemonBundleFileInstance(string path)
        {
            string absolutePath = assetAssistantPath + "\\Pokemon Database\\" + path;
            string gamePath = "romfs\\Data\\StreamingAssets\\AssetAssistant\\Pokemon Database\\" + path;

            if (!fileArchive.ContainsKey(gamePath))
            {
                if (!File.Exists(absolutePath))
                {
                    MessageBox.Show("File not found:\n" + gamePath,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return null;
                }

                FileData fd = new();
                fd.fileLocation = absolutePath;
                fd.gamePath = gamePath;
                fd.fileSource = FileSource.Dump;
                fileArchive[gamePath] = fd;
            }

            if (!fileArchive[gamePath].IsBundle())
            {
                FileData fd = fileArchive[gamePath];
                fd.bundle = am.LoadBundleFile(fd.fileLocation, false);
                DecompressBundle(fd.bundle);
            }

            return fileArchive[gamePath].bundle;
        }

        /// <summary>
        ///  Returns the specified BundleFileInstance in Pokemon Database if it's loaded, and null otherwise.
        /// </summary>
        public BundleFileInstance TryGetPokemonBundleFileInstance(string path)
        {
            string gamePath = "romfs\\Data\\StreamingAssets\\AssetAssistant\\Pokemon Database\\" + path;

            if (!fileArchive.ContainsKey(gamePath))
                return null;

            if (MessageBox.Show("Assetbundle detected in mod:\n" + gamePath +
                "\nNeeds full set of assetbundles to succeed.\nCopy assetbundles from mod instead?",
            "Assetbundle Detected", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
                return null;

            if (!fileArchive[gamePath].IsBundle())
            {
                FileData fd = fileArchive[gamePath];
                fd.bundle = am.LoadBundleFile(fd.fileLocation, false);
                DecompressBundle(fd.bundle);
            }

            return fileArchive[gamePath].bundle;
        }

        /// <summary>
        ///  Returns the BundleFileInstance of the texturemass bundle.
        /// </summary>
        public BundleFileInstance GetTexturemassBundle()
        {
            string absolutePath = assetAssistantPath + "\\UIs\\textures_mass\\texturemass";
            string gamePath = "romfs\\Data\\StreamingAssets\\AssetAssistant\\UIs\\textures_mass\\texturemass";

            if (!fileArchive.ContainsKey(gamePath))
            {
                if (!File.Exists(absolutePath))
                {
                    MessageBox.Show("File not found:\n" + gamePath,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return null;
                }

                FileData fd = new();
                fd.fileLocation = absolutePath;
                fd.gamePath = gamePath;
                fd.fileSource = FileSource.Dump;
                fileArchive[gamePath] = fd;
            }

            if (!fileArchive[gamePath].IsBundle())
            {
                FileData fd = fileArchive[gamePath];
                fd.bundle = am.LoadBundleFile(fd.fileLocation, false);
                DecompressBundle(fd.bundle);
            }

            return fileArchive[gamePath].bundle;
        }

        public void WriteTexturemassBundle(List<AssetsReplacer> ars)
        {
            string gamePath = "romfs\\Data\\StreamingAssets\\AssetAssistant\\UIs\\textures_mass\\texturemass";
            fileArchive[gamePath].fileSource = FileSource.App;
            MakeTempBundle(fileArchive[gamePath], ars, "texturemass");
        }

        /// <summary>
        ///  Gets a list of MonoBehaviour value fields by PathEnum.
        /// </summary>
        public List<AssetTypeValueField> GetMonoBehaviours(PathEnum pathEnum)
        {
            BundleFileInstance bfi = fileArchive[randomizerPaths[pathEnum]].bundle;
            AssetsFileInstance afi = am.LoadAssetsFileFromBundle(bfi, 0);
            return afi.table.GetAssetsOfType(114).Select(afie => am.GetTypeInstance(afi, afie).GetBaseField()).ToList();
        }

        /// <summary>
        ///  Gets a list of yamls by PathEnum.
        /// </summary>
        public List<YamlMonoContainer> GetYAMLs(PathEnum pathEnum)
        {
            return yamlArchive.Where(y => y.Value.bundleOrigin == pathEnum)
                .Select(y => y.Value.GetLoadedData())
                .ToList();
        }

        /// <summary>
        ///  Overwrites a MonoBehaviour in a bundle.
        /// </summary>
        public void WriteMonoBehaviour(PathEnum pathEnum, AssetTypeValueField monoBehaviour)
        {
            WriteMonoBehaviours(pathEnum, new AssetTypeValueField[] { monoBehaviour });
        }

        /// <summary>
        ///  Overwrites an array of MonoBehaviours in a bundle.
        /// </summary>
        public void WriteMonoBehaviours(PathEnum pathEnum, AssetTypeValueField[] atvfs)
        {
            FileData fd = fileArchive[randomizerPaths[pathEnum]];
            List<AssetsReplacer> ars = new();
            for (int i = 0; i < atvfs.Length; i++)
            {
                BundleFileInstance bfi = fd.bundle;
                AssetsFileInstance afi = am.LoadAssetsFileFromBundle(bfi, bfi.file.bundleInf6.dirInf[0].name);
                AssetTypeValueField atvf = atvfs[i];

                byte[] b = atvf.WriteToByteArray();
                AssetFileInfoEx afie = afi.table.GetAssetInfo(atvf.Get("m_Name").GetValue().AsString(), 114);
                AssetsReplacerFromMemory arfm = new AssetsReplacerFromMemory(0, afie.index, (int)afie.curFileType, AssetHelper.GetScriptIndex(afi.file, afie), b);
                ars.Add(arfm);
            }
            MakeTempBundle(fd, ars, Path.GetFileName(fd.gamePath));
            fd.fileSource = FileSource.App;
        }

        /// <summary>
        ///  Returns an array containing the filenames of all move command sequences.
        /// </summary>
        public string[] GetMoveSequences()
        {
            return Directory.GetFiles(assetAssistantPath + "\\Battle\\btlv\\waza\\sequence", "*", SearchOption.TopDirectoryOnly).Select(s => Path.GetFileName(s)).ToArray();
        }

        /// <summary>
        ///  Returns the Delphis_Main.bnk file as a byte array.
        /// </summary>
        public byte[] GetDelphisMainBuffer()
        {
            if (!fileArchive.ContainsKey(delphisMainPath))
            {
                FileData fd = new();
                fd.fileLocation = audioPath + "\\Delphis_Main.bnk";
                fd.fileSource = FileSource.Dump;
                fd.gamePath = delphisMainPath;
                fileArchive[delphisMainPath] = fd;
            }
            return File.ReadAllBytes(fileArchive[delphisMainPath].fileLocation);
        }

        public byte[] GetInitBuffer()
        {
            return File.ReadAllBytes(audioPath + "\\Init.bnk");
        }

        /// <summary>
        ///  Returns the global-metadata.dat file as a byte array.
        /// </summary>
        public byte[] GetGlobalMetadataBuffer()
        {
            if (!fileArchive.ContainsKey(globalMetadataPath))
            {
                FileData fd = new();
                fd.fileLocation = metadataPath + "\\global-metadata.dat";
                fd.fileSource = FileSource.Dump;
                fd.gamePath = globalMetadataPath;
                fileArchive[globalMetadataPath] = fd;
            }
            return File.ReadAllBytes(fileArchive[globalMetadataPath].fileLocation);
        }

        public AssetBundleDownloadManifest GetDprBin()
        {
            if (!fileArchive.ContainsKey(dprBinPath))
            {
                FileData fd = new();
                fd.fileLocation = assetAssistantPath + "\\Dpr.bin";
                fd.fileSource = FileSource.Dump;
                fd.gamePath = dprBinPath;
                fileArchive[dprBinPath] = fd;
            }
            try
            {
                return AssetBundleDownloadManifest.Load(fileArchive[dprBinPath].fileLocation);
            }
            catch (IOException)
            {
                return null;
            }
        }

        /// <summary>
        ///  Makes it so the audioCollection data is included when exporting.
        /// </summary>
        public void CommitAudio()
        {
            fileArchive[delphisMainPath].fileSource = FileSource.App;
        }

        /// <summary>
        ///  Makes it so the global-metadata is included when exporting.
        /// </summary>
        public void CommitGlobalMetadata()
        {
            fileArchive[globalMetadataPath].fileSource = FileSource.App;
        }

        /// <summary>
        ///  Makes it so dprBin is included when exporting.
        /// </summary>
        public void CommitDprBin()
        {
            fileArchive[dprBinPath].fileSource = FileSource.App;
        }

        public void DuplicateAudioSource(uint src, uint dst)
        {
            FileData fd = new();
            fd.fileLocation = audioPath + "\\" + src + ".wem";
            if (fileArchive.TryGetValue("romfs\\Data\\StreamingAssets\\Audio\\GeneratedSoundBanks\\Switch\\" + src + ".wem", out FileData srcFD))
            {
                fd.fileLocation = srcFD.fileLocation;
                (fd.tempLocation, srcFD.tempLocation) = (srcFD.tempLocation, false);
            }
            fd.gamePath = "romfs\\Data\\StreamingAssets\\Audio\\GeneratedSoundBanks\\Switch\\" + dst + ".wem";
            fd.fileSource = FileSource.App;
            fileArchive[fd.gamePath] = fd;
        }

        public StringBuilder GetAudioSourceLog()
        {
            string logPath = "romfs\\Data\\StreamingAssets\\Audio\\GeneratedSoundBanks\\Switch\\AudioSources.txt";
            if (!fileArchive.ContainsKey(logPath))
            {
                FileData fd = new();
                fd.fileLocation = audioPath + "\\AudioSources.txt";
                fd.gamePath = logPath;
                fd.fileSource = FileSource.App;
                fileArchive[logPath] = fd;
                return new("");
            }

            fileArchive[logPath].fileSource = FileSource.App;

            return new(File.ReadAllText(fileArchive[logPath].fileLocation));
        }

        public List<(string, T)> TryGetExternalJsons<T>(string externalJsonDir, bool unityPath = false)
        {
            if (unityPath)
            {
                string gamePath = Path.Combine(externalUnityJsonGamePath, externalJsonDir);
                List<(string, T)> result = new();
                foreach (string path in yamlArchive.Keys.Where(s => s.StartsWith(gamePath)))
                    result.Add((Path.GetFileNameWithoutExtension(path), GetExternalJson<T>(path, unityPath)));
                return result;
            }
            else
            {
                string gamePath = Path.Combine(externalJsonGamePath, externalJsonDir);
                List<(string, T)> result = new();
                foreach (string path in fileArchive.Keys.Where(s => s.StartsWith(gamePath)))
                    result.Add((Path.GetFileNameWithoutExtension(path), GetExternalJson<T>(path, unityPath)));
                return result;
            }
        }

        public T TryGetExternalJson<T>(string externalJsonPath, bool unityPath = false)
        {
            if (unityPath)
            {
                string gamePath = Path.Combine(externalUnityJsonGamePath, externalJsonPath);
                if (!yamlArchive.ContainsKey(gamePath))
                    return default;
                return GetExternalJson<T>(gamePath, unityPath);
            }
            else
            {
                string gamePath = Path.Combine(externalJsonGamePath, externalJsonPath);
                if (!fileArchive.ContainsKey(gamePath))
                    return default;
                return GetExternalJson<T>(gamePath, unityPath);
            }
        }

        private T GetExternalJson<T>(string gamePath, bool unityPath = false)
        {
            if (unityPath)
                return JsonConvert.DeserializeObject<T>(File.ReadAllText(yamlArchive[gamePath].fileLocation));
            else
                return JsonConvert.DeserializeObject<T>(File.ReadAllText(fileArchive[gamePath].fileLocation));
        }

        public void CommitExternalJson(string externalJsonPath, bool unityPath = false)
        {
            if (unityPath)
            {
                string gamePath = Path.Combine(externalUnityJsonGamePath, externalJsonPath);
                if (!yamlArchive.ContainsKey(gamePath))
                    yamlArchive[gamePath] = new() { assetPath = gamePath };
            }
            else
            {
                string gamePath = Path.Combine(externalJsonGamePath, externalJsonPath);
                if (!fileArchive.ContainsKey(gamePath))
                    fileArchive[gamePath] = new() { gamePath = gamePath };
                fileArchive[gamePath].fileSource = FileSource.App;
            }
        }

        public ModArgs TryGetModArgs()
        {
            if (!fileArchive.ContainsKey(modArgsPath))
                return null;
            return JsonConvert.DeserializeObject<ModArgs>(File.ReadAllText(fileArchive[modArgsPath].fileLocation));
        }

        public void DuplicateIconBundle(string srcPath, string dstPath)
        {
            FileData fd = new();
            fd.fileLocation = assetAssistantPath + "\\UIs\\" + srcPath;
            if (fileArchive.TryGetValue("romfs\\Data\\StreamingAssets\\AssetAssistant\\UIs\\" + srcPath, out FileData srcFD))
            {
                fd.fileLocation = srcFD.fileLocation;
                (fd.tempLocation, srcFD.tempLocation) = (srcFD.tempLocation, false);
            }
            fd.gamePath = "romfs\\Data\\StreamingAssets\\AssetAssistant\\UIs\\" + dstPath;
            fd.fileSource = FileSource.App;
            fileArchive[fd.gamePath] = fd;
        }

        /// <summary>
        ///  Places a file relative to the mod root in accordance with its FileData.
        /// </summary>
        private static void ExportFile(FileData fd, string modRoot)
        {
            Directory.CreateDirectory(modRoot + "\\" + Path.GetDirectoryName(fd.gamePath));
            string newLocation = modRoot + "\\" + fd.gamePath;

            if (fd.fileSource == FileSource.App)
            {
                if (ExportExternalJson(fd, modRoot))
                    return;
                byte[] buffer = null;
                switch (Path.GetFileName(fd.gamePath))
                {
                    case "Delphis_Main.bnk":
                        buffer = gameData.audioData.GetBytes();
                        break;
                    case "AudioSources.txt":
                        buffer = Encoding.UTF8.GetBytes(gameData.audioSourceLog.ToString());
                        break;
                    case "global-metadata.dat":
                        buffer = gameData.globalMetadata.buffer;
                        break;
                    case "Dpr.bin":
                        gameData.dprBin.Save(newLocation);
                        return;

                }
                if (buffer != null)
                {
                    FileStream fs = File.Create(newLocation);
                    fs.Write(buffer, 0, buffer.Length);
                    fs.Close();
                    return;
                }
            }

            if (!fd.IsBundle())
            {
                File.Copy(fd.fileLocation, newLocation);
                return;
            }

            FileStream stream = File.OpenWrite(newLocation);
            AssetsFileWriter afw = new AssetsFileWriter(stream);
            fd.bundle.file.Pack(fd.bundle.file.reader, afw, AssetBundleCompressionType.LZ4);
            afw.Close();
            fd.bundle.file.Close();
            fd.bundle.stream.Dispose();
            if (fd.tempLocation)
                File.Delete(fd.fileLocation);
        }

        /// <summary>
        ///  Places a yaml file relative to the mod root in accordance with its YAMLFileData.
        /// </summary>
        private void ExportYAMLFile(YAMLFileData fd, string modRoot)
        {
            var data = fd.GetLoadedData();

            if (fd.IsLoaded())
            {
                Directory.CreateDirectory(Path.Combine(modRoot, Path.GetDirectoryName(fd.assetPath)));
                string newLocation = Path.Combine(modRoot, fd.assetPath);

                using StreamWriter stream = new StreamWriter(newLocation);

                stream.Write(fd.unityPrefix);
                stream.Write(Serializer.Serialize(data));

                if (fd.tempLocation)
                    File.Delete(fd.fileLocation);
            }
            else if (fd.bundleOrigin == PathEnum.ExternalJSON)
            {
                ExportExternalJson(new FileData() { gamePath = fd.assetPath }, modRoot);
            }
        }

        private static bool ExportExternalJson(FileData fd, string modRoot)
        {
            if (!fd.gamePath.StartsWith(externalJsonGamePath) && !fd.gamePath.StartsWith(externalUnityJsonGamePath)) return false;
            if (!fd.gamePath.EndsWith(".json")) return false;

            string externalJsonPath;
            if (fd.gamePath.StartsWith(externalJsonGamePath))
                externalJsonPath = fd.gamePath[(externalJsonGamePath.Length + 1)..];
            else
                externalJsonPath = fd.gamePath[(externalUnityJsonGamePath.Length + 1)..];

            string fileName = Path.GetFileNameWithoutExtension(externalJsonPath);
            if (externalJsonPath.StartsWith("Encounters\\Starter"))
            {
                new FileInfo(modRoot + "\\" + fd.gamePath).Directory.Create();
                File.WriteAllText(modRoot + "\\" + fd.gamePath, JsonConvert.SerializeObject(gameData.externalStarters.First(t => t.name == fileName).obj, Formatting.Indented));
                return true;
            }
            if (externalJsonPath.StartsWith("Encounters\\HoneyTrees"))
            {
                new FileInfo(modRoot + "\\" + fd.gamePath).Directory.Create();
                File.WriteAllText(modRoot + "\\" + fd.gamePath, JsonConvert.SerializeObject(gameData.externalHoneyTrees.First(t => t.name == fileName).obj, Formatting.Indented));
                return true;
            }
            if (externalJsonPath.StartsWith("MonData\\TMLearnset"))
            {
                string[] segments = fileName.Split('_');
                int dexID = int.Parse(segments[1]);
                int formID = int.Parse(segments[3]);
                new FileInfo(modRoot + "\\" + fd.gamePath).Directory.Create();
                File.WriteAllText(modRoot + "\\" + fd.gamePath, JsonConvert.SerializeObject(gameData.GetPokemon(dexID, formID).externalTMLearnset, Formatting.Indented));
                return true;
            }

            return false;
        }

        public void DeleteTemporaryFiles()
        {
            foreach (FileData fd in fileArchive.Values)
                if (fd.tempLocation && File.Exists(fd.fileLocation))
                    File.Delete(fd.fileLocation);
        }

        /// <summary>
        ///  Merges two bundles by swapping out select files.
        /// </summary>
        private bool Merge(FileData fd, BundleFileInstance bfi2)
        {
            BundleFileInstance bfi1 = fd.bundle;
            AssetsFileInstance afi1 = am.LoadAssetsFileFromBundle(bfi1, bfi1.file.bundleInf6.dirInf[0].name);
            am = new();
            AssetsFileInstance afi2 = am.LoadAssetsFileFromBundle(bfi2, bfi2.file.bundleInf6.dirInf[0].name);
            AssetFileInfoEx[] assetFiles1 = afi1.table.assetFileInfo;
            AssetFileInfoEx[] assetFiles2 = afi2.table.assetFileInfo;

            //Checks if bundles even match
            if (assetFiles1.Length != assetFiles2.Length)
                return false;

            //Collects conflicts
            List<(int, string)> conflicts = new();
            for (int i = 0; i < assetFiles1.Length; i++)
            {
                AssetTypeValueField atvf1 = am.GetTypeInstance(afi1, assetFiles1[i]).GetBaseField();
                AssetTypeValueField atvf2 = am.GetTypeInstance(afi2, assetFiles2[i]).GetBaseField();
                byte[] b1 = atvf1.WriteToByteArray();
                byte[] b2 = atvf2.WriteToByteArray();

                if (!IsMatch(b1, b2))
                    conflicts.Add((i, atvf1.Get("m_Name").GetValue().AsString()));
            }

            //No conflicts, everyone is happy
            if (conflicts.Count == 0)
                return true;

            //Gets what files to overwrite from user
            List<int> overwrites = new();
            FileSelectForm fsf = new(conflicts, overwrites);
            fsf.ShowDialog();

            //Overwrites selected files, and I gotta say, surprisingly complicated.
            List<AssetsReplacer> ars = new();
            for (int i = 0; i < overwrites.Count; i++)
            {
                bfi1 = fd.bundle;
                afi1 = am.LoadAssetsFileFromBundle(bfi1, bfi1.file.bundleInf6.dirInf[0].name);
                AssetTypeValueField atvf = am.GetTypeInstance(afi2, assetFiles2[overwrites[i]]).GetBaseField();
                assetFiles1 = afi1.table.assetFileInfo;

                byte[] b2 = atvf.WriteToByteArray();
                AssetFileInfoEx afie1 = assetFiles1[overwrites[i]];
                AssetsReplacerFromMemory arfm = new AssetsReplacerFromMemory(0, afie1.index, (int)afie1.curFileType, AssetHelper.GetScriptIndex(afi1.file, afie1), b2);
                ars.Add(arfm);
            }

            MakeTempBundle(fd, ars, Path.GetFileName(fd.gamePath));
            return true;
        }

        /// <summary>
        ///  Swaps out the loaded bundle with a modified bundle
        /// </summary>
        private void MakeTempBundle(FileData fd, List<AssetsReplacer> ars, string fileName, string cab = null)
        {
            Directory.CreateDirectory(Environment.CurrentDirectory + "\\" + tempLocationName);
            string fileLocation = Environment.CurrentDirectory + "\\" + tempLocationName + "\\" + fileName + GetFileIndex();
            BundleFileInstance bfi = fd.bundle;
            AssetsFileInstance afi = am.LoadAssetsFileFromBundle(bfi, bfi.file.bundleInf6.dirInf[0].name);
            MemoryStream memoryStream = new MemoryStream();
            AssetsFileWriter afw = new(memoryStream);
            afi.file.dependencies.Write(afw);
            afi.file.Write(afw, 0, ars, 0);
            BundleReplacerFromMemory brfm = new(bfi.file.bundleInf6.dirInf[0].name, cab, true, memoryStream.ToArray(), -1);

            afw = new(File.OpenWrite(fileLocation));
            fd.bundle.file.Write(afw, new List<BundleReplacer> { brfm });
            afw.Close();
            am = new();
            fd.bundle = am.LoadBundleFile(fileLocation, false);
            DecompressBundle(fd.bundle);
            if (fd.tempLocation)
                File.Delete(fd.fileLocation);
            fd.fileLocation = fileLocation;
            fd.tempLocation = true;
        }

        public void MakeTempBundle(string srcGamePath, string dstGamePath, List<AssetsReplacer> ars, string cab = null)
        {
            Directory.CreateDirectory(Environment.CurrentDirectory + "\\" + tempLocationName);
            string fileLocation = Environment.CurrentDirectory  + "\\" + tempLocationName + "\\" + Path.GetFileName(dstGamePath) + GetFileIndex();
            if (!fileArchive.ContainsKey(dstGamePath))
            {
                FileData newFD = new();
                newFD.gamePath = dstGamePath;
                newFD.fileSource = FileSource.App;
                fileArchive[dstGamePath] = newFD;
            }
            FileData srcFD = fileArchive[srcGamePath];
            FileData dstFD = fileArchive[dstGamePath];
            BundleFileInstance bfi = srcFD.bundle;
            AssetsFileInstance afi = am.LoadAssetsFileFromBundle(bfi, bfi.file.bundleInf6.dirInf[0].name);
            MemoryStream memoryStream = new MemoryStream();
            AssetsFileWriter afw = new(memoryStream);
            afi.file.dependencies.Write(afw);
            afi.file.Write(afw, 0, ars, 0);
            BundleReplacerFromMemory brfm = new(bfi.file.bundleInf6.dirInf[0].name, cab, true, memoryStream.ToArray(), -1);

            afw = new(File.OpenWrite(fileLocation));
            bfi.file.Write(afw, new List<BundleReplacer> { brfm });
            afw.Close();
            am = new();
            srcFD.bundle = am.LoadBundleFile(srcFD.fileLocation, false);
            dstFD.bundle = am.LoadBundleFile(fileLocation, false);
            DecompressBundle(srcFD.bundle);
            DecompressBundle(dstFD.bundle);
            if (dstFD.tempLocation)
                File.Delete(dstFD.fileLocation);
            dstFD.fileLocation = fileLocation;
            dstFD.tempLocation = true;
        }

        private static void DecompressBundle(BundleFileInstance bfi)
        {
            AssetBundleFile abf = bfi.file;

            MemoryStream stream = new MemoryStream();
            abf.Unpack(abf.reader, new AssetsFileWriter(stream));

            stream.Position = 0;

            AssetBundleFile newAbf = new AssetBundleFile();
            newAbf.Read(new AssetsFileReader(stream), false);

            abf.reader.Close();
            bfi.file = newAbf;
        }

        /// <summary>
        ///  Checks if two byte arrays are identical.
        /// </summary>
        private static bool IsMatch(byte[] b1, byte[] b2)
        {
            if (b1.Length != b2.Length)
                return false;
            for (int i = 0; i < b1.Length; i++)
                if (b1[i] != b2[i])
                    return false;
            return true;
        }

        /// <summary>
        ///  Gets the path of the AssetAssistant folder given a game directory.
        /// </summary>
        private static string GetAssetAssistantPath(string gameLocation)
        {
            string correctPath = gameLocation + "\\romfs\\Data\\StreamingAssets\\AssetAssistant";
            if (Directory.Exists(correctPath))
                return correctPath;

            //Yuzu outputs a stupid dump with the wrong folder structure.
            string yuzuDumpPath = gameLocation + "\\romfs\\StreamingAssets\\AssetAssistant";
            if (Directory.Exists(yuzuDumpPath))
                return yuzuDumpPath;

            return "";
        }

        /// <summary>
        ///  Gets the path of the Data folder given a game directory.
        /// </summary>
        private static string GetDataPath(string gameLocation)
        {
            string correctPath = gameLocation + "\\romfs\\Data";
            if (Directory.Exists(correctPath))
                return correctPath;

            //Yuzu outputs a stupid dump with the wrong folder structure.
            string yuzuDumpPath = gameLocation + "\\romfs";
            if (Directory.Exists(yuzuDumpPath))
                return yuzuDumpPath;

            return "";
        }

        /// <summary>
        ///  Checks whether the path contains romfs or exefs folders.
        /// </summary>
        private static bool IsGameDirectory(string path, bool needRomfs = false)
        {
            return Directory.Exists(Path.Combine(path, "romfs")) || Directory.Exists(Path.Combine(path, "exefs")) && !needRomfs;
        }

        /// <summary>
        ///  Checks whether the path contains an Assets folder.
        /// </summary>
        private static bool IsUnityDirectory(string path)
        {
            return Directory.Exists(Path.Combine(path, "Assets"));
        }

        /// <summary>
        ///  Finds what path in the PathEnum a yaml file is for.
        /// </summary>
        private static PathEnum? FindPathEnumForYAML(string assetPath)
        {
            string directory = Path.GetDirectoryName(assetPath);
            if (yamlAssetPaths.ContainsKey(directory))
                return yamlAssetPaths[directory];
            else
                return null;
        }

        /// <summary>
        ///  Deserializes a yaml string to a known MonoBehaviour.
        /// </summary>
        private static MonoBehaviour FromYAML(PathEnum pathEnum, string yaml)
        {
            return pathEnum switch
            {
                PathEnum.BattleMasterdatas =>   FromYAML<BattleDataTable>(yaml),
                PathEnum.ContestMasterdatas =>  FromYAML<ContestConfigDatas>(yaml),
                PathEnum.EvScript =>            FromYAML<EvData>(yaml),
                PathEnum.DprMasterdatas =>      FromYAML<MonohiroiTable>(yaml) ??
                                                FromYAML<PokemonInfo>(yaml) ??
                                                FromYAML<ShopTable>(yaml) ??
                                                FromYAML<TowerDoubleStockTable>(yaml) ??
                                                FromYAML<TowerSingleStockTable>(yaml) ??
                                                FromYAML<TowerTrainerTable>(yaml) ??
                                                FromYAML<TrainerTable>(yaml) as MonoBehaviour,
                PathEnum.Gamesettings =>        FromYAML<FieldEncountTable>(yaml),
                PathEnum.English =>             FromYAML<MsbtData>(yaml),
                PathEnum.French =>              FromYAML<MsbtData>(yaml),
                PathEnum.German =>              FromYAML<MsbtData>(yaml),
                PathEnum.Italian =>             FromYAML<MsbtData>(yaml),
                PathEnum.Jpn =>                 FromYAML<MsbtData>(yaml),
                PathEnum.JpnKanji =>            FromYAML<MsbtData>(yaml),
                PathEnum.Korean =>              FromYAML<MsbtData>(yaml),
                PathEnum.SimpChinese =>         FromYAML<MsbtData>(yaml),
                PathEnum.Spanish =>             FromYAML<MsbtData>(yaml),
                PathEnum.TradChinese =>         FromYAML<MsbtData>(yaml),
                PathEnum.PersonalMasterdatas => FromYAML<AddPersonalTable>(yaml) ??
                                                FromYAML<EvolveTable>(yaml) ??
                                                FromYAML<GrowTable>(yaml) ??
                                                FromYAML<ItemTable>(yaml) ??
                                                FromYAML<PersonalTable>(yaml) ??
                                                FromYAML<TamagoWazaTable>(yaml) ??
                                                FromYAML<WazaTable>(yaml) ??
                                                FromYAML<WazaOboeTable>(yaml) as MonoBehaviour,
                PathEnum.UIMasterdatas =>       FromYAML<DistributionTable>(yaml) ??
                                                FromYAML<UIDatabase>(yaml) as MonoBehaviour,
                PathEnum.Ugdata =>              FromYAML<UgEncount>(yaml) ??
                                                FromYAML<UgEncountLevel>(yaml) ??
                                                FromYAML<UgPokemonData>(yaml) ??
                                                FromYAML<UgRandMark>(yaml) ??
                                                FromYAML<UgSpecialPokemon>(yaml) as MonoBehaviour,
                _ => null,
            };
        }

        /// <summary>
        ///  Deserializes a yaml string to the given type.
        /// </summary>
        private static T FromYAML<T>(string yaml) where T : class
        {
            try
            {
                return Serializer.Deserialize<T>(yaml);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(string.Format("Could not deserialize file starting with:\n{0}\n{1}", yaml[..350], ex.Message), "FromYAML<T>");
                return null;
            }
        }

        /// <summary>
        ///  For generating unique file names.
        /// </summary>
        private int GetFileIndex()
        {
            fileIndex++;
            return fileIndex;
        }
    }
}
