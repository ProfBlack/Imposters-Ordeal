using YamlDotNet.Serialization;

namespace ImpostersOrdeal
{
    public class GameSettings : MonoBehaviour
    {
        [YamlMember(Alias = "mapInfo", ApplyNamingConventions = false)]
        public UnityFile MapInfo { get; set; }

        [YamlMember(Alias = "arenaInfo", ApplyNamingConventions = false)]
        public UnityFile ArenaInfo { get; set; }

        [YamlMember(Alias = "mapAttributeTable", ApplyNamingConventions = false)]
        public UnityFile MapAttributeTable { get; set; }

        [YamlMember(Alias = "mapAttributeExTable", ApplyNamingConventions = false)]
        public UnityFile MapAttributeExTable { get; set; }

        [YamlMember(Alias = "calenderEncTable", ApplyNamingConventions = false)]
        public UnityFile CalenderEncTable { get; set; }

        [YamlMember(Alias = "fieldEncountTable_d", ApplyNamingConventions = false)]
        public UnityFile FieldEncountTableD { get; set; }

        [YamlMember(Alias = "fieldEncountTable_p", ApplyNamingConventions = false)]
        public UnityFile FieldEncountTableP { get; set; }

        [YamlMember(Alias = "waterSettings", ApplyNamingConventions = false)]
        public UnityFile WaterSettings { get; set; }
    }
}
