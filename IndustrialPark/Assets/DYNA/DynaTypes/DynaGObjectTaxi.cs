using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaGObjectTaxi : AssetDYNA
    {
        private const string dynaCategoryName = "game_object:Taxi";

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetID Marker { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Camera { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Portal { get; set; }
        [Category(dynaCategoryName)]
        public AssetID TalkBox { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Text { get; set; }
        [Category(dynaCategoryName)]
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

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(Marker);
                writer.Write(Camera);
                writer.Write(Portal);
                writer.Write(TalkBox);
                writer.Write(Text);
                writer.Write(SimpleObject);
                writer.Write(InvisibleTimer);
                writer.Write(TeleportTimer);

                return writer.ToArray();
            }
        }

        public override void Verify(ref List<string> result)
        {
            Verify(Marker, ref result);
            Verify(Camera, ref result);
            Verify(Portal, ref result);
            Verify(TalkBox, ref result);
            Verify(Text, ref result);
            Verify(SimpleObject, ref result);
            base.Verify(ref result);
        }
    }
}