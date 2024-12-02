using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    public struct UnityColor32
    {
        [YamlMember("serializedVersion", 0)]
        public int SerializedVersion { get; set; }

        [YamlMember("rgba", 1)]
        public uint RGBA { get; set; }
    }
}
