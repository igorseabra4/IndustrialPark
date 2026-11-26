using HipHopFile;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace IndustrialPark.Assets
{
    public class AssetPORT : BaseAsset
    {
        private const string categoryName = "Portal";
        public override string AssetInfo => DestinationLevel;

        [Category(categoryName), ValidReferenceRequired]
        public AssetID Camera { get; set; }
        [Category(categoryName), ValidReferenceRequired]
        public AssetID DestinationMarker { get; set; }
        [Category(categoryName)]
        public AssetSingle Rotation { get; set; }
        [Category(categoryName)]
        public string DestinationLevel
        {
            get;
            set
            {
                if (value == null || value.Length != 4 || value.Any(c => c > 127))
                    throw new ArgumentException("Value must contain exactly 4 ASCII characters.");
                field = value;
            }
        }

        public AssetPORT(string assetName) : base(assetName, AssetType.Portal, BaseAssetType.Portal)
        {
            Camera = "STARTCAM";
            DestinationLevel = "AA00";
        }

        public AssetPORT(Section_AHDR AHDR, EndianBinaryReader reader) : base(AHDR, reader)
        {
            Camera = reader.ReadUInt32();
            DestinationMarker = reader.ReadUInt32();
            Rotation = reader.ReadSingle();

            var chars = reader.ReadBytes(4);
            if (DestinationLevelIsBackwards(game, reader.endianness))
                Array.Reverse(chars);
            DestinationLevel = Encoding.ASCII.GetString(chars);
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Camera);
            writer.Write(DestinationMarker);
            writer.Write(Rotation);

            var chars = Encoding.ASCII.GetBytes(DestinationLevel);
            if (DestinationLevelIsBackwards(game, writer.endianness))
                Array.Reverse(chars);
            writer.Write(chars);

            SerializeLinks(writer);
        }

        // Backwards for Scooby and BFBB on GC, and for the other games on PS2 and Xbox, for some reason
        public static bool DestinationLevelIsBackwards(Game game, Endianness endianness) =>
            (game == Game.Scooby || game == Game.BFBB) == (endianness == Endianness.Big);
    }
}