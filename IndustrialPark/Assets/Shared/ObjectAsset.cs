using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public class ObjectAsset : Asset
    {
        public ObjectAsset(Section_AHDR AHDR) : base(AHDR)
        {
            if (AssetID != AHDR.assetID)
                AssetID = AHDR.assetID;
        }

        [Category("Object Base")]
        public AssetID AssetID
        {
            get => ReadUInt(0);
            set => Write(0, value);
        }

        [Category("Object Base")]
        public ObjectAssetType AssetType
        {
            get => (ObjectAssetType)ReadByte(0x4);
            set => Write(0x4, (byte)value);
        }

        [Category("Object Base"), ReadOnly(true)]
        public byte AmountOfEvents
        {
            get => ReadByte(0x5);
        }

        [Category("Object Base"), TypeConverter(typeof(HexShortTypeConverter))]
        public short Flags
        {
            get => ReadShort(0x6);
            set => Write(0x6, value);
        }

        [Category("Object Base")]
        public bool EnabledOnStart
        {
            get => (Flags & 0x01) != 0;
            set
            {
                if (value)
                    Flags = (short)(Flags | 0x01);
                else
                    Flags = (short)(Flags & (0xFF - 0x01));
            }
        }

        [Category("Object Base")]
        public bool StateIsPersistent
        {
            get => (Flags & 0x02) != 0;
            set
            {
                if (value)
                    Flags = (short)(Flags | 0x02);
                else
                    Flags = (short)(Flags & (0xFF - 0x02));
            }
        }

        [Category("Object Base")]
        public bool UnknownAlways1
        {
            get => (Flags & 0x04) != 0;
            set
            {
                if (value)
                    Flags = (short)(Flags | 0x04);
                else
                    Flags = (short)(Flags & (0xFF - 0x04));
            }
        }

        [Category("Object Base")]
        public bool VisibleDuringCutscenes
        {
            get => (Flags & 0x08) != 0;
            set
            {
                if (value)
                    Flags = (short)(Flags | 0x08);
                else
                    Flags = (short)(Flags & (0xFF - 0x08));
            }
        }

        [Category("Object Base")]
        public bool ReceiveShadows
        {
            get => (Flags & 0x10) != 0;
            set
            {
                if (value)
                    Flags = (short)(Flags | 0x10);
                else
                    Flags = (short)(Flags & (0xFF - 0x10));
            }
        }

        [Category("Object Base")]
        public AssetEventBFBB[] EventsBFBB
        {
            get => ReadEventsBFBB();
            set => WriteEvents(value);
        }

        [Category("Object Base")]
        public AssetEventTSSM[] EventsTSSM
        {
            get => ReadEventsTSSM();
            set => WriteEvents(value);
        }

        protected virtual int EventStartOffset { get => Data.Length - AmountOfEvents * AssetEvent.sizeOfStruct; }

        protected AssetEventBFBB[] ReadEventsBFBB()
        {
            byte amount = ReadByte(0x05);
            AssetEventBFBB[] events = new AssetEventBFBB[amount];

            for (int i = 0; i < amount; i++)
                events[i] = new AssetEventBFBB(Data, EventStartOffset + i * AssetEvent.sizeOfStruct);

            return events;
        }

        protected AssetEventTSSM[] ReadEventsTSSM()
        {
            byte amount = ReadByte(0x05);
            AssetEventTSSM[] events = new AssetEventTSSM[amount];

            for (int i = 0; i < amount; i++)
                events[i] = new AssetEventTSSM(Data, EventStartOffset + i * AssetEvent.sizeOfStruct);

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

        public override bool HasReference(uint assetID)
        {
            foreach (AssetEvent assetEvent in EventsBFBB)
                if (assetEvent.TargetAssetID == assetID)
                    return true;

            return false;
        }
    }
}