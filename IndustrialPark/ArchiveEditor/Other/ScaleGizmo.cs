using SharpDX;
using System;
using System.Collections.Generic;

namespace IndustrialPark
{
    public class ScaleGizmo : GizmoBase
    {
        public ScaleGizmo(GizmoType type) : base(type)
        {
        }

        public void SetPosition(BoundingBox Box, Matrix Rotation)
        {
            switch (type)
            {
                case GizmoType.ScaleX:
                    transformMatrix = Matrix.Scaling(Math.Max(Math.Abs(Box.Width) / 6f, 1f)) * Matrix.Translation(Math.Abs(Box.Width) + 1f, 0f, 0f) * Rotation * Matrix.Translation(Box.Center);
                    break;
                case GizmoType.ScaleY:
                    transformMatrix = Matrix.Scaling(Math.Max(Math.Abs(Box.Height) / 6f, 1f)) * Matrix.Translation(0f, Math.Abs(Box.Height) + 1f, 0f) * Rotation * Matrix.Translation(Box.Center);
                    break;
                case GizmoType.ScaleZ:
                    transformMatrix = Matrix.Scaling(Math.Max(Math.Abs(Box.Depth) / 6f, 1f)) * Matrix.Translation(0f, 0f, Math.Abs(Box.Depth) + 1f) * Rotation * Matrix.Translation(Box.Center);
                    break;
                case GizmoType.ScaleAll:
                    transformMatrix = Matrix.Scaling(Box.Size.Length() / 8f) * Rotation * Matrix.Translation(Box.Center);
                    break;
            }

            boundingBox = BoundingBox.FromPoints(SharpRenderer.cubeVertices.ToArray());
            boundingBox.Maximum = (Vector3)Vector3.Transform(boundingBox.Maximum, transformMatrix);
            boundingBox.Minimum = (Vector3)Vector3.Transform(boundingBox.Minimum, transformMatrix);
        }

        public override SharpMesh Mesh => SharpRenderer.Cube;

        public override bool TriangleIntersection(Ray r)
        {
            List<Vector3> cubeVertices = SharpRenderer.cubeVertices;

            foreach (Models.Triangle t in SharpRenderer.cubeTriangles)
            {
                Vector3 v1 = (Vector3)Vector3.Transform(cubeVertices[t.vertex1], transformMatrix);
                Vector3 v2 = (Vector3)Vector3.Transform(cubeVertices[t.vertex2], transformMatrix);
                Vector3 v3 = (Vector3)Vector3.Transform(cubeVertices[t.vertex3], transformMatrix);

                if (r.Intersects(ref v1, ref v2, ref v3))
                    return true;
            }
            return false;
        }
    }
}