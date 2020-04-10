using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;

namespace IndustrialPark
{
    public class ObjectAsset : Asset
    {
        public ObjectAsset(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
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
                LinkListEditor.endianness = EndianConverter.PlatformEndianness(platform);
                LinkListEditor.thisAssetID = AHDR.assetID;
                LinkBFBB[] events = new LinkBFBB[AmountOfEvents];

                for (int i = 0; i < AmountOfEvents; i++)
                    events[i] = new LinkBFBB(Data, EventStartOffset + i * Link.sizeOfStruct, false, EndianConverter.PlatformEndianness(platform));

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
                LinkListEditor.endianness = EndianConverter.PlatformEndianness(platform);
                LinkListEditor.thisAssetID = AHDR.assetID;
                LinkTSSM[] events = new LinkTSSM[AmountOfEvents];

                for (int i = 0; i < AmountOfEvents; i++)
                    events[i] = new LinkTSSM(Data, EventStartOffset + i * Link.sizeOfStruct, false, EndianConverter.PlatformEndianness(platform));

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
                LinkListEditor.endianness = EndianConverter.PlatformEndianness(platform);
                LinkListEditor.thisAssetID = AHDR.assetID;
                LinkIncredibles[] events = new LinkIncredibles[AmountOfEvents];

                for (int i = 0; i < AmountOfEvents; i++)
                    events[i] = new LinkIncredibles(Data, EventStartOffset + i * Link.sizeOfStruct, false, EndianConverter.PlatformEndianness(platform));

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
                if (link.TargetAssetID == assetID || link.ArgumentAssetID == assetID || link.SourceCheckAssetID == assetID)
                    return true;
            
            return false;
        }

        public override void Verify(ref List<string> result)
        {
            if (game == Game.BFBB || game == Game.Scooby)
            {
                foreach (LinkBFBB link in LinksBFBB)
                {
                    if (link.TargetAssetID == 0)
                        result.Add("Link with Target Asset set to 0");
                    Verify(link.TargetAssetID, ref result);
                    Verify(link.ArgumentAssetID, ref result);
                    Verify(link.SourceCheckAssetID, ref result);

                    if (link.EventReceiveID == 0 || link.EventReceiveID.ToString() == ((int)link.EventReceiveID).ToString())
                        result.Add("Link receives event of unknown type for BFBB: " + link.EventReceiveID.ToString());
                    if (link.EventSendID == 0 || link.EventSendID.ToString() == ((int)link.EventSendID).ToString())
                        result.Add("Link sends event of unknown type for BFBB: " + link.EventSendID.ToString());
                }
            }
            else if (game == Game.Incredibles)
            {
                foreach (LinkTSSM link in LinksTSSM)
                {
                    if (link.TargetAssetID == 0)
                        result.Add("Link with Target Asset set to 0");
                    Verify(link.TargetAssetID, ref result);
                    Verify(link.ArgumentAssetID, ref result);
                    Verify(link.SourceCheckAssetID, ref result);

                    if (link.EventReceiveID == 0 || link.EventReceiveID.ToString() == ((int)link.EventReceiveID).ToString())
                        result.Add("Link receives event of unknown type for TSSM: " + link.EventReceiveID.ToString());
                    if (link.EventSendID == 0 || link.EventSendID.ToString() == ((int)link.EventSendID).ToString())
                        result.Add("Link sends event of unknown type for TSSM: " + link.EventSendID.ToString());
                }
            }

            if (!(this is AssetPLYR))
            {
                if (EventStartOffset + AmountOfEvents * Link.sizeOfStruct < Data.Length)
                    result.Add("Additional data found at the end of asset data");
                if (EventStartOffset + AmountOfEvents * Link.sizeOfStruct > Data.Length)
                    result.Add("Asset expects mode data than present");
            }
        }
    }
}