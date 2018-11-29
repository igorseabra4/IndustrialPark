using SharpDX;

namespace IndustrialPark
{
    public interface IRotatableAsset
    {
        float Yaw { get; set; }
        float Pitch { get; set; }
        float Roll { get; set; }

        BoundingSphere GetObjectCenter();
    }
}