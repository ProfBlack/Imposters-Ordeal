using System;
using System.ComponentModel;
using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    public class UnityFile
    {
        [YamlMember("fileID")]
        public long FileID { get; set; }

        [YamlMember("guid")]
        [DefaultValue("")]
        public string GUID { get; set; }

        [YamlMember("type")]
        [DefaultValue(0)]
        public int Type { get; set; }
    }
}
