using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;
using static HipHopFile.Functions;

namespace IndustrialPark
{
    public class AssetPORT : ObjectAsset
    {
        public AssetPORT(Section_AHDR AHDR) : base(AHDR) { }

        public override bool HasReference(uint assetID) => Camera_AssetID == assetID || Destination_MRKR_AssetID == assetID || base.HasReference(assetID);
        
        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            if (Camera_AssetID == 0)
                result.Add("PORT with Camera_AssetID set to 0");
            if (Destination_MRKR_AssetID == 0)
                result.Add("PORT with Destination_MRKR_AssetID set to 0");
        }

        protected override int EventStartOffset => 0x18;

        [Category("Portal")]
        public AssetID Camera_AssetID
        {
            get => ReadUInt(0x8);
            set => Write(0x8, value);
        }

        [Category("Portal")]
        public AssetID Destination_MRKR_AssetID
        {
            get => ReadUInt(0xC);
            set => Write(0xC, value);
        }

        [Category("Portal"), TypeConverter(typeof(FloatTypeConverter))]
        public float Rotation
        {
            get => ReadFloat(0x10);
            set => Write(0x10, value);
        }

        [Category("Portal")]
        public string DestinationLevel
        {
            get
            {
                List<byte> bytes = new List<byte>();
                for (int i = 0; i < 4; i++)
                    bytes.Add(Data[0x14 + i]);

                if (currentGame != Game.Incredibles && currentPlatform == Platform.GameCube)
                    bytes.Reverse();

                return System.Text.Encoding.ASCII.GetString(bytes.ToArray(), 0, 4);
            }
            set
            {
                List<byte> bytes = new List<byte>();
                foreach (char c in value)
                    bytes.Add((byte)c);

                while (bytes.Count < 4)
                    bytes.Add((byte)' ');

                if (currentGame != Game.Incredibles && currentPlatform == Platform.GameCube)
                    bytes.Reverse();

                for (int i = 0; i < 4; i++)
                    Data[0x14 + i] = bytes[i];
            }
        }
    }
}