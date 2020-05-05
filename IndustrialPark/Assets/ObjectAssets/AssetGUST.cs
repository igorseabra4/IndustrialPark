using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetGUST : BaseAsset
    {
        public AssetGUST(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform) { }

        protected override int EventStartOffset => 0x28;

        public override bool HasReference(uint assetID) => Volume_AssetID == assetID ||base.HasReference(assetID);
        
        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            if (Volume_AssetID == 0)
                result.Add("GUST with Volume_AssetID set to 0");
            Verify(Volume_AssetID, ref result);
        }

        [Category("Gust")]
        public int UnknownInt08
        {
            get => ReadInt(0x8);
            set => Write(0x8, value);
        }

        [Category("Gust")]
        public AssetID Volume_AssetID
        {
            get => ReadUInt(0xC);
            set => Write(0xC, value);
        }

        [Category("Gust")]
        public int UnknownInt10
        {
            get => ReadInt(0x10);
            set => Write(0x10, value);
        }

        [Category("Gust")]
        public int UnknownInt14
        {
            get => ReadInt(0x14);
            set => Write(0x14, value);
        }

        [Category("Gust"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat18
        {
            get => ReadFloat(0x18);
            set => Write(0x18, value);
        }

        [Category("Gust")]
        public int UnknownInt1C
        {
            get => ReadInt(0x1C);
            set => Write(0x1C, value);
        }

        [Category("Gust"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat20
        {
            get => ReadFloat(0x20);
            set => Write(0x20, value);
        }

        [Category("Gust"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat24
        {
            get => ReadFloat(0x24);
            set => Write(0x24, value);
        }
    }
}