using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaGObjectBungeeHook : DynaBase
    {
        public string Note => "Version is always 13";

        public override int StructSize => 0x7C;

        public DynaGObjectBungeeHook(AssetDYNA asset) : base(asset) { }

        public override bool HasReference(uint assetID) => Placeable_AssetID == assetID || base.HasReference(assetID);
        
        public override void Verify(ref List<string> result)
        {
            Asset.Verify(Placeable_AssetID, ref result);
        }

        public AssetID Placeable_AssetID
        {
            get => ReadUInt(0x00);
            set => Write(0x00, value);
        }
        public int EnterX
        {
            get => ReadInt(0x04);
            set => Write(0x04, value);
        }
        public int EnterY
        {
            get => ReadInt(0x08);
            set => Write(0x08, value);
        }
        public int EnterZ
        {
            get => ReadInt(0x0C);
            set => Write(0x0C, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float AttachDist
        {
            get => ReadFloat(0x10);
            set => Write(0x10, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float AttachTravelTime
        {
            get => ReadFloat(0x14);
            set => Write(0x14, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float DetachDist
        {
            get => ReadFloat(0x18);
            set => Write(0x18, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float DetachFreeFallTime
        {
            get => ReadFloat(0x1C);
            set => Write(0x1C, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float DetachAccel
        {
            get => ReadFloat(0x20);
            set => Write(0x20, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float TurnUnused1
        {
            get => ReadFloat(0x24);
            set => Write(0x24, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float TurnUnused2
        {
            get => ReadFloat(0x28);
            set => Write(0x28, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float VerticalFrequency
        {
            get => ReadFloat(0x2C);
            set => Write(0x2C, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float VerticalGravity
        {
            get => ReadFloat(0x30);
            set => Write(0x30, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float VerticalDive
        {
            get => ReadFloat(0x34);
            set => Write(0x34, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float VerticalMinDist
        {
            get => ReadFloat(0x38);
            set => Write(0x38, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float VerticalMaxDist
        {
            get => ReadFloat(0x3C);
            set => Write(0x3C, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float VerticalDamp
        {
            get => ReadFloat(0x40);
            set => Write(0x40, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float HorizontalMaxDist
        {
            get => ReadFloat(0x44);
            set => Write(0x44, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float CameraRestDist
        {
            get => ReadFloat(0x48);
            set => Write(0x48, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Cameraview_angle
        {
            get => ReadFloat(0x4C);
            set => Write(0x4C, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float CameraOffset
        {
            get => ReadFloat(0x50);
            set => Write(0x50, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float CameraOffsetDir
        {
            get => ReadFloat(0x54);
            set => Write(0x54, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float CameraTurnSpeed
        {
            get => ReadFloat(0x58);
            set => Write(0x58, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float CameraVelScale
        {
            get => ReadFloat(0x5C);
            set => Write(0x5C, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float CameraRollSpeed
        {
            get => ReadFloat(0x60);
            set => Write(0x60, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float CameraUnused1_X
        {
            get => ReadFloat(0x64);
            set => Write(0x64, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float CameraUnused1_Y
        {
            get => ReadFloat(0x68);
            set => Write(0x68, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float CameraUnused1_Z
        {
            get => ReadFloat(0x6C);
            set => Write(0x6C, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float CollisionHitLoss
        {
            get => ReadFloat(0x70);
            set => Write(0x70, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float CollisionDamageVelocity
        {
            get => ReadFloat(0x74);
            set => Write(0x74, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float CollisionHitVelocity
        {
            get => ReadFloat(0x78);
            set => Write(0x78, value);
        }
    }
}