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
        public AssetSingle X { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Y { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Z { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle BlendTime { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle FovFilterPeriod { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle StartFov { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle EndFov { get; set; }

        public DynaGObjectCameraParamAsset(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__camera_param_asset, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                X = reader.ReadSingle();
                Y = reader.ReadSingle();
                Z = reader.ReadSingle();
                BlendTime = reader.ReadSingle();
                FovFilterPeriod = reader.ReadSingle();
                StartFov = reader.ReadSingle();
                EndFov = reader.ReadSingle();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {

            writer.Write(X);
            writer.Write(Y);
            writer.Write(Z);
            writer.Write(BlendTime);
            writer.Write(FovFilterPeriod);
            writer.Write(StartFov);
            writer.Write(EndFov);


        }
    }
}