using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;

namespace IndustrialPark
{
    public class AssetPGRS : BaseAsset
    {
        private const string categoryName = "Scripted Event";

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

        public override bool HasReference(uint assetID) => _progressLinks.Any(link => link.HasReference(assetID)) || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            foreach (Link link in _progressLinks)
            {
                Verify(link.TargetAssetID, ref result);
                Verify(link.ArgumentAssetID, ref result);

                if (link.EventSendID == 0 || link.EventSendID.ToString() == ((int)link.EventSendID).ToString())
                    result.Add("Progress link sends event of unknown type for TSSM: " + link.EventSendID.ToString());
            }
        }
    }
}