using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    public class TowerSingleStockTable : MonoBehaviour
    {
        [YamlMember("TowerSingleStock", 10)]
        public SheetTowerSingleStock[] TowerSingleStock { get; set; }
    }

    public class SheetTowerSingleStock
    {
        [YamlMember("ID", 0)]
        public uint ID { get; set; }

        [YamlMember("TrainerID", 1)]
        public int TrainerID { get; set; }

        [YamlMember("PokeID", 2)]
        public uint[] PokeID { get; set; }

        [YamlMember("BattleBGM", 3)]
        public string BattleBGM { get; set; }

        [YamlMember("WinBGM", 4)]
        public string WinBGM { get; set; }
    }
}
