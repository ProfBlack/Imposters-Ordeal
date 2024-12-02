using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    public class ContestConfigDatas : MonoBehaviour
    {
        [YamlMember("TapScoreData", 10)]
        public SheetTapScoreData[] TapScoreData { get; set; }

        [YamlMember("TapTimingData", 11)]
        public SheetTapTimingData[] TapTimingData { get; set; }

        [YamlMember("ComboBonusData", 12)]
        public SheetComboBonusData[] ComboBonusData { get; set; }

        [YamlMember("MatchingSkill", 13)]
        public SheetMatchingSkill[] MatchingSkill { get; set; }

        [YamlMember("LotteryEntryRank", 14)]
        public SheetLotteryEntryRank[] LotteryEntryRank { get; set; }

        [YamlMember("ResultCameraTween", 15)]
        public SheetResultCameraTween[] ResultCameraTween { get; set; }

        [YamlMember("ResultMotion", 16)]
        public SheetResultMotion[] ResultMotion { get; set; }

        [YamlMember("ValueData", 17)]
        public SheetValueData[] ValueData { get; set; }

        [YamlMember("StringData", 18)]
        public SheetStringData[] StringData { get; set; }
    }

    public class SheetTapScoreData
    {
        [YamlMember("valid_flag", 0)]
        public byte ValidFlag { get; set; }

        [YamlMember("id", 1)]
        public ushort ID { get; set; }

        [YamlMember("tension", 2)]
        public int Tension { get; set; }

        [YamlMember("best", 3)]
        public int Best { get; set; }

        [YamlMember("great", 4)]
        public int Great { get; set; }

        [YamlMember("nice", 5)]
        public int Nice { get; set; }

        [YamlMember("fast", 6)]
        public int Fast { get; set; }

        [YamlMember("slow", 7)]
        public int Slow { get; set; }

        [YamlMember("upCount", 8)]
        public int UpCount { get; set; }

        [YamlMember("downCount", 9)]
        public int DownCount { get; set; }
    }

    public class SheetTapTimingData
    {
        [YamlMember("valid_flag", 0)]
        public byte ValidFlag { get; set; }

        [YamlMember("id", 1)]
        public ushort ID { get; set; }

        [YamlMember("pattern", 2)]
        public int Pattern { get; set; }

        [YamlMember("bestRange", 3)]
        public float BestRange { get; set; }

        [YamlMember("greatRange", 4)]
        public float GreatRange { get; set; }

        [YamlMember("niceRange", 5)]
        public float NiceRange { get; set; }

        [YamlMember("fastRange", 6)]
        public float FastRange { get; set; }
    }

    public class SheetComboBonusData
    {
        [YamlMember("valid_flag", 0)]
        public byte ValidFlag { get; set; }

        [YamlMember("id", 1)]
        public ushort ID { get; set; }

        [YamlMember("chainCount", 2)]
        public int ChainCount { get; set; }

        [YamlMember("basePoint", 3)]
        public int BasePoint { get; set; }

        [YamlMember("bonusPoint", 4)]
        public int BonusPoint { get; set; }
    }

    public class SheetMatchingSkill
    {
        [YamlMember("valid_flag", 0)]
        public byte ValidFlag { get; set; }

        [YamlMember("id", 1)]
        public ushort ID { get; set; }

        [YamlMember("skillPoint", 2)]
        public int SkillPoint { get; set; }
    }

    public class SheetLotteryEntryRank
    {
        [YamlMember("valid_flag", 0)]
        public byte ValidFlag { get; set; }

        [YamlMember("id", 1)]
        public ushort ID { get; set; }

        [YamlMember("aveSkillPoint", 2)]
        public float AveSkillPoint { get; set; }

        [YamlMember("contestRank", 3)]
        public int ContestRank { get; set; }
    }

    public class SheetResultCameraTween
    {
        [YamlMember("valid_flag", 0)]
        public byte ValidFlag { get; set; }

        [YamlMember("id", 1)]
        public ushort ID { get; set; }

        [YamlMember("entryNumber", 2)]
        public int EntryNumber { get; set; }

        [YamlMember("duration", 3)]
        public float Duration { get; set; }

        [YamlMember("size", 4)]
        public int Size { get; set; }

        [YamlMember("posX", 5)]
        public float PosX { get; set; }

        [YamlMember("posY", 6)]
        public float PosY { get; set; }

        [YamlMember("posZ", 7)]
        public float PosZ { get; set; }

        [YamlMember("angleX", 8)]
        public float AngleX { get; set; }

        [YamlMember("angleY", 9)]
        public float AngleY { get; set; }

        [YamlMember("angleZ", 10)]
        public float AngleZ { get; set; }
    }

    public class SheetResultMotion
    {
        [YamlMember("valid_flag", 0)]
        public byte ValidFlag { get; set; }

        [YamlMember("id", 1)]
        public ushort ID { get; set; }

        [YamlMember("monsNo", 2)]
        public int MonsNo { get; set; }

        [YamlMember("winAnim", 3)]
        public uint WinAnim { get; set; }

        [YamlMember("loseAnim", 4)]
        public uint LoseAnim { get; set; }

        [YamlMember("waitAnim", 5)]
        public uint WaitAnim { get; set; }

        [YamlMember("duration", 6)]
        public float Duration { get; set; }
    }

    public class SheetValueData
    {
        [YamlMember("valid_flag", 0)]
        public byte ValidFlag { get; set; }

        [YamlMember("id", 1)]
        public ushort ID { get; set; }

        [YamlMember("value", 2)]
        public int Value { get; set; }
    }

    public class SheetStringData
    {
        [YamlMember("valid_flag", 0)]
        public byte ValidFlag { get; set; }

        [YamlMember("id", 1)]
        public ushort ID { get; set; }

        [YamlMember("value", 2)]
        public string Value { get; set; }
    }
}
