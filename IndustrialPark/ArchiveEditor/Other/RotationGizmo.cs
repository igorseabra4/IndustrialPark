using SharpDX;
using System.Collections.Generic;

namespace IndustrialPark
{
    public class RotationGizmo : GizmoBase
    {
        public RotationGizmo(GizmoType type) : base(type)
        {
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

            vertices = new Vector3[SharpRenderer.torusVertices.Count];
            for (int i = 0; i < SharpRenderer.torusVertices.Count; i++)
                vertices[i] = (Vector3)Vector3.Transform(SharpRenderer.torusVertices[i], transformMatrix);
            boundingBox = BoundingBox.FromPoints(vertices);
        }
                
        public override SharpMesh Mesh => SharpRenderer.Torus;

        protected override List<Models.Triangle> triangleList => SharpRenderer.torusTriangles;

        public override bool TriangleIntersection(Ray r)
        {
            foreach (Models.Triangle t in SharpRenderer.torusTriangles)
            {
                Vector3 v1 = vertices[t.vertex1];
                Vector3 v2 = vertices[t.vertex2];
                Vector3 v3 = vertices[t.vertex3];

                if (r.Intersects(ref v1, ref v2, ref v3, out float distance))
                {
                    clickPosition = r.Position + distance * r.Direction;
                    return true;
                }
            }
            return false;
        }

        public Vector3 clickPosition;
    }
}