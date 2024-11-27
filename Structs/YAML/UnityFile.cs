using System;
using System.ComponentModel;
using YamlDotNet.Serialization;

namespace ImpostersOrdeal
{
    public class UnityFile
    {
        [YamlMember(Alias = "fileID", ApplyNamingConventions = false)]
        public long FileID { get; set; }

        [YamlMember(Alias = "guid", ApplyNamingConventions = false)]
        [DefaultValue(0)]
        public Guid GUID { get; set; }

        [YamlMember(Alias = "type", ApplyNamingConventions = false)]
        [DefaultValue(0)]
        public int Type { get; set; }
    }
}
