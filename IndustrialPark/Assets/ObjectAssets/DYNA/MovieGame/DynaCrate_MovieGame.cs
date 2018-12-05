using System;
using System.Collections.Generic;
using System.ComponentModel;
using SharpDX;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaCrate_MovieGame : DynaBase
    {
        public override string Note => "Version is always 2";

        public DynaCrate_MovieGame() : base()
        {
            SurfaceAssetID = 0;
            ModelAssetID = 0;
            Unknown4C = 0;
            Unknown50 = 0;
        }

        public DynaCrate_MovieGame(IEnumerable<byte> enumerable) : base (enumerable)
        {
            Unknown00 = Switch(BitConverter.ToInt32(Data, 0x0));
            Unknown04 = Data[0x04];
            Unknown05 = Data[0x05];
            Flags06 = Switch(BitConverter.ToInt16(Data, 0x06));
            VisibilityFlag = Data[0x08];
            TypeFlag = Data[0x08];
            UnknownFlag0A = Data[0x08];
            SolidityFlag = Data[0x08];
            SurfaceAssetID = Switch(BitConverter.ToUInt32(Data, 0x0C));
            Yaw = Switch(BitConverter.ToSingle(Data, 0x10));
            Pitch = Switch(BitConverter.ToSingle(Data, 0x14));
            Roll = Switch(BitConverter.ToSingle(Data, 0x18));
            PositionX = Switch(BitConverter.ToSingle(Data, 0x1C));
            PositionY = Switch(BitConverter.ToSingle(Data, 0x20));
            PositionZ = Switch(BitConverter.ToSingle(Data, 0x24));
            ScaleX = Switch(BitConverter.ToSingle(Data, 0x28));
            ScaleY = Switch(BitConverter.ToSingle(Data, 0x2C));
            ScaleZ = Switch(BitConverter.ToSingle(Data, 0x30));
            ColorRed = Switch(BitConverter.ToSingle(Data, 0x34));
            ColorGreen = Switch(BitConverter.ToSingle(Data, 0x38));
            ColorBlue = Switch(BitConverter.ToSingle(Data, 0x3C));
            ColorAlpha = Switch(BitConverter.ToSingle(Data, 0x40));
            UnknownFloat44 = Switch(BitConverter.ToSingle(Data, 0x44));
            ModelAssetID = Switch(BitConverter.ToUInt32(Data, 0x48));
            Unknown4C = Switch(BitConverter.ToUInt32(Data, 0x4C));
            Unknown50 = Switch(BitConverter.ToUInt32(Data, 0x50));

            CreateTransformMatrix();
        }

        public override bool IsRenderableClickable => true;

        private Matrix world;
        private BoundingBox boundingBox;

        public override void CreateTransformMatrix()
        {
            world = Matrix.Scaling(ScaleX, ScaleY, ScaleZ)
                * Matrix.RotationYawPitchRoll(Yaw, Pitch, Roll)
                * Matrix.Translation(PositionX, PositionY, PositionZ);

            CreateBoundingBox();
        }

        protected virtual void CreateBoundingBox()
        {
            List<Vector3> vertexList = new List<Vector3>();
            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(ModelAssetID) &&
                ArchiveEditorFunctions.renderingDictionary[ModelAssetID].HasRenderWareModelFile() &&
                ArchiveEditorFunctions.renderingDictionary[ModelAssetID].GetRenderWareModelFile() != null)
            {
                CreateBoundingBox(ArchiveEditorFunctions.renderingDictionary[ModelAssetID].GetRenderWareModelFile().vertexListG);
            }
            else
            {
                CreateBoundingBox(SharpRenderer.cubeVertices, 0.5f);
            }
        }

        protected Vector3[] vertices;
        protected RenderWareFile.Triangle[] triangles;

        protected void CreateBoundingBox(List<Vector3> vertexList, float multiplier = 1f)
        {
            vertices = new Vector3[vertexList.Count];

            for (int i = 0; i < vertexList.Count; i++)
                vertices[i] = (Vector3)Vector3.Transform(vertexList[i] * multiplier, world);

            boundingBox = BoundingBox.FromPoints(vertices);

            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(ModelAssetID))
            {
                if (ArchiveEditorFunctions.renderingDictionary[ModelAssetID] is AssetMINF MINF)
                {
                    if (MINF.HasRenderWareModelFile())
                        triangles = ArchiveEditorFunctions.renderingDictionary[ModelAssetID].GetRenderWareModelFile().triangleList.ToArray();
                    else
                        triangles = null;
                }
                else
                    triangles = ArchiveEditorFunctions.renderingDictionary[ModelAssetID].GetRenderWareModelFile().triangleList.ToArray();
            }
            else
                triangles = null;
        }

        public override void Draw(SharpRenderer renderer, bool isSelected)
        {
            Vector4 Color = new Vector4(ColorRed, ColorGreen, ColorBlue, ColorAlpha);
            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(ModelAssetID))
                ArchiveEditorFunctions.renderingDictionary[ModelAssetID].Draw(renderer, world, isSelected ? renderer.selectedObjectColor * Color : Color);
            else
                renderer.DrawCube(world, isSelected);
        }

        public override float? IntersectsWith(Ray ray)
        {
            if (ray.Intersects(ref boundingBox, out float distance))
                return TriangleIntersection(ray, distance);
            return null;
        }

        protected float? TriangleIntersection(Ray r, float initialDistance)
        {
            if (triangles == null)
                return initialDistance;

            bool hasIntersected = false;
            float smallestDistance = 1000f;

            foreach (RenderWareFile.Triangle t in triangles)
                if (r.Intersects(ref vertices[t.vertex1], ref vertices[t.vertex2], ref vertices[t.vertex3], out float distance))
                {
                    hasIntersected = true;

                    if (distance < smallestDistance)
                        smallestDistance = distance;
                }

            if (hasIntersected)
                return smallestDistance;
            return null;
        }
        
        public override BoundingSphere GetGizmoCenter()
        {
            BoundingSphere boundingSphere = BoundingSphere.FromBox(boundingBox);
            boundingSphere.Radius *= 0.9f;
            return boundingSphere;
        }

        public override BoundingBox GetBoundingBox()
        {
            return boundingBox;
        }

        public override float GetDistance(Vector3 cameraPosition)
        {
            return Vector3.Distance(cameraPosition, new Vector3(PositionX, PositionY, PositionZ));
        }

        public override bool HasReference(uint assetID)
        {
            if (SurfaceAssetID == assetID)
                return true;
            if (ModelAssetID == assetID)
                return true;
            if (Unknown4C == assetID)
                return true;
            if (Unknown50 == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(Unknown00)));
            list.Add(Unknown04);
            list.Add(Unknown05);
            list.AddRange(BitConverter.GetBytes(Switch(Flags06)));
            list.Add(VisibilityFlag);
            list.Add(TypeFlag);
            list.Add(UnknownFlag0A);
            list.Add(SolidityFlag);
            list.AddRange(BitConverter.GetBytes(Switch(SurfaceAssetID)));
            list.AddRange(BitConverter.GetBytes(Switch(Yaw)));
            list.AddRange(BitConverter.GetBytes(Switch(Pitch)));
            list.AddRange(BitConverter.GetBytes(Switch(Roll)));
            list.AddRange(BitConverter.GetBytes(Switch(PositionX)));
            list.AddRange(BitConverter.GetBytes(Switch(PositionY)));
            list.AddRange(BitConverter.GetBytes(Switch(PositionZ)));
            list.AddRange(BitConverter.GetBytes(Switch(ScaleX)));
            list.AddRange(BitConverter.GetBytes(Switch(ScaleY)));
            list.AddRange(BitConverter.GetBytes(Switch(ScaleZ)));
            list.AddRange(BitConverter.GetBytes(Switch(ColorRed)));
            list.AddRange(BitConverter.GetBytes(Switch(ColorGreen)));
            list.AddRange(BitConverter.GetBytes(Switch(ColorBlue)));
            list.AddRange(BitConverter.GetBytes(Switch(ColorAlpha)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat44)));
            list.AddRange(BitConverter.GetBytes(Switch(ModelAssetID)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown4C)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown50)));
            return list.ToArray();
        }

        public int Unknown00 { get; set; }
        public byte Unknown04 { get; set; }
        public byte Unknown05 { get; set; }
        public short Flags06 { get; set; }
        public byte VisibilityFlag { get; set; }
        public byte TypeFlag { get; set; }
        public byte UnknownFlag0A { get; set; }
        public byte SolidityFlag { get; set; }
        public AssetID SurfaceAssetID { get; set; }
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public float Roll { get; set; }
        public override float PositionX { get; set; }
        public override float PositionY { get; set; }
        public override float PositionZ { get; set; }
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }
        public float ScaleZ { get; set; }
        public float ColorRed { get; set; }
        public float ColorGreen { get; set; }
        public float ColorBlue { get; set; }
        public float ColorAlpha { get; set; }
        public float UnknownFloat44 { get; set; } 
        public AssetID ModelAssetID { get; set; }
        public AssetID Unknown4C { get; set; }
        public AssetID Unknown50 { get; set; }
    }
}