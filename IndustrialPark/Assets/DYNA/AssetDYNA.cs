using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public abstract class AssetDYNA : BaseAsset
    {
        protected const string categoryName = "Dynamic";
        public override string TypeString => Type.ToString();

        [Category(categoryName)]
        public DynaType Type { get; private set; }
        [Category(categoryName)]
        public short Version { get; set; }
        [Category(categoryName)]
        public short Handle { get; set; }
        [Category(categoryName)]
        public virtual string Note => $"Version is {(constVersion == -1 ? "unknown" : $"always {constVersion}")}";
        protected virtual short constVersion => -1;

        protected int dynaDataStartPosition => baseHeaderEndPosition + 8;

        public AssetDYNA(string assetName, DynaType type, short version) : base(assetName, type.ToAssetType(), BaseAssetType.Unknown_Other)
        {
            Type = type;
            Version = version;
        }

        public AssetDYNA(Section_AHDR AHDR, DynaType type, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition + 4;

                Type = type;
                Version = reader.ReadInt16();
                Handle = reader.ReadInt16();
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeBase(endianness));
                writer.Write((uint)Type);
                writer.Write(Version);
                writer.Write(Handle);
                writer.Write(SerializeDyna(game, endianness));
                writer.Write(SerializeLinks(endianness));

                return writer.ToArray();
            }
        }

        protected abstract byte[] SerializeDyna(Game game, Endianness endianness);
    }
}