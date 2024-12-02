using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    public class ItemTable : MonoBehaviour
    {
        [YamlMember("Item", 10)]
        public SheetItem[] Item { get; set; }

        [YamlMember("WazaMachine", 11)]
        public SheetWazaMachine[] WazaMachine { get; set; }
    }

    public class SheetItem
    {
        [YamlMember("no", 0)]
        public short No { get; set; }

        [YamlMember("type", 1)]
        public byte Type { get; set; }

        [YamlMember("iconid", 2)]
        public int IconID { get; set; }

        [YamlMember("price", 3)]
        public int Price { get; set; }

        [YamlMember("bp_price", 4)]
        public int BPPrice { get; set; }

        [YamlMember("eqp", 5)]
        public byte Eqp { get; set; }

        [YamlMember("atc", 6)]
        public byte Atc { get; set; }

        [YamlMember("nage_atc", 7)]
        public byte NageAtc { get; set; }

        [YamlMember("sizen_atc", 8)]
        public byte SizenAtc { get; set; }

        [YamlMember("sizen_type", 9)]
        public byte SizenType { get; set; }

        [YamlMember("tuibamu_eff", 10)]
        public byte TuibamuEff { get; set; }

        [YamlMember("sort", 11)]
        public byte Sort { get; set; }

        [YamlMember("group", 12)]
        public byte Group { get; set; }

        [YamlMember("group_id", 13)]
        public byte GroupID { get; set; }

        [YamlMember("fld_pocket", 14)]
        public byte FldPocket { get; set; }

        [YamlMember("field_func", 15)]
        public byte FieldFunc { get; set; }

        [YamlMember("battle_func", 16)]
        public byte BattleFunc { get; set; }

        [YamlMember("wk_cmn", 17)]
        public byte WkCmn { get; set; }

        [YamlMember("wk_critical_up", 18)]
        public byte WkCriticalUp { get; set; }

        [YamlMember("wk_atc_up", 19)]
        public byte WkAtcUp { get; set; }

        [YamlMember("wk_def_up", 20)]
        public byte WkDefUp { get; set; }

        [YamlMember("wk_agi_up", 21)]
        public byte WkAgiUp { get; set; }

        [YamlMember("wk_hit_up", 22)]
        public byte WkHitUp { get; set; }

        [YamlMember("wk_spa_up", 23)]
        public byte WkSpaUp { get; set; }

        [YamlMember("wk_spd_up", 24)]
        public byte WkSpdUp { get; set; }

        [YamlMember("wk_prm_pp_rcv", 25)]
        public byte WkPrmPPRcv { get; set; }

        [YamlMember("wk_prm_hp_exp", 26)]
        public sbyte WkPrmHPExp { get; set; }

        [YamlMember("wk_prm_pow_exp", 27)]
        public sbyte WkPrmPowExp { get; set; }

        [YamlMember("wk_prm_def_exp", 28)]
        public sbyte WkPrmDefExp { get; set; }

        [YamlMember("wk_prm_agi_exp", 29)]
        public sbyte WkPrmAgiExp { get; set; }

        [YamlMember("wk_prm_spa_exp", 30)]
        public sbyte WkPrmSpaExp { get; set; }

        [YamlMember("wk_prm_spd_exp", 31)]
        public sbyte WkPrmSpdExp { get; set; }

        [YamlMember("wk_friend1", 32)]
        public sbyte WkFriend1 { get; set; }

        [YamlMember("wk_friend2", 33)]
        public sbyte WkFriend2 { get; set; }

        [YamlMember("wk_friend3", 34)]
        public sbyte WkFriend3 { get; set; }

        [YamlMember("wk_prm_hp_rcv", 35)]
        public byte WkPrmHPRcv { get; set; }

        [YamlMember("flags0", 36)]
        public uint Flags0 { get; set; }
    }

    public class SheetWazaMachine
    {
        [YamlMember("itemNo", 0)]
        public int ItemNo { get; set; }

        [YamlMember("machineNo", 1)]
        public int MachineNo { get; set; }

        [YamlMember("wazaNo", 2)]
        public int WazaNo { get; set; }
    }
}
