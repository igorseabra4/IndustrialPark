using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;

namespace IndustrialPark
{
    public class AssetSCRP : BaseAsset
    {
        private const string categoryName = "Timed Script";
        public override string AssetInfo => ItemsString(_timedLinks.Length, "timed link");

        [Category(categoryName)]
        public AssetSingle ScriptStartTime { get; set; }
        [Category(categoryName)]
        public AssetByte Flag1 { get; set; }
        [Category(categoryName)]
        public AssetByte Flag2 { get; set; }
        [Category(categoryName)]
        public AssetByte Flag3 { get; set; }
        [Category(categoryName)]
        public AssetByte Flag4 { get; set; }
        private Link[] _timedLinks;
        [Category(categoryName), Editor(typeof(LinkListEditor), typeof(UITypeEditor))]
        public Link[] TimedLinks
        {
            get
            {
                LinkListEditor.LinkType = LinkType.Timed;
                LinkListEditor.ThisAssetID = assetID;
                LinkListEditor.Game = game;

                return _timedLinks;
            }
            set
            {
                _timedLinks = value;
            }
        }

        public AssetSCRP(string assetName) : base(assetName, AssetType.Script, BaseAssetType.Script)
        {
            _timedLinks = new Link[0];
            ScriptStartTime = 1f;
        }

        public AssetSCRP(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                ScriptStartTime = reader.ReadSingle();
                int timedLinkCount = reader.ReadInt32();

                if (game == Game.Incredibles)
                {
                    Flag1 = reader.ReadByte();
                    Flag2 = reader.ReadByte();
                    Flag3 = reader.ReadByte();
                    Flag4 = reader.ReadByte();
                }

                _timedLinks = new Link[timedLinkCount];
                for (int i = 0; i < _timedLinks.Length; i++)
                    _timedLinks[i] = new Link(reader, LinkType.Timed, game);
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(ScriptStartTime);
            writer.Write(_timedLinks.Length);

            if (game == Game.Incredibles)
            {
                writer.Write(Flag1);
                writer.Write(Flag2);
                writer.Write(Flag3);
                writer.Write(Flag4);
            }

            foreach (var l in _timedLinks)
                l.Serialize(LinkType.Timed, writer);
            SerializeLinks(writer);
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game != Game.Incredibles)
            {
                dt.RemoveProperty("Flag1");
                dt.RemoveProperty("Flag2");
                dt.RemoveProperty("Flag3");
                dt.RemoveProperty("Flag4");
            }

            base.SetDynamicProperties(dt);
        }
    }
}