using SharpYaml;
using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    public abstract class MonoBehaviour
    {
        [YamlMember("m_ObjectHideFlags", 0)]
        public int ObjectHideFlags { get; set; }

        [YamlMember("m_CorrespondingSourceObject", 1)]
        [YamlStyle(YamlStyle.Flow)]
        public UnityFile CorrespondingSourceObject { get; set; }

        [YamlMember("m_PrefabInstance", 2)]
        [YamlStyle(YamlStyle.Flow)]
        public UnityFile PrefabInstance { get; set; }

        [YamlMember("m_PrefabAsset", 3)]
        [YamlStyle(YamlStyle.Flow)]
        public UnityFile PrefabAsset { get; set; }

        [YamlMember("m_GameObject", 4)]
        [YamlStyle(YamlStyle.Flow)]
        public UnityFile GameObject { get; set; }

        [YamlMember("m_Enabled", 5)]
        public int Enabled { get; set; }

        [YamlMember("m_EditorHideFlags", 6)]
        public int EditorHideFlags { get; set; }

        [YamlMember("m_Script", 7)]
        [YamlStyle(YamlStyle.Flow)]
        public UnityFile Script { get; set; }

        [YamlMember("m_Name", 8)]
        public string Name { get; set; }

        [YamlMember("m_EditorClassIdentifier", 9)]
        public string EditorClassIdentifier { get; set; }
    }

    public class YamlMonoContainer
    {
        [YamlMember("MonoBehaviour")]
        public MonoBehaviour MonoBehaviour { get; set; }
    }
}
