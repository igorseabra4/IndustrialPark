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
        [Category(dynaCategoryName), Description("Whether the NPC can detect the player. Can be set at runtime by events WanderOn, WanderOff, DetectPlayerOn, DetectPlayerOff.")]
        public bool AllowDetect { get; set; }
        [Category(dynaCategoryName), Description("Patrol lets NPCs follow a move point path precisely from point to point. Can be set at runtime by events PatrolOn and PatrolOff.")]
        public bool AllowPatrol { get; set; }
        [Category(dynaCategoryName), Description("Wander lets NPCs and enemies walk randomly within their move point's zone radius. For an enemy to wander, they must reach a move point with a zone bigger than 0, and the delay must also be bigger than 0 for the enemy to remain around this point.")]
        public bool AllowWander { get; set; }
        [Category(dynaCategoryName)]
        public bool ReduceCollide { get; set; }
        [Category(dynaCategoryName), Description("Allows NPCs to follow navigation splines, which are enabled when the move points have UseBezier = True, when patrolling. Can be set at runtime by events NPCPatrolSplineOn and NPCPatrolSplineOff.")]
        public bool UseNavSplines { get; set; }
        [Category(dynaCategoryName), Description("Can be set at runtime by events ChasePlayerOn and ChasePlayerOff. The chase behavior varies depending on the enemy type, but is usually related to when the enemy runs towards the player, circling you to avoid attacks.")]
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

        public DynaGObjectNPCSettings(string assetName, AssetTemplate template) : base(assetName, DynaType.game_object__NPCSettings)
        {
            BasisType = NpcSettingsBasisType.NPCP_BASIS_NONE;
            AllowDetect = true;
            UseNavSplines = true;
            DuploWaveMode = En_dupowavmod.NPCP_DUPOWAVE_CONTINUOUS;
            DuploSpawnLifeMax = -1;

            if (template == AssetTemplate.Duplicatotron_Settings)
            {
                ReduceCollide = true;
                AllowAttack = true;
                AssumeLOS = true;
                AssumeFOV = true;
                DuploSpawnDelay = 1f;
            }
            else if (template == AssetTemplate.NPC_Settings_Object)
            {
                AllowWander = true;
                AllowPatrol = true;
                ReduceCollide = false;
                AllowChase = true;
                AllowAttack = true;
                AssumeLOS = false;
                AssumeFOV = false;
                DuploSpawnDelay = 5f;
            }
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