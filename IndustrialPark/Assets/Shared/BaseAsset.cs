using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;

namespace IndustrialPark
{
    public class BaseAsset : Asset
    {
        public BaseAsset(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            if (AssetID != AHDR.assetID)
                AssetID = AHDR.assetID;
        }

        private const string categoryName = "Base Asset";

        [Category(categoryName)]
        public virtual AssetID AssetID
        {
            get => ReadUInt(0);
            set => Write(0, value);
        }

        [Category(categoryName)]
        public ObjectAssetType AssetType
        {
            get => (ObjectAssetType)ReadByte(0x4);
            set => Write(0x4, (byte)value);
        }

        [Browsable(false)]
        public byte LinkCount => ReadByte(0x5);

        [Category(categoryName)]
        public DynamicTypeDescriptor Flags
        {
            get => ShortFlagsDescriptor(0x6,
                "Enabled On Start",
                "State Is Persistent",
                "Unknown Always True",
                "Visible During Cutscenes",
                "Receive Shadows");
        }

        [Browsable(false)]
        public ushort BaseUshortFlags
        {
            get => ReadUShort(0x6);
            set => Write(0x6, value);
        }

        protected virtual int EventStartOffset => Data.Length - LinkCount * Link.sizeOfStruct;

        [Category(categoryName), DisplayName("Links"), Editor(typeof(LinkListEditor), typeof(UITypeEditor))]
        public LinkBFBB[] LinksBFBB
        {
            get
            {
                LinkListEditor.IsTimed = false;
                LinkListEditor.endianness = EndianConverter.PlatformEndianness(platform);
                LinkListEditor.thisAssetID = AHDR.assetID;
                LinkBFBB[] events = new LinkBFBB[LinkCount];

                for (int i = 0; i < LinkCount; i++)
                    events[i] = new LinkBFBB(Data, EventStartOffset + i * Link.sizeOfStruct, false, EndianConverter.PlatformEndianness(platform));

                return events;
            }
            set => WriteEvents(value);
        }
        [Category(categoryName), DisplayName("Links"), Editor(typeof(LinkListEditor), typeof(UITypeEditor))]
        public LinkTSSM[] LinksTSSM
        {
            get
            {
                LinkListEditor.IsTimed = false;
                LinkListEditor.endianness = EndianConverter.PlatformEndianness(platform);
                LinkListEditor.thisAssetID = AHDR.assetID;
                LinkTSSM[] events = new LinkTSSM[LinkCount];

                for (int i = 0; i < LinkCount; i++)
                    events[i] = new LinkTSSM(Data, EventStartOffset + i * Link.sizeOfStruct, false, EndianConverter.PlatformEndianness(platform));

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
            if (this is AssetDUPC)
                newData[0x39] = (byte)value.Length;

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
                if (EventStartOffset + LinkCount * Link.sizeOfStruct < Data.Length)
                    result.Add("Additional data found at the end of asset data");
                if (EventStartOffset + LinkCount * Link.sizeOfStruct > Data.Length)
                    result.Add("Asset expects mode data than present");
            }
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game == Game.Incredibles)
                dt.RemoveProperty("LinksBFBB");
            else
                dt.RemoveProperty("LinksTSSM");
            
            base.SetDynamicProperties(dt);
        }
    }
}