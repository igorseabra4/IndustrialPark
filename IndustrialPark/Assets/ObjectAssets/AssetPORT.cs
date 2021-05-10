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

        public AssetPORT(string assetName) : base(assetName, AssetType.PORT, BaseAssetType.Portal)
        {
            Camera_AssetID = "STARTCAM";
            DestinationLevel = "AA00";
        }

        public AssetPORT(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            var reader = new EndianBinaryReader(AHDR.data, endianness);
            reader.BaseStream.Position = baseHeaderEndPosition;

            Camera_AssetID = reader.ReadUInt32();
            Destination_MRKR_AssetID = reader.ReadUInt32();
            Rotation = reader.ReadSingle();

            var chars = reader.ReadChars(4);
            _destinationLevel = reader.endianness == Endianness.Little ? chars : chars.Reverse().ToArray();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);
            writer.Write(SerializeBase(endianness));

            writer.Write(Camera_AssetID);
            writer.Write(Destination_MRKR_AssetID);
            writer.Write(Rotation);
            writer.WriteMagic(new string(_destinationLevel.ToArray()));

            writer.Write(SerializeLinks(endianness));
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