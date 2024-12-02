using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    public class MonohiroiTable : MonoBehaviour
    {
        [YamlMember("MonoHiroi", 10)]
        public SheetMonoHiroi[] MonoHiroi { get; set; }
    }

    public class SheetMonoHiroi
    {
        [YamlMember("ID", 0)]
        public ushort ID { get; set; }

        [YamlMember("Ratios", 1)]
        public byte[] Ratios { get; set; }
    }
}
