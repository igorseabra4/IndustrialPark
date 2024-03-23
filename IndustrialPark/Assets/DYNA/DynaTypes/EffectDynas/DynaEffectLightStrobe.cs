using HipHopFile;
using IndustrialPark.AssetEditorColors;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark
{
    public class DynaEffectLightStrobe : AssetDYNA
    {
        private const string dynaCategoryName = "effect:LightEffectStrobe";
        public override string TypeString => dynaCategoryName;
        protected override short constVersion => 1;

        [Category(dynaCategoryName), DisplayName("Red (0 - 1)")]
        public AssetSingle RedMult { get; set; }
        [Category(dynaCategoryName), DisplayName("Green (0 - 1)")]
        public AssetSingle GreenMult { get; set; }
        [Category(dynaCategoryName), DisplayName("Blue (0 - 1)")]
        public AssetSingle BlueMult { get; set; }
        [Category(dynaCategoryName), DisplayName("Alpha (0 - 1)")]
        public AssetSingle SeeThru { get; set; }
        [Category(dynaCategoryName)]
        public AssetColor ColorRGBA
        {
            get => AssetColor.FromVector4(RedMult, GreenMult, BlueMult, SeeThru);
            set
            {
                var val = value.ToVector4();
                RedMult = val.X;
                GreenMult = val.Y;
                BlueMult = val.Z;
                SeeThru = val.W;
            }
        }
        [Category(dynaCategoryName)]
        public AssetSingle FadeUpTime { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle FadeDownTime { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle TimeMax { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle TimeMin { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle RandomTimeMax { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle RandomTimeMin { get; set; }

        public DynaEffectLightStrobe(string assetName) : base(assetName, DynaType.effect__LightEffectStrobe)
        {
        }

        public DynaEffectLightStrobe(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.effect__LightEffectStrobe, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                RedMult = reader.ReadSingle();
                GreenMult = reader.ReadSingle();
                BlueMult = reader.ReadSingle();
                SeeThru = reader.ReadSingle();
                FadeUpTime = reader.ReadSingle();
                FadeDownTime = reader.ReadSingle();
                TimeMax = reader.ReadSingle();
                TimeMin = reader.ReadSingle();
                RandomTimeMax = reader.ReadSingle();
                RandomTimeMin = reader.ReadSingle();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(RedMult);
            writer.Write(GreenMult);
            writer.Write(BlueMult);
            writer.Write(SeeThru);
            writer.Write(FadeUpTime);
            writer.Write(FadeDownTime);
            writer.Write(TimeMax);
            writer.Write(TimeMin);
            writer.Write(RandomTimeMax);
            writer.Write(RandomTimeMin);
        }
    }
}