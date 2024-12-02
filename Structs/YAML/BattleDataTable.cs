using SharpYaml;
using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    public class BattleDataTable : MonoBehaviour
    {
        [YamlMember("BattleSetupEffectData", 10)]
        public SheetBattleSetupEffectData[] BattleSetupEffectData { get; set; }

        [YamlMember("SetupIntroPlaySequenceData", 11)]
        public SheetSetupIntroPlaySequenceData[] SetupIntroPlaySequenceData { get; set; }

        [YamlMember("BattleStatusEffectObserverData", 12)]
        public SheetBattleStatusEffectObserverData[] BattleStatusEffectObserverData { get; set; }

        [YamlMember("BattleMiscEffectData", 13)]
        public SheetBattleMiscEffectData[] BattleMiscEffectData { get; set; }

        [YamlMember("BattleWazaData", 14)]
        public SheetBattleWazaData[] BattleWazaData { get; set; }

        [YamlMember("BattleUniqueWazaData", 15)]
        public SheetBattleUniqueWazaData[] BattleUniqueWazaData { get; set; }

        [YamlMember("SafariCmdData", 16)]
        public SheetSafariCmdData[] SafariCmdData { get; set; }

        [YamlMember("BattleWazaMsgFrameData", 17)]
        public SheetBattleWazaMsgFrameData[] BattleWazaMsgFrameData { get; set; }

        [YamlMember("WeatherData", 18)]
        public SheetWeatherData[] WeatherData { get; set; }

        [YamlMember("BattleGroundData", 19)]
        public SheetBattleGroundData[] BattleGroundData { get; set; }

        [YamlMember("BallEffectData", 20)]
        public SheetBallEffectData[] BallEffectData { get; set; }

        [YamlMember("MotionTimingData", 21)]
        public SheetMotionTimingData[] MotionTimingData { get; set; }

        [YamlMember("MotionReplaceData", 22)]
        public SheetMotionReplaceData[] MotionReplaceData { get; set; }

        [YamlMember("PokemonEntryMotionData", 23)]
        public SheetPokemonEntryMotionData[] PokemonEntryMotionData { get; set; }

        [YamlMember("BattleConstant", 24)]
        public SheetBattleConstant[] BattleConstant { get; set; }

        [YamlMember("AgeEyeBlink", 25)]
        public SheetAgeEyeBlink[] AgeEyeBlink { get; set; }

        [YamlMember("DisableBlinkPokemon", 26)]
        public SheetDisableBlinkPokemon[] DisableBlinkPokemon { get; set; }

        [YamlMember("PokemonMotionBlendTime", 27)]
        public SheetPokemonMotionBlendTime[] PokemonMotionBlendTime { get; set; }

        [YamlMember("InterpolationSequence", 28)]
        public SheetInterpolationSequence[] InterpolationSequence { get; set; }
    }

    public class SheetBattleSetupEffectData
    {
        [YamlMember("CmdSeqName", 0)]
        public string[] CmdSeqName { get; set; }

        [YamlMember("Weight", 1)]
        public byte[] Weight { get; set; }

        [YamlMember("Cond", 2)]
        public byte[] Cond { get; set; }

        [YamlMember("BattleBGM", 3)]
        public string BattleBGM { get; set; }

        [YamlMember("WinBGM", 4)]
        public string WinBGM { get; set; }

        [YamlMember("FadeType", 5)]
        public int FadeType { get; set; }
    }

    public class SheetSetupIntroPlaySequenceData
    {
        [YamlMember("Key", 0)]
        public int Key { get; set; }

        [YamlMember("SeqName", 1)]
        public string SeqName { get; set; }
    }

    public class SheetBattleStatusEffectObserverData
    {
        [YamlMember("SickType", 0)]
        public uint SickType { get; set; }

        [YamlMember("EffectID", 1)]
        public int EffectID { get; set; }

        [YamlMember("Offset", 2)]
        public UnityVector3 Offset { get; set; }

        [YamlMember("Scale", 3)]
        public UnityVector3 Scale { get; set; }

        [YamlMember("Node", 4)]
        public int Node { get; set; }

        [YamlMember("AdustScale", 5)]
        public byte AdustScale { get; set; }
    }

    public class SheetBattleMiscEffectData
    {
        [YamlMember("Index", 0)]
        public int Index { get; set; }

        [YamlMember("BtlEff", 1)]
        public int BtlEff { get; set; }

        [YamlMember("CmdSeqName", 2)]
        public string CmdSeqName { get; set; }
    }

    public class SheetBattleWazaData
    {
        [YamlMember("WazaNo", 0)]
        public int WazaNo { get; set; }

        [YamlMember("CmdSeqName", 1)]
        public string CmdSeqName { get; set; }

        [YamlMember("CmdSeqNameLegend", 2)]
        public string CmdSeqNameLegend { get; set; }

        [YamlMember("NotShortenTurnType0", 3)]
        public string NotShortenTurnType0 { get; set; }

        [YamlMember("NotShortenTurnType1", 4)]
        public string NotShortenTurnType1 { get; set; }

        [YamlMember("TurnType1", 5)]
        public string TurnType1 { get; set; }

        [YamlMember("TurnType2", 6)]
        public string TurnType2 { get; set; }

        [YamlMember("TurnType3", 7)]
        public string TurnType3 { get; set; }

        [YamlMember("TurnType4", 8)]
        public string TurnType4 { get; set; }
    }

    public class SheetBattleUniqueWazaData
    {
        [YamlMember("WazaNo", 0)]
        public int WazaNo { get; set; }

        [YamlMember("MonsNo", 1)]
        public int MonsNo { get; set; }

        [YamlMember("FormNo", 2)]
        public byte FormNo { get; set; }

        [YamlMember("SrcTurnType", 3)]
        public int SrcTurnType { get; set; }

        [YamlMember("DestTurnType", 4)]
        public int DestTurnType { get; set; }
    }

    public class SheetSafariCmdData
    {
        [YamlMember("SafariCmd", 0)]
        public int SafariCmd { get; set; }

        [YamlMember("CmdSeqName", 1)]
        public string CmdSeqName { get; set; }

        [YamlMember("CmdCritialSeqName", 2)]
        public string CmdCritialSeqName { get; set; }
    }

    public class SheetBattleWazaMsgFrameData
    {
        [YamlMember("Index", 0)]
        public int Index { get; set; }

        [YamlMember("FileName", 1)]
        public string FileName { get; set; }

        [YamlMember("frame", 2)]
        public int Frame { get; set; }
    }

    public class SheetWeatherData
    {
        [YamlMember("MainFileName", 0)]
        public string MainFileName { get; set; }

        [YamlMember("CameraFileName", 1)]
        public string CameraFileName { get; set; }

        [YamlMember("RangeFileName", 2)]
        public string RangeFileName { get; set; }

        [YamlMember("RegisterName", 3)]
        public string RegisterName { get; set; }

        [YamlMember("CameraScale", 4)]
        public UnityVector3 CameraScale { get; set; }

        [YamlMember("LightPower", 5)]
        public float LightPower { get; set; }

        [YamlMember("Time", 6)]
        public int Time { get; set; }

        [YamlMember("ShortTime", 7)]
        public int ShortTime { get; set; }
    }

    public class SheetBattleGroundData
    {
        [YamlMember("MainFileName", 0)]
        public string MainFileName { get; set; }

        [YamlMember("CameraFileName", 1)]
        public string CameraFileName { get; set; }

        [YamlMember("MainScale", 2)]
        public UnityVector3 MainScale { get; set; }

        [YamlMember("CameraScale", 3)]
        public UnityVector3 CameraScale { get; set; }
    }

    public class SheetBallEffectData
    {
        [YamlMember("BallID", 0)]
        public byte BallID { get; set; }

        [YamlMember("IntroEffectAssetbundleName", 1)]
        public string IntroEffectAssetbundleName { get; set; }

        [YamlMember("CaptureEffectAssetbundleName", 2)]
        public string CaptureEffectAssetbundleName { get; set; }
    }
     
    public class SheetMotionTimingData
    {
        [YamlMember("MonsNo", 0)]
        public int MonsNo { get; set; }

        [YamlMember("FormNo", 1)]
        public int FormNo { get; set; }

        [YamlMember("Sex", 2)]
        public int Sex { get; set; }

        [YamlMember("Buturi01", 3)]
        public int Buturi01 { get; set; }

        [YamlMember("Buturi02", 4)]
        public int Buturi02 { get; set; }

        [YamlMember("Buturi03", 5)]
        public int Buturi03 { get; set; }

        [YamlMember("Tokusyu01", 6)]
        public int Tokusyu01 { get; set; }

        [YamlMember("Tokusyu02", 7)]
        public int Tokusyu02 { get; set; }

        [YamlMember("Tokusyu03", 8)]
        public int Tokusyu03 { get; set; }

        [YamlMember("BodyBlow", 9)]
        public int BodyBlow { get; set; }

        [YamlMember("Punch", 10)]
        public int Punch { get; set; }

        [YamlMember("Kick", 11)]
        public int Kick { get; set; }

        [YamlMember("Tail", 12)]
        public int Tail { get; set; }

        [YamlMember("Bite", 13)]
        public int Bite { get; set; }

        [YamlMember("Peck", 14)]
        public int Peck { get; set; }

        [YamlMember("Radial", 15)]
        public int Radial { get; set; }

        [YamlMember("Cry", 16)]
        public int Cry { get; set; }

        [YamlMember("Dust", 17)]
        public int Dust { get; set; }

        [YamlMember("Shot", 18)]
        public int Shot { get; set; }

        [YamlMember("Guard", 19)]
        public int Guard { get; set; }

        [YamlMember("LandingFall", 20)]
        public int LandingFall { get; set; }

        [YamlMember("LandingFallEase", 21)]
        public int LandingFallEase { get; set; }
    }

    public class SheetMotionReplaceData
    {
        [YamlMember("UniqueID", 0)]
        public int UniqueID { get; set; }

        [YamlMember("No", 1)]
        public int No { get; set; }

        [YamlMember("MonsNo", 2)]
        public int MonsNo { get; set; }

        [YamlMember("FormNo", 3)]
        public int FormNo { get; set; }

        [YamlMember("BodyBlow", 4)]
        public int BodyBlow { get; set; }

        [YamlMember("Punch", 5)]
        public int Punch { get; set; }

        [YamlMember("Kick", 6)]
        public int Kick { get; set; }

        [YamlMember("Tail", 7)]
        public int Tail { get; set; }

        [YamlMember("Bite", 8)]
        public int Bite { get; set; }

        [YamlMember("Peck", 9)]
        public int Peck { get; set; }

        [YamlMember("Radial", 10)]
        public int Radial { get; set; }

        [YamlMember("Cry", 11)]
        public int Cry { get; set; }

        [YamlMember("Dust", 12)]
        public int Dust { get; set; }

        [YamlMember("Shot", 13)]
        public int Shot { get; set; }

        [YamlMember("Guard", 14)]
        public int Guard { get; set; }

        [YamlMember("Buturi01", 15)]
        public int Buturi01 { get; set; }

        [YamlMember("Tokusyu01", 16)]
        public int Tokusyu01 { get; set; }

        [YamlMember("WazaNo", 17)]
        public int WazaNo { get; set; }

        [YamlMember("wazaMotion", 18)]
        public int WazaMotion { get; set; }
    }

    public class SheetPokemonEntryMotionData
    {
        [YamlMember("Index", 0)]
        public int Index { get; set; }

        [YamlMember("friendship", 1)]
        public int[] Friendship { get; set; }

        [YamlMember("AnimationState", 2)]
        public int[] AnimationState { get; set; }

        [YamlMember("rate", 3)]
        public int[] Rate { get; set; }
    }

    public class SheetBattleConstant
    {
        [YamlMember("Key", 0)]
        public int Key { get; set; }

        [YamlMember("IntValue", 1)]
        public int IntValue { get; set; }

        [YamlMember("FloatValue", 2)]
        public float FloatValue { get; set; }

        [YamlMember("StringValue", 3)]
        public string StringValue { get; set; }
    }

    public class SheetAgeEyeBlink
    {
        [YamlMember("age", 0)]
        public byte Age { get; set; }

        [YamlMember("min", 1)]
        public int Min { get; set; }

        [YamlMember("max", 2)]
        public int Max { get; set; }

        [YamlMember("twiceRate", 3)]
        public int TwiceRate { get; set; }
    }

    public class SheetDisableBlinkPokemon
    {
        [YamlMember("id", 0)]
        public int Id { get; set; }

        [YamlMember("MonsNo", 1)]
        public int MonsNo { get; set; }
    }

    public class SheetPokemonMotionBlendTime
    {
        [YamlMember("id", 0)]
        public int Id { get; set; }

        [YamlMember("MonsNo", 1)]
        public int MonsNo { get; set; }

        [YamlMember("Damege", 2)]
        public float Damege { get; set; }
    }

    public class SheetInterpolationSequence
    {
        [YamlMember("id", 0)]
        public int Id { get; set; }

        [YamlMember("SeqFile", 1)]
        public string SeqFile { get; set; }
    }
}
