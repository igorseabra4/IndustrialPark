using System.ComponentModel;
using HipHopFile;

namespace IndustrialPark
{
    public class AssetVILP : Asset
    {
        public AssetVILP(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public override bool HasReference(uint assetID)
        {
            if (UnknownInt_00 == assetID)
                return true;

            return base.HasReference(assetID);
        }

        [Category("VILP")]
        public AssetID UnknownInt_00
        {
            get => ReadUInt(0x00);
            set => Write(0x00, value);
        }

        [Category("VILP")]
        public int UnknownInt_04
        {
            get => ReadInt(0x04);
            set => Write(0x04, value);
        }

        [Category("VILP")]
        public int UnknownInt_08
        {
            get => ReadInt(0x08);
            set => Write(0x08, value);
        }

        [Category("VILP")]
        public int UnknownInt_0C
        {
            get => ReadInt(0x0C);
            set => Write(0x0C, value);
        }

        [Category("VILP")]
        public int UnknownInt_10
        {
            get => ReadInt(0x10);
            set => Write(0x10, value);
        }

        [Category("VILP")]
        public int UnknownInt_14
        {
            get => ReadInt(0x14);
            set => Write(0x14, value);
        }

        [Category("VILP")]
        public int UnknownInt_18
        {
            get => ReadInt(0x18);
            set => Write(0x18, value);
        }

        [Category("VILP")]
        public int UnknownInt_1C
        {
            get => ReadInt(0x1C);
            set => Write(0x1C, value);
        }

        [Category("VILP")]
        public int UnknownInt_20
        {
            get => ReadInt(0x20);
            set => Write(0x20, value);
        }
    }
}