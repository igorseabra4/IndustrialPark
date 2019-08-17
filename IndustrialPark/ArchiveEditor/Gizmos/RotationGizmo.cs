using SharpDX;
using System.Collections.Generic;

namespace IndustrialPark
{
    public class RotationGizmo : GizmoBase
    {
        public RotationGizmo(GizmoType type) : base(type)
        {
            vertices = new Vector3[SharpRenderer.torusVertices.Count];
        }

        public void SetPosition(BoundingSphere Sphere, Matrix Rotation)
        {
            if (Sphere.Radius < 1f)
                Sphere.Radius = 1f;
                        
            switch (type)
            {
                case GizmoType.Yaw:
                    transformMatrix = Matrix.Scaling(Sphere.Radius / 2f) * Rotation * Matrix.Translation(Sphere.Center);
                    break;
                case GizmoType.Pitch:
                    transformMatrix = Matrix.Scaling(Sphere.Radius / 2f) * Matrix.RotationZ(MathUtil.Pi / 2) * Rotation * Matrix.Translation(Sphere.Center);
                    break;
                case GizmoType.Roll:
                    transformMatrix = Matrix.Scaling(Sphere.Radius / 2f) * Matrix.RotationX(MathUtil.Pi / 2) * Rotation * Matrix.Translation(Sphere.Center);
                    break;
            }

            for (int i = 0; i < SharpRenderer.torusVertices.Count; i++)
                vertices[i] = (Vector3)Vector3.Transform(SharpRenderer.torusVertices[i], transformMatrix);
            boundingBox = BoundingBox.FromPoints(vertices);
        }
                
        public override SharpMesh Mesh => SharpRenderer.Torus;

        protected override List<Models.Triangle> triangleList => SharpRenderer.torusTriangles;
    }
}