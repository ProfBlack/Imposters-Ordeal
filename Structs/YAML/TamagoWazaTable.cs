using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    public class TamagoWazaTable : MonoBehaviour
    {
        [YamlMember("Data", 10)]
        public SheetTamagoWazaData[] Data { get; set; }
    }

    public class SheetTamagoWazaData
    {
        [YamlMember("no", 0)]
        public ushort No { get; set; }

        [YamlMember("formNo", 1)]
        public ushort FormNo { get; set; }

        [YamlMember("wazaNo", 2)]
        public ushort[] WazaNo { get; set; }
    }
}
