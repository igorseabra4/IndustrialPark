using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaGObjectBoulderGen : AssetDYNA
    {
        private const string dynaCategoryName = "game_object:BoulderGenerator";

        protected override int constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetID ObjectAssetID { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle OffsetX { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle OffsetY { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle OffsetZ { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle OffsetRand { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle InitVelX { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle InitVelY { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle InitVelZ { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle VelAngleRand { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle VelMagRand { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle InitAxisX { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle InitAxisY { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle InitAxisZ { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle AngVel { get; set; }

        public DynaGObjectBoulderGen(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, DynaType.game_object__BoulderGenerator, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = dynaDataStartPosition;

            ObjectAssetID = reader.ReadUInt32();
            OffsetX = reader.ReadSingle();
            OffsetY = reader.ReadSingle();
            OffsetZ = reader.ReadSingle();
            OffsetRand = reader.ReadSingle();
            InitVelX = reader.ReadSingle();
            InitVelY = reader.ReadSingle();
            InitVelZ = reader.ReadSingle();
            VelAngleRand = reader.ReadSingle();
            VelMagRand = reader.ReadSingle();
            InitAxisX = reader.ReadSingle();
            InitAxisY = reader.ReadSingle();
            InitAxisZ = reader.ReadSingle();
            AngVel = reader.ReadSingle();
        }

        protected override byte[] SerializeDyna(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            
            writer.Write(ObjectAssetID);
            writer.Write(OffsetX);
            writer.Write(OffsetY);
            writer.Write(OffsetZ);
            writer.Write(OffsetRand);
            writer.Write(InitVelX);
            writer.Write(InitVelY);
            writer.Write(InitVelZ);
            writer.Write(VelAngleRand);
            writer.Write(VelMagRand);
            writer.Write(InitAxisX);
            writer.Write(InitAxisY);
            writer.Write(InitAxisZ);
            writer.Write(AngVel);

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID) => ObjectAssetID == assetID || base.HasReference(assetID);
        
        public override void Verify(ref List<string> result)
        {
            Verify(ObjectAssetID, ref result);
        }
    }
}