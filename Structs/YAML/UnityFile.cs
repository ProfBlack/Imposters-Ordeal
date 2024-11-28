using System;
using System.ComponentModel;
using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    public class UnityFile
    {
        [YamlMember("fileID", 0)]
        public long FileID { get; set; }

        [YamlMember("guid", 1)]
        [DefaultValue(0)]
        public Guid GUID { get; set; }

        [YamlMember("type", 2)]
        [DefaultValue(0)]
        public int Type { get; set; }
    }
}
