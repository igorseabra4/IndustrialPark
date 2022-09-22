using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaGObjectCameraParamAsset : AssetDYNA
    {
        private const string dynaCategoryName = "game_object:camera_param_asset";
        public override string TypeString => dynaCategoryName;

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat00 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat04 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat08 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat0C { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat10 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat14 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat18 { get; set; }

        public DynaGObjectCameraParamAsset(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__camera_param_asset, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                UnknownFloat00 = reader.ReadSingle();
                UnknownFloat04 = reader.ReadSingle();
                UnknownFloat08 = reader.ReadSingle();
                UnknownFloat0C = reader.ReadSingle();
                UnknownFloat10 = reader.ReadSingle();
                UnknownFloat14 = reader.ReadSingle();
                UnknownFloat18 = reader.ReadSingle();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {

                writer.Write(UnknownFloat00);
                writer.Write(UnknownFloat04);
                writer.Write(UnknownFloat08);
                writer.Write(UnknownFloat0C);
                writer.Write(UnknownFloat10);
                writer.Write(UnknownFloat14);
                writer.Write(UnknownFloat18);

                
        }
    }
}