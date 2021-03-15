using System.ComponentModel;

namespace IndustrialPark
{
    public enum NpcSettingsBasisType : int
    {
        NPCP_BASIS_NONE = 0,
        NPCP_BASIS_EVILROBOT = 1,
        NPCP_BASIS_FRIENDLYROBOT = 2,
        NPCP_BASIS_LOVINGCITIZEN = 3,
        NPCP_BASIS_GRUMPYCITIZEN = 4
    }

    public class DynaGObjectNPCSettings : DynaBase
    {
        public string Note => "Version is always 2";

        public override int StructSize => 0x1C;

        public DynaGObjectNPCSettings(AssetDYNA asset) : base(asset) { }

        public NpcSettingsBasisType BasisType
        {
            get => (NpcSettingsBasisType)ReadInt(0x00);
            set => Write(0x00, (int)value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte AllowDetect
        {
            get => ReadByte(0x04);
            set => Write(0x04, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte AllowPatrol
        {
            get => ReadByte(0x05);
            set => Write(0x05, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte AllowWander
        {
            get => ReadByte(0x06);
            set => Write(0x06, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte ReduceCollide
        {
            get => ReadByte(0x07);
            set => Write(0x07, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte UseNavSplines
        {
            get => ReadByte(0x08);
            set => Write(0x08, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Padding09
        {
            get => ReadByte(0x09);
            set => Write(0x09, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Padding0A
        {
            get => ReadByte(0x0A);
            set => Write(0x0A, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Padding0B
        {
            get => ReadByte(0x0B);
            set => Write(0x0B, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte AllowChase
        {
            get => ReadByte(0x0C);
            set => Write(0x0C, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte AllowAttack
        {
            get => ReadByte(0x0D);
            set => Write(0x0D, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte AssumeLOS
        {
            get => ReadByte(0x0E);
            set => Write(0x0E, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte AssumeFOV
        {
            get => ReadByte(0x0F);
            set => Write(0x0F, value);
        }
        public En_dupowavmod DuploWaveMode
        {
            get => (En_dupowavmod)ReadInt(0x10);
            set => Write(0x10, (int)value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float DuploSpawnDelay
        {
            get => ReadFloat(0x14);
            set => Write(0x14, value);
        }
        public int DuploSpawnLifeMax
        {
            get => ReadInt(0x18);
            set => Write(0x18, value);
        }

        public enum En_dupowavmod
        {
            NPCP_DUPOWAVE_CONTINUOUS = 0,
            NPCP_DUPOWAVE_DISCREET = 1
        }
    }
}