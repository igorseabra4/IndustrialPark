using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;
using HipHopFile;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class DynaGObjectTeleport : DynaBase
    {
        public string Note => "Version is always 1 or 2.";

        public override int StructSize => version == 1 ? 0x10 : 0x14;

        public DynaGObjectTeleport(AssetDYNA asset, int version) : base(asset)
        {
            if (asset.game == Game.Incredibles)
                version = 1;

            this.version = version;
        }
        
        private readonly int version;
        
        public override bool HasReference(uint assetID)
        {
            if (MRKR_ID == assetID)
                return true;
            if (TargetDYNATeleportID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            if (MRKR_ID == 0)
                result.Add("Teleport with no MRKR reference");
            Asset.Verify(MRKR_ID, ref result);
            if (TargetDYNATeleportID == 0)
                result.Add("Teleport with no target reference");
            Asset.Verify(TargetDYNATeleportID, ref result);
        }
        
        public AssetID MRKR_ID
        {
            get => ReadUInt(0x00);
            set
            {
                Write(0x00, value);
                ValidateMRKR();
            }
        }

        public int Opened
        {
            get => ReadInt(0x04);
            set => Write(0x04, value);
        }

        public int LaunchAngle
        {
            get => ReadInt(0x08);
            set
            {
                Write(0x08, value);
                CreateTransformMatrix();
            }
        }

        [Description("Not used in version 1 or Movie.")]
        public int CameraAngle
        {
            get => version == 1 ? 0 : ReadInt(0x0C);
            set
            {
                if (version != 1)
                    Write(0x0C, value);
            }
        }

        public AssetID TargetDYNATeleportID
        {
            get => version == 1 ? ReadUInt(0x0C) : ReadUInt(0x10);
            set => Write(version == 1 ? 0x0C : 0x10, value);
        }

        private void ValidateMRKR()
        {
            foreach (ArchiveEditor ae in Program.MainForm.archiveEditors)
                if (ae.archive.ContainsAsset(MRKR_ID) && ae.archive.GetFromAssetID(MRKR_ID) is AssetMRKR MRKR)
                {
                    this.MRKR = MRKR;
                    this.MRKR.isInvisible = true;
                    return;
                }
            MRKR = null;
        }

        [Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionX
        {
            get
            {
                ValidateMRKR();
                if (MRKR != null)
                    return MRKR.PositionX;
                return 0;
            }
            set
            {
                ValidateMRKR();
                if (MRKR != null)
                    MRKR.PositionX = value;
                CreateTransformMatrix();
            }
        }

        [Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionY
        {
            get
            {
                ValidateMRKR();
                if (MRKR != null)
                    return MRKR.PositionY;
                return 0;
            }
            set
            {
                ValidateMRKR();
                if (MRKR != null)
                    MRKR.PositionY = value;
                CreateTransformMatrix();
            }
        }

        [Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionZ
        {
            get
            {
                ValidateMRKR();
                if (MRKR != null)
                    return MRKR.PositionZ;
                return 0;
            }
            set
            {
                ValidateMRKR();
                if (MRKR != null)
                    MRKR.PositionZ = value;
                CreateTransformMatrix();
            }
        }

        public override bool IsRenderableClickable => true;

        private Matrix world;
        private BoundingBox boundingBox;
        private static readonly uint _modelAssetID = Functions.BKDRHash("teleportation_box_bind");

        private AssetMRKR MRKR;

        public override void CreateTransformMatrix()
        {
            ValidateMRKR();
            world = Matrix.RotationY(MathUtil.DegreesToRadians(LaunchAngle)) * Matrix.Translation(PositionX, PositionY, PositionZ);

            if (renderingDictionary.ContainsKey(_modelAssetID) &&
                renderingDictionary[_modelAssetID].HasRenderWareModelFile() &&
                renderingDictionary[_modelAssetID].GetRenderWareModelFile() != null)
            {
                CreateBoundingBox(renderingDictionary[_modelAssetID].GetRenderWareModelFile().vertexListG);
            }
            else
            {
                CreateBoundingBox(SharpRenderer.pyramidVertices);
            }
        }

        protected void CreateBoundingBox(List<Vector3> vertexList, float multiplier = 1f)
        {
            vertices = new Vector3[vertexList.Count];

            for (int i = 0; i < vertexList.Count; i++)
                vertices[i] = (Vector3)Vector3.Transform(vertexList[i] * multiplier, world);

            boundingBox = BoundingBox.FromPoints(vertices);

            if (renderingDictionary.ContainsKey(_modelAssetID))
            {
                if (renderingDictionary[_modelAssetID] is AssetMINF MINF)
                {
                    if (MINF.HasRenderWareModelFile())
                        triangles = MINF.GetRenderWareModelFile().triangleList.ToArray();
                    else
                        triangles = null;
                }
                else
                    triangles = renderingDictionary[_modelAssetID].GetRenderWareModelFile().triangleList.ToArray();
            }
            else
                triangles = null;
        }

        public override void Draw(SharpRenderer renderer, bool isSelected)
        {
            if (renderingDictionary.ContainsKey(_modelAssetID))
                renderingDictionary[_modelAssetID].Draw(renderer, world, isSelected ? renderer.selectedObjectColor : Vector4.One, Vector3.Zero);
            else
                renderer.DrawPyramid(world, isSelected);
        }

        public override float? IntersectsWith(Ray ray)
        {
            if (ray.Intersects(ref boundingBox, out float distance))
                return TriangleIntersection(ray, distance);
            return null;
        }

        public float? TriangleIntersection(Ray r, float initialDistance)
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

        protected Vector3[] vertices;
        protected RenderWareFile.Triangle[] triangles;
        
        public override BoundingBox GetBoundingBox()
        {
            return boundingBox;
        }

        public override float GetDistance(Vector3 cameraPosition)
        {
            return Vector3.Distance(cameraPosition, new Vector3(PositionX, PositionY, PositionZ));
        }
    }
}