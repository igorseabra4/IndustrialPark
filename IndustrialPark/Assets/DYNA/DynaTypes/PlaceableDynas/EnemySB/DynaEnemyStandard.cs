using HipHopFile;
using SharpDX;
using System.ComponentModel;

namespace IndustrialPark
{
    public enum EnemyStandardType : uint
    {
        flinger_v1_bind = 0xC63CD5CF,
        flinger_v2_bind = 0xC1C12142,
        flinger_v3_bind = 0xBD456CB5,
        fogger_de_bind = 0xCB40D3CA,
        fogger_gg_bind = 0xE0613E3B,
        fogger_jk_bind = 0xEC8A3F92,
        fogger_pt_bind = 0x00608DB3,
        fogger_tr_bind = 0xDC226631,
        fogger_tt_bind = 0xD32AFD17,
        fogger_v1_bind = 0xD979E410,
        fogger_v2_bind = 0xD4FE2F83,
        fogger_v3_bind = 0xD0827AF6,
        mervyn_v3_bind = 0xD8DF6639,
        minimerv_v1_bind = 0xF0C2E34D,
        popper_v1_bind = 0xD5BCDF92,
        popper_v3_bind = 0xCCC57678,
        slammer_v1_bind = 0xBEF858E7,
        slammer_des_bind = 0xA3CADF24,
        slammer_v3_bind = 0xB600EFCD,
        spinner_v1_bind = 0x1A6B3CDF,
        spinner_v2_bind = 0x15EF8852,
        spinner_v3_bind = 0x1173D3C5
    }

    public class DynaEnemyStandard : DynaEnemy
    {
        private const string dynaCategoryName = "Enemy:SB:Standard";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => EnemyType.ToString();

        protected override short constVersion => 7;

        [Category(dynaCategoryName)]
        public EnemyStandardType EnemyType
        {
            get => (EnemyStandardType)(uint)Model;
            set => Model = (uint)value;
        }
        [Category(dynaCategoryName)]
        public AssetID MovePoint { get; set; }
        [Category(dynaCategoryName)]
        public AssetID MovePointGroup { get; set; }
        [Category(dynaCategoryName)]
        public FlagBitmask EnemyFlags { get; set; } = IntFlagsDescriptor(
            "Prepare for Scare",
            null,
            "Walk on PLATs",
            "Walk on SIMPs");
        [Category(dynaCategoryName)]
        public AssetID Unknown5C { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Unknown60 { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Unknown64 { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Unknown68 { get; set; }

        public DynaEnemyStandard(string assetName, AssetTemplate template, Vector3 position, uint mvptAssetID) : base(assetName, DynaType.Enemy__SB__Standard, position)
        {
            switch (template)
            {
                case AssetTemplate.Fogger_GoofyGoober:
                    EnemyType = EnemyStandardType.fogger_gg_bind;
                    break;
                case AssetTemplate.Fogger_Desert:
                    EnemyType = EnemyStandardType.fogger_de_bind;
                    break;
                case AssetTemplate.Fogger_ThugTug:
                    EnemyType = EnemyStandardType.fogger_tt_bind;
                    break;
                case AssetTemplate.Fogger_Trench:
                    EnemyType = EnemyStandardType.fogger_tr_bind;
                    break;
                case AssetTemplate.Fogger_Junkyard:
                    EnemyType = EnemyStandardType.fogger_jk_bind;
                    break;
                case AssetTemplate.Fogger_Planktopolis:
                    EnemyType = EnemyStandardType.fogger_pt_bind;
                    break;
                case AssetTemplate.Fogger_v1:
                    EnemyType = EnemyStandardType.fogger_v1_bind;
                    break;
                case AssetTemplate.Fogger_v2:
                    EnemyType = EnemyStandardType.fogger_v2_bind;
                    break;
                case AssetTemplate.Fogger_v3:
                    EnemyType = EnemyStandardType.fogger_v3_bind;
                    break;
                case AssetTemplate.Slammer_GoofyGoober:
                    EnemyType = EnemyStandardType.slammer_v1_bind;
                    break;
                case AssetTemplate.Slammer_Desert:
                    EnemyType = EnemyStandardType.slammer_des_bind;
                    break;
                case AssetTemplate.Slammer_ThugTug:
                    EnemyType = EnemyStandardType.slammer_v3_bind;
                    break;
                case AssetTemplate.Spinner_ThugTug:
                    EnemyType = EnemyStandardType.spinner_v1_bind;
                    break;
                case AssetTemplate.Spinner_Junkyard:
                    EnemyType = EnemyStandardType.spinner_v2_bind;
                    break;
                case AssetTemplate.Spinner_Planktopolis:
                    EnemyType = EnemyStandardType.spinner_v3_bind;
                    break;
                case AssetTemplate.Minimerv:
                    EnemyType = EnemyStandardType.minimerv_v1_bind;
                    break;
                case AssetTemplate.Mervyn:
                    EnemyType = EnemyStandardType.mervyn_v3_bind;
                    break;
                case AssetTemplate.Flinger_Desert:
                    EnemyType = EnemyStandardType.flinger_v1_bind;
                    break;
                case AssetTemplate.Flinger_Trench:
                    EnemyType = EnemyStandardType.flinger_v2_bind;
                    break;
                case AssetTemplate.Flinger_Junkyard:
                    EnemyType = EnemyStandardType.flinger_v3_bind;
                    break;
                case AssetTemplate.Popper_Trench:
                    EnemyType = EnemyStandardType.popper_v1_bind;
                    break;
                case AssetTemplate.Popper_Planktopolis:
                    EnemyType = EnemyStandardType.popper_v3_bind;
                    break;
            }
            MovePoint = mvptAssetID;
            // Walk on SIMPs and PLATs by default.
            EnemyFlags.FlagValueByte = 8 | 4; 
        }

        public DynaEnemyStandard(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.Enemy__SB__Standard, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = entityDynaEndPosition;

                MovePoint = reader.ReadUInt32();
                MovePointGroup = reader.ReadUInt32();
                EnemyFlags.FlagValueInt = reader.ReadUInt32();
                Unknown5C = reader.ReadUInt32();
                Unknown60 = reader.ReadUInt32();
                Unknown64 = reader.ReadUInt32();
                Unknown68 = reader.ReadUInt32();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            SerializeEntityDyna(writer);
            writer.Write(MovePoint);
            writer.Write(MovePointGroup);
            writer.Write(EnemyFlags.FlagValueInt);
            writer.Write(Unknown5C);
            writer.Write(Unknown60);
            writer.Write(Unknown64);
            writer.Write(Unknown68);
        }

        public static bool dontRender = false;
        public override bool DontRender => dontRender;
    }
}