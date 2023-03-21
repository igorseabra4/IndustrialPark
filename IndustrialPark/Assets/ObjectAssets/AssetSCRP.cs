using HipHopFile;
using System;
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
                LinkListEditor.LinkType = TimedLinkType;
                LinkListEditor.ThisAssetID = assetID;
                LinkListEditor.Game = game;

                return _timedLinks;
            }
            set
            {
                _timedLinks = value;
            }
        }

        [Category(categoryName)]
        public EVersionROTUOthers AssetVersion { get; set; } = EVersionROTUOthers.Others;

        private LinkType TimedLinkType => AssetVersion == EVersionROTUOthers.ROTU ? LinkType.TimedRotu : LinkType.Timed;

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

                if (timedLinkCount != 0)
                {
                    int timedLinkSize = (int)((reader.BaseStream.Length - reader.BaseStream.Position - Link.sizeOfStruct * _links.Length) / timedLinkCount);

                    if (timedLinkSize == 0x24)
                        AssetVersion = EVersionROTUOthers.ROTU;
                    else if (timedLinkSize == 0x20)
                        AssetVersion = EVersionROTUOthers.Others;
                    else
                        throw new Exception("Unsupported format");
                }

                _timedLinks = new Link[timedLinkCount];
                for (int i = 0; i < _timedLinks.Length; i++)
                    _timedLinks[i] = new Link(reader, TimedLinkType, game);
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
                l.Serialize(TimedLinkType, writer);
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