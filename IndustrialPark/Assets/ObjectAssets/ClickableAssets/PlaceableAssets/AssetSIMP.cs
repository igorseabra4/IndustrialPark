using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetSIMP : EntityAsset
    {
        private const string categoryName = "Simple Object";

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float AnimSpeed { get; set; }
        [Category(categoryName)]
        public int InitialAnimState { get; set; }
        [Category(categoryName), Description("Only Static is functional.")]
        public FlagBitmask CollType { get; set; } = ByteFlagsDescriptor(
            "Trigger",
            "Static",
            "Dynamic",
            "NPC",
            "Player");
        [Category(categoryName), Description("Always 0.")]
        public FlagBitmask SimpFlags { get; set; } = ByteFlagsDescriptor();

        public AssetSIMP(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = entityEndPosition;

            AnimSpeed = reader.ReadSingle();
            InitialAnimState = reader.ReadInt32();
            CollType.FlagValueByte = reader.ReadByte();
            SimpFlags.FlagValueByte = reader.ReadByte();
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(SerializeEntity(game, platform));
            
            writer.Write(AnimSpeed);
            writer.Write(InitialAnimState);
            writer.Write(CollType.FlagValueByte);
            writer.Write(SimpFlags.FlagValueByte);
            writer.Write((short)0);

            writer.Write(SerializeLinks(platform));
            return writer.ToArray();
        }

        public static bool dontRender = false;

        public override bool DontRender => dontRender;
    }
}