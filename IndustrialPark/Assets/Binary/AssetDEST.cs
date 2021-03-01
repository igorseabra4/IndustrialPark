using System.Collections.Generic;
using System.ComponentModel;
using HipHopFile;

namespace IndustrialPark
{
    public class AssetDEST : Asset
    {
        public AssetDEST(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform) { }

        private const string destName = "Destructible";

        public override bool HasReference(uint assetID) =>
            MINF_AssetID == assetID || Model_AssetID == assetID || SHRP_AssetID == assetID || base.HasReference(assetID);
        
        public override void Verify(ref List<string> result)
        {
            Verify(MINF_AssetID, ref result);

            if (MINF_AssetID == 0)
                result.Add("DEST with MINF_AssetID set to 0");
            if (Model_AssetID == 0)
                result.Add("DEST with Model_AssetID set to 0");
            if (SHRP_AssetID == 0)
                result.Add("DEST with SHRP_AssetID set to 0");
        }

        [Category(destName)]
        public AssetID MINF_AssetID
        {
            get => ReadUInt(0x00);
            set => Write(0x00, value);
        }

        [Category(destName)]
        public int UnknownInt_04
        {
            get => ReadInt(0x04);
            set => Write(0x04, value);
        }

        [Category(destName)]
        public int UnknownInt_08
        {
            get => ReadInt(0x08);
            set => Write(0x08, value);
        }

        [Category(destName)]
        public int UnknownInt_0C
        {
            get => ReadInt(0x0C);
            set => Write(0x0C, value);
        }

        [Category(destName)]
        public int UnknownInt_10
        {
            get => ReadInt(0x10);
            set => Write(0x10, value);
        }

        [Category(destName)]
        public int UnknownInt_14
        {
            get => ReadInt(0x14);
            set => Write(0x14, value);
        }

        [Category(destName)]
        public int UnknownInt_18
        {
            get => ReadInt(0x18);
            set => Write(0x18, value);
        }

        [Category(destName)]
        public int UnknownInt_1C
        {
            get => ReadInt(0x1C);
            set => Write(0x1C, value);
        }

        [Category(destName)]
        public int UnknownInt_20
        {
            get => ReadInt(0x20);
            set => Write(0x20, value);
        }

        [Category(destName)]
        public byte UnknownByte_24
        {
            get => ReadByte(0x24);
            set => Write(0x24, value);
        }

        [Category(destName)]
        public byte UnknownByte_25
        {
            get => ReadByte(0x25);
            set => Write(0x25, value);
        }

        [Category(destName)]
        public byte UnknownByte_26
        {
            get => ReadByte(0x26);
            set => Write(0x26, value);
        }

        [Category(destName)]
        public byte UnknownByte_27
        {
            get => ReadByte(0x27);
            set => Write(0x27, value);
        }

        [Category(destName)]
        public int UnknownInt_28
        {
            get => ReadInt(0x28);
            set => Write(0x28, value);
        }

        [Category(destName)]
        public AssetID Model_AssetID
        {
            get => ReadUInt(0x2C);
            set => Write(0x2C, value);
        }

        [Category(destName)]
        public AssetID SHRP_AssetID
        {
            get => ReadUInt(0x30);
            set => Write(0x30, value);
        }

        [Category(destName)]
        public int UnknownInt_34
        {
            get => ReadInt(0x34);
            set => Write(0x34, value);
        }

        [Category(destName)]
        public int UnknownInt_38
        {
            get => ReadInt(0x38);
            set => Write(0x38, value);
        }

        [Category(destName)]
        public int UnknownInt_3C
        {
            get => ReadInt(0x3C);
            set => Write(0x3C, value);
        }

        [Category(destName)]
        public int UnknownInt_40
        {
            get => ReadInt(0x40);
            set => Write(0x40, value);
        }

        [Category(destName)]
        public int UnknownInt_44
        {
            get => ReadInt(0x44);
            set => Write(0x44, value);
        }

        [Category(destName)]
        public int UnknownInt_48
        {
            get => ReadInt(0x48);
            set => Write(0x48, value);
        }

        [Category(destName)]
        public int UnknownInt_4C
        {
            get => ReadInt(0x4C);
            set => Write(0x4C, value);
        }

        [Category(destName)]
        public int UnknownInt_50
        {
            get => ReadInt(0x50);
            set => Write(0x50, value);
        }

        [Category(destName)]
        public int UnknownInt_54
        {
            get => ReadInt(0x54);
            set => Write(0x54, value);
        }

        [Category(destName)]
        public int UnknownInt_58
        {
            get => ReadInt(0x58);
            set => Write(0x58, value);
        }

        [Category(destName)]
        public AssetID UnknownInt_5C
        {
            get => ReadUInt(0x5C);
            set => Write(0x5C, value);
        }
    }
}