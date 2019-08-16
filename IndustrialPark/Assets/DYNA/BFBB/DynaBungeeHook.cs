using System;
using System.Collections.Generic;
using System.ComponentModel;
using HipHopFile;

namespace IndustrialPark
{
    public class DynaBungeeHook : DynaBase
    {
        public override string Note => "Version is always 13";

        public DynaBungeeHook(Platform platform) : base(platform)
        {
            Placeable_AssetID = 0;
        }

        public override bool HasReference(uint assetID)
        {
            if (Placeable_AssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            Asset.Verify(Placeable_AssetID, ref result);
        }

        public DynaBungeeHook(IEnumerable<byte> enumerable, Platform platform) : base (enumerable, platform)
        {
            Placeable_AssetID = Switch(BitConverter.ToUInt32(Data, 0x0));
            EnterX = Switch(BitConverter.ToInt32(Data, 0x4));
            EnterY = Switch(BitConverter.ToInt32(Data, 0x8));
            EnterZ = Switch(BitConverter.ToInt32(Data, 0xC));
            AttachDist = Switch(BitConverter.ToSingle(Data, 0x10));
            AttachTravelTime = Switch(BitConverter.ToSingle(Data, 0x14));
            DetachDist = Switch(BitConverter.ToSingle(Data, 0x18));
            DetachFreeFallTime = Switch(BitConverter.ToSingle(Data, 0x1C));
            DetachAccel = Switch(BitConverter.ToSingle(Data, 0x20));
            TurnUnused1 = Switch(BitConverter.ToSingle(Data, 0x24));
            TurnUnused2 = Switch(BitConverter.ToSingle(Data, 0x28));
            VerticalFrequency = Switch(BitConverter.ToSingle(Data, 0x2C));
            VerticalGravity = Switch(BitConverter.ToSingle(Data, 0x30));
            VerticalDive = Switch(BitConverter.ToSingle(Data, 0x34));
            VerticalMinDist = Switch(BitConverter.ToSingle(Data, 0x38));
            VerticalMaxDist = Switch(BitConverter.ToSingle(Data, 0x3C));
            VerticalDamp = Switch(BitConverter.ToSingle(Data, 0x40));
            HorizontalMaxDist = Switch(BitConverter.ToSingle(Data, 0x44));
            CameraRestDist = Switch(BitConverter.ToSingle(Data, 0x48));
            Cameraview_angle = Switch(BitConverter.ToSingle(Data, 0x4C));
            CameraOffset = Switch(BitConverter.ToSingle(Data, 0x50));
            CameraOffsetDir = Switch(BitConverter.ToSingle(Data, 0x54));
            CameraTurnSpeed = Switch(BitConverter.ToSingle(Data, 0x58));
            CameraVelScale = Switch(BitConverter.ToSingle(Data, 0x5C));
            CameraRollSpeed = Switch(BitConverter.ToSingle(Data, 0x60));
            CameraUnused1_X = Switch(BitConverter.ToSingle(Data, 0x64));
            CameraUnused1_Y = Switch(BitConverter.ToSingle(Data, 0x68));
            CameraUnused1_Z = Switch(BitConverter.ToSingle(Data, 0x6C));
            CollisionHitLoss = Switch(BitConverter.ToSingle(Data, 0x70));
            CollisionDamageVelocity = Switch(BitConverter.ToSingle(Data, 0x74));
            CollisionHitVelocity = Switch(BitConverter.ToSingle(Data, 0x78));
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(Placeable_AssetID)));
            list.AddRange(BitConverter.GetBytes(Switch(EnterX)));
            list.AddRange(BitConverter.GetBytes(Switch(EnterY)));
            list.AddRange(BitConverter.GetBytes(Switch(EnterZ)));
            list.AddRange(BitConverter.GetBytes(Switch(AttachDist)));
            list.AddRange(BitConverter.GetBytes(Switch(AttachTravelTime)));
            list.AddRange(BitConverter.GetBytes(Switch(DetachDist)));
            list.AddRange(BitConverter.GetBytes(Switch(DetachFreeFallTime)));
            list.AddRange(BitConverter.GetBytes(Switch(DetachAccel)));
            list.AddRange(BitConverter.GetBytes(Switch(TurnUnused1)));
            list.AddRange(BitConverter.GetBytes(Switch(TurnUnused2)));
            list.AddRange(BitConverter.GetBytes(Switch(VerticalFrequency)));
            list.AddRange(BitConverter.GetBytes(Switch(VerticalGravity)));
            list.AddRange(BitConverter.GetBytes(Switch(VerticalDive)));
            list.AddRange(BitConverter.GetBytes(Switch(VerticalMinDist)));
            list.AddRange(BitConverter.GetBytes(Switch(VerticalMaxDist)));
            list.AddRange(BitConverter.GetBytes(Switch(VerticalDamp)));
            list.AddRange(BitConverter.GetBytes(Switch(HorizontalMaxDist)));
            list.AddRange(BitConverter.GetBytes(Switch(CameraRestDist)));
            list.AddRange(BitConverter.GetBytes(Switch(Cameraview_angle)));
            list.AddRange(BitConverter.GetBytes(Switch(CameraOffset)));
            list.AddRange(BitConverter.GetBytes(Switch(CameraOffsetDir)));
            list.AddRange(BitConverter.GetBytes(Switch(CameraTurnSpeed)));
            list.AddRange(BitConverter.GetBytes(Switch(CameraVelScale)));
            list.AddRange(BitConverter.GetBytes(Switch(CameraRollSpeed)));
            list.AddRange(BitConverter.GetBytes(Switch(CameraUnused1_X)));
            list.AddRange(BitConverter.GetBytes(Switch(CameraUnused1_Y)));
            list.AddRange(BitConverter.GetBytes(Switch(CameraUnused1_Z)));
            list.AddRange(BitConverter.GetBytes(Switch(CollisionHitLoss)));
            list.AddRange(BitConverter.GetBytes(Switch(CollisionDamageVelocity)));
            list.AddRange(BitConverter.GetBytes(Switch(CollisionHitVelocity)));

            return list.ToArray();
        }

        public AssetID Placeable_AssetID { get; set; }
        public int EnterX { get; set; }
        public int EnterY { get; set; }
        public int EnterZ { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float AttachDist { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float AttachTravelTime { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float DetachDist { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float DetachFreeFallTime { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float DetachAccel { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float TurnUnused1 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float TurnUnused2 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float VerticalFrequency { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float VerticalGravity { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float VerticalDive { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float VerticalMinDist { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float VerticalMaxDist { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float VerticalDamp { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float HorizontalMaxDist { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float CameraRestDist { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Cameraview_angle { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float CameraOffset { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float CameraOffsetDir { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float CameraTurnSpeed { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float CameraVelScale { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float CameraRollSpeed { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float CameraUnused1_X { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float CameraUnused1_Y { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float CameraUnused1_Z { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float CollisionHitLoss { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float CollisionDamageVelocity { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float CollisionHitVelocity { get; set; }
    }
}