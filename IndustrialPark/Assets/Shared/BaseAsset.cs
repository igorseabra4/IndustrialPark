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
        private const string categoryName = "Base Asset";

        public Game game;

        [Category(categoryName)]
        public ObjectAssetType AssetType { get; set; }

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
                LinkListEditor.IsTimed = false;
                LinkListEditor.thisAssetID = AssetID;
                LinkListEditor.game = game;

                return _links;
            }
            set
            {
                _links = value;
            }
        }

        protected int baseEndPosition => 0x8;
        protected int linkStartPosition(long streamLength, int linkCount) => (int)(streamLength - linkCount * Link.sizeOfStruct);

        public BaseAsset(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR)
        {
            this.game = game;

            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = 0x4;

            AssetType = (ObjectAssetType)reader.ReadByte();
            byte LinkCount = reader.ReadByte();

            BaseFlags.FlagValueShort = reader.ReadUInt16();

            reader.BaseStream.Position = linkStartPosition(reader.BaseStream.Length, LinkCount);

            if (this is AssetPLYR)
                reader.BaseStream.Position -= 4;
            
            _links = new Link[LinkCount];
            for (int i = 0; i < _links.Length; i++)
                _links[i] = new Link(reader, false, game);
        }

        // meant for use with DUPC VIL only
        protected BaseAsset(EndianBinaryReader reader)
        {
            AssetID = reader.ReadUInt32();
            AssetType = (ObjectAssetType)reader.ReadByte();
            reader.ReadByte();
            BaseFlags.FlagValueShort = reader.ReadUInt16();
            _links = new Link[0];
        }

        public byte[] SerializeBase(Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            writer.Write(AssetID);
            writer.Write((byte)AssetType);
            writer.Write((byte)_links.Length);
            writer.Write(BaseFlags.FlagValueShort);

            return writer.ToArray();
        }

        public byte[] SerializeLinks(Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            foreach(var l in _links)
                writer.Write(l.Serialize(platform));

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