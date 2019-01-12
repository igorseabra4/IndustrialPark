using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetPARE : ObjectAsset
    {
        public AssetPARE(Section_AHDR AHDR) : base(AHDR) { }

        protected override int EventStartOffset => 0x54;

        public override bool HasReference(uint assetID)
        {
            if (PARP_AssetID == assetID)
                return true;
            if (Emitter_AssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        [Category("Particle Emitter")]
        public byte UnknownByte08
        {
            get => ReadByte(0x8);
            set => Write(0x8, value);
        }

        [Category("Particle Emitter")]
        public byte UnknownByte09
        {
            get => ReadByte(0x9);
            set => Write(0x9, value);
        }

        [Category("Particle Emitter")]
        public byte UnknownByte0A
        {
            get => ReadByte(0xA);
            set => Write(0xA, value);
        }

        [Category("Particle Emitter")]
        public byte UnknownByte0B
        {
            get => ReadByte(0xB);
            set => Write(0xB, value);
        }

        [Category("Particle Emitter")]
        public AssetID PARP_AssetID
        {
            get => ReadUInt(0xC);
            set => Write(0xC, value);
        }

        [Category("Particle Emitter"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat10
        {
            get => ReadFloat(0x10);
            set => Write(0x10, value);
        }

        [Category("Particle Emitter"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat14
        {
            get => ReadFloat(0x14);
            set => Write(0x14, value);
        }

        [Category("Particle Emitter"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat18
        {
            get => ReadFloat(0x18);
            set => Write(0x18, value);
        }

        [Category("Particle Emitter"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat1C
        {
            get => ReadFloat(0x1C);
            set => Write(0x1C, value);
        }

        [Category("Particle Emitter"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat20
        {
            get => ReadFloat(0x20);
            set => Write(0x20, value);
        }

        [Category("Particle Emitter"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat24
        {
            get => ReadFloat(0x24);
            set => Write(0x24, value);
        }

        [Category("Particle Emitter"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat28
        {
            get => ReadFloat(0x28);
            set => Write(0x28, value);
        }

        [Category("Particle Emitter")]
        public AssetID Emitter_AssetID
        {
            get => ReadUInt(0x2C);
            set => Write(0x2C, value);
        }

        [Category("Particle Emitter"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat30
        {
            get => ReadFloat(0x30);
            set => Write(0x30, value);
        }

        [Category("Particle Emitter"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat34
        {
            get => ReadFloat(0x34);
            set => Write(0x34, value);
        }

        [Category("Particle Emitter"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat38
        {
            get => ReadFloat(0x38);
            set => Write(0x38, value);
        }

        [Category("Particle Emitter"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat3C
        {
            get => ReadFloat(0x3C);
            set => Write(0x3C, value);
        }

        [Category("Particle Emitter"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat40
        {
            get => ReadFloat(0x40);
            set => Write(0x40, value);
        }

        [Category("Particle Emitter"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat44
        {
            get => ReadFloat(0x44);
            set => Write(0x44, value);
        }

        [Category("Particle Emitter"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat48
        {
            get => ReadFloat(0x48);
            set => Write(0x48, value);
        }

        [Category("Particle Emitter")]
        public int UnknownInt4C
        {
            get => ReadInt(0x4C);
            set => Write(0x4C, value);
        }

        [Category("Particle Emitter"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat50
        {
            get => ReadFloat(0x50);
            set => Write(0x50, value);
        }
    }
}