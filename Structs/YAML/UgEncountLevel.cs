using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    public class UgEncountLevel : MonoBehaviour
    {
        [YamlMember("Data", 10)]
        public SheetUgEncountLevelData[] Data { get; set; }
    }

    public class SheetUgEncountLevelData
    {
        [YamlMember("MinLv", 0)]
        public int MinLv { get; set; }

        [YamlMember("MaxLv", 1)]
        public int MaxLv { get; set; }
    }
}
