using HipHopFile;
using System;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetTIMR : BaseAsset
    {
        public AssetTIMR(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform) { }

        protected override int EventStartOffset => game == Game.Scooby ? 0xC : 0x10;

        [Category("Timer"), TypeConverter(typeof(FloatTypeConverter))]
        public float Time
        {
            get => ReadFloat(0x8);
            set => Write(0x8, value);
        }

        [Category("Timer"), TypeConverter(typeof(FloatTypeConverter))]
        public float RandomRange
        {
            get => ReadFloat(0xC);
            set => Write(0xC, value);
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game == Game.Scooby)
                dt.RemoveProperty("RandomRange");
            base.SetDynamicProperties(dt);
        }
    }
}