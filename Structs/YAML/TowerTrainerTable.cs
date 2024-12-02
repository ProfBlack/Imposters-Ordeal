using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    public class TowerTrainerTable : MonoBehaviour
    {
        [YamlMember("TrainerData", 10)]
        public SheetTowerTrainerData[] TrainerData { get; set; }

        [YamlMember("TrainerPoke", 11)]
        public SheetTowerTrainerPoke[] TrainerPoke { get; set; }
    }

    public class SheetTowerTrainerData
    {
        [YamlMember("TrainerType", 0)]
        public int TrainerType { get; set; }

        [YamlMember("NameLabel", 1)]
        public string NameLabel { get; set; }

        [YamlMember("MsgFieldBefore", 2)]
        public string MsgFieldBefore { get; set; }

        [YamlMember("MsgBattle", 3)]
        public string[] MsgBattle { get; set; }

        [YamlMember("SeqBattle", 4)]
        public string[] SeqBattle { get; set; }

        [YamlMember("ColorID", 5)]
        public byte ColorID { get; set; }

        [YamlMember("fldGraphic", 6)]
        public int FldGraphic { get; set; }

        [YamlMember("singleEnc", 7)]
        public string SingleEnc { get; set; }

        [YamlMember("doubleEnc", 8)]
        public string DoubleEnc { get; set; }
    }

    public class SheetTowerTrainerPoke
    {
        [YamlMember("ID", 0)]
        public uint ID { get; set; }

        [YamlMember("MonsNo", 1)]
        public int MonsNo { get; set; }

        [YamlMember("FormNo", 2)]
        public ushort FormNo { get; set; }

        [YamlMember("IsRare", 3)]
        public byte IsRare { get; set; }

        [YamlMember("Level", 4)]
        public byte Level { get; set; }

        [YamlMember("Sex", 5)]
        public byte Sex { get; set; }

        [YamlMember("Seikaku", 6)]
        public int Seikaku { get; set; }

        [YamlMember("Tokusei", 7)]
        public int Tokusei { get; set; }

        [YamlMember("Waza1", 8)]
        public int Waza1 { get; set; }

        [YamlMember("Waza2", 9)]
        public int Waza2 { get; set; }

        [YamlMember("Waza3", 10)]
        public int Waza3 { get; set; }

        [YamlMember("Waza4", 11)]
        public int Waza4 { get; set; }

        [YamlMember("Item", 12)]
        public ushort Item { get; set; }

        [YamlMember("Ball", 13)]
        public byte Ball { get; set; }

        [YamlMember("Seal", 14)]
        public int Seal { get; set; }

        [YamlMember("TalentHp", 15)]
        public byte TalentHP { get; set; }

        [YamlMember("TalentAtk", 16)]
        public byte TalentAtk { get; set; }

        [YamlMember("TalentDef", 17)]
        public byte TalentDef { get; set; }

        [YamlMember("TalentSpAtk", 18)]
        public byte TalentSpAtk { get; set; }

        [YamlMember("TalentSpDef", 19)]
        public byte TalentSpDef { get; set; }

        [YamlMember("TalentAgi", 20)]
        public byte TalentAgi { get; set; }

        [YamlMember("EffortHp", 21)]
        public byte EffortHP { get; set; }

        [YamlMember("EffortAtk", 22)]
        public byte EffortAtk { get; set; }

        [YamlMember("EffortDef", 23)]
        public byte EffortDef { get; set; }

        [YamlMember("EffortSpAtk", 24)]
        public byte EffortSpAtk { get; set; }

        [YamlMember("EffortSpDef", 25)]
        public byte EffortSpDef { get; set; }

        [YamlMember("EffortAgi", 26)]
        public byte EffortAgi { get; set; }
    }
}
