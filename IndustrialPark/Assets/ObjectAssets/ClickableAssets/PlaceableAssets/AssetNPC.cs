using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetNPC : PlaceableAsset
    {
        public static bool dontRender = false;

        protected override bool DontRender => dontRender;

        protected override int EventStartOffset => 0xC8 + Offset;

        public AssetNPC(Section_AHDR AHDR) : base(AHDR) { }

        public override bool HasReference(uint assetID)
        {
            if (MovePoint_AssetID == assetID || UnknownAssetID_BC == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Verify(MovePoint_AssetID, ref result);
            Verify(UnknownAssetID_BC, ref result);
        }

        public override void Draw(SharpRenderer renderer)
        {
            if (DontRender || isInvisible) return;

            Vector4 Color = _color;
            Color.W = Color.W == 0f ? 1f : Color.W;

            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(_modelAssetID))
                ArchiveEditorFunctions.renderingDictionary[_modelAssetID].Draw(renderer, world, isSelected ? renderer.selectedObjectColor * Color : Color);
            else
                renderer.DrawCube(world, isSelected);
        }

        [Category("NPC"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat54
        {
            get => ReadFloat(0x54 + Offset);
            set => Write(0x54 + Offset, value);
        }

        [Category("NPC"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat58
        {
            get => ReadFloat(0x58 + Offset);
            set => Write(0x58 + Offset, value);
        }

        [Category("NPC"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat5C
        {
            get => ReadFloat(0x5C + Offset);
            set => Write(0x5C + Offset, value);
        }

        [Category("NPC"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat60
        {
            get => ReadFloat(0x60 + Offset);
            set => Write(0x60 + Offset, value);
        }

        [Category("NPC"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat64
        {
            get => ReadFloat(0x64 + Offset);
            set => Write(0x64 + Offset, value);
        }

        [Category("NPC"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat68
        {
            get => ReadFloat(0x68 + Offset);
            set => Write(0x68 + Offset, value);
        }

        [Category("NPC"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat6C
        {
            get => ReadFloat(0x6C + Offset);
            set => Write(0x5C + Offset, value);
        }

        [Category("NPC"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat70
        {
            get => ReadFloat(0x70 + Offset);
            set => Write(0x70 + Offset, value);
        }

        [Category("NPC")]
        public short UnknownShort74
        {
            get => ReadShort(0x74 + Offset);
            set => Write(0x74 + Offset, value);
        }

        [Category("NPC")]
        public short UnknownShort76
        {
            get => ReadShort(0x76 + Offset);
            set => Write(0x76 + Offset, value);
        }

        [Category("NPC")]
        public short UnknownShort78
        {
            get => ReadShort(0x78 + Offset);
            set => Write(0x78 + Offset, value);
        }

        [Category("NPC")]
        public short UnknownShort7A
        {
            get => ReadShort(0x7A + Offset);
            set => Write(0x7A + Offset, value);
        }

        [Category("NPC")]
        public byte UnknownByte7C
        {
            get => ReadByte(0x7C + Offset);
            set => Write(0x7C + Offset, value);
        }

        [Category("NPC")]
        public byte UnknownByte7D
        {
            get => ReadByte(0x7D + Offset);
            set => Write(0x7D + Offset, value);
        }

        [Category("NPC")]
        public byte UnknownByte7E
        {
            get => ReadByte(0x7E + Offset);
            set => Write(0x7E + Offset, value);
        }

        [Category("NPC")]
        public byte UnknownByte7F
        {
            get => ReadByte(0x7F + Offset);
            set => Write(0x7F + Offset, value);
        }

        [Category("NPC")]
        public byte UnknownByte80
        {
            get => ReadByte(0x80 + Offset);
            set => Write(0x80 + Offset, value);
        }

        [Category("NPC")]
        public byte UnknownByte81
        {
            get => ReadByte(0x81 + Offset);
            set => Write(0x81 + Offset, value);
        }

        [Category("NPC")]
        public byte UnknownByte82
        {
            get => ReadByte(0x82 + Offset);
            set => Write(0x82 + Offset, value);
        }

        [Category("NPC")]
        public byte UnknownByte83
        {
            get => ReadByte(0x83 + Offset);
            set => Write(0x83 + Offset, value);
        }

        [Category("NPC"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat84
        {
            get => ReadFloat(0x84 + Offset);
            set => Write(0x84 + Offset, value);
        }

        [Category("NPC"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat88
        {
            get => ReadFloat(0x88 + Offset);
            set => Write(0x88 + Offset, value);
        }

        [Category("NPC"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat8C
        {
            get => ReadFloat(0x8C + Offset);
            set => Write(0x8C + Offset, value);
        }

        [Category("NPC")]
        public int UnknownInt90
        {
            get => ReadInt(0x90 + Offset);
            set => Write(0x90 + Offset, value);
        }

        [Category("NPC")]
        public int UnknownInt94
        {
            get => ReadInt(0x94 + Offset);
            set => Write(0x94 + Offset, value);
        }

        [Category("NPC")]
        public int UnknownInt98
        {
            get => ReadInt(0x98 + Offset);
            set => Write(0x98 + Offset, value);
        }

        [Category("NPC"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat9C
        {
            get => ReadFloat(0x9C + Offset);
            set => Write(0x9C + Offset, value);
        }

        [Category("NPC"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloatA0
        {
            get => ReadFloat(0xA0 + Offset);
            set => Write(0xA0 + Offset, value);
        }

        [Category("NPC"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloatA4
        {
            get => ReadFloat(0xA4 + Offset);
            set => Write(0x64 + Offset, value);
        }

        [Category("NPC"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloatA8
        {
            get => ReadFloat(0xA8 + Offset);
            set => Write(0xA8 + Offset, value);
        }

        [Category("NPC"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloatAC
        {
            get => ReadFloat(0xAC + Offset);
            set => Write(0x5C + Offset, value);
        }

        [Category("NPC"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloatB0
        {
            get => ReadFloat(0xB0 + Offset);
            set => Write(0xB0 + Offset, value);
        }

        [Category("NPC"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloatB4
        {
            get => ReadFloat(0xB4 + Offset);
            set => Write(0xB4 + Offset, value);
        }

        [Category("NPC")]
        public AssetID MovePoint_AssetID
        {
            get => ReadUInt(0xB8 + Offset);
            set => Write(0xB8 + Offset, value);
        }

        [Category("NPC")]
        public AssetID UnknownAssetID_BC
        {
            get => ReadUInt(0xBC + Offset);
            set => Write(0xBC + Offset, value);
        }

        [Category("NPC")]
        public int UnknownIntC0
        {
            get => ReadInt(0xC0 + Offset);
            set => Write(0xC0 + Offset, value);
        }

        [Category("NPC")]
        public int UnknownIntC4
        {
            get => ReadInt(0xC4 + Offset);
            set => Write(0xC4 + Offset, value);
        }
    }
}