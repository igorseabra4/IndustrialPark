using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaEffectRumbleSphere : AssetDYNA
    {
        private const string dynaCategoryName = "effect:Rumble Spherical Emitter";

        protected override int constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetID Rumble_AssetID { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_04 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_08 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_0C { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_10 { get; set; }
        [Category(dynaCategoryName)]
        public short UnknownShort_14 { get; set; }
        [Category(dynaCategoryName)]
        public short UnknownShort_16 { get; set; }

        public DynaEffectRumbleSphere(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, DynaType.effect__RumbleSphericalEmitter, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = dynaDataStartPosition;

            Rumble_AssetID = reader.ReadUInt32();
            UnknownFloat_04 = reader.ReadSingle();
            UnknownFloat_08 = reader.ReadSingle();
            UnknownFloat_0C = reader.ReadSingle();
            UnknownFloat_10 = reader.ReadSingle();
            UnknownShort_14 = reader.ReadInt16();
            UnknownShort_16 = reader.ReadInt16();
        }

        protected override byte[] SerializeDyna(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            writer.Write(Rumble_AssetID);
            writer.Write(UnknownFloat_04);
            writer.Write(UnknownFloat_08);
            writer.Write(UnknownFloat_0C);
            writer.Write(UnknownFloat_10);
            writer.Write(UnknownShort_14);
            writer.Write(UnknownShort_16);

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID) => Rumble_AssetID == assetID || base.HasReference(assetID);
        
        public override void Verify(ref List<string> result)
        {
            Verify(Rumble_AssetID, ref result);
        }
    }
}