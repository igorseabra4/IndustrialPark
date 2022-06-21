using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;

namespace IndustrialPark
{
    public abstract class BaseAsset : Asset
    {
        private const string categoryName = "Base";

        [Category(categoryName)]
        public BaseAssetType BaseAssetType { get; set; }

        [Category(categoryName), Browsable(false)]
        public ushort BaseFlags { get; set; }
        //ShortFlagsDescriptor(
        //        "Enabled On Start",
        //        "State Is Persistent",
        //        "Unknown Always True",
        //        "Visible During Cutscenes",
        //        "Receive Shadows");

        [Category(categoryName)]
        public bool EnabledOnStart
        {
            get => (BaseFlags & 1) != 0;
            set
            {
                if (value)
                    BaseFlags |= 1;
                else
                    BaseFlags &= ushort.MaxValue - 1;
            }
        }
        [Category(categoryName)]
        public bool StateIsPersistent
        {
            get => (BaseFlags & 2) != 0;
            set
            {
                if (value)
                    BaseFlags |= 2;
                else
                    BaseFlags &= ushort.MaxValue - 2;
            }
        }
        [Category(categoryName)]
        public bool UnknownAlwaysTrue
        {
            get => (BaseFlags & 4) != 0;
            set
            {
                if (value)
                    BaseFlags |= 4;
                else
                    BaseFlags &= ushort.MaxValue - 4;
            }
        }
        [Category(categoryName)]
        public bool VisibleDuringCutscenes
        {
            get => (BaseFlags & 8) != 0;
            set
            {
                if (value)
                    BaseFlags |= 8;
                else
                    BaseFlags &= ushort.MaxValue - 8;
            }
        }
        [Category(categoryName)]
        public bool ReceiveShadows
        {
            get => (BaseFlags & 16) != 0;
            set
            {
                if (value)
                    BaseFlags |= 16;
                else
                    BaseFlags &= ushort.MaxValue - 16;
            }
        }

        protected Link[] _links;
        [Category(categoryName), Editor(typeof(LinkListEditor), typeof(UITypeEditor))]
        public Link[] Links
        {
            get
            {
                LinkListEditor.LinkType = LinkType.Normal;
                LinkListEditor.ThisAssetID = assetID;
                LinkListEditor.Game = game;

                return _links;
            }
            set
            {
                _links = value;
            }
        }

        protected int baseHeaderEndPosition => 8;

        protected int linkStartPosition(long streamLength, int linkCount) =>
            (int)(streamLength - linkCount * Link.sizeOfStruct - (this is AssetPLYR && game != Game.Scooby ? 4 : 0));

        public BaseAsset(string assetName, AssetType assetType, BaseAssetType baseAssetType) : base(assetName, assetType)
        {
            BaseAssetType = baseAssetType;
            BaseFlags = 0x1D;
            _links = new Link[0];
        }

        public BaseAsset(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = 0x4;

                BaseAssetType = (BaseAssetType)reader.ReadByte();
                byte LinkCount = reader.ReadByte();

                BaseFlags = reader.ReadUInt16();

                reader.BaseStream.Position = linkStartPosition(reader.BaseStream.Length, LinkCount);

                _links = new Link[LinkCount];
                for (int i = 0; i < _links.Length; i++)
                    _links[i] = new Link(reader, LinkType.Normal, game);
            }
        }

        // meant for use with DUPC VIL only
        protected BaseAsset(EndianBinaryReader reader)
        {
            assetID = reader.ReadUInt32();
            BaseAssetType = (BaseAssetType)reader.ReadByte();
            reader.ReadByte();
            BaseFlags = reader.ReadUInt16();
            _links = new Link[0];
        }

        protected byte[] SerializeBase(Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(assetID);
                writer.Write((byte)BaseAssetType);
                writer.Write((byte)_links.Length);
                writer.Write(BaseFlags);

                return writer.ToArray();
            }
        }

        public byte[] SerializeLinks(Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                foreach (var l in _links)
                    writer.Write(l.Serialize(LinkType.Normal, endianness));

                return writer.ToArray();
            }
        }

        public override void Verify(ref List<string> result)
        {
            foreach (Link link in _links)
            {
                if (link.TargetAsset == 0)
                    result.Add("Link with Target Asset set to 0");
                Verify(link.TargetAsset, ref result);
                Verify(link.ArgumentAsset, ref result);
                Verify(link.SourceCheckAsset, ref result);

                var eventCount = Enum.GetValues(game == Game.Scooby ? typeof(EventScooby) : game == Game.BFBB ? typeof(EventBFBB) : typeof(EventTSSM)).Length;

                if (link.EventReceiveID == 0 || link.EventReceiveID > eventCount)
                    result.Add("Link receives event of unknown type: " + link.EventReceiveID.ToString());
                if (link.EventSendID == 0 || link.EventSendID > eventCount)
                    result.Add("Link sends event of unknown type: " + link.EventSendID.ToString());
            }
        }
    }
}