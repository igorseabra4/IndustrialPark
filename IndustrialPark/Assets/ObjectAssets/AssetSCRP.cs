using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;

namespace IndustrialPark
{
    public class AssetSCRP : BaseAsset
    {
        private const string categoryName = "Scripted Event";

        [Category(categoryName)]
        public float ScriptStartTime { get; set; }
        [Category(categoryName), TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flag1 { get; set; }
        [Category(categoryName), TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flag2 { get; set; }
        [Category(categoryName), TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flag3 { get; set; }
        [Category(categoryName), TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flag4 { get; set; }
        public Link[] _timedLinks;
        [Category(categoryName), Editor(typeof(LinkListEditor), typeof(UITypeEditor))]
        public Link[] TimedLinks
        {
            get
            {
                LinkListEditor.IsTimed = true;
                LinkListEditor.thisAssetID = AssetID;
                LinkListEditor.game = game;

                return _timedLinks;
            }
            set
            {
                _timedLinks = value;
            }
        }

        public AssetSCRP(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = baseEndPosition;

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
                _timedLinks[i] = new Link(reader, true, game);
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(SerializeBase(platform));

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
                writer.Write(l.Serialize(platform));

            writer.Write(SerializeLinks(platform));
            return writer.ToArray();
        }

        public override bool HasReference(uint assetID) => _timedLinks.Any(link => link.HasReference(assetID)) || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            foreach (Link link in _timedLinks)
            {
                Verify(link.TargetAssetID, ref result);
                Verify(link.ArgumentAssetID, ref result);

                if (link.EventSendID == 0 || link.EventSendID.ToString() == ((int)link.EventSendID).ToString())
                    result.Add("Timed link sends event of unknown type for TSSM: " + link.EventSendID.ToString());
            }
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