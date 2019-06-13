using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetPARE : ObjectAsset
    {
        public AssetPARE(Section_AHDR AHDR) : base(AHDR) { }

        protected override int EventStartOffset => 0x54;

        public override bool HasReference(uint assetID) => PARP_AssetID == assetID || Emitter_AssetID == assetID || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            if (PARP_AssetID == 0)
                result.Add("PARE with PARP_AssetID set to 0");
            Verify(PARP_AssetID, ref result);
            Verify(Emitter_AssetID, ref result);
        }

        [Category("Particle Emitter")]
        public byte EmitterFlags
        {
            get => ReadByte(0x8);
            set => Write(0x8, value);
        }

        [Category("Particle Emitter")]
        public byte EmitterType
        {
            get => ReadByte(0x9);
            set => Write(0x9, value);
        }

        [Category("Particle Emitter")]
        public byte Padding0A
        {
            get => ReadByte(0xA);
            set => Write(0xA, value);
        }

        [Category("Particle Emitter")]
        public byte Padding0B
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
        public float E_circle
        {
            get => ReadFloat(0x10);
            set => Write(0x10, value);
        }

        [Category("Particle Emitter"), TypeConverter(typeof(FloatTypeConverter))]
        public float E_sphere
        {
            get => ReadFloat(0x14);
            set => Write(0x14, value);
        }

        [Category("Particle Emitter"), TypeConverter(typeof(FloatTypeConverter))]
        public float E_rectangle
        {
            get => ReadFloat(0x18);
            set => Write(0x18, value);
        }

        [Category("Particle Emitter"), TypeConverter(typeof(FloatTypeConverter))]
        public float E_line
        {
            get => ReadFloat(0x1C);
            set => Write(0x1C, value);
        }

        [Category("Particle Emitter"), TypeConverter(typeof(FloatTypeConverter))]
        public float E_volume
        {
            get => ReadFloat(0x20);
            set => Write(0x20, value);
        }

        [Category("Particle Emitter"), TypeConverter(typeof(FloatTypeConverter))]
        public float E_offsetp
        {
            get => ReadFloat(0x24);
            set => Write(0x24, value);
        }

        [Category("Particle Emitter"), TypeConverter(typeof(FloatTypeConverter))]
        public float E_vcyl
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
        public float Emitter_PosX
        {
            get => ReadFloat(0x30);
            set => Write(0x30, value);
        }

        [Category("Particle Emitter"), TypeConverter(typeof(FloatTypeConverter))]
        public float Emitter_PosY
        {
            get => ReadFloat(0x34);
            set => Write(0x34, value);
        }

        [Category("Particle Emitter"), TypeConverter(typeof(FloatTypeConverter))]
        public float Emitter_PosZ
        {
            get => ReadFloat(0x38);
            set => Write(0x38, value);
        }

        [Category("Particle Emitter"), TypeConverter(typeof(FloatTypeConverter))]
        public float Velocity_X
        {
            get => ReadFloat(0x3C);
            set => Write(0x3C, value);
        }

        [Category("Particle Emitter"), TypeConverter(typeof(FloatTypeConverter))]
        public float Velocity_Y
        {
            get => ReadFloat(0x40);
            set => Write(0x40, value);
        }

        [Category("Particle Emitter"), TypeConverter(typeof(FloatTypeConverter))]
        public float Velocity_Z
        {
            get => ReadFloat(0x44);
            set => Write(0x44, value);
        }

        [Category("Particle Emitter"), TypeConverter(typeof(FloatTypeConverter))]
        public float Velocity_AngleVariation
        {
            get => ReadFloat(0x48);
            set => Write(0x48, value);
        }

        [Category("Particle Emitter")]
        public int CullMode
        {
            get => ReadInt(0x4C);
            set => Write(0x4C, value);
        }

        [Category("Particle Emitter"), TypeConverter(typeof(FloatTypeConverter))]
        public float CullDistanceSqr
        {
            get => ReadFloat(0x50);
            set => Write(0x50, value);
        }
    }
}