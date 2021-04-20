using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public enum EnemyMindyType : uint
    {
        patrick_npc_bind = 0x7362A2AC,
        spongebob_npc_bind = 0xA92E3C8F,
        mindy_shell_bind = 0x5D13D2A4
    }

    public class DynaEnemyMindy : DynaEnemySB
    {
        private const string dynaCategoryName = "Enemy:SB:Mindy";

        protected override int constVersion => 3;

        [Category(dynaCategoryName)]
        public EnemyMindyType MindyType
        {
            get => (EnemyMindyType)(uint)Model_AssetID;
            set => Model_AssetID = (uint)value;
        }
        [Category(dynaCategoryName)]
        public AssetID TaskBox1_AssetID { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat54 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat58 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat5C { get; set; }
        [Category(dynaCategoryName)]
        public int UnknownInt60 { get; set; }
        [Category(dynaCategoryName)]
        public AssetID TaskBox2_AssetID { get; set; }

        public DynaEnemyMindy(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, DynaType.Enemy__SB__Mindy, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = entityDynaEndPosition;

            TaskBox1_AssetID = reader.ReadUInt32();
            UnknownFloat54 = reader.ReadSingle();
            UnknownFloat58 = reader.ReadSingle();
            UnknownFloat5C = reader.ReadSingle();
            UnknownInt60 = reader.ReadInt32();
            TaskBox2_AssetID = reader.ReadUInt32();
        }

        protected override byte[] SerializeDyna(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(SerializeEntityDyna(platform));

            writer.Write(TaskBox1_AssetID);
            writer.Write(UnknownFloat54);
            writer.Write(UnknownFloat58);
            writer.Write(UnknownFloat5C);
            writer.Write(UnknownInt60);
            writer.Write(TaskBox2_AssetID);

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID)
        {
            if (TaskBox1_AssetID == assetID)
                return true;
            if (TaskBox2_AssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Verify(TaskBox1_AssetID, ref result);
            Verify(TaskBox2_AssetID, ref result);
        }
    }
}