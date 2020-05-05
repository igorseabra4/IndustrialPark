using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetPRJT : BaseAsset
    {
        public AssetPRJT(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform) { }

        protected override int EventStartOffset => 0x44;

        public override bool HasReference(uint assetID) => Model_AssetID == assetID || base.HasReference(assetID);
        
        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            if (Model_AssetID == 0)
                result.Add("PRJT with Model_AssetID set to 0");
            Verify(Model_AssetID, ref result);
        }

        [Category("Projectile")]
        public int UnknownInt08
        {
            get => ReadInt(0x8);
            set => Write(0x8, value);
        }

        [Category("Projectile")]
        public AssetID Model_AssetID
        {
            get => ReadUInt(0xC);
            set => Write(0xC, value);
        }

        [Category("Projectile")]
        public int UnknownInt10
        {
            get => ReadInt(0x10);
            set => Write(0x10, value);
        }

        [Category("Projectile")]
        public int UnknownInt14
        {
            get => ReadInt(0x14);
            set => Write(0x14, value);
        }

        [Category("Projectile")]
        public int UnknownInt18
        {
            get => ReadInt(0x18);
            set => Write(0x18, value);
        }

        [Category("Projectile")]
        public int UnknownInt1C
        {
            get => ReadInt(0x1C);
            set => Write(0x1C, value);
        }

        [Category("Projectile"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat20
        {
            get => ReadFloat(0x20);
            set => Write(0x20, value);
        }

        [Category("Projectile"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat24
        {
            get => ReadFloat(0x24);
            set => Write(0x24, value);
        }

        [Category("Projectile")]
        public int UnknownInt28
        {
            get => ReadInt(0x28);
            set => Write(0x28, value);
        }

        [Category("Projectile")]
        public int UnknownInt2C
        {
            get => ReadInt(0x2C);
            set => Write(0x2C, value);
        }

        [Category("Projectile")]
        public int UnknownInt30
        {
            get => ReadInt(0x30);
            set => Write(0x30, value);
        }

        [Category("Projectile")]
        public int UnknownInt34
        {
            get => ReadInt(0x34);
            set => Write(0x34, value);
        }

        [Category("Projectile")]
        public int UnknownInt38
        {
            get => ReadInt(0x38);
            set => Write(0x38, value);
        }

        [Category("Projectile")]
        public int UnknownInt3C
        {
            get => ReadInt(0x3C);
            set => Write(0x3C, value);
        }

        [Category("Projectile")]
        public int UnknownInt40
        {
            get => ReadInt(0x40);
            set => Write(0x40, value);
        }
    }
}