using SharpYaml;
using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    public class PokemonInfo : MonoBehaviour
    {
        [YamlMember("Catalog", 10)]
        public SheetCatalog[] Catalog { get; set; }

        [YamlMember("Trearuki", 11)]
        public SheetTrearuki[] Trearuki { get; set; }
    }

    public class SheetCatalog
    {
        [YamlMember("UniqueID", 0)]
        public int UniqueID { get; set; }

        [YamlMember("No", 1)]
        public int No { get; set; }

        [YamlMember("SinnohNo", 2)]
        public int SinnohNo { get; set; }

        [YamlMember("MonsNo", 3)]
        public int MonsNo { get; set; }

        [YamlMember("FormNo", 4)]
        public int FormNo { get; set; }

        [YamlMember("Sex", 5)]
        public byte Sex { get; set; }

        [YamlMember("Rare", 6)]
        public byte Rare { get; set; }

        [YamlMember("AssetBundleName", 7)]
        public string AssetBundleName { get; set; }

        [YamlMember("BattleScale", 8)]
        public float BattleScale { get; set; }

        [YamlMember("ContestScale", 9)]
        public float ContestScale { get; set; }

        [YamlMember("ContestSize", 10)]
        public int ContestSize { get; set; }

        [YamlMember("FieldScale", 11)]
        public float FieldScale { get; set; }

        [YamlMember("FieldChikaScale", 12)]
        public float FieldChikaScale { get; set; }

        [YamlMember("StatueScale", 13)]
        public float StatueScale { get; set; }

        [YamlMember("FieldWalkingScale", 14)]
        public float FieldWalkingScale { get; set; }

        [YamlMember("FieldFureaiScale", 15)]
        public float FieldFureaiScale { get; set; }

        [YamlMember("MenuScale", 16)]
        public float MenuScale { get; set; }

        [YamlMember("ModelMotion", 17)]
        public string ModelMotion { get; set; }

        [YamlMember("ModelOffset", 18)]
        public UnityVector3 ModelOffset { get; set; }

        [YamlMember("ModelRotationAngle", 19)]
        public UnityVector3 ModelRotationAngle { get; set; }

        [YamlMember("DistributionScale", 20)]
        public float DistributionScale { get; set; }

        [YamlMember("DistributionModelMotion", 21)]
        public string DistributionModelMotion { get; set; }

        [YamlMember("DistributionModelOffset", 22)]
        public UnityVector3 DistributionModelOffset { get; set; }

        [YamlMember("DistributionModelRotationAngle", 23)]
        public UnityVector3 DistributionModelRotationAngle { get; set; }

        [YamlMember("VoiceScale", 24)]
        public float VoiceScale { get; set; }

        [YamlMember("VoiceModelMotion", 25)]
        public string VoiceModelMotion { get; set; }

        [YamlMember("VoiceModelOffset", 26)]
        public UnityVector3 VoiceModelOffset { get; set; }

        [YamlMember("VoiceModelRotationAngle", 27)]
        public UnityVector3 VoiceModelRotationAngle { get; set; }

        [YamlMember("CenterPointOffset", 28)]
        public UnityVector3 CenterPointOffset { get; set; }

        [YamlMember("RotationLimitAngle", 29)]
        public UnityVector2 RotationLimitAngle { get; set; }

        [YamlMember("StatusScale", 30)]
        public float StatusScale { get; set; }

        [YamlMember("StatusModelMotion", 31)]
        public string StatusModelMotion { get; set; }

        [YamlMember("StatusModelOffset", 32)]
        public UnityVector3 StatusModelOffset { get; set; }

        [YamlMember("StatusModelRotationAngle", 33)]
        public UnityVector3 StatusModelRotationAngle { get; set; }

        [YamlMember("BoxScale", 34)]
        public float BoxScale { get; set; }

        [YamlMember("BoxModelMotion", 35)]
        public string BoxModelMotion { get; set; }

        [YamlMember("BoxModelOffset", 36)]
        public UnityVector3 BoxModelOffset { get; set; }

        [YamlMember("BoxModelRotationAngle", 37)]
        public UnityVector3 BoxModelRotationAngle { get; set; }

        [YamlMember("CompareScale", 38)]
        public float CompareScale { get; set; }

        [YamlMember("CompareModelMotion", 39)]
        public string CompareModelMotion { get; set; }

        [YamlMember("CompareModelOffset", 40)]
        public UnityVector3 CompareModelOffset { get; set; }

        [YamlMember("CompareModelRotationAngle", 41)]
        public UnityVector3 CompareModelRotationAngle { get; set; }

        [YamlMember("BrakeStart", 42)]
        public float BrakeStart { get; set; }

        [YamlMember("BrakeEnd", 43)]
        public float BrakeEnd { get; set; }

        [YamlMember("WalkSpeed", 44)]
        public float WalkSpeed { get; set; }

        [YamlMember("RunSpeed", 45)]
        public float RunSpeed { get; set; }

        [YamlMember("WalkStart", 46)]
        public float WalkStart { get; set; }

        [YamlMember("RunStart", 47)]
        public float RunStart { get; set; }

        [YamlMember("BodySize", 48)]
        public float BodySize { get; set; }

        [YamlMember("AppearLimit", 49)]
        public float AppearLimit { get; set; }

        [YamlMember("MoveType", 50)]
        public int MoveType { get; set; }

        [YamlMember("GroundEffect", 51)]
        public byte GroundEffect { get; set; }

        [YamlMember("Waitmoving", 52)]
        public byte Waitmoving { get; set; }

        [YamlMember("BattleAjustHeight", 53)]
        public int BattleAjustHeight { get; set; }
    }

    public class SheetTrearuki
    {
        [YamlMember("Enable", 0)]
        public byte Enable { get; set; }

        [YamlMember("AnimeIndex", 1)]
        public int[] AnimeIndex { get; set; }

        [YamlMember("AnimeDuration", 2)]
        public float[] AnimeDuration { get; set; }
    }
}
