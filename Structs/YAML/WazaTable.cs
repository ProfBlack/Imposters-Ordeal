using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    public class WazaTable : MonoBehaviour
    {
        [YamlMember("Waza", 10)]
        public SheetWaza[] Waza { get; set; }

        [YamlMember("Yubiwohuru", 11)]
        public SheetYubiwohuru[] Yubiwohuru { get; set; }
    }

    public class SheetWaza
    {
        [YamlMember("wazaNo", 0)]
        public int WazaNo { get; set; }

        [YamlMember("isValid", 1)]
        public byte IsValid { get; set; }

        [YamlMember("type", 2)]
        public byte Type { get; set; }

        [YamlMember("category", 3)]
        public byte Category { get; set; }

        [YamlMember("damageType", 4)]
        public byte DamageType { get; set; }

        [YamlMember("power", 5)]
        public byte Power { get; set; }

        [YamlMember("hitPer", 6)]
        public byte HitPer { get; set; }

        [YamlMember("basePP", 7)]
        public byte BasePP { get; set; }

        [YamlMember("priority", 8)]
        public sbyte Priority { get; set; }

        [YamlMember("hitCountMax", 9)]
        public byte HitCountMax { get; set; }

        [YamlMember("hitCountMin", 10)]
        public byte HitCountMin { get; set; }

        [YamlMember("sickID", 11)]
        public ushort SickID { get; set; }

        [YamlMember("sickPer", 12)]
        public byte SickPer { get; set; }

        [YamlMember("sickCont", 13)]
        public byte SickCont { get; set; }

        [YamlMember("sickTurnMin", 14)]
        public byte SickTurnMin { get; set; }

        [YamlMember("sickTurnMax", 15)]
        public byte SickTurnMax { get; set; }

        [YamlMember("criticalRank", 16)]
        public byte CriticalRank { get; set; }

        [YamlMember("shrinkPer", 17)]
        public byte ShrinkPer { get; set; }

        [YamlMember("aiSeqNo", 18)]
        public ushort AISeqNo { get; set; }

        [YamlMember("damageRecoverRatio", 19)]
        public sbyte DamageRecoverRatio { get; set; }

        [YamlMember("hpRecoverRatio", 20)]
        public sbyte HPRecoverRatio { get; set; }

        [YamlMember("target", 21)]
        public byte Target { get; set; }

        [YamlMember("rankEffType1", 22)]
        public byte RankEffType1 { get; set; }

        [YamlMember("rankEffType2", 23)]
        public byte RankEffType2 { get; set; }

        [YamlMember("rankEffType3", 24)]
        public byte RankEffType3 { get; set; }

        [YamlMember("rankEffValue1", 25)]
        public sbyte RankEffValue1 { get; set; }

        [YamlMember("rankEffValue2", 26)]
        public sbyte RankEffValue2 { get; set; }

        [YamlMember("rankEffValue3", 27)]
        public sbyte RankEffValue3 { get; set; }

        [YamlMember("rankEffPer1", 28)]
        public byte RankEffPer1 { get; set; }

        [YamlMember("rankEffPer2", 29)]
        public byte RankEffPer2 { get; set; }

        [YamlMember("rankEffPer3", 30)]
        public byte RankEffPer3 { get; set; }

        [YamlMember("flags", 31)]
        public uint Flags { get; set; }

        [YamlMember("contestWazaNo", 32)]
        public uint ContestWazaNo { get; set; }
    }

    public class SheetYubiwohuru
    {
        [YamlMember("wazaNos", 0)]
        public ushort[] WazaNos { get; set; }
    }
}
