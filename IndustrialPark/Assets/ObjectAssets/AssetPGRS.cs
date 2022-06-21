using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;

namespace IndustrialPark
{
    public class AssetPGRS : BaseAsset
    {
        private const string categoryName = "Progress Script";

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

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeBase(endianness));
                writer.Write(_progressLinks.Length);

                foreach (var l in _progressLinks)
                    writer.Write(l.Serialize(LinkType.Progress, endianness));
                writer.Write(SerializeLinks(endianness));
                return writer.ToArray();
            }
        }


        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            foreach (Link link in _progressLinks)
            {
                Verify(link.TargetAsset, ref result);
                Verify(link.ArgumentAsset, ref result);

                if (link.EventSendID == 0 || ((EventTSSM)link.EventSendID).ToString() == link.EventSendID.ToString())
                    result.Add("Progress link sends event of unknown type for TSSM: " + link.EventSendID.ToString());
            }
        }
    }
}