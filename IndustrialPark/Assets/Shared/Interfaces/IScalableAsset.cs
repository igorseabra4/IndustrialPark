namespace IndustrialPark
{
    public interface IScalableAsset
    {
        float ScaleX { get; set; }
        float ScaleY { get; set; }
        float ScaleZ { get; set; }

        float PositionX { get; }
        float PositionY { get; }
        float PositionZ { get; }
    }
}