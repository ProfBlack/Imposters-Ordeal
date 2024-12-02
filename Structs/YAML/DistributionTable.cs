using SharpYaml;
using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    public class DistributionTable : MonoBehaviour
    {
        [YamlMember("MapDistribution", 10)]
        public SheetMapDistribution[] MapDistribution { get; set; }

        [YamlMember("Diamond_FieldTable", 11)]
        public SheetDistributionTable[] DiamondFieldTable { get; set; }

        [YamlMember("Diamond_DungeonTable", 12)]
        public SheetDistributionTable[] DiamondDungeonTable { get; set; }

        [YamlMember("Pearl_FieldTable", 13)]
        public SheetDistributionTable[] PearlFieldTable { get; set; }

        [YamlMember("Pearl_DungeonTable", 14)]
        public SheetDistributionTable[] PearlDungeonTable { get; set; }
    }

    public class SheetMapDistribution
    {
        [YamlMember("MapID", 0)]
        public int MapID { get; set; }

        [YamlMember("LightUpGridXZ", 1)]
        public UnityVector2Int[] LightUpGridXZ { get; set; }

        [YamlMember("DistributionHideFlag", 2)]
        public byte DistributionHideFlag { get; set; }
    }

    public class SheetDistributionTable
    {
        [YamlMember("BeforeMorning", 0)]
        public int[] BeforeMorning { get; set; }

        [YamlMember("BeforeDaytime", 1)]
        public int[] BeforeDaytime { get; set; }

        [YamlMember("BeforeNight", 2)]
        public int[] BeforeNight { get; set; }

        [YamlMember("AfterMorning", 3)]
        public int[] AfterMorning { get; set; }

        [YamlMember("AfterDaytime", 4)]
        public int[] AfterDaytime { get; set; }

        [YamlMember("AfterNight", 5)]
        public int[] AfterNight { get; set; }

        [YamlMember("Fishing", 6)]
        public int[] Fishing { get; set; }

        [YamlMember("PokemonTraser", 7)]
        public int[] PokemonTraser { get; set; }

        [YamlMember("HoneyTree", 8)]
        public int[] HoneyTree { get; set; }
    }
}
