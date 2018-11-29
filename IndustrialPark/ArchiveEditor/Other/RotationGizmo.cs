using SharpDX;
using System.Collections.Generic;

namespace IndustrialPark
{
    public class RotationGizmo : GizmoBase
    {
        public RotationGizmo(GizmoType type) : base(type)
        {
        }

        public void SetPosition(BoundingSphere Sphere, float Yaw, float Pitch, float Roll)
        {
            if (Sphere.Radius < 1f)
                Sphere.Radius = 1f;

            Matrix RotationMatrix = Matrix.RotationYawPitchRoll(MathUtil.DegreesToRadians(Yaw), MathUtil.DegreesToRadians(Pitch), MathUtil.DegreesToRadians(Roll));
            
            switch (type)
            {
                case GizmoType.Yaw:
                    transformMatrix = Matrix.Scaling(Sphere.Radius / 2f) * RotationMatrix * Matrix.Translation(Sphere.Center);
                    break;
                case GizmoType.Pitch:
                    transformMatrix = Matrix.Scaling(Sphere.Radius / 2f) * Matrix.RotationZ(MathUtil.Pi / 2) * RotationMatrix * Matrix.Translation(Sphere.Center);
                    break;
                case GizmoType.Roll:
                    transformMatrix = Matrix.Scaling(Sphere.Radius / 2f) * Matrix.RotationX(MathUtil.Pi / 2) * RotationMatrix * Matrix.Translation(Sphere.Center);
                    break;
            }

            boundingBox = BoundingBox.FromPoints(SharpRenderer.torusVertices.ToArray());
            boundingBox.Maximum = (Vector3)Vector3.Transform(boundingBox.Maximum, transformMatrix);
            boundingBox.Minimum = (Vector3)Vector3.Transform(boundingBox.Minimum, transformMatrix);
        }
        
        public override SharpMesh Mesh => SharpRenderer.Torus;

        public override bool TriangleIntersection(Ray r)
        {
            List<Vector3> torusVertices = SharpRenderer.torusVertices;

            foreach (Models.Triangle t in SharpRenderer.torusTriangles)
            {
                Vector3 v1 = (Vector3)Vector3.Transform(torusVertices[t.vertex1], transformMatrix);
                Vector3 v2 = (Vector3)Vector3.Transform(torusVertices[t.vertex2], transformMatrix);
                Vector3 v3 = (Vector3)Vector3.Transform(torusVertices[t.vertex3], transformMatrix);

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