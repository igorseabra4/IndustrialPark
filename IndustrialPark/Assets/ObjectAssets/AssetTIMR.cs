using HipHopFile;
using System;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetTIMR : ObjectAsset
    {
        public AssetTIMR(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform) { }

        protected override int EventStartOffset => game == Game.Scooby ? 0xC : 0x10;

        [Category("Timer"), TypeConverter(typeof(FloatTypeConverter))]
        public float Time
        {
            get => ReadFloat(0x8);
            set => Write(0x8, value);
        }

        [Category("Timer"), TypeConverter(typeof(FloatTypeConverter)), Description("Not present in Scooby.")]
        public float RandomRange
        {
            get => game == Game.Scooby ? 0 : BitConverter.ToSingle(Data, 0xC);
            set
            {
                if (game != Game.Scooby)
                    for (int i = 0; i < 4; i++)
                        Data[0xC + i] = BitConverter.GetBytes(value)[i];
            }
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game == Game.Scooby)
                dt.RemoveProperty("RandomRange");
            base.SetDynamicProperties(dt);
        }
    }
}