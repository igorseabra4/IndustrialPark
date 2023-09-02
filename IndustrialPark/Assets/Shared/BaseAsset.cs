using HipHopFile;
using System.ComponentModel;
using System.Drawing.Design;

namespace IndustrialPark
{
    public abstract class BaseAsset : Asset
    {
        private const string categoryName = "\t\t\t\t\tBase";

        public override string AssetInfoLinks => _links.Length.ToString();

        protected string ItemsString(int count, string name)
        {
            if (count == 1)
                return $"1 {name}";
            return $"{count} {name}s";
        }

        [Category(categoryName), ReadOnly(true)]
        public BaseAssetType BaseAssetType { get; set; }

        [Category(categoryName), Browsable(false)]
        public ushort BaseFlags { get; set; }
        //ShortFlagsDescriptor(
        //        "Enabled On Start",
        //        "State Is Persistent",
        //        "Unknown Always True",
        //        "Visible During Cutscenes",
        //        "Receive Shadows");

        [Category(categoryName), DisplayName("Enabled On Start")]
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
        [Category(categoryName), DisplayName("Persistent"), Description("Asset state is saved to the save file and persists after deaths and reloads.")]
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
        [Category(categoryName), DisplayName("Unknown"), Description("Always true")]
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
        [Category(categoryName), DisplayName("Visible During Cutscenes")]
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
        [Category(categoryName), DisplayName("Receive Shadows")]
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

        protected const int baseHeaderEndPosition = 8;

        protected int linkStartPosition(long streamLength, int linkCount) =>
            (int)(streamLength - linkCount * Link.sizeOfStruct - (this is AssetPLYR && game != Game.Scooby ? 4 : 0));

        public BaseAsset(string assetName, AssetType assetType, BaseAssetType baseAssetType) : base(assetName, assetType)
        {
            BaseAssetType = baseAssetType;
            BaseFlags = 0x1D;
            _links = new Link[0];
        }

        public BaseAsset(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game)
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
        protected BaseAsset(EndianBinaryReader reader, Game game)
        {
            _game = game;
            assetID = reader.ReadUInt32();
            BaseAssetType = (BaseAssetType)reader.ReadByte();
            reader.ReadByte();
            BaseFlags = reader.ReadUInt16();
            _links = new Link[0];
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(assetID);
            writer.Write((byte)BaseAssetType);
            writer.Write((byte)_links.Length);
            writer.Write(BaseFlags);
        }

        protected void SerializeLinks(EndianBinaryWriter writer)
        {
            foreach (var l in _links)
                l.Serialize(LinkType.Normal, writer);
        }
    }
}