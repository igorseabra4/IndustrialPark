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

        protected override int constVersion => 2;

        [Category(dynaCategoryName)]
        public NpcSettingsBasisType BasisType { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte AllowDetect { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte AllowPatrol { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte AllowWander { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte ReduceCollide { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte UseNavSplines { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte AllowChase { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte AllowAttack { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte AssumeLOS { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte AssumeFOV { get; set; }
        [Category(dynaCategoryName)]
        public En_dupowavmod DuploWaveMode { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle DuploSpawnDelay { get; set; }
        [Category(dynaCategoryName)]
        public int DuploSpawnLifeMax { get; set; }

        public DynaGObjectNPCSettings(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, DynaType.game_object__NPCSettings, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = dynaDataStartPosition;

            BasisType = (NpcSettingsBasisType)reader.ReadInt32();
            AllowDetect = reader.ReadByte();
            AllowPatrol = reader.ReadByte();
            AllowWander = reader.ReadByte();
            ReduceCollide = reader.ReadByte();
            UseNavSplines = reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            AllowChase = reader.ReadByte();
            AllowAttack = reader.ReadByte();
            AssumeLOS = reader.ReadByte();
            AssumeFOV = reader.ReadByte();
            DuploWaveMode = (En_dupowavmod)reader.ReadInt32();
            DuploSpawnDelay = reader.ReadSingle();
            DuploSpawnLifeMax = reader.ReadInt32();
        }

        protected override byte[] SerializeDyna(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

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

            return writer.ToArray();
        }
    }
}