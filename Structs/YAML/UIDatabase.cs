using SharpYaml;
using SharpYaml.Serialization;
using System;

namespace ImpostersOrdeal
{
    public class UIDatabase : MonoBehaviour
    {
        [YamlMember("UIWindow", 10)]
        public SheetUIWindow[] UIWindow { get; set; }

        [YamlMember("Transition", 11)]
        public SheetTransition[] Transition { get; set; }

        [YamlMember("SpriteAtlas", 12)]
        public SheetSpriteAtlas[] SpriteAtlas { get; set; }

        [YamlMember("SharedSprite", 13)]
        public SheetSharedSprite[] SharedSprite { get; set; }

        [YamlMember("CommonSprite", 14)]
        public SheetCommonSprite[] CommonSprite { get; set; }

        [YamlMember("PokemonIcon", 15)]
        public SheetPokemonIcon[] PokemonIcon { get; set; }

        [YamlMember("AshiatoIcon", 16)]
        public SheetAshiatoIcon[] AshiatoIcon { get; set; }

        [YamlMember("PokemonVoice", 17)]
        public SheetPokemonVoice[] PokemonVoice { get; set; }

        [YamlMember("MonsterBall", 18)]
        public SheetMonsterBall[] MonsterBall { get; set; }

        [YamlMember("ContextMenu", 19)]
        public SheetContextMenu[] ContextMenu { get; set; }

        [YamlMember("Keyguide", 20)]
        public SheetKeyguide[] Keyguide { get; set; }

        [YamlMember("CharacterBag", 21)]
        public SheetCharacterBag[] CharacterBag { get; set; }

        [YamlMember("ZukanDisplay", 22)]
        public SheetZukanDisplay[] ZukanDisplay { get; set; }

        [YamlMember("ZukanComparePlayer", 23)]
        public SheetZukanComparePlayer[] ZukanComparePlayer { get; set; }

        [YamlMember("ZukanCompareHeight", 24)]
        public SheetZukanCompareHeight[] ZukanCompareHeight { get; set; }

        [YamlMember("ZukanCompareWeight", 25)]
        public SheetZukanCompareWeight[] ZukanCompareWeight { get; set; }

        [YamlMember("FloorDisplay", 26)]
        public SheetFloorDisplay[] FloorDisplay { get; set; }

        [YamlMember("ShopMessage", 27)]
        public SheetShopMessage[] ShopMessage { get; set; }

        [YamlMember("PlaceMark", 28)]
        public SheetPlaceMark[] PlaceMark { get; set; }

        [YamlMember("MarkColor", 29)]
        public SheetMarkColor[] MarkColor { get; set; }

        [YamlMember("Wallpaper", 30)]
        public SheetWallpaper[] Wallpaper { get; set; }

        [YamlMember("Box", 31)]
        public SheetBox[] Box { get; set; }

        [YamlMember("PokeColor", 32)]
        public SheetPokeColor[] PokeColor { get; set; }

        [YamlMember("DistributionMapchip", 33)]
        public SheetDistributionMapchip[] DistributionMapchip { get; set; }

        [YamlMember("PolishedBadge", 34)]
        public SheetPolishedBadge[] PolishedBadge { get; set; }

        [YamlMember("SearchPokeIconSex", 35)]
        public SheetSearchPokeIconSex[] SearchPokeIconSex { get; set; }

        [YamlMember("HideWazaName", 36)]
        public SheetHideWazaName[] HideWazaName { get; set; }

        [YamlMember("HideTokusei", 37)]
        public SheetHideTokusei[] HideTokusei { get; set; }

        [YamlMember("ZukanRating", 38)]
        public SheetZukanRating[] ZukanRating { get; set; }

        [YamlMember("BoxOpenParam", 39)]
        public SheetBoxOpenParam[] BoxOpenParam { get; set; }

        [YamlMember("SealTemplate", 40)]
        public SheetSealTemplateUI[] SealTemplate { get; set; }

        [YamlMember("RankingDisplay", 41)]
        public SheetRankingDisplay[] RankingDisplay { get; set; }
    }

    [Serializable]
    public class SheetUIWindow
    {
        [YamlMember("WindowAnimId", 0)]
        public int WindowAnimID { get; set; }

        [YamlMember("AssetBundleName", 1)]
        public string AssetBundleName { get; set; }

        [YamlMember("AssetName", 2)]
        public string AssetName { get; set; }

        [YamlMember("Resident", 3)]
        public byte Resident { get; set; }

        [YamlMember("UiCameraMode", 4)]
        public int UICameraMode { get; set; }

        [YamlMember("Firstload", 5)]
        public byte FirstLoad { get; set; }
    }

    [Serializable]
    public class SheetTransition
    {
        [YamlMember("AssetBundleName", 0)]
        public string AssetBundleName { get; set; }

        [YamlMember("AssetName", 1)]
        public string AssetName { get; set; }
    }

    [Serializable]
    public class SheetSpriteAtlas
    {
        [YamlMember("AssetBundleName", 0)]
        public string AssetBundleName { get; set; }

        [YamlMember("AssetName", 1)]
        public string AssetName { get; set; }

        [YamlMember("Resident", 2)]
        public byte Resident { get; set; }

        [YamlMember("IsLanguage", 3)]
        public byte IsLanguage { get; set; }
    }

    [Serializable]
    public class SheetSharedSprite
    {
        [YamlMember("SpriteName", 0)]
        public string SpriteName { get; set; }
    }

    [Serializable]
    public class SheetCommonSprite
    {
        [YamlMember("SpriteName", 0)]
        public string SpriteName { get; set; }
    }

    [Serializable]
    public class SheetPokemonIcon
    {
        [YamlMember("UniqueID", 0)]
        public int UniqueID { get; set; }

        [YamlMember("AssetBundleName", 1)]
        public string AssetBundleName { get; set; }

        [YamlMember("AssetName", 2)]
        public string AssetName { get; set; }

        [YamlMember("AssetBundleNameLarge", 3)]
        public string AssetBundleNameLarge { get; set; }

        [YamlMember("AssetNameLarge", 4)]
        public string AssetNameLarge { get; set; }

        [YamlMember("AssetBundleNameDP", 5)]
        public string AssetBundleNameDP { get; set; }

        [YamlMember("AssetNameDP", 6)]
        public string AssetNameDP { get; set; }

        [YamlMember("HallofFameOffset", 7)]
        public UnityVector2 HallofFameOffset { get; set; }
    }

    [Serializable]
    public class SheetAshiatoIcon
    {
        [YamlMember("UniqueID", 0)]
        public int UniqueID { get; set; }

        [YamlMember("SideIconAssetName", 1)]
        public string SideIconAssetName { get; set; }

        [YamlMember("BothIconAssetName", 2)]
        public string BothIconAssetName { get; set; }
    }

    [Serializable]
    public class SheetPokemonVoice
    {
        [YamlMember("UniqueID", 0)]
        public int UniqueID { get; set; }

        [YamlMember("WwiseEvent", 1)]
        public string WwiseEvent { get; set; }

        [YamlMember("stopEventId", 2)]
        public string StopEventId { get; set; }

        [YamlMember("CenterPointOffset", 3)]
        public UnityVector3 CenterPointOffset { get; set; }

        [YamlMember("RotationLimits", 4)]
        public byte RotationLimits { get; set; }

        [YamlMember("RotationLimitAngle", 5)]
        public UnityVector2 RotationLimitAngle { get; set; }
    }

    [Serializable]
    public class SheetMonsterBall
    {
        [YamlMember("BallId", 0)]
        public byte BallId { get; set; }

        [YamlMember("ItemNo", 1)]
        public ushort ItemNo { get; set; }
    }

    [Serializable]
    public class SheetContextMenu
    {
        [YamlMember("MessageFile", 0)]
        public string MessageFile { get; set; }

        [YamlMember("MessageLabel", 1)]
        public string MessageLabel { get; set; }
    }

    [Serializable]
    public class SheetKeyguide
    {
        [YamlMember("ButtonSpriteId", 0)]
        public int ButtonSpriteId { get; set; }

        [YamlMember("MessageFile", 1)]
        public string MessageFile { get; set; }

        [YamlMember("MessageLabel", 2)]
        public string MessageLabel { get; set; }
    }

    [Serializable]
    public class SheetCharacterBag
    {
        [YamlMember("Index", 0)]
        public int Index { get; set; }

        [YamlMember("XMenuDefault", 1)]
        public string XMenuDefault { get; set; }

        [YamlMember("XMenuSelect", 2)]
        public string XMenuSelect { get; set; }

        [YamlMember("XMenuShadow", 3)]
        public string XMenuShadow { get; set; }

        [YamlMember("BagHeader", 4)]
        public string BagHeader { get; set; }

        [YamlMember("BagBase", 5)]
        public string BagBase { get; set; }

        [YamlMember("BagPockets", 6)]
        public string[] BagPockets { get; set; }
    }

    [Serializable]
    public class SheetZukanDisplay
    {
        [YamlMember("UniqueID", 0)]
        public int UniqueID { get; set; }

        [YamlMember("MoveLimit", 1)]
        public UnityVector3 MoveLimit { get; set; }

        [YamlMember("ModelOffset", 2)]
        public UnityVector3 ModelOffset { get; set; }

        [YamlMember("ModelRotationAngle", 3)]
        public UnityVector2 ModelRotationAngle { get; set; }
    }

    [Serializable]
    public class SheetZukanComparePlayer
    {
        [YamlMember("Sex", 0)]
        public byte Sex { get; set; }

        [YamlMember("Height", 1)]
        public ushort Height { get; set; }

        [YamlMember("Weight", 2)]
        public ushort Weight { get; set; }
    }

    [Serializable]
    public class SheetZukanCompareHeight
    {
        [YamlMember("UniqueID", 0)]
        public int UniqueID { get; set; }

        [YamlMember("PlayerScaleFactor", 1)]
        public float PlayerScaleFactor { get; set; }

        [YamlMember("PlayerOffset", 2)]
        public UnityVector3 PlayerOffset { get; set; }

        [YamlMember("PlayerRotationAngle", 3)]
        public UnityVector2 PlayerRotationAngle { get; set; }
    }

    [Serializable]
    public class SheetZukanCompareWeight
    {
        [YamlMember("No", 0)]
        public int No { get; set; }

        [YamlMember("DiffMin", 1)]
        public ushort DiffMin { get; set; }

        [YamlMember("DiffMax", 2)]
        public ushort DiffMax { get; set; }
    }

    [Serializable]
    public class SheetFloorDisplay
    {
        [YamlMember("MessageFile", 0)]
        public string MessageFile { get; set; }

        [YamlMember("MessageLabel", 1)]
        public string MessageLabel { get; set; }
    }

    [Serializable]
    public class SheetShopMessage
    {
        [YamlMember("Message0", 0)]
        public string[] Message0 { get; set; }

        [YamlMember("Message1", 1)]
        public string[] Message1 { get; set; }

        [YamlMember("Message2", 2)]
        public string[] Message2 { get; set; }

        [YamlMember("Message3", 3)]
        public string[] Message3 { get; set; }

        [YamlMember("Message4", 4)]
        public string[] Message4 { get; set; }

        [YamlMember("Message5", 5)]
        public string[] Message5 { get; set; }

        [YamlMember("Message6", 6)]
        public string[] Message6 { get; set; }

        [YamlMember("Message7", 7)]
        public string[] Message7 { get; set; }

        [YamlMember("Message8", 8)]
        public string[] Message8 { get; set; }
    }

    [Serializable]
    public class SheetPlaceMark
    {
        [YamlMember("SpriteName", 0)]
        public string SpriteName { get; set; }

        [YamlMember("CassetVersions", 1)]
        public int[] CassetVersions { get; set; }
    }

    [Serializable]
    public class SheetMarkColor
    {
        [YamlMember("Color", 0)]
        public UnityColor32 Color { get; set; }
    }

    [Serializable]
    public class SheetWallpaper
    {
        [YamlMember("MessageFile", 0)]
        public string MessageFile { get; set; }

        [YamlMember("MessageLabel", 1)]
        public string MessageLabel { get; set; }

        [YamlMember("Color", 2)]
        public UnityColor32 Color { get; set; }
    }

    [Serializable]
    public class SheetBox
    {
        [YamlMember("MessageFile", 0)]
        public string MessageFile { get; set; }

        [YamlMember("MessageLabel", 1)]
        public string MessageLabel { get; set; }

        [YamlMember("Wallpaper", 2)]
        public int Wallpaper { get; set; }
    }

    [Serializable]
    public class SheetPokeColor
    {
        [YamlMember("Color", 0)]
        public UnityColor32 Color { get; set; }
    }

    [Serializable]
    public class SheetDistributionMapchip
    {
        [YamlMember("SpriteName", 0)]
        public string SpriteName { get; set; }

        [YamlMember("FlipX", 1)]
        public byte FlipX { get; set; }

        [YamlMember("FlipY", 2)]
        public byte FlipY { get; set; }
    }

    [Serializable]
    public class SheetPolishedBadge
    {
        [YamlMember("Value", 0)]
        public ushort Value { get; set; }
    }

    [Serializable]
    public class SheetSearchPokeIconSex
    {
        [YamlMember("MonsNo", 0)]
        public int MonsNo { get; set; }

        [YamlMember("Sex", 1)]
        public int Sex { get; set; }
    }

    [Serializable]
    public class SheetHideWazaName
    {
        [YamlMember("WazaID", 0)]
        public string WazaID { get; set; }
    }

    [Serializable]
    public class SheetHideTokusei
    {
        [YamlMember("TokuseiID", 0)]
        public string TokuseiID { get; set; }
    }

    [Serializable]
    public class SheetZukanRating
    {
        [YamlMember("IsZenkokuZukan", 0)]
        public byte IsZenkokuZukan { get; set; }

        [YamlMember("IsGetCount", 1)]
        public byte IsGetCount { get; set; }

        [YamlMember("OtherCondition", 2)]
        public int OtherCondition { get; set; }

        [YamlMember("RatingMinCount", 3)]
        public ushort RatingMinCount { get; set; }

        [YamlMember("RatingMaxCount", 4)]
        public ushort RatingMaxCount { get; set; }

        [YamlMember("MessageFile", 5)]
        public string MessageFile { get; set; }

        [YamlMember("MessageLabel1", 6)]
        public string MessageLabel1 { get; set; }

        [YamlMember("MessageLabel2", 7)]
        public string MessageLabel2 { get; set; }

        [YamlMember("MessageLabel3", 8)]
        public string MessageLabel3 { get; set; }
    }

    [Serializable]
    public class SheetBoxOpenParam
    {
        [YamlMember("MonsNo", 0)]
        public int[] MonsNo { get; set; }

        [YamlMember("SelectCount", 1)]
        public int SelectCount { get; set; }

        [YamlMember("Level", 2)]
        public int Level { get; set; }

        [YamlMember("IsTrade", 3)]
        public byte IsTrade { get; set; }

        [YamlMember("IsEnableParty", 4)]
        public byte IsEnableParty { get; set; }

        [YamlMember("IsEnableDying", 5)]
        public byte IsEnableDying { get; set; }
    }

    [Serializable]
    public class SheetSealTemplateUI
    {
        [YamlMember("MessageFile", 0)]
        public string MessageFile { get; set; }

        [YamlMember("TitleMessageLabel", 1)]
        public string TitleMessageLabel { get; set; }

        [YamlMember("NameMessageLabel", 2)]
        public string NameMessageLabel { get; set; }

        [YamlMember("TrainerImageFile", 3)]
        public string TrainerImageFile { get; set; }

        [YamlMember("TrainerEmblemFile", 4)]
        public string TrainerEmblemFile { get; set; }
    }

    [Serializable]
    public class SheetRankingDisplay
    {
        [YamlMember("RankingId", 0)]
        public int RankingID { get; set; }

        [YamlMember("RecordId", 1)]
        public int RecordID { get; set; }

        [YamlMember("Category", 2)]
        public int Category { get; set; }

        [YamlMember("RankingItem", 3)]
        public string RankingItem { get; set; }

        [YamlMember("Guide", 4)]
        public string Guide { get; set; }

        [YamlMember("Counter", 5)]
        public string Counter { get; set; }
    }
}
