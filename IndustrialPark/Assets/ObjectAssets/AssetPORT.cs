using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public class AssetPORT : BaseAsset
    {
        private const string categoryName = "Portal";

        [Category(categoryName)]
        public AssetID Camera_AssetID { get; set; }
        [Category(categoryName)]
        public AssetID Destination_MRKR_AssetID { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float Rotation { get; set; }
        [Category(categoryName)]
        private char[] _destinationLevel;
        [Category(categoryName)]
        public char[] DestinationLevel
        {
            get => _destinationLevel;
            set
            {
                if (value.Length != 4)
                    throw new ArgumentException("Value must be 4 characters long");
                _destinationLevel = value;
            }
        }

        public AssetPORT(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = baseEndPosition;

            Camera_AssetID = reader.ReadUInt32();
            Destination_MRKR_AssetID = reader.ReadUInt32();
            Rotation = reader.ReadSingle();

            var chars = reader.ReadChars(4);
            _destinationLevel = reader.endianness == Endianness.Big ? chars : chars.Reverse().ToArray();
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(SerializeBase(platform));

            writer.Write(Camera_AssetID);
            writer.Write(Destination_MRKR_AssetID);
            writer.Write(Rotation);
            writer.Write(writer.endianness == Endianness.Big ? DestinationLevel : DestinationLevel.Reverse().ToArray());

            writer.Write(SerializeLinks(platform));
            return writer.ToArray();
        }

        public override bool HasReference(uint assetID) => Camera_AssetID == assetID || Destination_MRKR_AssetID == assetID || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            if (Camera_AssetID == 0)
                result.Add("PORT with Camera_AssetID set to 0");
            if (Destination_MRKR_AssetID == 0)
                result.Add("PORT with Destination_MRKR_AssetID set to 0");
        }
    }
}