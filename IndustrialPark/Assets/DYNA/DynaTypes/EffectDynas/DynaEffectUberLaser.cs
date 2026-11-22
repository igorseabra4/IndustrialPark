using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark.Assets
{
    public class DynaEffectUberLaser : AssetDYNA
    {
        private const string dynaCategoryName = "effect:uber_laser";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(MarkerStart);

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetID MarkerStart { get; set; }
        [Category(dynaCategoryName)]
        public AssetID MarkerEnd { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Time { get; set; }

        public DynaEffectUberLaser(Section_AHDR AHDR, EndianBinaryReader reader) : base(AHDR, DynaType.effect__uber_laser, reader)
        {
            MarkerStart = reader.ReadUInt32();
            MarkerEnd = reader.ReadUInt32();
            Time = reader.ReadSingle();
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {

            writer.Write(MarkerStart);
            writer.Write(MarkerEnd);
            writer.Write(Time);


        }
    }
}