using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaGObjectDashCameraSpline : AssetDYNA
    {
        private const string dynaCategoryName = "game_object:dash_camera_spline";

        protected override short constVersion => 2;

        [Category(dynaCategoryName)]
        public AssetID Spline { get; set; }

        public DynaGObjectDashCameraSpline(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__dash_camera_spline, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;
                Spline = reader.ReadUInt32();
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(Spline);
                return writer.ToArray();
            }
        }

        public override void Verify(ref List<string> result)
        {
            Verify(Spline, ref result);
            base.Verify(ref result);
        }
    }
}