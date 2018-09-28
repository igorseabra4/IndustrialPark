using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static HipHopFile.Functions;

namespace IndustrialPark
{
    public class ObjectAsset : Asset
    {
        public ObjectAsset(Section_AHDR AHDR) : base(AHDR)
        {
            if (AssetID != AHDR.assetID)
                AssetID = AHDR.assetID;
        }

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
            get => ReadEvents();
            set => WriteEvents(value);
        }

        protected virtual int EventStartOffset { get => Data.Length - AmountOfEvents * AssetEvent.sizeOfStruct; }

        protected AssetEvent[] ReadEvents()
        {
            byte amount = ReadByte(0x05);
            AssetEvent[] events = new AssetEvent[amount];

            if (currentGame == Game.BFBB | currentGame == Game.Scooby)
            {
                for (int i = 0; i < amount; i++)
                    events[i] = new AssetEventBFBB(Data, EventStartOffset + i * AssetEvent.sizeOfStruct);
            }
            else if (currentGame == Game.Incredibles)
            {
                for (int i = 0; i < amount; i++)
                    events[i] = new AssetEventTSSM(Data, EventStartOffset + i * AssetEvent.sizeOfStruct);
            }

            return events;
        }

        protected void WriteEvents(AssetEvent[] value)
        {
            List<byte> newData = Data.Take(EventStartOffset).ToList();
            List<byte> bytesAfterEvents = Data.Skip(EventStartOffset + ReadByte(0x05) * AssetEvent.sizeOfStruct).ToList();

            for (int i = 0; i < value.Length; i++)
                newData.AddRange(value[i].ToByteArray());

            newData.AddRange(bytesAfterEvents);
            newData[0x05] = (byte)value.Length;

            Data = newData.ToArray();
        }
    }
}