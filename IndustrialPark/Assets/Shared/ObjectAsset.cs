using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
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
        public byte AmountOfEvents => ReadByte(0x5);

        [Category("Object Base"), TypeConverter(typeof(HexShortTypeConverter))]
        public short Flags
        {
            get => ReadShort(0x6);
            set => Write(0x6, value);
        }

        [Category("Object Base")]
        public bool EnabledOnStart
        {
            get => (Flags & Mask(0)) != 0;
            set => Flags = (short)(value ? (Flags | (short)Mask(0)) : (Flags & (short)InvMask(0)));
        }

        [Category("Object Base")]
        public bool StateIsPersistent
        {
            get => (Flags & Mask(1)) != 0;
            set => Flags = (short)(value ? (Flags | (short)Mask(1)) : (Flags & (short)InvMask(1)));
        }

        [Category("Object Base")]
        public bool UnknownAlwaysTrue
        {
            get => (Flags & Mask(2)) != 0;
            set => Flags = (short)(value ? (Flags | (short)Mask(2)) : (Flags & (short)InvMask(2)));
        }

        [Category("Object Base")]
        public bool VisibleDuringCutscenes
        {
            get => (Flags & Mask(3)) != 0;
            set => Flags = (short)(value ? (Flags | (short)Mask(3)) : (Flags & (short)InvMask(3)));
        }

        [Category("Object Base")]
        public bool ReceiveShadows
        {
            get => (Flags & Mask(4)) != 0;
            set => Flags = (short)(value ? (Flags | (short)Mask(4)) : (Flags & (short)InvMask(4)));
        }

        protected virtual int EventStartOffset => Data.Length - AmountOfEvents * Link.sizeOfStruct;

        [Category("Object Base"), Editor(typeof(LinkListEditor), typeof(UITypeEditor))]
        public LinkBFBB[] LinksBFBB
        {
            get
            {
                LinkListEditor.IsTimed = false;
                LinkBFBB[] events = new LinkBFBB[AmountOfEvents];

                for (int i = 0; i < AmountOfEvents; i++)
                    events[i] = new LinkBFBB(Data, EventStartOffset + i * Link.sizeOfStruct, false);

                return events;
            }
            set => WriteEvents(value);
        }
        [Category("Object Base"), Editor(typeof(LinkListEditor), typeof(UITypeEditor))]
        public LinkTSSM[] LinksTSSM
        {
            get
            {
                LinkListEditor.IsTimed = false;
                LinkTSSM[] events = new LinkTSSM[AmountOfEvents];

                for (int i = 0; i < AmountOfEvents; i++)
                    events[i] = new LinkTSSM(Data, EventStartOffset + i * Link.sizeOfStruct, false);

                return events;
            }
            set => WriteEvents(value);
        }
        [Category("Object Base"), Editor(typeof(LinkListEditor), typeof(UITypeEditor))]
        public LinkIncredibles[] LinksIncredibles
        {
            get
            {
                LinkListEditor.IsTimed = false;
                LinkIncredibles[] events = new LinkIncredibles[AmountOfEvents];

                for (int i = 0; i < AmountOfEvents; i++)
                    events[i] = new LinkIncredibles(Data, EventStartOffset + i * Link.sizeOfStruct, false);

                return events;
            }
            set => WriteEvents(value);
        }

        protected void WriteEvents(Link[] value)
        {
            List<byte> newData = Data.Take(EventStartOffset).ToList();
            List<byte> bytesAfterEvents = Data.Skip(EventStartOffset + ReadByte(0x05) * Link.sizeOfStruct).ToList();

            for (int i = 0; i < value.Length; i++)
                newData.AddRange(value[i].ToByteArray());

            newData.AddRange(bytesAfterEvents);
            newData[0x05] = (byte)value.Length;

            Data = newData.ToArray();
        }

        public override bool HasReference(uint assetID)
        {
            foreach (Link link in LinksBFBB)
            {
                if (link.TargetAssetID == assetID)
                    return true;
                if (link.ArgumentAssetID == assetID)
                    return true;
                if (link.SourceCheckAssetID == assetID)
                    return true;
            }

            return false;
        }
    }
}