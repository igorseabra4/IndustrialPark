namespace IndustrialPark
{
    public interface IRotatableAsset
    {
        AssetSingle Yaw { get; set; }
        AssetSingle Pitch { get; set; }
        AssetSingle Roll { get; set; }

        AssetSingle PositionX { get; }
        AssetSingle PositionY { get; }
        AssetSingle PositionZ { get; }
    }
}