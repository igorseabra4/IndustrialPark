using SharpDX;

namespace IndustrialPark
{
    public interface IVolumeAsset
    {
        AssetSingle MinimumX { get; set; }
        AssetSingle MinimumY { get; set; }
        AssetSingle MinimumZ { get; set; }
        AssetSingle MaximumX { get; set; }
        AssetSingle MaximumY { get; set; }
        AssetSingle MaximumZ { get; set; }

        void ApplyScale(Vector3 scale, float singleFactor);
    }
}