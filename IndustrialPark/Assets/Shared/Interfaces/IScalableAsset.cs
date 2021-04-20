namespace IndustrialPark
{
    public interface IScalableAsset
    {
        AssetSingle ScaleX { get; set; }
        AssetSingle ScaleY { get; set; }
        AssetSingle ScaleZ { get; set; }

        AssetSingle PositionX { get; }
        AssetSingle PositionY { get; }
        AssetSingle PositionZ { get; }
    }
}