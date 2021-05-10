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
        public AssetSingle UnknownFloat54 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat58 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat5C { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat60 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat64 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat68 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat6C { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat70 { get; set; }
        [Category(categoryName)]
        public short UnknownShort74 { get; set; }
        [Category(categoryName)]
        public short UnknownShort76 { get; set; }
        [Category(categoryName)]
        public short UnknownShort78 { get; set; }
        [Category(categoryName)]
        public short UnknownShort7A { get; set; }
        [Category(categoryName)]
        public byte UnknownByte7C { get; set; }
        [Category(categoryName)]
        public byte UnknownByte7D { get; set; }
        [Category(categoryName)]
        public byte UnknownByte7E { get; set; }
        [Category(categoryName)]
        public byte UnknownByte7F { get; set; }
        [Category(categoryName)]
        public byte UnknownByte80 { get; set; }
        [Category(categoryName)]
        public byte UnknownByte81 { get; set; }
        [Category(categoryName)]
        public byte UnknownByte82 { get; set; }
        [Category(categoryName)]
        public byte UnknownByte83 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat84 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat88 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat8C { get; set; }
        [Category(categoryName)]
        public int UnknownInt90 { get; set; }
        [Category(categoryName)]
        public int UnknownInt94 { get; set; }
        [Category(categoryName)]
        public int UnknownInt98 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat9C { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloatA0 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloatA4 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloatA8 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloatAC { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloatB0 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloatB4 { get; set; }
        [Category(categoryName)]
        public AssetID MovePoint_AssetID { get; set; }
        [Category(categoryName)]
        public AssetID UnknownAssetID_BC { get; set; }
        [Category(categoryName)]
        public int UnknownIntC0 { get; set; }
        [Category(categoryName)]
        public int UnknownIntC4 { get; set; }

        public AssetNPC(string assetName, Vector3 position) : base(assetName, AssetType.NPC, BaseAssetType.NPC, position)
        {
        }
        public AssetNPC(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            var reader = new EndianBinaryReader(AHDR.data, endianness);
            reader.BaseStream.Position = entityHeaderEndPosition;

            UnknownFloat54 = reader.ReadSingle();
            UnknownFloat58 = reader.ReadSingle();
            UnknownFloat5C = reader.ReadSingle();
            UnknownFloat60 = reader.ReadSingle();
            UnknownFloat64 = reader.ReadSingle();
            UnknownFloat68 = reader.ReadSingle();
            UnknownFloat6C = reader.ReadSingle();
            UnknownFloat70 = reader.ReadSingle();
            UnknownShort74 = reader.ReadInt16();
            UnknownShort76 = reader.ReadInt16();
            UnknownShort78 = reader.ReadInt16();
            UnknownShort7A = reader.ReadInt16();
            UnknownByte7C = reader.ReadByte();
            UnknownByte7D = reader.ReadByte();
            UnknownByte7E = reader.ReadByte();
            UnknownByte7F = reader.ReadByte();
            UnknownByte80 = reader.ReadByte();
            UnknownByte81 = reader.ReadByte();
            UnknownByte82 = reader.ReadByte();
            UnknownByte83 = reader.ReadByte();
            UnknownFloat84 = reader.ReadSingle();
            UnknownFloat88 = reader.ReadSingle();
            UnknownFloat8C = reader.ReadSingle();
            UnknownInt90 = reader.ReadInt32();
            UnknownInt94 = reader.ReadInt32();
            UnknownInt98 = reader.ReadInt32();
            UnknownFloat9C = reader.ReadSingle();
            UnknownFloatA0 = reader.ReadSingle();
            UnknownFloatA4 = reader.ReadSingle();
            UnknownFloatA8 = reader.ReadSingle();
            UnknownFloatAC = reader.ReadSingle();
            UnknownFloatB0 = reader.ReadSingle();
            UnknownFloatB4 = reader.ReadSingle();
            MovePoint_AssetID = reader.ReadUInt32();
            UnknownAssetID_BC = reader.ReadUInt32();
            UnknownIntC0 = reader.ReadInt32();
            UnknownIntC4 = reader.ReadInt32();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);
            writer.Write(SerializeEntity(game, endianness));

            writer.Write(UnknownFloat54);
            writer.Write(UnknownFloat58);
            writer.Write(UnknownFloat5C);
            writer.Write(UnknownFloat60);
            writer.Write(UnknownFloat64);
            writer.Write(UnknownFloat68);
            writer.Write(UnknownFloat6C);
            writer.Write(UnknownFloat70);
            writer.Write(UnknownShort74);
            writer.Write(UnknownShort76);
            writer.Write(UnknownShort78);
            writer.Write(UnknownShort7A);
            writer.Write(UnknownByte7C);
            writer.Write(UnknownByte7D);
            writer.Write(UnknownByte7E);
            writer.Write(UnknownByte7F);
            writer.Write(UnknownByte80);
            writer.Write(UnknownByte81);
            writer.Write(UnknownByte82);
            writer.Write(UnknownByte83);
            writer.Write(UnknownFloat84);
            writer.Write(UnknownFloat88);
            writer.Write(UnknownFloat8C);
            writer.Write(UnknownInt90);
            writer.Write(UnknownInt94);
            writer.Write(UnknownInt98);
            writer.Write(UnknownFloat9C);
            writer.Write(UnknownFloatA0);
            writer.Write(UnknownFloatA4);
            writer.Write(UnknownFloatA8);
            writer.Write(UnknownFloatAC);
            writer.Write(UnknownFloatB0);
            writer.Write(UnknownFloatB4);
            writer.Write(MovePoint_AssetID);
            writer.Write(UnknownAssetID_BC);
            writer.Write(UnknownIntC0);
            writer.Write(UnknownIntC4);

            writer.Write(SerializeLinks(endianness));
            return writer.ToArray();
        }

        public static bool dontRender = false;

        public override bool DontRender => dontRender;

        public override bool HasReference(uint assetID) =>MovePoint_AssetID == assetID || UnknownAssetID_BC == assetID || base.HasReference(assetID);
        
        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Verify(MovePoint_AssetID, ref result);
            Verify(UnknownAssetID_BC, ref result);
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