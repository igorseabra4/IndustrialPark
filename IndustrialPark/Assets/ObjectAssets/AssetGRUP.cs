using HipHopFile;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IndustrialPark
{
    public class AssetGRUP : ObjectAsset
    {
        public AssetGRUP(Section_AHDR AHDR) : base(AHDR) { }
        
        public AssetID[] GroupAssetIDs
        {
            get
            {
                List<AssetID> group = new List<AssetID>();
                short amount = ReadShort(0x08);
                for (int i = 0; i < amount; i++)
                    group.Add(ReadUInt(0xC + 4 * i));

                return group.ToArray();
            }
            set
            {
                List<AssetID> newValues = value.ToList();
                short oldAmountOfPairs = ReadShort(0x08);

                List<byte> newData = Data.Take(0xC).ToList();
                List<byte> restOfOldData = Data.Skip(0xC + 4 * oldAmountOfPairs).ToList();

                foreach (AssetID i in newValues)
                    newData.AddRange(BitConverter.GetBytes(i).Reverse());

                newData.AddRange(restOfOldData);
                newData[0x08] = BitConverter.GetBytes((short)newValues.Count)[1];
                newData[0x09] = BitConverter.GetBytes((short)newValues.Count)[0];

                Data = newData.ToArray();
            }
        }

        public AssetEvent[] Events
        {
            get { return ReadEvents(0x0C + ReadUshort(0x08) * 4); }
            set { WriteEvents(0x0C + ReadUshort(0x08) * 4, value); }
        }
    }
}