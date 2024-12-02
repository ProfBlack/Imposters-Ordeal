using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    public class UgPokemonData : MonoBehaviour
    {
        [YamlMember("table", 10)]
        public SheetUgPokemonDataTable[] Table { get; set; }
    }

    public class SheetUgPokemonDataTable
    {
        [YamlMember("monsno", 0)]
        public int Monsno { get; set; }

        [YamlMember("type1ID", 1)]
        public int Type1ID { get; set; }

        [YamlMember("type2ID", 2)]
        public int Type2ID { get; set; }

        [YamlMember("size", 3)]
        public int Size { get; set; }

        [YamlMember("movetype", 4)]
        public int MoveType { get; set; }

        [YamlMember("reactioncode", 5)]
        public int[] ReactionCode { get; set; }

        [YamlMember("move_rate", 6)]
        public int[] MoveRate { get; set; }

        [YamlMember("submove_rate", 7)]
        public int[] SubmoveRate { get; set; }

        [YamlMember("reaction", 8)]
        public int[] Reaction { get; set; }

        [YamlMember("flagrate", 9)]
        public int[] FlagRate { get; set; }

        [YamlMember("rateup", 10)]
        public int RateUp { get; set; }
    }
}
