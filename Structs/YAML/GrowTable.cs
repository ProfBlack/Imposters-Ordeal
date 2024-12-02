using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    public class GrowTable : MonoBehaviour
    {
        [YamlMember("Data", 10)]
        public SheetData[] Data { get; set; }
    }

    public class SheetData
    {
        [YamlMember("exps", 0)]
        public uint[] Exps { get; set; }
    }
}
