using HipHopFile;
using System;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetENV : ObjectAsset
    {
        public AssetENV(Section_AHDR AHDR) : base(AHDR) { }

        protected override int EventStartOffset
        {
            get => 0x44;
        }

        public override bool HasReference(uint assetID)
        {
            if (JSP_AssetID == assetID)
                return true;
            if (CAM_AssetID == assetID)
                return true;
            if (LKIT_AssetID_0 == assetID)
                return true;
            if (LKIT_AssetID_1 == assetID)
                return true;
            if (MAPR_AssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        [Category("Environment")]
        public AssetID JSP_AssetID
        {
            get => ReadUInt(0x8);
            set => Write(0x8, value);
        }

        [Category("Environment")]
        public AssetID CAM_AssetID
        {
            get => ReadUInt(0xC);
            set => Write(0xC, value);
        }

        [Category("Environment")]
        public int Unknown10
        {
            get => ReadInt(0x10);
            set => Write(0x10, value);
        }

        [Category("Environment")]
        public int Unknown14
        {
            get => ReadInt(0x14);
            set => Write(0x14, value);
        }

        [Category("Environment")]
        public int Unknown18
        {
            get => ReadInt(0x18);
            set => Write(0x18, value);
        }

        [Category("Environment")]
        public AssetID LKIT_AssetID_0
        {
            get => ReadUInt(0x1C);
            set => Write(0x1C, value);
        }

        [Category("Environment")]
        public AssetID LKIT_AssetID_1
        {
            get => ReadUInt(0x20);
            set => Write(0x20, value);
        }

        [Category("Environment")]
        public int Unknown24
        {
            get => ReadInt(0x24);
            set => Write(0x24, value);
        }

        [Category("Environment")]
        public int Unknown28
        {
            get => ReadInt(0x28);
            set => Write(0x28, value);
        }

        [Category("Environment")]
        public int Unknown2C
        {
            get => ReadInt(0x2C);
            set => Write(0x2C, value);
        }

        [Category("Environment")]
        public int Unknown30
        {
            get => ReadInt(0x30);
            set => Write(0x30, value);
        }

        [Category("Environment")]
        public AssetID MAPR_AssetID
        {
            get => ReadUInt(0x34);
            set => Write(0x34, value);
        }

        [Category("Environment")]
        public int Unknown38
        {
            get => ReadInt(0x38);
            set => Write(0x38, value);
        }

        [Category("Environment")]
        public int Unknown3C
        {
            get => ReadInt(0x3C);
            set => Write(0x3C, value);
        }

        [Category("Environment")]
        public float UnknownFloat40
        {
            get => BitConverter.ToSingle(AHDR.data, 0x40);
            set
            {
                for (int i = 0; i < 4; i++)
                    AHDR.data[0x40 + i] = BitConverter.GetBytes(value)[i];
            }
        }
    }
}