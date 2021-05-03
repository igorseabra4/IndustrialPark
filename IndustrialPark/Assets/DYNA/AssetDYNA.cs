using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public abstract class AssetDYNA : BaseAsset
    {
        protected const string categoryName = "Dynamic";

        [Category(categoryName)]
        public DynaType Type { get; private set; }
        [Category(categoryName)]
        public short Version { get; set; }
        [Category(categoryName)]
        public short Handle { get; set; }
        [Category(categoryName)]
        public virtual string Note => $"Version is {(constVersion == -1 ? "unknown" : $"always {constVersion}")}";
        protected virtual int constVersion => -1;

        public override string AssetInfo => Type.ToString();

        protected int dynaDataStartPosition => baseHeaderEndPosition + 8;

        public static bool dontRender = false;

        public AssetDYNA(Section_AHDR AHDR, DynaType type, Game game, Platform platform) : base(AHDR, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = baseHeaderEndPosition + 4;

            Type = type;
            Version = reader.ReadInt16();
            Handle = reader.ReadInt16();
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            writer.Write(SerializeBase(platform));
            writer.Write((uint)Type);
            writer.Write(Version);
            writer.Write(Handle);
            writer.Write(SerializeDyna(game, platform));
            writer.Write(SerializeLinks(platform));

            return writer.ToArray();
        }

        protected abstract byte[] SerializeDyna(Game game, Platform platform);
    }
}