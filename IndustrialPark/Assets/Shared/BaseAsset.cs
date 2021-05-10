using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System;

namespace IndustrialPark
{
    public abstract class BaseAsset : Asset
    {
        private const string categoryName = "Base";

        [Category(categoryName)]
        public BaseAssetType BaseAssetType { get; set; }

        [Category(categoryName)]
        public FlagBitmask BaseFlags { get; set; } = ShortFlagsDescriptor(
                "Enabled On Start",
                "State Is Persistent",
                "Unknown Always True",
                "Visible During Cutscenes",
                "Receive Shadows");

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
            (int)(streamLength - linkCount * Link.sizeOfStruct - (this is AssetPLYR ? 4 : 0));

        public BaseAsset(string assetName, AssetType assetType, BaseAssetType baseAssetType) : base(assetName, assetType)
        {
            BaseAssetType = baseAssetType;
            BaseFlags.FlagValueShort = 0x1D;
            _links = new Link[0];
        }

        public BaseAsset(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            var reader = new EndianBinaryReader(AHDR.data, endianness);
            reader.BaseStream.Position = 0x4;

            BaseAssetType = (BaseAssetType)reader.ReadByte();
            byte LinkCount = reader.ReadByte();

            BaseFlags.FlagValueShort = reader.ReadUInt16();

            reader.BaseStream.Position = linkStartPosition(reader.BaseStream.Length, LinkCount);
                        
            _links = new Link[LinkCount];
            for (int i = 0; i < _links.Length; i++)
                _links[i] = new Link(reader, LinkType.Normal, game);
        }

        // meant for use with DUPC VIL only
        protected BaseAsset(EndianBinaryReader reader)
        {
            assetID = reader.ReadUInt32();
            BaseAssetType = (BaseAssetType)reader.ReadByte();
            reader.ReadByte();
            BaseFlags.FlagValueShort = reader.ReadUInt16();
            _links = new Link[0];
        }

        protected byte[] SerializeBase(Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);

            writer.Write(assetID);
            writer.Write((byte)BaseAssetType);
            writer.Write((byte)_links.Length);
            writer.Write(BaseFlags.FlagValueShort);

            return writer.ToArray();
        }

        public byte[] SerializeLinks(Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);

            foreach(var l in _links)
                writer.Write(l.Serialize(LinkType.Normal, endianness));

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID) => _links.Any(link => link.HasReference(assetID));

        public override void Verify(ref List<string> result)
        {
            foreach (Link link in _links)
            {
                if (link.TargetAssetID == 0)
                    result.Add("Link with Target Asset set to 0");
                Verify(link.TargetAssetID, ref result);
                Verify(link.ArgumentAssetID, ref result);
                Verify(link.SourceCheckAssetID, ref result);

                if (link.EventReceiveID == 0 || link.EventReceiveID > Enum.GetValues(game == Game.Incredibles ? typeof(EventTSSM) : typeof(EventBFBB)).Length)
                    result.Add("Link receives event of unknown type: " + link.EventReceiveID.ToString());
                if (link.EventSendID == 0 || link.EventSendID > Enum.GetValues(game == Game.Incredibles ? typeof(EventTSSM) : typeof(EventBFBB)).Length)
                    result.Add("Link sends event of unknown type: " + link.EventSendID.ToString());
            }
        }
    }
}