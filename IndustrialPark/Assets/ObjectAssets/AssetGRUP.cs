using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public class AssetGRUP : BaseAsset
    {
        public AssetGRUP(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform) { }

        public override bool HasReference(uint assetID)
        {
            foreach (AssetID a in GroupItems)
                if (a == assetID)
                    return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            foreach (AssetID a in GroupItems)
            {
                if (a == 0)
                    result.Add("GRUP with group item set to 0");
                Verify(a, ref result);
            }
        }

        protected override int EventStartOffset => 0x0C + ItemCount * 4;

        public enum Delegation
        {
            AllItems = 0,
            RandomItem = 1,
            InOrder = 2
        }

        [Category("Group"), ReadOnly(true)]
        public short ItemCount
        {
            get => ReadShort(0x08);
            set => Write(0x08, value);
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
                for (int i = 0; i < ItemCount; i++)
                    group.Add(ReadUInt(0xC + 4 * i));

                return group.ToArray();
            }
            set
            {
                List<byte> newData = Data.Take(0xC).ToList();

                foreach (AssetID i in value)
                    newData.AddRange(BitConverter.GetBytes(Switch(i)));

                newData.AddRange(Data.Skip(EventStartOffset).ToList());
                
                Data = newData.ToArray();
                ItemCount = (short)value.Length;
            }
        }
    }
}