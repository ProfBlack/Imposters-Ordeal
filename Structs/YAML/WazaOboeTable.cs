using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    public class WazaOboeTable : MonoBehaviour
    {
        [YamlMember("WazaOboe", 10)]
        public SheetWazaOboe[] WazaOboe { get; set; }
    }

    public class SheetWazaOboe
    {
        [YamlMember("id", 0)]
        public int ID { get; set; }

        [YamlMember("ar", 1)]
        public ushort[] Ar { get; set; }
    }
}
