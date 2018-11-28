using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public class AssetGRUP : ObjectAsset
    {
        public AssetGRUP(Section_AHDR AHDR) : base(AHDR) { }

        public override bool HasReference(uint assetID)
        {
            foreach (AssetID a in GroupItems)
                if (a == assetID)
                    return true;

            return base.HasReference(assetID);
        }

        protected override int EventStartOffset => 0x0C + ReadShort(0x08) * 4;

        public enum Delegation
        {
            AllItems = 0,
            RandomItem = 1,
            InOrder = 2
        }

        [Category("Group")]
        public Delegation ReceiveEventDelegation
        {
            get => (Delegation)ReadShort(0x0A);
            set => Write(0x0A, (short)value);
        }

        [Category("Group")]
        public AssetID[] GroupItems
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
    }
}