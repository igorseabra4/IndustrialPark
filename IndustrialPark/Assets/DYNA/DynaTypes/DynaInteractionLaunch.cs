using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaInteractionLaunch : AssetDYNA
    {
        private const string dynaCategoryName = "interaction:Launch";

        protected override short constVersion => 2;

        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_00 { get; set; }
        [Category(dynaCategoryName)]
        public AssetID SIMP_AssetID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Marker_AssetID { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_0C { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_10 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_14 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_18 { get; set; }

        public DynaInteractionLaunch(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.interaction__Launch, game, endianness)
        {
            var reader = new EndianBinaryReader(AHDR.data, endianness);
            reader.BaseStream.Position = dynaDataStartPosition;

            UnknownFloat_00 = reader.ReadSingle();
            SIMP_AssetID = reader.ReadUInt32();
            Marker_AssetID = reader.ReadUInt32();
            UnknownFloat_0C = reader.ReadSingle();
            UnknownFloat_10 = reader.ReadSingle();
            UnknownFloat_14 = reader.ReadSingle();
            UnknownFloat_18 = reader.ReadSingle();
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);

            writer.Write(UnknownFloat_00);
            writer.Write(SIMP_AssetID);
            writer.Write(Marker_AssetID);
            writer.Write(UnknownFloat_0C);
            writer.Write(UnknownFloat_10);
            writer.Write(UnknownFloat_14);
            writer.Write(UnknownFloat_18);

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID) =>
            SIMP_AssetID == assetID || Marker_AssetID == assetID || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            Verify(SIMP_AssetID, ref result);
            Verify(Marker_AssetID, ref result);
        }

    }
}