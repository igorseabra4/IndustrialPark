using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using HipHopFile;

namespace IndustrialPark
{
    public class KeyFrame
    {
        public ushort TimeIndex { get; set; }
        public short RotationX { get; set; }
        public short RotationY { get; set; }
        public short RotationZ { get; set; }
        public short RotationW { get; set; }
        public short PositionX { get; set; }
        public short PositionY { get; set; }
        public short PositionZ { get; set; }
    }
    
    public class AssetANIM : Asset
    {
        public AssetANIM(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform) { }

        public override void Verify(ref List<string> result)
        {
            KeyFrame[] a = KeyFrames;
            float[] b = Times;
            short[][] c = Offsets;
        }

        [Category("Animation")]
        public int Flags
        {
            get => ReadInt(0x04);
            set => Write(0x04, value);
        }

        [Category("Animation")]
        public short BoneCount
        {
            get => ReadShort(0x08);
            set => Write(0x08, value);
        }

        [Category("Animation")]
        public short FrameCount
        {
            get => ReadShort(0x0A);
            set => Write(0x0A, value);
        }

        [Category("Animation")]
        public int KeyframeCount
        {
            get => ReadInt(0xC);
            set => Write(0xC, value);
        }

        [Category("Animation"), TypeConverter(typeof(FloatTypeConverter))]
        public float ScaleX
        {
            get => ReadFloat(0x10);
            set => Write(0x10, value);
        }

        [Category("Animation"), TypeConverter(typeof(FloatTypeConverter))]
        public float ScaleY
        {
            get => ReadFloat(0x14);
            set => Write(0x14, value);
        }

        [Category("Animation"), TypeConverter(typeof(FloatTypeConverter))]
        public float ScaleZ
        {
            get => ReadFloat(0x18);
            set => Write(0x18, value);
        }

        private const int KeyFramesSectionStart = 0x1C;

        [Category("Animation")]
        public KeyFrame[] KeyFrames
        {
            get
            {
                List<KeyFrame> keyFrames = new List<KeyFrame>();
                for (int i = KeyFramesSectionStart; i < KeyFramesSectionStart + KeyframeCount * 0x10; i += 0x10)
                {
                    keyFrames.Add(new KeyFrame
                    {
                        TimeIndex = ReadUShort(i + 0x00),
                        RotationX = ReadShort(i + 0x02),
                        RotationY = ReadShort(i + 0x04),
                        RotationZ = ReadShort(i + 0x06),
                        RotationW = ReadShort(i + 0x08),
                        PositionX = ReadShort(i + 0x0A),
                        PositionY = ReadShort(i + 0x0C),
                        PositionZ = ReadShort(i + 0x0E)
                    });
                }
                return keyFrames.ToArray();
            }
            set
            {
                List<byte> before = Data.Take(KeyFramesSectionStart).ToList();
                List<byte> after = Data.Skip(TimesSectionStart).ToList();
                foreach (KeyFrame k in value)
                {
                    before.AddRange(BitConverter.GetBytes(Switch(k.TimeIndex)));
                    before.AddRange(BitConverter.GetBytes(Switch(k.RotationX)));
                    before.AddRange(BitConverter.GetBytes(Switch(k.RotationY)));
                    before.AddRange(BitConverter.GetBytes(Switch(k.RotationZ)));
                    before.AddRange(BitConverter.GetBytes(Switch(k.RotationW)));
                    before.AddRange(BitConverter.GetBytes(Switch(k.PositionX)));
                    before.AddRange(BitConverter.GetBytes(Switch(k.PositionY)));
                    before.AddRange(BitConverter.GetBytes(Switch(k.PositionZ)));
                }
                before.AddRange(after);
                Data = before.ToArray();
                KeyframeCount = value.Length;
            }
        }

        private int TimesSectionStart => KeyFramesSectionStart + KeyframeCount * 0x10;

        [Category("Animation")]
        public float[] Times
        {
            get
            {
                List<float> timeMap = new List<float>();
                for (int i = TimesSectionStart; i < OffsetsSectionStart; i += 4)
                    timeMap.Add(ReadFloat(i));
                return timeMap.ToArray();
            }
            set
            {
                List<byte> before = Data.Take(TimesSectionStart).ToList();
                List<byte> after = Data.Skip(OffsetsSectionStart).ToList();
                foreach (float k in value)
                    before.AddRange(BitConverter.GetBytes(Switch(k)));
                before.AddRange(after);
                Data = before.ToArray();
                FrameCount = (short)value.Length;
            }
        }

        private int OffsetsSectionStart => TimesSectionStart + FrameCount * 4;

        [Category("Animation")]
        public short[][] Offsets
        {
            get
            {
                List<short[]> keyFrameMap = new List<short[]>();
                for (int i = OffsetsSectionStart; i < OffsetsSectionStart + BoneCount * 2 * (FrameCount - 1); i += BoneCount * 2)
                {
                    List<short> keyframes = new List<short>();
                    for (int j = i; j < i + BoneCount * 2; j += 2)
                        keyframes.Add(ReadShort(j));

                    keyFrameMap.Add(keyframes.ToArray());
                }
                return keyFrameMap.ToArray();
            }
            set
            {
                List<byte> before = Data.Take(OffsetsSectionStart).ToList();

                foreach (short[] i in value)
                    foreach (short j in i)
                        before.AddRange(BitConverter.GetBytes(Switch(j)));

                while (before.Count % 4 != 0)
                    before.Add(0xCD);

                Data = before.ToArray();
            }
        }
    }
}