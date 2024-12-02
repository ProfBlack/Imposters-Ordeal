using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    public class AddPersonalTable : MonoBehaviour
    {
        [YamlMember("AddPersonal", 10)]
        public SheetAddPersonal[] AddPersonal { get; set; }
    }

    public class SheetAddPersonal
    {
        [YamlMember("valid_flag", 0)]
        public byte ValidFlag { get; set; }

        [YamlMember("monsno", 1)]
        public ushort Monsno { get; set; }

        [YamlMember("formno", 2)]
        public ushort Formno { get; set; }

        [YamlMember("isEnableSynchronize", 3)]
        public byte IsEnableSynchronize { get; set; }

        [YamlMember("escape", 4)]
        public byte Escape { get; set; }

        [YamlMember("isDisableReverce", 5)]
        public byte IsDisableReverce { get; set; }
    }
}
