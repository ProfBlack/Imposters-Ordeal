using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    public class FieldEncountTable : MonoBehaviour
    {
        [YamlMember("table")]
        public Sheettable[] Table { get; set; }

        [YamlMember("urayama")]
        public Sheeturayama[] Urayama { get; set; }

        [YamlMember("mistu")]
        public Sheetmistu[] Mistu { get; set; }

        [YamlMember("honeytree")]
        public Sheethoneytree[] HoneyTree { get; set; }

        [YamlMember("safari")]
        public Sheetsafari[] Safari { get; set; }

        [YamlMember("mvpoke")]
        public Sheetmvpoke[] MVPoke { get; set; }

        [YamlMember("legendpoke")]
        public Sheetlegendpoke[] LegendPoke { get; set; }

        [YamlMember("zui")]
        public Sheetzui[] Zui { get; set; }
    }

    public class MonsLv
    {
        [YamlMember("maxlv")]
        public int MaxLv { get; set; }

        [YamlMember("minlv")]
        public int MinLv { get; set; }

        [YamlMember("monsNo")]
        public int MonsNo { get; set; }
    }

    public class Sheettable
    {
        [YamlMember("zoneID")]
        public int ZoneID { get; set; }

        [YamlMember("encRate_gr")]
        public int EncRateGr { get; set; }

        [YamlMember("ground_mons")]
        public MonsLv[] GroundMons { get; set; }

        [YamlMember("tairyo")]
        public MonsLv[] Tairyo { get; set; }

        [YamlMember("day")]
        public MonsLv[] Day { get; set; }

        [YamlMember("night")]
        public MonsLv[] Night { get; set; }

        [YamlMember("swayGrass")]
        public MonsLv[] SwayGrass { get; set; }

        [YamlMember("FormProb")]
        public int[] FormProb { get; set; }

        [YamlMember("Nazo")]
        public int[] Nazo { get; set; }

        [YamlMember("AnnoonTable")]
        public int[] AnnoonTable { get; set; }

        [YamlMember("gbaRuby")]
        public MonsLv[] GBARuby { get; set; }

        [YamlMember("gbaSapp")]
        public MonsLv[] GBASapp { get; set; }

        [YamlMember("gbaEme")]
        public MonsLv[] GBAEme { get; set; }

        [YamlMember("gbaFire")]
        public MonsLv[] GBAFire { get; set; }

        [YamlMember("gbaLeaf")]
        public MonsLv[] GBALeaf { get; set; }

        [YamlMember("encRate_wat")]
        public int EncRateWat { get; set; }

        [YamlMember("water_mons")]
        public MonsLv[] WaterMons { get; set; }

        [YamlMember("encRate_turi_boro")]
        public int EncRateTuriBoro { get; set; }

        [YamlMember("boro_mons")]
        public MonsLv[] BoroMons { get; set; }

        [YamlMember("encRate_turi_ii")]
        public int EncRateTuriIi { get; set; }

        [YamlMember("ii_mons")]
        public MonsLv[] IiMons { get; set; }

        [YamlMember("encRate_sugoi")]
        public int EncRateSugoi { get; set; }

        [YamlMember("sugoi_mons")]
        public MonsLv[] SugoiMons { get; set; }
    }

    public class Sheeturayama
    {
        [YamlMember("monsNo")]
        public int MonsNo { get; set; }
    }

    public class Sheetmistu
    {
        [YamlMember("Rate")]
        public int Rate { get; set; }

        [YamlMember("Normal")]
        public int Normal { get; set; }

        [YamlMember("Rare")]
        public int Rare { get; set; }

        [YamlMember("SuperRare")]
        public int SuperRare { get; set; }
    }

    public class Sheethoneytree
    {
        [YamlMember("Normal")]
        public int Normal { get; set; }

        [YamlMember("Rare")]
        public int Rare { get; set; }
    }

    public class Sheetsafari
    {
        [YamlMember("MonsNo")]
        public int MonsNo { get; set; }
    }

    public class Sheetmvpoke
    {
        [YamlMember("zoneID")]
        public int ZoneID { get; set; }

        [YamlMember("nextCount")]
        public int NextCount { get; set; }

        [YamlMember("nextZoneID")]
        public int[] NextZoneID { get; set; }
    }

    public class Sheetlegendpoke
    {
        [YamlMember("monsNo")]
        public int MonsNo { get; set; }

        [YamlMember("formNo")]
        public int FormNo { get; set; }

        [YamlMember("isFixedEncSeq")]
        public bool IsFixedEncSeq { get; set; }

        [YamlMember("encSeq")]
        public string EncSeq { get; set; }

        [YamlMember("isFixedBGM")]
        public bool IsFixedBGM { get; set; }

        [YamlMember("bgmEvent")]
        public string BGMEvent { get; set; }

        [YamlMember("isFixedBtlBg")]
        public bool IsFixedBtlBg { get; set; }

        [YamlMember("btlBg")]
        public int BtlBg { get; set; }

        [YamlMember("isFixedSetupEffect")]
        public bool IsFixedSetupEffect { get; set; }

        [YamlMember("setupEffect")]
        public int SetupEffect { get; set; }

        [YamlMember("waza1")]
        public int Waza1 { get; set; }

        [YamlMember("waza2")]
        public int Waza2 { get; set; }

        [YamlMember("waza3")]
        public int Waza3 { get; set; }

        [YamlMember("waza4")]
        public int Waza4 { get; set; }
    }

    public class Sheetzui
    {
        [YamlMember("zoneID")]
        public int ZoneID { get; set; }

        [YamlMember("form")]
        public bool[] Form { get; set; }
    }
}
