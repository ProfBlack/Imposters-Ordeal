using AssetsTools.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ImpostersOrdeal.GlobalData;
using static ImpostersOrdeal.GameDataTypes;
using static ImpostersOrdeal.ExternalJsonStructs;
using static ImpostersOrdeal.Wwise;
using AssetsTools.NET.Extra;
using SmartPoint.AssetAssistant;

namespace ImpostersOrdeal
{
    /// <summary>
    ///  Responsible for converting AssetsTools.NET objects into easier to work with objects and back.
    /// </summary>
    static public class DataParser
    {
        static Dictionary<PathEnum, Task<List<AssetTypeValueField>>> monoBehaviourCollection;
        static Dictionary<PathEnum, Task<List<YamlMonoContainer>>> yamlCollection;

        static AssetTypeTemplateField tagDataTemplate = null;
        static AssetTypeTemplateField attributeValueTemplate = null;
        static AssetTypeTemplateField tagWordTemplate = null;
        static AssetTypeTemplateField wordDataTemplate = null;

        /// <summary>
        ///  Parses all files necessary for analysis and configuration.
        /// </summary>
        public static void PrepareAnalysis()
        {
            LoadMonoBehaviours();
            List<Task> tasks = new()
            {
                Task.Run(() => ParseNatures()),
                Task.Run(() => ParseEvScripts()),
                Task.Run(() => ParseAllMessageFiles()),
                Task.Run(() => ParseGrowthRates()),
                Task.Run(() => ParseItems()),
                Task.Run(() => ParsePickupItems()),
                Task.Run(() => ParseShopTables()),
                Task.Run(() => ParseMoves()),
                Task.Run(() => ParseTMs()),
                Task.Run(() => ParsePokemon()),
                Task.Run(() => ParseEncounterTables()),
                Task.Run(() => ParseTrainers()),
                Task.Run(() => ParseBattleTowerTrainers()),
                Task.Run(() => ParseUgTables()),
                Task.Run(() => ParseAbilities()),
                Task.Run(() => ParseTypings()),
                Task.Run(() => ParseTrainerTypes()),
                Task.Run(() => ParseBattleMasterDatas()),
                Task.Run(() => ParseMasterDatas()),
                Task.Run(() => ParsePersonalMasterDatas()),
                Task.Run(() => ParseUIMasterDatas()),
                Task.Run(() => ParseContestMasterDatas()),
                Task.Run(() => TryParseExternalStarters()),
                Task.Run(() => TryParseExternalHoneyTrees())
            };
            ParseDamagaCategories();
            ParseGlobalMetadata();
            ParseDprBin();
            ParseAudioData();
            TryParseModArgs();
            Task.WaitAll(tasks.ToArray());
            //Hot damn! 4GB?? This has got to go.
            monoBehaviourCollection = null;
            GC.Collect();
        }

        /// <summary>
        ///  Parses all yaml files necessary for analysis and configuration.
        /// </summary>
        public static void PrepareYAMLAnalysis()
        {
            LoadYAMLs();
            List<Task> tasks = new()
            {
                Task.Run(() => ParseNaturesYAML()),
                Task.Run(() => ParseEvScriptsYAML()),
                Task.Run(() => ParseAllMessageFilesYAML()),
                Task.Run(() => ParseGrowthRatesYAML()),
                Task.Run(() => ParseItemsYAML()),
                Task.Run(() => ParsePickupItemsYAML()),
                Task.Run(() => ParseShopTablesYAML()),
                Task.Run(() => ParseMovesYAML()),
                Task.Run(() => ParseTMsYAML()),
                Task.Run(() => ParsePokemonYAML()),
                Task.Run(() => ParseEncounterTablesYAML()),
                Task.Run(() => ParseTrainersYAML()),
                Task.Run(() => ParseBattleTowerTrainersYAML()),
                Task.Run(() => ParseUgTablesYAML()),
                Task.Run(() => ParseAbilitiesYAML()),
                Task.Run(() => ParseTypingsYAML()),
                Task.Run(() => ParseTrainerTypesYAML()),
                Task.Run(() => ParseBattleMasterDatasYAML()),
                Task.Run(() => ParseMasterDatasYAML()),
                Task.Run(() => ParsePersonalMasterDatasYAML()),
                Task.Run(() => ParseUIMasterDatasYAML()),
                Task.Run(() => ParseContestMasterDatasYAML()),
                Task.Run(() => TryParseExternalStarters(true)),
                Task.Run(() => TryParseExternalHoneyTrees(true)),
            };
            ParseDamagaCategories();
            ParseGlobalMetadata();
            ParseDprBin();
            ParseAudioData();
            TryParseModArgs();
            Task.WaitAll(tasks.ToArray());
            GC.Collect();
        }

        private static void TryParseModArgs()
        {
            gameData.modArgs = fileManager.TryGetModArgs();
        }

        private static void TryParseExternalHoneyTrees(bool unityPath = false)
        {
            gameData.externalHoneyTrees = null;
            List<(string name, HoneyTreeZone obj)> files = fileManager.TryGetExternalJsons<HoneyTreeZone>($"Encounters\\HoneyTrees", unityPath);
            if (files.Count == 0) return;
            gameData.externalHoneyTrees = files;
        }

        private static void TryParseExternalStarters(bool unityPath = false)
        {
            gameData.externalStarters = null;
            List<(string name, Starter obj)> files = fileManager.TryGetExternalJsons<Starter>($"Encounters\\Starter", unityPath);
            if (files.Count == 0) return;
            gameData.externalStarters = files;
        }

        private static async Task ParseContestMasterDatas()
        {
            gameData.contestResultMotion = new();
            AssetTypeValueField monoBehaviour = (await monoBehaviourCollection[PathEnum.ContestMasterdatas]).Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "ContestConfigDatas");

            AssetTypeValueField[] resultMotionSheet = monoBehaviour["ResultMotion"].children[0].children;
            for (int i = 0; i < resultMotionSheet.Length; i++)
            {
                ResultMotion rm = new()
                {
                    validFlag = resultMotionSheet[i]["valid_flag"].GetValue().value.asUInt8,
                    id = resultMotionSheet[i]["id"].GetValue().value.asUInt16,
                    monsNo = resultMotionSheet[i]["monsNo"].GetValue().AsInt(),
                    winAnim = resultMotionSheet[i]["winAnim"].GetValue().AsUInt(),
                    loseAnim = resultMotionSheet[i]["loseAnim"].GetValue().AsUInt(),
                    waitAnim = resultMotionSheet[i]["waitAnim"].GetValue().AsUInt(),
                    duration = resultMotionSheet[i]["duration"].GetValue().AsFloat()
                };
                gameData.contestResultMotion.Add(rm);
            }
        }

        private static async Task ParseContestMasterDatasYAML()
        {
            gameData.contestResultMotion = new();

            var monoBehaviour = (await yamlCollection[PathEnum.ContestMasterdatas]).Find(y => y.MonoBehaviour?.Name == "ContestConfigDatas").MonoBehaviour as ContestConfigDatas;
            var resultMotionSheet = monoBehaviour.ResultMotion;
            for (int i = 0; i < resultMotionSheet.Length; i++)
            {
                ResultMotion rm = new()
                {
                    validFlag = resultMotionSheet[i].ValidFlag,
                    id = resultMotionSheet[i].ID,
                    monsNo = resultMotionSheet[i].MonsNo,
                    winAnim = resultMotionSheet[i].WinAnim,
                    loseAnim = resultMotionSheet[i].LoseAnim,
                    waitAnim = resultMotionSheet[i].WaitAnim,
                    duration = resultMotionSheet[i].Duration,
                };

                gameData.contestResultMotion.Add(rm);
            }
        }

        /// <summary>
        ///  Loads all monobehaviours asyncronously into monoBehaviourCollection.
        /// </summary>
        private static void LoadMonoBehaviours()
        {
            monoBehaviourCollection = new();
            foreach (PathEnum pe in Enum.GetValues(typeof(PathEnum)))
                if (pe != PathEnum.ExternalJSON)
                    monoBehaviourCollection[pe] = Task.Run(() => fileManager.GetMonoBehaviours(pe));
        }

        /// <summary>
        ///  Loads all yaml files asyncronously into yamlCollection.
        /// </summary>
        private static void LoadYAMLs()
        {
            yamlCollection = new();
            foreach (PathEnum pe in Enum.GetValues(typeof(PathEnum)))
                if (pe != PathEnum.ExternalJSON)
                    yamlCollection[pe] = Task.Run(() => fileManager.GetYAMLs(pe));
        }

        /// <summary>
        ///  Gets the appropriate path for a specific language.
        /// </summary>
        private static PathEnum GetMessageBundlePathForLanguage(Language language, bool isKanji = false)
        {
            return language switch
            {
                Language.Japanese => isKanji ? PathEnum.JpnKanji : PathEnum.Jpn,
                Language.English => PathEnum.English,
                Language.French => PathEnum.French,
                Language.Italian => PathEnum.Italian,
                Language.German => PathEnum.German,
                Language.Spanish => PathEnum.Spanish,
                Language.Korean => PathEnum.Korean,
                Language.SimpChinese => PathEnum.SimpChinese,
                Language.TradChinese => PathEnum.TradChinese,
                _ => PathEnum.CommonMsbt,
            };
        }

        /// <summary>
        ///  Gets the appropriate message file prefix for a specific language.
        /// </summary>
        private static string GetMessageFilePrefixForLanguage(Language language, bool isKanji = false)
        {
            return language switch
            {
                Language.Japanese => isKanji ? "jpn_kanji" : "jpn",
                Language.English => "english",
                Language.French => "french",
                Language.Italian => "italian",
                Language.German => "german",
                Language.Spanish => "spanish",
                Language.Korean => "korean",
                Language.SimpChinese => "simp_chinese",
                Language.TradChinese => "trad_chinese",
                _ => "",
            };
        }

        /// <summary>
        ///  Formats the message file name to have the proper language prefix.
        /// </summary>
        private static string FormatMessageFileNameForLanguage(string fileName, Language language, bool isKanji = false)
        {
            return string.Join("_", GetMessageFilePrefixForLanguage(language, isKanji), fileName);
        }

        /// <summary>
        ///  Gets all the labels of a message file in a specific language.
        /// </summary>
        private static async Task<AssetTypeValueField[]> FindLabelArrayOfMessageFileAsync(string fileName, Language language, bool isKanji = false)
        {
            string fullFileName = FormatMessageFileNameForLanguage(fileName, language, isKanji);
            PathEnum pathForLanguage = GetMessageBundlePathForLanguage(language, isKanji);

            var baseField = (await monoBehaviourCollection[PathEnum.CommonMsbt]).Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == fullFileName) ??
                            (await monoBehaviourCollection[pathForLanguage]).Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == fullFileName);
            return baseField?.children[8].children[0].children ?? Array.Empty<AssetTypeValueField>();
        }

        /// <summary>
        ///  Gets all the labels of a message file in a specific language.
        /// </summary>
        private static AssetTypeValueField[] FindLabelArrayOfMessageFile(string fileName, Language language, bool isKanji = false)
        {
            string fullFileName = FormatMessageFileNameForLanguage(fileName, language, isKanji);
            PathEnum pathForLanguage = GetMessageBundlePathForLanguage(language, isKanji);
            
            var baseField = fileManager.GetMonoBehaviours(PathEnum.CommonMsbt).Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == fullFileName) ??
                            fileManager.GetMonoBehaviours(pathForLanguage).Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == fullFileName);
            return baseField?.children[8].children[0].children ?? Array.Empty<AssetTypeValueField>();
        }

        /// <summary>
        ///  Gets all the labels of a yaml message file in a specific language.
        /// </summary>
        private static async Task<LabelData[]> FindLabelArrayOfYAMLMessageFileAsync(string fileName, Language language, bool isKanji = false)
        {
            string fullFileName = FormatMessageFileNameForLanguage(fileName, language, isKanji);
            PathEnum pathForLanguage = GetMessageBundlePathForLanguage(language, isKanji);

            var mono = (await yamlCollection[pathForLanguage]).Find(y => y.MonoBehaviour?.Name == fullFileName);
            return (mono.MonoBehaviour as MsbtData).LabelDataArray ?? Array.Empty<LabelData>();
        }

        /// <summary>
        ///  Gets all the labels of a yaml message file in a specific language.
        /// </summary>
        private static LabelData[] FindLabelArrayOfYAMLMessageFile(string fileName, Language language, bool isKanji = false)
        {
            string fullFileName = FormatMessageFileNameForLanguage(fileName, language, isKanji);
            PathEnum pathForLanguage = GetMessageBundlePathForLanguage(language, isKanji);
            
            var mono = fileManager.GetYAMLs(pathForLanguage).Find(y => y.MonoBehaviour?.Name == fullFileName);
            return (mono.MonoBehaviour as MsbtData).LabelDataArray ?? Array.Empty<LabelData>();
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed TrainerTypes.
        /// </summary>
        private static async Task ParseTrainerTypes()
        {
            gameData.trainerTypes = new();
            AssetTypeValueField monoBehaviour = (await monoBehaviourCollection[PathEnum.DprMasterdatas]).Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "TrainerTable");

            AssetTypeValueField[] nameFields = await FindLabelArrayOfMessageFileAsync("dp_trainers_type", Language.English);
            Dictionary<string, string> trainerTypeNames = new();
            foreach (AssetTypeValueField label in nameFields)
                if (label.children[6].children[0].childrenCount > 0)
                    trainerTypeNames[label.children[2].GetValue().AsString()] = label.children[6].children[0].children[0].children[4].GetValue().AsString();

            AssetTypeValueField[] trainerTypeFields = monoBehaviour.children[4].children[0].children;
            for (int trainerTypeIdx = 0; trainerTypeIdx < trainerTypeFields.Length; trainerTypeIdx++)
            {
                if (trainerTypeFields[trainerTypeIdx].children[0].GetValue().AsInt() == -1)
                    continue;

                TrainerType trainerType = new();
                trainerType.trainerTypeID = trainerTypeIdx;
                trainerType.label = trainerTypeFields[trainerTypeIdx].children[1].GetValue().AsString();

                trainerType.name = !trainerTypeNames.ContainsKey(trainerType.label) ? "" : trainerTypeNames[trainerType.label];

                gameData.trainerTypes.Add(trainerType);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed TrainerTypes.
        /// </summary>
        private static async Task ParseTrainerTypesYAML()
        {
            gameData.trainerTypes = new();
            var monoBehaviour = (await yamlCollection[PathEnum.DprMasterdatas]).Find(y => y.MonoBehaviour?.Name == "TrainerTable").MonoBehaviour as TrainerTable;

            var nameFields = await FindLabelArrayOfYAMLMessageFileAsync("dp_trainers_type", Language.English);
            Dictionary<string, string> trainerTypeNames = new();
            foreach (var label in nameFields)
                if (label.WordDataArray.Length > 0)
                    trainerTypeNames[label.LabelName] = label.WordDataArray[0].Str;

            var trainerTypeFields = monoBehaviour.TrainerType;
            for (int trainerTypeIdx = 0; trainerTypeIdx < trainerTypeFields.Length; trainerTypeIdx++)
            {
                if (trainerTypeFields[trainerTypeIdx].TrainerID == -1)
                    continue;

                TrainerType trainerType = new();
                trainerType.trainerTypeID = trainerTypeIdx;
                trainerType.label = trainerTypeFields[trainerTypeIdx].LabelTrType;
                trainerType.name = !trainerTypeNames.ContainsKey(trainerType.label) ? string.Empty : trainerTypeNames[trainerType.label];

                gameData.trainerTypes.Add(trainerType);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed Natures.
        /// </summary>
        private static async Task ParseNatures()
        {
            gameData.natures = new();
            AssetTypeValueField[] natureFields = await FindLabelArrayOfMessageFileAsync("ss_seikaku", Language.English);

            for (int natureID = 0; natureID < natureFields.Length; natureID++)
            {
                Nature nature = new();
                nature.natureID = natureID;
                nature.name = Encoding.UTF8.GetString(natureFields[natureID].children[6].children[0].children[0].children[4].value.value.asString);

                gameData.natures.Add(nature);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed yaml Natures.
        /// </summary>
        private static async Task ParseNaturesYAML()
        {
            gameData.natures = new();
            var labels = await FindLabelArrayOfYAMLMessageFileAsync("ss_seikaku", Language.English);

            for (int natureID = 0; natureID < labels.Length; natureID++)
            {
                Nature nature = new()
                {
                    natureID = natureID,
                    name = labels[natureID].WordDataArray[0].Str
                };

                gameData.natures.Add(nature);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed DamageCategorie.
        /// </summary>
        private static void ParseDamagaCategories()
        {
            gameData.damageCategories = new();
            for (int i = 0; i < 3; i++)
            {
                DamageCategory damageCategory = new();
                damageCategory.damageCategoryID = i;
                gameData.damageCategories.Add(damageCategory);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed Typings.
        /// </summary>
        private static async Task ParseTypings()
        {
            gameData.typings = new();
            AssetTypeValueField[] typingFields = await FindLabelArrayOfMessageFileAsync("ss_typename", Language.English);

            for (int typingID = 0; typingID < typingFields.Length; typingID++)
            {
                Typing typing = new();
                typing.typingID = typingID;
                typing.name = Encoding.UTF8.GetString(typingFields[typingID].children[6].children[0].children[0].children[4].value.value.asString);

                gameData.typings.Add(typing);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed yaml Typings.
        /// </summary>
        private static async Task ParseTypingsYAML()
        {
            gameData.typings = new();
            var typingFields = await FindLabelArrayOfYAMLMessageFileAsync ("ss_typename", Language.English);

            for (int typingID = 0; typingID < typingFields.Length; typingID++)
            {
                Typing typing = new();
                typing.typingID = typingID;
                typing.name = typingFields[typingID].WordDataArray[0].Str;

                gameData.typings.Add(typing);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed Abilities.
        /// </summary>
        private static async Task ParseAbilities()
        {
            gameData.abilities = new();
            AssetTypeValueField[] abilityFields = await FindLabelArrayOfMessageFileAsync("ss_tokusei", Language.English);

            for (int abilityID = 0; abilityID < abilityFields.Length; abilityID++)
            {
                Ability ability = new();
                ability.abilityID = abilityID;
                ability.name = Encoding.UTF8.GetString(abilityFields[abilityID].children[6].children[0].children[0].children[4].value.value.asString);

                gameData.abilities.Add(ability);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed yaml Abilities.
        /// </summary>
        private static async Task ParseAbilitiesYAML()
        {
            gameData.abilities = new();
            var abilityFields = await FindLabelArrayOfYAMLMessageFileAsync("ss_tokusei", Language.English);

            for (int abilityID = 0; abilityID < abilityFields.Length; abilityID++)
            {
                Ability ability = new();
                ability.abilityID = abilityID;
                ability.name = abilityFields[abilityID].WordDataArray[0].Str;

                gameData.abilities.Add(ability);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed underground data.
        /// </summary>
        private static async Task ParseUgTables()
        {
            gameData.ugAreas = new();
            gameData.ugEncounterFiles = new();
            gameData.ugEncounterLevelSets = new();
            gameData.ugSpecialEncounters = new();
            gameData.ugPokemonData = new();
            List<AssetTypeValueField> monoBehaviours = await monoBehaviourCollection[PathEnum.Ugdata];

            AssetTypeValueField[] ugAreaFields = monoBehaviours.Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "UgRandMark").children[4].children[0].children;
            for (int ugAreaIdx = 0; ugAreaIdx < ugAreaFields.Length; ugAreaIdx++)
            {
                UgArea ugArea = new();
                ugArea.id = ugAreaFields[ugAreaIdx]["id"].GetValue().AsInt();
                ugArea.fileName = ugAreaFields[ugAreaIdx]["FileName"].GetValue().AsString();

                gameData.ugAreas.Add(ugArea);
            }

            List<AssetTypeValueField> ugEncounterFiles = monoBehaviours.Where(m => Encoding.Default.GetString(m.children[3].value.value.asString).StartsWith("UgEncount_")).ToList();
            for (int ugEncounterFileIdx = 0; ugEncounterFileIdx < ugEncounterFiles.Count; ugEncounterFileIdx++)
            {
                UgEncounterFile ugEncounterFile = new();
                ugEncounterFile.mName = Encoding.Default.GetString(ugEncounterFiles[ugEncounterFileIdx].children[3].value.value.asString);

                ugEncounterFile.ugEncounters = new();
                AssetTypeValueField[] ugMonFields = ugEncounterFiles[ugEncounterFileIdx].children[4].children[0].children;
                for (int ugMonIdx = 0; ugMonIdx < ugMonFields.Length; ugMonIdx++)
                {
                    UgEncounter ugEncounter = new();
                    ugEncounter.dexID = ugMonFields[ugMonIdx].children[0].value.value.asInt32;
                    ugEncounter.version = ugMonFields[ugMonIdx].children[1].value.value.asInt32;
                    ugEncounter.zukanFlag = ugMonFields[ugMonIdx].children[2].value.value.asInt32;

                    ugEncounterFile.ugEncounters.Add(ugEncounter);
                }

                gameData.ugEncounterFiles.Add(ugEncounterFile);
            }

            AssetTypeValueField[] ugEncounterLevelFields = monoBehaviours.Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "UgEncountLevel").children[4].children[0].children;
            for (int ugEncouterLevelIdx = 0; ugEncouterLevelIdx < ugEncounterLevelFields.Length; ugEncouterLevelIdx++)
            {
                UgEncounterLevelSet ugLevels = new();
                ugLevels.minLv = ugEncounterLevelFields[ugEncouterLevelIdx].children[0].value.value.asInt32;
                ugLevels.maxLv = ugEncounterLevelFields[ugEncouterLevelIdx].children[1].value.value.asInt32;

                gameData.ugEncounterLevelSets.Add(ugLevels);
            }

            AssetTypeValueField[] ugSpecialEncounterFields = monoBehaviours.Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "UgSpecialPokemon").children[4].children[0].children;
            for (int ugSpecialEncounterIdx = 0; ugSpecialEncounterIdx < ugSpecialEncounterFields.Length; ugSpecialEncounterIdx++)
            {
                UgSpecialEncounter ugSpecialEncounter = new();
                ugSpecialEncounter.id = ugSpecialEncounterFields[ugSpecialEncounterIdx]["id"].GetValue().AsInt();
                ugSpecialEncounter.dexID = ugSpecialEncounterFields[ugSpecialEncounterIdx]["monsno"].GetValue().AsInt();
                ugSpecialEncounter.version = ugSpecialEncounterFields[ugSpecialEncounterIdx]["version"].GetValue().AsInt();
                ugSpecialEncounter.dRate = ugSpecialEncounterFields[ugSpecialEncounterIdx]["Dspecialrate"].GetValue().AsInt();
                ugSpecialEncounter.pRate = ugSpecialEncounterFields[ugSpecialEncounterIdx]["Pspecialrate"].GetValue().AsInt();

                gameData.ugSpecialEncounters.Add(ugSpecialEncounter);
            }

            AssetTypeValueField[] ugPokemonDataFields = monoBehaviours.Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "UgPokemonData").children[4].children[0].children;
            for (int ugPokemonDataIdx = 0; ugPokemonDataIdx < ugPokemonDataFields.Length; ugPokemonDataIdx++)
            {
                GameDataTypes.UgPokemonData ugPokemonData = new();
                ugPokemonData.monsno = ugPokemonDataFields[ugPokemonDataIdx]["monsno"].GetValue().AsInt();
                ugPokemonData.type1ID = ugPokemonDataFields[ugPokemonDataIdx]["type1ID"].GetValue().AsInt();
                ugPokemonData.type2ID = ugPokemonDataFields[ugPokemonDataIdx]["type2ID"].GetValue().AsInt();
                ugPokemonData.size = ugPokemonDataFields[ugPokemonDataIdx]["size"].GetValue().AsInt();
                ugPokemonData.movetype = ugPokemonDataFields[ugPokemonDataIdx]["movetype"].GetValue().AsInt();
                ugPokemonData.reactioncode = new int[2];
                for (int i = 0; i < ugPokemonData.reactioncode.Length; i++)
                    ugPokemonData.reactioncode[i] = ugPokemonDataFields[ugPokemonDataIdx]["reactioncode"][0][i].GetValue().AsInt();
                ugPokemonData.moveRate = new int[2];
                for (int i = 0; i < ugPokemonData.moveRate.Length; i++)
                    ugPokemonData.moveRate[i] = ugPokemonDataFields[ugPokemonDataIdx]["move_rate"][0][i].GetValue().AsInt();
                ugPokemonData.submoveRate = new int[5];
                for (int i = 0; i < ugPokemonData.submoveRate.Length; i++)
                    ugPokemonData.submoveRate[i] = ugPokemonDataFields[ugPokemonDataIdx]["submove_rate"][0][i].GetValue().AsInt();
                ugPokemonData.reaction = new int[5];
                for (int i = 0; i < ugPokemonData.reaction.Length; i++)
                    ugPokemonData.reaction[i] = ugPokemonDataFields[ugPokemonDataIdx]["reaction"][0][i].GetValue().AsInt();
                ugPokemonData.flagrate = new int[6];
                for (int i = 0; i < ugPokemonData.flagrate.Length; i++)
                    ugPokemonData.flagrate[i] = ugPokemonDataFields[ugPokemonDataIdx]["flagrate"][0][i].GetValue().AsInt();
                ugPokemonData.rateup = ugPokemonDataFields[ugPokemonDataIdx]["rateup"].GetValue().AsInt();

                gameData.ugPokemonData.Add(ugPokemonData);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed yaml underground data.
        /// </summary>
        private static async Task ParseUgTablesYAML()
        {
            gameData.ugAreas = new();
            gameData.ugEncounterFiles = new();
            gameData.ugEncounterLevelSets = new();
            gameData.ugSpecialEncounters = new();
            gameData.ugPokemonData = new();
            var monoBehaviours = await yamlCollection[PathEnum.Ugdata];

            var ugAreaFields = (monoBehaviours.Find(y => y.MonoBehaviour?.Name == "UgRandMark").MonoBehaviour as UgRandMark).Table;
            for (int ugAreaIdx = 0; ugAreaIdx < ugAreaFields.Length; ugAreaIdx++)
            {
                UgArea ugArea = new()
                {
                    id = ugAreaFields[ugAreaIdx].ID,
                    fileName = ugAreaFields[ugAreaIdx].FileName,
                };

                gameData.ugAreas.Add(ugArea);
            }

            var ugEncounterFiles = monoBehaviours.Select(y => y.MonoBehaviour).OfType<UgEncount>().ToList();
            for (int ugEncounterFileIdx = 0; ugEncounterFileIdx < ugEncounterFiles.Count; ugEncounterFileIdx++)
            {
                UgEncounterFile ugEncounterFile = new();
                ugEncounterFile.mName = ugEncounterFiles[ugEncounterFileIdx].Name;

                ugEncounterFile.ugEncounters = new();
                var ugMonFields = ugEncounterFiles[ugEncounterFileIdx].Table;
                for (int ugMonIdx = 0; ugMonIdx < ugMonFields.Length; ugMonIdx++)
                {
                    UgEncounter ugEncounter = new()
                    {
                        dexID = ugMonFields[ugMonIdx].Monsno,
                        version = ugMonFields[ugMonIdx].Version,
                        zukanFlag = ugMonFields[ugMonIdx].ZukanFlag,
                    };

                    ugEncounterFile.ugEncounters.Add(ugEncounter);
                }

                gameData.ugEncounterFiles.Add(ugEncounterFile);
            }

            var ugEncounterLevelFields = (monoBehaviours.Find(y => y.MonoBehaviour?.Name == "UgEncountLevel").MonoBehaviour as UgEncountLevel).Data;
            for (int ugEncouterLevelIdx = 0; ugEncouterLevelIdx < ugEncounterLevelFields.Length; ugEncouterLevelIdx++)
            {
                UgEncounterLevelSet ugLevels = new()
                {
                    minLv = ugEncounterLevelFields[ugEncouterLevelIdx].MinLv,
                    maxLv = ugEncounterLevelFields[ugEncouterLevelIdx].MaxLv,
                };

                gameData.ugEncounterLevelSets.Add(ugLevels);
            }

            var ugSpecialEncounterFields = (monoBehaviours.Find(y => y.MonoBehaviour?.Name == "UgSpecialPokemon").MonoBehaviour as UgSpecialPokemon).Sheet1;
            for (int ugSpecialEncounterIdx = 0; ugSpecialEncounterIdx < ugSpecialEncounterFields.Length; ugSpecialEncounterIdx++)
            {
                UgSpecialEncounter ugSpecialEncounter = new()
                {
                    id = ugSpecialEncounterFields[ugSpecialEncounterIdx].ID,
                    dexID = ugSpecialEncounterFields[ugSpecialEncounterIdx].Monsno,
                    version = ugSpecialEncounterFields[ugSpecialEncounterIdx].Version,
                    dRate = ugSpecialEncounterFields[ugSpecialEncounterIdx].DSpecialRate,
                    pRate = ugSpecialEncounterFields[ugSpecialEncounterIdx].PSpecialRate,
                };

                gameData.ugSpecialEncounters.Add(ugSpecialEncounter);
            }

            var ugPokemonDataFields = (monoBehaviours.Find(y => y.MonoBehaviour?.Name == "UgPokemonData").MonoBehaviour as UgPokemonData).Table;
            for (int ugPokemonDataIdx = 0; ugPokemonDataIdx < ugPokemonDataFields.Length; ugPokemonDataIdx++)
            {
                GameDataTypes.UgPokemonData ugPokemonData = new();
                ugPokemonData.monsno = ugPokemonDataFields[ugPokemonDataIdx].Monsno;
                ugPokemonData.type1ID = ugPokemonDataFields[ugPokemonDataIdx].Type1ID;
                ugPokemonData.type2ID = ugPokemonDataFields[ugPokemonDataIdx].Type2ID;
                ugPokemonData.size = ugPokemonDataFields[ugPokemonDataIdx].Size;
                ugPokemonData.movetype = ugPokemonDataFields[ugPokemonDataIdx].MoveType;

                ugPokemonData.reactioncode = new int[2];
                for (int i = 0; i < ugPokemonData.reactioncode.Length; i++)
                    ugPokemonData.reactioncode[i] = ugPokemonDataFields[ugPokemonDataIdx].Reaction[i];

                ugPokemonData.moveRate = new int[2];
                for (int i = 0; i < ugPokemonData.moveRate.Length; i++)
                    ugPokemonData.moveRate[i] = ugPokemonDataFields[ugPokemonDataIdx].MoveRate[i];

                ugPokemonData.submoveRate = new int[5];
                for (int i = 0; i < ugPokemonData.submoveRate.Length; i++)
                    ugPokemonData.submoveRate[i] = ugPokemonDataFields[ugPokemonDataIdx].SubmoveRate[i];

                ugPokemonData.reaction = new int[5];
                for (int i = 0; i < ugPokemonData.reaction.Length; i++)
                    ugPokemonData.reaction[i] = ugPokemonDataFields[ugPokemonDataIdx].Reaction[i];

                ugPokemonData.flagrate = new int[6];
                for (int i = 0; i < ugPokemonData.flagrate.Length; i++)
                    ugPokemonData.flagrate[i] = ugPokemonDataFields[ugPokemonDataIdx].FlagRate[i];

                ugPokemonData.rateup = ugPokemonDataFields[ugPokemonDataIdx].RateUp;

                gameData.ugPokemonData.Add(ugPokemonData);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with a parsed Trainer table.
        /// </summary>
        private static async Task ParseTrainers()
        {
            gameData.trainers = new();
            AssetTypeValueField monoBehaviour = (await monoBehaviourCollection[PathEnum.DprMasterdatas]).Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "TrainerTable");

            AssetTypeValueField[] nameFields = await FindLabelArrayOfMessageFileAsync("dp_trainers_name", Language.English);
            Dictionary<string, string> trainerNames = new();
            gameData.trainerNames = trainerNames;
            foreach (AssetTypeValueField label in nameFields)
                if (label.children[6].children[0].childrenCount > 0)
                    trainerNames[label.children[2].GetValue().AsString()] = label.children[6].children[0].children[0].children[4].GetValue().AsString();

            AssetTypeValueField[] trainerFields = monoBehaviour.children[5].children[0].children;
            AssetTypeValueField[] trainerPokemonFields = monoBehaviour.children[6].children[0].children;
            for (int trainerIdx = 0; trainerIdx < Math.Min(trainerFields.Length, trainerPokemonFields.Length); trainerIdx++)
            {
                Trainer trainer = new();
                trainer.trainerTypeID = trainerFields[trainerIdx].children[0].value.value.asInt32;
                trainer.colorID = trainerFields[trainerIdx].children[1].value.value.asUInt8;
                trainer.fightType = trainerFields[trainerIdx].children[2].value.value.asUInt8;
                trainer.arenaID = trainerFields[trainerIdx].children[3].value.value.asInt32;
                trainer.effectID = trainerFields[trainerIdx].children[4].value.value.asInt32;
                trainer.gold = trainerFields[trainerIdx].children[5].value.value.asUInt8;
                trainer.useItem1 = trainerFields[trainerIdx].children[6].value.value.asUInt16;
                trainer.useItem2 = trainerFields[trainerIdx].children[7].value.value.asUInt16;
                trainer.useItem3 = trainerFields[trainerIdx].children[8].value.value.asUInt16;
                trainer.useItem4 = trainerFields[trainerIdx].children[9].value.value.asUInt16;
                trainer.hpRecoverFlag = trainerFields[trainerIdx].children[10].value.value.asUInt8;
                trainer.giftItem = trainerFields[trainerIdx].children[11].value.value.asUInt16;
                trainer.nameLabel = trainerFields[trainerIdx].children[12].GetValue().AsString();
                trainer.aiBit = trainerFields[trainerIdx].children[19].value.value.asUInt32;
                trainer.trainerID = trainerIdx;
                trainer.name = trainerNames[trainer.nameLabel];

                //Parse trainer pokemon
                trainer.trainerPokemon = new();
                AssetTypeValueField[] pokemonFields = trainerPokemonFields[trainerIdx].children;
                for (int pokemonIdx = 1; pokemonIdx < pokemonFields.Length && pokemonFields[pokemonIdx].value.value.asUInt16 != 0; pokemonIdx += 26)
                {
                    TrainerPokemon pokemon = new();
                    pokemon.dexID = pokemonFields[pokemonIdx + 0].value.value.asUInt16;
                    pokemon.formID = pokemonFields[pokemonIdx + 1].value.value.asUInt16;
                    pokemon.isRare = pokemonFields[pokemonIdx + 2].value.value.asUInt8;
                    pokemon.level = pokemonFields[pokemonIdx + 3].value.value.asUInt8;
                    pokemon.sex = pokemonFields[pokemonIdx + 4].value.value.asUInt8;
                    pokemon.natureID = pokemonFields[pokemonIdx + 5].value.value.asUInt8;
                    pokemon.abilityID = pokemonFields[pokemonIdx + 6].value.value.asUInt16;
                    pokemon.moveID1 = pokemonFields[pokemonIdx + 7].value.value.asUInt16;
                    pokemon.moveID2 = pokemonFields[pokemonIdx + 8].value.value.asUInt16;
                    pokemon.moveID3 = pokemonFields[pokemonIdx + 9].value.value.asUInt16;
                    pokemon.moveID4 = pokemonFields[pokemonIdx + 10].value.value.asUInt16;
                    pokemon.itemID = pokemonFields[pokemonIdx + 11].value.value.asUInt16;
                    pokemon.ballID = pokemonFields[pokemonIdx + 12].value.value.asUInt8;
                    pokemon.seal = pokemonFields[pokemonIdx + 13].value.value.asInt32;
                    pokemon.hpIV = pokemonFields[pokemonIdx + 14].value.value.asUInt8;
                    pokemon.atkIV = pokemonFields[pokemonIdx + 15].value.value.asUInt8;
                    pokemon.defIV = pokemonFields[pokemonIdx + 16].value.value.asUInt8;
                    pokemon.spAtkIV = pokemonFields[pokemonIdx + 17].value.value.asUInt8;
                    pokemon.spDefIV = pokemonFields[pokemonIdx + 18].value.value.asUInt8;
                    pokemon.spdIV = pokemonFields[pokemonIdx + 19].value.value.asUInt8;
                    pokemon.hpEV = pokemonFields[pokemonIdx + 20].value.value.asUInt8;
                    pokemon.atkEV = pokemonFields[pokemonIdx + 21].value.value.asUInt8;
                    pokemon.defEV = pokemonFields[pokemonIdx + 22].value.value.asUInt8;
                    pokemon.spAtkEV = pokemonFields[pokemonIdx + 23].value.value.asUInt8;
                    pokemon.spDefEV = pokemonFields[pokemonIdx + 24].value.value.asUInt8;
                    pokemon.spdEV = pokemonFields[pokemonIdx + 25].value.value.asUInt8;

                    trainer.trainerPokemon.Add(pokemon);
                }

                gameData.trainers.Add(trainer);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with a parsed yaml Trainer table.
        /// </summary>
        private static async Task ParseTrainersYAML()
        {
            gameData.trainers = new();
            var monoBehaviour = (await yamlCollection[PathEnum.DprMasterdatas]).Find(y => y.MonoBehaviour?.Name == "TrainerTable").MonoBehaviour as TrainerTable;

            var nameFields = await FindLabelArrayOfYAMLMessageFileAsync("dp_trainers_name", Language.English);
            Dictionary<string, string> trainerNames = new();
            gameData.trainerNames = trainerNames;
            foreach (var label in nameFields)
                if (label.WordDataArray.Length > 0)
                    trainerNames[label.LabelName] = label.WordDataArray[0].Str;

            var trainerFields = monoBehaviour.TrainerData;
            var trainerPokemonFields = monoBehaviour.TrainerPoke;
            for (int trainerIdx = 0; trainerIdx < Math.Min(trainerFields.Length, trainerPokemonFields.Length); trainerIdx++)
            {
                Trainer trainer = new()
                {
                    trainerTypeID = trainerFields[trainerIdx].TypeID,
                    colorID = trainerFields[trainerIdx].ColorID,
                    fightType = trainerFields[trainerIdx].FightType,
                    arenaID = trainerFields[trainerIdx].ArenaID,
                    effectID = trainerFields[trainerIdx].EffectID,
                    gold = trainerFields[trainerIdx].Gold,
                    useItem1 = trainerFields[trainerIdx].UseItem1,
                    useItem2 = trainerFields[trainerIdx].UseItem2,
                    useItem3 = trainerFields[trainerIdx].UseItem3,
                    useItem4 = trainerFields[trainerIdx].UseItem4,
                    hpRecoverFlag = trainerFields[trainerIdx].HPRecoverFlag,
                    giftItem = trainerFields[trainerIdx].GiftItem,
                    nameLabel = trainerFields[trainerIdx].NameLabel,
                    aiBit = trainerFields[trainerIdx].AIBit,
                    trainerID = trainerIdx
                };

                trainer.name = trainerNames[trainer.nameLabel];

                //Parse trainer pokemon
                trainer.trainerPokemon = new();
                var pokemonFields = trainerPokemonFields[trainerIdx];
                
                if (pokemonFields.P1MonsNo != 0)
                    trainer.trainerPokemon.Add(new TrainerPokemon
                    {
                        dexID = pokemonFields.P1MonsNo,
                        formID = pokemonFields.P1FormNo,
                        isRare = pokemonFields.P1IsRare,
                        level = pokemonFields.P1Level,
                        sex = pokemonFields.P1Sex,
                        natureID = pokemonFields.P1Seikaku,
                        abilityID = pokemonFields.P1Tokusei,
                        moveID1 = pokemonFields.P1Waza1,
                        moveID2 = pokemonFields.P1Waza2,
                        moveID3 = pokemonFields.P1Waza3,
                        moveID4 = pokemonFields.P1Waza4,
                        itemID = pokemonFields.P1Item,
                        ballID = pokemonFields.P1Ball,
                        seal = pokemonFields.P1Seal,
                        hpIV = pokemonFields.P1TalentHp,
                        atkIV = pokemonFields.P1TalentAtk,
                        defIV = pokemonFields.P1TalentDef,
                        spAtkIV = pokemonFields.P1TalentSpAtk,
                        spDefIV = pokemonFields.P1TalentSpDef,
                        spdIV = pokemonFields.P1TalentAgi,
                        hpEV = pokemonFields.P1EffortHp,
                        atkEV = pokemonFields.P1EffortAtk,
                        defEV = pokemonFields.P1EffortDef,
                        spAtkEV = pokemonFields.P1EffortSpAtk,
                        spDefEV = pokemonFields.P1EffortSpDef,
                        spdEV = pokemonFields.P1EffortAgi,
                    });

                if (pokemonFields.P2MonsNo != 0)
                    trainer.trainerPokemon.Add(new TrainerPokemon
                    {
                        dexID = pokemonFields.P2MonsNo,
                        formID = pokemonFields.P2FormNo,
                        isRare = pokemonFields.P2IsRare,
                        level = pokemonFields.P2Level,
                        sex = pokemonFields.P2Sex,
                        natureID = pokemonFields.P2Seikaku,
                        abilityID = pokemonFields.P2Tokusei,
                        moveID1 = pokemonFields.P2Waza1,
                        moveID2 = pokemonFields.P2Waza2,
                        moveID3 = pokemonFields.P2Waza3,
                        moveID4 = pokemonFields.P2Waza4,
                        itemID = pokemonFields.P2Item,
                        ballID = pokemonFields.P2Ball,
                        seal = pokemonFields.P2Seal,
                        hpIV = pokemonFields.P2TalentHp,
                        atkIV = pokemonFields.P2TalentAtk,
                        defIV = pokemonFields.P2TalentDef,
                        spAtkIV = pokemonFields.P2TalentSpAtk,
                        spDefIV = pokemonFields.P2TalentSpDef,
                        spdIV = pokemonFields.P2TalentAgi,
                        hpEV = pokemonFields.P2EffortHp,
                        atkEV = pokemonFields.P2EffortAtk,
                        defEV = pokemonFields.P2EffortDef,
                        spAtkEV = pokemonFields.P2EffortSpAtk,
                        spDefEV = pokemonFields.P2EffortSpDef,
                        spdEV = pokemonFields.P2EffortAgi,
                    });

                if (pokemonFields.P3MonsNo != 0)
                    trainer.trainerPokemon.Add(new TrainerPokemon
                    {
                        dexID = pokemonFields.P3MonsNo,
                        formID = pokemonFields.P3FormNo,
                        isRare = pokemonFields.P3IsRare,
                        level = pokemonFields.P3Level,
                        sex = pokemonFields.P3Sex,
                        natureID = pokemonFields.P3Seikaku,
                        abilityID = pokemonFields.P3Tokusei,
                        moveID1 = pokemonFields.P3Waza1,
                        moveID2 = pokemonFields.P3Waza2,
                        moveID3 = pokemonFields.P3Waza3,
                        moveID4 = pokemonFields.P3Waza4,
                        itemID = pokemonFields.P3Item,
                        ballID = pokemonFields.P3Ball,
                        seal = pokemonFields.P3Seal,
                        hpIV = pokemonFields.P3TalentHp,
                        atkIV = pokemonFields.P3TalentAtk,
                        defIV = pokemonFields.P3TalentDef,
                        spAtkIV = pokemonFields.P3TalentSpAtk,
                        spDefIV = pokemonFields.P3TalentSpDef,
                        spdIV = pokemonFields.P3TalentAgi,
                        hpEV = pokemonFields.P3EffortHp,
                        atkEV = pokemonFields.P3EffortAtk,
                        defEV = pokemonFields.P3EffortDef,
                        spAtkEV = pokemonFields.P3EffortSpAtk,
                        spDefEV = pokemonFields.P3EffortSpDef,
                        spdEV = pokemonFields.P3EffortAgi,
                    });

                if (pokemonFields.P4MonsNo != 0)
                    trainer.trainerPokemon.Add(new TrainerPokemon
                    {
                        dexID = pokemonFields.P4MonsNo,
                        formID = pokemonFields.P4FormNo,
                        isRare = pokemonFields.P4IsRare,
                        level = pokemonFields.P4Level,
                        sex = pokemonFields.P4Sex,
                        natureID = pokemonFields.P4Seikaku,
                        abilityID = pokemonFields.P4Tokusei,
                        moveID1 = pokemonFields.P4Waza1,
                        moveID2 = pokemonFields.P4Waza2,
                        moveID3 = pokemonFields.P4Waza3,
                        moveID4 = pokemonFields.P4Waza4,
                        itemID = pokemonFields.P4Item,
                        ballID = pokemonFields.P4Ball,
                        seal = pokemonFields.P4Seal,
                        hpIV = pokemonFields.P4TalentHp,
                        atkIV = pokemonFields.P4TalentAtk,
                        defIV = pokemonFields.P4TalentDef,
                        spAtkIV = pokemonFields.P4TalentSpAtk,
                        spDefIV = pokemonFields.P4TalentSpDef,
                        spdIV = pokemonFields.P4TalentAgi,
                        hpEV = pokemonFields.P4EffortHp,
                        atkEV = pokemonFields.P4EffortAtk,
                        defEV = pokemonFields.P4EffortDef,
                        spAtkEV = pokemonFields.P4EffortSpAtk,
                        spDefEV = pokemonFields.P4EffortSpDef,
                        spdEV = pokemonFields.P4EffortAgi,
                    });

                if (pokemonFields.P5MonsNo != 0)
                    trainer.trainerPokemon.Add(new TrainerPokemon
                    {
                        dexID = pokemonFields.P5MonsNo,
                        formID = pokemonFields.P5FormNo,
                        isRare = pokemonFields.P5IsRare,
                        level = pokemonFields.P5Level,
                        sex = pokemonFields.P5Sex,
                        natureID = pokemonFields.P5Seikaku,
                        abilityID = pokemonFields.P5Tokusei,
                        moveID1 = pokemonFields.P5Waza1,
                        moveID2 = pokemonFields.P5Waza2,
                        moveID3 = pokemonFields.P5Waza3,
                        moveID4 = pokemonFields.P5Waza4,
                        itemID = pokemonFields.P5Item,
                        ballID = pokemonFields.P5Ball,
                        seal = pokemonFields.P5Seal,
                        hpIV = pokemonFields.P5TalentHp,
                        atkIV = pokemonFields.P5TalentAtk,
                        defIV = pokemonFields.P5TalentDef,
                        spAtkIV = pokemonFields.P5TalentSpAtk,
                        spDefIV = pokemonFields.P5TalentSpDef,
                        spdIV = pokemonFields.P5TalentAgi,
                        hpEV = pokemonFields.P5EffortHp,
                        atkEV = pokemonFields.P5EffortAtk,
                        defEV = pokemonFields.P5EffortDef,
                        spAtkEV = pokemonFields.P5EffortSpAtk,
                        spDefEV = pokemonFields.P5EffortSpDef,
                        spdEV = pokemonFields.P5EffortAgi,
                    });

                if (pokemonFields.P6MonsNo != 0)
                    trainer.trainerPokemon.Add(new TrainerPokemon
                    {
                        dexID = pokemonFields.P6MonsNo,
                        formID = pokemonFields.P6FormNo,
                        isRare = pokemonFields.P6IsRare,
                        level = pokemonFields.P6Level,
                        sex = pokemonFields.P6Sex,
                        natureID = pokemonFields.P6Seikaku,
                        abilityID = pokemonFields.P6Tokusei,
                        moveID1 = pokemonFields.P6Waza1,
                        moveID2 = pokemonFields.P6Waza2,
                        moveID3 = pokemonFields.P6Waza3,
                        moveID4 = pokemonFields.P6Waza4,
                        itemID = pokemonFields.P6Item,
                        ballID = pokemonFields.P6Ball,
                        seal = pokemonFields.P6Seal,
                        hpIV = pokemonFields.P6TalentHp,
                        atkIV = pokemonFields.P6TalentAtk,
                        defIV = pokemonFields.P6TalentDef,
                        spAtkIV = pokemonFields.P6TalentSpAtk,
                        spDefIV = pokemonFields.P6TalentSpDef,
                        spdIV = pokemonFields.P6TalentAgi,
                        hpEV = pokemonFields.P6EffortHp,
                        atkEV = pokemonFields.P6EffortAtk,
                        defEV = pokemonFields.P6EffortDef,
                        spAtkEV = pokemonFields.P6EffortSpAtk,
                        spDefEV = pokemonFields.P6EffortSpDef,
                        spdEV = pokemonFields.P6EffortAgi,
                    });

                gameData.trainers.Add(trainer);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with a parsed Battle Tower Trainer table .
        /// </summary>
        private static async Task ParseBattleTowerTrainers()
        {
            gameData.battleTowerTrainers = new();
            gameData.battleTowerTrainersDouble = new();
            gameData.battleTowerTrainerPokemons = new();
            AssetTypeValueField monoBehaviour = (await monoBehaviourCollection[PathEnum.DprMasterdatas]).Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "TowerTrainerTable");
            AssetTypeValueField monoBehaviour2 = (await monoBehaviourCollection[PathEnum.DprMasterdatas]).Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "TowerSingleStockTable");
            AssetTypeValueField monoBehaviour3 = (await monoBehaviourCollection[PathEnum.DprMasterdatas]).Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "TowerDoubleStockTable");

            AssetTypeValueField[] nameFields = await FindLabelArrayOfMessageFileAsync("dp_trainers_name", Language.English);
            Dictionary<string, string> trainerNames = new();
            gameData.trainerNames = trainerNames;
            foreach (AssetTypeValueField label in nameFields)
                if (label.children[6].children[0].childrenCount > 0)
                    trainerNames[label.children[2].GetValue().AsString()] = label.children[6].children[0].children[0].children[4].GetValue().AsString();
            AssetTypeValueField[] trainerFields = monoBehaviour2.children[4].children[0].children;
            AssetTypeValueField[] trainerFieldsDouble = monoBehaviour3.children[4].children[0].children;
            AssetTypeValueField[] pokemonFields = monoBehaviour.children[5].children[0].children;
            AssetTypeValueField[] nameFieldsTower = monoBehaviour.children[4].children[0].children;
            //Single battle parser
            for (int trainerIdx = 0; trainerIdx < trainerFields.Length; trainerIdx++)
            {
                BattleTowerTrainer trainer = new();
                trainer.trainerID2 = trainerFields[trainerIdx].children[0].GetValue().AsUInt();
                trainer.trainerTypeID = trainerFields[trainerIdx].children[1].GetValue().AsInt();
                trainer.trainerTypeID2 = -1; ;
                trainer.battleTowerPokemonID1 = trainerFields[trainerIdx].children[2].children[0].children[0].GetValue().AsUInt();
                trainer.battleTowerPokemonID2 = trainerFields[trainerIdx].children[2].children[0].children[1].GetValue().AsUInt();
                trainer.battleTowerPokemonID3 = trainerFields[trainerIdx].children[2].children[0].children[2].GetValue().AsUInt();
                trainer.battleTowerPokemonID4 = 0;
                trainer.battleBGM = trainerFields[trainerIdx].children[3].GetValue().AsString();
                trainer.winBGM = trainerFields[trainerIdx].children[4].GetValue().AsString();
                trainer.nameLabel = nameFieldsTower[trainer.trainerTypeID].children[1].GetValue().AsString();
                trainer.name = trainerNames[trainer.nameLabel];
                trainer.nameLabel2 = null;
                trainer.name2 = null;
                trainer.isDouble = false;
                gameData.battleTowerTrainers.Add(trainer);
            }
            //Double battle parser
            for (int trainerIdx = 0; trainerIdx < trainerFieldsDouble.Length; trainerIdx++)
            {
                BattleTowerTrainer trainer = new();
                trainer.trainerID2 = trainerFieldsDouble[trainerIdx].children[0].GetValue().AsUInt();
                trainer.trainerTypeID = trainerFieldsDouble[trainerIdx].children[1].children[0].children[0].GetValue().AsInt();
                trainer.trainerTypeID2 = trainerFieldsDouble[trainerIdx].children[1].children[0].children[1].GetValue().AsInt();
                trainer.battleTowerPokemonID1 = trainerFieldsDouble[trainerIdx].children[2].children[0].children[0].GetValue().AsUInt();
                trainer.battleTowerPokemonID2 = trainerFieldsDouble[trainerIdx].children[2].children[0].children[1].GetValue().AsUInt();
                trainer.battleTowerPokemonID3 = trainerFieldsDouble[trainerIdx].children[2].children[0].children[2].GetValue().AsUInt();
                trainer.battleTowerPokemonID4 = trainerFieldsDouble[trainerIdx].children[2].children[0].children[3].GetValue().AsUInt();
                trainer.battleBGM = trainerFieldsDouble[trainerIdx].children[3].GetValue().AsString();
                trainer.winBGM = trainerFieldsDouble[trainerIdx].children[4].GetValue().AsString();
                trainer.nameLabel = nameFieldsTower[trainer.trainerTypeID].children[1].GetValue().AsString();
                trainer.name = trainerNames[trainer.nameLabel];
                if (trainer.trainerTypeID2 != -1)
                {
                    trainer.nameLabel2 = nameFieldsTower[trainer.trainerTypeID2].children[1].GetValue().AsString();
                    trainer.name2 = trainerNames[trainer.nameLabel2];
                }
                else
                {
                    trainer.nameLabel2 = null;
                    trainer.name2 = null;
                }
                trainer.isDouble = true;
                gameData.battleTowerTrainersDouble.Add(trainer);
            }

            //Parse battle tower trainer pokemon
            for (int pokemonIdx = 0; pokemonIdx < pokemonFields.Length && pokemonFields[pokemonIdx].children[0].value.value.asUInt32 != 0; pokemonIdx += 1)
            {
                BattleTowerTrainerPokemon pokemon = new();
                pokemon.pokemonID = pokemonFields[pokemonIdx].children[0].value.value.asUInt32;
                pokemon.dexID = pokemonFields[pokemonIdx].children[1].value.value.asInt32;
                pokemon.formID = pokemonFields[pokemonIdx].children[2].value.value.asUInt16;
                pokemon.isRare = pokemonFields[pokemonIdx].children[3].value.value.asUInt8;
                pokemon.level = pokemonFields[pokemonIdx].children[4].value.value.asUInt8;
                pokemon.sex = pokemonFields[pokemonIdx].children[5].value.value.asUInt8;
                pokemon.natureID = pokemonFields[pokemonIdx].children[6].value.value.asInt32;
                pokemon.abilityID = pokemonFields[pokemonIdx].children[7].value.value.asInt32;
                pokemon.moveID1 = pokemonFields[pokemonIdx].children[8].value.value.asInt32;
                pokemon.moveID2 = pokemonFields[pokemonIdx].children[9].value.value.asInt32;
                pokemon.moveID3 = pokemonFields[pokemonIdx].children[10].value.value.asInt32;
                pokemon.moveID4 = pokemonFields[pokemonIdx].children[11].value.value.asInt32;
                pokemon.itemID = pokemonFields[pokemonIdx].children[12].value.value.asUInt16;
                pokemon.ballID = pokemonFields[pokemonIdx].children[13].value.value.asUInt8;
                pokemon.seal = pokemonFields[pokemonIdx].children[14].value.value.asInt32;
                pokemon.hpIV = pokemonFields[pokemonIdx].children[15].value.value.asUInt8;
                pokemon.atkIV = pokemonFields[pokemonIdx].children[16].value.value.asUInt8;
                pokemon.defIV = pokemonFields[pokemonIdx].children[17].value.value.asUInt8;
                pokemon.spAtkIV = pokemonFields[pokemonIdx].children[18].value.value.asUInt8;
                pokemon.spDefIV = pokemonFields[pokemonIdx].children[19].value.value.asUInt8;
                pokemon.spdIV = pokemonFields[pokemonIdx].children[20].value.value.asUInt8;
                pokemon.hpEV = pokemonFields[pokemonIdx].children[21].value.value.asUInt8;
                pokemon.atkEV = pokemonFields[pokemonIdx].children[22].value.value.asUInt8;
                pokemon.defEV = pokemonFields[pokemonIdx].children[23].value.value.asUInt8;
                pokemon.spAtkEV = pokemonFields[pokemonIdx].children[24].value.value.asUInt8;
                pokemon.spDefEV = pokemonFields[pokemonIdx].children[25].value.value.asUInt8;
                pokemon.spdEV = pokemonFields[pokemonIdx].children[26].value.value.asUInt8;
                gameData.battleTowerTrainerPokemons.Add(pokemon);
            }

        }

        /// <summary>
        ///  Overwrites GlobalData with a parsed yaml Battle Tower Trainer table.
        /// </summary>
        private static async Task ParseBattleTowerTrainersYAML()
        {
            gameData.battleTowerTrainers = new();
            gameData.battleTowerTrainersDouble = new();
            gameData.battleTowerTrainerPokemons = new();

            var towerTrainers = (await yamlCollection[PathEnum.DprMasterdatas]).Find(y => y.MonoBehaviour?.Name == "TowerTrainerTable").MonoBehaviour as TowerTrainerTable;
            var singleStock = (await yamlCollection[PathEnum.DprMasterdatas]).Find(y => y.MonoBehaviour?.Name == "TowerSingleStockTable").MonoBehaviour as TowerSingleStockTable;
            var doubleStock = (await yamlCollection[PathEnum.DprMasterdatas]).Find(y => y.MonoBehaviour?.Name == "TowerDoubleStockTable").MonoBehaviour as TowerDoubleStockTable;

            var nameFields = await FindLabelArrayOfYAMLMessageFileAsync("dp_trainers_name", Language.English);
            Dictionary<string, string> trainerNames = new();
            gameData.trainerNames = trainerNames;
            foreach (var label in nameFields)
                if (label.WordDataArray.Length > 0)
                    trainerNames[label.LabelName] = label.WordDataArray[0].Str;

            var trainerFields = singleStock.TowerSingleStock;
            var trainerFieldsDouble = doubleStock.TowerDoubleStock;
            var pokemonFields = towerTrainers.TrainerPoke;
            var nameFieldsTower = towerTrainers.TrainerData;

            //Single battle parser
            for (int trainerIdx = 0; trainerIdx < trainerFields.Length; trainerIdx++)
            {
                BattleTowerTrainer trainer = new();
                trainer.trainerID2 = trainerFields[trainerIdx].ID;
                trainer.trainerTypeID = trainerFields[trainerIdx].TrainerID;
                trainer.trainerTypeID2 = -1;
                trainer.battleTowerPokemonID1 = trainerFields[trainerIdx].PokeID[0];
                trainer.battleTowerPokemonID2 = trainerFields[trainerIdx].PokeID[1];
                trainer.battleTowerPokemonID3 = trainerFields[trainerIdx].PokeID[2];
                trainer.battleTowerPokemonID4 = 0;
                trainer.battleBGM = trainerFields[trainerIdx].BattleBGM;
                trainer.winBGM = trainerFields[trainerIdx].WinBGM;
                trainer.nameLabel = nameFieldsTower[trainer.trainerTypeID].NameLabel;
                trainer.name = trainerNames[trainer.nameLabel];
                trainer.nameLabel2 = null;
                trainer.name2 = null;
                trainer.isDouble = false;

                gameData.battleTowerTrainers.Add(trainer);
            }

            //Double battle parser
            for (int trainerIdx = 0; trainerIdx < trainerFieldsDouble.Length; trainerIdx++)
            {
                BattleTowerTrainer trainer = new();
                trainer.trainerID2 = trainerFieldsDouble[trainerIdx].ID;
                trainer.trainerTypeID = trainerFieldsDouble[trainerIdx].TrainerID[0];
                trainer.trainerTypeID2 = trainerFieldsDouble[trainerIdx].TrainerID[1];
                trainer.battleTowerPokemonID1 = trainerFieldsDouble[trainerIdx].PokeID[0];
                trainer.battleTowerPokemonID2 = trainerFieldsDouble[trainerIdx].PokeID[1];
                trainer.battleTowerPokemonID3 = trainerFieldsDouble[trainerIdx].PokeID[2];
                trainer.battleTowerPokemonID4 = trainerFieldsDouble[trainerIdx].PokeID[3];
                trainer.battleBGM = trainerFieldsDouble[trainerIdx].BattleBGM;
                trainer.winBGM = trainerFieldsDouble[trainerIdx].WinBGM;
                trainer.nameLabel = nameFieldsTower[trainer.trainerTypeID].NameLabel;
                trainer.name = trainerNames[trainer.nameLabel];

                if (trainer.trainerTypeID2 != -1)
                {
                    trainer.nameLabel2 = nameFieldsTower[trainer.trainerTypeID2].NameLabel;
                    trainer.name2 = trainerNames[trainer.nameLabel2];
                }
                else
                {
                    trainer.nameLabel2 = null;
                    trainer.name2 = null;
                }
                trainer.isDouble = true;

                gameData.battleTowerTrainersDouble.Add(trainer);
            }

            //Parse battle tower trainer pokemon
            for (int pokemonIdx = 0; pokemonIdx < pokemonFields.Length && pokemonFields[pokemonIdx].ID != 0; pokemonIdx++)
            {
                BattleTowerTrainerPokemon pokemon = new()
                {
                    pokemonID = pokemonFields[pokemonIdx].ID,
                    dexID = pokemonFields[pokemonIdx].MonsNo,
                    formID = pokemonFields[pokemonIdx].FormNo,
                    isRare = pokemonFields[pokemonIdx].IsRare,
                    level = pokemonFields[pokemonIdx].Level,
                    sex = pokemonFields[pokemonIdx].Sex,
                    natureID = pokemonFields[pokemonIdx].Seikaku,
                    abilityID = pokemonFields[pokemonIdx].Tokusei,
                    moveID1 = pokemonFields[pokemonIdx].Waza1,
                    moveID2 = pokemonFields[pokemonIdx].Waza2,
                    moveID3 = pokemonFields[pokemonIdx].Waza3,
                    moveID4 = pokemonFields[pokemonIdx].Waza4,
                    itemID = pokemonFields[pokemonIdx].Item,
                    ballID = pokemonFields[pokemonIdx].Ball,
                    seal = pokemonFields[pokemonIdx].Seal,
                    hpIV = pokemonFields[pokemonIdx].TalentHP,
                    atkIV = pokemonFields[pokemonIdx].TalentAtk,
                    defIV = pokemonFields[pokemonIdx].TalentDef,
                    spAtkIV = pokemonFields[pokemonIdx].TalentSpAtk,
                    spDefIV = pokemonFields[pokemonIdx].TalentSpDef,
                    spdIV = pokemonFields[pokemonIdx].TalentAgi,
                    hpEV = pokemonFields[pokemonIdx].EffortHP,
                    atkEV = pokemonFields[pokemonIdx].EffortAtk,
                    defEV = pokemonFields[pokemonIdx].EffortDef,
                    spAtkEV = pokemonFields[pokemonIdx].EffortSpAtk,
                    spDefEV = pokemonFields[pokemonIdx].EffortSpDef,
                    spdEV = pokemonFields[pokemonIdx].EffortAgi,
                };

                gameData.battleTowerTrainerPokemons.Add(pokemon);
            }

        }

        /// <summary>
        ///  Overwrites GlobalData with parsed EncounterTables.
        /// </summary>
        private static async Task ParseEncounterTables()
        {
            gameData.encounterTableFiles = new EncounterTableFile[2];

            List<AssetTypeValueField> monoBehaviours = await monoBehaviourCollection[PathEnum.Gamesettings];
            AssetTypeValueField[] encounterTableMonoBehaviours = new AssetTypeValueField[2];
            encounterTableMonoBehaviours[0] = monoBehaviours.Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "FieldEncountTable_d");
            encounterTableMonoBehaviours[1] = monoBehaviours.Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "FieldEncountTable_p");
            for (int encounterTableFileIdx = 0; encounterTableFileIdx < encounterTableMonoBehaviours.Length; encounterTableFileIdx++)
            {
                EncounterTableFile encounterTableFile = new();
                encounterTableFile.mName = Encoding.Default.GetString(encounterTableMonoBehaviours[encounterTableFileIdx].children[3].value.value.asString);

                //Parse wild encounter tables
                encounterTableFile.encounterTables = new();
                AssetTypeValueField[] encounterTableFields = encounterTableMonoBehaviours[encounterTableFileIdx].children[4].children[0].children;
                for (int encounterTableIdx = 0; encounterTableIdx < encounterTableFields.Length; encounterTableIdx++)
                {
                    EncounterTable encounterTable = new();
                    encounterTable.zoneID = (ZoneID)encounterTableFields[encounterTableIdx].children[0].value.value.asInt32;
                    encounterTable.encRateGround = encounterTableFields[encounterTableIdx].children[1].value.value.asInt32;
                    encounterTable.formProb = encounterTableFields[encounterTableIdx].children[7].children[0].children[0].value.value.asInt32;
                    encounterTable.unownTable = encounterTableFields[encounterTableIdx].children[9].children[0].children[1].value.value.asInt32;
                    encounterTable.encRateWater = encounterTableFields[encounterTableIdx].children[15].value.value.asInt32;
                    encounterTable.encRateOldRod = encounterTableFields[encounterTableIdx].children[17].value.value.asInt32;
                    encounterTable.encRateGoodRod = encounterTableFields[encounterTableIdx].children[19].value.value.asInt32;
                    encounterTable.encRateSuperRod = encounterTableFields[encounterTableIdx].children[21].value.value.asInt32;

                    //Parse ground tables
                    encounterTable.groundMons = GetParsedEncounters(encounterTableFields[encounterTableIdx].children[2].children[0].children);

                    //Parse morning tables
                    encounterTable.tairyo = GetParsedEncounters(encounterTableFields[encounterTableIdx].children[3].children[0].children);

                    //Parse day tables
                    encounterTable.day = GetParsedEncounters(encounterTableFields[encounterTableIdx].children[4].children[0].children);

                    //Parse night tables
                    encounterTable.night = GetParsedEncounters(encounterTableFields[encounterTableIdx].children[5].children[0].children);

                    //Parse pokefinder tables
                    encounterTable.swayGrass = GetParsedEncounters(encounterTableFields[encounterTableIdx].children[6].children[0].children);

                    //Parse ruby tables
                    encounterTable.gbaRuby = GetParsedEncounters(encounterTableFields[encounterTableIdx]["gbaRuby"].children[0].children);

                    //Parse sapphire tables
                    encounterTable.gbaSapphire = GetParsedEncounters(encounterTableFields[encounterTableIdx]["gbaSapp"].children[0].children);

                    //Parse emerald tables
                    encounterTable.gbaEmerald = GetParsedEncounters(encounterTableFields[encounterTableIdx]["gbaEme"].children[0].children);

                    //Parse fire tables
                    encounterTable.gbaFire = GetParsedEncounters(encounterTableFields[encounterTableIdx]["gbaFire"].children[0].children);

                    //Parse leaf tables
                    encounterTable.gbaLeaf = GetParsedEncounters(encounterTableFields[encounterTableIdx]["gbaLeaf"].children[0].children);

                    //Parse surfing tables
                    encounterTable.waterMons = GetParsedEncounters(encounterTableFields[encounterTableIdx].children[16].children[0].children);

                    //Parse old rod tables
                    encounterTable.oldRodMons = GetParsedEncounters(encounterTableFields[encounterTableIdx].children[18].children[0].children);

                    //Parse good rod tables
                    encounterTable.goodRodMons = GetParsedEncounters(encounterTableFields[encounterTableIdx].children[20].children[0].children);

                    //Parse super rod tables
                    encounterTable.superRodMons = GetParsedEncounters(encounterTableFields[encounterTableIdx].children[22].children[0].children);

                    encounterTableFile.encounterTables.Add(encounterTable);
                }

                //Parse trophy garden table
                encounterTableFile.trophyGardenMons = new();
                AssetTypeValueField[] trophyGardenMonFields = encounterTableMonoBehaviours[encounterTableFileIdx].children[5].children[0].children;
                for (int trophyGardenMonIdx = 0; trophyGardenMonIdx < trophyGardenMonFields.Length; trophyGardenMonIdx++)
                    encounterTableFile.trophyGardenMons.Add(trophyGardenMonFields[trophyGardenMonIdx].children[0].value.value.asInt32);

                //Parse honey tree tables
                encounterTableFile.honeyTreeEnconters = new();
                AssetTypeValueField[] honeyTreeEncounterFields = encounterTableMonoBehaviours[encounterTableFileIdx].children[6].children[0].children;
                for (int honeyTreeEncounterIdx = 0; honeyTreeEncounterIdx < honeyTreeEncounterFields.Length; honeyTreeEncounterIdx++)
                {
                    HoneyTreeEncounter honeyTreeEncounter = new();
                    honeyTreeEncounter.rate = honeyTreeEncounterFields[honeyTreeEncounterIdx].children[0].value.value.asInt32;
                    honeyTreeEncounter.normalDexID = honeyTreeEncounterFields[honeyTreeEncounterIdx].children[1].value.value.asInt32;
                    honeyTreeEncounter.rareDexID = honeyTreeEncounterFields[honeyTreeEncounterIdx].children[2].value.value.asInt32;
                    honeyTreeEncounter.superRareDexID = honeyTreeEncounterFields[honeyTreeEncounterIdx].children[3].value.value.asInt32;

                    encounterTableFile.honeyTreeEnconters.Add(honeyTreeEncounter);
                }

                //Parse safari table
                encounterTableFile.safariMons = new();
                AssetTypeValueField[] safariMonFields = encounterTableMonoBehaviours[encounterTableFileIdx].children[8].children[0].children;
                for (int safariMonIdx = 0; safariMonIdx < safariMonFields.Length; safariMonIdx++)
                    encounterTableFile.safariMons.Add(safariMonFields[safariMonIdx].children[0].value.value.asInt32);

                gameData.encounterTableFiles[encounterTableFileIdx] = encounterTableFile;
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed yaml EncounterTables.
        /// </summary>
        private static async Task ParseEncounterTablesYAML()
        {
            gameData.encounterTableFiles = new EncounterTableFile[2];

            List<YamlMonoContainer> monoBehaviours = await yamlCollection[PathEnum.Gamesettings];
            FieldEncountTable[] encounterTableMonoBehaviours = new FieldEncountTable[2]
            {
                monoBehaviours.Find(m => m.MonoBehaviour?.Name == "FieldEncountTable_d").MonoBehaviour as FieldEncountTable,
                monoBehaviours.Find(m => m.MonoBehaviour?.Name == "FieldEncountTable_p").MonoBehaviour as FieldEncountTable,
            };

            for (int encounterTableFileIdx = 0; encounterTableFileIdx < encounterTableMonoBehaviours.Length; encounterTableFileIdx++)
            {
                EncounterTableFile encounterTableFile = new();
                encounterTableFile.mName = encounterTableMonoBehaviours[encounterTableFileIdx].Name;

                //Parse wild encounter tables
                encounterTableFile.encounterTables = new();
                var table = encounterTableMonoBehaviours[encounterTableFileIdx].Table;
                for (int encounterTableIdx = 0; encounterTableIdx < table.Length; encounterTableIdx++)
                {
                    EncounterTable encounterTable = new();
                    encounterTable.zoneID = (ZoneID)table[encounterTableIdx].ZoneID;
                    encounterTable.encRateGround = table[encounterTableIdx].EncRateGr;
                    encounterTable.formProb = table[encounterTableIdx].FormProb[0];
                    encounterTable.unownTable = table[encounterTableIdx].AnnoonTable[1];
                    encounterTable.encRateWater = table[encounterTableIdx].EncRateWat;
                    encounterTable.encRateOldRod = table[encounterTableIdx].EncRateTuriBoro;
                    encounterTable.encRateGoodRod = table[encounterTableIdx].EncRateTuriIi;
                    encounterTable.encRateSuperRod = table[encounterTableIdx].EncRateSugoi;

                    //Parse ground tables
                    encounterTable.groundMons = ConvertMonsLvToEncounters(table[encounterTableIdx].GroundMons);

                    //Parse morning tables
                    encounterTable.tairyo = ConvertMonsLvToEncounters(table[encounterTableIdx].Tairyo);

                    //Parse day tables
                    encounterTable.day = ConvertMonsLvToEncounters(table[encounterTableIdx].Day);

                    //Parse night tables
                    encounterTable.night = ConvertMonsLvToEncounters(table[encounterTableIdx].Night);

                    //Parse pokefinder tables
                    encounterTable.swayGrass = ConvertMonsLvToEncounters(table[encounterTableIdx].SwayGrass);

                    //Parse ruby tables
                    encounterTable.gbaRuby = ConvertMonsLvToEncounters(table[encounterTableIdx].GBARuby);

                    //Parse sapphire tables
                    encounterTable.gbaSapphire = ConvertMonsLvToEncounters(table[encounterTableIdx].GBASapp);

                    //Parse emerald tables
                    encounterTable.gbaEmerald = ConvertMonsLvToEncounters(table[encounterTableIdx].GBAEme);

                    //Parse fire tables
                    encounterTable.gbaFire = ConvertMonsLvToEncounters(table[encounterTableIdx].GBAFire);

                    //Parse leaf tables
                    encounterTable.gbaLeaf = ConvertMonsLvToEncounters(table[encounterTableIdx].GBALeaf);

                    //Parse surfing tables
                    encounterTable.waterMons = ConvertMonsLvToEncounters(table[encounterTableIdx].WaterMons);

                    //Parse old rod tables
                    encounterTable.oldRodMons = ConvertMonsLvToEncounters(table[encounterTableIdx].BoroMons);

                    //Parse good rod tables
                    encounterTable.goodRodMons = ConvertMonsLvToEncounters(table[encounterTableIdx].IiMons);

                    //Parse super rod tables
                    encounterTable.superRodMons = ConvertMonsLvToEncounters(table[encounterTableIdx].SugoiMons);

                    encounterTableFile.encounterTables.Add(encounterTable);
                }

                //Parse trophy garden table
                encounterTableFile.trophyGardenMons = new();
                var trophyTable = encounterTableMonoBehaviours[encounterTableFileIdx].Urayama;
                for (int trophyGardenMonIdx = 0; trophyGardenMonIdx < trophyTable.Length; trophyGardenMonIdx++)
                    encounterTableFile.trophyGardenMons.Add(trophyTable[trophyGardenMonIdx].MonsNo);

                //Parse honey tree tables
                encounterTableFile.honeyTreeEnconters = new();
                var honeyTable = encounterTableMonoBehaviours[encounterTableFileIdx].Mistu;
                for (int honeyTreeEncounterIdx = 0; honeyTreeEncounterIdx < honeyTable.Length; honeyTreeEncounterIdx++)
                {
                    HoneyTreeEncounter honeyTreeEncounter = new()
                    {
                        rate = honeyTable[honeyTreeEncounterIdx].Rate,
                        normalDexID = honeyTable[honeyTreeEncounterIdx].Normal,
                        rareDexID = honeyTable[honeyTreeEncounterIdx].Rare,
                        superRareDexID = honeyTable[honeyTreeEncounterIdx].SuperRare
                    };

                    encounterTableFile.honeyTreeEnconters.Add(honeyTreeEncounter);
                }

                //Parse safari table
                encounterTableFile.safariMons = new();
                var safariTable = encounterTableMonoBehaviours[encounterTableFileIdx].Safari;
                for (int safariMonIdx = 0; safariMonIdx < safariTable.Length; safariMonIdx++)
                    encounterTableFile.safariMons.Add(safariTable[safariMonIdx].MonsNo);

                gameData.encounterTableFiles[encounterTableFileIdx] = encounterTableFile;
            }
        }

        /// <summary>
        ///  Parses an array of encounters in a monobehaviour into a list of Encounters.
        /// </summary>
        private static List<Encounter> GetParsedEncounters(AssetTypeValueField[] encounterFields)
        {
            List<Encounter> encounters = new();
            for (int encounterIdx = 0; encounterIdx < encounterFields.Length; encounterIdx++)
            {
                Encounter encounter = new();
                encounter.maxLv = encounterFields[encounterIdx].children[0].value.value.asInt32;
                encounter.minLv = encounterFields[encounterIdx].children[1].value.value.asInt32;
                encounter.dexID = encounterFields[encounterIdx].children[2].value.value.asInt32;

                encounters.Add(encounter);
            }
            return encounters;
        }

        /// <summary>
        ///  Converts an array of MonsLv into a List of Encounters.
        /// </summary>
        private static List<Encounter> ConvertMonsLvToEncounters(MonsLv[] monsLv)
        {
            List<Encounter> encounters = new();
            for (int encounterIdx = 0; encounterIdx < monsLv.Length; encounterIdx++)
            {
                Encounter encounter = new()
                {
                    maxLv = monsLv[encounterIdx].MaxLv,
                    minLv = monsLv[encounterIdx].MinLv,
                    dexID = monsLv[encounterIdx].MonsNo
                };

                encounters.Add(encounter);
            }
            return encounters;
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed DexEntries and PersonalEntries.
        /// </summary>
        private static async Task ParsePokemon()
        {
            gameData.dexEntries = new();
            gameData.personalEntries = new();
            List<AssetTypeValueField> monoBehaviours = (await monoBehaviourCollection[PathEnum.PersonalMasterdatas]);

            AssetTypeValueField[] levelUpMoveFields = monoBehaviours.Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "WazaOboeTable").children[4].children[0].children;
            AssetTypeValueField[] eggMoveFields = monoBehaviours.Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "TamagoWazaTable").children[4].children[0].children;
            AssetTypeValueField[] evolveFields = monoBehaviours.Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "EvolveTable").children[4].children[0].children;
            AssetTypeValueField[] personalFields = monoBehaviours.Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "PersonalTable").children[4].children[0].children;
            AssetTypeValueField[] textFields = await FindLabelArrayOfMessageFileAsync("ss_monsname", Language.English);

            if (levelUpMoveFields.Length < personalFields.Length)
                MainForm.ShowParserError("Oh my, this WazaOboeTable is missing some stuff...\n" +
                    "I don't feel so good...\n" +
                    "PersonalTable entries: " + personalFields.Length + "\n" +
                    "WazaOboeTable entries: " + levelUpMoveFields.Length + "??");
            if (eggMoveFields.Length < personalFields.Length)
                MainForm.ShowParserError("Oh my, this TamagoWazaTable is missing some stuff...\n" +
                    "I don't feel so good...\n" +
                    "PersonalTable entries: " + personalFields.Length + "\n" +
                    "TamagoWazaTable entries: " + eggMoveFields.Length + "??");
            if (evolveFields.Length < personalFields.Length)
                MainForm.ShowParserError("Oh my, this EvolveTable is missing some stuff...\n" +
                    "I don't feel so good...\n" +
                    "PersonalTable entries: " + personalFields.Length + "\n" +
                    "EvolveTable entries: " + evolveFields.Length + "??");

            for (int personalID = 0; personalID < personalFields.Length; personalID++)
            {
                Pokemon pokemon = new();
                pokemon.validFlag = personalFields[personalID].children[0].value.value.asUInt8;
                pokemon.personalID = personalFields[personalID].children[1].value.value.asUInt16;
                pokemon.dexID = personalFields[personalID].children[2].value.value.asUInt16;
                pokemon.formIndex = personalFields[personalID].children[3].value.value.asUInt16;
                pokemon.formMax = personalFields[personalID].children[4].value.value.asUInt8;
                pokemon.color = personalFields[personalID].children[5].value.value.asUInt8;
                pokemon.graNo = personalFields[personalID].children[6].value.value.asUInt16;
                pokemon.basicHp = personalFields[personalID].children[7].value.value.asUInt8;
                pokemon.basicAtk = personalFields[personalID].children[8].value.value.asUInt8;
                pokemon.basicDef = personalFields[personalID].children[9].value.value.asUInt8;
                pokemon.basicSpd = personalFields[personalID].children[10].value.value.asUInt8;
                pokemon.basicSpAtk = personalFields[personalID].children[11].value.value.asUInt8;
                pokemon.basicSpDef = personalFields[personalID].children[12].value.value.asUInt8;
                pokemon.typingID1 = personalFields[personalID].children[13].value.value.asUInt8;
                pokemon.typingID2 = personalFields[personalID].children[14].value.value.asUInt8;
                pokemon.getRate = personalFields[personalID].children[15].value.value.asUInt8;
                pokemon.rank = personalFields[personalID].children[16].value.value.asUInt8;
                pokemon.expValue = personalFields[personalID].children[17].value.value.asUInt16;
                pokemon.item1 = personalFields[personalID].children[18].value.value.asUInt16;
                pokemon.item2 = personalFields[personalID].children[19].value.value.asUInt16;
                pokemon.item3 = personalFields[personalID].children[20].value.value.asUInt16;
                pokemon.sex = personalFields[personalID].children[21].value.value.asUInt8;
                pokemon.eggBirth = personalFields[personalID].children[22].value.value.asUInt8;
                pokemon.initialFriendship = personalFields[personalID].children[23].value.value.asUInt8;
                pokemon.eggGroup1 = personalFields[personalID].children[24].value.value.asUInt8;
                pokemon.eggGroup2 = personalFields[personalID].children[25].value.value.asUInt8;
                pokemon.grow = personalFields[personalID].children[26].value.value.asUInt8;
                pokemon.abilityID1 = personalFields[personalID].children[27].value.value.asUInt16;
                pokemon.abilityID2 = personalFields[personalID].children[28].value.value.asUInt16;
                pokemon.abilityID3 = personalFields[personalID].children[29].value.value.asUInt16;
                pokemon.giveExp = personalFields[personalID].children[30].value.value.asUInt16;
                pokemon.height = personalFields[personalID].children[31].value.value.asUInt16;
                pokemon.weight = personalFields[personalID].children[32].value.value.asUInt16;
                pokemon.chihouZukanNo = personalFields[personalID].children[33].value.value.asUInt16;
                pokemon.machine1 = personalFields[personalID].children[34].value.value.asUInt32;
                pokemon.machine2 = personalFields[personalID].children[35].value.value.asUInt32;
                pokemon.machine3 = personalFields[personalID].children[36].value.value.asUInt32;
                pokemon.machine4 = personalFields[personalID].children[37].value.value.asUInt32;
                pokemon.hiddenMachine = personalFields[personalID].children[38].value.value.asUInt32;
                pokemon.eggMonsno = personalFields[personalID].children[39].value.value.asUInt16;
                pokemon.eggFormno = personalFields[personalID].children[40].value.value.asUInt16;
                pokemon.eggFormnoKawarazunoishi = personalFields[personalID].children[41].value.value.asUInt16;
                pokemon.eggFormInheritKawarazunoishi = personalFields[personalID].children[42].value.value.asUInt8;

                pokemon.formID = 0;
                if (pokemon.personalID != pokemon.dexID)
                    pokemon.formID = pokemon.personalID - pokemon.formIndex + 1;
                pokemon.name = "";
                if (textFields[pokemon.dexID].children[6].children[0].childrenCount > 0)
                    pokemon.name = Encoding.UTF8.GetString(textFields[pokemon.dexID].children[6].children[0].children[0].children[4].value.value.asString);
                pokemon.nextEvoLvs = (ushort.MaxValue, ushort.MaxValue); //(wildLevel, trainerLevel)
                pokemon.pastPokemon = new();
                pokemon.nextPokemon = new();
                pokemon.inferiorForms = new();
                pokemon.superiorForms = new();

                //Parse level up moves
                pokemon.levelUpMoves = new();
                for (int levelUpMoveIdx = 0; levelUpMoveIdx < levelUpMoveFields[personalID].children[1].children[0].childrenCount; levelUpMoveIdx += 2)
                {
                    LevelUpMove levelUpMove = new();
                    levelUpMove.level = levelUpMoveFields[personalID].children[1].children[0].children[levelUpMoveIdx].value.value.asUInt16;
                    levelUpMove.moveID = levelUpMoveFields[personalID].children[1].children[0].children[levelUpMoveIdx + 1].value.value.asUInt16;

                    pokemon.levelUpMoves.Add(levelUpMove);
                }

                //Parse egg moves
                pokemon.eggMoves = new();
                for (int eggMoveIdx = 0; eggMoveIdx < eggMoveFields[personalID].children[2].children[0].childrenCount; eggMoveIdx++)
                    pokemon.eggMoves.Add(eggMoveFields[personalID].children[2].children[0].children[eggMoveIdx].value.value.asUInt16);

                //Parse evolutions
                pokemon.evolutionPaths = new();
                for (int evolutionIdx = 0; evolutionIdx < evolveFields[personalID].children[1].children[0].childrenCount; evolutionIdx += 5)
                {
                    EvolutionPath evolution = new();
                    evolution.method = evolveFields[personalID].children[1].children[0].children[evolutionIdx].value.value.asUInt16;
                    evolution.parameter = evolveFields[personalID].children[1].children[0].children[evolutionIdx + 1].value.value.asUInt16;
                    evolution.destDexID = evolveFields[personalID].children[1].children[0].children[evolutionIdx + 2].value.value.asUInt16;
                    evolution.destFormID = evolveFields[personalID].children[1].children[0].children[evolutionIdx + 3].value.value.asUInt16;
                    evolution.level = evolveFields[personalID].children[1].children[0].children[evolutionIdx + 4].value.value.asUInt16;

                    pokemon.evolutionPaths.Add(evolution);
                }

                pokemon.externalTMLearnset = fileManager.TryGetExternalJson<TMLearnset>(
                    $"MonData\\TMLearnset\\monsno_{pokemon.dexID}_formno_{pokemon.formID}.json");

                gameData.personalEntries.Add(pokemon);

                if (gameData.dexEntries.Count == pokemon.dexID)
                {
                    gameData.dexEntries.Add(new());
                    gameData.dexEntries[pokemon.dexID].dexID = pokemon.dexID;
                    gameData.dexEntries[pokemon.dexID].forms = new();
                    gameData.dexEntries[pokemon.dexID].name = pokemon.name;
                }

                gameData.dexEntries[pokemon.dexID].forms.Add(pokemon);
            }

            SetFamilies();
            SetLegendaries();
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed yaml DexEntries and PersonalEntries.
        /// </summary>
        private static async Task ParsePokemonYAML()
        {
            gameData.dexEntries = new();
            gameData.personalEntries = new();
            var monoBehaviours = (await yamlCollection[PathEnum.PersonalMasterdatas]);

            var levelUpMoveFields = (monoBehaviours.Find(y => y.MonoBehaviour?.Name == "WazaOboeTable").MonoBehaviour as WazaOboeTable).WazaOboe;
            var eggMoveFields = (monoBehaviours.Find(y => y.MonoBehaviour?.Name == "TamagoWazaTable").MonoBehaviour as TamagoWazaTable).Data;
            var evolveFields = (monoBehaviours.Find(y => y.MonoBehaviour?.Name == "EvolveTable").MonoBehaviour as EvolveTable).Evolve;
            var personalFields = (monoBehaviours.Find(y => y.MonoBehaviour?.Name == "PersonalTable").MonoBehaviour as PersonalTable).Personal;
            var textFields = await FindLabelArrayOfYAMLMessageFileAsync("ss_monsname", Language.English);

            if (levelUpMoveFields.Length < personalFields.Length)
                MainForm.ShowParserError("Oh my, this WazaOboeTable is missing some stuff...\n" +
                    "I don't feel so good...\n" +
                    "PersonalTable entries: " + personalFields.Length + "\n" +
                    "WazaOboeTable entries: " + levelUpMoveFields.Length + "??");
            if (eggMoveFields.Length < personalFields.Length)
                MainForm.ShowParserError("Oh my, this TamagoWazaTable is missing some stuff...\n" +
                    "I don't feel so good...\n" +
                    "PersonalTable entries: " + personalFields.Length + "\n" +
                    "TamagoWazaTable entries: " + eggMoveFields.Length + "??");
            if (evolveFields.Length < personalFields.Length)
                MainForm.ShowParserError("Oh my, this EvolveTable is missing some stuff...\n" +
                    "I don't feel so good...\n" +
                    "PersonalTable entries: " + personalFields.Length + "\n" +
                    "EvolveTable entries: " + evolveFields.Length + "??");

            for (int personalID = 0; personalID < personalFields.Length; personalID++)
            {
                Pokemon pokemon = new()
                {
                    validFlag = personalFields[personalID].ValidFlag,
                    personalID = personalFields[personalID].ID,
                    dexID = personalFields[personalID].Monsno,
                    formIndex = personalFields[personalID].FormIndex,
                    formMax = personalFields[personalID].FormMax,
                    color = personalFields[personalID].Color,
                    graNo = personalFields[personalID].GraNo,
                    basicHp = personalFields[personalID].BasicHP,
                    basicAtk = personalFields[personalID].BasicAtk,
                    basicDef = personalFields[personalID].BasicDef,
                    basicSpd = personalFields[personalID].BasicAgi,
                    basicSpAtk = personalFields[personalID].BasicSpatk,
                    basicSpDef = personalFields[personalID].BasicSpdef,
                    typingID1 = personalFields[personalID].Type1,
                    typingID2 = personalFields[personalID].Type2,
                    getRate = personalFields[personalID].GetRate,
                    rank = personalFields[personalID].Rank,
                    expValue = personalFields[personalID].ExpValue,
                    item1 = personalFields[personalID].Item1,
                    item2 = personalFields[personalID].Item2,
                    item3 = personalFields[personalID].Item3,
                    sex = personalFields[personalID].Sex,
                    eggBirth = personalFields[personalID].EggBirth,
                    initialFriendship = personalFields[personalID].InitialFriendship,
                    eggGroup1 = personalFields[personalID].EggGroup1,
                    eggGroup2 = personalFields[personalID].EggGroup2,
                    grow = personalFields[personalID].Grow,
                    abilityID1 = personalFields[personalID].Tokusei1,
                    abilityID2 = personalFields[personalID].Tokusei2,
                    abilityID3 = personalFields[personalID].Tokusei3,
                    giveExp = personalFields[personalID].GiveExp,
                    height = personalFields[personalID].Height,
                    weight = personalFields[personalID].Weight,
                    chihouZukanNo = personalFields[personalID].ChihouZukanNo,
                    machine1 = personalFields[personalID].Machine1,
                    machine2 = personalFields[personalID].Machine2,
                    machine3 = personalFields[personalID].Machine3,
                    machine4 = personalFields[personalID].Machine4,
                    hiddenMachine = personalFields[personalID].HidenMachine,
                    eggMonsno = personalFields[personalID].EggMonsno,
                    eggFormno = personalFields[personalID].EggFormno,
                    eggFormnoKawarazunoishi = personalFields[personalID].EggFormnoKawarazunoishi,
                    eggFormInheritKawarazunoishi = personalFields[personalID].EggFormInheritKawarazunoishi,
                };

                pokemon.formID = 0;
                if (pokemon.personalID != pokemon.dexID)
                    pokemon.formID = pokemon.personalID - pokemon.formIndex + 1;

                pokemon.name = "";
                if (textFields[pokemon.dexID].WordDataArray.Length > 0)
                    pokemon.name = textFields[pokemon.dexID].WordDataArray[0].Str;

                pokemon.nextEvoLvs = (ushort.MaxValue, ushort.MaxValue); //(wildLevel, trainerLevel)
                pokemon.pastPokemon = new();
                pokemon.nextPokemon = new();
                pokemon.inferiorForms = new();
                pokemon.superiorForms = new();

                //Parse level up moves
                pokemon.levelUpMoves = new();
                for (int levelUpMoveIdx = 0; levelUpMoveIdx < levelUpMoveFields[personalID].Ar.Length; levelUpMoveIdx += 2)
                {
                    LevelUpMove levelUpMove = new()
                    {
                        level = levelUpMoveFields[personalID].Ar[levelUpMoveIdx],
                        moveID = levelUpMoveFields[personalID].Ar[levelUpMoveIdx + 1],
                    };

                    pokemon.levelUpMoves.Add(levelUpMove);
                }

                //Parse egg moves
                pokemon.eggMoves = new();
                for (int eggMoveIdx = 0; eggMoveIdx < eggMoveFields[personalID].WazaNo.Length; eggMoveIdx++)
                    pokemon.eggMoves.Add(eggMoveFields[personalID].WazaNo[eggMoveIdx]);

                //Parse evolutions
                pokemon.evolutionPaths = new();
                for (int evolutionIdx = 0; evolutionIdx < evolveFields[personalID].Ar.Length; evolutionIdx += 5)
                {
                    EvolutionPath evolution = new()
                    {
                        method = evolveFields[personalID].Ar[evolutionIdx],
                        parameter = evolveFields[personalID].Ar[evolutionIdx + 1],
                        destDexID = evolveFields[personalID].Ar[evolutionIdx + 2],
                        destFormID = evolveFields[personalID].Ar[evolutionIdx + 3],
                        level = evolveFields[personalID].Ar[evolutionIdx + 4]
                    };

                    pokemon.evolutionPaths.Add(evolution);
                }

                pokemon.externalTMLearnset = fileManager.TryGetExternalJson<TMLearnset>(
                    $"MonData\\TMLearnset\\monsno_{pokemon.dexID}_formno_{pokemon.formID}.json");

                gameData.personalEntries.Add(pokemon);

                if (gameData.dexEntries.Count == pokemon.dexID)
                {
                    gameData.dexEntries.Add(new());
                    gameData.dexEntries[pokemon.dexID].dexID = pokemon.dexID;
                    gameData.dexEntries[pokemon.dexID].forms = new();
                    gameData.dexEntries[pokemon.dexID].name = pokemon.name;
                }

                gameData.dexEntries[pokemon.dexID].forms.Add(pokemon);
            }

            SetFamilies();
            SetLegendariesYAML();
        }

        private static async void SetLegendaries()
        {
            AssetTypeValueField[] legendFields = (await monoBehaviourCollection[PathEnum.Gamesettings]).Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "FieldEncountTable_d").children[10].children[0].children;
            for (int legendEntryIdx = 0; legendEntryIdx < legendFields.Length; legendEntryIdx++)
            {
                List<Pokemon> forms = gameData.dexEntries[legendFields[legendEntryIdx].children[0].value.value.asInt32].forms;
                for (int formID = 0; formID < forms.Count; formID++)
                    forms[formID].legendary = true;
            }
        }

        private static async void SetLegendariesYAML()
        {
            var legendFields = ((await yamlCollection[PathEnum.Gamesettings]).Find(y => y.MonoBehaviour?.Name == "FieldEncountTable_d").MonoBehaviour as FieldEncountTable).LegendPoke;
            for (int legendEntryIdx = 0; legendEntryIdx < legendFields.Length; legendEntryIdx++)
            {
                List<Pokemon> forms = gameData.dexEntries[legendFields[legendEntryIdx].MonsNo].forms;
                for (int formID = 0; formID < forms.Count; formID++)
                    forms[formID].legendary = true;
            }
        }

        /// <summary>
        ///  Overwrites and updates all pokemons' evolution info for easier BST logic.
        /// </summary>
        public static void SetFamilies()
        {
            for (int dexID = 0; dexID < gameData.dexEntries.Count; dexID++)
            {
                for (int formID = 0; formID < gameData.dexEntries[dexID].forms.Count; formID++)
                {
                    Pokemon pokemon = gameData.dexEntries[dexID].forms[formID];
                    for (int evolutionIdx = 0; evolutionIdx < gameData.dexEntries[dexID].forms[formID].evolutionPaths.Count; evolutionIdx++)
                    {
                        EvolutionPath evo = pokemon.evolutionPaths[evolutionIdx];
                        Pokemon next = gameData.dexEntries[evo.destDexID].forms[evo.destFormID];

                        if (pokemon.dexID == next.dexID)
                            continue;

                        pokemon.nextPokemon.Add(next);
                        next.pastPokemon.Add(pokemon);
                    }

                    for (int formID2 = 0; formID2 < gameData.dexEntries[dexID].forms.Count; formID2++)
                    {
                        Pokemon pokemon2 = gameData.dexEntries[dexID].forms[formID2];
                        if (pokemon2.GetBST() - pokemon.GetBST() >= 30)
                        {
                            pokemon2.inferiorForms.Add(pokemon);
                            pokemon.superiorForms.Add(pokemon2);
                        }
                    }
                }
            }

            for (int dexID = 0; dexID < gameData.dexEntries.Count; dexID++)
            {
                for (int formID = 0; formID < gameData.dexEntries[dexID].forms.Count; formID++)
                {
                    Pokemon pokemon = gameData.dexEntries[dexID].forms[formID];
                    (ushort, ushort) evoLvs = GetEvoLvs(pokemon);
                    if (evoLvs == (0, 0))
                        continue;

                    pokemon.nextEvoLvs = evoLvs;

                    for (int evolutionIdx = 0; evolutionIdx < gameData.dexEntries[dexID].forms[formID].evolutionPaths.Count; evolutionIdx++)
                    {
                        EvolutionPath evo = pokemon.evolutionPaths[evolutionIdx];
                        Pokemon next = gameData.dexEntries[evo.destDexID].forms[evo.destFormID];

                        if (pokemon.dexID == next.dexID)
                            continue;

                        next.pastEvoLvs.Item1 = Math.Max(next.pastEvoLvs.Item1, evoLvs.Item1);
                        next.pastEvoLvs.Item2 = Math.Max(next.pastEvoLvs.Item2, evoLvs.Item2);
                    }
                }
            }
        }

        /// <summary>
        ///  Finds the levels the specified pokemon is likely to evolve: (wildLevel, trainerLevel).
        /// </summary>
        private static (ushort, ushort) GetEvoLvs(Pokemon pokemon)
        {
            (ushort, ushort) evoLvs = (0, 0);
            for (int evolutionIdx = 0; evolutionIdx < pokemon.evolutionPaths.Count; evolutionIdx++)
            {
                EvolutionPath evo = pokemon.evolutionPaths[evolutionIdx];
                if (pokemon.dexID == evo.destDexID)
                    continue;

                switch (evo.method)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 21:
                    case 29:
                    case 43:
                        for (int pastEvos = 0; pastEvos < pokemon.pastPokemon.Count; pastEvos++)
                        {
                            (ushort, ushort) pastPokemonEvoLvs = GetEvoLvs(pokemon.pastPokemon[pastEvos]);
                            evoLvs.Item1 = Math.Max(evoLvs.Item1, pastPokemonEvoLvs.Item1);
                            evoLvs.Item2 = Math.Max(evoLvs.Item2, pastPokemonEvoLvs.Item2);
                        }
                        if (evoLvs == (0, 0))
                            evoLvs = (1, 1);
                        evoLvs.Item1 += 16;
                        evoLvs.Item2 += 16;
                        break;
                    case 4:
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                    case 23:
                    case 24:
                    case 28:
                    case 32:
                    case 33:
                    case 34:
                    case 36:
                    case 37:
                    case 38:
                    case 40:
                    case 41:
                    case 46:
                    case 47:
                        evoLvs.Item1 = evo.level;
                        evoLvs.Item2 = evo.level;
                        break;
                    case 5:
                    case 8:
                    case 17:
                    case 18:
                    case 22:
                    case 25:
                    case 26:
                    case 27:
                    case 39:
                    case 42:
                    case 44:
                    case 45:
                        for (int pastEvos = 0; pastEvos < pokemon.pastPokemon.Count; pastEvos++)
                        {
                            (ushort, ushort) pastPokemonEvoLvs = GetEvoLvs(pokemon.pastPokemon[pastEvos]);
                            evoLvs.Item1 = Math.Max(evoLvs.Item1, pastPokemonEvoLvs.Item1);
                            evoLvs.Item2 = Math.Max(evoLvs.Item2, pastPokemonEvoLvs.Item2);
                        }
                        if (evoLvs == (0, 0))
                            evoLvs = (1, 1);
                        evoLvs.Item1 += 32;
                        evoLvs.Item2 += 16;
                        break;
                    case 6:
                    case 7:
                        for (int pastEvos = 0; pastEvos < pokemon.pastPokemon.Count; pastEvos++)
                        {
                            (ushort, ushort) pastPokemonEvoLvs = GetEvoLvs(pokemon.pastPokemon[pastEvos]);
                            evoLvs.Item1 = Math.Max(evoLvs.Item1, pastPokemonEvoLvs.Item1);
                            evoLvs.Item2 = Math.Max(evoLvs.Item2, pastPokemonEvoLvs.Item2);
                        }
                        if (evoLvs == (0, 0))
                            evoLvs = (1, 1);
                        evoLvs.Item1 += 48;
                        evoLvs.Item2 += 16;
                        break;
                    case 16:
                        for (int pastEvos = 0; pastEvos < pokemon.pastPokemon.Count; pastEvos++)
                        {
                            (ushort, ushort) pastPokemonEvoLvs = GetEvoLvs(pokemon.pastPokemon[pastEvos]);
                            evoLvs.Item1 = Math.Max(evoLvs.Item1, pastPokemonEvoLvs.Item1);
                            evoLvs.Item2 = Math.Max(evoLvs.Item2, pastPokemonEvoLvs.Item2);
                        }
                        if (evoLvs == (0, 0))
                            evoLvs = (1, 1);
                        evoLvs.Item1 += 48;
                        evoLvs.Item2 += 32;
                        break;
                    case 19:
                    case 20:
                    case 30:
                    case 31:
                        evoLvs.Item1 = (ushort)(evo.level + 16);
                        evoLvs.Item2 = evo.level;
                        break;
                }
            }
            return evoLvs;
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed TMs.
        /// </summary>
        private static async Task ParseTMs()
        {
            gameData.tms = new();
            AssetTypeValueField monoBehaviour = (await monoBehaviourCollection[PathEnum.PersonalMasterdatas]).Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "ItemTable");

            AssetTypeValueField[] tmFields = monoBehaviour.children[5].children[0].children;
            AssetTypeValueField[] textFields = await FindLabelArrayOfMessageFileAsync("ss_itemname", Language.English);
            for (int tmID = 0; tmID < tmFields.Length; tmID++)
            {
                TM tm = new();
                tm.itemID = tmFields[tmID].children[0].value.value.asInt32;
                tm.machineNo = tmFields[tmID].children[1].value.value.asInt32;
                tm.moveID = tmFields[tmID].children[2].value.value.asInt32;

                tm.tmID = tmID;
                tm.name = "";
                if (textFields[tm.itemID].children[6].children[0].childrenCount > 0)
                    tm.name = Encoding.UTF8.GetString(textFields[tm.itemID].children[6].children[0].children[0].children[4].value.value.asString);

                gameData.tms.Add(tm);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed yaml TMs.
        /// </summary>
        private static async Task ParseTMsYAML()
        {
            gameData.tms = new();
            var monoBehaviour = (await yamlCollection[PathEnum.PersonalMasterdatas]).Find(y => y.MonoBehaviour?.Name == "ItemTable").MonoBehaviour as ItemTable;

            var tmFields = monoBehaviour.WazaMachine;
            var textFields = await FindLabelArrayOfYAMLMessageFileAsync("ss_itemname", Language.English);
            for (int tmID = 0; tmID < tmFields.Length; tmID++)
            {
                TM tm = new()
                {
                    itemID = tmFields[tmID].ItemNo,
                    machineNo = tmFields[tmID].MachineNo,
                    moveID = tmFields[tmID].WazaNo,
                    tmID = tmID,
                };

                tm.name = "";
                if (textFields[tm.itemID].WordDataArray.Length > 0)
                    tm.name = textFields[tm.itemID].WordDataArray[0].Str;

                gameData.tms.Add(tm);
            }
        }
        
        private static async Task ParsePersonalMasterDatas()
        {
            gameData.addPersonalTables = new();
            AssetTypeValueField addPersonalTable = (await monoBehaviourCollection[PathEnum.PersonalMasterdatas]).Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "AddPersonalTable");
            AssetTypeValueField[] addPersonalTableArray = addPersonalTable["AddPersonal"].children[0].children;
            for (int i = 0; i < addPersonalTableArray.Length; i++)
            {
                PersonalMasterdatas.AddPersonalTable addPersonal = new();
                addPersonal.valid_flag = addPersonalTableArray[i]["valid_flag"].value.value.asUInt8 != 0;
                addPersonal.monsno = addPersonalTableArray[i]["monsno"].value.value.asUInt16;
                addPersonal.formno = addPersonalTableArray[i]["formno"].value.value.asUInt16;
                addPersonal.isEnableSynchronize = addPersonalTableArray[i]["isEnableSynchronize"].value.value.asUInt8 != 0;
                addPersonal.escape = addPersonalTableArray[i]["escape"].value.value.asUInt8;
                addPersonal.isDisableReverce = addPersonalTableArray[i]["isDisableReverce"].value.value.asUInt8 != 0;
                gameData.addPersonalTables.Add(addPersonal);
            }
        }
        
        private static async Task ParsePersonalMasterDatasYAML()
        {
            gameData.addPersonalTables = new();
            var addPersonalTableArray = ((await yamlCollection[PathEnum.PersonalMasterdatas]).Find(y => y.MonoBehaviour?.Name == "AddPersonalTable").MonoBehaviour as AddPersonalTable).AddPersonal;
            for (int i = 0; i < addPersonalTableArray.Length; i++)
            {
                PersonalMasterdatas.AddPersonalTable addPersonal = new()
                {
                    valid_flag = addPersonalTableArray[i].ValidFlag != 0,
                    monsno = addPersonalTableArray[i].Monsno,
                    formno = addPersonalTableArray[i].Formno,
                    isEnableSynchronize = addPersonalTableArray[i].IsEnableSynchronize != 0,
                    escape = addPersonalTableArray[i].Escape,
                    isDisableReverce = addPersonalTableArray[i].IsDisableReverce != 0,
                };

                gameData.addPersonalTables.Add(addPersonal);
            }
        }

        private static async Task ParseUIMasterDatas()
        {
            gameData.uiPokemonIcon = new();
            gameData.uiAshiatoIcon = new();
            gameData.uiPokemonVoice = new();
            gameData.uiZukanDisplay = new();
            gameData.uiZukanCompareHeights = new();
            gameData.uiSearchPokeIconSex = new();
            gameData.uiDistributionTable = new();
            AssetTypeValueField uiDatabase = (await monoBehaviourCollection[PathEnum.UIMasterdatas]).Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "UIDatabase");
            AssetTypeValueField distributionTable = (await monoBehaviourCollection[PathEnum.UIMasterdatas]).Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "DistributionTable");

            AssetTypeValueField[] pokemonIcons = uiDatabase["PokemonIcon"].children[0].children;
            for (int i = 0; i < pokemonIcons.Length; i++)
            {
                UIMasterdatas.PokemonIcon pokemonIcon = new();
                pokemonIcon.uniqueID = pokemonIcons[i]["UniqueID"].value.value.asInt32;
                pokemonIcon.assetBundleName = pokemonIcons[i]["AssetBundleName"].GetValue().AsString();
                pokemonIcon.assetName = pokemonIcons[i]["AssetName"].GetValue().AsString();
                pokemonIcon.assetBundleNameLarge = pokemonIcons[i]["AssetBundleNameLarge"].GetValue().AsString();
                pokemonIcon.assetNameLarge = pokemonIcons[i]["AssetNameLarge"].GetValue().AsString();
                pokemonIcon.assetBundleNameDP = pokemonIcons[i]["AssetBundleNameDP"].GetValue().AsString();
                pokemonIcon.assetNameDP = pokemonIcons[i]["AssetNameDP"].GetValue().AsString();
                pokemonIcon.hallofFameOffset = new();
                pokemonIcon.hallofFameOffset.X = pokemonIcons[i]["HallofFameOffset"].children[0].value.value.asFloat;
                pokemonIcon.hallofFameOffset.Y = pokemonIcons[i]["HallofFameOffset"].children[1].value.value.asFloat;

                gameData.uiPokemonIcon.Add(pokemonIcon);
            }

            AssetTypeValueField[] ashiatoIcons = uiDatabase["AshiatoIcon"].children[0].children;
            for (int i = 0; i < ashiatoIcons.Length; i++)
            {
                UIMasterdatas.AshiatoIcon ashiatoIcon = new();
                ashiatoIcon.uniqueID = ashiatoIcons[i]["UniqueID"].value.value.asInt32;
                ashiatoIcon.sideIconAssetName = ashiatoIcons[i]["SideIconAssetName"].GetValue().AsString();
                ashiatoIcon.bothIconAssetName = ashiatoIcons[i]["BothIconAssetName"].GetValue().AsString();

                gameData.uiAshiatoIcon.Add(ashiatoIcon);
            }

            AssetTypeValueField[] pokemonVoices = uiDatabase["PokemonVoice"].children[0].children;
            for (int i = 0; i < pokemonVoices.Length; i++)
            {
                UIMasterdatas.PokemonVoice pokemonVoice = new();
                pokemonVoice.uniqueID = pokemonVoices[i]["UniqueID"].value.value.asInt32;
                pokemonVoice.wwiseEvent = pokemonVoices[i]["WwiseEvent"].GetValue().AsString();
                pokemonVoice.stopEventId = pokemonVoices[i]["stopEventId"].GetValue().AsString();
                pokemonVoice.centerPointOffset = new();
                pokemonVoice.centerPointOffset.X = pokemonVoices[i]["CenterPointOffset"].children[0].value.value.asFloat;
                pokemonVoice.centerPointOffset.Y = pokemonVoices[i]["CenterPointOffset"].children[1].value.value.asFloat;
                pokemonVoice.centerPointOffset.Z = pokemonVoices[i]["CenterPointOffset"].children[2].value.value.asFloat;
                pokemonVoice.rotationLimits = pokemonVoices[i]["RotationLimits"].value.value.asUInt8 == 0;
                pokemonVoice.rotationLimitAngle = new();
                pokemonVoice.rotationLimitAngle.X = pokemonVoices[i]["RotationLimitAngle"].children[0].value.value.asFloat;
                pokemonVoice.rotationLimitAngle.Y = pokemonVoices[i]["RotationLimitAngle"].children[1].value.value.asFloat;

                gameData.uiPokemonVoice.Add(pokemonVoice);
            }

            AssetTypeValueField[] zukanDisplays = uiDatabase["ZukanDisplay"].children[0].children;
            for (int i = 0; i < zukanDisplays.Length; i++)
            {
                UIMasterdatas.ZukanDisplay zukanDisplay = new();
                zukanDisplay.uniqueID = zukanDisplays[i]["UniqueID"].value.value.asInt32;

                zukanDisplay.moveLimit = new();
                zukanDisplay.moveLimit.X = zukanDisplays[i]["MoveLimit"].children[0].value.value.asFloat;
                zukanDisplay.moveLimit.Y = zukanDisplays[i]["MoveLimit"].children[1].value.value.asFloat;
                zukanDisplay.moveLimit.Z = zukanDisplays[i]["MoveLimit"].children[2].value.value.asFloat;

                zukanDisplay.modelOffset = new();
                zukanDisplay.modelOffset.X = zukanDisplays[i]["ModelOffset"].children[0].value.value.asFloat;
                zukanDisplay.modelOffset.Y = zukanDisplays[i]["ModelOffset"].children[1].value.value.asFloat;
                zukanDisplay.modelOffset.Z = zukanDisplays[i]["ModelOffset"].children[2].value.value.asFloat;

                zukanDisplay.modelRotationAngle = new();
                zukanDisplay.modelRotationAngle.X = zukanDisplays[i]["ModelRotationAngle"].children[0].value.value.asFloat;
                zukanDisplay.modelRotationAngle.Y = zukanDisplays[i]["ModelRotationAngle"].children[1].value.value.asFloat;

                gameData.uiZukanDisplay.Add(zukanDisplay);
            }

            AssetTypeValueField[] zukanCompareHeights = uiDatabase["ZukanCompareHeight"].children[0].children;
            for (int i = 0; i < zukanCompareHeights.Length; i++)
            {
                UIMasterdatas.ZukanCompareHeight zukanCompareHeight = new();
                zukanCompareHeight.uniqueID = zukanCompareHeights[i]["UniqueID"].value.value.asInt32;

                zukanCompareHeight.playerScaleFactor = zukanCompareHeights[i]["PlayerScaleFactor"].value.value.asFloat;
                zukanCompareHeight.playerOffset = new();
                zukanCompareHeight.playerOffset.X = zukanCompareHeights[i]["PlayerOffset"].children[0].value.value.asFloat;
                zukanCompareHeight.playerOffset.Y = zukanCompareHeights[i]["PlayerOffset"].children[1].value.value.asFloat;
                zukanCompareHeight.playerOffset.Z = zukanCompareHeights[i]["PlayerOffset"].children[2].value.value.asFloat;

                zukanCompareHeight.playerRotationAngle = new();
                zukanCompareHeight.playerRotationAngle.X = zukanCompareHeights[i]["PlayerRotationAngle"].children[0].value.value.asFloat;
                zukanCompareHeight.playerRotationAngle.Y = zukanCompareHeights[i]["PlayerRotationAngle"].children[1].value.value.asFloat;

                gameData.uiZukanCompareHeights.Add(zukanCompareHeight);
            }

            AssetTypeValueField[] searchPokeIconSexes = uiDatabase["SearchPokeIconSex"].children[0].children;
            for (int i = 0; i < searchPokeIconSexes.Length; i++)
            {
                UIMasterdatas.SearchPokeIconSex searchPokeIconSex = new();
                searchPokeIconSex.monsNo = searchPokeIconSexes[i]["MonsNo"].value.value.asInt32;
                searchPokeIconSex.sex = searchPokeIconSexes[i]["Sex"].value.value.asInt32;

                gameData.uiSearchPokeIconSex.Add(searchPokeIconSex);
            }

            AssetTypeValueField[] diamondField = distributionTable["Diamond_FieldTable"].children[0].children;
            gameData.uiDistributionTable.diamondFieldTable = ParseDistributionSheet(diamondField);
            AssetTypeValueField[] diamondDungeon = distributionTable["Diamond_DungeonTable"].children[0].children;
            gameData.uiDistributionTable.diamondDungeonTable = ParseDistributionSheet(diamondDungeon);
            AssetTypeValueField[] pearlField = distributionTable["Pearl_FieldTable"].children[0].children;
            gameData.uiDistributionTable.pearlFieldTable = ParseDistributionSheet(pearlField);
            AssetTypeValueField[] pearlDungeon = distributionTable["Pearl_DungeonTable"].children[0].children;
            gameData.uiDistributionTable.pearlDungeonTable = ParseDistributionSheet(pearlDungeon);
        }

        private static async Task ParseUIMasterDatasYAML()
        {
            gameData.uiPokemonIcon = new();
            gameData.uiAshiatoIcon = new();
            gameData.uiPokemonVoice = new();
            gameData.uiZukanDisplay = new();
            gameData.uiZukanCompareHeights = new();
            gameData.uiSearchPokeIconSex = new();
            gameData.uiDistributionTable = new();

            var uiDatabase = (await yamlCollection[PathEnum.UIMasterdatas]).Find(y => y.MonoBehaviour?.Name == "UIDatabase").MonoBehaviour as UIDatabase;
            var distributionTable = (await yamlCollection[PathEnum.UIMasterdatas]).Find(y => y.MonoBehaviour?.Name == "DistributionTable").MonoBehaviour as DistributionTable;

            var pokemonIcons = uiDatabase.PokemonIcon;
            for (int i = 0; i < pokemonIcons.Length; i++)
            {
                UIMasterdatas.PokemonIcon pokemonIcon = new()
                {
                    uniqueID = pokemonIcons[i].UniqueID,
                    assetBundleName = pokemonIcons[i].AssetBundleName,
                    assetName = pokemonIcons[i].AssetName,
                    assetBundleNameLarge = pokemonIcons[i].AssetBundleNameLarge,
                    assetNameLarge = pokemonIcons[i].AssetNameLarge,
                    assetBundleNameDP = pokemonIcons[i].AssetBundleNameDP,
                    assetNameDP = pokemonIcons[i].AssetNameDP,
                    hallofFameOffset = new()
                    {
                        X = pokemonIcons[i].HallofFameOffset.X,
                        Y = pokemonIcons[i].HallofFameOffset.Y,
                    }
                };

                gameData.uiPokemonIcon.Add(pokemonIcon);
            }

            var ashiatoIcons = uiDatabase.AshiatoIcon;
            for (int i = 0; i < ashiatoIcons.Length; i++)
            {
                UIMasterdatas.AshiatoIcon ashiatoIcon = new()
                {
                    uniqueID = ashiatoIcons[i].UniqueID,
                    sideIconAssetName = ashiatoIcons[i].SideIconAssetName,
                    bothIconAssetName = ashiatoIcons[i].BothIconAssetName,
                };

                gameData.uiAshiatoIcon.Add(ashiatoIcon);
            }

            var pokemonVoices = uiDatabase.PokemonVoice;
            for (int i = 0; i < pokemonVoices.Length; i++)
            {
                UIMasterdatas.PokemonVoice pokemonVoice = new()
                {
                    uniqueID = pokemonVoices[i].UniqueID,
                    wwiseEvent = pokemonVoices[i].WwiseEvent,
                    stopEventId = pokemonVoices[i].StopEventId,
                    centerPointOffset = new()
                    {
                        X = pokemonVoices[i].CenterPointOffset.X,
                        Y = pokemonVoices[i].CenterPointOffset.Y,
                        Z = pokemonVoices[i].CenterPointOffset.Z,
                    },
                    rotationLimits = pokemonVoices[i].RotationLimits == 0,
                    rotationLimitAngle = new()
                    {
                        X = pokemonVoices[i].RotationLimitAngle.X,
                        Y = pokemonVoices[i].RotationLimitAngle.Y,
                    }
                };

                gameData.uiPokemonVoice.Add(pokemonVoice);
            }

            var zukanDisplays = uiDatabase.ZukanDisplay;
            for (int i = 0; i < zukanDisplays.Length; i++)
            {
                UIMasterdatas.ZukanDisplay zukanDisplay = new()
                {
                    uniqueID = zukanDisplays[i].UniqueID,
                    moveLimit = new()
                    {
                        X = zukanDisplays[i].MoveLimit.X,
                        Y = zukanDisplays[i].MoveLimit.Y,
                        Z = zukanDisplays[i].MoveLimit.Z,
                    },
                    modelOffset = new()
                    {
                        X = zukanDisplays[i].ModelOffset.X,
                        Y = zukanDisplays[i].ModelOffset.Y,
                        Z = zukanDisplays[i].ModelOffset.Z,
                    },
                    modelRotationAngle = new()
                    {
                        X = zukanDisplays[i].ModelRotationAngle.X,
                        Y = zukanDisplays[i].ModelRotationAngle.Y,
                    }
                };

                gameData.uiZukanDisplay.Add(zukanDisplay);
            }

            var zukanCompareHeights = uiDatabase.ZukanCompareHeight;
            for (int i = 0; i < zukanCompareHeights.Length; i++)
            {
                UIMasterdatas.ZukanCompareHeight zukanCompareHeight = new()
                {
                    uniqueID = zukanCompareHeights[i].UniqueID,
                    playerScaleFactor = zukanCompareHeights[i].PlayerScaleFactor,
                    playerOffset = new()
                    {
                        X = zukanCompareHeights[i].PlayerOffset.X,
                        Y = zukanCompareHeights[i].PlayerOffset.Y,
                        Z = zukanCompareHeights[i].PlayerOffset.Z,
                    },
                    playerRotationAngle = new()
                    {
                        X = zukanCompareHeights[i].PlayerRotationAngle.X,
                        Y = zukanCompareHeights[i].PlayerRotationAngle.Y,
                    }
                };

                gameData.uiZukanCompareHeights.Add(zukanCompareHeight);
            }

            var searchPokeIconSexes = uiDatabase.SearchPokeIconSex;
            for (int i = 0; i < searchPokeIconSexes.Length; i++)
            {
                UIMasterdatas.SearchPokeIconSex searchPokeIconSex = new()
                {
                    monsNo = searchPokeIconSexes[i].MonsNo,
                    sex = searchPokeIconSexes[i].Sex,
                };

                gameData.uiSearchPokeIconSex.Add(searchPokeIconSex);
            }

            gameData.uiDistributionTable.diamondFieldTable = ParseDistributionSheetYAML(distributionTable.DiamondFieldTable);
            gameData.uiDistributionTable.diamondDungeonTable = ParseDistributionSheetYAML(distributionTable.DiamondDungeonTable);
            gameData.uiDistributionTable.pearlFieldTable = ParseDistributionSheetYAML(distributionTable.PearlFieldTable);
            gameData.uiDistributionTable.pearlDungeonTable = ParseDistributionSheetYAML(distributionTable.PearlDungeonTable);
        }

        private static List<UIMasterdatas.DistributionEntry> ParseDistributionSheet(AssetTypeValueField[] sheetATVF)
        {
            List<UIMasterdatas.DistributionEntry> sheet = new();
            for (int i = 0; i < sheetATVF.Length; i++)
            {
                UIMasterdatas.DistributionEntry entry = new()
                {
                    beforeMorning = ParseDistributionCoord(sheetATVF[i]["BeforeMorning"]),
                    beforeDaytime = ParseDistributionCoord(sheetATVF[i]["BeforeDaytime"]),
                    beforeNight = ParseDistributionCoord(sheetATVF[i]["BeforeNight"]),
                    afterMorning = ParseDistributionCoord(sheetATVF[i]["AfterMorning"]),
                    afterDaytime = ParseDistributionCoord(sheetATVF[i]["AfterDaytime"]),
                    afterNight = ParseDistributionCoord(sheetATVF[i]["AfterNight"]),
                    fishing = ParseDistributionCoord(sheetATVF[i]["Fishing"]),
                    pokemonTraser = ParseDistributionCoord(sheetATVF[i]["PokemonTraser"]),
                    honeyTree = ParseDistributionCoord(sheetATVF[i]["HoneyTree"])
                };
                sheet.Add(entry);
            }
            return sheet;
        }

        private static List<UIMasterdatas.DistributionEntry> ParseDistributionSheetYAML(SheetDistributionTable[] sheetTable)
        {
            List<UIMasterdatas.DistributionEntry> sheet = new();
            for (int i = 0; i < sheetTable.Length; i++)
            {
                UIMasterdatas.DistributionEntry entry = new()
                {
                    beforeMorning = sheetTable[i].BeforeMorning,
                    beforeDaytime = sheetTable[i].BeforeDaytime,
                    beforeNight = sheetTable[i].BeforeNight,
                    afterMorning = sheetTable[i].AfterMorning,
                    afterDaytime = sheetTable[i].AfterDaytime,
                    afterNight = sheetTable[i].AfterNight,
                    fishing = sheetTable[i].Fishing,
                    pokemonTraser = sheetTable[i].PokemonTraser,
                    honeyTree = sheetTable[i].HoneyTree,
                };
                sheet.Add(entry);
            }
            return sheet;
        }

        private static int[] ParseDistributionCoord(AssetTypeValueField posATVF) => posATVF[0].children.Select(a => a.GetValue().AsInt()).ToArray();

        private static async Task ParseMasterDatas()
        {
            gameData.pokemonInfos = new();
            AssetTypeValueField pokemonInfo = (await monoBehaviourCollection[PathEnum.DprMasterdatas]).Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "PokemonInfo");
            AssetTypeValueField[] catalogArray = pokemonInfo["Catalog"].children[0].children;
            AssetTypeValueField[] trearukiArray = null;
            if (pokemonInfo["Trearuki"].children != null)
                trearukiArray = pokemonInfo["Trearuki"].children[0].children;
            //pre-1.3.0 versions don't have this field

            for (int i = 0; i < catalogArray.Length; i++)
            {
                Masterdatas.PokemonInfoCatalog catalog = new();
                catalog.UniqueID = catalogArray[i]["UniqueID"].value.value.asInt32;
                catalog.No = catalogArray[i]["No"].value.value.asInt32;
                catalog.SinnohNo = catalogArray[i]["SinnohNo"].value.value.asInt32;
                catalog.MonsNo = catalogArray[i]["MonsNo"].value.value.asInt32;
                catalog.FormNo = catalogArray[i]["FormNo"].value.value.asInt32;
                catalog.Sex = catalogArray[i]["Sex"].value.value.asUInt8;
                catalog.Rare = catalogArray[i]["Rare"].value.value.asUInt8 == 1;
                catalog.AssetBundleName = catalogArray[i]["AssetBundleName"].GetValue().AsString();
                catalog.BattleScale = catalogArray[i]["BattleScale"].value.value.asFloat;
                catalog.ContestScale = catalogArray[i]["ContestScale"].value.value.asFloat;
                catalog.ContestSize = (Masterdatas.Size)catalogArray[i]["ContestSize"].value.value.asInt32;
                catalog.FieldScale = catalogArray[i]["FieldScale"].value.value.asFloat;
                catalog.FieldChikaScale = catalogArray[i]["FieldChikaScale"].value.value.asFloat;
                catalog.StatueScale = catalogArray[i]["StatueScale"].value.value.asFloat;
                catalog.FieldWalkingScale = catalogArray[i]["FieldWalkingScale"].value.value.asFloat;
                catalog.FieldFureaiScale = catalogArray[i]["FieldFureaiScale"].value.value.asFloat;
                catalog.MenuScale = catalogArray[i]["MenuScale"].value.value.asFloat;
                catalog.ModelMotion = catalogArray[i]["ModelMotion"].GetValue().AsString();

                catalog.ModelOffset = new();
                catalog.ModelOffset.X = catalogArray[i]["ModelOffset"].children[0].value.value.asFloat;
                catalog.ModelOffset.Y = catalogArray[i]["ModelOffset"].children[1].value.value.asFloat;
                catalog.ModelOffset.Z = catalogArray[i]["ModelOffset"].children[2].value.value.asFloat;

                catalog.ModelRotationAngle = new();
                catalog.ModelRotationAngle.X = catalogArray[i]["ModelRotationAngle"].children[0].value.value.asFloat;
                catalog.ModelRotationAngle.Y = catalogArray[i]["ModelRotationAngle"].children[1].value.value.asFloat;
                catalog.ModelRotationAngle.Z = catalogArray[i]["ModelRotationAngle"].children[2].value.value.asFloat;

                catalog.DistributionScale = catalogArray[i]["DistributionScale"].value.value.asFloat;
                catalog.DistributionModelMotion = catalogArray[i]["DistributionModelMotion"].GetValue().AsString();

                catalog.DistributionModelOffset = new();
                catalog.DistributionModelOffset.X = catalogArray[i]["DistributionModelOffset"].children[0].value.value.asFloat;
                catalog.DistributionModelOffset.Y = catalogArray[i]["DistributionModelOffset"].children[1].value.value.asFloat;
                catalog.DistributionModelOffset.Z = catalogArray[i]["DistributionModelOffset"].children[2].value.value.asFloat;

                catalog.DistributionModelRotationAngle = new();
                catalog.DistributionModelRotationAngle.X = catalogArray[i]["DistributionModelRotationAngle"].children[0].value.value.asFloat;
                catalog.DistributionModelRotationAngle.Y = catalogArray[i]["DistributionModelRotationAngle"].children[1].value.value.asFloat;
                catalog.DistributionModelRotationAngle.Z = catalogArray[i]["DistributionModelRotationAngle"].children[2].value.value.asFloat;

                catalog.VoiceScale = catalogArray[i]["VoiceScale"].value.value.asFloat;
                catalog.VoiceModelMotion = catalogArray[i]["VoiceModelMotion"].GetValue().AsString();

                catalog.VoiceModelOffset = new();
                catalog.VoiceModelOffset.X = catalogArray[i]["VoiceModelOffset"].children[0].value.value.asFloat;
                catalog.VoiceModelOffset.Y = catalogArray[i]["VoiceModelOffset"].children[1].value.value.asFloat;
                catalog.VoiceModelOffset.Z = catalogArray[i]["VoiceModelOffset"].children[2].value.value.asFloat;

                catalog.VoiceModelRotationAngle = new();
                catalog.VoiceModelRotationAngle.X = catalogArray[i]["VoiceModelRotationAngle"].children[0].value.value.asFloat;
                catalog.VoiceModelRotationAngle.Y = catalogArray[i]["VoiceModelRotationAngle"].children[1].value.value.asFloat;
                catalog.VoiceModelRotationAngle.Z = catalogArray[i]["VoiceModelRotationAngle"].children[2].value.value.asFloat;

                catalog.CenterPointOffset = new();
                catalog.CenterPointOffset.X = catalogArray[i]["CenterPointOffset"].children[0].value.value.asFloat;
                catalog.CenterPointOffset.Y = catalogArray[i]["CenterPointOffset"].children[1].value.value.asFloat;
                catalog.CenterPointOffset.Z = catalogArray[i]["CenterPointOffset"].children[2].value.value.asFloat;

                catalog.RotationLimitAngle = new();
                catalog.RotationLimitAngle.X = catalogArray[i]["RotationLimitAngle"].children[0].value.value.asFloat;
                catalog.RotationLimitAngle.Y = catalogArray[i]["RotationLimitAngle"].children[1].value.value.asFloat;

                catalog.StatusScale = catalogArray[i]["StatusScale"].value.value.asFloat;
                catalog.StatusModelMotion = catalogArray[i]["StatusModelMotion"].GetValue().AsString();

                catalog.StatusModelOffset = new();
                catalog.StatusModelOffset.X = catalogArray[i]["StatusModelOffset"].children[0].value.value.asFloat;
                catalog.StatusModelOffset.Y = catalogArray[i]["StatusModelOffset"].children[1].value.value.asFloat;
                catalog.StatusModelOffset.Z = catalogArray[i]["StatusModelOffset"].children[2].value.value.asFloat;

                catalog.StatusModelRotationAngle = new();
                catalog.StatusModelRotationAngle.X = catalogArray[i]["StatusModelRotationAngle"].children[0].value.value.asFloat;
                catalog.StatusModelRotationAngle.Y = catalogArray[i]["StatusModelRotationAngle"].children[1].value.value.asFloat;
                catalog.StatusModelRotationAngle.Z = catalogArray[i]["StatusModelRotationAngle"].children[2].value.value.asFloat;

                catalog.BoxScale = catalogArray[i]["BoxScale"].value.value.asFloat;
                catalog.BoxModelMotion = catalogArray[i]["BoxModelMotion"].GetValue().AsString();

                catalog.BoxModelOffset = new();
                catalog.BoxModelOffset.X = catalogArray[i]["BoxModelOffset"].children[0].value.value.asFloat;
                catalog.BoxModelOffset.Y = catalogArray[i]["BoxModelOffset"].children[1].value.value.asFloat;
                catalog.BoxModelOffset.Z = catalogArray[i]["BoxModelOffset"].children[2].value.value.asFloat;

                catalog.BoxModelRotationAngle = new();
                catalog.BoxModelRotationAngle.X = catalogArray[i]["BoxModelRotationAngle"].children[0].value.value.asFloat;
                catalog.BoxModelRotationAngle.Y = catalogArray[i]["BoxModelRotationAngle"].children[1].value.value.asFloat;
                catalog.BoxModelRotationAngle.Z = catalogArray[i]["BoxModelRotationAngle"].children[2].value.value.asFloat;

                catalog.CompareScale = catalogArray[i]["CompareScale"].value.value.asFloat;
                catalog.CompareModelMotion = catalogArray[i]["CompareModelMotion"].GetValue().AsString();

                catalog.CompareModelOffset = new();
                catalog.CompareModelOffset.X = catalogArray[i]["CompareModelOffset"].children[0].value.value.asFloat;
                catalog.CompareModelOffset.Y = catalogArray[i]["CompareModelOffset"].children[1].value.value.asFloat;
                catalog.CompareModelOffset.Z = catalogArray[i]["CompareModelOffset"].children[2].value.value.asFloat;

                catalog.CompareModelRotationAngle = new();
                catalog.CompareModelRotationAngle.X = catalogArray[i]["CompareModelRotationAngle"].children[0].value.value.asFloat;
                catalog.CompareModelRotationAngle.Y = catalogArray[i]["CompareModelRotationAngle"].children[1].value.value.asFloat;
                catalog.CompareModelRotationAngle.Z = catalogArray[i]["CompareModelRotationAngle"].children[2].value.value.asFloat;

                catalog.BrakeStart = catalogArray[i]["BrakeStart"].value.value.asFloat;
                catalog.BrakeEnd = catalogArray[i]["BrakeEnd"].value.value.asFloat;
                catalog.WalkSpeed = catalogArray[i]["WalkSpeed"].value.value.asFloat;
                catalog.RunSpeed = catalogArray[i]["RunSpeed"].value.value.asFloat;
                catalog.WalkStart = catalogArray[i]["WalkStart"].value.value.asFloat;
                catalog.RunStart = catalogArray[i]["RunStart"].value.value.asFloat;
                catalog.BodySize = catalogArray[i]["BodySize"].value.value.asFloat;
                catalog.AppearLimit = catalogArray[i]["AppearLimit"].value.value.asFloat;
                catalog.MoveType = (Masterdatas.MoveType)catalogArray[i]["MoveType"].value.value.asInt32;

                catalog.GroundEffect = catalogArray[i]["GroundEffect"].value.value.asUInt8 != 0;
                catalog.Waitmoving = catalogArray[i]["Waitmoving"].value.value.asUInt8 != 0;
                catalog.BattleAjustHeight = catalogArray[i]["BattleAjustHeight"].value.value.asInt32;

                if (trearukiArray != null)
                {
                    Masterdatas.Trearuki t = new()
                    {
                        enable = trearukiArray[i]["Enable"].GetValue().value.asUInt8 != 0,
                        animeIndex = new(),
                        animeDuration = new()
                    };
                    catalog.trearuki = t;

                    AssetTypeValueField[] animeIndexATVFS = trearukiArray[i]["AnimeIndex"].children[0].children;
                    foreach (AssetTypeValueField atvf in animeIndexATVFS)
                        t.animeIndex.Add(atvf.GetValue().AsInt());

                    AssetTypeValueField[] animeDurationATVFS = trearukiArray[i]["AnimeDuration"].children[0].children;
                    foreach (AssetTypeValueField atvf in animeDurationATVFS)
                        t.animeDuration.Add(atvf.GetValue().AsFloat());
                }

                gameData.pokemonInfos.Add(catalog);
            }
        }

        private static async Task ParseMasterDatasYAML()
        {
            gameData.pokemonInfos = new();
            var pokemonInfo = (await yamlCollection[PathEnum.DprMasterdatas]).Find(y => y.MonoBehaviour?.Name == "PokemonInfo").MonoBehaviour as PokemonInfo;
            var catalogArray = pokemonInfo.Catalog;
            var trearukiArray = pokemonInfo.Trearuki;

            for (int i = 0; i < catalogArray.Length; i++)
            {
                Masterdatas.PokemonInfoCatalog catalog = new();
                catalog.UniqueID = catalogArray[i].UniqueID;
                catalog.No = catalogArray[i].No;
                catalog.SinnohNo = catalogArray[i].SinnohNo;
                catalog.MonsNo = catalogArray[i].MonsNo;
                catalog.FormNo = catalogArray[i].FormNo;
                catalog.Sex = catalogArray[i].Sex;
                catalog.Rare = catalogArray[i].Rare != 0;
                catalog.AssetBundleName = catalogArray[i].AssetBundleName;
                catalog.BattleScale = catalogArray[i].BattleScale;
                catalog.ContestScale = catalogArray[i].ContestScale;
                catalog.ContestSize = (Masterdatas.Size)catalogArray[i].ContestSize;
                catalog.FieldScale = catalogArray[i].FieldScale;
                catalog.FieldChikaScale = catalogArray[i].FieldChikaScale;
                catalog.StatueScale = catalogArray[i].StatueScale;
                catalog.FieldWalkingScale = catalogArray[i].FieldWalkingScale;
                catalog.FieldFureaiScale = catalogArray[i].FieldFureaiScale;
                catalog.MenuScale = catalogArray[i].MenuScale;
                catalog.ModelMotion = catalogArray[i].ModelMotion;

                catalog.ModelOffset = new()
                {
                    X = catalogArray[i].ModelOffset.X,
                    Y = catalogArray[i].ModelOffset.Y,
                    Z = catalogArray[i].ModelOffset.Z,
                };

                catalog.ModelRotationAngle = new()
                {
                    X = catalogArray[i].ModelRotationAngle.X,
                    Y = catalogArray[i].ModelRotationAngle.Y,
                    Z = catalogArray[i].ModelRotationAngle.Z,
                };

                catalog.DistributionScale = catalogArray[i].DistributionScale;
                catalog.DistributionModelMotion = catalogArray[i].DistributionModelMotion;

                catalog.DistributionModelOffset = new()
                {
                    X = catalogArray[i].DistributionModelOffset.X,
                    Y = catalogArray[i].DistributionModelOffset.Y,
                    Z = catalogArray[i].DistributionModelOffset.Z,
                };

                catalog.DistributionModelRotationAngle = new()
                {
                    X = catalogArray[i].DistributionModelRotationAngle.X,
                    Y = catalogArray[i].DistributionModelRotationAngle.Y,
                    Z = catalogArray[i].DistributionModelRotationAngle.Z
                };

                catalog.VoiceScale = catalogArray[i].VoiceScale;
                catalog.VoiceModelMotion = catalogArray[i].VoiceModelMotion;

                catalog.VoiceModelOffset = new()
                {
                    X = catalogArray[i].VoiceModelOffset.X,
                    Y = catalogArray[i].VoiceModelOffset.Y,
                    Z = catalogArray[i].VoiceModelOffset.Z,
                };

                catalog.VoiceModelRotationAngle = new()
                {
                    X = catalogArray[i].VoiceModelRotationAngle.X,
                    Y = catalogArray[i].VoiceModelRotationAngle.Y,
                    Z = catalogArray[i].VoiceModelRotationAngle.Z,
                };

                catalog.CenterPointOffset = new()
                {
                    X = catalogArray[i].CenterPointOffset.X,
                    Y = catalogArray[i].CenterPointOffset.Y,
                    Z = catalogArray[i].CenterPointOffset.Z,
                };

                catalog.RotationLimitAngle = new()
                {
                    X = catalogArray[i].RotationLimitAngle.X,
                    Y = catalogArray[i].RotationLimitAngle.Y,
                };

                catalog.StatusScale = catalogArray[i].StatusScale;
                catalog.StatusModelMotion = catalogArray[i].StatusModelMotion;

                catalog.StatusModelOffset = new()
                {
                    X = catalogArray[i].StatusModelOffset.X,
                    Y = catalogArray[i].StatusModelOffset.Y,
                    Z = catalogArray[i].StatusModelOffset.Z,
                };

                catalog.StatusModelRotationAngle = new()
                {
                    X = catalogArray[i].StatusModelRotationAngle.X,
                    Y = catalogArray[i].StatusModelRotationAngle.Y,
                    Z = catalogArray[i].StatusModelRotationAngle.Z,
                };

                catalog.BoxScale = catalogArray[i].BoxScale;
                catalog.BoxModelMotion = catalogArray[i].BoxModelMotion;

                catalog.BoxModelOffset = new()
                {
                    X = catalogArray[i].BoxModelOffset.X,
                    Y = catalogArray[i].BoxModelOffset.Y,
                    Z = catalogArray[i].BoxModelOffset.Z,
                };

                catalog.BoxModelRotationAngle = new()
                {
                    X = catalogArray[i].BoxModelRotationAngle.X,
                    Y = catalogArray[i].BoxModelRotationAngle.Y,
                    Z = catalogArray[i].BoxModelRotationAngle.Z,
                };

                catalog.CompareScale = catalogArray[i].CompareScale;
                catalog.CompareModelMotion = catalogArray[i].CompareModelMotion;

                catalog.CompareModelOffset = new()
                {
                    X = catalogArray[i].CompareModelOffset.X,
                    Y = catalogArray[i].CompareModelOffset.Y,
                    Z = catalogArray[i].CompareModelOffset.Z,
                };

                catalog.CompareModelRotationAngle = new()
                {
                    X = catalogArray[i].CompareModelRotationAngle.X,
                    Y = catalogArray[i].CompareModelRotationAngle.Y,
                    Z = catalogArray[i].CompareModelRotationAngle.Z,
                };

                catalog.BrakeStart = catalogArray[i].BrakeStart;
                catalog.BrakeEnd = catalogArray[i].BrakeEnd;
                catalog.WalkSpeed = catalogArray[i].WalkSpeed;
                catalog.RunSpeed = catalogArray[i].RunSpeed;
                catalog.WalkStart = catalogArray[i].WalkStart;
                catalog.RunStart = catalogArray[i].RunStart;
                catalog.BodySize = catalogArray[i].BodySize;
                catalog.AppearLimit = catalogArray[i].AppearLimit;
                catalog.MoveType = (Masterdatas.MoveType)catalogArray[i].MoveType;

                catalog.GroundEffect = catalogArray[i].GroundEffect != 0;
                catalog.Waitmoving = catalogArray[i].Waitmoving != 0;
                catalog.BattleAjustHeight = catalogArray[i].BattleAjustHeight;

                Masterdatas.Trearuki t = new()
                {
                    enable = trearukiArray[i].Enable != 0,
                    animeIndex = new(),
                    animeDuration = new()
                };
                catalog.trearuki = t;

                foreach (var animeIndex in trearukiArray[i].AnimeIndex)
                    t.animeIndex.Add(animeIndex);

                foreach (var animeDuration in trearukiArray[i].AnimeDuration)
                    t.animeDuration.Add(animeDuration);

                gameData.pokemonInfos.Add(catalog);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed battle data.
        /// </summary>
        private static async Task ParseBattleMasterDatas()
        {
            gameData.motionTimingData = new();
            AssetTypeValueField battleDataTable = (await monoBehaviourCollection[PathEnum.BattleMasterdatas]).Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "BattleDataTable");
            AssetTypeValueField[] motionTimingDataArray = battleDataTable["MotionTimingData"].children[0].children;

            for (int i = 0; i < motionTimingDataArray.Length; i++)
            {
                BattleMasterdatas.MotionTimingData motionTimingData = new();
                motionTimingData.MonsNo = motionTimingDataArray[i]["MonsNo"].value.value.asInt32;
                motionTimingData.FormNo = motionTimingDataArray[i]["FormNo"].value.value.asInt32;
                motionTimingData.Sex = motionTimingDataArray[i]["Sex"].value.value.asInt32;
                motionTimingData.Buturi01 = motionTimingDataArray[i]["Buturi01"].value.value.asInt32;
                motionTimingData.Buturi02 = motionTimingDataArray[i]["Buturi02"].value.value.asInt32;
                motionTimingData.Buturi03 = motionTimingDataArray[i]["Buturi03"].value.value.asInt32;
                motionTimingData.Tokusyu01 = motionTimingDataArray[i]["Tokusyu01"].value.value.asInt32;
                motionTimingData.Tokusyu02 = motionTimingDataArray[i]["Tokusyu02"].value.value.asInt32;
                motionTimingData.Tokusyu03 = motionTimingDataArray[i]["Tokusyu03"].value.value.asInt32;
                motionTimingData.BodyBlow = motionTimingDataArray[i]["BodyBlow"].value.value.asInt32;
                motionTimingData.Punch = motionTimingDataArray[i]["Punch"].value.value.asInt32;
                motionTimingData.Kick = motionTimingDataArray[i]["Kick"].value.value.asInt32;
                motionTimingData.Tail = motionTimingDataArray[i]["Tail"].value.value.asInt32;
                motionTimingData.Bite = motionTimingDataArray[i]["Bite"].value.value.asInt32;
                motionTimingData.Peck = motionTimingDataArray[i]["Peck"].value.value.asInt32;
                motionTimingData.Radial = motionTimingDataArray[i]["Radial"].value.value.asInt32;
                motionTimingData.Cry = motionTimingDataArray[i]["Cry"].value.value.asInt32;
                motionTimingData.Dust = motionTimingDataArray[i]["Dust"].value.value.asInt32;
                motionTimingData.Shot = motionTimingDataArray[i]["Shot"].value.value.asInt32;
                motionTimingData.Guard = motionTimingDataArray[i]["Guard"].value.value.asInt32;
                motionTimingData.LandingFall = motionTimingDataArray[i]["LandingFall"].value.value.asInt32;
                motionTimingData.LandingFallEase = motionTimingDataArray[i]["LandingFallEase"].value.value.asInt32;

                gameData.motionTimingData.Add(motionTimingData);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed yaml battle data.
        /// </summary>
        private static async Task ParseBattleMasterDatasYAML()
        {
            gameData.motionTimingData = new();
            var battleDataTable = (await yamlCollection[PathEnum.BattleMasterdatas]).Find(y => y.MonoBehaviour?.Name == "BattleDataTable").MonoBehaviour as BattleDataTable;
            var motionTimingDataArray = battleDataTable.MotionTimingData;

            for (int i = 0; i < motionTimingDataArray.Length; i++)
            {
                BattleMasterdatas.MotionTimingData motionTimingData = new()
                {
                    MonsNo = motionTimingDataArray[i].MonsNo,
                    FormNo = motionTimingDataArray[i].FormNo,
                    Sex = motionTimingDataArray[i].Sex,
                    Buturi01 = motionTimingDataArray[i].Buturi01,
                    Buturi02 = motionTimingDataArray[i].Buturi02,
                    Buturi03 = motionTimingDataArray[i].Buturi03,
                    Tokusyu01 = motionTimingDataArray[i].Tokusyu01,
                    Tokusyu02 = motionTimingDataArray[i].Tokusyu02,
                    Tokusyu03 = motionTimingDataArray[i].Tokusyu03,
                    BodyBlow = motionTimingDataArray[i].BodyBlow,
                    Punch = motionTimingDataArray[i].Punch,
                    Kick = motionTimingDataArray[i].Kick,
                    Tail = motionTimingDataArray[i].Tail,
                    Bite = motionTimingDataArray[i].Bite,
                    Peck = motionTimingDataArray[i].Peck,
                    Radial = motionTimingDataArray[i].Radial,
                    Cry = motionTimingDataArray[i].Cry,
                    Dust = motionTimingDataArray[i].Dust,
                    Shot = motionTimingDataArray[i].Shot,
                    Guard = motionTimingDataArray[i].Guard,
                    LandingFall = motionTimingDataArray[i].LandingFall,
                    LandingFallEase = motionTimingDataArray[i].LandingFallEase
                };

                gameData.motionTimingData.Add(motionTimingData);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed Moves.
        /// </summary>
        private static async Task ParseMoves()
        {
            gameData.moves = new();
            AssetTypeValueField monoBehaviour = (await monoBehaviourCollection[PathEnum.PersonalMasterdatas]).Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "WazaTable");
            AssetTypeValueField animationData = (await monoBehaviourCollection[PathEnum.BattleMasterdatas]).Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "BattleDataTable");

            AssetTypeValueField[] moveFields = monoBehaviour.children[4].children[0].children;
            AssetTypeValueField[] animationFields = animationData.children[8].children[0].children;
            AssetTypeValueField[] textFields = await FindLabelArrayOfMessageFileAsync("ss_wazaname", Language.English);

            if (animationFields.Length < moveFields.Length)
                MainForm.ShowParserError("Oh my, this BattleDataTable is missing some stuff...\n" +
                    "I don't feel so good...\n" +
                    "WazaTable entries: " + moveFields.Length + "\n" +
                    "BattleDataTable entries: " + animationFields.Length + "??");

            for (int moveID = 0; moveID < moveFields.Length; moveID++)
            {
                Move move = new();
                move.moveID = moveFields[moveID].children[0].value.value.asInt32;
                move.isValid = moveFields[moveID].children[1].value.value.asUInt8;
                move.typingID = moveFields[moveID].children[2].value.value.asUInt8;
                move.category = moveFields[moveID].children[3].value.value.asUInt8;
                move.damageCategoryID = moveFields[moveID].children[4].value.value.asUInt8;
                move.power = moveFields[moveID].children[5].value.value.asUInt8;
                move.hitPer = moveFields[moveID].children[6].value.value.asUInt8;
                move.basePP = moveFields[moveID].children[7].value.value.asUInt8;
                move.priority = moveFields[moveID].children[8].value.value.asInt8;
                move.hitCountMax = moveFields[moveID].children[9].value.value.asUInt8;
                move.hitCountMin = moveFields[moveID].children[10].value.value.asUInt8;
                move.sickID = moveFields[moveID].children[11].value.value.asUInt16;
                move.sickPer = moveFields[moveID].children[12].value.value.asUInt8;
                move.sickCont = moveFields[moveID].children[13].value.value.asUInt8;
                move.sickTurnMin = moveFields[moveID].children[14].value.value.asUInt8;
                move.sickTurnMax = moveFields[moveID].children[15].value.value.asUInt8;
                move.criticalRank = moveFields[moveID].children[16].value.value.asUInt8;
                move.shrinkPer = moveFields[moveID].children[17].value.value.asUInt8;
                move.aiSeqNo = moveFields[moveID].children[18].value.value.asUInt16;
                move.damageRecoverRatio = moveFields[moveID].children[19].value.value.asInt8;
                move.hpRecoverRatio = moveFields[moveID].children[20].value.value.asInt8;
                move.target = moveFields[moveID].children[21].value.value.asUInt8;
                move.rankEffType1 = moveFields[moveID].children[22].value.value.asUInt8;
                move.rankEffType2 = moveFields[moveID].children[23].value.value.asUInt8;
                move.rankEffType3 = moveFields[moveID].children[24].value.value.asUInt8;
                move.rankEffValue1 = moveFields[moveID].children[25].value.value.asInt8;
                move.rankEffValue2 = moveFields[moveID].children[26].value.value.asInt8;
                move.rankEffValue3 = moveFields[moveID].children[27].value.value.asInt8;
                move.rankEffPer1 = moveFields[moveID].children[28].value.value.asUInt8;
                move.rankEffPer2 = moveFields[moveID].children[29].value.value.asUInt8;
                move.rankEffPer3 = moveFields[moveID].children[30].value.value.asUInt8;
                move.flags = moveFields[moveID].children[31].value.value.asUInt32;
                move.contestWazaNo = moveFields[moveID].children[32].value.value.asUInt32;

                move.cmdSeqName = animationFields[moveID].children[1].GetValue().AsString();
                move.cmdSeqNameLegend = animationFields[moveID].children[2].GetValue().AsString();
                move.notShortenTurnType0 = animationFields[moveID].children[3].GetValue().AsString();
                move.notShortenTurnType1 = animationFields[moveID].children[4].GetValue().AsString();
                move.turnType1 = animationFields[moveID].children[5].GetValue().AsString();
                move.turnType2 = animationFields[moveID].children[6].GetValue().AsString();
                move.turnType3 = animationFields[moveID].children[7].GetValue().AsString();
                move.turnType4 = animationFields[moveID].children[8].GetValue().AsString();

                move.name = "";
                if (textFields[moveID].children[6].children[0].childrenCount > 0)
                    move.name = Encoding.UTF8.GetString(textFields[moveID].children[6].children[0].children[0].children[4].value.value.asString);

                gameData.moves.Add(move);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed yaml Moves.
        /// </summary>
        private static async Task ParseMovesYAML()
        {
            gameData.moves = new();
            var monoBehaviour = (await yamlCollection[PathEnum.PersonalMasterdatas]).Find(y => y.MonoBehaviour?.Name == "WazaTable").MonoBehaviour as WazaTable;
            var animationData = (await yamlCollection[PathEnum.BattleMasterdatas]).Find(y => y.MonoBehaviour?.Name == "BattleDataTable").MonoBehaviour as BattleDataTable;

            var moveFields = monoBehaviour.Waza;
            var animationFields = animationData.BattleWazaData;
            var textFields = await FindLabelArrayOfYAMLMessageFileAsync("ss_wazaname", Language.English);

            if (animationFields.Length < moveFields.Length)
                MainForm.ShowParserError("Oh my, this BattleDataTable is missing some stuff...\n" +
                    "I don't feel so good...\n" +
                    "WazaTable entries: " + moveFields.Length + "\n" +
                    "BattleDataTable entries: " + animationFields.Length + "??");

            for (int moveID = 0; moveID < moveFields.Length; moveID++)
            {
                Move move = new()
                {
                    moveID = moveFields[moveID].WazaNo,
                    isValid = moveFields[moveID].IsValid,
                    typingID = moveFields[moveID].Type,
                    category = moveFields[moveID].Category,
                    damageCategoryID = moveFields[moveID].DamageType,
                    power = moveFields[moveID].Power,
                    hitPer = moveFields[moveID].HitPer,
                    basePP = moveFields[moveID].BasePP,
                    priority = moveFields[moveID].Priority,
                    hitCountMax = moveFields[moveID].HitCountMax,
                    hitCountMin = moveFields[moveID].HitCountMin,
                    sickID = moveFields[moveID].SickID,
                    sickPer = moveFields[moveID].SickPer,
                    sickCont = moveFields[moveID].SickCont,
                    sickTurnMin = moveFields[moveID].SickTurnMin,
                    sickTurnMax = moveFields[moveID].SickTurnMax,
                    criticalRank = moveFields[moveID].CriticalRank,
                    shrinkPer = moveFields[moveID].ShrinkPer,
                    aiSeqNo = moveFields[moveID].AISeqNo,
                    damageRecoverRatio = moveFields[moveID].DamageRecoverRatio,
                    hpRecoverRatio = moveFields[moveID].HPRecoverRatio,
                    target = moveFields[moveID].Target,
                    rankEffType1 = moveFields[moveID].RankEffType1,
                    rankEffType2 = moveFields[moveID].RankEffType2,
                    rankEffType3 = moveFields[moveID].RankEffType3,
                    rankEffValue1 = moveFields[moveID].RankEffValue1,
                    rankEffValue2 = moveFields[moveID].RankEffValue2,
                    rankEffValue3 = moveFields[moveID].RankEffValue3,
                    rankEffPer1 = moveFields[moveID].RankEffPer1,
                    rankEffPer2 = moveFields[moveID].RankEffPer2,
                    rankEffPer3 = moveFields[moveID].RankEffPer3,
                    flags = moveFields[moveID].Flags,
                    contestWazaNo = moveFields[moveID].ContestWazaNo,

                    cmdSeqName = animationFields[moveID].CmdSeqName,
                    cmdSeqNameLegend = animationFields[moveID].CmdSeqNameLegend,
                    notShortenTurnType0 = animationFields[moveID].NotShortenTurnType0,
                    notShortenTurnType1 = animationFields[moveID].NotShortenTurnType1,
                    turnType1 = animationFields[moveID].TurnType1,
                    turnType2 = animationFields[moveID].TurnType2,
                    turnType3 = animationFields[moveID].TurnType3,
                    turnType4 = animationFields[moveID].TurnType4,
                };

                move.name = "";
                if (textFields[moveID].WordDataArray.Length > 0)
                    move.name = textFields[moveID].WordDataArray[0].Str;

                gameData.moves.Add(move);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed ShopTables.
        /// </summary>
        private static async Task ParseShopTables()
        {
            gameData.shopTables = new();
            AssetTypeValueField monoBehaviour = (await monoBehaviourCollection[PathEnum.DprMasterdatas]).Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "ShopTable");

            gameData.shopTables.martItems = new();
            AssetTypeValueField[] martItemFields = monoBehaviour.children[4].children[0].children;
            for (int martItemIdx = 0; martItemIdx < martItemFields.Length; martItemIdx++)
            {
                MartItem martItem = new();
                martItem.itemID = martItemFields[martItemIdx].children[0].value.value.asUInt16;
                martItem.badgeNum = martItemFields[martItemIdx].children[1].value.value.asInt32;
                martItem.zoneID = martItemFields[martItemIdx].children[2].value.value.asInt32;

                gameData.shopTables.martItems.Add(martItem);
            }

            gameData.shopTables.fixedShopItems = new();
            AssetTypeValueField[] fixedShopItemFields = monoBehaviour.children[5].children[0].children;
            for (int fixedShopItemIdx = 0; fixedShopItemIdx < fixedShopItemFields.Length; fixedShopItemIdx++)
            {
                FixedShopItem fixedShopItem = new();
                fixedShopItem.itemID = fixedShopItemFields[fixedShopItemIdx].children[0].value.value.asUInt16;
                fixedShopItem.shopID = fixedShopItemFields[fixedShopItemIdx].children[1].value.value.asInt32;

                gameData.shopTables.fixedShopItems.Add(fixedShopItem);
            }

            gameData.shopTables.bpShopItems = new();
            AssetTypeValueField[] bpShopItemFields = monoBehaviour.children[9].children[0].children;
            for (int bpShopItemIdx = 0; bpShopItemIdx < bpShopItemFields.Length; bpShopItemIdx++)
            {
                BpShopItem bpShopItem = new();
                bpShopItem.itemID = bpShopItemFields[bpShopItemIdx].children[0].value.value.asUInt16;
                try
                {
                    bpShopItem.npcID = bpShopItemFields[bpShopItemIdx].children[1].value.value.asInt32;
                }
                catch (IndexOutOfRangeException)
                {
                    MainForm.ShowParserError("Oh my, this dump might be a bit outdated...\n" +
                        "Please input at least the v1.1.3 version of BDSP.\n" +
                        "I don't feel so good...");
                    throw;
                }

                gameData.shopTables.bpShopItems.Add(bpShopItem);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed ysml ShopTables.
        /// </summary>
        private static async Task ParseShopTablesYAML()
        {
            gameData.shopTables = new();
            var monoBehaviour = (await yamlCollection[PathEnum.DprMasterdatas]).Find(y => y.MonoBehaviour?.Name == "ShopTable").MonoBehaviour as ShopTable;

            gameData.shopTables.martItems = new();
            var martItemFields = monoBehaviour.FS;
            for (int martItemIdx = 0; martItemIdx < martItemFields.Length; martItemIdx++)
            {
                MartItem martItem = new()
                {
                    itemID = martItemFields[martItemIdx].ItemNo,
                    badgeNum = martItemFields[martItemIdx].BadgeNum,
                    zoneID = martItemFields[martItemIdx].ZoneID
                };

                gameData.shopTables.martItems.Add(martItem);
            }

            gameData.shopTables.fixedShopItems = new();
            var fixedShopItemFields = monoBehaviour.FixedShop;
            for (int fixedShopItemIdx = 0; fixedShopItemIdx < fixedShopItemFields.Length; fixedShopItemIdx++)
            {
                FixedShopItem fixedShopItem = new()
                {
                    itemID = fixedShopItemFields[fixedShopItemIdx].ItemNo,
                    shopID = fixedShopItemFields[fixedShopItemIdx].ShopID
                };

                gameData.shopTables.fixedShopItems.Add(fixedShopItem);
            }

            gameData.shopTables.bpShopItems = new();
            var bpShopItemFields = monoBehaviour.BPShop;
            for (int bpShopItemIdx = 0; bpShopItemIdx < bpShopItemFields.Length; bpShopItemIdx++)
            {
                BpShopItem bpShopItem = new()
                {
                    itemID = bpShopItemFields[bpShopItemIdx].ItemNo,
                    npcID = bpShopItemFields[bpShopItemIdx].NPCID
                };

                gameData.shopTables.bpShopItems.Add(bpShopItem);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed PickupItems.
        /// </summary>
        private static async Task ParsePickupItems()
        {
            gameData.pickupItems = new();
            AssetTypeValueField monoBehaviour = (await monoBehaviourCollection[PathEnum.DprMasterdatas]).Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "MonohiroiTable");

            AssetTypeValueField[] pickupItemFields = monoBehaviour.children[4].children[0].children;
            for (int pickupItemIdx = 0; pickupItemIdx < pickupItemFields.Length; pickupItemIdx++)
            {
                PickupItem pickupItem = new();
                pickupItem.itemID = pickupItemFields[pickupItemIdx].children[0].value.value.asUInt16;

                //Parse item probabilities
                pickupItem.ratios = new();
                for (int ratio = 0; ratio < pickupItemFields[pickupItemIdx].children[1].children[0].childrenCount; ratio++)
                    pickupItem.ratios.Add(pickupItemFields[pickupItemIdx].children[1].children[0].children[ratio].value.value.asUInt8);

                gameData.pickupItems.Add(pickupItem);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed yaml PickupItems.
        /// </summary>
        private static async Task ParsePickupItemsYAML()
        {
            gameData.pickupItems = new();
            var monoBehaviour = (await yamlCollection[PathEnum.DprMasterdatas]).Find(y => y.MonoBehaviour?.Name == "MonohiroiTable").MonoBehaviour as MonohiroiTable;

            var pickupItemFields = monoBehaviour.MonoHiroi;
            for (int pickupItemIdx = 0; pickupItemIdx < pickupItemFields.Length; pickupItemIdx++)
            {
                PickupItem pickupItem = new()
                {
                    itemID = pickupItemFields[pickupItemIdx].ID
                };

                //Parse item probabilities
                pickupItem.ratios = new();
                for (int ratio = 0; ratio < pickupItemFields[pickupItemIdx].Ratios.Length; ratio++)
                    pickupItem.ratios.Add(pickupItemFields[pickupItemIdx].Ratios[ratio]);

                gameData.pickupItems.Add(pickupItem);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed Items.
        /// </summary>
        private static async Task ParseItems()
        {
            gameData.items = new();
            AssetTypeValueField monoBehaviour = (await monoBehaviourCollection[PathEnum.PersonalMasterdatas]).Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "ItemTable");

            AssetTypeValueField[] itemFields = monoBehaviour.children[4].children[0].children;
            AssetTypeValueField[] textFields = await FindLabelArrayOfMessageFileAsync("ss_itemname", Language.English);

            if (textFields.Length < itemFields.Length)
                MainForm.ShowParserError("Oh my, this " + FormatMessageFileNameForLanguage("ss_itemname", Language.English) + " is missing some stuff...\n" +
                    "I don't feel so good...\n" +
                    "ItemTable entries: " + itemFields.Length + "\n" +
                    FormatMessageFileNameForLanguage("ss_itemname", Language.English) + " entries: " + textFields.Length + "??");

            for (int itemIdx = 0; itemIdx < itemFields.Length; itemIdx++)
            {
                Item item = new();
                item.itemID = itemFields[itemIdx].children[0].value.value.asInt16;
                item.type = itemFields[itemIdx].children[1].value.value.asUInt8;
                item.iconID = itemFields[itemIdx].children[2].value.value.asInt32;
                item.price = itemFields[itemIdx].children[3].value.value.asInt32;
                item.bpPrice = itemFields[itemIdx].children[4].value.value.asInt32;
                item.nageAtc = itemFields[itemIdx].children[7].value.value.asUInt8;
                item.sizenAtc = itemFields[itemIdx].children[8].value.value.asUInt8;
                item.sizenType = itemFields[itemIdx].children[9].value.value.asUInt8;
                item.tuibamuEff = itemFields[itemIdx].children[10].value.value.asUInt8;
                item.sort = itemFields[itemIdx].children[11].value.value.asUInt8;
                item.group = itemFields[itemIdx].children[12].value.value.asUInt8;
                item.groupID = itemFields[itemIdx].children[13].value.value.asUInt8;
                item.fldPocket = itemFields[itemIdx].children[14].value.value.asUInt8;
                item.fieldFunc = itemFields[itemIdx].children[15].value.value.asUInt8;
                item.battleFunc = itemFields[itemIdx].children[16].value.value.asUInt8;
                item.criticalRanks = itemFields[itemIdx].children[18].value.value.asUInt8;
                item.atkStages = itemFields[itemIdx].children[19].value.value.asUInt8;
                item.defStages = itemFields[itemIdx].children[20].value.value.asUInt8;
                item.spdStages = itemFields[itemIdx].children[21].value.value.asUInt8;
                item.accStages = itemFields[itemIdx].children[22].value.value.asUInt8;
                item.spAtkStages = itemFields[itemIdx].children[23].value.value.asUInt8;
                item.spDefStages = itemFields[itemIdx].children[24].value.value.asUInt8;
                item.ppRestoreAmount = itemFields[itemIdx].children[25].value.value.asUInt8;
                item.hpEvIncrease = itemFields[itemIdx].children[26].value.value.asInt8;
                item.atkEvIncrease = itemFields[itemIdx].children[27].value.value.asInt8;
                item.defEvIncrease = itemFields[itemIdx].children[28].value.value.asInt8;
                item.spdEvIncrease = itemFields[itemIdx].children[29].value.value.asInt8;
                item.spAtkEvIncrease = itemFields[itemIdx].children[30].value.value.asInt8;
                item.spDefEvIncrease = itemFields[itemIdx].children[31].value.value.asInt8;
                item.friendshipIncrease1 = itemFields[itemIdx].children[32].value.value.asInt8;
                item.friendshipIncrease2 = itemFields[itemIdx].children[33].value.value.asInt8;
                item.friendshipIncrease3 = itemFields[itemIdx].children[34].value.value.asInt8;
                item.hpRestoreAmount = itemFields[itemIdx].children[35].value.value.asUInt8;
                item.flags0 = itemFields[itemIdx].children[36].value.value.asUInt32;

                item.name = "";
                if (textFields[itemIdx].children[6].children[0].childrenCount > 0)
                    item.name = Encoding.UTF8.GetString(textFields[itemIdx].children[6].children[0].children[0].children[4].value.value.asString);

                gameData.items.Add(item);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed yaml Items.
        /// </summary>
        private static async Task ParseItemsYAML()
        {
            gameData.items = new();
            var monoBehaviour = (await yamlCollection[PathEnum.PersonalMasterdatas]).Find(y => y.MonoBehaviour?.Name == "ItemTable").MonoBehaviour as ItemTable;

            var itemFields = monoBehaviour.Item;
            var textFields = await FindLabelArrayOfYAMLMessageFileAsync("ss_itemname", Language.English);

            if (textFields.Length < itemFields.Length)
                MainForm.ShowParserError("Oh my, this " + FormatMessageFileNameForLanguage("ss_itemname", Language.English) + " is missing some stuff...\n" +
                    "I don't feel so good...\n" +
                    "ItemTable entries: " + itemFields.Length + "\n" +
                    FormatMessageFileNameForLanguage("ss_itemname", Language.English) + " entries: " + textFields.Length + "??");

            for (int itemIdx = 0; itemIdx < itemFields.Length; itemIdx++)
            {
                Item item = new()
                {
                    itemID = itemFields[itemIdx].No,
                    type = itemFields[itemIdx].Type,
                    iconID = itemFields[itemIdx].IconID,
                    price = itemFields[itemIdx].Price,
                    bpPrice = itemFields[itemIdx].BPPrice,
                    nageAtc = itemFields[itemIdx].NageAtc,
                    sizenAtc = itemFields[itemIdx].SizenAtc,
                    sizenType = itemFields[itemIdx].SizenType,
                    tuibamuEff = itemFields[itemIdx].TuibamuEff,
                    sort = itemFields[itemIdx].Sort,
                    group = itemFields[itemIdx].Group,
                    groupID = itemFields[itemIdx].GroupID,
                    fldPocket = itemFields[itemIdx].FldPocket,
                    fieldFunc = itemFields[itemIdx].FieldFunc,
                    battleFunc = itemFields[itemIdx].BattleFunc,
                    criticalRanks = itemFields[itemIdx].WkCriticalUp,
                    atkStages = itemFields[itemIdx].WkAtcUp,
                    defStages = itemFields[itemIdx].WkDefUp,
                    spdStages = itemFields[itemIdx].WkAgiUp,
                    accStages = itemFields[itemIdx].WkHitUp,
                    spAtkStages = itemFields[itemIdx].WkSpaUp,
                    spDefStages = itemFields[itemIdx].WkSpdUp,
                    ppRestoreAmount = itemFields[itemIdx].WkPrmPPRcv,
                    hpEvIncrease = itemFields[itemIdx].WkPrmHPExp,
                    atkEvIncrease = itemFields[itemIdx].WkPrmPowExp,
                    defEvIncrease = itemFields[itemIdx].WkPrmDefExp,
                    spdEvIncrease = itemFields[itemIdx].WkPrmAgiExp,
                    spAtkEvIncrease = itemFields[itemIdx].WkPrmSpaExp,
                    spDefEvIncrease = itemFields[itemIdx].WkPrmSpdExp,
                    friendshipIncrease1 = itemFields[itemIdx].WkFriend1,
                    friendshipIncrease2 = itemFields[itemIdx].WkFriend2,
                    friendshipIncrease3 = itemFields[itemIdx].WkFriend3,
                    hpRestoreAmount = itemFields[itemIdx].WkPrmHPRcv,
                    flags0 = itemFields[itemIdx].Flags0,
                };

                item.name = "";
                if (textFields[itemIdx].WordDataArray.Length > 0)
                    item.name = textFields[itemIdx].WordDataArray[0].Str;

                gameData.items.Add(item);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed growth rates.
        /// </summary>
        private static async Task ParseGrowthRates()
        {
            gameData.growthRates = new();
            AssetTypeValueField monoBehaviour = (await monoBehaviourCollection[PathEnum.PersonalMasterdatas]).Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "GrowTable");

            AssetTypeValueField[] growthRateFields = monoBehaviour.children[4].children[0].children;
            for (int growthRateIdx = 0; growthRateIdx < growthRateFields.Length; growthRateIdx++)
            {
                GrowthRate growthRate = new();
                growthRate.growthID = growthRateIdx;

                //Parse exp requirement
                growthRate.expRequirements = new();
                for (int level = 0; level < growthRateFields[growthRateIdx].children[0].children[0].childrenCount; level++)
                    growthRate.expRequirements.Add(growthRateFields[growthRateIdx].children[0].children[0].children[level].value.value.asUInt32);

                gameData.growthRates.Add(growthRate);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed yaml growth rates.
        /// </summary>
        private static async Task ParseGrowthRatesYAML()
        {
            gameData.growthRates = new();
            var monoBehaviour = (await yamlCollection[PathEnum.PersonalMasterdatas]).Find(y => y.MonoBehaviour?.Name == "GrowTable").MonoBehaviour as GrowTable;

            var growthRateFields = monoBehaviour.Data;
            for (int growthRateIdx = 0; growthRateIdx < growthRateFields.Length; growthRateIdx++)
            {
                GrowthRate growthRate = new()
                {
                    growthID = growthRateIdx
                };

                growthRate.expRequirements = new();
                for (int level = 0; level < growthRateFields[growthRateIdx].Exps.Length; level++)
                    growthRate.expRequirements.Add(growthRateFields[growthRateIdx].Exps[level]);

                gameData.growthRates.Add(growthRate);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed MessageFiles.
        /// </summary>
        public static async Task ParseAllMessageFiles()
        {
            gameData.messageFileSets = new MessageFileSet[10];
            for (int i = 0; i < gameData.messageFileSets.Length; i++)
            {
                gameData.messageFileSets[i] = new();
                gameData.messageFileSets[i].messageFiles = new();
            }
            gameData.messageFileSets[0].langID = Language.Japanese;
            gameData.messageFileSets[1].langID = Language.Japanese;
            gameData.messageFileSets[2].langID = Language.English;
            gameData.messageFileSets[3].langID = Language.French;
            gameData.messageFileSets[4].langID = Language.Italian;
            gameData.messageFileSets[5].langID = Language.German;
            gameData.messageFileSets[6].langID = Language.Spanish;
            gameData.messageFileSets[7].langID = Language.Korean;
            gameData.messageFileSets[8].langID = Language.SimpChinese;
            gameData.messageFileSets[9].langID = Language.TradChinese;

            List<AssetTypeValueField> monoBehaviours = await monoBehaviourCollection[PathEnum.CommonMsbt];
            monoBehaviours.AddRange(await monoBehaviourCollection[PathEnum.English]);
            monoBehaviours.AddRange(await monoBehaviourCollection[PathEnum.French]);
            monoBehaviours.AddRange(await monoBehaviourCollection[PathEnum.German]);
            monoBehaviours.AddRange(await monoBehaviourCollection[PathEnum.Italian]);
            monoBehaviours.AddRange(await monoBehaviourCollection[PathEnum.Jpn]);
            monoBehaviours.AddRange(await monoBehaviourCollection[PathEnum.JpnKanji]);
            monoBehaviours.AddRange(await monoBehaviourCollection[PathEnum.Korean]);
            monoBehaviours.AddRange(await monoBehaviourCollection[PathEnum.SimpChinese]);
            monoBehaviours.AddRange(await monoBehaviourCollection[PathEnum.Spanish]);
            monoBehaviours.AddRange(await monoBehaviourCollection[PathEnum.TradChinese]);

            for (int mIdx = 0; mIdx < monoBehaviours.Count; mIdx++)
            {
                MessageFile messageFile = new();
                messageFile.mName = Encoding.Default.GetString(monoBehaviours[mIdx].children[3].value.value.asString);
                messageFile.langID = (Language)monoBehaviours[mIdx].children[5].value.value.asInt32;
                messageFile.isKanji = monoBehaviours[mIdx].children[7].value.value.asUInt8;

                //Parse LabelData
                messageFile.labelDatas = new();
                AssetTypeValueField[] labelDataFields = monoBehaviours[mIdx].children[8].children[0].children;
                for (int labelDataIdx = 0; labelDataIdx < labelDataFields.Length; labelDataIdx++)
                {
                    GameDataTypes.LabelData labelData = new();
                    labelData.labelIndex = labelDataFields[labelDataIdx].children[0].value.value.asInt32;
                    labelData.arrayIndex = labelDataFields[labelDataIdx].children[1].value.value.asInt32;
                    labelData.labelName = Encoding.Default.GetString(labelDataFields[labelDataIdx].children[2].value.value.asString);
                    labelData.styleIndex = labelDataFields[labelDataIdx].children[3].children[0].value.value.asInt32;
                    labelData.colorIndex = labelDataFields[labelDataIdx].children[3].children[1].value.value.asInt32;
                    labelData.fontSize = labelDataFields[labelDataIdx].children[3].children[2].value.value.asInt32;
                    labelData.maxWidth = labelDataFields[labelDataIdx].children[3].children[3].value.value.asInt32;
                    labelData.controlID = labelDataFields[labelDataIdx].children[3].children[4].value.value.asInt32;

                    // Parse Attribute Array
                    AssetTypeValueField[] attrArray = labelDataFields[labelDataIdx].children[4].children[0].children;
                    labelData.attributeValues = new();
                    for (int attrIdx = 0; attrIdx < attrArray.Length; attrIdx++)
                    {
                        labelData.attributeValues.Add(attrArray[attrIdx].value.value.asInt32);
                    }

                    // Parse TagData
                    AssetTypeValueField[] tagDataFields = labelDataFields[labelDataIdx].children[5].children[0].children;
                    labelData.tagDatas = new();
                    for (int tagDataIdx = 0; tagDataIdx < tagDataFields.Length; tagDataIdx++)
                    {
                        GameDataTypes.TagData tagData = new();
                        tagData.tagIndex = tagDataFields[tagDataIdx].children[0].value.value.asInt32;
                        tagData.groupID = tagDataFields[tagDataIdx].children[1].value.value.asInt32;
                        tagData.tagID = tagDataFields[tagDataIdx].children[2].value.value.asInt32;
                        tagData.tagPatternID = tagDataFields[tagDataIdx].children[3].value.value.asInt32;
                        tagData.forceArticle = tagDataFields[tagDataIdx].children[4].value.value.asInt32;
                        tagData.tagParameter = tagDataFields[tagDataIdx].children[5].value.value.asInt32;
                        tagData.tagWordArray = new();
                        foreach (AssetTypeValueField tagWordField in tagDataFields[tagDataIdx].children[6][0].children)
                        {
                            tagData.tagWordArray.Add(tagWordField.GetValue().AsString());
                        }

                        tagData.forceGrmID = tagDataFields[tagDataIdx].children[7].value.value.asInt32;

                        labelData.tagDatas.Add(tagData);
                    }

                    //Parse WordData
                    labelData.wordDatas = new();
                    AssetTypeValueField[] wordDataFields = labelDataFields[labelDataIdx].children[6].children[0].children;
                    for (int wordDataIdx = 0; wordDataIdx < wordDataFields.Length; wordDataIdx++)
                    {
                        GameDataTypes.WordData wordData = new();
                        wordData.patternID = wordDataFields[wordDataIdx].children[0].value.value.asInt32;
                        wordData.eventID = wordDataFields[wordDataIdx].children[1].value.value.asInt32;
                        wordData.tagIndex = wordDataFields[wordDataIdx].children[2].value.value.asInt32;
                        wordData.tagValue = wordDataFields[wordDataIdx].children[3].value.value.asFloat;
                        wordData.str = Encoding.UTF8.GetString(wordDataFields[wordDataIdx].children[4].value.value.asString);
                        wordData.strWidth = wordDataFields[wordDataIdx].children[5].value.value.asFloat;

                        labelData.wordDatas.Add(wordData);
                    }

                    messageFile.labelDatas.Add(labelData);
                }

                switch (messageFile.langID)
                {
                    case Language.Japanese:
                        if (messageFile.isKanji == 0)
                            gameData.messageFileSets[0].messageFiles.Add(messageFile);
                        else
                            gameData.messageFileSets[1].messageFiles.Add(messageFile);
                        break;
                    case Language.English:
                        gameData.messageFileSets[2].messageFiles.Add(messageFile);
                        break;
                    case Language.French:
                        gameData.messageFileSets[3].messageFiles.Add(messageFile);
                        break;
                    case Language.Italian:
                        gameData.messageFileSets[4].messageFiles.Add(messageFile);
                        break;
                    case Language.German:
                        gameData.messageFileSets[5].messageFiles.Add(messageFile);
                        break;
                    case Language.Spanish:
                        gameData.messageFileSets[6].messageFiles.Add(messageFile);
                        break;
                    case Language.Korean:
                        gameData.messageFileSets[7].messageFiles.Add(messageFile);
                        break;
                    case Language.SimpChinese:
                        gameData.messageFileSets[8].messageFiles.Add(messageFile);
                        break;
                    case Language.TradChinese:
                        gameData.messageFileSets[9].messageFiles.Add(messageFile);
                        break;
                }
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed yaml MessageFiles.
        /// </summary>
        public static async Task ParseAllMessageFilesYAML()
        {
            gameData.messageFileSets = new MessageFileSet[10];
            for (int i = 0; i < gameData.messageFileSets.Length; i++)
            {
                gameData.messageFileSets[i] = new();
                gameData.messageFileSets[i].messageFiles = new();
            }
            gameData.messageFileSets[0].langID = Language.Japanese;
            gameData.messageFileSets[1].langID = Language.Japanese;
            gameData.messageFileSets[2].langID = Language.English;
            gameData.messageFileSets[3].langID = Language.French;
            gameData.messageFileSets[4].langID = Language.Italian;
            gameData.messageFileSets[5].langID = Language.German;
            gameData.messageFileSets[6].langID = Language.Spanish;
            gameData.messageFileSets[7].langID = Language.Korean;
            gameData.messageFileSets[8].langID = Language.SimpChinese;
            gameData.messageFileSets[9].langID = Language.TradChinese;

            List<YamlMonoContainer> monoBehaviours = await yamlCollection[PathEnum.CommonMsbt];
            monoBehaviours.AddRange(await yamlCollection[PathEnum.English]);
            monoBehaviours.AddRange(await yamlCollection[PathEnum.French]);
            monoBehaviours.AddRange(await yamlCollection[PathEnum.German]);
            monoBehaviours.AddRange(await yamlCollection[PathEnum.Italian]);
            monoBehaviours.AddRange(await yamlCollection[PathEnum.Jpn]);
            monoBehaviours.AddRange(await yamlCollection[PathEnum.JpnKanji]);
            monoBehaviours.AddRange(await yamlCollection[PathEnum.Korean]);
            monoBehaviours.AddRange(await yamlCollection[PathEnum.SimpChinese]);
            monoBehaviours.AddRange(await yamlCollection[PathEnum.Spanish]);
            monoBehaviours.AddRange(await yamlCollection[PathEnum.TradChinese]);

            for (int mIdx = 0; mIdx < monoBehaviours.Count; mIdx++)
            {
                var mono = monoBehaviours[mIdx].MonoBehaviour as MsbtData;

                MessageFile messageFile = new()
                {
                    mName = mono.Name,
                    langID = (Language)mono.LangID,
                    isKanji = mono.IsKanji,
                };

                //Parse LabelData
                LabelData[] yamlLabelData = mono.LabelDataArray;
                messageFile.labelDatas = new();
                for (int labelDataIdx = 0; labelDataIdx < yamlLabelData.Length; labelDataIdx++)
                {
                    GameDataTypes.LabelData labelData = new()
                    {
                        labelIndex = yamlLabelData[labelDataIdx].LabelIndex,
                        arrayIndex = yamlLabelData[labelDataIdx].ArrayIndex,
                        labelName = yamlLabelData[labelDataIdx].LabelName,
                        styleIndex = yamlLabelData[labelDataIdx].StyleInfo.StyleIndex,
                        colorIndex = yamlLabelData[labelDataIdx].StyleInfo.ColorIndex,
                        fontSize = yamlLabelData[labelDataIdx].StyleInfo.FontSize,
                        maxWidth = yamlLabelData[labelDataIdx].StyleInfo.MaxWidth,
                        controlID = yamlLabelData[labelDataIdx].StyleInfo.ControlID,
                    };

                    // Parse Attribute Array
                    int[] yamlAttrArray = yamlLabelData[labelDataIdx].AttributeValueArray;
                    labelData.attributeValues = new();
                    for (int attrIdx = 0; attrIdx < yamlAttrArray.Length; attrIdx++)
                        labelData.attributeValues.Add(yamlAttrArray[attrIdx]);

                    // Parse TagData
                    TagData[] yamlTagData = yamlLabelData[labelDataIdx].TagDataArray;
                    labelData.tagDatas = new();
                    for (int tagDataIdx = 0; tagDataIdx < yamlTagData.Length; tagDataIdx++)
                    {
                        GameDataTypes.TagData tagData = new()
                        {
                            tagIndex = yamlTagData[tagDataIdx].TagIndex,
                            groupID = yamlTagData[tagDataIdx].GroupID,
                            tagID = yamlTagData[tagDataIdx].TagID,
                            tagPatternID = yamlTagData[tagDataIdx].TagPatternID,
                            forceArticle = yamlTagData[tagDataIdx].ForceArticle,
                            tagParameter = yamlTagData[tagDataIdx].TagParameter,
                        };
                        
                        tagData.tagWordArray = new();
                        foreach (string tagWord in yamlTagData[tagDataIdx].TagWordArray)
                            tagData.tagWordArray.Add(tagWord);

                        tagData.forceGrmID = yamlTagData[tagDataIdx].ForceGrmID;

                        labelData.tagDatas.Add(tagData);
                    }

                    //Parse WordData
                    WordData[] yamlWordData = yamlLabelData[labelDataIdx].WordDataArray;
                    labelData.wordDatas = new();
                    for (int wordDataIdx = 0; wordDataIdx < yamlWordData.Length; wordDataIdx++)
                    {
                        GameDataTypes.WordData wordData = new()
                        {
                            patternID = yamlWordData[wordDataIdx].PatternID,
                            eventID = yamlWordData[wordDataIdx].EventID,
                            tagIndex = yamlWordData[wordDataIdx].TagIndex,
                            tagValue = yamlWordData[wordDataIdx].TagValue,
                            str = yamlWordData[wordDataIdx].Str,
                            strWidth = yamlWordData[wordDataIdx].StrWidth
                        };

                        labelData.wordDatas.Add(wordData);
                    }

                    messageFile.labelDatas.Add(labelData);
                }

                switch (messageFile.langID)
                {
                    case Language.Japanese:
                        if (messageFile.isKanji == 0)
                            gameData.messageFileSets[0].messageFiles.Add(messageFile);
                        else
                            gameData.messageFileSets[1].messageFiles.Add(messageFile);
                        break;
                    case Language.English:
                        gameData.messageFileSets[2].messageFiles.Add(messageFile);
                        break;
                    case Language.French:
                        gameData.messageFileSets[3].messageFiles.Add(messageFile);
                        break;
                    case Language.Italian:
                        gameData.messageFileSets[4].messageFiles.Add(messageFile);
                        break;
                    case Language.German:
                        gameData.messageFileSets[5].messageFiles.Add(messageFile);
                        break;
                    case Language.Spanish:
                        gameData.messageFileSets[6].messageFiles.Add(messageFile);
                        break;
                    case Language.Korean:
                        gameData.messageFileSets[7].messageFiles.Add(messageFile);
                        break;
                    case Language.SimpChinese:
                        gameData.messageFileSets[8].messageFiles.Add(messageFile);
                        break;
                    case Language.TradChinese:
                        gameData.messageFileSets[9].messageFiles.Add(messageFile);
                        break;
                }
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed EvScripts.
        /// </summary>
        private static async Task ParseEvScripts()
        {
            gameData.evScripts = new();
            List<AssetTypeValueField> monoBehaviours = (await monoBehaviourCollection[PathEnum.EvScript]).Where(m => m.children[4].GetName() == "Scripts").ToList();

            for (int mIdx = 0; mIdx < monoBehaviours.Count; mIdx++)
            {
                EvScript evScript = new();
                evScript.mName = Encoding.Default.GetString(monoBehaviours[mIdx].children[3].value.value.asString);

                //Parse Scripts
                evScript.scripts = new();
                AssetTypeValueField[] scriptFields = monoBehaviours[mIdx].children[4].children[0].children;
                for (int scriptIdx = 0; scriptIdx < scriptFields.Length; scriptIdx++)
                {
                    GameDataTypes.Script script = new();
                    script.evLabel = Encoding.Default.GetString(scriptFields[scriptIdx].children[0].value.value.asString);

                    //Parse Commands
                    script.commands = new();
                    AssetTypeValueField[] commandFields = scriptFields[scriptIdx].children[1].children[0].children;
                    for (int commandIdx = 0; commandIdx < commandFields.Length; commandIdx++)
                    {
                        GameDataTypes.Command command = new();

                        //Check for commands without data, because those exist for some reason.
                        if (commandFields[commandIdx].children[0].children[0].children.Length == 0)
                        {
                            command.cmdType = -1;
                            script.commands.Add(command);
                            continue;
                        }
                        command.cmdType = commandFields[commandIdx].children[0].children[0].children[0].children[1].value.value.asInt32;

                        //Parse Arguments
                        command.args = new();
                        AssetTypeValueField[] argumentFields = commandFields[commandIdx].children[0].children[0].children;
                        for (int argIdx = 1; argIdx < argumentFields.Length; argIdx++)
                        {
                            Argument arg = new();
                            arg.argType = argumentFields[argIdx].children[0].value.value.asInt32;
                            arg.data = argumentFields[argIdx].children[1].value.value.asInt32;
                            if (arg.argType == 1)
                                arg.data = ConvertToFloat((int)arg.data);

                            command.args.Add(arg);
                        }

                        script.commands.Add(command);
                    }

                    evScript.scripts.Add(script);
                }

                //Parse StrLists
                evScript.strList = new();
                AssetTypeValueField[] stringFields = monoBehaviours[mIdx].children[5].children[0].children;
                for (int stringIdx = 0; stringIdx < stringFields.Length; stringIdx++)
                    evScript.strList.Add(Encoding.Default.GetString(stringFields[stringIdx].value.value.asString));

                gameData.evScripts.Add(evScript);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed yaml EvScripts.
        /// </summary>
        private static async Task ParseEvScriptsYAML()
        {
            gameData.evScripts = new();
            var monoBehaviours = (await yamlCollection[PathEnum.EvScript]).Select(y => y.MonoBehaviour).OfType<EvData>().ToList();

            for (int mIdx = 0; mIdx < monoBehaviours.Count; mIdx++)
            {
                EvScript evScript = new();
                evScript.mName = monoBehaviours[mIdx].Name;

                //Parse Scripts
                evScript.scripts = new();
                var scriptFields = monoBehaviours[mIdx].Scripts;
                for (int scriptIdx = 0; scriptIdx < scriptFields.Count; scriptIdx++)
                {
                    GameDataTypes.Script script = new();
                    script.evLabel = scriptFields[scriptIdx].Label;

                    //Parse Commands
                    script.commands = new();
                    var commandFields = scriptFields[scriptIdx].Commands;
                    for (int commandIdx = 0; commandIdx < commandFields.Count; commandIdx++)
                    {
                        GameDataTypes.Command command = new();

                        //Check for commands without data, because those exist for some reason.
                        if (commandFields[commandIdx].Arg.Count == 0)
                        {
                            command.cmdType = -1;
                            script.commands.Add(command);
                            continue;
                        }
                        command.cmdType = commandFields[commandIdx].Arg[0].Data;

                        //Parse Arguments
                        command.args = new();
                        var argumentFields = commandFields[commandIdx].Arg;
                        for (int argIdx = 1; argIdx < argumentFields.Count; argIdx++)
                        {
                            Argument arg = new();
                            arg.argType = argumentFields[argIdx].ArgType;
                            arg.data = argumentFields[argIdx].Data;
                            if (arg.argType == 1)
                                arg.data = ConvertToFloat((int)arg.data);

                            command.args.Add(arg);
                        }

                        script.commands.Add(command);
                    }

                    evScript.scripts.Add(script);
                }

                //Parse StrLists
                evScript.strList = new();
                var stringFields = monoBehaviours[mIdx].StrList;
                for (int stringIdx = 0; stringIdx < stringFields.Count; stringIdx++)
                    evScript.strList.Add(stringFields[stringIdx]);

                gameData.evScripts.Add(evScript);
            }
        }

        /// <summary>
        ///  Interprets bytes of an int32 as a float.
        /// </summary>
        private static float ConvertToFloat(int n)
        {
            return BitConverter.ToSingle(BitConverter.GetBytes(n));
        }

        /// <summary>
        ///  Overwrites GlobalData with a parsed WwiseData.
        /// </summary>
        private static void ParseAudioData()
        {
            gameData.audioData = new();
            gameData.audioData.Parse(fileManager.GetDelphisMainBuffer());
        }

        /// <summary>
        ///  Overwrites GlobalData with a parsed GlobalMetadata.
        /// </summary>
        private static void ParseGlobalMetadata()
        {
            gameData.globalMetadata = new();
            byte[] buffer = fileManager.GetGlobalMetadataBuffer();
            gameData.globalMetadata.buffer = buffer;

            gameData.globalMetadata.stringOffset = BitConverter.ToUInt32(buffer, 0x18);

            gameData.globalMetadata.defaultValuePtrOffset = BitConverter.ToUInt32(buffer, 0x40);
            gameData.globalMetadata.defaultValuePtrSecSize = BitConverter.ToUInt32(buffer, 0x44);
            uint defaultValuePtrSize = 0xC;
            uint defaultValuePtrCount = gameData.globalMetadata.defaultValuePtrSecSize / defaultValuePtrSize;

            gameData.globalMetadata.defaultValueOffset = BitConverter.ToUInt32(buffer, 0x48);
            gameData.globalMetadata.defaultValueSecSize = BitConverter.ToUInt32(buffer, 0x4C);

            gameData.globalMetadata.fieldOffset = BitConverter.ToUInt32(buffer, 0x60);
            uint fieldSize = 0xC;

            gameData.globalMetadata.typeOffset = BitConverter.ToUInt32(buffer, 0xA0);
            uint typeSize = 0x5C;

            gameData.globalMetadata.imageOffset = BitConverter.ToUInt32(buffer, 0xA8);
            gameData.globalMetadata.imageSecSize = BitConverter.ToUInt32(buffer, 0xAC);
            uint imageSize = 0x28;
            uint imageCount = gameData.globalMetadata.imageSecSize / imageSize;

            gameData.globalMetadata.defaultValueDic = new();
            uint defaultValuePtrOffset = gameData.globalMetadata.defaultValuePtrOffset;
            for (int defaultValuePtrIdx = 0; defaultValuePtrIdx < defaultValuePtrCount; defaultValuePtrIdx++)
            {
                FieldDefaultValue fdv = new();
                fdv.offset = gameData.globalMetadata.defaultValueOffset + BitConverter.ToUInt32(buffer, (int)defaultValuePtrOffset + 8);
                long nextOffset = gameData.globalMetadata.defaultValueOffset + gameData.globalMetadata.defaultValueSecSize;
                if (defaultValuePtrIdx < defaultValuePtrCount - 1)
                    nextOffset = gameData.globalMetadata.defaultValueOffset + BitConverter.ToUInt32(buffer, (int)defaultValuePtrOffset + 20);
                fdv.length = (int)(nextOffset - fdv.offset);
                uint fieldIdx = BitConverter.ToUInt32(buffer, (int)defaultValuePtrOffset + 0);

                gameData.globalMetadata.defaultValueDic[fieldIdx] = fdv;
                defaultValuePtrOffset += defaultValuePtrSize;
            }

            gameData.globalMetadata.images = new();
            uint imageOffset = gameData.globalMetadata.imageOffset;
            for (int imageIdx = 0; imageIdx < imageCount; imageIdx++)
            {
                ImageDefinition id = new();
                uint imageNameIdx = BitConverter.ToUInt32(buffer, (int)imageOffset + 0);
                id.name = ReadNullTerminatedString(buffer, gameData.globalMetadata.stringOffset + imageNameIdx);
                id.typeStart = BitConverter.ToUInt32(buffer, (int)imageOffset + 8);
                id.typeCount = BitConverter.ToUInt32(buffer, (int)imageOffset + 12);

                id.types = new();
                uint typeOffset = gameData.globalMetadata.typeOffset + id.typeStart * typeSize;
                for (uint typeIdx = id.typeStart; typeIdx < id.typeStart + id.typeCount; typeIdx++)
                {
                    TypeDefinition td = new();
                    uint typeNameIdx = BitConverter.ToUInt32(buffer, (int)typeOffset + 0);
                    uint namespaceNameIdx = BitConverter.ToUInt32(buffer, (int)typeOffset + 4);
                    td.name = ReadNullTerminatedString(buffer, gameData.globalMetadata.stringOffset + namespaceNameIdx);
                    td.name += td.name.Length > 0 ? "." : "";
                    td.name += ReadNullTerminatedString(buffer, gameData.globalMetadata.stringOffset + typeNameIdx);
                    td.fieldStart = BitConverter.ToInt32(buffer, (int)typeOffset + 36);
                    td.fieldCount = BitConverter.ToUInt16(buffer, (int)typeOffset + 72);

                    td.fields = new();
                    uint fieldOffset = (uint)(gameData.globalMetadata.fieldOffset + td.fieldStart * fieldSize);
                    for (uint fieldIdx = (uint)td.fieldStart; fieldIdx < td.fieldStart + td.fieldCount; fieldIdx++)
                    {
                        FieldDefinition fd = new();
                        uint fieldNameIdx = BitConverter.ToUInt32(buffer, (int)fieldOffset + 0);
                        fd.name = ReadNullTerminatedString(buffer, gameData.globalMetadata.stringOffset + fieldNameIdx);
                        if (gameData.globalMetadata.defaultValueDic.TryGetValue(fieldIdx, out FieldDefaultValue fdv))
                            fd.defautValue = fdv;

                        td.fields.Add(fd);
                        fieldOffset += fieldSize;
                    }

                    id.types.Add(td);
                    typeOffset += typeSize;
                }

                gameData.globalMetadata.images.Add(id);
                imageOffset += imageSize;
            }

            string[] typeArrayNames = new string[]
            {
"A3758C06C7FB42A47D220A11FBA532C6E8C62A77",
"4B289ECFF3C0F0970CFBB23E3106E05803CB0010",
"B9D3FD531E1A63CC167C4B98C0EC93F0249D9944",
"347E5A9763B5C5AD3094AEC4B91A98983001E87D",
"C089A0863406C198B5654996536BAC473C816234",
"BCEEC8610D8506C3EDAC1C28CED532E5E2D8AD32",
"A6F987666C679A4472D8CD64F600B501D2241486",
"ACBC28AD33161A13959E63783CBFC94EB7FB2D90",
"0459498E9764395D87F7F43BE89CCE657C669BFC",
"C4215116A59F8DBC29910FA47BFBC6A82702816F",
"AEDBD0B97A96E5BDD926058406DB246904438044",
"DF2387E4B816070AE396698F2BD7359657EADE81",
"64FFED43123BBC9517F387412947F1C700527EB4",
"B5D988D1CB442CF60C021541BF2DC2A008819FD4",
"D64329EA3A838F1B4186746A734070A5DFDA4983",
"37DF3221C4030AC4E0EB9DD64616D020BB628CC1",
"B2DD1970DDE852F750899708154090300541F4DE",
"F774719D6A36449B152496136177E900605C9778"
            };

            TypeDefinition privateImplementationDetails = gameData.globalMetadata.images
            .Where(i => i.name == "Assembly-CSharp.dll").SelectMany(i => i.types)
            .First(t => t.name == "<PrivateImplementationDetails>");

            gameData.globalMetadata.typeMatchupOffsets = typeArrayNames
            .Select(s => privateImplementationDetails.fields.First(f => f.name == s).defautValue.offset).ToArray();
        }

        /// <summary>
        ///  Returns the null terminated UTF8 string starting at the specified offset.
        /// </summary>
        private static string ReadNullTerminatedString(byte[] buffer, long offset)
        {
            long endOffset = offset;
            while (buffer[endOffset] != 0)
                endOffset++;
            return Encoding.UTF8.GetString(buffer, (int)offset, (int)(endOffset - offset));
        }

        /// <summary>
        ///  Overwrites GlobalData with a parsed AssetBundleDownloadManifest.
        /// </summary>
        private static void ParseDprBin()
        {
            AssetBundleDownloadManifest abdm = fileManager.GetDprBin();
            if (abdm != null)
                gameData.dprBin = abdm;
        }

        /// <summary>
        ///  Commits all modified files and prepares them for exporting.
        /// </summary>
        public static void CommitChanges()
        {
            if (gameData.IsModified(GameDataSet.DataField.EvScripts))
                CommitEvScripts();
            if (gameData.IsModified(GameDataSet.DataField.PickupItems))
                CommitPickupItems();
            if (gameData.IsModified(GameDataSet.DataField.ShopTables))
                CommitShopTables();
            if (gameData.IsModified(GameDataSet.DataField.Trainers))
                CommitTrainers();
            if (gameData.IsModified(GameDataSet.DataField.BattleTowerTrainers))
                CommitBattleTowerTrainers();
            if (gameData.IsModified(GameDataSet.DataField.BattleTowerTrainerPokemons))
                CommitBattleTowerPokemon();
            if (gameData.IsModified(GameDataSet.DataField.EncounterTableFiles))
                CommitEncounterTables();
            if (gameData.IsModified(GameDataSet.DataField.MessageFileSets))
                CommitMessageFileSets();
            if (gameData.IsModified(GameDataSet.DataField.UgAreas) ||
            gameData.IsModified(GameDataSet.DataField.UgEncounterFiles) ||
            gameData.IsModified(GameDataSet.DataField.UgEncounterLevelSets) ||
            gameData.IsModified(GameDataSet.DataField.UgSpecialEncounters) ||
            gameData.IsModified(GameDataSet.DataField.UgPokemonData))
                CommitUgTables();
            if (gameData.IsModified(GameDataSet.DataField.PersonalEntries))
                CommitPokemon();
            if (gameData.IsModified(GameDataSet.DataField.Items))
                CommitItems();
            if (gameData.IsModified(GameDataSet.DataField.TMs))
                CommitTMs();
            if (gameData.IsModified(GameDataSet.DataField.Moves))
                CommitMoves();
            if (gameData.IsModified(GameDataSet.DataField.AudioData))
                CommitAudio();
            if (gameData.IsModified(GameDataSet.DataField.GlobalMetadata))
                CommitGlobalMetadata();
            if (gameData.IsModified(GameDataSet.DataField.UIMasterdatas))
                CommitUIMasterdatas();
            if (gameData.IsModified(GameDataSet.DataField.AddPersonalTable))
                CommitAddPersonalTable();
            if (gameData.IsModified(GameDataSet.DataField.MotionTimingData))
                CommitMotionTimingData();
            if (gameData.IsModified(GameDataSet.DataField.PokemonInfo))
                CommitPokemonInfo();
            if (gameData.IsModified(GameDataSet.DataField.ContestResultMotion))
                CommitContestMasterDatas();
            if (gameData.IsModified(GameDataSet.DataField.DprBin))
                CommitDprBin();
            if (gameData.IsModified(GameDataSet.DataField.ExternalStarters))
                CommitExternalStarters();
            if (gameData.IsModified(GameDataSet.DataField.ExternalHoneyTrees))
                CommitExternalHoneyTrees();
        }

        /// <summary>
        ///  Commits all modified files to YAML and prepares them for exporting.
        /// </summary>
        public static void CommitChangesToYAML()
        {
            if (gameData.IsModified(GameDataSet.DataField.EvScripts))
                CommitEvScriptsYAML();
            if (gameData.IsModified(GameDataSet.DataField.PickupItems))
                CommitPickupItemsYAML();
            if (gameData.IsModified(GameDataSet.DataField.ShopTables))
                CommitShopTablesYAML();
            if (gameData.IsModified(GameDataSet.DataField.Trainers))
                CommitTrainersYAML();
            if (gameData.IsModified(GameDataSet.DataField.BattleTowerTrainers))
                CommitBattleTowerTrainersYAML();
            if (gameData.IsModified(GameDataSet.DataField.BattleTowerTrainerPokemons))
                CommitBattleTowerPokemonYAML();
            if (gameData.IsModified(GameDataSet.DataField.EncounterTableFiles))
                CommitEncounterTablesYAML();
            if (gameData.IsModified(GameDataSet.DataField.MessageFileSets))
                CommitMessageFileSetsYAML();
            if (gameData.IsModified(GameDataSet.DataField.UgAreas) ||
            gameData.IsModified(GameDataSet.DataField.UgEncounterFiles) ||
            gameData.IsModified(GameDataSet.DataField.UgEncounterLevelSets) ||
            gameData.IsModified(GameDataSet.DataField.UgSpecialEncounters) ||
            gameData.IsModified(GameDataSet.DataField.UgPokemonData))
                CommitUgTablesYAML();
            if (gameData.IsModified(GameDataSet.DataField.PersonalEntries))
                CommitPokemonYAML();
            if (gameData.IsModified(GameDataSet.DataField.Items))
                CommitItemsYAML();
            if (gameData.IsModified(GameDataSet.DataField.TMs))
                CommitTMsYAML();
            if (gameData.IsModified(GameDataSet.DataField.Moves))
                CommitMovesYAML();
            if (gameData.IsModified(GameDataSet.DataField.UIMasterdatas))
                CommitUIMasterdatasYAML();
            if (gameData.IsModified(GameDataSet.DataField.AddPersonalTable))
                CommitAddPersonalTableYAML();
            if (gameData.IsModified(GameDataSet.DataField.MotionTimingData))
                CommitMotionTimingDataYAML();
            if (gameData.IsModified(GameDataSet.DataField.PokemonInfo))
                CommitPokemonInfoYAML();
            if (gameData.IsModified(GameDataSet.DataField.ContestResultMotion))
                CommitContestMasterDatasYAML();
            if (gameData.IsModified(GameDataSet.DataField.ExternalStarters))
                CommitExternalStarters(true);
            if (gameData.IsModified(GameDataSet.DataField.ExternalHoneyTrees))
                CommitExternalHoneyTrees(true);
        }

        private static void CommitExternalHoneyTrees(bool unityPath = false)
        {
            foreach ((string name, Starter _) in gameData.externalStarters)
                fileManager.CommitExternalJson($"Encounters\\Starter\\{name}.json", unityPath);
        }

        private static void CommitExternalStarters(bool unityPath = false)
        {
            foreach ((string name, HoneyTreeZone _) in gameData.externalHoneyTrees)
                fileManager.CommitExternalJson($"Encounters\\HoneyTrees\\{name}.json", unityPath);
        }

        private static void CommitContestMasterDatas()
        {
            gameData.contestResultMotion.Sort();
            AssetTypeValueField monoBehaviour = fileManager.GetMonoBehaviours(PathEnum.ContestMasterdatas).Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "ContestConfigDatas");

            AssetTypeValueField[] resultMotionSheet = monoBehaviour["ResultMotion"].children[0].children;
            AssetTypeValueField resultMotionRef = resultMotionSheet[0];
            List<AssetTypeValueField> newResultMotionSheet = new();
            for (int i = 0; i < gameData.contestResultMotion.Count; i++)
            {
                ResultMotion rm = gameData.contestResultMotion[i];
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromTemplate(resultMotionRef.GetTemplateField());
                baseField["valid_flag"].GetValue().Set(rm.validFlag);
                baseField["id"].GetValue().Set(rm.id);
                baseField["monsNo"].GetValue().Set(rm.monsNo);
                baseField["winAnim"].GetValue().Set(rm.winAnim);
                baseField["loseAnim"].GetValue().Set(rm.loseAnim);
                baseField["waitAnim"].GetValue().Set(rm.waitAnim);
                baseField["duration"].GetValue().Set(rm.duration);
                newResultMotionSheet.Add(baseField);
            }
            monoBehaviour["ResultMotion"].children[0].SetChildrenList(newResultMotionSheet.ToArray());
            fileManager.WriteMonoBehaviour(PathEnum.ContestMasterdatas, monoBehaviour);
        }

        private static void CommitContestMasterDatasYAML()
        {
            gameData.contestResultMotion.Sort();
            var monoBehaviour = fileManager.GetYAMLs(PathEnum.ContestMasterdatas).Find(y => y.MonoBehaviour?.Name == "ContestConfigDatas").MonoBehaviour as ContestConfigDatas;

            monoBehaviour.ResultMotion = gameData.contestResultMotion.Select(r => new SheetResultMotion()
            {
                ValidFlag = r.validFlag,
                ID = r.id,
                MonsNo = r.monsNo,
                WinAnim = r.winAnim,
                LoseAnim = r.loseAnim,
                WaitAnim = r.waitAnim,
                Duration = r.duration,
            }).ToArray();
        }

        private static void CommitDprBin()
        {
            fileManager.CommitDprBin();
        }

        private static void CommitGlobalMetadata()
        {
            fileManager.CommitGlobalMetadata();
        }

        private static void CommitAudio()
        {
            gameData.audioSourceLog ??= fileManager.GetAudioSourceLog();
            gameData.audioSourceLog.Clear();
            Dictionary<uint, WwiseObject> lookup = gameData.audioData.objectsByID;

            foreach (Pokemon p in gameData.personalEntries)
            {
                IEnumerable<string> eventNames = PokemonInserter.GetWwiseEvents(p.dexID, p.formID).Take(5);
                foreach (string eventName in eventNames)
                {
                    uint eventID = FNV132(eventName);
                    if (!lookup.TryGetValue(eventID, out WwiseObject ewo)) continue;
                    Event e = (Event)ewo;
                    if (!lookup.TryGetValue(e.actionIDs.First(), out WwiseObject apwo)) continue;
                    ActionPlay ap = (ActionPlay)apwo;
                    if (!lookup.TryGetValue(ap.idExt, out WwiseObject swo)) continue;
                    if (swo is not Sound) continue;
                    Sound s = (Sound)swo;
                    gameData.audioSourceLog.Append(eventName + " → " + s.bankSourceData.mediaInformation.sourceID + "\n");
                }
            }

            fileManager.CommitAudio();
        }

        private static void CommitMoves()
        {
            AssetTypeValueField monoBehaviour = fileManager.GetMonoBehaviours(PathEnum.PersonalMasterdatas).Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "WazaTable");
            AssetTypeValueField animationData = fileManager.GetMonoBehaviours(PathEnum.BattleMasterdatas).Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "BattleDataTable");

            AssetTypeValueField[] moveFields = monoBehaviour.children[4].children[0].children;
            AssetTypeValueField[] animationFields = animationData.children[8].children[0].children;
            AssetTypeValueField[] textFields = FindLabelArrayOfMessageFile("ss_wazaname", Language.English);
            for (int moveID = 0; moveID < moveFields.Length; moveID++)
            {
                Move move = gameData.moves[moveID];
                moveFields[moveID].children[0].GetValue().Set(move.moveID);
                moveFields[moveID].children[1].GetValue().Set(move.isValid);
                moveFields[moveID].children[2].GetValue().Set(move.typingID);
                moveFields[moveID].children[3].GetValue().Set(move.category);
                moveFields[moveID].children[4].GetValue().Set(move.damageCategoryID);
                moveFields[moveID].children[5].GetValue().Set(move.power);
                moveFields[moveID].children[6].GetValue().Set(move.hitPer);
                moveFields[moveID].children[7].GetValue().Set(move.basePP);
                moveFields[moveID].children[8].GetValue().Set(move.priority);
                moveFields[moveID].children[9].GetValue().Set(move.hitCountMax);
                moveFields[moveID].children[10].GetValue().Set(move.hitCountMin);
                moveFields[moveID].children[11].GetValue().Set(move.sickID);
                moveFields[moveID].children[12].GetValue().Set(move.sickPer);
                moveFields[moveID].children[13].GetValue().Set(move.sickCont);
                moveFields[moveID].children[14].GetValue().Set(move.sickTurnMin);
                moveFields[moveID].children[15].GetValue().Set(move.sickTurnMax);
                moveFields[moveID].children[16].GetValue().Set(move.criticalRank);
                moveFields[moveID].children[17].GetValue().Set(move.shrinkPer);
                moveFields[moveID].children[18].GetValue().Set(move.aiSeqNo);
                moveFields[moveID].children[19].GetValue().Set(move.damageRecoverRatio);
                moveFields[moveID].children[20].GetValue().Set(move.hpRecoverRatio);
                moveFields[moveID].children[21].GetValue().Set(move.target);
                moveFields[moveID].children[22].GetValue().Set(move.rankEffType1);
                moveFields[moveID].children[23].GetValue().Set(move.rankEffType2);
                moveFields[moveID].children[24].GetValue().Set(move.rankEffType3);
                moveFields[moveID].children[25].GetValue().Set(move.rankEffValue1);
                moveFields[moveID].children[26].GetValue().Set(move.rankEffValue2);
                moveFields[moveID].children[27].GetValue().Set(move.rankEffValue3);
                moveFields[moveID].children[28].GetValue().Set(move.rankEffPer1);
                moveFields[moveID].children[29].GetValue().Set(move.rankEffPer2);
                moveFields[moveID].children[30].GetValue().Set(move.rankEffPer3);
                moveFields[moveID].children[31].GetValue().Set(move.flags);
                moveFields[moveID].children[32].GetValue().Set(move.contestWazaNo);
                animationFields[moveID].children[1].GetValue().Set(move.cmdSeqName);
                animationFields[moveID].children[2].GetValue().Set(move.cmdSeqNameLegend);
                animationFields[moveID].children[3].GetValue().Set(move.notShortenTurnType0);
                animationFields[moveID].children[4].GetValue().Set(move.notShortenTurnType1);
                animationFields[moveID].children[5].GetValue().Set(move.turnType1);
                animationFields[moveID].children[6].GetValue().Set(move.turnType2);
                animationFields[moveID].children[7].GetValue().Set(move.turnType3);
                animationFields[moveID].children[8].GetValue().Set(move.turnType4);
            }

            fileManager.WriteMonoBehaviour(PathEnum.PersonalMasterdatas, monoBehaviour);
            fileManager.WriteMonoBehaviour(PathEnum.BattleMasterdatas, animationData);
        }

        private static void CommitMovesYAML()
        {
            var monoBehaviour = fileManager.GetYAMLs(PathEnum.PersonalMasterdatas).Find(y => y.MonoBehaviour?.Name == "WazaTable").MonoBehaviour as WazaTable;
            var animationData = fileManager.GetYAMLs(PathEnum.BattleMasterdatas).Find(y => y.MonoBehaviour?.Name == "BattleDataTable").MonoBehaviour as BattleDataTable;

            monoBehaviour.Waza = gameData.moves.Select(m => new SheetWaza()
            {
                WazaNo = m.moveID,
                IsValid = m.isValid,
                Type = m.typingID,
                Category = m.category,
                DamageType = m.damageCategoryID,
                Power = m.power,
                HitPer = m.hitPer,
                BasePP = m.basePP,
                Priority = m.priority,
                HitCountMax = m.hitCountMax,
                HitCountMin = m.hitCountMin,
                SickID = m.sickID,
                SickPer = m.sickPer,
                SickCont = m.sickCont,
                SickTurnMin = m.sickTurnMin,
                SickTurnMax = m.sickTurnMax,
                CriticalRank = m.criticalRank,
                ShrinkPer = m.shrinkPer,
                AISeqNo = m.aiSeqNo,
                DamageRecoverRatio = m.damageRecoverRatio,
                HPRecoverRatio = m.hpRecoverRatio,
                Target = m.target,
                RankEffType1 = m.rankEffType1,
                RankEffType2 = m.rankEffType2,
                RankEffType3 = m.rankEffType3,
                RankEffValue1 = m.rankEffValue1,
                RankEffValue2 = m.rankEffValue2,
                RankEffValue3 = m.rankEffValue3,
                RankEffPer1 = m.rankEffPer1,
                RankEffPer2 = m.rankEffPer2,
                RankEffPer3 = m.rankEffPer3,
                Flags = m.flags,
                ContestWazaNo = m.contestWazaNo,
            }).ToArray();

            animationData.BattleWazaData = gameData.moves.Select(m => new SheetBattleWazaData()
            {
                WazaNo = m.moveID,
                CmdSeqName = m.cmdSeqName,
                CmdSeqNameLegend = m.cmdSeqNameLegend,
                NotShortenTurnType0 = m.notShortenTurnType0,
                NotShortenTurnType1 = m.notShortenTurnType1,
                TurnType1 = m.turnType1,
                TurnType2 = m.turnType2,
                TurnType3 = m.turnType3,
                TurnType4 = m.turnType4,
            }).ToArray();
        }

        /// <summary>
        ///  Updates loaded bundle with TMs.
        /// </summary>
        private static void CommitTMs()
        {
            AssetTypeValueField monoBehaviour = fileManager.GetMonoBehaviours(PathEnum.PersonalMasterdatas).Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "ItemTable");

            AssetTypeValueField[] tmFields = monoBehaviour.children[5].children[0].children;
            AssetTypeValueField[] textFields = FindLabelArrayOfMessageFile("ss_itemname", Language.English);
            for (int tmID = 0; tmID < tmFields.Length; tmID++)
            {
                TM tm = gameData.tms[tmID];
                tmFields[tmID].children[0].GetValue().Set(tm.itemID);
                tmFields[tmID].children[1].GetValue().Set(tm.machineNo);
                tmFields[tmID].children[2].GetValue().Set(tm.moveID);
            }

            fileManager.WriteMonoBehaviour(PathEnum.PersonalMasterdatas, monoBehaviour);
        }

        /// <summary>
        ///  Updates loaded yaml with TMs.
        /// </summary>
        private static void CommitTMsYAML()
        {
            var monoBehaviour = fileManager.GetYAMLs(PathEnum.PersonalMasterdatas).Find(y => y.MonoBehaviour?.Name == "ItemTable").MonoBehaviour as ItemTable;

            monoBehaviour.WazaMachine = gameData.tms.Select(t => new SheetWazaMachine()
            {
                ItemNo = t.itemID,
                MachineNo = t.machineNo,
                WazaNo = t.moveID,
            }).ToArray();
        }

        /// <summary>
        ///  Updates loaded bundle with Items.
        /// </summary>
        private static void CommitItems()
        {
            AssetTypeValueField monoBehaviour = fileManager.GetMonoBehaviours(PathEnum.PersonalMasterdatas).Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "ItemTable");

            AssetTypeValueField[] itemFields = monoBehaviour.children[4].children[0].children;
            AssetTypeValueField[] textFields = FindLabelArrayOfMessageFile("ss_itemname", Language.English);
            for (int itemIdx = 0; itemIdx < itemFields.Length; itemIdx++)
            {
                Item item = gameData.items[itemIdx];
                itemFields[itemIdx].children[0].GetValue().Set(item.itemID);
                itemFields[itemIdx].children[1].GetValue().Set(item.type);
                itemFields[itemIdx].children[2].GetValue().Set(item.iconID);
                itemFields[itemIdx].children[3].GetValue().Set(item.price);
                itemFields[itemIdx].children[4].GetValue().Set(item.bpPrice);
                itemFields[itemIdx].children[7].GetValue().Set(item.nageAtc);
                itemFields[itemIdx].children[8].GetValue().Set(item.sizenAtc);
                itemFields[itemIdx].children[9].GetValue().Set(item.sizenType);
                itemFields[itemIdx].children[10].GetValue().Set(item.tuibamuEff);
                itemFields[itemIdx].children[11].GetValue().Set(item.sort);
                itemFields[itemIdx].children[12].GetValue().Set(item.group);
                itemFields[itemIdx].children[13].GetValue().Set(item.groupID);
                itemFields[itemIdx].children[14].GetValue().Set(item.fldPocket);
                itemFields[itemIdx].children[15].GetValue().Set(item.fieldFunc);
                itemFields[itemIdx].children[16].GetValue().Set(item.battleFunc);
                itemFields[itemIdx].children[18].GetValue().Set(item.criticalRanks);
                itemFields[itemIdx].children[19].GetValue().Set(item.atkStages);
                itemFields[itemIdx].children[20].GetValue().Set(item.defStages);
                itemFields[itemIdx].children[21].GetValue().Set(item.spdStages);
                itemFields[itemIdx].children[22].GetValue().Set(item.accStages);
                itemFields[itemIdx].children[23].GetValue().Set(item.spAtkStages);
                itemFields[itemIdx].children[24].GetValue().Set(item.spDefStages);
                itemFields[itemIdx].children[25].GetValue().Set(item.ppRestoreAmount);
                itemFields[itemIdx].children[26].GetValue().Set(item.hpEvIncrease);
                itemFields[itemIdx].children[27].GetValue().Set(item.atkEvIncrease);
                itemFields[itemIdx].children[28].GetValue().Set(item.defEvIncrease);
                itemFields[itemIdx].children[29].GetValue().Set(item.spdEvIncrease);
                itemFields[itemIdx].children[30].GetValue().Set(item.spAtkEvIncrease);
                itemFields[itemIdx].children[31].GetValue().Set(item.spDefEvIncrease);
                itemFields[itemIdx].children[32].GetValue().Set(item.friendshipIncrease1);
                itemFields[itemIdx].children[33].GetValue().Set(item.friendshipIncrease2);
                itemFields[itemIdx].children[34].GetValue().Set(item.friendshipIncrease3);
                itemFields[itemIdx].children[35].GetValue().Set(item.hpRestoreAmount);
                itemFields[itemIdx].children[36].GetValue().Set(item.flags0);
            }

            fileManager.WriteMonoBehaviour(PathEnum.PersonalMasterdatas, monoBehaviour);
        }

        /// <summary>
        ///  Updates loaded bundle with Items.
        /// </summary>
        private static void CommitItemsYAML()
        {
            var monoBehaviour = fileManager.GetYAMLs(PathEnum.PersonalMasterdatas).Find(y => y.MonoBehaviour?.Name == "ItemTable").MonoBehaviour as ItemTable;

            monoBehaviour.Item = gameData.items.Select((it, i) => new SheetItem()
            {
                No = it.itemID,
                Type = it.type,
                IconID = it.iconID,
                Price = it.price,
                BPPrice = it.bpPrice,
                Eqp = i < monoBehaviour.Item.Length ? monoBehaviour.Item[i].Eqp : byte.MinValue,
                Atc = i < monoBehaviour.Item.Length ? monoBehaviour.Item[i].Atc : byte.MinValue,
                NageAtc = it.nageAtc,
                SizenAtc = it.sizenAtc,
                SizenType = it.sizenType,
                TuibamuEff = it.tuibamuEff,
                Sort = it.sort,
                Group = it.group,
                GroupID = it.groupID,
                FldPocket = it.fldPocket,
                FieldFunc = it.fieldFunc,
                BattleFunc = it.battleFunc,
                WkCmn = i < monoBehaviour.Item.Length ? monoBehaviour.Item[i].WkCmn : byte.MinValue,
                WkCriticalUp = it.criticalRanks,
                WkAtcUp = it.atkStages,
                WkDefUp = it.defStages,
                WkAgiUp = it.spdStages,
                WkHitUp = it.accStages,
                WkSpaUp = it.spAtkStages,
                WkSpdUp = it.spDefStages,
                WkPrmPPRcv = it.ppRestoreAmount,
                WkPrmHPExp = it.hpEvIncrease,
                WkPrmPowExp = it.atkEvIncrease,
                WkPrmDefExp = it.defEvIncrease,
                WkPrmAgiExp = it.spdEvIncrease,
                WkPrmSpaExp = it.spAtkEvIncrease,
                WkPrmSpdExp = it.spDefEvIncrease,
                WkFriend1 = it.friendshipIncrease1,
                WkFriend2 = it.friendshipIncrease2,
                WkFriend3 = it.friendshipIncrease3,
                WkPrmHPRcv = it.hpRestoreAmount,
                Flags0 = it.flags0,
            }).ToArray();
        }

        private static void CommitPokemonInfo()
        {
            gameData.pokemonInfos.Sort();

            List<AssetTypeValueField> monoBehaviours = fileManager.GetMonoBehaviours(PathEnum.DprMasterdatas);
            AssetTypeValueField pokemonInfo = monoBehaviours.Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "PokemonInfo");

            AssetTypeValueField[] PokemonInfoCatalog = pokemonInfo["Catalog"].children[0].children;
            AssetTypeValueField catalogRef = PokemonInfoCatalog[0];

            AssetTypeValueField[] PokemonInfoTrearuki = null;
            AssetTypeValueField trearukiRef = null;
            if (pokemonInfo["Trearuki"].children != null)
            {
                PokemonInfoTrearuki = pokemonInfo["Trearuki"].children[0].children;
                trearukiRef = PokemonInfoTrearuki[0];
            }

            List<AssetTypeValueField> newCatalogs = new();
            List<AssetTypeValueField> newTrearukis = new();
            foreach (Masterdatas.PokemonInfoCatalog pokemonInfoCatalog in gameData.pokemonInfos)
            {
                AssetTypeValueField catalogBaseField = ValueBuilder.DefaultValueFieldFromTemplate(catalogRef.GetTemplateField());
                AssetTypeValueField trearukiBaseField = null;
                if (trearukiRef != null)
                    trearukiBaseField = ValueBuilder.DefaultValueFieldFromTemplate(trearukiRef.GetTemplateField());
                Masterdatas.Trearuki t = pokemonInfoCatalog.trearuki;

                catalogBaseField["UniqueID"].GetValue().Set(pokemonInfoCatalog.UniqueID);
                catalogBaseField["No"].GetValue().Set(pokemonInfoCatalog.No);
                catalogBaseField["SinnohNo"].GetValue().Set(pokemonInfoCatalog.SinnohNo);
                catalogBaseField["MonsNo"].GetValue().Set(pokemonInfoCatalog.MonsNo);
                catalogBaseField["FormNo"].GetValue().Set(pokemonInfoCatalog.FormNo);
                catalogBaseField["Sex"].GetValue().Set(pokemonInfoCatalog.Sex);
                catalogBaseField["Rare"].GetValue().Set(pokemonInfoCatalog.Rare);
                catalogBaseField["AssetBundleName"].GetValue().Set(pokemonInfoCatalog.AssetBundleName);
                catalogBaseField["BattleScale"].GetValue().Set(pokemonInfoCatalog.BattleScale);
                catalogBaseField["ContestScale"].GetValue().Set(pokemonInfoCatalog.ContestScale);
                catalogBaseField["ContestSize"].GetValue().Set(pokemonInfoCatalog.ContestSize);
                catalogBaseField["FieldScale"].GetValue().Set(pokemonInfoCatalog.FieldScale);
                catalogBaseField["FieldChikaScale"].GetValue().Set(pokemonInfoCatalog.FieldChikaScale);
                catalogBaseField["StatueScale"].GetValue().Set(pokemonInfoCatalog.StatueScale);
                catalogBaseField["FieldWalkingScale"].GetValue().Set(pokemonInfoCatalog.FieldWalkingScale);
                catalogBaseField["FieldFureaiScale"].GetValue().Set(pokemonInfoCatalog.FieldFureaiScale);
                catalogBaseField["MenuScale"].GetValue().Set(pokemonInfoCatalog.MenuScale);
                catalogBaseField["ModelMotion"].GetValue().Set(pokemonInfoCatalog.ModelMotion);
                catalogBaseField["ModelOffset"].children[0].GetValue().Set(pokemonInfoCatalog.ModelOffset.X);
                catalogBaseField["ModelOffset"].children[1].GetValue().Set(pokemonInfoCatalog.ModelOffset.Y);
                catalogBaseField["ModelOffset"].children[2].GetValue().Set(pokemonInfoCatalog.ModelOffset.Z);
                catalogBaseField["ModelRotationAngle"].children[0].GetValue().Set(pokemonInfoCatalog.ModelRotationAngle.X);
                catalogBaseField["ModelRotationAngle"].children[1].GetValue().Set(pokemonInfoCatalog.ModelRotationAngle.Y);
                catalogBaseField["ModelRotationAngle"].children[2].GetValue().Set(pokemonInfoCatalog.ModelRotationAngle.Z);
                catalogBaseField["DistributionScale"].GetValue().Set(pokemonInfoCatalog.DistributionScale);
                catalogBaseField["DistributionModelMotion"].GetValue().Set(pokemonInfoCatalog.DistributionModelMotion);
                catalogBaseField["DistributionModelOffset"].children[0].GetValue().Set(pokemonInfoCatalog.DistributionModelOffset.X);
                catalogBaseField["DistributionModelOffset"].children[1].GetValue().Set(pokemonInfoCatalog.DistributionModelOffset.Y);
                catalogBaseField["DistributionModelOffset"].children[2].GetValue().Set(pokemonInfoCatalog.DistributionModelOffset.Z);
                catalogBaseField["DistributionModelRotationAngle"].children[0].GetValue().Set(pokemonInfoCatalog.DistributionModelRotationAngle.X);
                catalogBaseField["DistributionModelRotationAngle"].children[1].GetValue().Set(pokemonInfoCatalog.DistributionModelRotationAngle.Y);
                catalogBaseField["DistributionModelRotationAngle"].children[2].GetValue().Set(pokemonInfoCatalog.DistributionModelRotationAngle.Z);
                catalogBaseField["VoiceScale"].GetValue().Set(pokemonInfoCatalog.VoiceScale);
                catalogBaseField["VoiceModelMotion"].GetValue().Set(pokemonInfoCatalog.VoiceModelMotion);
                catalogBaseField["VoiceModelOffset"].children[0].GetValue().Set(pokemonInfoCatalog.VoiceModelOffset.X);
                catalogBaseField["VoiceModelOffset"].children[1].GetValue().Set(pokemonInfoCatalog.VoiceModelOffset.Y);
                catalogBaseField["VoiceModelOffset"].children[2].GetValue().Set(pokemonInfoCatalog.VoiceModelOffset.Z);
                catalogBaseField["VoiceModelRotationAngle"].children[0].GetValue().Set(pokemonInfoCatalog.VoiceModelRotationAngle.X);
                catalogBaseField["VoiceModelRotationAngle"].children[1].GetValue().Set(pokemonInfoCatalog.VoiceModelRotationAngle.Y);
                catalogBaseField["VoiceModelRotationAngle"].children[2].GetValue().Set(pokemonInfoCatalog.VoiceModelRotationAngle.Z);
                catalogBaseField["CenterPointOffset"].children[0].GetValue().Set(pokemonInfoCatalog.CenterPointOffset.X);
                catalogBaseField["CenterPointOffset"].children[1].GetValue().Set(pokemonInfoCatalog.CenterPointOffset.Y);
                catalogBaseField["CenterPointOffset"].children[2].GetValue().Set(pokemonInfoCatalog.CenterPointOffset.Z);
                catalogBaseField["RotationLimitAngle"].children[0].GetValue().Set(pokemonInfoCatalog.RotationLimitAngle.X);
                catalogBaseField["RotationLimitAngle"].children[1].GetValue().Set(pokemonInfoCatalog.RotationLimitAngle.Y);
                catalogBaseField["StatusScale"].GetValue().Set(pokemonInfoCatalog.StatusScale);
                catalogBaseField["StatusModelMotion"].GetValue().Set(pokemonInfoCatalog.StatusModelMotion);
                catalogBaseField["StatusModelOffset"].children[0].GetValue().Set(pokemonInfoCatalog.StatusModelOffset.X);
                catalogBaseField["StatusModelOffset"].children[1].GetValue().Set(pokemonInfoCatalog.StatusModelOffset.Y);
                catalogBaseField["StatusModelOffset"].children[2].GetValue().Set(pokemonInfoCatalog.StatusModelOffset.Z);
                catalogBaseField["StatusModelRotationAngle"].children[0].GetValue().Set(pokemonInfoCatalog.StatusModelRotationAngle.X);
                catalogBaseField["StatusModelRotationAngle"].children[1].GetValue().Set(pokemonInfoCatalog.StatusModelRotationAngle.Y);
                catalogBaseField["StatusModelRotationAngle"].children[2].GetValue().Set(pokemonInfoCatalog.StatusModelRotationAngle.Z);
                catalogBaseField["BoxScale"].GetValue().Set(pokemonInfoCatalog.BoxScale);
                catalogBaseField["BoxModelMotion"].GetValue().Set(pokemonInfoCatalog.BoxModelMotion);
                catalogBaseField["BoxModelOffset"].children[0].GetValue().Set(pokemonInfoCatalog.BoxModelOffset.X);
                catalogBaseField["BoxModelOffset"].children[1].GetValue().Set(pokemonInfoCatalog.BoxModelOffset.Y);
                catalogBaseField["BoxModelOffset"].children[2].GetValue().Set(pokemonInfoCatalog.BoxModelOffset.Z);
                catalogBaseField["BoxModelRotationAngle"].children[0].GetValue().Set(pokemonInfoCatalog.BoxModelRotationAngle.X);
                catalogBaseField["BoxModelRotationAngle"].children[1].GetValue().Set(pokemonInfoCatalog.BoxModelRotationAngle.Y);
                catalogBaseField["BoxModelRotationAngle"].children[2].GetValue().Set(pokemonInfoCatalog.BoxModelRotationAngle.Z);
                catalogBaseField["CompareScale"].GetValue().Set(pokemonInfoCatalog.CompareScale);
                catalogBaseField["CompareModelMotion"].GetValue().Set(pokemonInfoCatalog.CompareModelMotion);
                catalogBaseField["CompareModelOffset"].children[0].GetValue().Set(pokemonInfoCatalog.CompareModelOffset.X);
                catalogBaseField["CompareModelOffset"].children[1].GetValue().Set(pokemonInfoCatalog.CompareModelOffset.Y);
                catalogBaseField["CompareModelOffset"].children[2].GetValue().Set(pokemonInfoCatalog.CompareModelOffset.Z);
                catalogBaseField["CompareModelRotationAngle"].children[0].GetValue().Set(pokemonInfoCatalog.CompareModelRotationAngle.X);
                catalogBaseField["CompareModelRotationAngle"].children[1].GetValue().Set(pokemonInfoCatalog.CompareModelRotationAngle.Y);
                catalogBaseField["CompareModelRotationAngle"].children[2].GetValue().Set(pokemonInfoCatalog.CompareModelRotationAngle.Z);
                catalogBaseField["BrakeStart"].GetValue().Set(pokemonInfoCatalog.BrakeStart);
                catalogBaseField["BrakeEnd"].GetValue().Set(pokemonInfoCatalog.BrakeEnd);
                catalogBaseField["WalkSpeed"].GetValue().Set(pokemonInfoCatalog.WalkSpeed);
                catalogBaseField["RunSpeed"].GetValue().Set(pokemonInfoCatalog.RunSpeed);
                catalogBaseField["WalkStart"].GetValue().Set(pokemonInfoCatalog.WalkStart);
                catalogBaseField["RunStart"].GetValue().Set(pokemonInfoCatalog.RunStart);
                catalogBaseField["BodySize"].GetValue().Set(pokemonInfoCatalog.BodySize);
                catalogBaseField["AppearLimit"].GetValue().Set(pokemonInfoCatalog.AppearLimit);
                catalogBaseField["MoveType"].GetValue().Set(pokemonInfoCatalog.MoveType);
                catalogBaseField["GroundEffect"].GetValue().Set(pokemonInfoCatalog.GroundEffect);
                catalogBaseField["Waitmoving"].GetValue().Set(pokemonInfoCatalog.Waitmoving);
                catalogBaseField["BattleAjustHeight"].GetValue().Set(pokemonInfoCatalog.BattleAjustHeight);

                if (trearukiBaseField != null)
                {
                    trearukiBaseField["Enable"].GetValue().Set(t.enable ? 1 : 0);

                    List<AssetTypeValueField> newAnimeIndices = new();
                    for (int i = 0; i < t.animeIndex.Count; i++)
                    {
                        AssetTypeValueField animeIndexBaseField = ValueBuilder.DefaultValueFieldFromTemplate(trearukiRef["AnimeIndex"].children[0].children[0].GetTemplateField());
                        animeIndexBaseField.GetValue().Set(t.animeIndex[i]);
                        newAnimeIndices.Add(animeIndexBaseField);
                    }
                    trearukiBaseField["AnimeIndex"].children[0].SetChildrenList(newAnimeIndices.ToArray());

                    List<AssetTypeValueField> newAnimeDurations = new();
                    for (int i = 0; i < t.animeDuration.Count; i++)
                    {
                        AssetTypeValueField animeDurationBaseField = ValueBuilder.DefaultValueFieldFromTemplate(trearukiRef["AnimeDuration"].children[0].children[0].GetTemplateField());
                        animeDurationBaseField.GetValue().Set(t.animeDuration[i]);
                        newAnimeDurations.Add(animeDurationBaseField);
                    }
                    trearukiBaseField["AnimeDuration"].children[0].SetChildrenList(newAnimeDurations.ToArray());

                    newTrearukis.Add(trearukiBaseField);
                }
                newCatalogs.Add(catalogBaseField);
            }

            pokemonInfo["Catalog"].children[0].SetChildrenList(newCatalogs.ToArray());
            pokemonInfo["Trearuki"].children?[0].SetChildrenList(newTrearukis.ToArray());

            fileManager.WriteMonoBehaviour(PathEnum.DprMasterdatas, pokemonInfo);
        }

        private static void CommitPokemonInfoYAML()
        {
            gameData.pokemonInfos.Sort();

            var pokemonInfo = fileManager.GetYAMLs(PathEnum.DprMasterdatas).Find(y => y.MonoBehaviour?.Name == "PokemonInfo").MonoBehaviour as PokemonInfo;

            pokemonInfo.Catalog = gameData.pokemonInfos.Select(p => new SheetCatalog()
            {
                UniqueID = p.UniqueID,
                No = p.No,
                SinnohNo = p.SinnohNo,
                MonsNo = p.MonsNo,
                FormNo = p.FormNo,
                Sex = p.Sex,
                Rare = p.Rare ? (byte)1 : (byte)0,
                AssetBundleName = p.AssetBundleName,
                BattleScale = p.BattleScale,
                ContestScale = p.ContestScale,
                FieldScale = p.FieldScale,
                FieldChikaScale = p.FieldChikaScale,
                StatueScale = p.StatueScale,
                FieldWalkingScale = p.FieldWalkingScale,
                FieldFureaiScale = p.FieldFureaiScale,
                MenuScale = p.MenuScale,
                ModelMotion = p.ModelMotion,
                ModelOffset = new UnityVector3() { X = p.ModelOffset.X, Y = p.ModelOffset.Y, Z = p.ModelOffset.Z },
                ModelRotationAngle = new UnityVector3() { X = p.ModelRotationAngle.X, Y = p.ModelRotationAngle.Y, Z = p.ModelRotationAngle.Z },
                DistributionScale = p.DistributionScale,
                DistributionModelMotion = p.DistributionModelMotion,
                DistributionModelOffset = new UnityVector3() { X = p.DistributionModelOffset.X, Y = p.DistributionModelOffset.Y, Z = p.DistributionModelOffset.Z },
                DistributionModelRotationAngle = new UnityVector3() { X = p.DistributionModelRotationAngle.X, Y = p.DistributionModelRotationAngle.Y, Z = p.DistributionModelRotationAngle.Z },
                VoiceScale = p.VoiceScale,
                VoiceModelMotion = p.VoiceModelMotion,
                VoiceModelOffset = new UnityVector3() { X = p.VoiceModelOffset.X, Y = p.VoiceModelOffset.Y, Z = p.VoiceModelOffset.Z },
                VoiceModelRotationAngle = new UnityVector3() { X = p.VoiceModelRotationAngle.X, Y = p.VoiceModelRotationAngle.Y, Z = p.VoiceModelRotationAngle.Z },
                CenterPointOffset = new UnityVector3() { X = p.CenterPointOffset.X, Y = p.CenterPointOffset.Y, Z = p.CenterPointOffset.Z },
                RotationLimitAngle = new UnityVector2() { X = p.RotationLimitAngle.X, Y = p.RotationLimitAngle.Y },
                StatusScale = p.StatusScale,
                StatusModelMotion = p.StatusModelMotion,
                StatusModelOffset = new UnityVector3() { X = p.StatusModelOffset.X, Y = p.StatusModelOffset.Y, Z = p.StatusModelOffset.Z },
                StatusModelRotationAngle = new UnityVector3() { X = p.StatusModelRotationAngle.X, Y = p.StatusModelRotationAngle.Y, Z = p.StatusModelRotationAngle.Z },
                BoxScale = p.BoxScale,
                BoxModelMotion = p.BoxModelMotion,
                BoxModelOffset = new UnityVector3() { X = p.BoxModelOffset.X, Y = p.BoxModelOffset.Y, Z = p.BoxModelOffset.Z },
                BoxModelRotationAngle = new UnityVector3() { X = p.BoxModelRotationAngle.X, Y = p.BoxModelRotationAngle.Y, Z = p.BoxModelRotationAngle.Z },
                CompareScale = p.CompareScale,
                CompareModelMotion = p.CompareModelMotion,
                CompareModelOffset = new UnityVector3() { X = p.CompareModelOffset.X, Y = p.CompareModelOffset.Y, Z = p.CompareModelOffset.Z },
                CompareModelRotationAngle = new UnityVector3() { X = p.CompareModelRotationAngle.X, Y = p.CompareModelRotationAngle.Y, Z = p.CompareModelRotationAngle.Z },
                BrakeStart = p.BrakeStart,
                BrakeEnd = p.BrakeEnd,
                WalkSpeed = p.WalkSpeed,
                RunSpeed = p.RunSpeed,
                WalkStart = p.WalkStart,
                RunStart = p.RunStart,
                BodySize = p.BodySize,
                AppearLimit = p.AppearLimit,
                MoveType = (int)p.MoveType,
                GroundEffect = p.GroundEffect ? (byte)1 : (byte)0,
                Waitmoving = p.Waitmoving ? (byte)1 : (byte)0,
                BattleAjustHeight = p.BattleAjustHeight,
            }).ToArray();

            pokemonInfo.Trearuki = gameData.pokemonInfos.Select(p => new SheetTrearuki()
            {
                Enable = p.trearuki.enable ? (byte)1 : (byte)0,
                AnimeIndex = p.trearuki.animeIndex.ToArray(),
                AnimeDuration = p.trearuki.animeDuration.ToArray(),
            }).ToArray();
        }

        private static void CommitMotionTimingData()
        {
            gameData.motionTimingData.Sort();

            List<AssetTypeValueField> monoBehaviours = fileManager.GetMonoBehaviours(PathEnum.BattleMasterdatas);
            AssetTypeValueField BattleDataTable = monoBehaviours.Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "BattleDataTable");

            AssetTypeValueField[] MotionTimingData = BattleDataTable["MotionTimingData"].children[0].children;
            AssetTypeTemplateField templateField = new();

            AssetTypeValueField motionTimingDataRef = MotionTimingData[0];
            List<AssetTypeValueField> newMotionTimingData = new();
            foreach (BattleMasterdatas.MotionTimingData motionTimingData in gameData.motionTimingData)
            {
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromTemplate(motionTimingDataRef.GetTemplateField());
                baseField["MonsNo"].GetValue().Set(motionTimingData.MonsNo);
                baseField["FormNo"].GetValue().Set(motionTimingData.FormNo);
                baseField["Sex"].GetValue().Set(motionTimingData.Sex);
                baseField["Buturi01"].GetValue().Set(motionTimingData.Buturi01);
                baseField["Buturi02"].GetValue().Set(motionTimingData.Buturi02);
                baseField["Buturi03"].GetValue().Set(motionTimingData.Buturi03);
                baseField["Tokusyu01"].GetValue().Set(motionTimingData.Tokusyu01);
                baseField["Tokusyu02"].GetValue().Set(motionTimingData.Tokusyu02);
                baseField["Tokusyu03"].GetValue().Set(motionTimingData.Tokusyu03);
                baseField["BodyBlow"].GetValue().Set(motionTimingData.BodyBlow);
                baseField["Punch"].GetValue().Set(motionTimingData.Punch);
                baseField["Kick"].GetValue().Set(motionTimingData.Kick);
                baseField["Tail"].GetValue().Set(motionTimingData.Tail);
                baseField["Bite"].GetValue().Set(motionTimingData.Bite);
                baseField["Peck"].GetValue().Set(motionTimingData.Peck);
                baseField["Radial"].GetValue().Set(motionTimingData.Radial);
                baseField["Cry"].GetValue().Set(motionTimingData.Cry);
                baseField["Dust"].GetValue().Set(motionTimingData.Dust);
                baseField["Shot"].GetValue().Set(motionTimingData.Shot);
                baseField["Guard"].GetValue().Set(motionTimingData.Guard);
                baseField["LandingFall"].GetValue().Set(motionTimingData.LandingFall);
                baseField["LandingFallEase"].GetValue().Set(motionTimingData.LandingFallEase);
                newMotionTimingData.Add(baseField);
            }

            BattleDataTable["MotionTimingData"].children[0].SetChildrenList(newMotionTimingData.ToArray());

            fileManager.WriteMonoBehaviour(PathEnum.BattleMasterdatas, BattleDataTable);
        }

        private static void CommitMotionTimingDataYAML()
        {
            gameData.motionTimingData.Sort();

            var monoBehaviours = fileManager.GetYAMLs(PathEnum.BattleMasterdatas).Find(y => y.MonoBehaviour?.Name == "BattleDataTable").MonoBehaviour as BattleDataTable;
            monoBehaviours.MotionTimingData = gameData.motionTimingData.Select(m => new SheetMotionTimingData()
            {
                MonsNo = m.MonsNo,
                FormNo = m.FormNo,
                Sex = m.Sex,
                Buturi01 = m.Buturi01,
                Buturi02 = m.Buturi02,
                Buturi03 = m.Buturi03,
                Tokusyu01 = m.Tokusyu01,
                Tokusyu02 = m.Tokusyu02,
                Tokusyu03 = m.Tokusyu03,
                BodyBlow = m.BodyBlow,
                Punch = m.Punch,
                Kick = m.Kick,
                Tail = m.Tail,
                Bite = m.Bite,
                Peck = m.Peck,
                Radial = m.Radial,
                Cry = m.Cry,
                Dust = m.Dust,
                Shot = m.Shot,
                Guard = m.Guard,
                LandingFall = m.LandingFall,
                LandingFallEase = m.LandingFallEase,
            }).ToArray();
        }

        private static void CommitAddPersonalTable()
        {
            gameData.addPersonalTables.Sort();

            List<AssetTypeValueField> monoBehaviours = fileManager.GetMonoBehaviours(PathEnum.PersonalMasterdatas);
            AssetTypeValueField AddPersonalTable = monoBehaviours.Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "AddPersonalTable");

            AssetTypeValueField[] addPersonals = AddPersonalTable["AddPersonal"].children[0].children;

            AssetTypeValueField addPersonalRef = addPersonals[0];

            List<AssetTypeValueField> newAddPersonals = new();
            foreach (PersonalMasterdatas.AddPersonalTable addPersonal in gameData.addPersonalTables)
            {
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromTemplate(addPersonalRef.GetTemplateField());
                baseField["valid_flag"].GetValue().Set(addPersonal.valid_flag ? 1 : 0);
                baseField["monsno"].GetValue().Set(addPersonal.monsno);
                baseField["formno"].GetValue().Set(addPersonal.formno);
                baseField["isEnableSynchronize"].GetValue().Set(addPersonal.isEnableSynchronize);
                baseField["escape"].GetValue().Set(addPersonal.escape);
                baseField["isDisableReverce"].GetValue().Set(addPersonal.isDisableReverce);
                newAddPersonals.Add(baseField);
            }

            AddPersonalTable["AddPersonal"].children[0].SetChildrenList(newAddPersonals.ToArray());

            fileManager.WriteMonoBehaviour(PathEnum.PersonalMasterdatas, AddPersonalTable);
        }


        private static void CommitAddPersonalTableYAML()
        {
            gameData.addPersonalTables.Sort();

            var monoBehaviours = fileManager.GetYAMLs(PathEnum.PersonalMasterdatas).Find(y => y.MonoBehaviour?.Name == "AddPersonalTable").MonoBehaviour as AddPersonalTable;
            monoBehaviours.AddPersonal = gameData.addPersonalTables.Select(p => new SheetAddPersonal()
            {
                ValidFlag = p.valid_flag ? (byte)1 : (byte)0,
                Monsno = p.monsno,
                Formno = p.formno,
                IsEnableSynchronize = p.isEnableSynchronize ? (byte)1 : (byte)0,
                Escape = p.escape,
                IsDisableReverce = p.isDisableReverce ? (byte)1 : (byte)0,
            }).ToArray();
        }

        private static void CommitUIMasterdatas()
        {
            gameData.uiPokemonIcon.Sort();
            gameData.uiAshiatoIcon.Sort();
            gameData.uiPokemonVoice.Sort();
            gameData.uiZukanDisplay.Sort();
            gameData.uiZukanCompareHeights.Sort();
            gameData.uiSearchPokeIconSex.Sort();

            List<AssetTypeValueField> monoBehaviours = fileManager.GetMonoBehaviours(PathEnum.UIMasterdatas);

            AssetTypeValueField uiDatabase = monoBehaviours.Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "UIDatabase");
            AssetTypeValueField distributionTable = monoBehaviours.Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "DistributionTable");
            monoBehaviours = new() { uiDatabase, distributionTable };

            // Pokemon Icon
            AssetTypeValueField[] pokemonIcons = uiDatabase["PokemonIcon"].children[0].children;

            AssetTypeValueField pokemonIconRef = pokemonIcons[0];
            List<AssetTypeValueField> newPokemonIcons = new();
            foreach (UIMasterdatas.PokemonIcon pokemonIcon in gameData.uiPokemonIcon)
            {
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromTemplate(pokemonIconRef.GetTemplateField());
                baseField["UniqueID"].GetValue().Set(pokemonIcon.uniqueID);
                baseField["AssetBundleName"].GetValue().Set(pokemonIcon.assetBundleName);
                baseField["AssetName"].GetValue().Set(pokemonIcon.assetName);
                baseField["AssetBundleNameLarge"].GetValue().Set(pokemonIcon.assetBundleNameLarge);
                baseField["AssetNameLarge"].GetValue().Set(pokemonIcon.assetNameLarge);
                baseField["AssetBundleNameDP"].GetValue().Set(pokemonIcon.assetBundleNameDP);
                baseField["AssetNameDP"].GetValue().Set(pokemonIcon.assetNameDP);
                baseField["HallofFameOffset"].children[0].GetValue().Set(pokemonIcon.hallofFameOffset.X);
                baseField["HallofFameOffset"].children[1].GetValue().Set(pokemonIcon.hallofFameOffset.Y);
                newPokemonIcons.Add(baseField);
            }
            uiDatabase["PokemonIcon"].children[0].SetChildrenList(newPokemonIcons.ToArray());

            // Ashiato Icon
            AssetTypeValueField[] ashiatoIcons = uiDatabase["AshiatoIcon"].children[0].children;

            AssetTypeValueField ashiatoIconRef = ashiatoIcons[0];
            List<AssetTypeValueField> newAshiatoIcons = new();
            foreach (UIMasterdatas.AshiatoIcon ashiatoIcon in gameData.uiAshiatoIcon)
            {
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromTemplate(ashiatoIconRef.GetTemplateField());
                baseField["UniqueID"].GetValue().Set(ashiatoIcon.uniqueID);
                baseField["SideIconAssetName"].GetValue().Set(ashiatoIcon.sideIconAssetName);
                baseField["BothIconAssetName"].GetValue().Set(ashiatoIcon.bothIconAssetName);
                newAshiatoIcons.Add(baseField);
            }
            uiDatabase["AshiatoIcon"].children[0].SetChildrenList(newAshiatoIcons.ToArray());


            // Pokemon Voice
            AssetTypeValueField[] pokemonVoices = uiDatabase["PokemonVoice"].children[0].children;

            AssetTypeValueField pokemonVoiceRef = pokemonVoices[0];
            List<AssetTypeValueField> newPokemonVoices = new();
            foreach (UIMasterdatas.PokemonVoice pokemonVoice in gameData.uiPokemonVoice)
            {
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromTemplate(pokemonVoiceRef.GetTemplateField());
                baseField["UniqueID"].GetValue().Set(pokemonVoice.uniqueID);
                baseField["WwiseEvent"].GetValue().Set(pokemonVoice.wwiseEvent);
                baseField["stopEventId"].GetValue().Set(pokemonVoice.stopEventId);
                baseField["CenterPointOffset"].children[0].GetValue().Set(pokemonVoice.centerPointOffset.X);
                baseField["CenterPointOffset"].children[1].GetValue().Set(pokemonVoice.centerPointOffset.Y);
                baseField["CenterPointOffset"].children[2].GetValue().Set(pokemonVoice.centerPointOffset.Z);
                baseField["RotationLimits"].GetValue().Set(pokemonVoice.rotationLimits);
                baseField["RotationLimitAngle"].children[0].GetValue().Set(pokemonVoice.rotationLimitAngle.X);
                baseField["RotationLimitAngle"].children[1].GetValue().Set(pokemonVoice.rotationLimitAngle.Y);
                newPokemonVoices.Add(baseField);
            }
            uiDatabase["PokemonVoice"].children[0].SetChildrenList(newPokemonVoices.ToArray());


            // ZukanDisplay
            AssetTypeValueField[] zukanDisplays = uiDatabase["ZukanDisplay"].children[0].children;

            AssetTypeValueField zukanDisplayRef = zukanDisplays[0];
            List<AssetTypeValueField> newZukanDisplays = new();
            foreach (UIMasterdatas.ZukanDisplay zukanDisplay in gameData.uiZukanDisplay)
            {
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromTemplate(zukanDisplayRef.GetTemplateField());
                baseField["UniqueID"].GetValue().Set(zukanDisplay.uniqueID);
                baseField["MoveLimit"].children[0].GetValue().Set(zukanDisplay.moveLimit.X);
                baseField["MoveLimit"].children[1].GetValue().Set(zukanDisplay.moveLimit.Y);
                baseField["MoveLimit"].children[2].GetValue().Set(zukanDisplay.moveLimit.Z);
                baseField["ModelOffset"].children[0].GetValue().Set(zukanDisplay.modelOffset.X);
                baseField["ModelOffset"].children[1].GetValue().Set(zukanDisplay.modelOffset.Y);
                baseField["ModelOffset"].children[2].GetValue().Set(zukanDisplay.modelOffset.Z);
                baseField["ModelRotationAngle"].children[0].GetValue().Set(zukanDisplay.modelRotationAngle.X);
                baseField["ModelRotationAngle"].children[1].GetValue().Set(zukanDisplay.modelRotationAngle.Y);
                newZukanDisplays.Add(baseField);
            }
            uiDatabase["ZukanDisplay"].children[0].SetChildrenList(newZukanDisplays.ToArray());

            // ZukanCompareHeight
            AssetTypeValueField[] zukanCompareHeights = uiDatabase["ZukanCompareHeight"].children[0].children;

            AssetTypeValueField zukanCompareHeightRef = zukanCompareHeights[0];
            List<AssetTypeValueField> newZukanCompareHeights = new();
            foreach (UIMasterdatas.ZukanCompareHeight zukanCompareHeight in gameData.uiZukanCompareHeights)
            {
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromTemplate(zukanCompareHeightRef.GetTemplateField());
                baseField["UniqueID"].GetValue().Set(zukanCompareHeight.uniqueID);
                baseField["PlayerScaleFactor"].GetValue().Set(zukanCompareHeight.playerScaleFactor);
                baseField["PlayerOffset"].children[0].GetValue().Set(zukanCompareHeight.playerOffset.X);
                baseField["PlayerOffset"].children[1].GetValue().Set(zukanCompareHeight.playerOffset.Y);
                baseField["PlayerOffset"].children[2].GetValue().Set(zukanCompareHeight.playerOffset.Z);
                baseField["PlayerRotationAngle"].children[0].GetValue().Set(zukanCompareHeight.playerRotationAngle.X);
                baseField["PlayerRotationAngle"].children[1].GetValue().Set(zukanCompareHeight.playerRotationAngle.Y);
                newZukanCompareHeights.Add(baseField);
            }
            uiDatabase["ZukanCompareHeight"].children[0].SetChildrenList(newZukanCompareHeights.ToArray());

            // SearchPokeIconSex
            AssetTypeValueField[] searchPokeIconSexes = uiDatabase["SearchPokeIconSex"].children[0].children;

            AssetTypeValueField searchPokeIconSexRef = searchPokeIconSexes[0];
            List<AssetTypeValueField> newSearchPokeIconSexes = new();
            foreach (UIMasterdatas.SearchPokeIconSex searchPokeIconSex in gameData.uiSearchPokeIconSex)
            {
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromTemplate(searchPokeIconSexRef.GetTemplateField());
                baseField["MonsNo"].GetValue().Set(searchPokeIconSex.monsNo);
                baseField["Sex"].GetValue().Set(searchPokeIconSex.sex);
                newSearchPokeIconSexes.Add(baseField);
            }
            uiDatabase["SearchPokeIconSex"].children[0].SetChildrenList(newSearchPokeIconSexes.ToArray());

            // DistributionTable
            CommitDistributionSheet(distributionTable["Diamond_FieldTable"].children[0], gameData.uiDistributionTable.diamondFieldTable);
            CommitDistributionSheet(distributionTable["Diamond_DungeonTable"].children[0], gameData.uiDistributionTable.diamondDungeonTable);
            CommitDistributionSheet(distributionTable["Pearl_FieldTable"].children[0], gameData.uiDistributionTable.pearlFieldTable);
            CommitDistributionSheet(distributionTable["Pearl_DungeonTable"].children[0], gameData.uiDistributionTable.pearlDungeonTable);

            fileManager.WriteMonoBehaviours(PathEnum.UIMasterdatas, monoBehaviours.ToArray());
        }

        private static void CommitUIMasterdatasYAML()
        {
            gameData.uiPokemonIcon.Sort();
            gameData.uiAshiatoIcon.Sort();
            gameData.uiPokemonVoice.Sort();
            gameData.uiZukanDisplay.Sort();
            gameData.uiZukanCompareHeights.Sort();
            gameData.uiSearchPokeIconSex.Sort();

            var monoBehaviours = fileManager.GetYAMLs(PathEnum.UIMasterdatas);

            var uiDatabase = monoBehaviours.Find(y => y.MonoBehaviour?.Name == "UIDatabase").MonoBehaviour as UIDatabase;
            var distributionTable = monoBehaviours.Find(y => y.MonoBehaviour?.Name == "DistributionTable").MonoBehaviour as DistributionTable;

            // Pokemon Icon
            uiDatabase.PokemonIcon = gameData.uiPokemonIcon.Select(i => new SheetPokemonIcon()
            {
                UniqueID = i.uniqueID,
                AssetBundleName = i.assetBundleName,
                AssetName = i.assetName,
                AssetBundleNameLarge = i.assetBundleNameLarge,
                AssetNameLarge = i.assetNameLarge,
                AssetBundleNameDP = i.assetBundleNameDP,
                AssetNameDP = i.assetNameDP,
                HallofFameOffset = new UnityVector2() { X = i.hallofFameOffset.X, Y = i.hallofFameOffset.Y },
            }).ToArray();

            // Ashiato Icon
            uiDatabase.AshiatoIcon = gameData.uiAshiatoIcon.Select(i => new SheetAshiatoIcon()
            {
                UniqueID = i.uniqueID,
                SideIconAssetName = i.sideIconAssetName,
                BothIconAssetName = i.bothIconAssetName,
            }).ToArray();

            // Pokemon Voice
            uiDatabase.PokemonVoice = gameData.uiPokemonVoice.Select(v => new SheetPokemonVoice()
            {
                UniqueID = v.uniqueID,
                WwiseEvent = v.wwiseEvent,
                StopEventId = v.stopEventId,
                CenterPointOffset = new UnityVector3(){ X = v.centerPointOffset.X, Y = v.centerPointOffset.Y, Z = v.centerPointOffset.Z },
                RotationLimits = v.rotationLimits ? (byte)1 : (byte)0,
                RotationLimitAngle = new UnityVector2() { X = v.rotationLimitAngle.X, Y = v.rotationLimitAngle.Y },
            }).ToArray();

            // ZukanDisplay
            uiDatabase.ZukanDisplay = gameData.uiZukanDisplay.Select(z => new SheetZukanDisplay()
            {
                UniqueID = z.uniqueID,
                MoveLimit = new UnityVector3() { X = z.moveLimit.X, Y = z.moveLimit.Y, Z = z.moveLimit.Z },
                ModelOffset = new UnityVector3() { X = z.modelOffset.X, Y = z.modelOffset.Y, Z = z.modelOffset.Z },
                ModelRotationAngle = new UnityVector2() { X = z.modelRotationAngle.X, Y = z.modelRotationAngle.Y },
            }).ToArray();

            // ZukanCompareHeight
            uiDatabase.ZukanCompareHeight = gameData.uiZukanCompareHeights.Select(z => new SheetZukanCompareHeight()
            {
                UniqueID = z.uniqueID,
                PlayerScaleFactor = z.playerScaleFactor,
                PlayerOffset = new UnityVector3() { X = z.playerOffset.X, Y = z.playerOffset.Y, Z = z.playerOffset.Z },
                PlayerRotationAngle = new UnityVector2() { X = z.playerRotationAngle.X, Y = z.playerRotationAngle.Y },
            }).ToArray();

            // SearchPokeIconSex
            uiDatabase.SearchPokeIconSex = gameData.uiSearchPokeIconSex.Select(s => new SheetSearchPokeIconSex()
            {
                MonsNo = s.monsNo,
                Sex = s.sex,
            }).ToArray();

            // DistributionTable
            distributionTable.DiamondFieldTable = gameData.uiDistributionTable.diamondFieldTable.Select(d => new SheetDistributionTable()
            {
                BeforeMorning = d.beforeMorning,
                BeforeDaytime = d.beforeDaytime,
                BeforeNight = d.beforeNight,
                AfterMorning = d.afterMorning,
                AfterDaytime = d.afterDaytime,
                AfterNight = d.afterNight,
                Fishing = d.fishing,
                PokemonTraser = d.pokemonTraser,
                HoneyTree = d.honeyTree,
            }).ToArray();

            distributionTable.DiamondDungeonTable = gameData.uiDistributionTable.diamondDungeonTable.Select(d => new SheetDistributionTable()
            {
                BeforeMorning = d.beforeMorning,
                BeforeDaytime = d.beforeDaytime,
                BeforeNight = d.beforeNight,
                AfterMorning = d.afterMorning,
                AfterDaytime = d.afterDaytime,
                AfterNight = d.afterNight,
                Fishing = d.fishing,
                PokemonTraser = d.pokemonTraser,
                HoneyTree = d.honeyTree,
            }).ToArray();

            distributionTable.PearlFieldTable = gameData.uiDistributionTable.pearlFieldTable.Select(d => new SheetDistributionTable()
            {
                BeforeMorning = d.beforeMorning,
                BeforeDaytime = d.beforeDaytime,
                BeforeNight = d.beforeNight,
                AfterMorning = d.afterMorning,
                AfterDaytime = d.afterDaytime,
                AfterNight = d.afterNight,
                Fishing = d.fishing,
                PokemonTraser = d.pokemonTraser,
                HoneyTree = d.honeyTree,
            }).ToArray();

            distributionTable.PearlDungeonTable = gameData.uiDistributionTable.pearlDungeonTable.Select(d => new SheetDistributionTable()
            {
                BeforeMorning = d.beforeMorning,
                BeforeDaytime = d.beforeDaytime,
                BeforeNight = d.beforeNight,
                AfterMorning = d.afterMorning,
                AfterDaytime = d.afterDaytime,
                AfterNight = d.afterNight,
                Fishing = d.fishing,
                PokemonTraser = d.pokemonTraser,
                HoneyTree = d.honeyTree,
            }).ToArray();
        }

        private static void CommitDistributionSheet(AssetTypeValueField sheetATVF, List<UIMasterdatas.DistributionEntry> sheet)
        {
            AssetTypeValueField distributionEntryRef = sheetATVF.children[0];
            List<AssetTypeValueField> newSheet = new();
            foreach (UIMasterdatas.DistributionEntry entry in sheet)
            {
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromTemplate(distributionEntryRef.GetTemplateField());
                CommitDistributionCoord(baseField["BeforeMorning"], entry.beforeMorning, distributionEntryRef[0][0][0]);
                CommitDistributionCoord(baseField["BeforeDaytime"], entry.beforeDaytime, distributionEntryRef[0][0][0]);
                CommitDistributionCoord(baseField["BeforeNight"], entry.beforeNight, distributionEntryRef[0][0][0]);
                CommitDistributionCoord(baseField["AfterMorning"], entry.afterMorning, distributionEntryRef[0][0][0]);
                CommitDistributionCoord(baseField["AfterDaytime"], entry.afterDaytime, distributionEntryRef[0][0][0]);
                CommitDistributionCoord(baseField["AfterNight"], entry.afterNight, distributionEntryRef[0][0][0]);
                CommitDistributionCoord(baseField["Fishing"], entry.fishing, distributionEntryRef[0][0][0]);
                CommitDistributionCoord(baseField["PokemonTraser"], entry.pokemonTraser, distributionEntryRef[0][0][0]);
                CommitDistributionCoord(baseField["HoneyTree"], entry.honeyTree, distributionEntryRef[0][0][0]);
                newSheet.Add(baseField);
            }
            sheetATVF.SetChildrenList(newSheet.ToArray());
        }

        private static void CommitDistributionCoord(AssetTypeValueField posATVF, int[] pos, AssetTypeValueField intRef)
        {
            List<AssetTypeValueField> newInts = new();
            for (int i = 0; i < pos.Length; i++)
            {
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromTemplate(intRef.GetTemplateField());
                baseField.GetValue().Set(pos[i]);
                newInts.Add(baseField);
            }
            posATVF[0].SetChildrenList(newInts.ToArray());
        }

        /// <summary>
        ///  Updates loaded bundle with Pokemon.
        /// </summary>
        private static void CommitPokemon()
        {
            List<AssetTypeValueField> monoBehaviours = fileManager.GetMonoBehaviours(PathEnum.PersonalMasterdatas);

            AssetTypeValueField wazaOboeTable = monoBehaviours.Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "WazaOboeTable");
            AssetTypeValueField tamagoWazaTable = monoBehaviours.Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "TamagoWazaTable");
            AssetTypeValueField evolveTable = monoBehaviours.Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "EvolveTable");
            AssetTypeValueField personalTable = monoBehaviours.Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "PersonalTable");
            monoBehaviours = new()
            {
                wazaOboeTable,
                tamagoWazaTable,
                evolveTable,
                personalTable
            };

            AssetTypeValueField[] levelUpMoveFields = wazaOboeTable.children[4].children[0].children;
            AssetTypeValueField[] eggMoveFields = tamagoWazaTable.children[4].children[0].children;
            AssetTypeValueField[] evolveFields = evolveTable.children[4].children[0].children;
            AssetTypeValueField[] personalFields = personalTable.children[4].children[0].children;

            List<AssetTypeValueField> newLevelUpMoveFields = new();
            List<AssetTypeValueField> newEggMoveFields = new();
            List<AssetTypeValueField> newEvolveFields = new();
            List<AssetTypeValueField> newPersonalFields = new();

            AssetTypeValueField personalFieldRef = personalFields[0];
            for (int personalID = 0; personalID < gameData.personalEntries.Count; personalID++)
            {
                AssetTypeValueField personalField = ValueBuilder.DefaultValueFieldFromTemplate(personalFieldRef.GetTemplateField());
                Pokemon pokemon = gameData.personalEntries[personalID];
                personalField.children[0].GetValue().Set(pokemon.validFlag);
                personalField.children[1].GetValue().Set(pokemon.personalID);
                personalField.children[2].GetValue().Set(pokemon.dexID);
                personalField.children[3].GetValue().Set(pokemon.formIndex);
                personalField.children[4].GetValue().Set(pokemon.formMax);
                personalField.children[5].GetValue().Set(pokemon.color);
                personalField.children[6].GetValue().Set(pokemon.graNo);
                personalField.children[7].GetValue().Set(pokemon.basicHp);
                personalField.children[8].GetValue().Set(pokemon.basicAtk);
                personalField.children[9].GetValue().Set(pokemon.basicDef);
                personalField.children[10].GetValue().Set(pokemon.basicSpd);
                personalField.children[11].GetValue().Set(pokemon.basicSpAtk);
                personalField.children[12].GetValue().Set(pokemon.basicSpDef);
                personalField.children[13].GetValue().Set(pokemon.typingID1);
                personalField.children[14].GetValue().Set(pokemon.typingID2);
                personalField.children[15].GetValue().Set(pokemon.getRate);
                personalField.children[16].GetValue().Set(pokemon.rank);
                personalField.children[17].GetValue().Set(pokemon.expValue);
                personalField.children[18].GetValue().Set(pokemon.item1);
                personalField.children[19].GetValue().Set(pokemon.item2);
                personalField.children[20].GetValue().Set(pokemon.item3);
                personalField.children[21].GetValue().Set(pokemon.sex);
                personalField.children[22].GetValue().Set(pokemon.eggBirth);
                personalField.children[23].GetValue().Set(pokemon.initialFriendship);
                personalField.children[24].GetValue().Set(pokemon.eggGroup1);
                personalField.children[25].GetValue().Set(pokemon.eggGroup2);
                personalField.children[26].GetValue().Set(pokemon.grow);
                personalField.children[27].GetValue().Set(pokemon.abilityID1);
                personalField.children[28].GetValue().Set(pokemon.abilityID2);
                personalField.children[29].GetValue().Set(pokemon.abilityID3);
                personalField.children[30].GetValue().Set(pokemon.giveExp);
                personalField.children[31].GetValue().Set(pokemon.height);
                personalField.children[32].GetValue().Set(pokemon.weight);
                personalField.children[33].GetValue().Set(pokemon.chihouZukanNo);
                personalField.children[34].GetValue().Set(pokemon.machine1);
                personalField.children[35].GetValue().Set(pokemon.machine2);
                personalField.children[36].GetValue().Set(pokemon.machine3);
                personalField.children[37].GetValue().Set(pokemon.machine4);
                personalField.children[38].GetValue().Set(pokemon.hiddenMachine);
                personalField.children[39].GetValue().Set(pokemon.eggMonsno);
                personalField.children[40].GetValue().Set(pokemon.eggFormno);
                personalField.children[41].GetValue().Set(pokemon.eggFormnoKawarazunoishi);
                personalField.children[42].GetValue().Set(pokemon.eggFormInheritKawarazunoishi);
                newPersonalFields.Add(personalField);

                // Level Up Moves
                AssetTypeValueField levelUpMoveField = ValueBuilder.DefaultValueFieldFromTemplate(levelUpMoveFields[0].GetTemplateField());
                levelUpMoveField["id"].GetValue().Set(pokemon.personalID);

                List<AssetTypeValueField> levelUpMoveAr = new();
                AssetTypeTemplateField arTemplate = levelUpMoveFields[1]["ar"][0][0].GetTemplateField();
                foreach (LevelUpMove levelUpMove in pokemon.levelUpMoves)
                {
                    AssetTypeValueField levelField = ValueBuilder.DefaultValueFieldFromTemplate(arTemplate);
                    AssetTypeValueField moveIDField = ValueBuilder.DefaultValueFieldFromTemplate(arTemplate);
                    levelField.GetValue().Set(levelUpMove.level);
                    moveIDField.GetValue().Set(levelUpMove.moveID);
                    levelUpMoveAr.Add(levelField);
                    levelUpMoveAr.Add(moveIDField);
                }
                levelUpMoveField["ar"][0].SetChildrenList(levelUpMoveAr.ToArray());

                newLevelUpMoveFields.Add(levelUpMoveField);

                // Evolution Paths
                AssetTypeValueField evolveField = ValueBuilder.DefaultValueFieldFromTemplate(evolveFields[0].GetTemplateField());
                evolveField["id"].GetValue().Set(pokemon.personalID);

                List<AssetTypeValueField> evolveAr = new();
                arTemplate = evolveFields[1]["ar"][0][0].GetTemplateField();
                foreach (EvolutionPath evolutionPath in pokemon.evolutionPaths)
                {
                    AssetTypeValueField methodField = ValueBuilder.DefaultValueFieldFromTemplate(arTemplate);
                    AssetTypeValueField parameterField = ValueBuilder.DefaultValueFieldFromTemplate(arTemplate);
                    AssetTypeValueField destDexIDField = ValueBuilder.DefaultValueFieldFromTemplate(arTemplate);
                    AssetTypeValueField destFormIDField = ValueBuilder.DefaultValueFieldFromTemplate(arTemplate);
                    AssetTypeValueField levelField = ValueBuilder.DefaultValueFieldFromTemplate(arTemplate);
                    methodField.GetValue().Set(evolutionPath.method);
                    parameterField.GetValue().Set(evolutionPath.parameter);
                    destDexIDField.GetValue().Set(evolutionPath.destDexID);
                    destFormIDField.GetValue().Set(evolutionPath.destFormID);
                    levelField.GetValue().Set(evolutionPath.level);
                    evolveAr.Add(methodField);
                    evolveAr.Add(parameterField);
                    evolveAr.Add(destDexIDField);
                    evolveAr.Add(destFormIDField);
                    evolveAr.Add(levelField);
                }
                evolveField["ar"][0].SetChildrenList(evolveAr.ToArray());

                newEvolveFields.Add(evolveField);

                // Egg Moves
                AssetTypeValueField eggMoveField = ValueBuilder.DefaultValueFieldFromTemplate(eggMoveFields[0].GetTemplateField());
                eggMoveField["no"].GetValue().Set(pokemon.dexID);
                eggMoveField["formNo"].GetValue().Set(pokemon.formID);

                List<AssetTypeValueField> wazaNos = new();
                AssetTypeTemplateField wazaNoTemplate = eggMoveFields[1]["wazaNo"][0][0].GetTemplateField();
                foreach (ushort wazaNo in pokemon.eggMoves)
                {
                    AssetTypeValueField wazaNoField = ValueBuilder.DefaultValueFieldFromTemplate(wazaNoTemplate);
                    wazaNoField.GetValue().Set(wazaNo);
                    wazaNos.Add(wazaNoField);
                }
                eggMoveField["wazaNo"][0].SetChildrenList(wazaNos.ToArray());

                newEggMoveFields.Add(eggMoveField);

                // External TM Learnsets
                if (pokemon.externalTMLearnset != null)
                    fileManager.CommitExternalJson($"MonData\\TMLearnset\\monsno_{pokemon.dexID}_formno_{pokemon.formID}.json");
            }

            newPersonalFields.Sort((atvf1, atvf2) => atvf1[1].GetValue().AsUInt().CompareTo(atvf2[1].GetValue().AsUInt()));
            newLevelUpMoveFields.Sort((atvf1, atvf2) => atvf1["id"].GetValue().AsInt().CompareTo(atvf2["id"].GetValue().AsInt()));
            newEvolveFields.Sort((atvf1, atvf2) => atvf1["id"].GetValue().AsInt().CompareTo(atvf2["id"].GetValue().AsInt()));
            newEggMoveFields.Sort((atvf1, atvf2) =>
            {
                if (atvf1["formNo"].GetValue().AsUInt().CompareTo(atvf2["formNo"].GetValue().AsUInt()) != 0)
                {
                    if (atvf1["formNo"].GetValue().AsUInt() == 0)
                        return -1;
                    if (atvf2["formNo"].GetValue().AsUInt() == 0)
                        return 1;
                }

                int i = atvf1["no"].GetValue().AsUInt().CompareTo(atvf2["no"].GetValue().AsUInt());
                if (i == 0)
                    i = atvf1["formNo"].GetValue().AsUInt().CompareTo(atvf2["formNo"].GetValue().AsUInt());
                return i;
            });

            wazaOboeTable.children[4].children[0].SetChildrenList(newLevelUpMoveFields.ToArray());
            tamagoWazaTable.children[4].children[0].SetChildrenList(newEggMoveFields.ToArray());
            evolveTable.children[4].children[0].SetChildrenList(newEvolveFields.ToArray());
            personalTable.children[4].children[0].SetChildrenList(newPersonalFields.ToArray());

            fileManager.WriteMonoBehaviours(PathEnum.PersonalMasterdatas, monoBehaviours.ToArray());
        }

        /// <summary>
        ///  Updates loaded yaml with Pokemon.
        /// </summary>
        private static void CommitPokemonYAML()
        {
            var monoBehaviours = fileManager.GetYAMLs(PathEnum.PersonalMasterdatas);

            var wazaOboeTable = monoBehaviours.Find(y => y.MonoBehaviour?.Name == "WazaOboeTable").MonoBehaviour as WazaOboeTable;
            var tamagoWazaTable = monoBehaviours.Find(y => y.MonoBehaviour?.Name == "TamagoWazaTable").MonoBehaviour as TamagoWazaTable;
            var evolveTable = monoBehaviours.Find(y => y.MonoBehaviour?.Name == "EvolveTable").MonoBehaviour as EvolveTable;
            var personalTable = monoBehaviours.Find(y => y.MonoBehaviour?.Name == "PersonalTable").MonoBehaviour as PersonalTable;

            personalTable.Personal = gameData.personalEntries.Select(p => new SheetPersonal()
            {
                ValidFlag = p.validFlag,
                ID = p.personalID,
                Monsno = p.dexID,
                FormIndex = p.formIndex,
                FormMax = p.formMax,
                Color = p.color,
                GraNo = p.graNo,
                BasicHP = p.basicHp,
                BasicAtk = p.basicAtk,
                BasicDef = p.basicDef,
                BasicAgi = p.basicSpd,
                BasicSpatk = p.basicSpAtk,
                BasicSpdef = p.basicSpDef,
                Type1 = p.typingID1,
                Type2 = p.typingID2,
                GetRate = p.getRate,
                Rank = p.rank,
                ExpValue = p.expValue,
                Item1 = p.item1,
                Item2 = p.item2,
                Item3 = p.item3,
                Sex = p.sex,
                EggBirth = p.eggBirth,
                InitialFriendship = p.initialFriendship,
                EggGroup1 = p.eggGroup1,
                EggGroup2 = p.eggGroup2,
                Grow = p.grow,
                Tokusei1 = p.abilityID1,
                Tokusei2 = p.abilityID2,
                Tokusei3 = p.abilityID3,
                GiveExp = p.giveExp,
                Height = p.height,
                Weight = p.weight,
                ChihouZukanNo = p.chihouZukanNo,
                Machine1 = p.machine1,
                Machine2 = p.machine2,
                Machine3 = p.machine3,
                Machine4 = p.machine4,
                HidenMachine = p.hiddenMachine,
                EggMonsno = p.eggMonsno,
                EggFormno = p.eggFormno,
                EggFormnoKawarazunoishi = p.eggFormnoKawarazunoishi,
                EggFormInheritKawarazunoishi = p.eggFormInheritKawarazunoishi,
            }).ToArray();

            wazaOboeTable.WazaOboe = gameData.personalEntries.Select(p => new SheetWazaOboe()
            {
                ID = p.personalID,
                Ar = p.levelUpMoves.SelectMany(l => new ushort[2]{ l.level, l.moveID }).ToArray(),
            }).ToArray();

            evolveTable.Evolve = gameData.personalEntries.Select(p => new SheetEvolve()
            {
                ID = p.personalID,
                Ar = p.evolutionPaths.SelectMany(e => new ushort[5]{ e.method, e.parameter, e.destDexID, e.destFormID, e.level }).ToArray(),
            }).ToArray();

            tamagoWazaTable.Data = gameData.personalEntries.Select(p => new SheetTamagoWazaData()
            {
                No = p.dexID,
                FormNo = (ushort)p.formID,
                WazaNo = p.eggMoves.ToArray(),
            }).ToArray();

            foreach (var pokemon in gameData.personalEntries)
                if (pokemon.externalTMLearnset != null)
                    fileManager.CommitExternalJson($"MonData\\TMLearnset\\monsno_{pokemon.dexID}_formno_{pokemon.formID}.json");


            // Sort?
            /*newPersonalFields.Sort((atvf1, atvf2) => atvf1[1].GetValue().AsUInt().CompareTo(atvf2[1].GetValue().AsUInt()));
            newLevelUpMoveFields.Sort((atvf1, atvf2) => atvf1["id"].GetValue().AsInt().CompareTo(atvf2["id"].GetValue().AsInt()));
            newEvolveFields.Sort((atvf1, atvf2) => atvf1["id"].GetValue().AsInt().CompareTo(atvf2["id"].GetValue().AsInt()));
            newEggMoveFields.Sort((atvf1, atvf2) =>
            {
                if (atvf1["formNo"].GetValue().AsUInt().CompareTo(atvf2["formNo"].GetValue().AsUInt()) != 0)
                {
                    if (atvf1["formNo"].GetValue().AsUInt() == 0)
                        return -1;
                    if (atvf2["formNo"].GetValue().AsUInt() == 0)
                        return 1;
                }

                int i = atvf1["no"].GetValue().AsUInt().CompareTo(atvf2["no"].GetValue().AsUInt());
                if (i == 0)
                    i = atvf1["formNo"].GetValue().AsUInt().CompareTo(atvf2["formNo"].GetValue().AsUInt());
                return i;
            });*/
        }

        /// <summary>
        ///  Updates loaded bundle with UgEncounters.
        /// </summary>
        private static void CommitUgTables()
        {
            List<AssetTypeValueField> monoBehaviours = fileManager.GetMonoBehaviours(PathEnum.Ugdata);
            List<AssetTypeValueField> updatedMonoBehaviours = new();

            AssetTypeValueField ugRandMark = monoBehaviours.Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "UgRandMark");
            AssetTypeValueField[] ugAreaFields = ugRandMark["table"].children[0].children;

            for (int ugAreaIdx = 0; ugAreaIdx < gameData.ugAreas.Count; ugAreaIdx++)
            {
                UgArea ugArea = gameData.ugAreas[ugAreaIdx];
                ugAreaFields[ugAreaIdx]["id"].GetValue().Set(ugArea.id);
                ugAreaFields[ugAreaIdx]["FileName"].GetValue().Set(ugArea.fileName);
            }
            updatedMonoBehaviours.Add(ugRandMark);

            List<AssetTypeValueField> ugEncounterFileMonobehaviours = monoBehaviours.Where(m => Encoding.Default.GetString(m.children[3].value.value.asString).StartsWith("UgEncount_")).ToList();
            AssetTypeTemplateField ugEncounterTemplate = ugEncounterFileMonobehaviours[0]["table"][0][0].GetTemplateField();
            for (int ugEncounterFileIdx = 0; ugEncounterFileIdx < ugEncounterFileMonobehaviours.Count; ugEncounterFileIdx++)
            {
                UgEncounterFile ugEncounterFile = gameData.ugEncounterFiles[ugEncounterFileIdx];
                ugEncounterFileMonobehaviours[ugEncounterFileIdx]["m_Name"].GetValue().Set(ugEncounterFile.mName);

                List<AssetTypeValueField> newUgEncounters = new();
                foreach (UgEncounter ugEncounter in ugEncounterFile.ugEncounters)
                {
                    AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromTemplate(ugEncounterTemplate);
                    baseField["monsno"].GetValue().Set(ugEncounter.dexID);
                    baseField["version"].GetValue().Set(ugEncounter.version);
                    baseField["zukanflag"].GetValue().Set(ugEncounter.zukanFlag);

                    newUgEncounters.Add(baseField);
                }
                ugEncounterFileMonobehaviours[ugEncounterFileIdx]["table"][0].SetChildrenList(newUgEncounters.ToArray());

                updatedMonoBehaviours.Add(ugEncounterFileMonobehaviours[ugEncounterFileIdx]);
            }

            AssetTypeValueField ugEncounterLevelMonobehaviour = monoBehaviours.Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "UgEncountLevel");
            AssetTypeValueField[] ugEncounterLevelFields = ugEncounterLevelMonobehaviour.children[4].children[0].children;
            for (int ugEncouterLevelIdx = 0; ugEncouterLevelIdx < ugEncounterLevelFields.Length; ugEncouterLevelIdx++)
            {
                UgEncounterLevelSet ugLevels = gameData.ugEncounterLevelSets[ugEncouterLevelIdx];
                ugEncounterLevelFields[ugEncouterLevelIdx].children[0].GetValue().Set(ugLevels.minLv);
                ugEncounterLevelFields[ugEncouterLevelIdx].children[1].GetValue().Set(ugLevels.maxLv);
            }
            updatedMonoBehaviours.Add(ugEncounterLevelMonobehaviour);

            AssetTypeValueField ugSpecialPokemon = monoBehaviours.Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "UgSpecialPokemon");
            AssetTypeValueField[] ugSpecialPokemonFields = ugSpecialPokemon["Sheet1"].children[0].children;

            AssetTypeTemplateField ugSpecialPokemonTemplate = ugSpecialPokemonFields[0].GetTemplateField();

            List<AssetTypeValueField> newUgSpecialPokemon = new();
            foreach (UgSpecialEncounter ugSpecialEncounter in gameData.ugSpecialEncounters)
            {
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromTemplate(ugSpecialPokemonTemplate);

                baseField["id"].GetValue().Set(ugSpecialEncounter.id);
                baseField["monsno"].GetValue().Set(ugSpecialEncounter.dexID);
                baseField["version"].GetValue().Set(ugSpecialEncounter.version);
                baseField["Dspecialrate"].GetValue().Set(ugSpecialEncounter.dRate);
                baseField["Pspecialrate"].GetValue().Set(ugSpecialEncounter.pRate);

                newUgSpecialPokemon.Add(baseField);
            }
            ugSpecialPokemon["Sheet1"].children[0].SetChildrenList(newUgSpecialPokemon.ToArray());

            updatedMonoBehaviours.Add(ugSpecialPokemon);

            AssetTypeValueField ugPokemonDataMonobehaviour = monoBehaviours.Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "UgPokemonData");
            AssetTypeValueField[] ugPokemonDataFields = ugPokemonDataMonobehaviour["table"].children[0].children;

            AssetTypeTemplateField ugPokemonDataTemplate = ugPokemonDataFields[0].GetTemplateField();
            AssetTypeTemplateField intATTF = ugPokemonDataFields[0]["reactioncode"][0][0].GetTemplateField();

            List<AssetTypeValueField> newUgPokemonData = new();
            foreach (var ugPokemonData in gameData.ugPokemonData)
            {
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromTemplate(ugPokemonDataTemplate);

                baseField["monsno"].GetValue().Set(ugPokemonData.monsno);
                baseField["type1ID"].GetValue().Set(ugPokemonData.type1ID);
                baseField["type2ID"].GetValue().Set(ugPokemonData.type2ID);
                baseField["size"].GetValue().Set(ugPokemonData.size);
                baseField["movetype"].GetValue().Set(ugPokemonData.movetype);

                baseField["reactioncode"][0].SetChildrenList(PrimitiveATVFArray(ugPokemonData.reactioncode, intATTF));
                baseField["move_rate"][0].SetChildrenList(PrimitiveATVFArray(ugPokemonData.moveRate, intATTF));
                baseField["submove_rate"][0].SetChildrenList(PrimitiveATVFArray(ugPokemonData.submoveRate, intATTF));
                baseField["reaction"][0].SetChildrenList(PrimitiveATVFArray(ugPokemonData.reaction, intATTF));
                baseField["flagrate"][0].SetChildrenList(PrimitiveATVFArray(ugPokemonData.flagrate, intATTF));

                newUgPokemonData.Add(baseField);
            }
            ugPokemonDataMonobehaviour["table"].children[0].SetChildrenList(newUgPokemonData.ToArray());

            updatedMonoBehaviours.Add(ugPokemonDataMonobehaviour);

            fileManager.WriteMonoBehaviours(PathEnum.Ugdata, updatedMonoBehaviours.ToArray());
        }

        /// <summary>
        ///  Updates loaded yaml with UgEncounters.
        /// </summary>
        private static void CommitUgTablesYAML()
        {
            var monoBehaviours = fileManager.GetYAMLs(PathEnum.Ugdata);
            List<AssetTypeValueField> updatedMonoBehaviours = new();

            var ugRandMark = monoBehaviours.Find(y => y.MonoBehaviour?.Name == "UgRandMark").MonoBehaviour as UgRandMark;
            ugRandMark.Table = gameData.ugAreas.Select((u, i) => new SheetUgRandTable()
            {
                ID = u.id,
                FileName = u.fileName,
                Size = ugRandMark.Table[i].Size,
                Min = ugRandMark.Table[i].Min,
                Max = ugRandMark.Table[i].Max,
                SMax = ugRandMark.Table[i].SMax,
                MMax = ugRandMark.Table[i].MMax,
                LMax = ugRandMark.Table[i].LMax,
                LLMax = ugRandMark.Table[i].LLMax,
                WaterMax = ugRandMark.Table[i].WaterMax,
                TypeRate = ugRandMark.Table­[i].TypeRate,
            }).ToArray();

            var ugEncounterFileMonobehaviours = monoBehaviours.OfType<UgEncount>().ToList();
            foreach (var encount in ugEncounterFileMonobehaviours)
            {
                var newMono = gameData.ugEncounterFiles.Find(u => u.mName == encount.Name);
                encount.Table = newMono.ugEncounters.Select(u => new SheetUgEncountTable()
                {
                    Monsno = u.dexID,
                    Version = u.version,
                    ZukanFlag = u.zukanFlag,
                }).ToArray();
            }

            var ugEncounterLevelMonobehaviour = monoBehaviours.Find(y => y.MonoBehaviour?.Name == "UgEncountLevel").MonoBehaviour as UgEncountLevel;
            ugEncounterLevelMonobehaviour.Data = gameData.ugEncounterLevelSets.Select(u => new SheetUgEncountLevelData()
            {
                MinLv = u.minLv,
                MaxLv = u.maxLv,
            }).ToArray();

            var ugSpecialPokemon = monoBehaviours.Find(y => y.MonoBehaviour?.Name == "UgSpecialPokemon").MonoBehaviour as UgSpecialPokemon;
            ugSpecialPokemon.Sheet1 = gameData.ugSpecialEncounters.Select(u => new SheetUgSpecialPokemonSheet1()
            {
                ID = u.id,
                Monsno = u.dexID,
                Version = u.version,
                DSpecialRate = u.dRate,
                PSpecialRate = u.pRate,
            }).ToArray();

            var ugPokemonDataMonobehaviour = monoBehaviours.Find(y => y.MonoBehaviour?.Name == "UgPokemonData").MonoBehaviour as UgPokemonData;
            ugPokemonDataMonobehaviour.Table = gameData.ugPokemonData.Select(u => new SheetUgPokemonDataTable()
            {
                Monsno = u.monsno,
                Type1ID = u.type1ID,
                Type2ID = u.type2ID,
                Size = u.size,
                MoveType = u.movetype,
                ReactionCode = u.reactioncode,
                MoveRate = u.moveRate,
                SubmoveRate = u.submoveRate,
                Reaction = u.reaction,
                FlagRate = u.flagrate,
                RateUp = u.rateup,
            }).ToArray();
        }

        /// <summary>
        /// Builds an array of AssetTypeValueField objects from primitive values.
        /// </summary>
        private static AssetTypeValueField[] PrimitiveATVFArray<T>(T[] values, AssetTypeTemplateField attf)
        {
            List<AssetTypeValueField> atvfList = new();
            for (int i = 0; i < values.Length; i++)
            {
                AssetTypeValueField intField = ValueBuilder.DefaultValueFieldFromTemplate(attf);
                intField.GetValue().Set(values[i]);
                atvfList.Add(intField);
            }
            return atvfList.ToArray();
        }

        /// <summary>
        ///  Updates loaded bundle with MessageFileSets.
        /// </summary>
        private static void CommitMessageFileSets()
        {
            List<MessageFile> messageFiles = gameData.messageFileSets.SelectMany(mfs => mfs.messageFiles).ToList();

            List<AssetTypeValueField> monoBehaviours = fileManager.GetMonoBehaviours(PathEnum.CommonMsbt);
            CommitMessageFiles(monoBehaviours, messageFiles);
            fileManager.WriteMonoBehaviours(PathEnum.CommonMsbt, monoBehaviours.ToArray());

            monoBehaviours = fileManager.GetMonoBehaviours(PathEnum.English);
            CommitMessageFiles(monoBehaviours, messageFiles);
            fileManager.WriteMonoBehaviours(PathEnum.English, monoBehaviours.ToArray());

            monoBehaviours = fileManager.GetMonoBehaviours(PathEnum.French);
            CommitMessageFiles(monoBehaviours, messageFiles);
            fileManager.WriteMonoBehaviours(PathEnum.French, monoBehaviours.ToArray());

            monoBehaviours = fileManager.GetMonoBehaviours(PathEnum.German);
            CommitMessageFiles(monoBehaviours, messageFiles);
            fileManager.WriteMonoBehaviours(PathEnum.German, monoBehaviours.ToArray());

            monoBehaviours = fileManager.GetMonoBehaviours(PathEnum.Italian);
            CommitMessageFiles(monoBehaviours, messageFiles);
            fileManager.WriteMonoBehaviours(PathEnum.Italian, monoBehaviours.ToArray());

            monoBehaviours = fileManager.GetMonoBehaviours(PathEnum.Jpn);
            CommitMessageFiles(monoBehaviours, messageFiles);
            fileManager.WriteMonoBehaviours(PathEnum.Jpn, monoBehaviours.ToArray());

            monoBehaviours = fileManager.GetMonoBehaviours(PathEnum.JpnKanji);
            CommitMessageFiles(monoBehaviours, messageFiles);
            fileManager.WriteMonoBehaviours(PathEnum.JpnKanji, monoBehaviours.ToArray());

            monoBehaviours = fileManager.GetMonoBehaviours(PathEnum.Korean);
            CommitMessageFiles(monoBehaviours, messageFiles);
            fileManager.WriteMonoBehaviours(PathEnum.Korean, monoBehaviours.ToArray());

            monoBehaviours = fileManager.GetMonoBehaviours(PathEnum.SimpChinese);
            CommitMessageFiles(monoBehaviours, messageFiles);
            fileManager.WriteMonoBehaviours(PathEnum.SimpChinese, monoBehaviours.ToArray());

            monoBehaviours = fileManager.GetMonoBehaviours(PathEnum.Spanish);
            CommitMessageFiles(monoBehaviours, messageFiles);
            fileManager.WriteMonoBehaviours(PathEnum.Spanish, monoBehaviours.ToArray());

            monoBehaviours = fileManager.GetMonoBehaviours(PathEnum.TradChinese);
            CommitMessageFiles(monoBehaviours, messageFiles);
            fileManager.WriteMonoBehaviours(PathEnum.TradChinese, monoBehaviours.ToArray());
        }

        /// <summary>
        ///  Updates loaded yaml with MessageFileSets.
        /// </summary>
        private static void CommitMessageFileSetsYAML()
        {
            List<MessageFile> messageFiles = gameData.messageFileSets.SelectMany(mfs => mfs.messageFiles).ToList();

            CommitMessageFilesYAML(fileManager.GetYAMLs(PathEnum.English).Select(y => y.MonoBehaviour as MsbtData).ToList(), messageFiles);
            CommitMessageFilesYAML(fileManager.GetYAMLs(PathEnum.French).Select(y => y.MonoBehaviour as MsbtData).ToList(), messageFiles);
            CommitMessageFilesYAML(fileManager.GetYAMLs(PathEnum.German).Select(y => y.MonoBehaviour as MsbtData).ToList(), messageFiles);
            CommitMessageFilesYAML(fileManager.GetYAMLs(PathEnum.Italian).Select(y => y.MonoBehaviour as MsbtData).ToList(), messageFiles);
            CommitMessageFilesYAML(fileManager.GetYAMLs(PathEnum.Jpn).Select(y => y.MonoBehaviour as MsbtData).ToList(), messageFiles);
            CommitMessageFilesYAML(fileManager.GetYAMLs(PathEnum.JpnKanji).Select(y => y.MonoBehaviour as MsbtData).ToList(), messageFiles);
            CommitMessageFilesYAML(fileManager.GetYAMLs(PathEnum.Korean).Select(y => y.MonoBehaviour as MsbtData).ToList(), messageFiles);
            CommitMessageFilesYAML(fileManager.GetYAMLs(PathEnum.SimpChinese).Select(y => y.MonoBehaviour as MsbtData).ToList(), messageFiles);
            CommitMessageFilesYAML(fileManager.GetYAMLs(PathEnum.Spanish).Select(y => y.MonoBehaviour as MsbtData).ToList(), messageFiles);
            CommitMessageFilesYAML(fileManager.GetYAMLs(PathEnum.TradChinese).Select(y => y.MonoBehaviour as MsbtData).ToList(), messageFiles);
        }

        /// <summary>
        ///  Writes all data into monobehaviors from a superset of MessageFiles.
        /// </summary>
        private static void CommitMessageFiles(List<AssetTypeValueField> monoBehaviours, List<MessageFile> messageFiles)
        {
            foreach (AssetTypeValueField monoBehaviour in monoBehaviours)
            {
                MessageFile messageFile = messageFiles.Find(mf => mf.mName == monoBehaviour.children[3].GetValue().AsString());
                AssetTypeValueField[] labelDataArray = monoBehaviour["labelDataArray"].children[0].children;
                AssetTypeTemplateField templateField = new();

                AssetTypeValueField labelDataRef = labelDataArray[0];

                // tagDataFields[tagDataIdx].children[6][0].children
                foreach (AssetTypeValueField field in labelDataArray)
                {
                    if (tagDataTemplate == null && field["tagDataArray"].children[0].childrenCount > 0)
                    {
                        tagDataTemplate = field["tagDataArray"].children[0][0].GetTemplateField();
                    }


                    if (tagWordTemplate == null && field["tagDataArray"].children[0].childrenCount > 0)
                    {
                        if (field["tagDataArray"].children[0][0]["tagWordArray"].children[0].childrenCount > 0)
                        {
                            tagWordTemplate = field["tagDataArray"].children[0][0]["tagWordArray"].children[0][0].GetTemplateField();
                            if (tagWordTemplate != null)
                            {

                            }
                        }
                    }

                    if (attributeValueTemplate == null && field["attributeValueArray"].children[0].childrenCount > 0)
                    {
                        attributeValueTemplate = field["attributeValueArray"].children[0][0].GetTemplateField();
                    }

                    if (wordDataTemplate == null && field["wordDataArray"].children[0].childrenCount > 0)
                    {
                        wordDataTemplate = field["wordDataArray"].children[0][0].GetTemplateField();
                    }
                }

                List<AssetTypeValueField> newLabelDataArray = new();
                foreach (GameDataTypes.LabelData labelData in messageFile.labelDatas)
                {
                    AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromTemplate(labelDataRef.GetTemplateField());
                    baseField["labelIndex"].GetValue().Set(labelData.labelIndex);
                    baseField["arrayIndex"].GetValue().Set(labelData.arrayIndex);
                    baseField["labelName"].GetValue().Set(labelData.labelName);
                    baseField["styleInfo"]["styleIndex"].GetValue().Set(labelData.styleIndex);
                    baseField["styleInfo"]["colorIndex"].GetValue().Set(labelData.colorIndex);
                    baseField["styleInfo"]["fontSize"].GetValue().Set(labelData.fontSize);
                    baseField["styleInfo"]["maxWidth"].GetValue().Set(labelData.maxWidth);
                    baseField["styleInfo"]["controlID"].GetValue().Set(labelData.controlID);

                    List<AssetTypeValueField> attributeValueArray = new();
                    foreach (int attrVal in labelData.attributeValues)
                    {
                        AssetTypeValueField attributeValueField = ValueBuilder.DefaultValueFieldFromTemplate(attributeValueTemplate);
                        attributeValueField.GetValue().Set(attrVal);
                        attributeValueArray.Add(attributeValueField);
                    }
                    baseField["attributeValueArray"][0].SetChildrenList(attributeValueArray.ToArray());

                    List<AssetTypeValueField> tagDataArray = new();
                    foreach (GameDataTypes.TagData tagData in labelData.tagDatas)
                    {
                        AssetTypeValueField tagDataField = ValueBuilder.DefaultValueFieldFromTemplate(tagDataTemplate);
                        tagDataField["tagIndex"].GetValue().Set(tagData.tagIndex);
                        tagDataField["groupID"].GetValue().Set(tagData.groupID);
                        tagDataField["tagID"].GetValue().Set(tagData.tagID);
                        tagDataField["tagPatternID"].GetValue().Set(tagData.tagPatternID);
                        tagDataField["forceArticle"].GetValue().Set(tagData.forceArticle);
                        tagDataField["tagParameter"].GetValue().Set(tagData.tagParameter);
                        List<AssetTypeValueField> tagWordArray = new();
                        foreach (string tagWord in tagData.tagWordArray)
                        {
                            AssetTypeValueField tagWordField = ValueBuilder.DefaultValueFieldFromTemplate(tagWordTemplate);
                            tagWordField.GetValue().Set(tagWord);
                            tagWordArray.Add(tagWordField);
                        }
                        tagDataField["tagWordArray"][0].SetChildrenList(tagWordArray.ToArray());
                        // tagWordArray
                        tagDataField["forceGrmID"].GetValue().Set(tagData.forceGrmID);
                        tagDataArray.Add(tagDataField);
                    }
                    baseField["tagDataArray"][0].SetChildrenList(tagDataArray.ToArray());

                    List<AssetTypeValueField> wordDataArray = new();
                    foreach (GameDataTypes.WordData wordData in labelData.wordDatas)
                    {
                        AssetTypeValueField wordDataField = ValueBuilder.DefaultValueFieldFromTemplate(wordDataTemplate);
                        wordDataField["patternID"].GetValue().Set(wordData.patternID);
                        wordDataField["eventID"].GetValue().Set(wordData.eventID);
                        wordDataField["tagIndex"].GetValue().Set(wordData.tagIndex);
                        wordDataField["tagValue"].GetValue().Set(wordData.tagValue);
                        wordDataField["str"].GetValue().Set(wordData.str);
                        wordDataField["strWidth"].GetValue().Set(wordData.strWidth);
                        wordDataArray.Add(wordDataField);
                    }
                    baseField["wordDataArray"][0].SetChildrenList(wordDataArray.ToArray());

                    newLabelDataArray.Add(baseField);
                }

                monoBehaviour["labelDataArray"].children[0].SetChildrenList(newLabelDataArray.ToArray());
            }
        }

        /// <summary>
        ///  Writes all data into yaml monobehaviors from a superset of MessageFiles.
        /// </summary>
        private static void CommitMessageFilesYAML(List<MsbtData> monoBehaviours, List<MessageFile> messageFiles)
        {
            foreach (var monoBehaviour in monoBehaviours)
            {
                MessageFile messageFile = messageFiles.Find(mf => mf.mName == monoBehaviour?.Name);

                monoBehaviour.LabelDataArray = messageFile.labelDatas.Select(l => new LabelData()
                {
                    LabelIndex = l.labelIndex,
                    ArrayIndex = l.arrayIndex,
                    LabelName = l.labelName,
                    StyleInfo = new StyleInfo()
                    {
                        StyleIndex = l.styleIndex,
                        ColorIndex = l.colorIndex,
                        FontSize = l.fontSize,
                        MaxWidth = l.maxWidth,
                        ControlID = l.controlID,
                    },
                    AttributeValueArray = l.attributeValues.ToArray(),
                    TagDataArray = l.tagDatas.Select(t => new TagData()
                    {
                        TagIndex = t.tagIndex,
                        GroupID = t.groupID,
                        TagID = t.tagID,
                        TagPatternID = t.tagPatternID,
                        ForceArticle = t.forceArticle,
                        TagParameter = t.tagParameter,
                        TagWordArray = t.tagWordArray.ToArray(),
                        ForceGrmID = t.forceGrmID,
                    }).ToArray(),
                    WordDataArray = l.wordDatas.Select(w => new WordData()
                    {
                        PatternID = w.patternID,
                        EventID = w.eventID,
                        TagIndex = w.tagIndex,
                        TagValue = w.tagValue,
                        Str = w.str,
                        StrWidth = w.strWidth,
                    }).ToArray(),
                }).ToArray();
            }
        }

        /// <summary>
        ///  Updates loaded bundle with EncounterTables.
        /// </summary>
        private static void CommitEncounterTables()
        {
            List<AssetTypeValueField> monoBehaviours = fileManager.GetMonoBehaviours(PathEnum.Gamesettings);
            AssetTypeValueField[] encounterTableMonoBehaviours = new AssetTypeValueField[2];
            encounterTableMonoBehaviours[0] = monoBehaviours.Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "FieldEncountTable_d");
            encounterTableMonoBehaviours[1] = monoBehaviours.Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "FieldEncountTable_p");
            for (int encounterTableFileIdx = 0; encounterTableFileIdx < encounterTableMonoBehaviours.Length; encounterTableFileIdx++)
            {
                EncounterTableFile encounterTableFile = gameData.encounterTableFiles[encounterTableFileIdx];
                encounterTableMonoBehaviours[encounterTableFileIdx].children[3].GetValue().Set(encounterTableFile.mName);

                //Write wild encounter tables
                AssetTypeValueField[] encounterTableFields = encounterTableMonoBehaviours[encounterTableFileIdx].children[4].children[0].children;
                for (int encounterTableIdx = 0; encounterTableIdx < encounterTableFields.Length; encounterTableIdx++)
                {
                    EncounterTable encounterTable = encounterTableFile.encounterTables[encounterTableIdx];
                    encounterTableFields[encounterTableIdx].children[0].GetValue().Set(encounterTable.zoneID);
                    encounterTableFields[encounterTableIdx].children[1].GetValue().Set(encounterTable.encRateGround);
                    encounterTableFields[encounterTableIdx].children[7].children[0].children[0].GetValue().Set(encounterTable.formProb);
                    encounterTableFields[encounterTableIdx].children[9].children[0].children[1].GetValue().Set(encounterTable.unownTable);
                    encounterTableFields[encounterTableIdx].children[15].GetValue().Set(encounterTable.encRateWater);
                    encounterTableFields[encounterTableIdx].children[17].GetValue().Set(encounterTable.encRateOldRod);
                    encounterTableFields[encounterTableIdx].children[19].GetValue().Set(encounterTable.encRateGoodRod);
                    encounterTableFields[encounterTableIdx].children[21].GetValue().Set(encounterTable.encRateSuperRod);

                    //Write ground tables
                    SetEncounters(encounterTableFields[encounterTableIdx].children[2].children[0], encounterTable.groundMons);

                    //Write morning tables
                    SetEncounters(encounterTableFields[encounterTableIdx].children[3].children[0], encounterTable.tairyo);

                    //Write day tables
                    SetEncounters(encounterTableFields[encounterTableIdx].children[4].children[0], encounterTable.day);

                    //Write night tables
                    SetEncounters(encounterTableFields[encounterTableIdx].children[5].children[0], encounterTable.night);

                    //Write pokefinder tables
                    SetEncounters(encounterTableFields[encounterTableIdx].children[6].children[0], encounterTable.swayGrass);

                    //Write ruby tables
                    SetEncounters(encounterTableFields[encounterTableIdx]["gbaRuby"].children[0], encounterTable.gbaRuby);

                    //Write sapphire tables
                    SetEncounters(encounterTableFields[encounterTableIdx]["gbaSapp"].children[0], encounterTable.gbaSapphire);

                    //Write emerald tables
                    SetEncounters(encounterTableFields[encounterTableIdx]["gbaEme"].children[0], encounterTable.gbaEmerald);

                    //Write fire tables
                    SetEncounters(encounterTableFields[encounterTableIdx]["gbaFire"].children[0], encounterTable.gbaFire);

                    //Write leaf tables
                    SetEncounters(encounterTableFields[encounterTableIdx]["gbaLeaf"].children[0], encounterTable.gbaLeaf);

                    //Write surfing tables
                    SetEncounters(encounterTableFields[encounterTableIdx].children[16].children[0], encounterTable.waterMons);

                    //Write old rod tables
                    SetEncounters(encounterTableFields[encounterTableIdx].children[18].children[0], encounterTable.oldRodMons);

                    //Write good rod tables
                    SetEncounters(encounterTableFields[encounterTableIdx].children[20].children[0], encounterTable.goodRodMons);

                    //Write super rod tables
                    SetEncounters(encounterTableFields[encounterTableIdx].children[22].children[0], encounterTable.superRodMons);
                }

                //Write trophy garden table
                AssetTypeValueField[] trophyGardenMonFields = encounterTableMonoBehaviours[encounterTableFileIdx].children[5].children[0].children;
                for (int trophyGardenMonIdx = 0; trophyGardenMonIdx < trophyGardenMonFields.Length; trophyGardenMonIdx++)
                    trophyGardenMonFields[trophyGardenMonIdx].children[0].GetValue().Set(encounterTableFile.trophyGardenMons[trophyGardenMonIdx]);

                //Write honey tree tables
                AssetTypeValueField[] honeyTreeEncounterFields = encounterTableMonoBehaviours[encounterTableFileIdx].children[6].children[0].children;
                for (int honeyTreeEncounterIdx = 0; honeyTreeEncounterIdx < honeyTreeEncounterFields.Length; honeyTreeEncounterIdx++)
                {
                    HoneyTreeEncounter honeyTreeEncounter = encounterTableFile.honeyTreeEnconters[honeyTreeEncounterIdx];
                    honeyTreeEncounterFields[honeyTreeEncounterIdx].children[0].GetValue().Set(honeyTreeEncounter.rate);
                    honeyTreeEncounterFields[honeyTreeEncounterIdx].children[1].GetValue().Set(honeyTreeEncounter.normalDexID);
                    honeyTreeEncounterFields[honeyTreeEncounterIdx].children[2].GetValue().Set(honeyTreeEncounter.rareDexID);
                    honeyTreeEncounterFields[honeyTreeEncounterIdx].children[3].GetValue().Set(honeyTreeEncounter.superRareDexID);
                }

                //Write safari table
                AssetTypeValueField[] safariMonFields = encounterTableMonoBehaviours[encounterTableFileIdx].children[8].children[0].children;
                for (int safariMonIdx = 0; safariMonIdx < safariMonFields.Length; safariMonIdx++)
                    safariMonFields[safariMonIdx].children[0].GetValue().Set(encounterTableFile.safariMons[safariMonIdx]);
            }

            fileManager.WriteMonoBehaviours(PathEnum.Gamesettings, encounterTableMonoBehaviours);
            return;
        }

        /// <summary>
        ///  Updates loaded yaml with EncounterTables.
        /// </summary>
        private static void CommitEncounterTablesYAML()
        {
            List<YamlMonoContainer> monoBehaviours = fileManager.GetYAMLs(PathEnum.Gamesettings);
            FieldEncountTable[] encounterTableMonoBehaviours = new FieldEncountTable[2]
            {
                monoBehaviours.Find(m => m.MonoBehaviour.Name == "FieldEncountTable_d").MonoBehaviour as FieldEncountTable,
                monoBehaviours.Find(m => m.MonoBehaviour.Name == "FieldEncountTable_p").MonoBehaviour as FieldEncountTable,
            };

            for (int encounterTableFileIdx = 0; encounterTableFileIdx < encounterTableMonoBehaviours.Length; encounterTableFileIdx++)
            {
                EncounterTableFile encounterTableFile = gameData.encounterTableFiles[encounterTableFileIdx];
                encounterTableMonoBehaviours[encounterTableFileIdx].Name = encounterTableFile.mName;

                //Write wild encounter tables
                var table = encounterTableMonoBehaviours[encounterTableFileIdx].Table;
                for (int encounterTableIdx = 0; encounterTableIdx < table.Length; encounterTableIdx++)
                {
                    EncounterTable encounterTable = encounterTableFile.encounterTables[encounterTableIdx];
                    table[encounterTableIdx].ZoneID = (int)encounterTable.zoneID;
                    table[encounterTableIdx].EncRateGr = encounterTable.encRateGround;
                    table[encounterTableIdx].FormProb[0] = encounterTable.formProb;
                    table[encounterTableIdx].AnnoonTable[1] = encounterTable.unownTable;
                    table[encounterTableIdx].EncRateWat = encounterTable.encRateWater;
                    table[encounterTableIdx].EncRateTuriBoro = encounterTable.encRateOldRod;
                    table[encounterTableIdx].EncRateTuriIi = encounterTable.encRateGoodRod;
                    table[encounterTableIdx].EncRateSugoi = encounterTable.encRateSuperRod;

                    //Write ground tables
                    table[encounterTableIdx].GroundMons = ConvertEncountersToMonsLv(encounterTable.groundMons);

                    //Write morning tables
                    table[encounterTableIdx].Tairyo = ConvertEncountersToMonsLv(encounterTable.tairyo);

                    //Write day tables
                    table[encounterTableIdx].Day = ConvertEncountersToMonsLv(encounterTable.day);

                    //Write night tables
                    table[encounterTableIdx].Night = ConvertEncountersToMonsLv(encounterTable.night);

                    //Write pokefinder tables
                    table[encounterTableIdx].SwayGrass = ConvertEncountersToMonsLv(encounterTable.swayGrass);

                    //Write ruby tables
                    table[encounterTableIdx].GBARuby = ConvertEncountersToMonsLv(encounterTable.gbaRuby);

                    //Write sapphire tables
                    table[encounterTableIdx].GBASapp = ConvertEncountersToMonsLv(encounterTable.gbaSapphire);

                    //Write emerald tables
                    table[encounterTableIdx].GBAEme = ConvertEncountersToMonsLv(encounterTable.gbaEmerald);

                    //Write fire tables
                    table[encounterTableIdx].GBAFire = ConvertEncountersToMonsLv(encounterTable.gbaFire);

                    //Write leaf tables
                    table[encounterTableIdx].GBALeaf = ConvertEncountersToMonsLv(encounterTable.gbaLeaf);

                    //Write surfing tables
                    table[encounterTableIdx].WaterMons = ConvertEncountersToMonsLv(encounterTable.waterMons);

                    //Write old rod tables
                    table[encounterTableIdx].BoroMons = ConvertEncountersToMonsLv(encounterTable.oldRodMons);

                    //Write good rod tables
                    table[encounterTableIdx].IiMons = ConvertEncountersToMonsLv(encounterTable.goodRodMons);

                    //Write super rod tables
                    table[encounterTableIdx].SugoiMons = ConvertEncountersToMonsLv(encounterTable.superRodMons);
                }

                //Write trophy garden table
                encounterTableMonoBehaviours[encounterTableFileIdx].Urayama = new Sheeturayama[encounterTableFile.trophyGardenMons.Count];
                for (int trophyGardenMonIdx = 0; trophyGardenMonIdx < encounterTableMonoBehaviours[encounterTableFileIdx].Urayama.Length; trophyGardenMonIdx++)
                {
                    encounterTableMonoBehaviours[encounterTableFileIdx].Urayama[trophyGardenMonIdx] = new()
                    {
                        MonsNo = encounterTableFile.trophyGardenMons[trophyGardenMonIdx]
                    };
                }

                //Write honey tree tables (not actually honey trees?)
                encounterTableMonoBehaviours[encounterTableFileIdx].Mistu = new Sheetmistu[encounterTableFile.honeyTreeEnconters.Count];
                for (int honeyTreeEncounterIdx = 0; honeyTreeEncounterIdx < encounterTableMonoBehaviours[encounterTableFileIdx].HoneyTree.Length; honeyTreeEncounterIdx++)
                {
                    encounterTableMonoBehaviours[encounterTableFileIdx].Mistu[honeyTreeEncounterIdx] = new()
                    {
                        Rate = encounterTableFile.honeyTreeEnconters[honeyTreeEncounterIdx].rate,
                        Normal = encounterTableFile.honeyTreeEnconters[honeyTreeEncounterIdx].normalDexID,
                        Rare = encounterTableFile.honeyTreeEnconters[honeyTreeEncounterIdx].rareDexID,
                        SuperRare = encounterTableFile.honeyTreeEnconters[honeyTreeEncounterIdx].superRareDexID,
                    };
                }

                //Write safari table
                encounterTableMonoBehaviours[encounterTableFileIdx].Safari = new Sheetsafari[encounterTableFile.safariMons.Count];
                for (int safariMonIdx = 0; safariMonIdx < encounterTableMonoBehaviours[encounterTableFileIdx].Safari.Length; safariMonIdx++)
                {
                    encounterTableMonoBehaviours[encounterTableFileIdx].Safari[safariMonIdx] = new()
                    {
                        MonsNo = encounterTableFile.safariMons[safariMonIdx]
                    };
                }
            }
        }

        /// <summary>
        ///  Updates loaded bundle with Trainers.
        /// </summary>
        private static void CommitTrainers()
        {
            AssetTypeValueField monoBehaviour = fileManager.GetMonoBehaviours(PathEnum.DprMasterdatas).Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "TrainerTable");

            AssetTypeValueField[] trainerFields = monoBehaviour.children[5].children[0].children;
            for (int trainerIdx = 0; trainerIdx < gameData.trainers.Count; trainerIdx++)
            {
                Trainer trainer = gameData.trainers[trainerIdx];
                trainerFields[trainerIdx].children[0].GetValue().Set(trainer.trainerTypeID);
                trainerFields[trainerIdx].children[1].GetValue().Set(trainer.colorID);
                trainerFields[trainerIdx].children[2].GetValue().Set(trainer.fightType);
                trainerFields[trainerIdx].children[3].GetValue().Set(trainer.arenaID);
                trainerFields[trainerIdx].children[4].GetValue().Set(trainer.effectID);
                trainerFields[trainerIdx].children[5].GetValue().Set(trainer.gold);
                trainerFields[trainerIdx].children[6].GetValue().Set(trainer.useItem1);
                trainerFields[trainerIdx].children[7].GetValue().Set(trainer.useItem2);
                trainerFields[trainerIdx].children[8].GetValue().Set(trainer.useItem3);
                trainerFields[trainerIdx].children[9].GetValue().Set(trainer.useItem4);
                trainerFields[trainerIdx].children[10].GetValue().Set(trainer.hpRecoverFlag);
                trainerFields[trainerIdx].children[11].GetValue().Set(trainer.giftItem);
                trainerFields[trainerIdx].children[12].GetValue().Set(trainer.nameLabel);
                trainerFields[trainerIdx].children[19].GetValue().Set(trainer.aiBit);

                //Write trainer pokemon
                List<List<AssetTypeValue>> tranierPokemons = new();
                List<AssetTypeValue> atvs = new();
                atvs.Add(monoBehaviour.children[6].children[0].children[trainerIdx].children[0].GetValue());
                for (int trainerPokemonIdx = 0; trainerPokemonIdx < (int)GetBoundaries(AbsoluteBoundary.TrainerPokemonCount)[2]; trainerPokemonIdx++)
                {
                    TrainerPokemon trainerPokemon = new();
                    if (gameData.trainers[trainerIdx].trainerPokemon.Count > trainerPokemonIdx)
                        trainerPokemon = gameData.trainers[trainerIdx].trainerPokemon[trainerPokemonIdx];
                    atvs.Add(new AssetTypeValue(EnumValueTypes.UInt16, trainerPokemon.dexID));
                    atvs.Add(new AssetTypeValue(EnumValueTypes.UInt16, trainerPokemon.formID));
                    atvs.Add(new AssetTypeValue(EnumValueTypes.UInt8, trainerPokemon.isRare));
                    atvs.Add(new AssetTypeValue(EnumValueTypes.UInt8, trainerPokemon.level));
                    atvs.Add(new AssetTypeValue(EnumValueTypes.UInt8, trainerPokemon.sex));
                    atvs.Add(new AssetTypeValue(EnumValueTypes.UInt8, trainerPokemon.natureID));
                    atvs.Add(new AssetTypeValue(EnumValueTypes.UInt16, trainerPokemon.abilityID));
                    atvs.Add(new AssetTypeValue(EnumValueTypes.UInt16, trainerPokemon.moveID1));
                    atvs.Add(new AssetTypeValue(EnumValueTypes.UInt16, trainerPokemon.moveID2));
                    atvs.Add(new AssetTypeValue(EnumValueTypes.UInt16, trainerPokemon.moveID3));
                    atvs.Add(new AssetTypeValue(EnumValueTypes.UInt16, trainerPokemon.moveID4));
                    atvs.Add(new AssetTypeValue(EnumValueTypes.UInt16, trainerPokemon.itemID));
                    atvs.Add(new AssetTypeValue(EnumValueTypes.UInt8, trainerPokemon.ballID));
                    atvs.Add(new AssetTypeValue(EnumValueTypes.Int32, trainerPokemon.seal));
                    atvs.Add(new AssetTypeValue(EnumValueTypes.UInt8, trainerPokemon.hpIV));
                    atvs.Add(new AssetTypeValue(EnumValueTypes.UInt8, trainerPokemon.atkIV));
                    atvs.Add(new AssetTypeValue(EnumValueTypes.UInt8, trainerPokemon.defIV));
                    atvs.Add(new AssetTypeValue(EnumValueTypes.UInt8, trainerPokemon.spAtkIV));
                    atvs.Add(new AssetTypeValue(EnumValueTypes.UInt8, trainerPokemon.spDefIV));
                    atvs.Add(new AssetTypeValue(EnumValueTypes.UInt8, trainerPokemon.spdIV));
                    atvs.Add(new AssetTypeValue(EnumValueTypes.UInt8, trainerPokemon.hpEV));
                    atvs.Add(new AssetTypeValue(EnumValueTypes.UInt8, trainerPokemon.atkEV));
                    atvs.Add(new AssetTypeValue(EnumValueTypes.UInt8, trainerPokemon.defEV));
                    atvs.Add(new AssetTypeValue(EnumValueTypes.UInt8, trainerPokemon.spAtkEV));
                    atvs.Add(new AssetTypeValue(EnumValueTypes.UInt8, trainerPokemon.spDefEV));
                    atvs.Add(new AssetTypeValue(EnumValueTypes.UInt8, trainerPokemon.spdEV));
                }
                tranierPokemons.Add(atvs);
                AssetTypeValueField trainerPokemonsReference = monoBehaviour.children[6].children[0].children[trainerIdx];
                monoBehaviour.children[6].children[0].children[trainerIdx] = GetATVFs(trainerPokemonsReference, tranierPokemons)[0];
            }

            fileManager.WriteMonoBehaviour(PathEnum.DprMasterdatas, monoBehaviour);
        }

        /// <summary>
        ///  Updates loaded yaml with Trainers.
        /// </summary>
        private static void CommitTrainersYAML()
        {
            var monoBehaviour = fileManager.GetYAMLs(PathEnum.DprMasterdatas).Find(y => y.MonoBehaviour?.Name == "TrainerTable").MonoBehaviour as TrainerTable;

            monoBehaviour.TrainerData = gameData.trainers.Select((t, i) => new SheetTrainerData()
            {
                TypeID = t.trainerTypeID,
                ColorID = t.colorID,
                FightType = t.fightType,
                ArenaID = t.arenaID,
                EffectID = t.effectID,
                Gold = t.gold,
                UseItem1 = t.useItem1,
                UseItem2 = t.useItem2,
                UseItem3 = t.useItem3,
                UseItem4 = t.useItem4,
                HPRecoverFlag = t.hpRecoverFlag,
                GiftItem = t.giftItem,
                NameLabel = t.nameLabel,
                MsgFieldPokeOne = i < monoBehaviour.TrainerData.Length ? monoBehaviour.TrainerData[i].MsgFieldPokeOne : string.Empty,
                MsgFieldBefore = i < monoBehaviour.TrainerData.Length ? monoBehaviour.TrainerData[i].MsgFieldBefore : string.Empty,
                MsgFieldRevenge = i < monoBehaviour.TrainerData.Length ? monoBehaviour.TrainerData[i].MsgFieldRevenge : string.Empty,
                MsgFieldAfter = i < monoBehaviour.TrainerData.Length ? monoBehaviour.TrainerData[i].MsgFieldAfter : string.Empty,
                MsgBattle = i < monoBehaviour.TrainerData.Length ? monoBehaviour.TrainerData[i].MsgBattle : new string[8],
                SeqBattle = i < monoBehaviour.TrainerData.Length ? monoBehaviour.TrainerData[i].SeqBattle : Array.Empty<string>(),
                AIBit = t.aiBit,
            }).ToArray();

            monoBehaviour.TrainerPoke = gameData.trainers.Select((t, i) =>
            {
                var poke = new SheetTrainerPoke() { ID = i };

                if (t.trainerPokemon.Count >= 1)
                {
                    poke.P1MonsNo = t.trainerPokemon[0].dexID;
                    poke.P1FormNo = t.trainerPokemon[0].formID;
                    poke.P1IsRare = t.trainerPokemon[0].isRare;
                    poke.P1Level = t.trainerPokemon[0].level;
                    poke.P1Sex = t.trainerPokemon[0].sex;
                    poke.P1Seikaku = t.trainerPokemon[0].natureID;
                    poke.P1Tokusei = t.trainerPokemon[0].abilityID;
                    poke.P1Waza1 = t.trainerPokemon[0].moveID1;
                    poke.P1Waza2 = t.trainerPokemon[0].moveID2;
                    poke.P1Waza3 = t.trainerPokemon[0].moveID3;
                    poke.P1Waza4 = t.trainerPokemon[0].moveID4;
                    poke.P1Item = t.trainerPokemon[0].itemID;
                    poke.P1Ball = t.trainerPokemon[0].ballID;
                    poke.P1Seal = t.trainerPokemon[0].seal;
                    poke.P1TalentHp = t.trainerPokemon[0].hpIV;
                    poke.P1TalentAtk = t.trainerPokemon[0].atkIV;
                    poke.P1TalentDef = t.trainerPokemon[0].defIV;
                    poke.P1TalentSpAtk = t.trainerPokemon[0].spAtkIV;
                    poke.P1TalentSpDef = t.trainerPokemon[0].spDefIV;
                    poke.P1TalentAgi = t.trainerPokemon[0].spdIV;
                    poke.P1EffortHp = t.trainerPokemon[0].hpEV;
                    poke.P1EffortAtk = t.trainerPokemon[0].atkEV;
                    poke.P1EffortDef = t.trainerPokemon[0].defEV;
                    poke.P1EffortSpAtk = t.trainerPokemon[0].spAtkEV;
                    poke.P1EffortSpDef = t.trainerPokemon[0].spDefEV;
                    poke.P1EffortAgi = t.trainerPokemon[0].spdEV;
                }

                if (t.trainerPokemon.Count >= 2)
                {
                    poke.P2MonsNo = t.trainerPokemon[1].dexID;
                    poke.P2FormNo = t.trainerPokemon[1].formID;
                    poke.P2IsRare = t.trainerPokemon[1].isRare;
                    poke.P2Level = t.trainerPokemon[1].level;
                    poke.P2Sex = t.trainerPokemon[1].sex;
                    poke.P2Seikaku = t.trainerPokemon[1].natureID;
                    poke.P2Tokusei = t.trainerPokemon[1].abilityID;
                    poke.P2Waza1 = t.trainerPokemon[1].moveID1;
                    poke.P2Waza2 = t.trainerPokemon[1].moveID2;
                    poke.P2Waza3 = t.trainerPokemon[1].moveID3;
                    poke.P2Waza4 = t.trainerPokemon[1].moveID4;
                    poke.P2Item = t.trainerPokemon[1].itemID;
                    poke.P2Ball = t.trainerPokemon[1].ballID;
                    poke.P2Seal = t.trainerPokemon[1].seal;
                    poke.P2TalentHp = t.trainerPokemon[1].hpIV;
                    poke.P2TalentAtk = t.trainerPokemon[1].atkIV;
                    poke.P2TalentDef = t.trainerPokemon[1].defIV;
                    poke.P2TalentSpAtk = t.trainerPokemon[1].spAtkIV;
                    poke.P2TalentSpDef = t.trainerPokemon[1].spDefIV;
                    poke.P2TalentAgi = t.trainerPokemon[1].spdIV;
                    poke.P2EffortHp = t.trainerPokemon[1].hpEV;
                    poke.P2EffortAtk = t.trainerPokemon[1].atkEV;
                    poke.P2EffortDef = t.trainerPokemon[1].defEV;
                    poke.P2EffortSpAtk = t.trainerPokemon[1].spAtkEV;
                    poke.P2EffortSpDef = t.trainerPokemon[1].spDefEV;
                    poke.P2EffortAgi = t.trainerPokemon[1].spdEV;
                }

                if (t.trainerPokemon.Count >= 3)
                {
                    poke.P3MonsNo = t.trainerPokemon[2].dexID;
                    poke.P3FormNo = t.trainerPokemon[2].formID;
                    poke.P3IsRare = t.trainerPokemon[2].isRare;
                    poke.P3Level = t.trainerPokemon[2].level;
                    poke.P3Sex = t.trainerPokemon[2].sex;
                    poke.P3Seikaku = t.trainerPokemon[2].natureID;
                    poke.P3Tokusei = t.trainerPokemon[2].abilityID;
                    poke.P3Waza1 = t.trainerPokemon[2].moveID1;
                    poke.P3Waza2 = t.trainerPokemon[2].moveID2;
                    poke.P3Waza3 = t.trainerPokemon[2].moveID3;
                    poke.P3Waza4 = t.trainerPokemon[2].moveID4;
                    poke.P3Item = t.trainerPokemon[2].itemID;
                    poke.P3Ball = t.trainerPokemon[2].ballID;
                    poke.P3Seal = t.trainerPokemon[2].seal;
                    poke.P3TalentHp = t.trainerPokemon[2].hpIV;
                    poke.P3TalentAtk = t.trainerPokemon[2].atkIV;
                    poke.P3TalentDef = t.trainerPokemon[2].defIV;
                    poke.P3TalentSpAtk = t.trainerPokemon[2].spAtkIV;
                    poke.P3TalentSpDef = t.trainerPokemon[2].spDefIV;
                    poke.P3TalentAgi = t.trainerPokemon[2].spdIV;
                    poke.P3EffortHp = t.trainerPokemon[2].hpEV;
                    poke.P3EffortAtk = t.trainerPokemon[2].atkEV;
                    poke.P3EffortDef = t.trainerPokemon[2].defEV;
                    poke.P3EffortSpAtk = t.trainerPokemon[2].spAtkEV;
                    poke.P3EffortSpDef = t.trainerPokemon[2].spDefEV;
                    poke.P3EffortAgi = t.trainerPokemon[2].spdEV;
                }

                if (t.trainerPokemon.Count >= 4)
                {
                    poke.P4MonsNo = t.trainerPokemon[3].dexID;
                    poke.P4FormNo = t.trainerPokemon[3].formID;
                    poke.P4IsRare = t.trainerPokemon[3].isRare;
                    poke.P4Level = t.trainerPokemon[3].level;
                    poke.P4Sex = t.trainerPokemon[3].sex;
                    poke.P4Seikaku = t.trainerPokemon[3].natureID;
                    poke.P4Tokusei = t.trainerPokemon[3].abilityID;
                    poke.P4Waza1 = t.trainerPokemon[3].moveID1;
                    poke.P4Waza2 = t.trainerPokemon[3].moveID2;
                    poke.P4Waza3 = t.trainerPokemon[3].moveID3;
                    poke.P4Waza4 = t.trainerPokemon[3].moveID4;
                    poke.P4Item = t.trainerPokemon[3].itemID;
                    poke.P4Ball = t.trainerPokemon[3].ballID;
                    poke.P4Seal = t.trainerPokemon[3].seal;
                    poke.P4TalentHp = t.trainerPokemon[3].hpIV;
                    poke.P4TalentAtk = t.trainerPokemon[3].atkIV;
                    poke.P4TalentDef = t.trainerPokemon[3].defIV;
                    poke.P4TalentSpAtk = t.trainerPokemon[3].spAtkIV;
                    poke.P4TalentSpDef = t.trainerPokemon[3].spDefIV;
                    poke.P4TalentAgi = t.trainerPokemon[3].spdIV;
                    poke.P4EffortHp = t.trainerPokemon[3].hpEV;
                    poke.P4EffortAtk = t.trainerPokemon[3].atkEV;
                    poke.P4EffortDef = t.trainerPokemon[3].defEV;
                    poke.P4EffortSpAtk = t.trainerPokemon[3].spAtkEV;
                    poke.P4EffortSpDef = t.trainerPokemon[3].spDefEV;
                    poke.P4EffortAgi = t.trainerPokemon[3].spdEV;
                }

                if (t.trainerPokemon.Count >= 5)
                {
                    poke.P5MonsNo = t.trainerPokemon[4].dexID;
                    poke.P5FormNo = t.trainerPokemon[4].formID;
                    poke.P5IsRare = t.trainerPokemon[4].isRare;
                    poke.P5Level = t.trainerPokemon[4].level;
                    poke.P5Sex = t.trainerPokemon[4].sex;
                    poke.P5Seikaku = t.trainerPokemon[4].natureID;
                    poke.P5Tokusei = t.trainerPokemon[4].abilityID;
                    poke.P5Waza1 = t.trainerPokemon[4].moveID1;
                    poke.P5Waza2 = t.trainerPokemon[4].moveID2;
                    poke.P5Waza3 = t.trainerPokemon[4].moveID3;
                    poke.P5Waza4 = t.trainerPokemon[4].moveID4;
                    poke.P5Item = t.trainerPokemon[4].itemID;
                    poke.P5Ball = t.trainerPokemon[4].ballID;
                    poke.P5Seal = t.trainerPokemon[4].seal;
                    poke.P5TalentHp = t.trainerPokemon[4].hpIV;
                    poke.P5TalentAtk = t.trainerPokemon[4].atkIV;
                    poke.P5TalentDef = t.trainerPokemon[4].defIV;
                    poke.P5TalentSpAtk = t.trainerPokemon[4].spAtkIV;
                    poke.P5TalentSpDef = t.trainerPokemon[4].spDefIV;
                    poke.P5TalentAgi = t.trainerPokemon[4].spdIV;
                    poke.P5EffortHp = t.trainerPokemon[4].hpEV;
                    poke.P5EffortAtk = t.trainerPokemon[4].atkEV;
                    poke.P5EffortDef = t.trainerPokemon[4].defEV;
                    poke.P5EffortSpAtk = t.trainerPokemon[4].spAtkEV;
                    poke.P5EffortSpDef = t.trainerPokemon[4].spDefEV;
                    poke.P5EffortAgi = t.trainerPokemon[4].spdEV;
                }

                if (t.trainerPokemon.Count >= 6)
                {
                    poke.P6MonsNo = t.trainerPokemon[5].dexID;
                    poke.P6FormNo = t.trainerPokemon[5].formID;
                    poke.P6IsRare = t.trainerPokemon[5].isRare;
                    poke.P6Level = t.trainerPokemon[5].level;
                    poke.P6Sex = t.trainerPokemon[5].sex;
                    poke.P6Seikaku = t.trainerPokemon[5].natureID;
                    poke.P6Tokusei = t.trainerPokemon[5].abilityID;
                    poke.P6Waza1 = t.trainerPokemon[5].moveID1;
                    poke.P6Waza2 = t.trainerPokemon[5].moveID2;
                    poke.P6Waza3 = t.trainerPokemon[5].moveID3;
                    poke.P6Waza4 = t.trainerPokemon[5].moveID4;
                    poke.P6Item = t.trainerPokemon[5].itemID;
                    poke.P6Ball = t.trainerPokemon[5].ballID;
                    poke.P6Seal = t.trainerPokemon[5].seal;
                    poke.P6TalentHp = t.trainerPokemon[5].hpIV;
                    poke.P6TalentAtk = t.trainerPokemon[5].atkIV;
                    poke.P6TalentDef = t.trainerPokemon[5].defIV;
                    poke.P6TalentSpAtk = t.trainerPokemon[5].spAtkIV;
                    poke.P6TalentSpDef = t.trainerPokemon[5].spDefIV;
                    poke.P6TalentAgi = t.trainerPokemon[5].spdIV;
                    poke.P6EffortHp = t.trainerPokemon[5].hpEV;
                    poke.P6EffortAtk = t.trainerPokemon[5].atkEV;
                    poke.P6EffortDef = t.trainerPokemon[5].defEV;
                    poke.P6EffortSpAtk = t.trainerPokemon[5].spAtkEV;
                    poke.P6EffortSpDef = t.trainerPokemon[5].spDefEV;
                    poke.P6EffortAgi = t.trainerPokemon[5].spdEV;
                }
                return poke;
            }).ToArray();
        }

        /// <summary>
        ///  Updates loaded bundle with Battle Tower Trainers.
        /// </summary>
        private static void CommitBattleTowerTrainers()
        {
            AssetTypeValueField monoBehaviour = fileManager.GetMonoBehaviours(PathEnum.DprMasterdatas).Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "TowerSingleStockTable");
            AssetTypeValueField monoBehaviour2 = fileManager.GetMonoBehaviours(PathEnum.DprMasterdatas).Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "TowerDoubleStockTable");

            AssetTypeValueField[] trainerFields = monoBehaviour.children[4].children[0].children;
            AssetTypeValueField[] trainerFieldsDouble = monoBehaviour2.children[4].children[0].children;
            for (int trainerIdx = 0; trainerIdx < trainerFields.Length; trainerIdx++)
            {
                BattleTowerTrainer trainer = gameData.battleTowerTrainers[trainerIdx];
                trainerFields[trainerIdx].children[0].GetValue().Set(trainer.trainerID2);
                trainerFields[trainerIdx].children[1].GetValue().Set(trainer.trainerTypeID);
                trainerFields[trainerIdx].children[2].children[0].children[0].GetValue().Set(trainer.battleTowerPokemonID1);
                trainerFields[trainerIdx].children[2].children[0].children[1].GetValue().Set(trainer.battleTowerPokemonID2);
                trainerFields[trainerIdx].children[2].children[0].children[2].GetValue().Set(trainer.battleTowerPokemonID3);
                trainerFields[trainerIdx].children[3].GetValue().Set(trainer.battleBGM);
                trainerFields[trainerIdx].children[4].GetValue().Set(trainer.winBGM);
            }

            for (int trainerIdx = 0; trainerIdx < trainerFieldsDouble.Length; trainerIdx++)
            {
                BattleTowerTrainer trainer = gameData.battleTowerTrainersDouble[trainerIdx];
                trainerFieldsDouble[trainerIdx].children[0].GetValue().Set(trainer.trainerID2);
                trainerFieldsDouble[trainerIdx].children[1].children[0].children[0].GetValue().Set(trainer.trainerTypeID);
                trainerFieldsDouble[trainerIdx].children[1].children[0].children[1].GetValue().Set(trainer.trainerTypeID2);
                trainerFieldsDouble[trainerIdx].children[2].children[0].children[0].GetValue().Set(trainer.battleTowerPokemonID1);
                trainerFieldsDouble[trainerIdx].children[2].children[0].children[1].GetValue().Set(trainer.battleTowerPokemonID2);
                trainerFieldsDouble[trainerIdx].children[2].children[0].children[2].GetValue().Set(trainer.battleTowerPokemonID3);
                trainerFieldsDouble[trainerIdx].children[2].children[0].children[3].GetValue().Set(trainer.battleTowerPokemonID4);
                trainerFieldsDouble[trainerIdx].children[3].GetValue().Set(trainer.battleBGM);
                trainerFieldsDouble[trainerIdx].children[4].GetValue().Set(trainer.winBGM);
            }

            fileManager.WriteMonoBehaviour(PathEnum.DprMasterdatas, monoBehaviour);
            fileManager.WriteMonoBehaviour(PathEnum.DprMasterdatas, monoBehaviour2);
        }

        /// <summary>
        ///  Updates loaded yaml with Battle Tower Trainers.
        /// </summary>
        private static void CommitBattleTowerTrainersYAML()
        {
            var trainers = fileManager.GetYAMLs(PathEnum.DprMasterdatas).Find(y => y.MonoBehaviour?.Name == "TowerSingleStockTable").MonoBehaviour as TowerSingleStockTable;
            var trainersDouble = fileManager.GetYAMLs(PathEnum.DprMasterdatas).Find(y => y.MonoBehaviour?.Name == "TowerDoubleStockTable").MonoBehaviour as TowerDoubleStockTable;

            trainers.TowerSingleStock = gameData.battleTowerTrainers.Select(t => new SheetTowerSingleStock()
            {
                ID = t.trainerID2,
                TrainerID = t.trainerTypeID,
                PokeID = new uint[3] { t.battleTowerPokemonID1, t.battleTowerPokemonID2, t.battleTowerPokemonID3 },
                BattleBGM = t.battleBGM,
                WinBGM = t.winBGM,
            }).ToArray();

            trainersDouble.TowerDoubleStock = gameData.battleTowerTrainersDouble.Select(t => new SheetTowerDoubleStock()
            {
                ID = t.trainerID2,
                TrainerID = new int[2] { t.trainerTypeID, t.trainerTypeID2 },
                PokeID = new uint[4] { t.battleTowerPokemonID1, t.battleTowerPokemonID2, t.battleTowerPokemonID3, t.battleTowerPokemonID4 },
                BattleBGM = t.battleBGM,
                WinBGM = t.winBGM,
            }).ToArray();
        }

        /// <summary>
        ///  Updates loaded bundle with Battle Tower Pokémon.
        /// </summary>
        private static void CommitBattleTowerPokemon()
        {
            AssetTypeValueField monoBehaviour = fileManager.GetMonoBehaviours(PathEnum.DprMasterdatas).Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "TowerTrainerTable");

            AssetTypeValueField[] pokemonFields = monoBehaviour.children[5].children[0].children;
            //Parse battle tower trainer pokemon
            for (int pokemonIdx = 0; pokemonIdx < pokemonFields.Length && pokemonFields[pokemonIdx].children[0].value.value.asUInt32 != 0; pokemonIdx += 1)
            {
                BattleTowerTrainerPokemon pokemon = gameData.battleTowerTrainerPokemons[pokemonIdx];
                // Trainer trainer = gameData.trainers[trainerIdx];
                pokemonFields[pokemonIdx].children[0].GetValue().Set(pokemon.pokemonID);
                pokemonFields[pokemonIdx].children[1].GetValue().Set(pokemon.dexID);
                pokemonFields[pokemonIdx].children[2].GetValue().Set(pokemon.formID);
                pokemonFields[pokemonIdx].children[3].GetValue().Set(pokemon.isRare);
                pokemonFields[pokemonIdx].children[4].GetValue().Set(pokemon.level);
                pokemonFields[pokemonIdx].children[5].GetValue().Set(pokemon.sex);
                pokemonFields[pokemonIdx].children[6].GetValue().Set(pokemon.natureID);
                pokemonFields[pokemonIdx].children[7].GetValue().Set(pokemon.abilityID);
                pokemonFields[pokemonIdx].children[8].GetValue().Set(pokemon.moveID1);
                pokemonFields[pokemonIdx].children[9].GetValue().Set(pokemon.moveID2);
                pokemonFields[pokemonIdx].children[10].GetValue().Set(pokemon.moveID3);
                pokemonFields[pokemonIdx].children[11].GetValue().Set(pokemon.moveID4);
                pokemonFields[pokemonIdx].children[12].GetValue().Set(pokemon.itemID);
                pokemonFields[pokemonIdx].children[13].GetValue().Set(pokemon.ballID);
                pokemonFields[pokemonIdx].children[14].GetValue().Set(pokemon.seal);
                pokemonFields[pokemonIdx].children[15].GetValue().Set(pokemon.hpIV);
                pokemonFields[pokemonIdx].children[16].GetValue().Set(pokemon.atkIV);
                pokemonFields[pokemonIdx].children[17].GetValue().Set(pokemon.defIV);
                pokemonFields[pokemonIdx].children[18].GetValue().Set(pokemon.spAtkIV);
                pokemonFields[pokemonIdx].children[19].GetValue().Set(pokemon.spDefIV);
                pokemonFields[pokemonIdx].children[20].GetValue().Set(pokemon.spdIV);
                pokemonFields[pokemonIdx].children[21].GetValue().Set(pokemon.hpEV);
                pokemonFields[pokemonIdx].children[22].GetValue().Set(pokemon.atkEV);
                pokemonFields[pokemonIdx].children[23].GetValue().Set(pokemon.defEV);
                pokemonFields[pokemonIdx].children[24].GetValue().Set(pokemon.spAtkEV);
                pokemonFields[pokemonIdx].children[25].GetValue().Set(pokemon.spDefEV);
                pokemonFields[pokemonIdx].children[26].GetValue().Set(pokemon.spdEV);
            }

            fileManager.WriteMonoBehaviour(PathEnum.DprMasterdatas, monoBehaviour);
        }
        
        /// <summary>
        ///  Updates loaded yaml with Battle Tower Pokémon.
        /// </summary>
        private static void CommitBattleTowerPokemonYAML()
        {
            var monoBehaviour = fileManager.GetYAMLs(PathEnum.DprMasterdatas).Find(y => y.MonoBehaviour?.Name == "TowerTrainerTable").MonoBehaviour as TowerTrainerTable;

            monoBehaviour.TrainerPoke = gameData.battleTowerTrainerPokemons.Select(p => new SheetTowerTrainerPoke()
            {
                ID = p.pokemonID,
                MonsNo = p.dexID,
                FormNo = (ushort)p.formID,
                IsRare = p.isRare,
                Level = p.level,
                Sex = p.sex,
                Seikaku = p.natureID,
                Tokusei = p.abilityID,
                Waza1 = p.moveID1,
                Waza2 = p.moveID2,
                Waza3 = p.moveID3,
                Waza4 = p.moveID4,
                Item = p.itemID,
                Ball = p.ballID,
                Seal = p.seal,
                TalentHP = p.hpIV,
                TalentAtk = p.atkIV,
                TalentDef = p.defIV,
                TalentSpAtk = p.spAtkIV,
                TalentSpDef = p.spDefIV,
                TalentAgi = p.spdIV,
                EffortHP = p.hpEV,
                EffortAtk = p.atkEV,
                EffortDef = p.defEV,
                EffortSpAtk = p.spAtkEV,
                EffortSpDef = p.spDefEV,
                EffortAgi = p.spdEV,
            }).ToArray();
        }

        /// <summary>
        ///  Updates loaded bundle with ShopTables.
        /// </summary>
        private static void CommitShopTables()
        {
            AssetTypeValueField monoBehaviour = fileManager.GetMonoBehaviours(PathEnum.DprMasterdatas).Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "ShopTable");

            List<List<AssetTypeValue>> martItems = new();
            for (int martItemIdx = 0; martItemIdx < gameData.shopTables.martItems.Count; martItemIdx++)
            {
                MartItem martItem = gameData.shopTables.martItems[martItemIdx];
                List<AssetTypeValue> atvs = new();
                atvs.Add(new AssetTypeValue(EnumValueTypes.UInt16, martItem.itemID));
                atvs.Add(new AssetTypeValue(EnumValueTypes.Int32, martItem.badgeNum));
                atvs.Add(new AssetTypeValue(EnumValueTypes.Int32, martItem.zoneID));
                martItems.Add(atvs);
            }
            AssetTypeValueField martItemReference = monoBehaviour.children[4].children[0].children[0];
            monoBehaviour.children[4].children[0].SetChildrenList(GetATVFs(martItemReference, martItems));

            List<List<AssetTypeValue>> fixedShopItems = new();
            for (int fixedShopItemIdx = 0; fixedShopItemIdx < gameData.shopTables.fixedShopItems.Count; fixedShopItemIdx++)
            {
                FixedShopItem fixedShopItem = gameData.shopTables.fixedShopItems[fixedShopItemIdx];
                List<AssetTypeValue> atvs = new();
                atvs.Add(new AssetTypeValue(EnumValueTypes.UInt16, fixedShopItem.itemID));
                atvs.Add(new AssetTypeValue(EnumValueTypes.Int32, fixedShopItem.shopID));
                fixedShopItems.Add(atvs);
            }
            AssetTypeValueField fixedShopItemReference = monoBehaviour.children[5].children[0].children[0];
            monoBehaviour.children[5].children[0].SetChildrenList(GetATVFs(fixedShopItemReference, fixedShopItems));

            List<List<AssetTypeValue>> bpShopItems = new();
            for (int bpShopItemIdx = 0; bpShopItemIdx < gameData.shopTables.bpShopItems.Count; bpShopItemIdx++)
            {
                BpShopItem bpShopItem = gameData.shopTables.bpShopItems[bpShopItemIdx];
                List<AssetTypeValue> atvs = new();
                atvs.Add(new AssetTypeValue(EnumValueTypes.UInt16, bpShopItem.itemID));
                atvs.Add(new AssetTypeValue(EnumValueTypes.Int32, bpShopItem.npcID));
                bpShopItems.Add(atvs);
            }
            AssetTypeValueField bpShopItemReference = monoBehaviour.children[9].children[0].children[0];
            monoBehaviour.children[9].children[0].SetChildrenList(GetATVFs(bpShopItemReference, bpShopItems));

            fileManager.WriteMonoBehaviour(PathEnum.DprMasterdatas, monoBehaviour);
        }

        /// <summary>
        ///  Updates loaded yaml with ShopTables.
        /// </summary>
        private static void CommitShopTablesYAML()
        {
            var monoBehaviour = fileManager.GetYAMLs(PathEnum.DprMasterdatas).Find(y => y.MonoBehaviour?.Name == "ShopTable").MonoBehaviour as ShopTable;

            monoBehaviour.FS = gameData.shopTables.martItems.Select(i => new SheetFS()
            {
                ItemNo = i.itemID,
                BadgeNum = i.badgeNum,
                ZoneID = i.zoneID,
            }).ToArray();

            monoBehaviour.FixedShop = gameData.shopTables.fixedShopItems.Select(i => new SheetFixedShop()
            {
                ItemNo = i.itemID,
                ShopID = i.shopID,
            }).ToArray();

            monoBehaviour.BPShop = gameData.shopTables.bpShopItems.Select(i => new SheetBPShop()
            {
                ItemNo = i.itemID,
                NPCID = i.npcID,
            }).ToArray();
        }

        /// <summary>
        ///  Updates loaded bundle with PickupItems.
        /// </summary>
        private static void CommitPickupItems()
        {
            AssetTypeValueField monoBehaviour = fileManager.GetMonoBehaviours(PathEnum.DprMasterdatas).Find(m => Encoding.Default.GetString(m.children[3].value.value.asString) == "MonohiroiTable");

            AssetTypeValueField[] pickupItemFields = monoBehaviour.children[4].children[0].children;
            for (int pickupItemIdx = 0; pickupItemIdx < pickupItemFields.Length; pickupItemIdx++)
            {
                PickupItem pickupItem = gameData.pickupItems[pickupItemIdx];
                pickupItemFields[pickupItemIdx].children[0].GetValue().Set(pickupItem.itemID);

                //Write item probabilities
                for (int ratio = 0; ratio < pickupItemFields[pickupItemIdx].children[1].children[0].childrenCount; ratio++)
                    pickupItemFields[pickupItemIdx].children[1].children[0].children[ratio].GetValue().Set(pickupItem.ratios[ratio]);
            }

            fileManager.WriteMonoBehaviour(PathEnum.DprMasterdatas, monoBehaviour);
        }

        /// <summary>
        ///  Updates loaded yaml with PickupItems.
        /// </summary>
        private static void CommitPickupItemsYAML()
        {
            var monoBehaviour = fileManager.GetYAMLs(PathEnum.DprMasterdatas).Find(y => y.MonoBehaviour?.Name == "MonohiroiTable").MonoBehaviour as MonohiroiTable;

            var pickupItemFields = monoBehaviour.MonoHiroi;
            for (int pickupItemIdx = 0; pickupItemIdx < pickupItemFields.Length; pickupItemIdx++)
            {
                PickupItem pickupItem = gameData.pickupItems[pickupItemIdx];
                pickupItemFields[pickupItemIdx].ID = pickupItem.itemID;

                //Write item probabilities
                for (int ratio = 0; ratio < pickupItemFields[pickupItemIdx].Ratios.Length && ratio < pickupItem.ratios.Count; ratio++)
                    pickupItemFields[pickupItemIdx].Ratios[ratio] = pickupItem.ratios[ratio];
            }
        }

        /// <summary>
        ///  Updates loaded bundle with EvScripts.
        /// </summary>
        private static void CommitEvScripts()
        {
            List<AssetTypeValueField> monoBehaviours = fileManager.GetMonoBehaviours(PathEnum.EvScript).Where(m => m.children[4].GetName() == "Scripts").ToList();

            for (int mIdx = 0; mIdx < monoBehaviours.Count; mIdx++)
            {
                EvScript evScript = gameData.evScripts[mIdx];
                monoBehaviours[mIdx].children[3].GetValue().Set(evScript.mName);

                //Write Scripts
                AssetTypeValueField[] scriptFields = monoBehaviours[mIdx].children[4].children[0].children;
                for (int scriptIdx = 0; scriptIdx < scriptFields.Length; scriptIdx++)
                {
                    var script = evScript.scripts[scriptIdx];
                    scriptFields[scriptIdx].children[0].GetValue().Set(script.evLabel);

                    //Write Commands
                    AssetTypeValueField[] commandFields = scriptFields[scriptIdx].children[1].children[0].children;
                    for (int commandIdx = 0; commandIdx < commandFields.Length; commandIdx++)
                    {
                        var command = script.commands[commandIdx];

                        //Check for commands without data, because those exist for some reason.
                        if (commandFields[commandIdx].children[0].children[0].children.Length == 0)
                            continue;

                        commandFields[commandIdx].children[0].children[0].children[0].children[1].GetValue().Set(command.cmdType);

                        //Write Arguments
                        AssetTypeValueField[] argumentFields = commandFields[commandIdx].children[0].children[0].children;
                        for (int argIdx = 1; argIdx < argumentFields.Length; argIdx++)
                        {
                            Argument arg = command.args[argIdx - 1];
                            argumentFields[argIdx].children[0].GetValue().Set(arg.argType);
                            argumentFields[argIdx].children[1].GetValue().Set(arg.data);
                            if (arg.argType == 1)
                                argumentFields[argIdx].children[1].GetValue().Set(ConvertToInt(arg.data));
                        }
                    }
                }

                //Write StrLists
                AssetTypeValueField[] stringFields = monoBehaviours[mIdx].children[5].children[0].children;
                for (int stringIdx = 0; stringIdx < stringFields.Length; stringIdx++)
                    stringFields[stringIdx].GetValue().Set(evScript.strList[stringIdx]);
            }

            fileManager.WriteMonoBehaviours(PathEnum.EvScript, monoBehaviours.ToArray());
        }

        /// <summary>
        ///  Updates loaded yaml with EvScripts.
        /// </summary>
        private static void CommitEvScriptsYAML()
        {
            var monoBehaviours = fileManager.GetYAMLs(PathEnum.EvScript).Select(y => y.MonoBehaviour).OfType<EvData>().ToList();

            for (int mIdx = 0; mIdx < monoBehaviours.Count; mIdx++)
            {
                EvScript evScript = gameData.evScripts[mIdx];
                monoBehaviours[mIdx].Name = evScript.mName;

                //Write Scripts
                var scriptFields = monoBehaviours[mIdx].Scripts;
                for (int scriptIdx = 0; scriptIdx < scriptFields.Count; scriptIdx++)
                {
                    var script = evScript.scripts[scriptIdx];
                    scriptFields[scriptIdx].Label = script.evLabel;

                    //Write Commands
                    var commandFields = scriptFields[scriptIdx].Commands;
                    for (int commandIdx = 0; commandIdx < commandFields.Count; commandIdx++)
                    {
                        var command = script.commands[commandIdx];

                        //Check for commands without data, because those exist for some reason.
                        if (commandFields[commandIdx].Arg.Count == 0)
                            continue;

                        commandFields[commandIdx].Arg[0] = new Aregment()
                        {
                            ArgType = commandFields[commandIdx].Arg[0].ArgType,
                            Data = command.cmdType,
                        };

                        //Write Arguments
                        var argumentFields = commandFields[commandIdx].Arg;
                        for (int argIdx = 1; argIdx < argumentFields.Count; argIdx++)
                        {
                            Argument arg = command.args[argIdx - 1];
                            argumentFields[argIdx] = new Aregment()
                            {
                                ArgType = arg.argType,
                                Data = arg.argType == 1 ? ConvertToInt(arg.data) : (int)arg.data,
                            };
                        }
                    }
                }

                //Write StrLists
                var stringFields = monoBehaviours[mIdx].StrList;
                for (int stringIdx = 0; stringIdx < stringFields.Count; stringIdx++)
                    stringFields[stringIdx] = evScript.strList[stringIdx];
            }
        }

        /// <summary>
        ///  Converts a List of Encounters into a AssetTypeValueField array.
        /// </summary>
        private static void SetEncounters(AssetTypeValueField encounterSetAtvf, List<Encounter> encounters)
        {
            List<List<AssetTypeValue>> encounterAtvs = new();
            for (int encounterIdx = 0; encounterIdx < encounterSetAtvf.GetChildrenCount(); encounterIdx++)
            {
                Encounter encounter = encounters[encounterIdx];
                List<AssetTypeValue> atvs = new();
                atvs.Add(new AssetTypeValue(EnumValueTypes.Int32, encounter.maxLv));
                atvs.Add(new AssetTypeValue(EnumValueTypes.Int32, encounter.minLv));
                atvs.Add(new AssetTypeValue(EnumValueTypes.Int32, encounter.dexID));
                encounterAtvs.Add(atvs);
            }
            AssetTypeValueField martItemReference = encounterSetAtvf.children[0];
            encounterSetAtvf.SetChildrenList(GetATVFs(martItemReference, encounterAtvs));
        }

        /// <summary>
        ///  Converts a List of Encounters into an array of MonsLv.
        /// </summary>
        private static MonsLv[] ConvertEncountersToMonsLv(List<Encounter> encounters)
        {
            MonsLv[] monsLv = new MonsLv[encounters.Count];
            for (int encounterIdx = 0; encounterIdx < monsLv.Length; encounterIdx++)
            {
                monsLv[encounterIdx] = new()
                {
                    MaxLv = encounters[encounterIdx].maxLv,
                    MinLv = encounters[encounterIdx].minLv,
                    MonsNo = encounters[encounterIdx].dexID,
                };
            }

            return monsLv;
        }

        /// <summary>
        ///  Updates loaded bundle with PickupItems.
        /// </summary>
        private static AssetTypeValueField[] GetATVFs(AssetTypeValueField reference, List<List<AssetTypeValue>> items)
        {
            List<AssetTypeValueField> atvfs = new();
            for (int itemIdx = 0; itemIdx < items.Count; itemIdx++)
            {
                List<AssetTypeValue> item = items[itemIdx];
                AssetTypeValueField atvf = new();
                AssetTypeValueField[] children = new AssetTypeValueField[item.Count];

                for (int i = 0; i < children.Length; i++)
                {
                    children[i] = new AssetTypeValueField();
                    children[i].Read(item[i], reference.children[i].templateField, Array.Empty<AssetTypeValueField>());
                }

                atvf.Read(null, reference.templateField, children);
                atvfs.Add(atvf);
            }
            return atvfs.ToArray();
        }

        /// <summary>
        ///  Interprets bytes of a float as an int32.
        /// </summary>
        private static int ConvertToInt(float n)
        {
            return BitConverter.ToInt32(BitConverter.GetBytes(n));
        }
    }
}
