using SharpYaml;
using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    [YamlStyle(YamlStyle.Flow)]
    public struct UnityVector2
    {
        [YamlMember("x", 0)]
        public float X { get; set; }

        [YamlMember("y", 1)]
        public float Y { get; set; }
    }
}
