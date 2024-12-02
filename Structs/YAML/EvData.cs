using SharpYaml.Serialization;
using System.Collections.Generic;

namespace ImpostersOrdeal
{
    public class EvData : MonoBehaviour
    {
        [YamlMember("Scripts", 10)]
        public List<Script> Scripts { get; set; }

        [YamlMember("StrList", 11)]
        public List<string> StrList { get; set; }
    }

    public struct Aregment
    {
        [YamlMember("argType", 0)]
        public int ArgType { get; set; }

        [YamlMember("data", 1)]
        public int Data { get; set; }
    }

    public class Script
    {
        [YamlMember("Label", 0)]
        public string Label { get; set; }

        [YamlMember("Commands", 1)]
        public List<Command> Commands { get; set; }
    }

    public class Command
    {
        [YamlMember("Arg", 0)]
        public List<Aregment> Arg { get; set; }
    }
}
