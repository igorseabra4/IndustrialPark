using HipHopFile;
using System;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetTIMR : ObjectAsset
    {
        public AssetTIMR(Section_AHDR AHDR) : base(AHDR) { }

        protected override int EventStartOffset
        {
            get => 0x10;
        }

        [Category("Timer")]
        public float Time
        {
            get => ReadFloat(0x8);
            set => Write(0x8, value);
        }

        [Category("Timer")]
        public float Unknown
        {
            get => BitConverter.ToSingle(AHDR.data, 0xC);
            set
            {
                for (int i = 0; i < 4; i++)
                    AHDR.data[0xC + i] = BitConverter.GetBytes(value)[i];
            }
        }
    }
}