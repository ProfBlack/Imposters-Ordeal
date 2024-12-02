using SharpYaml;
using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    [YamlStyle(YamlStyle.Flow)]
    public struct UnityVector2Int
    {
        [YamlMember("x", 0)]
        public int X { get; set; }

        [YamlMember("y", 1)]
        public int Y { get; set; }
    }
}
