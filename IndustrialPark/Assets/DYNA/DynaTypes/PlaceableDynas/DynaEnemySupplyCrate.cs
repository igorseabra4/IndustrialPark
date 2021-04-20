using System.Collections.Generic;
using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public enum EnemySupplyCrateType : uint
    {
        crate_wood_bind = 0x71A87E7B,
        crate_hover_bind = 0xEE848998,
        crate_explode_bind = 0x8A54F14F,
        crate_shrink_bind = 0xAA6440C3,
        crate_steel_bind = 0xB3384F91
    }

    public class DynaEnemySupplyCrate : DynaEnemySB
    {
        private const string dynaCategoryName = "Enemy:SB:SupplyCrate";

        protected override int constVersion => 2;

        [Category(dynaCategoryName)]
        public EnemySupplyCrateType CrateType
        {
            get => (EnemySupplyCrateType)(uint)Model_AssetID;
            set => Model_AssetID = (uint)value;
        }
        [Category(dynaCategoryName)]
        public AssetID MVPT_AssetID { get; set; }

        public DynaEnemySupplyCrate(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, DynaType.Enemy__SB__SupplyCrate, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = entityDynaEndPosition;

            MVPT_AssetID = reader.ReadUInt32();
        }

        protected override byte[] SerializeDyna(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(SerializeEntityDyna(platform));

            writer.Write(MVPT_AssetID);

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID)
        {
            if (MVPT_AssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);
            Verify(MVPT_AssetID, ref result);
        }
    }
}