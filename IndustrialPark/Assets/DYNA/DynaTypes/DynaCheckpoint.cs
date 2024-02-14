using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaCheckpoint : AssetDYNA
    {
        private const string dynaCategoryName = "Checkpoint";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(Trigger);

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public uint Number { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Trigger { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Script { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Camera { get; set; }
        [Category(dynaCategoryName)]
        public AssetID PointerMrIncredible { get; set; }
        [Category(dynaCategoryName)]
        public AssetID PointerFrozone { get; set; }

        public DynaCheckpoint(string assetName) : base(assetName, DynaType.Checkpoint)
        {
        }

        public DynaCheckpoint(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.Checkpoint, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                Number = reader.ReadUInt32();
                Trigger = reader.ReadUInt32();
                Script = reader.ReadUInt32();
                Camera = reader.ReadUInt32();
                PointerMrIncredible = reader.ReadUInt32();
                PointerFrozone = reader.ReadUInt32();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(Number);
            writer.Write(Trigger);
            writer.Write(Script);
            writer.Write(Camera);
            writer.Write(PointerMrIncredible);
            writer.Write(PointerFrozone);
        }
    }
}