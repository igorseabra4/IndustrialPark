using AssetEditorColors;
using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaGObjectGrapple : AssetDYNA
    {
        private const string dynaCategoryName = "game_object:Grapple";

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetID Object_AssetID { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownX { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownY { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownZ { get; set; }
        [Category(dynaCategoryName)]
        public FlagBitmask GrappleFlags { get; set; } = IntFlagsDescriptor();

        public DynaGObjectGrapple(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__Grapple, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                Object_AssetID = reader.ReadUInt32();
                UnknownX = reader.ReadSingle();
                UnknownY = reader.ReadSingle();
                UnknownZ = reader.ReadSingle();
                GrappleFlags.FlagValueInt = reader.ReadUInt32();
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(Object_AssetID);
                writer.Write(UnknownX);
                writer.Write(UnknownY);
                writer.Write(UnknownZ);
                writer.Write(GrappleFlags.FlagValueInt);

                return writer.ToArray();
            }
        }

        public override bool HasReference(uint assetID) => Object_AssetID == assetID || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            Verify(Object_AssetID, ref result);
            base.Verify(ref result);
        }
    }
}