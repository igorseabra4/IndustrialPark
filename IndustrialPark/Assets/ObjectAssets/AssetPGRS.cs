using HipHopFile;
using System.ComponentModel;
using System.Drawing.Design;

namespace IndustrialPark
{
    public class AssetPGRS : BaseAsset
    {
        private const string categoryName = "Progress Script";
        public override string AssetInfo => ItemsString(_progressLinks.Length, "progress link");

        public Link[] _progressLinks;
        [Category(categoryName), Editor(typeof(LinkListEditor), typeof(UITypeEditor))]
        public Link[] ProgressLinks
        {
            get
            {
                LinkListEditor.LinkType = LinkType.Progress;
                LinkListEditor.ThisAssetID = assetID;
                LinkListEditor.Game = game;

                return _progressLinks;
            }
            set
            {
                _progressLinks = value;
            }
        }

        public AssetPGRS(string assetName) : base(assetName, AssetType.ProgressScript, BaseAssetType.ProgressScript)
        {
            _progressLinks = new Link[0];
        }

        public AssetPGRS(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                int progressLinkCount = reader.ReadInt32();

                _progressLinks = new Link[progressLinkCount];
                for (int i = 0; i < _progressLinks.Length; i++)
                    _progressLinks[i] = new Link(reader, LinkType.Progress, game);
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(_progressLinks.Length);

            foreach (var l in _progressLinks)
                l.Serialize(LinkType.Progress, writer);
            SerializeLinks(writer);
        }
    }
}