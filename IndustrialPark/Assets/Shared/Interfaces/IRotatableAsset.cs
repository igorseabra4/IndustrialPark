namespace IndustrialPark
{
    public interface IRotatableAsset
    {
        float Yaw { get; set; }
        float Pitch { get; set; }
        float Roll { get; set; }

        float PositionX { get; }
        float PositionY { get; }
        float PositionZ { get; }
    }
}