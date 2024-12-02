using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    public class ShopTable : MonoBehaviour
    {
        [YamlMember("FS", 10)]
        public SheetFS[] FS { get; set; }

        [YamlMember("FixedShop", 11)]
        public SheetFixedShop[] FixedShop { get; set; }

        [YamlMember("FlowerShop", 12)]
        public SheetFlowerShop[] FlowerShop { get; set; }

        [YamlMember("RibonShop", 13)]
        public SheetRibonShop[] RibonShop { get; set; }

        [YamlMember("SealShop", 14)]
        public SheetSealShop[] SealShop { get; set; }

        [YamlMember("BPShop", 15)]
        public SheetBPShop[] BPShop { get; set; }

        [YamlMember("OtenkiShop", 16)]
        public SheetOtenkiShop[] OtenkiShop { get; set; }

        [YamlMember("BoutiqueShop", 17)]
        public SheetBoutiqueShop[] BoutiqueShop { get; set; }

        [YamlMember("PalParkShop", 18)]
        public SheetPalParkShop[] PalParkShop { get; set; }

        [YamlMember("TobariDepartment4FShop", 19)]
        public SheetTobariDepartment4FShop[] TobariDepartment4FShop { get; set; }
    }

    public class SheetFS
    {
        [YamlMember("ItemNo", 0)]
        public ushort ItemNo { get; set; }

        [YamlMember("BadgeNum", 1)]
        public int BadgeNum { get; set; }

        [YamlMember("ZoneID", 2)]
        public int ZoneID { get; set; }
    }

    public class SheetFixedShop
    {
        [YamlMember("ItemNo", 0)]
        public ushort ItemNo { get; set; }

        [YamlMember("ShopID", 1)]
        public int ShopID { get; set; }
    }

    public class SheetFlowerShop
    {
        [YamlMember("SealNo", 0)]
        public int SealNo { get; set; }

        [YamlMember("ItemNo", 1)]
        public ushort ItemNo { get; set; }

        [YamlMember("Price", 2)]
        public int Price { get; set; }
    }

    public class SheetRibonShop
    {
        [YamlMember("Price", 0)]
        public int Price { get; set; }
    }

    public class SheetSealShop
    {
        [YamlMember("SealNo", 0)]
        public int SealNo { get; set; }

        [YamlMember("Price", 1)]
        public int Price { get; set; }

        [YamlMember("Week", 2)]
        public int Week { get; set; }
    }

    public class SheetBPShop
    {
        [YamlMember("ItemNo", 0)]
        public ushort ItemNo { get; set; }

        [YamlMember("NPCID", 1)]
        public int NPCID { get; set; }
    }

    public class SheetOtenkiShop
    {
        [YamlMember("ItemNo", 0)]
        public ushort ItemNo { get; set; }

        [YamlMember("RequestItem", 1)]
        public ushort RequestItem { get; set; }

        [YamlMember("Price", 2)]
        public int Price { get; set; }
    }

    public class SheetBoutiqueShop
    {
        [YamlMember("DressNo", 0)]
        public int DressNo { get; set; }

        [YamlMember("OpenDress", 1)]
        public int OpenDress { get; set; }

        [YamlMember("DressGet", 2)]
        public int DressGet { get; set; }

        [YamlMember("Price", 3)]
        public int Price { get; set; }
    }

    public class SheetPalParkShop
    {
        [YamlMember("ItemNo", 0)]
        public ushort ItemNo { get; set; }

        [YamlMember("ItemNo2", 1)]
        public ushort ItemNo2 { get; set; }

        [YamlMember("Price", 2)]
        public int Price { get; set; }

        [YamlMember("ShopID", 3)]
        public int ShopID { get; set; }

        [YamlMember("ParkNameID", 4)]
        public int ParkNameID { get; set; }

        [YamlMember("ParkNameNazo", 5)]
        public int ParkNameNazo { get; set; }
    }

    public class SheetTobariDepartment4FShop
    {
        [YamlMember("UgItemID", 0)]
        public int UgItemID { get; set; }

        [YamlMember("price", 1)]
        public int Price { get; set; }

        [YamlMember("ShopID", 3)]
        public int ShopID { get; set; }
    }
}
