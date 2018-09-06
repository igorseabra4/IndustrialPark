using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public abstract class ObjectAsset : Asset
    {
        public ObjectAsset(Section_AHDR AHDR) : base(AHDR) { }

        [Category("Object")]
        public AssetID AssetID
        {
            get { return ReadUInt(0); }
            set { Write(0, value); }
        }

        [Category("Object")]
        public ObjectAssetType AssetType
        {
            get { return (ObjectAssetType)ReadByte(0x4); }
            set { Write(0x4, (byte)value); }
        }

        [Category("Object"), ReadOnly(true)]
        public byte AmountOfEvents
        {
            get { return ReadByte(0x5); }
            set { }
        }

        [Category("Object")]
        public short UnknownFlag
        {
            get { return ReadShort(0x6); }
            set { Write(0x6, value); }
        }

        [Category("Object")]
        public AssetEvent[] Events
        {
            get { return ReadEvents(getEventStartOffset()); }
            set { WriteEvents(getEventStartOffset(), value); }
        }

        protected abstract int getEventStartOffset();
    }
}