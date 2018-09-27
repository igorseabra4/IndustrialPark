using HipHopFile;
using System.Collections.Generic;
using System;
using System.Linq;
using static HipHopFile.Functions;

namespace IndustrialPark
{
    public class AssetALST : Asset
    {
        public AssetALST(Section_AHDR AHDR) : base(AHDR) { }

        public AssetID[] ANIM_AssetIDs
        {
            get
            {
                List<AssetID> assetIDs = new List<AssetID>();
                for (int i = 0; i < AHDR.data.Length; i += 4)
                    assetIDs.Add(ReadUInt(i));

                return assetIDs.ToArray();
            }
            set
            {
                List<byte> newData = new List<byte>();

                foreach (AssetID i in value)
                {
                    if (currentPlatform == Platform.GameCube)
                        newData.AddRange(BitConverter.GetBytes(i).Reverse());
                    else
                        newData.AddRange(BitConverter.GetBytes(i));
                }

                Data = newData.ToArray();
            }
        }
    }
}