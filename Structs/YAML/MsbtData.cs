using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    public class MsbtData : MonoBehaviour
    {
        [YamlMember("hash", 10)]
        public int Hash { get; set; }

        [YamlMember("langID", 11)]
        public int LangID { get; set; }

        [YamlMember("isResident", 12)]
        public byte IsResident { get; set; }

        [YamlMember("isKanji", 13)]
        public byte IsKanji { get; set; }

        [YamlMember("labelDataArray", 14)]
        public LabelData[] LabelDataArray { get; set; }
    }

    public class LabelData
    {
        [YamlMember("labelIndex", 0)]
        public int LabelIndex { get; set; }

        [YamlMember("arrayIndex", 1)]
        public int ArrayIndex { get; set; }

        [YamlMember("labelName", 2)]
        public string LabelName { get; set; }

        [YamlMember("styleInfo", 3)]
        public StyleInfo StyleInfo { get; set; }

        [YamlMember("attributeValueArray", 4)]
        public int[] AttributeValueArray { get; set; }

        [YamlMember("tagDataArray", 5)]
        public TagData[] TagDataArray { get; set; }

        [YamlMember("wordDataArray", 6)]
        public WordData[] WordDataArray { get; set; }
    }

    public class StyleInfo
    {
        [YamlMember("styleIndex", 0)]
        public int StyleIndex { get; set; }

        [YamlMember("colorIndex", 1)]
        public int ColorIndex { get; set; }

        [YamlMember("fontSize", 2)]
        public int FontSize { get; set; }

        [YamlMember("maxWidth", 3)]
        public int MaxWidth { get; set; }

        [YamlMember("controlID", 4)]
        public int ControlID { get; set; }
    }

    public class TagData
    {
        [YamlMember("tagIndex", 0)]
        public int TagIndex { get; set; }

        [YamlMember("groupID", 1)]
        public int GroupID { get; set; }

        [YamlMember("tagID", 2)]
        public int TagID { get; set; }

        [YamlMember("tagPatternID", 3)]
        public int TagPatternID { get; set; }

        [YamlMember("forceArticle", 4)]
        public int ForceArticle { get; set; }

        [YamlMember("tagParameter", 5)]
        public int TagParameter { get; set; }

        [YamlMember("tagWordArray", 6)]
        public string[] TagWordArray { get; set; }

        [YamlMember("forceGrmID", 7)]
        public int ForceGrmID { get; set; }
    }

    public class WordData
    {
        [YamlMember("patternID", 0)]
        public int PatternID { get; set; }

        [YamlMember("eventID", 1)]
        public int EventID { get; set; }

        [YamlMember("tagIndex", 2)]
        public int TagIndex { get; set; }

        [YamlMember("tagValue", 3)]
        public float TagValue { get; set; }

        [YamlMember("str", 4)]
        public string Str { get; set; }

        [YamlMember("strWidth", 5)]
        public float StrWidth { get; set; }
    }
}
