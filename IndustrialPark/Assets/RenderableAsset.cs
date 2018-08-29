using HipHopFile;
using SharpDX;
using System;
using System.Collections.Generic;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class RenderableAsset : Asset
    {
        public RenderableAsset(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;

        public Matrix world;
        public int modelAssetID;

        public override void Setup(SharpRenderer renderer, bool defaultMode = true)
        {
            modelAssetID = Switch(BitConverter.ToInt32(AHDR.containedFile, 0x4C));

            Rotation.Y = Switch(BitConverter.ToSingle(AHDR.containedFile, 0x14));
            Rotation.X = Switch(BitConverter.ToSingle(AHDR.containedFile, 0x18));
            Rotation.Z = Switch(BitConverter.ToSingle(AHDR.containedFile, 0x1C));

            Position.X = Switch(BitConverter.ToSingle(AHDR.containedFile, 0x20));
            Position.Y = Switch(BitConverter.ToSingle(AHDR.containedFile, 0x24));
            Position.Z = Switch(BitConverter.ToSingle(AHDR.containedFile, 0x28));

            Scale.X = Switch(BitConverter.ToSingle(AHDR.containedFile, 0x2C));
            Scale.Y = Switch(BitConverter.ToSingle(AHDR.containedFile, 0x30));
            Scale.Z = Switch(BitConverter.ToSingle(AHDR.containedFile, 0x34));

            world = Matrix.Scaling(Scale)
            * Matrix.RotationY(Rotation.Y)
            * Matrix.RotationX(Rotation.X)
            * Matrix.RotationZ(Rotation.Z)
            * Matrix.Translation(Position);

            boundingBox = CreateBoundingBox(renderer, modelAssetID);
            boundingBox.Maximum = (Vector3)Vector3.Transform(boundingBox.Maximum, world);
            boundingBox.Minimum = (Vector3)Vector3.Transform(boundingBox.Minimum, world);

            ArchiveEditorFunctions.renderableAssetSet.Add(this);
        }

        protected virtual BoundingBox CreateBoundingBox(SharpRenderer renderer, int modelAssetID)
        {
            List<Vector3> list = new List<Vector3>();
            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(modelAssetID))
            {
                try
                {
                    list.AddRange(ArchiveEditorFunctions.renderingDictionary[modelAssetID].GetRenderWareModelFile().GetVertexList());
                }
                catch
                {
                    return BoundingBox.FromPoints(renderer.cubeVertices.ToArray());
                }
            }
            else
                return BoundingBox.FromPoints(renderer.cubeVertices.ToArray());

            return BoundingBox.FromPoints(list.ToArray());
        }

        public virtual void Draw(SharpRenderer renderer)
        {
            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(modelAssetID))
            {
                ArchiveEditorFunctions.renderingDictionary[modelAssetID].Draw(renderer, world, isSelected);
            }
            else
            {
                renderer.DrawCube(world, isSelected);
            }
        }

        public BoundingBox boundingBox;

        public float? IntersectsWith(Ray ray)
        {
            if (ray.Intersects(ref boundingBox, out float distance))
            {
                return TriangleIntersection(ray, distance);
            }
            else return null;
        }

        private float? TriangleIntersection(Ray r, float initialDistance)
        {
            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(modelAssetID))
            {
                RenderWareModelFile rwmf;

                if (ArchiveEditorFunctions.renderingDictionary[modelAssetID] is AssetMINF MINF)
                {
                    if (MINF.HasRenderWareModelFile())
                        rwmf = ArchiveEditorFunctions.renderingDictionary[modelAssetID].GetRenderWareModelFile();
                    else return initialDistance;
                }
                else rwmf = ArchiveEditorFunctions.renderingDictionary[modelAssetID].GetRenderWareModelFile();

                foreach (RenderWareFile.Triangle t in rwmf.triangleList)
                {
                    Vector3 v1 = (Vector3)Vector3.Transform(rwmf.vertexListG[t.vertex1], world);
                    Vector3 v2 = (Vector3)Vector3.Transform(rwmf.vertexListG[t.vertex2], world);
                    Vector3 v3 = (Vector3)Vector3.Transform(rwmf.vertexListG[t.vertex3], world);

                    if (r.Intersects(ref v1, ref v2, ref v3, out float distance))
                        return distance;
                }
            }
            else
                return initialDistance;

            return null;
        }
    }
}