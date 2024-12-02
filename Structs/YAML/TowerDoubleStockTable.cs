using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    public class TowerDoubleStockTable : MonoBehaviour
    {
        [YamlMember("TowerDoubleStock", 10)]
        public SheetTowerDoubleStock[] TowerDoubleStock { get; set; }
    }

    public class SheetTowerDoubleStock
    {
        [YamlMember("ID", 0)]
        public uint ID { get; set; }

        [YamlMember("TrainerID", 1)]
        public int[] TrainerID { get; set; }

        [YamlMember("PokeID", 2)]
        public uint[] PokeID { get; set; }

        [YamlMember("BattleBGM", 3)]
        public string BattleBGM { get; set; }

        [YamlMember("WinBGM", 4)]
        public string WinBGM { get; set; }
    }
}
