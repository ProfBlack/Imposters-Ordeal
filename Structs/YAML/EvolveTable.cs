using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    public class EvolveTable : MonoBehaviour
    {
        [YamlMember("Evolve", 10)]
        public SheetEvolve[] Evolve { get; set; }
    }

    public class SheetEvolve
    {
        [YamlMember("id", 0)]
        public int ID { get; set; }

        [YamlMember("ar", 1)]
        public ushort[] Ar { get; set; }
    }
}
