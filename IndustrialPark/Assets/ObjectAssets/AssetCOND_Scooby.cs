using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetCOND_Scooby : ObjectAsset
    {
        public AssetCOND_Scooby(Section_AHDR AHDR) : base(AHDR) { }

        public override bool HasReference(uint assetID) => Conditional == assetID || base.HasReference(assetID);
        
        protected override int EventStartOffset => 0x14;

        [Category("Conditional")]
        public AssetID Conditional
        {
            get => ReadUInt(0xC);
            set => Write(0xC, value);
        }

        [Category("Conditional")]
        public CONDOperation Operation
        {
            get => (CONDOperation)ReadInt(0x10);
            set => Write(0x10, (int)value);
        }

        [Category("Conditional")]
        public int EvaluationAmount
        {
            get => ReadInt(0x8);
            set => Write(0x8, value);
        }
    }
}