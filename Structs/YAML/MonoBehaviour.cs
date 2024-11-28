using SharpYaml;
using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    public abstract class MonoBehaviour
    {
        [YamlMember("m_ObjectHideFlags")]
        public int ObjectHideFlags { get; set; }

        [YamlMember("m_CorrespondingSourceObject")]
        [YamlStyle(YamlStyle.Flow)]
        public UnityFile CorrespondingSourceObject { get; set; }

        [YamlMember("m_PrefabInstance")]
        [YamlStyle(YamlStyle.Flow)]
        public UnityFile PrefabInstance { get; set; }

        [YamlMember("m_PrefabAsset")]
        [YamlStyle(YamlStyle.Flow)]
        public UnityFile PrefabAsset { get; set; }

        [YamlMember("m_GameObject")]
        [YamlStyle(YamlStyle.Flow)]
        public UnityFile GameObject { get; set; }

        [YamlMember("m_Enabled")]
        public int Enabled { get; set; }

        [YamlMember("m_EditorHideFlags")]
        public int EditorHideFlags { get; set; }

        [YamlMember("m_Script")]
        [YamlStyle(YamlStyle.Flow)]
        public UnityFile Script { get; set; }

        [YamlMember("m_Name")]
        public string Name { get; set; }

        [YamlMember("m_EditorClassIdentifier")]
        public string EditorClassIdentifier { get; set; }
    }

    public class YamlMonoContainer
    {
        [YamlMember("MonoBehaviour")]
        public MonoBehaviour MonoBehaviour { get; set; }
    }
}
