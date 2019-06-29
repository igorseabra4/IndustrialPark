using HipHopFile;
using System;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetTIMR : ObjectAsset
    {
        public AssetTIMR(Section_AHDR AHDR) : base(AHDR) { }

        protected override int EventStartOffset => 0x10;

        [Category("Timer"), TypeConverter(typeof(FloatTypeConverter))]
        public float Time
        {
            get => ReadFloat(0x8);
            set => Write(0x8, value);
        }

        [Category("Timer"), TypeConverter(typeof(FloatTypeConverter))]
        public float RandomRange
        {
            get => Functions.currentGame == Game.Scooby ? 0 : BitConverter.ToSingle(Data, 0xC);
            set
            {
                if (Functions.currentGame != Game.Scooby)
                    for (int i = 0; i < 4; i++)
                        Data[0xC + i] = BitConverter.GetBytes(value)[i];
            }
        }
    }
}