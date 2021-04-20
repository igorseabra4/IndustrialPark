using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public enum EnemyCritterType : uint
    {
        jellyfish_v1_bind = 0x878C2B70,
        jellybucket_v1_bind = 0xA320F2AE
    }

    public class DynaEnemyCritter : DynaEnemySB
    {
        private const string dynaCategoryName = "Enemy:SB:Critter";

        protected override int constVersion => 2;

        [Category(dynaCategoryName)]
        public EnemyCritterType CritterType
        {
            get => (EnemyCritterType)(uint)Model_AssetID;
            set => Model_AssetID = (uint)value;
        }
        [Category(dynaCategoryName)]
        public AssetID MVPT_AssetID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Unknown54 { get; set; }

        public DynaEnemyCritter(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, DynaType.Enemy__SB__Critter, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = entityDynaEndPosition;

            MVPT_AssetID = reader.ReadUInt32();
            Unknown54 = reader.ReadUInt32();
        }

        protected override byte[] SerializeDyna(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(SerializeEntityDyna(platform));
            writer.Write(MVPT_AssetID);
            writer.Write(Unknown54);
            return writer.ToArray();
        }

        public override bool HasReference(uint assetID)
        {
            if (MVPT_AssetID == assetID)
                return true;
            if (Unknown54 == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            if (MVPT_AssetID == 0)
                result.Add("DYNA Critter with MVPT Asset ID set to 0");

            Verify(MVPT_AssetID, ref result);
            Verify(Unknown54, ref result);
        }
    }
}