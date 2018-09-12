using HipHopFile;

namespace IndustrialPark
{
    public class AssetDYNA : ObjectAsset
    {
        public AssetDYNA(Section_AHDR AHDR) : base(AHDR) { }

        protected override int EventStartOffset
        {
            get => Data.Length - AmountOfEvents * AssetEvent.sizeOfStruct;
        }
    }
}