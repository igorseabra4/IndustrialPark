using HipHopFile;
using System;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
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
        private char[] _destinationLevel;
        [Category(categoryName)]
        public string DestinationLevel
        {
            get => new string(_destinationLevel);
            set
            {
                if (value.Length != 4)
                    throw new ArgumentException("Value must be 4 characters long");
                _destinationLevel = value.ToCharArray();
            }
        }

        public AssetPORT(string assetName) : base(assetName, AssetType.Portal, BaseAssetType.Portal)
        {
            Camera = "STARTCAM";
            DestinationLevel = "AA00";
        }

        public AssetPORT(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                Camera = reader.ReadUInt32();
                DestinationMarker = reader.ReadUInt32();
                Rotation = reader.ReadSingle();

                var chars = reader.ReadChars(4);
                _destinationLevel = reader.endianness == Endianness.Little ? chars : chars.Reverse().ToArray();
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Camera);
            writer.Write(DestinationMarker);
            writer.Write(Rotation);
            writer.WriteMagic(new string(_destinationLevel.ToArray()));
            SerializeLinks(writer);
        }
    }
}