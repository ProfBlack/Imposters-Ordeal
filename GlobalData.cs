using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using static ImpostersOrdeal.GameDataTypes;
using static ImpostersOrdeal.ExternalJsonStructs;
using SmartPoint.AssetAssistant;
using System.IO;

namespace ImpostersOrdeal
{
    /// <summary>
    ///  Stores data for easy access.
    /// </summary>
    public static class GlobalData
    {
        public class GameDataSet
        {
            public List<EvScript> evScripts;
            public List<MapWarpAsset> mapWarpAssets;
            public List<PickupItem> pickupItems;
            public ShopTables shopTables;
            public List<Trainer> trainers;
            public List<BattleTowerTrainer> battleTowerTrainers;
            public List<BattleTowerTrainer> battleTowerTrainersDouble;
            public List<BattleTowerTrainerPokemon> battleTowerTrainerPokemons;
            public EncounterTableFile[] encounterTableFiles;
            public MessageFileSet[] messageFileSets;
            public List<GrowthRate> growthRates;
            public List<UgArea> ugAreas;
            public List<UgEncounterFile> ugEncounterFiles;
            public List<UgEncounterLevelSet> ugEncounterLevelSets;
            public List<UgSpecialEncounter> ugSpecialEncounters;
            public List<GameDataTypes.UgPokemonData> ugPokemonData;
            public List<Ability> abilities; //Readonly
            public List<Typing> typings; //Readonly
            public List<DamageCategory> damageCategories; //Readonly
            public List<Nature> natures; //Readonly
            public List<TrainerType> trainerTypes; //Readonly
            public List<Pokemon> personalEntries; //Ordered, idx=personalID
            public List<DexEntry> dexEntries; //Ordered, idx=dexID
            public List<Item> items; //Ordered, idx=itemID
            public List<TM> tms; //Ordered, idx=tmID
            public List<Move> moves; //Ordered, idx=moveID
            public Wwise.WwiseData audioData;
            public GlobalMetadata globalMetadata;
            public List<BattleMasterdatas.MotionTimingData> motionTimingData;
            public List<Masterdatas.PokemonInfoCatalog> pokemonInfos;
            public List<PersonalMasterdatas.AddPersonalTable> addPersonalTables;
            public List<UIMasterdatas.PokemonIcon> uiPokemonIcon;
            public List<UIMasterdatas.AshiatoIcon> uiAshiatoIcon;
            public List<UIMasterdatas.PokemonVoice> uiPokemonVoice;
            public List<UIMasterdatas.ZukanDisplay> uiZukanDisplay;
            public List<UIMasterdatas.ZukanCompareHeight> uiZukanCompareHeights;
            public List<UIMasterdatas.SearchPokeIconSex> uiSearchPokeIconSex;
            public UIMasterdatas.DistributionTable uiDistributionTable;
            public List<ResultMotion> contestResultMotion;
            public AssetBundleDownloadManifest dprBin;

            public List<(string name, Starter obj)> externalStarters;
            public List<(string name, HoneyTreeZone obj)> externalHoneyTrees;

            public Dictionary<string, string> trainerNames;
            public StringBuilder audioSourceLog;
            public ModArgs modArgs;

            private readonly bool[] fieldStates = new bool[Enum.GetNames(typeof(DataField)).Length];

            public enum DataField
            {
                EvScripts,
                MapWarpAssets,
                PickupItems,
                ShopTables,
                Trainers,
                BattleTowerTrainers,
                BattleTowerTrainerPokemons,
                EncounterTableFiles,
                MessageFileSets,
                GrowthRates,
                UgAreas,
                UgEncounterFiles,
                UgEncounterLevelSets,
                UgSpecialEncounters,
                UgPokemonData,
                Abilities,
                Typings,
                DamageCategories,
                Natures,
                PersonalEntries,
                DexEntries,
                Items,
                TMs,
                Moves,
                AudioData,
                GlobalMetadata,
                UIMasterdatas,
                AddPersonalTable,
                MotionTimingData,
                PokemonInfo,
                ContestResultMotion,
                DprBin,
                ExternalStarters,
                ExternalHoneyTrees
            }

            public bool IsModified(DataField d)
            {
                return fieldStates[(int)d];
            }

            public void SetModified(DataField d)
            {
                fieldStates[(int)d] = true;
            }

            public Pokemon GetPokemon(int dexID, int formID)
            {
                return dexEntries[dexID].forms[formID];
            }

            public string GetTPDisplayName(TrainerPokemon tp)
            {
                return "Lv. " + tp.level + " " + GetPokemon(tp.dexID, tp.formID).GetName();
            }

            public bool UgVersionsUnbounded()
            {
                return modArgs != null ? modArgs.ugVersionsUnbounded : ugEncounterFiles
                    .SelectMany(o => o.ugEncounters)
                    .Any(e => e.version < 1 || e.version > 3);
            }

            public bool Uint16UgTables()
            {
                return modArgs != null ? modArgs.uint16UgTables : ugEncounterFiles
                    .SelectMany(ugef => ugef.ugEncounters)
                    .Any(uge => (uint)uge.dexID > 0xFFFF); 
            }

            public bool Uint16EncounterTables()
            {
                return modArgs != null ? modArgs.uint16EncounterTables : encounterTableFiles
                    .Any(etf => etf.encounterTables
                    .Any(o => o.GetAllTables()
                    .Any(l => l
                    .Any(e => (uint)e.dexID > 0xFFFF))));
            }

            public int GetEvolutionMethodCount()
            {
                int emCount = modArgs != null ? modArgs.evolutionMethodCount : 0;
                if (emCount == 0)
                    emCount = Math.Max(48, (int)personalEntries.SelectMany(p => p.evolutionPaths.Select(em => em.method)).Max());
                return emCount;
            }

            public bool FormDescriptionsExist()
            {
                return messageFileSets.SelectMany(mfs => mfs.messageFiles)
                    .First(mf => mf.mName.Contains("dp_pokedex_diamond"))
                    .labelDatas.Count >= personalEntries.Count - 1;
            }

            public int GetTMCompatibilitySetSize()
            {
                return personalEntries.First(p => p.IsValid()).GetTMCompatibility().Length;
            }
        }

        public static FileManager fileManager;
        public static Dictionary<PathEnum, string> randomizerPaths = new();
        public static Dictionary<string, PathEnum> yamlAssetPaths = new();
        public static GameDataSet gameData;
        public static DataTable absoluteBoundaries = new();

        public static DataRow GetBoundaries(AbsoluteBoundary a)
        {
            if ((int)a == -1)
                return null;
            return absoluteBoundaries.Rows[(int)a];
        }

        public static bool IsWithin(AbsoluteBoundary a, int x)
        {
            if ((int)a == -1)
                return true;
            DataRow b = GetBoundaries(a);
            return x >= (int)b[1] && x <= (int)b[2];
        }

        public static int Conform(AbsoluteBoundary a, int x)
        {
            if ((int)a == -1)
                return x;
            DataRow b = GetBoundaries(a);
            x = (int)(Math.Round((double)x / (int)b[3]) * (int)b[3]);
            return Math.Clamp(x, (int)b[1], (int)b[2]);
        }

        public static string GetZoneName(int index)
        {
            if (index > -1 && index < Zones.zoneNames.Length)
                return Zones.zoneNames[index];

            return Zones.defaultName;
        }

        public enum PathEnum
        {
            EvScript,
            DprMasterdatas,
            Gamesettings,
            CommonMsbt,
            English,
            French,
            German,
            Italian,
            Jpn,
            JpnKanji,
            Korean,
            SimpChinese,
            Spanish,
            TradChinese,
            PersonalMasterdatas,
            Ugdata,
            BattleMasterdatas,
            UIMasterdatas,
            ContestMasterdatas,

            ExternalJSON,
        }

        public enum AbsoluteBoundary
        {
            Level,
            BaseStat,
            CatchRate,
            EvYield,
            EvYieldTotal,
            InitialFriendship,
            ExpYield,
            LevelUpMoveCount,
            EggMoveCount,
            Power,
            Accuracy,
            Pp,
            TrainerPokemonCount,
            Iv,
            Ev,
            EvTotal,
            Price,
            None = -1
        }

        public static void Initialize()
        {
            randomizerPaths[PathEnum.BattleMasterdatas] =   Path.Combine("romfs", "Data", "StreamingAssets", "AssetAssistant", "Battle", "battle_masterdatas");
            randomizerPaths[PathEnum.ContestMasterdatas] =  Path.Combine("romfs", "Data", "StreamingAssets", "AssetAssistant", "Contest", "md", "contest_masterdatas");
            randomizerPaths[PathEnum.EvScript] =            Path.Combine("romfs", "Data", "StreamingAssets", "AssetAssistant", "Dpr", "ev_script");
            randomizerPaths[PathEnum.DprMasterdatas] =      Path.Combine("romfs", "Data", "StreamingAssets", "AssetAssistant", "Dpr", "masterdatas");
            randomizerPaths[PathEnum.Gamesettings] =        Path.Combine("romfs", "Data", "StreamingAssets", "AssetAssistant", "Dpr", "scriptableobjects", "gamesettings");
            randomizerPaths[PathEnum.CommonMsbt] =          Path.Combine("romfs", "Data", "StreamingAssets", "AssetAssistant", "Message", "common_msbt");
            randomizerPaths[PathEnum.English] =             Path.Combine("romfs", "Data", "StreamingAssets", "AssetAssistant", "Message", "english");
            randomizerPaths[PathEnum.French] =              Path.Combine("romfs", "Data", "StreamingAssets", "AssetAssistant", "Message", "french");
            randomizerPaths[PathEnum.German] =              Path.Combine("romfs", "Data", "StreamingAssets", "AssetAssistant", "Message", "german");
            randomizerPaths[PathEnum.Italian] =             Path.Combine("romfs", "Data", "StreamingAssets", "AssetAssistant", "Message", "italian");
            randomizerPaths[PathEnum.Jpn] =                 Path.Combine("romfs", "Data", "StreamingAssets", "AssetAssistant", "Message", "jpn");
            randomizerPaths[PathEnum.JpnKanji] =            Path.Combine("romfs", "Data", "StreamingAssets", "AssetAssistant", "Message", "jpn_kanji");
            randomizerPaths[PathEnum.Korean] =              Path.Combine("romfs", "Data", "StreamingAssets", "AssetAssistant", "Message", "korean");
            randomizerPaths[PathEnum.SimpChinese] =         Path.Combine("romfs", "Data", "StreamingAssets", "AssetAssistant", "Message", "simp_chinese");
            randomizerPaths[PathEnum.Spanish] =             Path.Combine("romfs", "Data", "StreamingAssets", "AssetAssistant", "Message", "spanish");
            randomizerPaths[PathEnum.TradChinese] =         Path.Combine("romfs", "Data", "StreamingAssets", "AssetAssistant", "Message", "trad_chinese");
            randomizerPaths[PathEnum.PersonalMasterdatas] = Path.Combine("romfs", "Data", "StreamingAssets", "AssetAssistant", "Pml", "personal_masterdatas");
            randomizerPaths[PathEnum.UIMasterdatas] =       Path.Combine("romfs", "Data", "StreamingAssets", "AssetAssistant", "UIs", "masterdatas", "uimasterdatas");
            randomizerPaths[PathEnum.Ugdata] =              Path.Combine("romfs", "Data", "StreamingAssets", "AssetAssistant", "UnderGround", "data", "ugdata");

            yamlAssetPaths­["md"]                                                     = PathEnum.BattleMasterdatas;
            yamlAssetPaths­[Path.Combine("masterdatas", "contestdatas")]              = PathEnum.ContestMasterdatas;
            yamlAssetPaths­[Path.Combine("evscriptdata", "eventasset")]               = PathEnum.EvScript;
            //yamlAssetPaths­[Path.Combine("evscriptdata", "eventcamera")]              = PathEnum.EvScript;
            //yamlAssetPaths­[Path.Combine("md", "adventurenote")]                      = PathEnum.DprMasterdatas;
            //yamlAssetPaths­[Path.Combine("md", "characteranime")]                     = PathEnum.DprMasterdatas;
            yamlAssetPaths­[Path.Combine("md", "characterinfo")]                      = PathEnum.DprMasterdatas;
            //yamlAssetPaths­[Path.Combine("md", "common")]                             = PathEnum.DprMasterdatas;
            //yamlAssetPaths­[Path.Combine("md", "honeytree")]                          = PathEnum.DprMasterdatas;
            //yamlAssetPaths­[Path.Combine("md", "kinomidata")]                         = PathEnum.DprMasterdatas;
            //yamlAssetPaths­[Path.Combine("md", "localkoukan")]                        = PathEnum.DprMasterdatas;
            //yamlAssetPaths­[Path.Combine("md", "mapwarpdata")]                        = PathEnum.DprMasterdatas;
            //yamlAssetPaths­[Path.Combine("md", "msgwindowdata")]                      = PathEnum.DprMasterdatas;
            //yamlAssetPaths­[Path.Combine("md", "network")]                            = PathEnum.DprMasterdatas;
            //yamlAssetPaths­[Path.Combine("md", "placedata")]                          = PathEnum.DprMasterdatas;
            //yamlAssetPaths­[Path.Combine("md", "placetagdata")]                       = PathEnum.DprMasterdatas;
            yamlAssetPaths­[Path.Combine("md", "pokemondata")]                        = PathEnum.DprMasterdatas;
            yamlAssetPaths­[Path.Combine("md", "shopdata")]                           = PathEnum.DprMasterdatas;
            //yamlAssetPaths­[Path.Combine("md", "stopdata")]                           = PathEnum.DprMasterdatas;
            yamlAssetPaths­[Path.Combine("md", "tower")]                              = PathEnum.DprMasterdatas;
            //yamlAssetPaths­[Path.Combine("md", "tv")]                                 = PathEnum.DprMasterdatas;
            //yamlAssetPaths­[Path.Combine("md", "underground")]                        = PathEnum.DprMasterdatas;
            //yamlAssetPaths­["scriptableobjects"]                                      = PathEnum.Gamesettings;
            yamlAssetPaths­[Path.Combine("scriptableobjects", "fieldencount")]        = PathEnum.Gamesettings;
            yamlAssetPaths­[Path.Combine("format_msbt", "en", "english")]             = PathEnum.English;
            yamlAssetPaths­[Path.Combine("format_msbt", "fr", "french")]              = PathEnum.French;
            yamlAssetPaths­[Path.Combine("format_msbt", "ge", "german")]              = PathEnum.German;
            yamlAssetPaths­[Path.Combine("format_msbt", "it", "italian")]             = PathEnum.Italian;
            yamlAssetPaths­[Path.Combine("format_msbt", "jp", "jpn")]                 = PathEnum.Jpn;
            yamlAssetPaths­[Path.Combine("format_msbt", "jp", "jpn_kanji")]           = PathEnum.JpnKanji;
            yamlAssetPaths­[Path.Combine("format_msbt", "ko", "korean")]              = PathEnum.Korean;
            yamlAssetPaths­[Path.Combine("format_msbt", "si", "simp_chinese")]        = PathEnum.SimpChinese;
            yamlAssetPaths­[Path.Combine("format_msbt", "sp", "spanish")]             = PathEnum.Spanish;
            yamlAssetPaths­[Path.Combine("format_msbt", "tr", "trad_chinese")]        = PathEnum.TradChinese;
            yamlAssetPaths­[Path.Combine("pml", "data")]                              = PathEnum.PersonalMasterdatas;
            yamlAssetPaths­["masterdatas"]                                            = PathEnum.UIMasterdatas;
            //yamlAssetPaths­[Path.Combine("ugassets", "datatable")]                    = PathEnum.Ugdata;
            //yamlAssetPaths­[Path.Combine("ugassets", "datatable", "ug_encounttable")] = PathEnum.Ugdata;

            yamlAssetPaths­[Path.Combine("ExtraData", "Encounters", "Starter")]       = PathEnum.ExternalJSON;
            yamlAssetPaths­[Path.Combine("ExtraData", "Encounters", "HoneyTrees")]    = PathEnum.ExternalJSON;
            yamlAssetPaths­[Path.Combine("ExtraData", "MonData", "TMLearnset")]       = PathEnum.ExternalJSON;

            DataColumn[] columns = { new DataColumn("Value", typeof(string)), new DataColumn("Minimum", typeof(int)), new DataColumn("Maximum", typeof(int)), new DataColumn("Increment", typeof(int)) };
            absoluteBoundaries.Columns.AddRange(columns);
            columns[0].ReadOnly = true;
            absoluteBoundaries.Rows.Add(new Object[] { "Level", 1, 100, 1 });
            absoluteBoundaries.Rows.Add(new Object[] { "Base Stat", 1, 255, 1 });
            absoluteBoundaries.Rows.Add(new Object[] { "Catch Rate", 1, 255, 1 });
            absoluteBoundaries.Rows.Add(new Object[] { "EV Yield", 0, 3, 1 });
            absoluteBoundaries.Rows.Add(new Object[] { "EV Yield Total", 1, 3, 1 });
            absoluteBoundaries.Rows.Add(new Object[] { "Initial Friendship", 0, 250, 10 });
            absoluteBoundaries.Rows.Add(new Object[] { "EXP Yield", 1, 65535, 1 });
            absoluteBoundaries.Rows.Add(new Object[] { "Level Up Move Count", 1, 255, 1 });
            absoluteBoundaries.Rows.Add(new Object[] { "Egg Move Count", 0, 255, 1 });
            absoluteBoundaries.Rows.Add(new Object[] { "Power", 5, 255, 5 });
            absoluteBoundaries.Rows.Add(new Object[] { "Accuracy", 5, 100, 5 });
            absoluteBoundaries.Rows.Add(new Object[] { "PP", 5, 40, 5 });
            absoluteBoundaries.Rows.Add(new Object[] { "Trainer Pokémon Count", 1, 6, 1 });
            absoluteBoundaries.Rows.Add(new Object[] { "IV", 0, 31, 1 });
            absoluteBoundaries.Rows.Add(new Object[] { "EV", 0, 255, 1 });
            absoluteBoundaries.Rows.Add(new Object[] { "EV Total", 0, 510, 1 });
            absoluteBoundaries.Rows.Add(new Object[] { "Price", 10, 999990, 10 });
        }
    }
}
