using HipHopFile;
using SharpDX;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetHANG : EntityAsset
    {
        protected const string categoryName = "Hangable";

        [Category(categoryName)]
        public FlagBitmask HangFlags { get; set; } = IntFlagsDescriptor();
        [Category(categoryName)]
        public AssetSingle PivotOffset { get; set; }
        [Category(categoryName)]
        public AssetSingle LeverArm { get; set; }
        [Category(categoryName)]
        public AssetSingle Gravity { get; set; }
        [Category(categoryName)]
        public AssetSingle Acceleration { get; set; }
        [Category(categoryName)]
        public AssetSingle Decay { get; set; }
        [Category(categoryName)]
        public AssetSingle GrabDelay { get; set; }
        [Category(categoryName)]
        public AssetSingle StopDeceleration { get; set; }

        public AssetHANG(string assetName, Vector3 position) : base(assetName, AssetType.Hangable, BaseAssetType.Hangable, position)
        {
        }

        public AssetHANG(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = entityHeaderEndPosition;

                HangFlags.FlagValueInt = reader.ReadUInt32();
                PivotOffset = reader.ReadSingle();
                LeverArm = reader.ReadSingle();
                Gravity = reader.ReadSingle();
                Acceleration = reader.ReadSingle();
                Decay = reader.ReadSingle();
                GrabDelay = reader.ReadSingle();
                StopDeceleration = reader.ReadSingle();
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeEntity(game, endianness));
                writer.Write(HangFlags.FlagValueInt);
                writer.Write(PivotOffset);
                writer.Write(LeverArm);
                writer.Write(Gravity);
                writer.Write(Acceleration);
                writer.Write(Decay);
                writer.Write(GrabDelay);
                writer.Write(StopDeceleration);
                writer.Write(SerializeLinks(endianness));
                return writer.ToArray();
            }
        }

        public static bool dontRender = false;

        public override bool DontRender => dontRender;
    }
}