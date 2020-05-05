using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetEGEN : EntityAsset
    {
        public static bool dontRender = false;

        protected override bool DontRender => dontRender;
        
        public AssetEGEN(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform) { }

        protected override int EventStartOffset => 0x6C + Offset;

        public override bool HasReference(uint assetID) => OnAnim_AssetID == assetID || base.HasReference(assetID);
        
        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Verify(OnAnim_AssetID, ref result);
        }

        protected const string categoryName = "Electric Arc";

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float Src_dpos_X
        {
            get => ReadFloat(0x54 + Offset);
            set => Write(0x54 + Offset, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float Src_dpos_Y
        {
            get => ReadFloat(0x58 + Offset);
            set => Write(0x58 + Offset, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float Src_dpos_Z
        {
            get => ReadFloat(0x5C + Offset);
            set => Write(0x5C + Offset, value);
        }

        [Category(categoryName)]
        public byte DamageType
        {
            get => ReadByte(0x60 + Offset);
            set => Write(0x60 + Offset, value);
        }

        [Category(categoryName)]
        public DynamicTypeDescriptor EgenFlags => ByteFlagsDescriptor(0x61 + Offset);

        [Category(categoryName)]
        public byte UnknownByte62
        {
            get => ReadByte(0x62 + Offset);
            set => Write(0x62 + Offset, value);
        }

        [Category(categoryName)]
        public byte UnknownByte63
        {
            get => ReadByte(0x63 + Offset);
            set => Write(0x63 + Offset, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float ActiveTimeSeconds
        {
            get => ReadFloat(0x64 + Offset);
            set => Write(0x64 + Offset, value);
        }

        [Category(categoryName)]
        public AssetID OnAnim_AssetID
        {
            get => ReadUInt(0x68 + Offset);
            set => Write(0x68 + Offset, value);
        }
    }
}