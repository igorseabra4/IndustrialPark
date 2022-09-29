using HipHopFile;
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

    public enum En_dupowavmod
    {
        NPCP_DUPOWAVE_CONTINUOUS = 0,
        NPCP_DUPOWAVE_DISCREET = 1
    }

    public class DynaGObjectNPCSettings : AssetDYNA
    {
        private const string dynaCategoryName = "game_object:NPCSettings";
        public override string TypeString => dynaCategoryName;

        protected override short constVersion => 2;

        [Category(dynaCategoryName)]
        public NpcSettingsBasisType BasisType { get; set; }
        [Category(dynaCategoryName)]
        public bool AllowDetect { get; set; }
        [Category(dynaCategoryName)]
        public bool AllowPatrol { get; set; }
        [Category(dynaCategoryName)]
        public bool AllowWander { get; set; }
        [Category(dynaCategoryName)]
        public bool ReduceCollide { get; set; }
        [Category(dynaCategoryName)]
        public bool UseNavSplines { get; set; }
        [Category(dynaCategoryName)]
        public bool AllowChase { get; set; }
        [Category(dynaCategoryName)]
        public bool AllowAttack { get; set; }
        [Category(dynaCategoryName)]
        public bool AssumeLOS { get; set; }
        [Category(dynaCategoryName)]
        public bool AssumeFOV { get; set; }
        [Category(dynaCategoryName)]
        public En_dupowavmod DuploWaveMode { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle DuploSpawnDelay { get; set; }
        [Category(dynaCategoryName)]
        public int DuploSpawnLifeMax { get; set; }

        public DynaGObjectNPCSettings(string assetName) : base(assetName, DynaType.game_object__NPCSettings)
        {
            AllowDetect = true;
            ReduceCollide = true;
            UseNavSplines = true;
            AllowAttack = true;
            AssumeLOS = true;
            AssumeFOV = true;
            DuploSpawnDelay = 1f;
            DuploSpawnLifeMax = -1;
        }

        public DynaGObjectNPCSettings(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__NPCSettings, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                BasisType = (NpcSettingsBasisType)reader.ReadInt32();
                AllowDetect = reader.ReadByteBool();
                AllowPatrol = reader.ReadByteBool();
                AllowWander = reader.ReadByteBool();
                ReduceCollide = reader.ReadByteBool();
                UseNavSplines = reader.ReadByteBool();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                AllowChase = reader.ReadByteBool();
                AllowAttack = reader.ReadByteBool();
                AssumeLOS = reader.ReadByteBool();
                AssumeFOV = reader.ReadByteBool();
                DuploWaveMode = (En_dupowavmod)reader.ReadInt32();
                DuploSpawnDelay = reader.ReadSingle();
                DuploSpawnLifeMax = reader.ReadInt32();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write((int)BasisType);
            writer.Write(AllowDetect);
            writer.Write(AllowPatrol);
            writer.Write(AllowWander);
            writer.Write(ReduceCollide);
            writer.Write(UseNavSplines);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write(AllowChase);
            writer.Write(AllowAttack);
            writer.Write(AssumeLOS);
            writer.Write(AssumeFOV);
            writer.Write((int)DuploWaveMode);
            writer.Write(DuploSpawnDelay);
            writer.Write(DuploSpawnLifeMax);
        }
    }
}