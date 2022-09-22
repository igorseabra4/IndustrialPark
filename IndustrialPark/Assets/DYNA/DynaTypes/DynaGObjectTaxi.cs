using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaGObjectTaxi : AssetDYNA
    {
        private const string dynaCategoryName = "game_object:Taxi";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => $"{HexUIntTypeConverter.StringFromAssetID(Marker)} {HexUIntTypeConverter.StringFromAssetID(Portal)}";

        protected override short constVersion => 1;

        [Category(dynaCategoryName), ValidReferenceRequired]
        public AssetID Marker { get; set; }
        [Category(dynaCategoryName), ValidReferenceRequired]
        public AssetID Camera { get; set; }
        [Category(dynaCategoryName), ValidReferenceRequired]
        public AssetID Portal { get; set; }
        [Category(dynaCategoryName), ValidReferenceRequired]
        public AssetID TalkBox { get; set; }
        [Category(dynaCategoryName), ValidReferenceRequired]
        public AssetID Text { get; set; }
        [Category(dynaCategoryName), ValidReferenceRequired]
        public AssetID SimpleObject { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle InvisibleTimer { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle TeleportTimer { get; set; }

        public DynaGObjectTaxi(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__Taxi, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                Marker = reader.ReadUInt32();
                Camera = reader.ReadUInt32();
                Portal = reader.ReadUInt32();
                TalkBox = reader.ReadUInt32();
                Text = reader.ReadUInt32();
                SimpleObject = reader.ReadUInt32();
                InvisibleTimer = reader.ReadSingle();
                TeleportTimer = reader.ReadSingle();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {

                writer.Write(Marker);
                writer.Write(Camera);
                writer.Write(Portal);
                writer.Write(TalkBox);
                writer.Write(Text);
                writer.Write(SimpleObject);
                writer.Write(InvisibleTimer);
                writer.Write(TeleportTimer);

                
        }
    }
}