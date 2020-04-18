using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class PareSpecific_Generic : AssetSpecific_Generic
    {
        public PareSpecific_Generic(AssetPARE asset) : base(asset, 0x10) { }
    }

    public class PareSpecific_xPECircle : PareSpecific_Generic
    {
        public PareSpecific_xPECircle(AssetPARE asset) : base(asset) { }

        [TypeConverter(typeof(FloatTypeConverter))]
        public float Radius
        {
            get => ReadFloat(0x00);
            set => Write(0x00, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Deflection
        {
            get => ReadFloat(0x04);
            set => Write(0x04, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float DirX
        {
            get => ReadFloat(0x08);
            set => Write(0x08, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float DirY
        {
            get => ReadFloat(0x0C);
            set => Write(0x0C, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float DirZ
        {
            get => ReadFloat(0x10);
            set => Write(0x10, value);
        }
    }

    public class PareSpecific_tagEmitSphere : PareSpecific_Generic
    {
        public PareSpecific_tagEmitSphere(AssetPARE asset) : base(asset) { }

        [TypeConverter(typeof(FloatTypeConverter))]
        public float Radius
        {
            get => ReadFloat(0x00);
            set => Write(0x00, value);
        }
    }

    public class PareSpecific_tagEmitRect : PareSpecific_Generic
    {
        public PareSpecific_tagEmitRect(AssetPARE asset) : base(asset) { }

        [TypeConverter(typeof(FloatTypeConverter))]
        public float X_Len
        {
            get => ReadFloat(0x00);
            set => Write(0x00, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Z_Len
        {
            get => ReadFloat(0x04);
            set => Write(0x04, value);
        }
    }

    public class PareSpecific_tagEmitLine : PareSpecific_Generic
    {
        public PareSpecific_tagEmitLine(AssetPARE asset) : base(asset) { }

        [TypeConverter(typeof(FloatTypeConverter))]
        public float Position_0_X
        {
            get => ReadFloat(0x00);
            set => Write(0x00, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Position_0_Y
        {
            get => ReadFloat(0x04);
            set => Write(0x04, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Position_0_Z
        {
            get => ReadFloat(0x08);
            set => Write(0x08, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Position_1_X
        {
            get => ReadFloat(0x0C);
            set => Write(0x0C, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Position_1_Y
        {
            get => ReadFloat(0x10);
            set => Write(0x10, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Position_1_Z
        {
            get => ReadFloat(0x14);
            set => Write(0x14, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Radius
        {
            get => ReadFloat(0x18);
            set => Write(0x18, value);
        }
    }

    public class PareSpecific_tagEmitVolume : PareSpecific_Generic
    {
        public PareSpecific_tagEmitVolume(AssetPARE asset) : base(asset) { }

        public AssetID VolumeAssetID
        {
            get => ReadUInt(0x00);
            set => Write(0x00, value);
        }

        public override bool HasReference(uint assetID) => VolumeAssetID == assetID || base.HasReference(assetID);
        public override void Verify(ref List<string> result)
        {
            Asset.Verify(VolumeAssetID, ref result);
            base.Verify(ref result);
        }
    }

    public class PareSpecific_tagEmitOffsetPoint : PareSpecific_Generic
    {
        public PareSpecific_tagEmitOffsetPoint(AssetPARE asset) : base(asset) { }

        [TypeConverter(typeof(FloatTypeConverter))]
        public float Position_X
        {
            get => ReadFloat(0x00);
            set => Write(0x00, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Position_Y
        {
            get => ReadFloat(0x04);
            set => Write(0x04, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Position_Z
        {
            get => ReadFloat(0x08);
            set => Write(0x08, value);
        }
    }

    public class PareSpecific_xPEVCyl : PareSpecific_Generic
    {
        public PareSpecific_xPEVCyl(AssetPARE asset) : base(asset) { }

        [TypeConverter(typeof(FloatTypeConverter))]
        public float Height
        {
            get => ReadFloat(0x00);
            set => Write(0x00, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Radius
        {
            get => ReadFloat(0x04);
            set => Write(0x04, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Deflection
        {
            get => ReadFloat(0x08);
            set => Write(0x08, value);
        }
    }

    public class PareSpecific_xPEEntBone : PareSpecific_Generic
    {
        public PareSpecific_xPEEntBone(AssetPARE asset) : base(asset) { }

        public byte flags
        {
            get => ReadByte(0x00);
            set => Write(0x00, value);
        }

        public byte type
        {
            get => ReadByte(0x01);
            set => Write(0x01, value);
        }

        public byte bone
        {
            get => ReadByte(0x02);
            set => Write(0x02, value);
        }

        public byte pad1
        {
            get => ReadByte(0x03);
            set => Write(0x03, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float OffsetX
        {
            get => ReadFloat(0x04);
            set => Write(0x04, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float OffsetY
        {
            get => ReadFloat(0x08);
            set => Write(0x08, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float OffsetZ
        {
            get => ReadFloat(0x0C);
            set => Write(0x0C, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Radius
        {
            get => ReadFloat(0x10);
            set => Write(0x10, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Deflection
        {
            get => ReadFloat(0x14);
            set => Write(0x14, value);
        }
    }

    public class PareSpecific_xPEEntBound : PareSpecific_Generic
    {
        public PareSpecific_xPEEntBound(AssetPARE asset) : base(asset) { }

        public byte flags
        {
            get => ReadByte(0x00);
            set => Write(0x00, value);
        }

        public byte type
        {
            get => ReadByte(0x01);
            set => Write(0x01, value);
        }

        public byte pad1
        {
            get => ReadByte(0x02);
            set => Write(0x02, value);
        }

        public byte pad2
        {
            get => ReadByte(0x03);
            set => Write(0x03, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Expand
        {
            get => ReadFloat(0x04);
            set => Write(0x04, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Deflection
        {
            get => ReadFloat(0x08);
            set => Write(0x08, value);
        }
    }
}
