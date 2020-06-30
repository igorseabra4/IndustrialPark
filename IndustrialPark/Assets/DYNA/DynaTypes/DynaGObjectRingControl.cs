using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public class DynaGObjectRingControl : DynaBase
    {
        public static uint RingModelAssetID = 0;

        public string Note => "Version is always 3";

        public override int StructSize => 0x28 + 4 * RingCount;

        public DynaGObjectRingControl(AssetDYNA asset) : base(asset)
        {
            RingModel_AssetID = ReadUInt(0x04);
        }
        
        public override bool HasReference(uint assetID)
        {
            if (RingModel_AssetID == assetID)
                return true;
            if (RingSoundGroup_AssetID == assetID)
                return true;
            foreach (AssetID ring in Rings_AssetIDs)
                if (ring == assetID)
                    return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            if (RingModel_AssetID == 0)
                result.Add("Ring Control with no Ring Model reference");
            Asset.Verify(RingModel_AssetID, ref result);

            if (RingSoundGroup_AssetID == 0)
                result.Add("Ring Control with no SGRP reference");
            Asset.Verify(RingSoundGroup_AssetID, ref result);

            foreach (AssetID ring in Rings_AssetIDs)
                Asset.Verify(ring, ref result);
        }
        
        public enum DynaRingControlPlayerType
        {
            Drive = 0,
            SpongebobPatrick = 1,
            Spongeball = 2,
            Unknown3 = 3,
            Slide = 4,
            SonicWaveGuitar = 5
        }

        public DynaRingControlPlayerType PlayerType
        {
            get => (DynaRingControlPlayerType)ReadInt(0x00);
            set => Write(0x00, (int)value);
        }
        private uint _ringModelAssetID;
        
        public AssetID RingModel_AssetID
        {
            get => _ringModelAssetID;
            set
            {
                _ringModelAssetID = value;
                RingModelAssetID = value;
                Write(0x04, value);
            }
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat1
        {
            get => ReadFloat(0x08);
            set => Write(0x08, value);
        }
        private int RingCount
        {
            get => ReadInt(0x0C);
            set => Write(0x0C, value);
        }
        public int UnknownInt1
        {
            get => ReadInt(0x10);
            set => Write(0x10, value);
        }
        public AssetID RingSoundGroup_AssetID
        {
            get => ReadUInt(0x14);
            set => Write(0x14, value);
        }
        public int UnknownInt2
        {
            get => ReadInt(0x18);
            set => Write(0x18, value);
        }
        public int UnknownInt3
        {
            get => ReadInt(0x1C);
            set => Write(0x1C, value);
        }
        public int UnknownInt4
        {
            get => ReadInt(0x20);
            set => Write(0x20, value);
        }
        public int RingsAreVisible
        {
            get => ReadInt(0x24);
            set => Write(0x24, value);
        }
        public AssetID[] Rings_AssetIDs
        {
            get
            {
                try
                {
                    AssetID[] rings = new AssetID[RingCount];
                    for (int i = 0; i < RingCount; i++)
                        rings[i] = ReadUInt(0x28 + 4 * i);

                    return rings;
                }
                catch
                {
                    return new AssetID[0];
                }
            }
            set
            {
                List<byte> newData = asset.Data.Take(0x38).ToList();
                List<byte> restOfOldData = asset.Data.Skip(0x38 + 4 * RingCount).ToList();

                foreach (AssetID i in value)
                    if (platform == Platform.GameCube)
                        newData.AddRange(BitConverter.GetBytes(i).Reverse());
                    else
                        newData.AddRange(BitConverter.GetBytes(i));
                
                newData.AddRange(restOfOldData);

                asset.Data = newData.ToArray();

                RingCount = value.Length;
            }
        }
    }
}