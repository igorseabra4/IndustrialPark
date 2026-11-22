using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark.Assets
{
    public class DynaGObjectDashCameraSpline : AssetDYNA
    {
        private const string dynaCategoryName = "game_object:dash_camera_spline";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(Spline);

        protected override short constVersion => 2;

        [Category(dynaCategoryName), ValidReferenceRequired]
        public AssetID Spline { get; set; }

        public DynaGObjectDashCameraSpline(Section_AHDR AHDR, EndianBinaryReader reader) : base(AHDR, DynaType.game_object__dash_camera_spline, reader)
        {
            Spline = reader.ReadUInt32();
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(Spline);
        }
    }
}