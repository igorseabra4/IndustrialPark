using SharpDX;

namespace IndustrialPark
{
    public enum GizmoType
    {
        X,
        Y,
        Z,
        Yaw,
        Pitch,
        Roll,
        ScaleX,
        ScaleY,
        ScaleZ,
        ScaleAll
    }

    public enum GizmoMode
    {
        Null,
        Position,
        Rotation,
        Scale,
        PositionLocal
    }

    public abstract class GizmoBase
    {
        protected GizmoType type;
        public bool isSelected;
        protected Matrix transformMatrix = Matrix.Identity;

        public GizmoBase(GizmoType type)
        {
            this.type = type;

            switch (type)
            {
                case GizmoType.X:
                case GizmoType.Yaw:
                case GizmoType.ScaleX:
                    renderData.Color = new Vector4(1f, 0f, 0f, 0.4f);
                    break;
                case GizmoType.Y:
                case GizmoType.Pitch:
                case GizmoType.ScaleY:
                    renderData.Color = new Vector4(0f, 1f, 0f, 0.4f);
                    break;
                case GizmoType.Z:
                case GizmoType.Roll:
                case GizmoType.ScaleZ:
                    renderData.Color = new Vector4(0f, 0f, 1f, 0.4f);
                    break;
                case GizmoType.ScaleAll:
                    renderData.Color = new Vector4(0.6f, 0.5f, 0.5f, 0.4f);
                    break;
            }
            isSelected = false;
        }
        
        protected DefaultRenderData renderData;

        public void Draw(SharpRenderer renderer)
        {
            renderData.worldViewProjection = transformMatrix * renderer.viewProjection;

            renderer.device.SetFillModeSolid();
            renderer.device.SetCullModeNone();
            renderer.device.SetBlendStateAlphaBlend();
            renderer.device.ApplyRasterState();
            renderer.device.SetDepthStateNone();
            renderer.device.UpdateAllStates();

            renderer.device.UpdateData(renderer.basicBuffer, renderData);
            renderer.device.DeviceContext.VertexShader.SetConstantBuffer(0, renderer.basicBuffer);
            renderer.basicShader.Apply();

            Mesh.Draw(renderer.device);

            renderer.device.SetDefaultDepthState();
        }

        public abstract SharpMesh Mesh { get; }

        public BoundingBox boundingBox;

        public float? IntersectsWith(Ray r)
        {
            if (r.Intersects(ref boundingBox, out float distance))
                if (TriangleIntersection(r))
                    return distance;

            return null;
        }

        public abstract bool TriangleIntersection(Ray r);
    }
}