using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaTeleport_BFBB : DynaBase
    {
        public override string Note => "Version is always 1 or 2. Version 1 doesn't use the Rotation2";

        public DynaTeleport_BFBB(int version) : base()
        {
            this.version = version;
            MRKR_ID = 0;
            TargetDYNATeleportID = 0;
        }

        private int version;

        public DynaTeleport_BFBB(IEnumerable<byte> enumerable, int version) : base(enumerable)
        {
            this.version = version;
            MRKR_ID = Switch(BitConverter.ToUInt32(Data, 0x0));
            Opened = Switch(BitConverter.ToInt32(Data, 0x4));
            LaunchAngle = Switch(BitConverter.ToInt32(Data, 0x8));
            if (version == 2)
            {
                CameraAngle = Switch(BitConverter.ToInt32(Data, 0xC));
                TargetDYNATeleportID = Switch(BitConverter.ToUInt32(Data, 0x10));
            }
            else
            {
                TargetDYNATeleportID = Switch(BitConverter.ToUInt32(Data, 0x0C));
            }
        }

        public override bool HasReference(uint assetID)
        {
            if (MRKR_ID == assetID)
                return true;
            if (TargetDYNATeleportID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(MRKR_ID)));
            list.AddRange(BitConverter.GetBytes(Switch(Opened)));
            list.AddRange(BitConverter.GetBytes(Switch(LaunchAngle)));
            if (version == 2)
                list.AddRange(BitConverter.GetBytes(Switch(CameraAngle)));
            list.AddRange(BitConverter.GetBytes(Switch(TargetDYNATeleportID)));
            return list.ToArray();
        }

        private AssetID _MRKR_ID;
        public AssetID MRKR_ID
        {
            get => _MRKR_ID;
            set
            {
                _MRKR_ID = value;
                ValidateMRKR();
            }
        }

        private void ValidateMRKR()
        {
            foreach (ArchiveEditor ae in Program.MainForm.archiveEditors)
                if (ae.archive.ContainsAsset(_MRKR_ID))
                {
                    Asset asset = ae.archive.GetFromAssetID(_MRKR_ID);
                    if (asset is AssetMRKR MRKR)
                    {
                        this.MRKR = MRKR;
                        this.MRKR.isInvisible = true;
                    }
                    return;
                }

            MRKR = null;
        }

        [Category("Teleport Box")]
        public int Opened { get; set; }
        private int _rotation;
        [Category("Teleport Box"), Browsable(true)]
        public int LaunchAngle
        {
            get => _rotation;
            set
            {
                _rotation = value;
                CreateTransformMatrix();
            }
        }
        [Category("Teleport Box")]
        public int CameraAngle { get; set; }
        [Category("Teleport Box")]
        public AssetID TargetDYNATeleportID { get; set; }

        [Category("Teleport Box"), Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionX
        {
            get
            {
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

        [Category("Teleport Box"), Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionY
        {
            get
            {
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

        [Category("Teleport Box"), Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionZ
        {
            get
            {
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
        private static readonly uint _modelAssetID = HipHopFile.Functions.BKDRHash("teleportation_box_bind");

        private AssetMRKR MRKR;

        public override void CreateTransformMatrix()
        {
            ValidateMRKR();
            world = Matrix.RotationY(MathUtil.DegreesToRadians(LaunchAngle)) * Matrix.Translation(PositionX, PositionY, PositionZ);

            List<Vector3> vertexList = new List<Vector3>();
            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(_modelAssetID) &&
                ArchiveEditorFunctions.renderingDictionary[_modelAssetID].HasRenderWareModelFile() &&
                ArchiveEditorFunctions.renderingDictionary[_modelAssetID].GetRenderWareModelFile() != null)
            {
                CreateBoundingBox(ArchiveEditorFunctions.renderingDictionary[_modelAssetID].GetRenderWareModelFile().vertexListG);
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

            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(_modelAssetID))
            {
                if (ArchiveEditorFunctions.renderingDictionary[_modelAssetID] is AssetMINF MINF)
                {
                    if (MINF.HasRenderWareModelFile())
                        triangles = ArchiveEditorFunctions.renderingDictionary[_modelAssetID].GetRenderWareModelFile().triangleList.ToArray();
                    else
                        triangles = null;
                }
                else
                    triangles = ArchiveEditorFunctions.renderingDictionary[_modelAssetID].GetRenderWareModelFile().triangleList.ToArray();
            }
            else
                triangles = null;
        }

        public override void Draw(SharpRenderer renderer, bool isSelected)
        {
            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(_modelAssetID))
                ArchiveEditorFunctions.renderingDictionary[_modelAssetID].Draw(renderer, world, isSelected ? renderer.selectedObjectColor : Vector4.One);
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
        
        public override BoundingSphere GetGizmoCenter()
        {
            BoundingSphere boundingSphere = BoundingSphere.FromBox(boundingBox);
            boundingSphere.Radius *= 0.9f;
            return boundingSphere;
        }
    }
}