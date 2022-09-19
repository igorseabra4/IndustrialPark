using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public enum PlayerEnum
    {
        Patrick = 0,
        Sandy = 1
    }

    public class DynaGObjectBusStop : AssetDYNA
    {
        private const string dynaCategoryName = "game_object:BusStop";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(Marker);

        protected override short constVersion => 2;

        [Category(dynaCategoryName)]
        public AssetID Marker { get; set; }
        [Category(dynaCategoryName)]
        public PlayerEnum Player { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Camera { get; set; }
        [Category(dynaCategoryName)]
        public AssetID SimpleObject { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Delay { get; set; }

        public DynaGObjectBusStop(string assetName, uint mrkrAssetId, uint camAssetId, uint simpAssetId) : base(assetName, DynaType.game_object__BusStop)
        {
            Marker = mrkrAssetId;
            Camera = camAssetId;
            SimpleObject = simpAssetId;

            Delay = 1.5f;
        }

        public DynaGObjectBusStop(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__BusStop, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                Marker = reader.ReadUInt32();
                Player = (PlayerEnum)reader.ReadInt32();
                Camera = reader.ReadUInt32();
                SimpleObject = reader.ReadUInt32();
                Delay = reader.ReadSingle();
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(Marker);
                writer.Write((int)Player);
                writer.Write(Camera);
                writer.Write(SimpleObject);
                writer.Write(Delay);

                return writer.ToArray();
            }
        }

        public override void Verify(ref List<string> result)
        {
            if (Marker == 0)
                result.Add("Bus stop with no MRKR reference");
            Verify(Marker, ref result);
            if (Camera == 0)
                result.Add("Bus stop with no CAM reference");
            Verify(Camera, ref result);
            if (SimpleObject == 0)
                result.Add("Bus stop with no SIMP reference");
            Verify(SimpleObject, ref result);

            base.Verify(ref result);
        }
    }
}