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
            get => ReadUInt(0);
            set => Write(0, value);
        }

        [Category("Object")]
        public ObjectAssetType AssetType
        {
            get => (ObjectAssetType)ReadByte(0x4);
            set => Write(0x4, (byte)value);
        }

        [Category("Object"), ReadOnly(true)]
        public byte AmountOfEvents
        {
            get => ReadByte(0x5);
        }

        [Category("Object"), TypeConverter(typeof(HexShortTypeConverter))]
        public short UnknownFlag
        {
            get => ReadShort(0x6);
            set => Write(0x6, value);
        }

        [Category("Object")]
        public AssetEvent[] Events
        {
            get => ReadEvents(EventStartOffset);
            set => WriteEvents(EventStartOffset, value);
        }

        protected abstract int EventStartOffset { get; }
    }
}