using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public enum NPCType
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
        private const string categoryName = "NPC";

        [Category(categoryName)]
        public NPCType NPCCType
        {
            get => (NPCType)(byte)TypeFlag;
            set => TypeFlag = (byte)value;
        }
        [Category(categoryName)]
        public AssetSingle ActivateRadius { get; set; }
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
        public AssetID ProjectileType_AssetID { get; set; }
        [Category(categoryName)]
        public AssetID Bullseye_MovePoint_AssetID { get; set; }
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
        public AssetID MovePoint_AssetID { get; set; }
        [Category(categoryName)]
        public AssetID Path_AssetID { get; set; }
        [Category(categoryName)]
        public int MinPlayerPowerups { get; set; }
        [Category(categoryName)]
        public int MinGameDifficulty { get; set; }

        public AssetNPC(string assetName, Vector3 position) : base(assetName, AssetType.NPC, BaseAssetType.NPC, position)
        {
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
                ProjectileType_AssetID = reader.ReadUInt32();
                Bullseye_MovePoint_AssetID = reader.ReadUInt32();
                LobArcness = reader.ReadSingle();
                LobHeavy = reader.ReadSingle();
                ExtenderRange = reader.ReadSingle();
                ExtenderWidth = reader.ReadSingle();
                ExtenderDuration = reader.ReadSingle();
                ExtenderRate = reader.ReadSingle();
                ExtenderReloadTime = reader.ReadSingle();
                MovePoint_AssetID = reader.ReadUInt32();
                Path_AssetID = reader.ReadUInt32();
                MinPlayerPowerups = reader.ReadInt32();
                MinGameDifficulty = reader.ReadInt32();
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeEntity(game, endianness));
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
                writer.Write(ProjectileType_AssetID);
                writer.Write(Bullseye_MovePoint_AssetID);
                writer.Write(LobArcness);
                writer.Write(LobHeavy);
                writer.Write(ExtenderRange);
                writer.Write(ExtenderWidth);
                writer.Write(ExtenderDuration);
                writer.Write(ExtenderRate);
                writer.Write(ExtenderReloadTime);
                writer.Write(MovePoint_AssetID);
                writer.Write(Path_AssetID);
                writer.Write(MinPlayerPowerups);
                writer.Write(MinGameDifficulty);
                writer.Write(SerializeLinks(endianness));
                return writer.ToArray();
            }
        }

        public static bool dontRender = false;

        public override bool DontRender => dontRender;

        public override bool HasReference(uint assetID) =>
            ProjectileType_AssetID == assetID ||
            Bullseye_MovePoint_AssetID == assetID ||
            MovePoint_AssetID == assetID ||
            Path_AssetID == assetID ||
            base.HasReference(assetID);
     
        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Verify(ProjectileType_AssetID, ref result);
            Verify(Bullseye_MovePoint_AssetID, ref result);
            Verify(MovePoint_AssetID, ref result);
            Verify(Path_AssetID, ref result);
        }

        public override void Draw(SharpRenderer renderer)
        {
            Vector4 Color = _color;
            Color.W = Color.W == 0f ? 1f : Color.W;

            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(_modelAssetID))
                ArchiveEditorFunctions.renderingDictionary[_modelAssetID].Draw(renderer, LocalWorld(), isSelected ? renderer.selectedObjectColor * Color : Color, UvAnimOffset);
            else
                renderer.DrawCube(LocalWorld(), isSelected);
        }
    }
}