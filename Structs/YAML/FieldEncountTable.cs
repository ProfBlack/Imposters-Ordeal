using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    public class FieldEncountTable : MonoBehaviour
    {
        [YamlMember("table", 10)]
        public Sheettable[] Table { get; set; }

        [YamlMember("urayama", 11)]
        public Sheeturayama[] Urayama { get; set; }

        [YamlMember("mistu", 12)]
        public Sheetmistu[] Mistu { get; set; }

        [YamlMember("honeytree", 13)]
        public Sheethoneytree[] HoneyTree { get; set; }

        [YamlMember("safari", 14)]
        public Sheetsafari[] Safari { get; set; }

        [YamlMember("mvpoke", 15)]
        public Sheetmvpoke[] MVPoke { get; set; }

        [YamlMember("legendpoke", 16)]
        public Sheetlegendpoke[] LegendPoke { get; set; }

        [YamlMember("zui", 17)]
        public Sheetzui[] Zui { get; set; }
    }

    public class MonsLv
    {
        [YamlMember("maxlv", 0)]
        public int MaxLv { get; set; }

        [YamlMember("minlv", 1)]
        public int MinLv { get; set; }

        [YamlMember("monsNo", 2)]
        public int MonsNo { get; set; }
    }

    public class Sheettable
    {
        [YamlMember("zoneID", 0)]
        public int ZoneID { get; set; }

        [YamlMember("encRate_gr", 1)]
        public int EncRateGr { get; set; }

        [YamlMember("ground_mons", 2)]
        public MonsLv[] GroundMons { get; set; }

        [YamlMember("tairyo", 3)]
        public MonsLv[] Tairyo { get; set; }

        [YamlMember("day", 4)]
        public MonsLv[] Day { get; set; }

        [YamlMember("night", 5)]
        public MonsLv[] Night { get; set; }

        [YamlMember("swayGrass", 6)]
        public MonsLv[] SwayGrass { get; set; }

        [YamlMember("FormProb", 7)]
        public int[] FormProb { get; set; }

        [YamlMember("Nazo", 8)]
        public int[] Nazo { get; set; }

        [YamlMember("AnnoonTable", 9)]
        public int[] AnnoonTable { get; set; }

        [YamlMember("gbaRuby", 10)]
        public MonsLv[] GBARuby { get; set; }

        [YamlMember("gbaSapp", 11)]
        public MonsLv[] GBASapp { get; set; }

        [YamlMember("gbaEme", 12)]
        public MonsLv[] GBAEme { get; set; }

        [YamlMember("gbaFire", 13)]
        public MonsLv[] GBAFire { get; set; }

        [YamlMember("gbaLeaf", 14)]
        public MonsLv[] GBALeaf { get; set; }

        [YamlMember("encRate_wat", 15)]
        public int EncRateWat { get; set; }

        [YamlMember("water_mons", 16)]
        public MonsLv[] WaterMons { get; set; }

        [YamlMember("encRate_turi_boro", 17)]
        public int EncRateTuriBoro { get; set; }

        [YamlMember("boro_mons", 18)]
        public MonsLv[] BoroMons { get; set; }

        [YamlMember("encRate_turi_ii", 19)]
        public int EncRateTuriIi { get; set; }

        [YamlMember("ii_mons", 20)]
        public MonsLv[] IiMons { get; set; }

        [YamlMember("encRate_sugoi", 21)]
        public int EncRateSugoi { get; set; }

        [YamlMember("sugoi_mons", 22)]
        public MonsLv[] SugoiMons { get; set; }
    }

    public class Sheeturayama
    {
        [YamlMember("monsNo", 0)]
        public int MonsNo { get; set; }
    }

    public class Sheetmistu
    {
        [YamlMember("Rate", 0)]
        public int Rate { get; set; }

        [YamlMember("Normal", 1)]
        public int Normal { get; set; }

        [YamlMember("Rare", 2)]
        public int Rare { get; set; }

        [YamlMember("SuperRare", 3)]
        public int SuperRare { get; set; }
    }

    public class Sheethoneytree
    {
        [YamlMember("Normal", 0)]
        public int Normal { get; set; }

        [YamlMember("Rare", 1)]
        public int Rare { get; set; }
    }

    public class Sheetsafari
    {
        [YamlMember("MonsNo", 0)]
        public int MonsNo { get; set; }
    }

    public class Sheetmvpoke
    {
        [YamlMember("zoneID", 0)]
        public int ZoneID { get; set; }

        [YamlMember("nextCount", 1)]
        public int NextCount { get; set; }

        [YamlMember("nextZoneID", 2)]
        public int[] NextZoneID { get; set; }
    }

    public class Sheetlegendpoke
    {
        [YamlMember("monsNo", 0)]
        public int MonsNo { get; set; }

        [YamlMember("formNo", 1)]
        public int FormNo { get; set; }

        [YamlMember("isFixedEncSeq", 2)]
        public int IsFixedEncSeq { get; set; }

        [YamlMember("encSeq", 3)]
        public string EncSeq { get; set; }

        [YamlMember("isFixedBGM", 4)]
        public int IsFixedBGM { get; set; }

        [YamlMember("bgmEvent", 5)]
        public string BGMEvent { get; set; }

        [YamlMember("isFixedBtlBg", 6)]
        public int IsFixedBtlBg { get; set; }

        [YamlMember("btlBg", 7)]
        public int BtlBg { get; set; }

        [YamlMember("isFixedSetupEffect", 8)]
        public int IsFixedSetupEffect { get; set; }

        [YamlMember("setupEffect", 9)]
        public int SetupEffect { get; set; }

        [YamlMember("waza1", 10)]
        public int Waza1 { get; set; }

        [YamlMember("waza2", 11)]
        public int Waza2 { get; set; }

        [YamlMember("waza3", 12)]
        public int Waza3 { get; set; }

        [YamlMember("waza4", 13)]
        public int Waza4 { get; set; }
    }

    public class Sheetzui
    {
        [YamlMember("zoneID", 0)]
        public int ZoneID { get; set; }

        [YamlMember("form", 1)]
        public int[] Form { get; set; }
    }
}
