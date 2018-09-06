using SharpDX;

namespace IndustrialPark
{
    public interface IClickableAsset
    {
        float PositionX { get; set; }
        float PositionY { get; set; }
        float PositionZ { get; set; }
                        
        float? IntersectsWith(Ray ray);
        
        Vector3 GetGizmoCenter();

        float GetGizmoRadius();
    }
}