using YamlDotNet.Serialization;

namespace ImpostersOrdeal
{
    public abstract class MonoBehaviour
    {
        [YamlMember(Alias = "m_ObjectHideFlags", ApplyNamingConventions = false)]
        public int ObjectHideFlags { get; set; }

        [YamlMember(Alias = "m_CorrespondingSourceObject", ApplyNamingConventions = false)]
        public UnityFile CorrespondingSourceObject { get; set; }

        [YamlMember(Alias = "m_PrefabInstance", ApplyNamingConventions = false)]
        public UnityFile PrefabInstance { get; set; }

        [YamlMember(Alias = "m_PrefabAsset", ApplyNamingConventions = false)]
        public UnityFile PrefabAsset { get; set; }

        [YamlMember(Alias = "m_GameObject", ApplyNamingConventions = false)]
        public UnityFile GameObject { get; set; }

        [YamlMember(Alias = "m_Enabled", ApplyNamingConventions = false)]
        public int Enabled { get; set; }

        [YamlMember(Alias = "m_EditorHideFlags", ApplyNamingConventions = false)]
        public int EditorHideFlags { get; set; }

        [YamlMember(Alias = "m_Script", ApplyNamingConventions = false)]
        public UnityFile Script { get; set; }

        [YamlMember(Alias = "m_Name", ApplyNamingConventions = false)]
        public string Name { get; set; }

        [YamlMember(Alias = "m_EditorClassIdentifier", ApplyNamingConventions = false)]
        public string EditorClassIdentifier { get; set; }
    }
}
