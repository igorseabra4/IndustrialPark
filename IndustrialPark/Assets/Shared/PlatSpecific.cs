using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class PlatSpecific_Generic
    {
        public static int Size => 0x38;

        public PlatSpecific_Generic() { }

        public PlatSpecific_Generic(byte[] data) { }

        public virtual byte[] ToByteArray()
        {
            return new byte[Size];
        }

        public virtual bool HasReference(uint assetID)
        {
            return false;
        }

        public virtual void Verify(ref List<string> result)
        {

        }
    }

    public class PlatSpecific_ConveryorBelt : PlatSpecific_Generic
    {
        public PlatSpecific_ConveryorBelt(byte[] data)
        {
            Speed = Switch(BitConverter.ToSingle(data, 0));
        }

        [Category("Conveyor Belt")]
        public float Speed { get; set; }

        public override byte[] ToByteArray()
        {
            List<byte> data = new List<byte>(Size);
            data.AddRange(BitConverter.GetBytes(Switch(Speed)));

            while (data.Count < Size)
                data.Add(0);

            return data.ToArray();
        }
    }

    public class PlatSpecific_FallingPlatform : PlatSpecific_Generic
    {
        public PlatSpecific_FallingPlatform(byte[] data)
        {
            Speed = Switch(BitConverter.ToSingle(data, 0));
            BustModel_AssetID = Switch(BitConverter.ToUInt32(data, 4));
        }

        [Category("Falling Platform")]
        public float Speed { get; set; }
        [Category("Falling Platform")]
        public AssetID BustModel_AssetID { get; set; }

        public override byte[] ToByteArray()
        {
            List<byte> data = new List<byte>(Size);
            data.AddRange(BitConverter.GetBytes(Switch(Speed)));
            data.AddRange(BitConverter.GetBytes(Switch(BustModel_AssetID)));

            while (data.Count < Size)
                data.Add(0);

            return data.ToArray();
        }

        public override bool HasReference(uint assetID)
        {
            return BustModel_AssetID == assetID;
        }

        public override void Verify(ref List<string> result)
        {
            Asset.Verify(BustModel_AssetID, ref result);
        }
    }


    public class PlatSpecific_FR : PlatSpecific_Generic
    {
        public PlatSpecific_FR(byte[] data)
        {
            fspeed = Switch(BitConverter.ToSingle(data, 0));
            rspeed = Switch(BitConverter.ToSingle(data, 4));
            ret_delay = Switch(BitConverter.ToSingle(data, 8));
            post_ret_delay = Switch(BitConverter.ToSingle(data, 12));
        }

        [Category("FR")]
        public float fspeed { get; set; }
        [Category("FR")]
        public float rspeed { get; set; }
        [Category("FR")]
        public float ret_delay { get; set; }
        [Category("FR")]
        public float post_ret_delay { get; set; }

        public override byte[] ToByteArray()
        {
            List<byte> data = new List<byte>(Size);
            data.AddRange(BitConverter.GetBytes(Switch(fspeed)));
            data.AddRange(BitConverter.GetBytes(Switch(rspeed)));
            data.AddRange(BitConverter.GetBytes(Switch(ret_delay)));
            data.AddRange(BitConverter.GetBytes(Switch(post_ret_delay)));

            while (data.Count < Size)
                data.Add(0);

            return data.ToArray();
        }
    }

    public class PlatSpecific_BreakawayPlatform : PlatSpecific_Generic
    {
        public PlatSpecific_BreakawayPlatform(byte[] data)
        {
            BreakawayDelay = Switch(BitConverter.ToSingle(data, 0));
            BustModel_AssetID = Switch(BitConverter.ToUInt32(data, 4));
            ResetDelay = Switch(BitConverter.ToSingle(data, 8));
            Flags = Switch(BitConverter.ToInt32(data, 12));
        }

        [Category("Breakaway Platform")]
        public float BreakawayDelay { get; set; }
        [Category("Breakaway Platform")]
        public AssetID BustModel_AssetID { get; set; }
        [Category("Breakaway Platform")]
        public float ResetDelay { get; set; }
        [Category("Breakaway Platform")]
        public int Flags { get; set; }

        public override byte[] ToByteArray()
        {
            List<byte> data = new List<byte>(Size);
            data.AddRange(BitConverter.GetBytes(Switch(BreakawayDelay)));
            data.AddRange(BitConverter.GetBytes(Switch(BustModel_AssetID)));
            data.AddRange(BitConverter.GetBytes(Switch(ResetDelay)));
            data.AddRange(BitConverter.GetBytes(Switch(Flags)));

            while (data.Count < Size)
                data.Add(0);

            return data.ToArray();
        }
    }

    public class PlatSpecific_Springboard : PlatSpecific_Generic
    {
        public PlatSpecific_Springboard()
        {
            Anim1_AssetID = 0;
            Anim2_AssetID = 0;
            Anim3_AssetID = 0;
        }

        public PlatSpecific_Springboard(byte[] data)
        {
            Height1 = Switch(BitConverter.ToSingle(data, 0));
            Height2 = Switch(BitConverter.ToSingle(data, 4));
            Height3 = Switch(BitConverter.ToSingle(data, 8));
            HeightBubbleBounce = Switch(BitConverter.ToSingle(data, 12));
            Anim1_AssetID = Switch(BitConverter.ToUInt32(data, 16));
            Anim2_AssetID = Switch(BitConverter.ToUInt32(data, 20));
            Anim3_AssetID = Switch(BitConverter.ToUInt32(data, 24));
            DirectionX = Switch(BitConverter.ToSingle(data, 28));
            DirectionY = Switch(BitConverter.ToSingle(data, 32));
            DirectionZ = Switch(BitConverter.ToSingle(data, 36));
            Flags = Switch(BitConverter.ToInt32(data, 40));
        }

        [Category("Springboard")]
        public float Height1 { get; set; }
        [Category("Springboard")]
        public float Height2 { get; set; }
        [Category("Springboard")]
        public float Height3 { get; set; }
        [Category("Springboard")]
        public float HeightBubbleBounce { get; set; }
        [Category("Springboard")]
        public AssetID Anim1_AssetID { get; set; }
        [Category("Springboard")]
        public AssetID Anim2_AssetID { get; set; }
        [Category("Springboard")]
        public AssetID Anim3_AssetID { get; set; }
        [Category("Springboard")]
        public float DirectionX { get; set; }
        [Category("Springboard")]
        public float DirectionY { get; set; }
        [Category("Springboard")]
        public float DirectionZ { get; set; }
        [Category("Springboard")]
        public int Flags { get; set; }

        public override byte[] ToByteArray()
        {
            List<byte> data = new List<byte>(Size);
            data.AddRange(BitConverter.GetBytes(Switch(Height1)));
            data.AddRange(BitConverter.GetBytes(Switch(Height2)));
            data.AddRange(BitConverter.GetBytes(Switch(Height3)));
            data.AddRange(BitConverter.GetBytes(Switch(HeightBubbleBounce)));
            data.AddRange(BitConverter.GetBytes(Switch(Anim1_AssetID)));
            data.AddRange(BitConverter.GetBytes(Switch(Anim2_AssetID)));
            data.AddRange(BitConverter.GetBytes(Switch(Anim3_AssetID)));
            data.AddRange(BitConverter.GetBytes(Switch(DirectionX)));
            data.AddRange(BitConverter.GetBytes(Switch(DirectionY)));
            data.AddRange(BitConverter.GetBytes(Switch(DirectionZ)));
            data.AddRange(BitConverter.GetBytes(Switch(Flags)));

            while (data.Count < Size)
                data.Add(0);

            return data.ToArray();
        }

        public override bool HasReference(uint assetID)
        {
            return Anim1_AssetID == assetID || Anim2_AssetID == assetID || Anim3_AssetID == assetID;
        }

        public override void Verify(ref List<string> result)
        {
            Asset.Verify(Anim1_AssetID, ref result);
            Asset.Verify(Anim2_AssetID, ref result);
            Asset.Verify(Anim3_AssetID, ref result);
        }
    }

    public class PlatSpecific_TeeterTotter : PlatSpecific_Generic
    {
        public PlatSpecific_TeeterTotter(byte[] data)
        {
            InitialTilt_Rad = Switch(BitConverter.ToSingle(data, 0));
            MaxTilt_Rad = Switch(BitConverter.ToSingle(data, 4));
            InverseMass = Switch(BitConverter.ToSingle(data, 8));
        }

        [Category("Teeter-Totter")]
        private float InitialTilt_Rad { get; set; }
        [Category("Teeter-Totter")]
        private float MaxTilt_Rad { get; set; }

        [Category("Teeter-Totter")]
        public float InitialTilt_Deg
        {
            get => MathUtil.RadiansToDegrees(InitialTilt_Rad);
            set => InitialTilt_Rad = MathUtil.DegreesToRadians(value);
        }
        [Category("Teeter-Totter")]
        public float MaxTilt_Deg
        {
            get => MathUtil.RadiansToDegrees(MaxTilt_Rad);
            set => MaxTilt_Rad = MathUtil.DegreesToRadians(value);
        }

        [Category("Teeter-Totter")]
        public float InverseMass { get; set; }

        public override byte[] ToByteArray()
        {
            List<byte> data = new List<byte>(Size);
            data.AddRange(BitConverter.GetBytes(Switch(InitialTilt_Rad)));
            data.AddRange(BitConverter.GetBytes(Switch(MaxTilt_Rad)));
            data.AddRange(BitConverter.GetBytes(Switch(InverseMass)));

            while (data.Count < Size)
                data.Add(0);

            return data.ToArray();
        }
    }

    public class PlatSpecific_Paddle : PlatSpecific_Generic
    {
        public PlatSpecific_Paddle(byte[] data)
        {
            StartOrient = Switch(BitConverter.ToInt32(data, 0));
            OrientCount = Switch(BitConverter.ToInt32(data, 4));
            OrientLoop = Switch(BitConverter.ToSingle(data, 8));
            Orient1 = Switch(BitConverter.ToSingle(data, 12));
            Orient2 = Switch(BitConverter.ToSingle(data, 16));
            Orient3 = Switch(BitConverter.ToSingle(data, 20));
            Orient4 = Switch(BitConverter.ToSingle(data, 24));
            Orient5 = Switch(BitConverter.ToSingle(data, 28));
            Orient6 = Switch(BitConverter.ToSingle(data, 32));
            Flags = Switch(BitConverter.ToInt32(data, 36));
            RotateSpeed = Switch(BitConverter.ToSingle(data, 36));
            AccelTime = Switch(BitConverter.ToSingle(data, 36));
            DecelTime = Switch(BitConverter.ToSingle(data, 36));
            HubRadius = Switch(BitConverter.ToSingle(data, 36));
        }

        [Category("Paddle")]
        public int StartOrient { get; set; }
        [Category("Paddle")]
        public int OrientCount { get; set; }
        [Category("Paddle")]
        public float OrientLoop { get; set; }
        [Category("Paddle")]
        public float Orient1 { get; set; }
        [Category("Paddle")]
        public float Orient2{ get; set; }
        [Category("Paddle")]
        public float Orient3 { get; set; }
        [Category("Paddle")]
        public float Orient4 { get; set; }
        [Category("Paddle")]
        public float Orient5 { get; set; }
        [Category("Paddle")]
        public float Orient6 { get; set; }
        [Category("Paddle")]
        public int Flags { get; set; }
        [Category("Paddle")]
        public float RotateSpeed { get; set; }
        [Category("Paddle")]
        public float AccelTime { get; set; }
        [Category("Paddle")]
        public float DecelTime { get; set; }
        [Category("Paddle")]
        public float HubRadius { get; set; }

        public override byte[] ToByteArray()
        {
            List<byte> data = new List<byte>(Size);

            data.AddRange(BitConverter.GetBytes(Switch(StartOrient)));
            data.AddRange(BitConverter.GetBytes(Switch(OrientCount)));
            data.AddRange(BitConverter.GetBytes(Switch(OrientLoop)));
            data.AddRange(BitConverter.GetBytes(Switch(Orient1)));
            data.AddRange(BitConverter.GetBytes(Switch(Orient2)));
            data.AddRange(BitConverter.GetBytes(Switch(Orient3)));
            data.AddRange(BitConverter.GetBytes(Switch(Orient4)));
            data.AddRange(BitConverter.GetBytes(Switch(Orient5)));
            data.AddRange(BitConverter.GetBytes(Switch(Orient5)));
            data.AddRange(BitConverter.GetBytes(Switch(Flags)));
            data.AddRange(BitConverter.GetBytes(Switch(RotateSpeed)));
            data.AddRange(BitConverter.GetBytes(Switch(AccelTime)));
            data.AddRange(BitConverter.GetBytes(Switch(DecelTime)));
            data.AddRange(BitConverter.GetBytes(Switch(HubRadius)));

            while (data.Count < Size)
                data.Add(0);

            return data.ToArray();
        }
    }
}
