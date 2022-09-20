using HipHopFile;
using SharpDX;
using System.Collections.Generic;
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
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => CrateType.ToString();

        protected override short constVersion => 2;

        [Category(dynaCategoryName)]
        public EnemySupplyCrateType CrateType
        {
            get => (EnemySupplyCrateType)(uint)Model;
            set => Model = (uint)value;
        }
        [Category(dynaCategoryName)]
        public AssetID MovePoint { get; set; }

        public DynaEnemySupplyCrate(string assetName, AssetTemplate template, Vector3 position) : base(assetName, DynaType.Enemy__SB__SupplyCrate, position)
        {
            CrateType =
                template == AssetTemplate.Wood_Crate ? EnemySupplyCrateType.crate_wood_bind :
                template == AssetTemplate.Hover_Crate ? EnemySupplyCrateType.crate_hover_bind :
                template == AssetTemplate.Explode_Crate ? EnemySupplyCrateType.crate_explode_bind :
                template == AssetTemplate.Shrink_Crate ? EnemySupplyCrateType.crate_shrink_bind :
                template == AssetTemplate.Steel_Crate ? EnemySupplyCrateType.crate_steel_bind : 0;
        }

        public DynaEnemySupplyCrate(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.Enemy__SB__SupplyCrate, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = entityDynaEndPosition;

                MovePoint = reader.ReadUInt32();
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeEntityDyna(endianness));
                writer.Write(MovePoint);

                return writer.ToArray();
            }
        }

        public static bool dontRender = false;
        public override bool DontRender => dontRender;
    }
}