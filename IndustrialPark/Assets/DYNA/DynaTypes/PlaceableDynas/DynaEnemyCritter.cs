using HipHopFile;
using SharpDX;
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

        protected override short constVersion => 2;

        [Category(dynaCategoryName)]
        public EnemyCritterType CritterType
        {
            get => (EnemyCritterType)(uint)Model;
            set => Model = (uint)value;
        }
        [Category(dynaCategoryName)]
        public AssetID MovePoint { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Unknown54 { get; set; }

        public DynaEnemyCritter(string assetName, AssetTemplate template, Vector3 position, uint mvptAssetID) : base(assetName, DynaType.Enemy__SB__Critter, 2, position)
        {
            BaseFlags = 0x0D;

            CritterType =
            template == AssetTemplate.Jellyfish ? EnemyCritterType.jellyfish_v1_bind :
            template == AssetTemplate.Jellyfish_Bucket ? EnemyCritterType.jellybucket_v1_bind : 0;

            MovePoint = mvptAssetID;
        }

        public DynaEnemyCritter(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.Enemy__SB__Critter, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = entityDynaEndPosition;

                MovePoint = reader.ReadUInt32();
                Unknown54 = reader.ReadUInt32();
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeEntityDyna(endianness));
                writer.Write(MovePoint);
                writer.Write(Unknown54);
                return writer.ToArray();
            }
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            if (MovePoint == 0)
                result.Add("DYNA Critter with MVPT Asset ID set to 0");

            Verify(MovePoint, ref result);
            Verify(Unknown54, ref result);
        }

        public static bool dontRender = false;
        [Browsable(false)]
        public override bool DontRender => dontRender;
    }
}