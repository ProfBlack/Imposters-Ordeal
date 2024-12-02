using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    public class UgRandMark : MonoBehaviour
    {
        [YamlMember("table", 10)]
        public SheetUgRandTable[] Table { get; set; }
    }

    public class SheetUgRandTable
    {
        [YamlMember("id", 0)]
        public int ID { get; set; }

        [YamlMember("FileName", 1)]
        public string FileName { get; set; }

        [YamlMember("size", 2)]
        public int Size { get; set; }

        [YamlMember("min", 3)]
        public int Min { get; set; }

        [YamlMember("max", 4)]
        public int Max { get; set; }

        [YamlMember("smax", 5)]
        public int SMax { get; set; }

        [YamlMember("mmax", 6)]
        public int MMax { get; set; }

        [YamlMember("lmax", 7)]
        public int LMax { get; set; }

        [YamlMember("llmax", 8)]
        public int LLMax { get; set; }

        [YamlMember("watermax", 9)]
        public int WaterMax { get; set; }

        [YamlMember("typerate", 10)]
        public int[] TypeRate { get; set; }
    }
}
