using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using static HipHopFile.Functions;

namespace IndustrialPark
{
    public class AssetSCRP : BaseAsset
    {
        public AssetSCRP(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform) { }

        public override bool HasReference(uint assetID)
        {
            foreach (Link link in TimedLinksBFBB)
                if (link.TargetAssetID == assetID || link.ArgumentAssetID == assetID || link.SourceCheckAssetID == assetID)
                    return true;
            
            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            foreach (LinkTSSM link in TimedLinksTSSM)
            {
                Verify(link.TargetAssetID, ref result);
                Verify(link.ArgumentAssetID, ref result);

                if (link.EventSendID == 0 || link.EventSendID.ToString() == ((int)link.EventSendID).ToString())
                    result.Add("Timed link sends event of unknown type for TSSM: " + link.EventSendID.ToString());
            }
        }

        [Category("Scripted Event")]
        public float UnknownFloat08
        {
            get => ReadFloat(0x08);
            set => Write(0x08, value);
        }

        [Category("Scripted Event"), ReadOnly(true)]
        public int TimedLinkCount
        {
            get => ReadInt(0x0C);
            set => Write(0x0C, value);
        }

        [Category("Scripted Event"), Description("Movie only."), TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flag1
        {
            get => ReadByte(0x10);
            set => Write(0x10, value);
        }

        [Category("Scripted Event"), Description("Movie only."), TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flag2
        {
            get => ReadByte(0x11);
            set => Write(0x11, value);
        }

        [Category("Scripted Event"), Description("Movie only."), TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flag3
        {
            get => ReadByte(0x12);
            set => Write(0x12, value);
        }

        [Category("Scripted Event"), Description("Movie only."), TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flag4
        {
            get => ReadByte(0x13);
            set => Write(0x13, value);
        }

        private int TimedLinksStartOffset => game == Game.Incredibles ? 0x14 : 0x10;

        private void WriteTimedLinks(Link[] links)
        {
            List<byte> newData = Data.Take(TimedLinksStartOffset).ToList();
            List<byte> restOfOldData = Data.Skip(TimedLinksStartOffset + Link.sizeOfStruct * TimedLinkCount).ToList();

            foreach (Link i in links)
                newData.AddRange(i.ToByteArray());

            newData.AddRange(restOfOldData);
            Data = newData.ToArray();

            TimedLinkCount = links.Length;
        }

        
        [Category("Scripted Event"), Editor(typeof(LinkListEditor), typeof(UITypeEditor))]
        public LinkBFBB[] TimedLinksBFBB
        {
            get
            {
                LinkBFBB[] events = new LinkBFBB[TimedLinkCount];

                for (int i = 0; i < TimedLinkCount; i++)
                    events[i] = new LinkBFBB(Data, TimedLinksStartOffset + i * Link.sizeOfStruct, true, EndianConverter.PlatformEndianness(platform));

                LinkListEditor.IsTimed = true;
                LinkListEditor.thisAssetID = AHDR.assetID;
                LinkListEditor.endianness = EndianConverter.PlatformEndianness(platform);
                return events;
            }
            set
            {
                WriteTimedLinks(value);
            }
        }

        [Category("Scripted Event"), Editor(typeof(LinkListEditor), typeof(UITypeEditor))]
        public LinkTSSM[] TimedLinksTSSM
        {
            get
            {
                LinkTSSM[] events = new LinkTSSM[TimedLinkCount];

                for (int i = 0; i < TimedLinkCount; i++)
                    events[i] = new LinkTSSM(Data, TimedLinksStartOffset + i * Link.sizeOfStruct, true, EndianConverter.PlatformEndianness(platform));

                LinkListEditor.IsTimed = true;
                LinkListEditor.thisAssetID = AHDR.assetID;
                LinkListEditor.endianness = EndianConverter.PlatformEndianness(platform);
                return events;
            }
            set
            {
                WriteTimedLinks(value);
            }
        }
    }
}