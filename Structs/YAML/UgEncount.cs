using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    public class UgEncount : MonoBehaviour
    {
        [YamlMember("table", 10)]
        public SheetUgEncountTable[] Table { get; set; }
    }

    public class SheetUgEncountTable
    {
        [YamlMember("monsno", 0)]
        public int Monsno { get; set; }

        [YamlMember("version", 1)]
        public int Version { get; set; }

        [YamlMember("zukanflag", 2)]
        public int ZukanFlag { get; set; }
    }
}
