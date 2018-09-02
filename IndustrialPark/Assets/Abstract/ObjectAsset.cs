using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public abstract class ObjectAsset : Asset
    {
        public ObjectAsset(Section_AHDR AHDR) : base(AHDR) { }

        public AssetID AssetID
        {
            get { return ReadUInt(0); }
            set { Write(0, value); }
        }

        public byte AssetType
        {
            get { return ReadByte(0x4); }
            set { Write(0x4, value); }
        }

        [ReadOnly(true)]
        public byte AmountOfEvents
        {
            get { return ReadByte(0x5); }
            set { }
        }

        public byte UnknownFlag06
        {
            get { return ReadByte(0x6); }
            set { Write(0x6, value); }
        }

        public byte UnknownFlag07
        {
            get { return ReadByte(0x7); }
            set { Write(0x7, value); }
        }
    }
}