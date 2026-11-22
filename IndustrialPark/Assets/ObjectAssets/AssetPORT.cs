using HipHopFile;
using System;
using System.ComponentModel;
using System.Linq;

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
                if (value.Length != 4)
                    throw new ArgumentException("Value must be 4 characters long");
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

            var chars = reader.ReadChars(4);
            DestinationLevel = (reader.endianness == Endianness.Little && game < Game.Incredibles) ? new string(chars) : new string(chars.Reverse().ToArray());
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Camera);
            writer.Write(DestinationMarker);
            writer.Write(Rotation);

            if (writer.endianness == Endianness.Little && game < Game.Incredibles)
                writer.WritePaddedString(DestinationLevel.Reverse().ToString(), 4);
            else
                writer.WritePaddedString(DestinationLevel, 4);

            writer.WritePaddedString(DestinationLevel, 4);
            SerializeLinks(writer);
        }
    }
}