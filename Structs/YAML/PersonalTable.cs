using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    public class PersonalTable : MonoBehaviour
    {
        [YamlMember("Personal", 10)]
        public SheetPersonal[] Personal { get; set; }
    }

    public class SheetPersonal
    {
        [YamlMember("valid_flag", 0)]
        public byte ValidFlag { get; set; }

        [YamlMember("id", 1)]
        public ushort ID { get; set; }

        [YamlMember("monsno", 2)]
        public ushort Monsno { get; set; }

        [YamlMember("form_index", 3)]
        public ushort FormIndex { get; set; }

        [YamlMember("form_max", 4)]
        public byte FormMax { get; set; }

        [YamlMember("color", 5)]
        public byte Color { get; set; }

        [YamlMember("gra_no", 6)]
        public ushort GraNo { get; set; }

        [YamlMember("basic_hp", 7)]
        public byte BasicHP { get; set; }

        [YamlMember("basic_atk", 8)]
        public byte BasicAtk { get; set; }

        [YamlMember("basic_def", 9)]
        public byte BasicDef { get; set; }

        [YamlMember("basic_agi", 10)]
        public byte BasicAgi { get; set; }

        [YamlMember("basic_spatk", 11)]
        public byte BasicSpatk { get; set; }

        [YamlMember("basic_spdef", 12)]
        public byte BasicSpdef { get; set; }

        [YamlMember("type1", 13)]
        public byte Type1 { get; set; }

        [YamlMember("type2", 14)]
        public byte Type2 { get; set; }

        [YamlMember("get_rate", 15)]
        public byte GetRate { get; set; }

        [YamlMember("rank", 16)]
        public byte Rank { get; set; }

        [YamlMember("exp_value", 17)]
        public ushort ExpValue { get; set; }

        [YamlMember("item1", 18)]
        public ushort Item1 { get; set; }

        [YamlMember("item2", 19)]
        public ushort Item2 { get; set; }

        [YamlMember("item3", 20)]
        public ushort Item3 { get; set; }

        [YamlMember("sex", 21)]
        public byte Sex { get; set; }

        [YamlMember("egg_birth", 22)]
        public byte EggBirth { get; set; }

        [YamlMember("initial_friendship", 23)]
        public byte InitialFriendship { get; set; }

        [YamlMember("egg_group1", 24)]
        public byte EggGroup1 { get; set; }

        [YamlMember("egg_group2", 25)]
        public byte EggGroup2 { get; set; }

        [YamlMember("grow", 26)]
        public byte Grow { get; set; }

        [YamlMember("tokusei1", 27)]
        public ushort Tokusei1 { get; set; }

        [YamlMember("tokusei2", 28)]
        public ushort Tokusei2 { get; set; }

        [YamlMember("tokusei3", 29)]
        public ushort Tokusei3 { get; set; }

        [YamlMember("give_exp", 30)]
        public ushort GiveExp { get; set; }

        [YamlMember("height", 31)]
        public ushort Height { get; set; }

        [YamlMember("weight", 32)]
        public ushort Weight { get; set; }

        [YamlMember("chihou_zukan_no", 33)]
        public ushort ChihouZukanNo { get; set; }

        [YamlMember("machine1", 34)]
        public uint Machine1 { get; set; }

        [YamlMember("machine2", 35)]
        public uint Machine2 { get; set; }

        [YamlMember("machine3", 36)]
        public uint Machine3 { get; set; }

        [YamlMember("machine4", 37)]
        public uint Machine4 { get; set; }

        [YamlMember("hiden_machine", 38)]
        public uint HidenMachine { get; set; }

        [YamlMember("egg_monsno", 39)]
        public ushort EggMonsno { get; set; }

        [YamlMember("egg_formno", 40)]
        public ushort EggFormno { get; set; }

        [YamlMember("egg_formno_kawarazunoishi", 41)]
        public ushort EggFormnoKawarazunoishi { get; set; }

        [YamlMember("egg_form_inherit_kawarazunoishi", 42)]
        public byte EggFormInheritKawarazunoishi { get; set; }
    }
}
