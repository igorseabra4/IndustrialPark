using IndustrialPark.AssetEditorColors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ShrapnelLightInfo
    {
        public AssetSingle LightColorRed { get; set; }
        public AssetSingle LightColorGreen { get; set; }
        public AssetSingle LightColorBlue { get; set; }
        public AssetSingle LightColorAlpha { get; set; }
        public AssetColor LightColor
        {
            get => AssetColor.FromVector4(LightColorRed, LightColorGreen, LightColorBlue, LightColorAlpha);
            set
            {
                var val = value.ToVector4();
                LightColorRed = val.X;
                LightColorGreen = val.Y;
                LightColorBlue = val.Z;
                LightColorAlpha = val.W;
            }
        }

        public AssetSingle Radius { get; set; }
        public AssetSingle FadeUpTime { get; set; }
        public AssetSingle FadeDownTime { get; set; }
        public AssetSingle Duration { get; set; }

        public ShrapnelLightInfo() { }

        public ShrapnelLightInfo(EndianBinaryReader reader)
        {
            LightColorRed = reader.ReadSingle();
            LightColorGreen = reader.ReadSingle();
            LightColorBlue = reader.ReadSingle();
            LightColorAlpha = reader.ReadSingle();
            Radius = reader.ReadSingle();
            FadeUpTime = reader.ReadSingle();
            FadeDownTime = reader.ReadSingle();
            Duration = reader.ReadSingle();
        }

        public void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(LightColorRed);
            writer.Write(LightColorGreen);
            writer.Write(LightColorBlue);
            writer.Write(LightColorAlpha);
            writer.Write(Radius);
            writer.Write(FadeUpTime);
            writer.Write(FadeDownTime);
            writer.Write(Duration);
        }
    }
}
