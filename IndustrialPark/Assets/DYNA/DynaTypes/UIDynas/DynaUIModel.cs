using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaUIModel : DynaUI
    {
        private const string dynaCategoryName = "ui:model";

        protected override int constVersion => 2;

        [Category(dynaCategoryName)]
        public AssetID Model_AssetID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID AnimList_AssetID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Surface_AssetID { get; set; }

        public DynaUIModel(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, DynaType.ui__model, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = dynaUIEnd;

            Model_AssetID = reader.ReadUInt32();
            AnimList_AssetID = reader.ReadUInt32();
            Surface_AssetID = reader.ReadUInt32();
        }

        protected override byte[] SerializeDyna(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(SerializeDynaUI(platform));

            writer.Write(Model_AssetID);
            writer.Write(AnimList_AssetID);
            writer.Write(Surface_AssetID);

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID) =>
            Model_AssetID == assetID ||
            AnimList_AssetID == assetID ||
            Surface_AssetID == assetID ||
            base.HasReference(assetID);
    }
}