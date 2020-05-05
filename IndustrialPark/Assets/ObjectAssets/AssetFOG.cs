using HipHopFile;
using System.ComponentModel;
using System.Drawing.Design;
using AssetEditorColors;

namespace IndustrialPark
{
    public class AssetFOG : BaseAsset
    {
        public AssetFOG(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform) { }

        protected override int EventStartOffset => 0x24;
        
        [Category("Fog"), Editor(typeof(MyColorEditor), typeof(UITypeEditor)), DisplayName("End Color (R, G, B)")]
        public MyColor BackgroundColor
        {
            get => new MyColor(Data[8], Data[9], Data[10], Data[11]);
            
            set
            {
                Data[8] = value.R;
                Data[9] = value.G;
                Data[10] = value.B;
            }
        }

        [Category("Fog"), DisplayName("End Color Alpha (0 - 255)")]
        public byte BackgroundColorAlpha
        {
            get => ReadByte(11);
            set => Write(value, 11);
        }

        [Category("Fog"), Editor(typeof(MyColorEditor), typeof(UITypeEditor)), DisplayName("Start Color (R, G, B)")]
        public MyColor FogColor
        {
            get => new MyColor(Data[12], Data[13], Data[14], Data[15]);
            set
            {
                Data[12] = value.R;
                Data[13] = value.G;
                Data[14] = value.B;
            }
        }

        [Category("Fog"), DisplayName("Start Color Alpha (0 - 255)")]
        public byte FogColorAlpha
        {
            get => ReadByte(15);
            set => Write(value, 15);
        }

        [Category("Fog"), TypeConverter(typeof(FloatTypeConverter))]
        public float FogDensity
        {
            get => ReadFloat(0x10);
            set => Write(0x10, value);
        }

        [Category("Fog"), TypeConverter(typeof(FloatTypeConverter))]
        public float StartDistance
        {
            get => ReadFloat(0x14);
            set => Write(0x14, value);
        }

        [Category("Fog"), TypeConverter(typeof(FloatTypeConverter))]
        public float EndDistance
        {
            get => ReadFloat(0x18);
            set => Write(0x18, value);
        }

        [Category("Fog"), TypeConverter(typeof(FloatTypeConverter))]
        public float TransitionTime
        {
            get => ReadFloat(0x1C);
            set => Write(0x1C, value);
        }

        [Category("Fog")]
        public byte FogType
        {
            get => ReadByte(0x20);
            set => Write(0x20, value);
        }

        [Category("Fog")]
        public byte Padding21
        {
            get => ReadByte(0x21);
            set => Write(0x21, value);
        }

        [Category("Fog")]
        public byte Padding22
        {
            get => ReadByte(0x22);
            set => Write(0x22, value);
        }

        [Category("Fog")]
        public byte Padding23
        {
            get => ReadByte(0x23);
            set => Write(0x23, value);
        }
    }
}