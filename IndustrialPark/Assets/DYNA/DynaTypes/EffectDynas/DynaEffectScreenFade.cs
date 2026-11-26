using HipHopFile;
using IndustrialPark.Assets.Colors;
using System.ComponentModel;

namespace IndustrialPark.Assets
{
    public class DynaEffectScreenFade : AssetDYNA
    {
        private const string dynaCategoryName = "effect:ScreenFade";
        public override string TypeString => dynaCategoryName;

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetColor Color { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle FadeDownTime { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle WaitTime { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle FadeUpTime { get; set; }

        public DynaEffectScreenFade(Section_AHDR AHDR, EndianBinaryReader reader) : base(AHDR, DynaType.effect__ScreenFade, reader)
        {
            Color = reader.ReadColor();
            FadeDownTime = reader.ReadSingle();
            WaitTime = reader.ReadSingle();
            FadeUpTime = reader.ReadSingle();
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(Color);
            writer.Write(FadeDownTime);
            writer.Write(WaitTime);
            writer.Write(FadeUpTime);
        }
    }
}