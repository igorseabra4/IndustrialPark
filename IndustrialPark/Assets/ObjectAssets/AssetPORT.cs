﻿using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;
using static HipHopFile.Functions;

namespace IndustrialPark
{
    public class AssetPORT : ObjectAsset
    {
        public AssetPORT(Section_AHDR AHDR) : base(AHDR) { }

        public override bool HasReference(uint assetID)
        {
            if (Camera_Unknown == assetID)
                return true;
            if (Destination_MRKR == assetID)
                return true;

            return base.HasReference(assetID);
        }

        protected override int EventStartOffset => 0x18;

        [Category("Portal")]
        public AssetID Camera_Unknown
        {
            get => ReadUInt(0x8);
            set => Write(0x8, value);
        }

        [Category("Portal")]
        public AssetID Destination_MRKR
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

                if (currentPlatform == Platform.GameCube)
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

                if (currentPlatform == Platform.GameCube)
                    bytes.Reverse();

                for (int i = 0; i < 4; i++)
                    Data[0x14 + i] = bytes[i];
            }
        }
    }
}