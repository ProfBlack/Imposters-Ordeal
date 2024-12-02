using SharpYaml;
using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    [YamlStyle(YamlStyle.Flow)]
    public struct UnityVector3
    {
        [YamlMember("x", 0)]
        public float X { get; set; }

        [YamlMember("y", 1)]
        public float Y { get; set; }

        [YamlMember("z", 2)]
        public float Z { get; set; }
    }
}
