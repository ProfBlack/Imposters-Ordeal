using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    public struct UnityColor
    {
        [YamlMember("r", 0)]
        public float R { get; set; }

        [YamlMember("g", 1)]
        public float G { get; set; }

        [YamlMember("b", 2)]
        public float B { get; set; }

        [YamlMember("a", 3)]
        public float A { get; set; }
    }
}
