using SharpDX;
using System.Collections.Generic;

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
        ScaleAll,

        TrigX1,
        TrigY1,
        TrigZ1
    }

    public enum GizmoMode
    {
        None,
        Cycle,
        Position,
        Rotation,
        Scale,
        PositionLocal
    }

    public abstract class GizmoBase
    {
        public static bool UseCones = true;
        public static SharpDX.Direct3D11.Buffer VertexBuffer;

        public GizmoType Type { get; protected set; }
        public bool isSelected;
        protected Matrix transformMatrix = Matrix.Identity;
        protected Matrix lineTransformMatrix = Matrix.Identity;

        public GizmoBase(GizmoType type)
        {
            vertices = new Vector3[vertexList.Count];

            this.Type = type;

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
                case GizmoType.TrigX1:
                    renderData.Color = new Vector4(0f, 1f, 1f, 0.4f);
                    break;
                case GizmoType.TrigY1:
                    renderData.Color = new Vector4(1f, 0f, 1f, 0.4f);
                    break;
                case GizmoType.TrigZ1:
                    renderData.Color = new Vector4(1f, 1f, 0f, 0.4f);
                    break;
            }
            isSelected = false;
        }

        protected Vector3 position;
        protected Vector3[] vertices;
        public BoundingBox boundingBox;

        protected void ResetBounds()
        {
            for (int i = 0; i < vertexList.Count; i++)
                vertices[i] = Vector3.TransformCoordinate(vertexList[i], transformMatrix);
            boundingBox = BoundingBox.FromPoints(vertices);
            position = Vector3.TransformCoordinate(Vector3.Zero, transformMatrix);
        }

        public virtual SharpMesh Mesh => UseCones ? SharpRenderer.Cone : SharpRenderer.Pyramid;
        protected virtual List<Vector3> vertexList => UseCones ? SharpRenderer.coneVertices : SharpRenderer.pyramidVertices;
        protected virtual List<Models.Triangle> triangleList => UseCones ? SharpRenderer.coneTriangles : SharpRenderer.pyramidTriangles;

        protected virtual bool drawLine => true;

        protected RenderData.DefaultRenderData renderData;
        
        public void Draw(SharpRenderer renderer)
        {
            renderData.worldViewProjection = transformMatrix * renderer.viewProjection;

            renderer.device.SetFillModeSolid();
            renderer.device.SetCullModeNone();
            renderer.device.SetBlendStateAlphaBlend();
            renderer.device.ApplyRasterState();
            renderer.device.SetDepthStateNone();
            renderer.device.UpdateAllStates();

            renderer.basicBuffer.UpdateValue(renderData);
            renderer.device.DeviceContext.VertexShader.SetConstantBuffer(0, renderer.basicBuffer.Buffer);
            renderer.basicShader.Apply();

            Mesh.Draw(renderer.device);

            renderer.device.SetDefaultDepthState();

            if (drawLine)
                renderer.DrawSpline(VertexBuffer, 2, lineTransformMatrix, renderData.Color, true);
        }

        public float? IntersectsWith(Ray ray)
        {
            float? smallestDistance = null;

            if (ray.Intersects(ref boundingBox))
                foreach (Models.Triangle t in triangleList)
                    if (ray.Intersects(ref vertices[t.vertex1], ref vertices[t.vertex2], ref vertices[t.vertex3], out float distance))
                        if (smallestDistance == null || distance < smallestDistance)
                            smallestDistance = distance;

            return smallestDistance;
        }

        public float GetDistance(Vector3 position)
        {
            return Vector3.Distance(this.position, position);
        }
    }
}