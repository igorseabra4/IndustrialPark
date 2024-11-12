using HipHopFile;
using SharpDX;
using System.ComponentModel;

namespace IndustrialPark
{
    public enum VillainType
    {
        Bat = 0x00,
        Geronimo = 0x02,
        Creeper = 0x03,
        SeaCreature = 0x04,
        Gargoyle = 0x05,
        Ghost = 0x06,
        GhostDiver = 0x07,
        HeadlessSpecter = 0x09,
        FunlandRobot = 0x0C,
        Scarecrow = 0x0D,
        Shark = 0x0E,
        SpaceKook = 0x0F,
        TarMonster = 0x10,
        Witch = 0x11,
        WitchDoctor = 0x12,
        Wolfman = 0x13,
        Zombie = 0x14,
        Crab = 0x15,
        Rat = 0x16,
        FlyingFish = 0x17,
        Spider = 0x18,
        KillerPlant = 0x1A,
        Shaggy0 = 0x1C,
        Shaggy1 = 0x1D,
        Shaggy4 = 0x20,
        Shaggy5 = 0x21,
        Shaggy8 = 0x24,
        Fred = 0x26,
        Daphne = 0x27,
        Velma = 0x28,
        BlackKnight = 0x29,
        GreenGhost = 0x2A,
        Redbeard = 0x2B,
        Mastermind = 0x2C,
        GhostOfCaptainMoody = 0x2D,
        Caveman = 0x2E,
        Holly = 0x2F,
        Groundskeeper = 0x30
    }

    public class AssetNPC : EntityAsset
    {
        private const string categoryName = "Villain";

        [Category(categoryName)]
        public VillainType VillainType
        {
            get => (VillainType)(byte)TypeFlag;
            set => TypeFlag = (byte)value;
        }
        private float _activateRadius;
        [Category(categoryName)]
        public AssetSingle ActivateRadius
        {
            get => _activateRadius;
            set { _activateRadius = value; CreateTransformMatrix(); }
        }
        [Category(categoryName)]
        public AssetSingle ActivateFOV { get; set; }
        [Category(categoryName)]
        public AssetSingle DetectHeight { get; set; }
        [Category(categoryName)]
        public AssetSingle DetectHeightOffset { get; set; }
        [Category(categoryName)]
        public AssetSingle SpeedMovement { get; set; }
        [Category(categoryName)]
        public AssetSingle SpeedPursue { get; set; }
        [Category(categoryName)]
        public AssetSingle SpeedTurn { get; set; }
        [Category(categoryName)]
        public AssetSingle PursuitRange { get; set; }
        [Category(categoryName)]
        public short DurDazedState { get; set; }
        [Category(categoryName)]
        public short DurGloatState { get; set; }
        [Category(categoryName)]
        public short DurGummedState { get; set; }
        [Category(categoryName)]
        public short DurBubbleState { get; set; }
        [Category(categoryName)]
        public byte Hitpoints { get; set; }
        [Category(categoryName)]
        public byte BehaviorState { get; set; }
        [Category(categoryName)]
        public FlagBitmask Flags { get; set; } = IntFlagsDescriptor();
        [Category(categoryName)]
        public AssetSingle LobSpeed { get; set; }
        [Category(categoryName)]
        public AssetSingle LobDurReload { get; set; }
        [Category(categoryName)]
        public AssetSingle LobRange { get; set; }
        [Category(categoryName)]
        public int LobSalvo { get; set; }
        [Category(categoryName)]
        public AssetID ProjectileType { get; set; }
        [Category(categoryName)]
        public AssetID Bullseye_MovePoint { get; set; }
        [Category(categoryName)]
        public AssetSingle LobArcness { get; set; }
        [Category(categoryName)]
        public AssetSingle LobHeavy { get; set; }
        [Category(categoryName)]
        public AssetSingle ExtenderRange { get; set; }
        [Category(categoryName)]
        public AssetSingle ExtenderWidth { get; set; }
        [Category(categoryName)]
        public AssetSingle ExtenderDuration { get; set; }
        [Category(categoryName)]
        public AssetSingle ExtenderRate { get; set; }
        [Category(categoryName)]
        public AssetSingle ExtenderReloadTime { get; set; }
        [Category(categoryName)]
        public AssetID MovePoint { get; set; }
        [Category(categoryName)]
        public AssetID Path { get; set; }
        [Category(categoryName)]
        public int MinPlayerPowerups { get; set; }
        [Category(categoryName)]
        public int MinGameDifficulty { get; set; }

        public AssetNPC(string assetName, Vector3 position, AssetTemplate template) : base(assetName, AssetType.Villain, BaseAssetType.Villain, position)
        {
            ColorAlpha = 0f;
            ColorAlphaSpeed = 0f;

            ActivateRadius = 5f;
            ActivateFOV = 90f;
            DetectHeight = 2f;
            DetectHeightOffset = 0f;
            SpeedMovement = 2f;
            SpeedPursue = 4f;
            SpeedTurn = 300f;
            PursuitRange = 10f;
            DurDazedState = 5;
            DurGloatState = 3;
            DurGummedState = 5;
            DurBubbleState = 5;
            Hitpoints = 1;
            BehaviorState = 1;
            unchecked
            {
                Flags.FlagValueInt = (uint)((1 << 0) | (1 << 1) | (1 << 2) | (1 << 3) | (1 << 4) | (1 << 5) | (1 << 6) | (1 << 31));
            }
            LobSpeed = 2f;
            LobDurReload = 1f;
            LobRange = 4f;
            LobSalvo = 1;
            LobArcness = -1f;
            LobHeavy = -1f;
            ExtenderRange = 5f;
            ExtenderWidth = 1f;
            ExtenderDuration = 10f;
            ExtenderRate = 2.5f;
            ExtenderReloadTime = 10f;
            Path = 0xCDCDCDCD;

            switch (template)
            {
                case AssetTemplate.Caveman:
                    Model = "cv.MINF";
                    VillainType = VillainType.Caveman;
                    break;
                case AssetTemplate.Creeper:
                    Model = "cr.MINF";
                    VillainType = VillainType.Creeper;
                    break;
                case AssetTemplate.Funland_Robot:
                    Model = "ar.MINF";
                    VillainType = VillainType.FunlandRobot;
                    break;
                case AssetTemplate.Gargoyle:
                    Model = "ga.MINF";
                    VillainType = VillainType.Gargoyle;
                    ProjectileType = "FIREBALL";
                    LobHeavy = 9f;
                    break;
                case AssetTemplate.Geronimo:
                    Model = "bm.MINF";
                    VillainType = VillainType.Geronimo;
                    break;
                case AssetTemplate.Ghost:
                    Model = "gz.MINF";
                    VillainType = VillainType.Ghost;
                    break;
                case AssetTemplate.Ghost_Diver:
                    Model = "gd.MINF";
                    VillainType = VillainType.GhostDiver;
                    break;
                case AssetTemplate.Ghost_of_Captain_Moody:
                    Model = "cm.MINF";
                    VillainType = VillainType.GhostOfCaptainMoody;
                    break;
                case AssetTemplate.Headless_Specter:
                    Model = "hs.MINF";
                    VillainType = VillainType.HeadlessSpecter;
                    break;
                case AssetTemplate.Scarecrow:
                    Model = "sw.MINF";
                    VillainType = VillainType.Scarecrow;
                    break;
                case AssetTemplate.Sea_Creature:
                    Model = "sc.MINF";
                    VillainType = VillainType.SeaCreature;
                    break;
                case AssetTemplate.Space_Kook:
                    Model = "gs.MINF";
                    VillainType = VillainType.SpaceKook;
                    break;
                case AssetTemplate.Tar_Monster:
                    Model = "tm.MINF";
                    VillainType = VillainType.TarMonster;
                    ProjectileType = "TARBALL_PROJ";
                    LobSpeed = 10f;
                    LobDurReload = 1f;
                    LobRange = 10f;
                    LobSalvo = 1;
                    LobArcness = 0.5f;
                    LobHeavy = -4f;
                    break;
                case AssetTemplate.Witch:
                    Model = "wc.MINF";
                    VillainType = VillainType.Witch;
                    break;
                case AssetTemplate.Witch_Doctor:
                    Model = "wd.MINF";
                    VillainType = VillainType.Witch;
                    ProjectileType = "FIREBALL";
                    LobSpeed = 5f;
                    LobDurReload = 2f;
                    LobRange = 10f;
                    LobHeavy = 7f;
                    break;
                case AssetTemplate.Wolfman:
                    Model = "wm.MINF";
                    VillainType = VillainType.Wolfman;
                    break;
                case AssetTemplate.Zombie:
                    Model = "zz.MINF";
                    VillainType = VillainType.Zombie;
                    break;
                case AssetTemplate.Bat:
                    Model = "ba.MINF";
                    VillainType = VillainType.Bat;
                    break;
                case AssetTemplate.Crab:
                    Model = "cb.MINF";
                    VillainType = VillainType.Crab;
                    break;
                case AssetTemplate.Flying_Fish:
                    Model = "ff.MINF";
                    VillainType = VillainType.FlyingFish;
                    break;
                case AssetTemplate.Rat:
                    Model = "rt.MINF";
                    VillainType = VillainType.Rat;
                    break;
                case AssetTemplate.Spider:
                    Model = "sp.MINF";
                    VillainType = VillainType.Spider;
                    break;
                case AssetTemplate.Killer_Plant:
                    Model = "kp.MINF";
                    VillainType = VillainType.KillerPlant;
                    ActivateRadius = 10f;
                    ActivateFOV = 360f;
                    ExtenderRange = 7f;
                    ExtenderWidth = 1.5f;
                    ExtenderDuration = 3f;
                    ExtenderRate = 5f;
                    ExtenderReloadTime = 3f;
                    break;
                case AssetTemplate.Groundskeeper:
                    Model = "gr.MINF";
                    VillainType = VillainType.Groundskeeper;
                    break;
                case AssetTemplate.Holly:
                    Model = "ho.MINF";
                    VillainType = VillainType.Holly;
                    break;
            }
        }

        public AssetNPC(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = entityHeaderEndPosition;

                ActivateRadius = reader.ReadSingle();
                ActivateFOV = reader.ReadSingle();
                DetectHeight = reader.ReadSingle();
                DetectHeightOffset = reader.ReadSingle();
                SpeedMovement = reader.ReadSingle();
                SpeedPursue = reader.ReadSingle();
                SpeedTurn = reader.ReadSingle();
                PursuitRange = reader.ReadSingle();
                DurDazedState = reader.ReadInt16();
                DurGloatState = reader.ReadInt16();
                DurGummedState = reader.ReadInt16();
                DurBubbleState = reader.ReadInt16();
                Hitpoints = reader.ReadByte();
                BehaviorState = reader.ReadByte();
                reader.ReadInt16();
                Flags.FlagValueInt = reader.ReadUInt32();
                LobSpeed = reader.ReadSingle();
                LobDurReload = reader.ReadSingle();
                LobRange = reader.ReadSingle();
                LobSalvo = reader.ReadInt32();
                ProjectileType = reader.ReadUInt32();
                Bullseye_MovePoint = reader.ReadUInt32();
                LobArcness = reader.ReadSingle();
                LobHeavy = reader.ReadSingle();
                ExtenderRange = reader.ReadSingle();
                ExtenderWidth = reader.ReadSingle();
                ExtenderDuration = reader.ReadSingle();
                ExtenderRate = reader.ReadSingle();
                ExtenderReloadTime = reader.ReadSingle();
                MovePoint = reader.ReadUInt32();
                Path = reader.ReadUInt32();
                MinPlayerPowerups = reader.ReadInt32();
                MinGameDifficulty = reader.ReadInt32();
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(ActivateRadius);
            writer.Write(ActivateFOV);
            writer.Write(DetectHeight);
            writer.Write(DetectHeightOffset);
            writer.Write(SpeedMovement);
            writer.Write(SpeedPursue);
            writer.Write(SpeedTurn);
            writer.Write(PursuitRange);
            writer.Write(DurDazedState);
            writer.Write(DurGloatState);
            writer.Write(DurGummedState);
            writer.Write(DurBubbleState);
            writer.Write(Hitpoints);
            writer.Write(BehaviorState);
            writer.Write((short)0);
            writer.Write(Flags.FlagValueInt);
            writer.Write(LobSpeed);
            writer.Write(LobDurReload);
            writer.Write(LobRange);
            writer.Write(LobSalvo);
            writer.Write(ProjectileType);
            writer.Write(Bullseye_MovePoint);
            writer.Write(LobArcness);
            writer.Write(LobHeavy);
            writer.Write(ExtenderRange);
            writer.Write(ExtenderWidth);
            writer.Write(ExtenderDuration);
            writer.Write(ExtenderRate);
            writer.Write(ExtenderReloadTime);
            writer.Write(MovePoint);
            writer.Write(Path);
            writer.Write(MinPlayerPowerups);
            writer.Write(MinGameDifficulty);
            SerializeLinks(writer);
        }

        public static bool dontRender = false;

        public override bool DontRender => dontRender;

        public override void Draw(SharpRenderer renderer)
        {
            Vector4 Color = _color;
            Color.W = Color.W == 0f ? 1f : Color.W;

            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(_model))
                ArchiveEditorFunctions.renderingDictionary[_model].Draw(renderer, LocalWorld(), Color, UvAnimOffset, isSelected);
            else
                renderer.DrawCube(LocalWorld(), isSelected);

            if (isSelected)
                renderer.DrawSphere(radiusMatrix, false, renderer.mvptColor);
        }

        private Matrix radiusMatrix;
        public override void CreateTransformMatrix()
        {
            radiusMatrix = Matrix.Scaling(ActivateRadius) * Matrix.Translation(_position);
            base.CreateTransformMatrix();
        }
    }
}