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
    public class DynaEffectLightFlicker : AssetDYNA
    {
        private const string dynaCategoryName = "effect:LightEffectFlicker";
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
        public AssetSingle FlickerRate { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle RandomFlickerRate { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle BlendTime { get; set; }
        [Category(dynaCategoryName)]
        public FlagBitmask Flags { get; set; } = IntFlagsDescriptor();

        public DynaEffectLightFlicker(string assetName) : base(assetName, DynaType.effect__LightEffectFlicker)
        {
            Flags.FlagValueInt = 1;
        }

        public DynaEffectLightFlicker(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.effect__LightEffectFlicker, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                RedMult = reader.ReadSingle();
                GreenMult = reader.ReadSingle();
                BlueMult = reader.ReadSingle();
                SeeThru = reader.ReadSingle();
                FlickerRate = reader.ReadSingle();
                RandomFlickerRate = reader.ReadSingle();
                BlendTime = reader.ReadSingle();
                Flags.FlagValueInt = reader.ReadUInt32();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(RedMult);
            writer.Write(GreenMult);
            writer.Write(BlueMult);
            writer.Write(SeeThru);
            writer.Write(FlickerRate);
            writer.Write(RandomFlickerRate);
            writer.Write(BlendTime);
            writer.Write(Flags.FlagValueInt);
        }

    }
}