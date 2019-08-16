using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class CamSpecific_Generic : EndianConvertible
    {
        public static int Size => 0x18;

        public CamSpecific_Generic(Platform platform) : base(EndianConverter.PlatformEndianness(platform)) { }

        public CamSpecific_Generic(byte[] data, Platform platform) : this(platform) { }

        public virtual byte[] ToByteArray()
        {
            return new byte[0x18];
        }
    }

    public class CamSpecific_Follow : CamSpecific_Generic
    {
        public CamSpecific_Follow(byte[] data, Platform platform) : base(platform)
        {
            Rotation = Switch(BitConverter.ToSingle(data, 0));
            Distance = Switch(BitConverter.ToSingle(data, 4));
            Height = Switch(BitConverter.ToSingle(data, 8));
            RubberBand = Switch(BitConverter.ToSingle(data, 12));
            StartSpeed = Switch(BitConverter.ToSingle(data, 16));
            EndSpeed = Switch(BitConverter.ToSingle(data, 20));
        }

        [Category("Follow"), TypeConverter(typeof(FloatTypeConverter))]
        public float Rotation { get; set; }
        [Category("Follow"), TypeConverter(typeof(FloatTypeConverter))]
        public float Distance { get; set; }
        [Category("Follow"), TypeConverter(typeof(FloatTypeConverter))]
        public float Height { get; set; }
        [Category("Follow"), TypeConverter(typeof(FloatTypeConverter))]
        public float RubberBand { get; set; }
        [Category("Follow"), TypeConverter(typeof(FloatTypeConverter))]
        public float StartSpeed { get; set; }
        [Category("Follow"), TypeConverter(typeof(FloatTypeConverter))]
        public float EndSpeed { get; set; }

        public override byte[] ToByteArray()
        {
            List<byte> data = new List<byte>(Size);
            data.AddRange(BitConverter.GetBytes(Switch(Rotation)));
            data.AddRange(BitConverter.GetBytes(Switch(Distance)));
            data.AddRange(BitConverter.GetBytes(Switch(Height)));
            data.AddRange(BitConverter.GetBytes(Switch(RubberBand)));
            data.AddRange(BitConverter.GetBytes(Switch(StartSpeed)));
            data.AddRange(BitConverter.GetBytes(Switch(EndSpeed)));

            while (data.Count < Size)
                data.Add(0);

            return data.ToArray();
        }
    }

    public class CamSpecific_Shoulder : CamSpecific_Generic
    {
        public CamSpecific_Shoulder(byte[] data, Platform platform) : base(platform)
        {
            Distance = Switch(BitConverter.ToSingle(data, 0));
            Height = Switch(BitConverter.ToSingle(data, 4));
            RealignSpeed = Switch(BitConverter.ToSingle(data, 8));
            RealignDelay = Switch(BitConverter.ToSingle(data, 12));
        }

        [Category("Shoulder"), TypeConverter(typeof(FloatTypeConverter))]
        public float Distance { get; set; }
        [Category("Shoulder"), TypeConverter(typeof(FloatTypeConverter))]
        public float Height { get; set; }
        [Category("Shoulder"), TypeConverter(typeof(FloatTypeConverter))]
        public float RealignSpeed { get; set; }
        [Category("Shoulder"), TypeConverter(typeof(FloatTypeConverter))]
        public float RealignDelay { get; set; }

        public override byte[] ToByteArray()
        {
            List<byte> data = new List<byte>(Size);
            data.AddRange(BitConverter.GetBytes(Switch(Distance)));
            data.AddRange(BitConverter.GetBytes(Switch(Height)));
            data.AddRange(BitConverter.GetBytes(Switch(RealignSpeed)));
            data.AddRange(BitConverter.GetBytes(Switch(RealignDelay)));

            while (data.Count < Size)
                data.Add(0);

            return data.ToArray();
        }
    }

    public class CamSpecific_Static : CamSpecific_Generic
    {
        public CamSpecific_Static(byte[] data, Platform platform) : base(platform)
        {
            Unused = Switch(BitConverter.ToInt32(data, 0));
        }

        [Category("Static")]
        public int Unused { get; set; }

        public override byte[] ToByteArray()
        {
            List<byte> data = new List<byte>(Size);
            data.AddRange(BitConverter.GetBytes(Switch(Unused)));

            while (data.Count < Size)
                data.Add(0);

            return data.ToArray();
        }
    }

    public class CamSpecific_Path : CamSpecific_Generic
    {
        public CamSpecific_Path(byte[] data, Platform platform) : base(platform)
        {
            Unknown_AssetID = Switch(BitConverter.ToUInt32(data, 0));
            TimeEnd = Switch(BitConverter.ToSingle(data, 4));
            TimeDelay = Switch(BitConverter.ToSingle(data, 8));
        }

        [Category("Shoulder")]
        public AssetID Unknown_AssetID { get; set; }
        [Category("Shoulder"), TypeConverter(typeof(FloatTypeConverter))]
        public float TimeEnd { get; set; }
        [Category("Shoulder"), TypeConverter(typeof(FloatTypeConverter))]
        public float TimeDelay { get; set; }

        public override byte[] ToByteArray()
        {
            List<byte> data = new List<byte>(Size);
            data.AddRange(BitConverter.GetBytes(Switch(Unknown_AssetID)));
            data.AddRange(BitConverter.GetBytes(Switch(TimeEnd)));
            data.AddRange(BitConverter.GetBytes(Switch(TimeDelay)));

            while (data.Count < Size)
                data.Add(0);

            return data.ToArray();
        }
    }

    public class CamSpecific_StaticFollow : CamSpecific_Generic
    {
        public CamSpecific_StaticFollow(byte[] data, Platform platform) : base(platform)
        {
            RubberBand = Switch(BitConverter.ToSingle(data, 0));
        }

        [Category("Shoulder"), TypeConverter(typeof(FloatTypeConverter))]
        public float RubberBand { get; set; }

        public override byte[] ToByteArray()
        {
            List<byte> data = new List<byte>(Size);
            data.AddRange(BitConverter.GetBytes(Switch(RubberBand)));

            while (data.Count < Size)
                data.Add(0);

            return data.ToArray();
        }
    }
}
