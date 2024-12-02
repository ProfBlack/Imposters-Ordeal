using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    public class UgSpecialPokemon : MonoBehaviour
    {
        [YamlMember("Sheet1", 10)]
        public SheetUgSpecialPokemonSheet1[] Sheet1 { get; set; }
    }

    public class SheetUgSpecialPokemonSheet1
    {
        [YamlMember("id", 0)]
        public int ID { get; set; }

        [YamlMember("monsno", 1)]
        public int Monsno { get; set; }

        [YamlMember("version", 2)]
        public int Version { get; set; }

        [YamlMember("Dspecialrate", 3)]
        public int DSpecialRate { get; set; }

        [YamlMember("Pspecialrate", 4)]
        public int PSpecialRate { get; set; }
    }
}
