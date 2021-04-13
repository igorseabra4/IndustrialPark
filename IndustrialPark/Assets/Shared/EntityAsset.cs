using HipHopFile;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public abstract class EntityAsset : BaseAsset, IRenderableAsset, IClickableAsset, IRotatableAsset, IScalableAsset
    {
        private const string categoryName = "Entity";

        [Category(categoryName)]
        public FlagBitmask VisibilityFlags { get; set; } = ByteFlagsDescriptor(
            "Visible",
            "Stackable");
        [Category(categoryName), TypeConverter(typeof(HexByteTypeConverter))]
        public byte TypeFlag { get; set; }
        [Category(categoryName)]
        public FlagBitmask Flag0A { get; set; } = ByteFlagsDescriptor();
        [Category(categoryName)]
        public FlagBitmask SolidityFlags { get; set; } = ByteFlagsDescriptor(
            null,
            "Precise Collision",
            null,
            null,
            "Hittable",
            "Animate Collision",
            null,
            "Ledge Grab");

        protected Vector3 _position;
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float PositionX
        {
            get => _position.X;
            set { _position.X = value; CreateTransformMatrix(); }
        }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float PositionY
        {
            get => _position.Y;
            set { _position.Y = value; CreateTransformMatrix(); }
        }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float PositionZ
        {
            get => _position.Z;
            set { _position.Z = value; CreateTransformMatrix(); }
        }

        protected float _yaw;
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public virtual float Yaw
        {
            get => MathUtil.RadiansToDegrees(_yaw);
            set { _yaw = MathUtil.DegreesToRadians(value); CreateTransformMatrix(); }
        }

        protected float _pitch;
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public virtual float Pitch
        {
            get => MathUtil.RadiansToDegrees(_pitch);
            set { _pitch = MathUtil.DegreesToRadians(value); CreateTransformMatrix(); }
        }

        protected float _roll;
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public virtual float Roll
        {
            get => MathUtil.RadiansToDegrees(_roll);
            set { _roll = MathUtil.DegreesToRadians(value); CreateTransformMatrix(); }
        }

        protected Vector3 _scale;
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public virtual float ScaleX
        {
            get => _scale.X;
            set { _scale.X = value; CreateTransformMatrix(); }
        }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public virtual float ScaleY
        {
            get => _scale.Y;
            set { _scale.Y = value; CreateTransformMatrix(); }
        }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public virtual float ScaleZ
        {
            get => _scale.Z;
            set { _scale.Z = value; CreateTransformMatrix(); }
        }

        protected Vector4 _color;
        [Category(categoryName + " Color"), DisplayName("Red (0 - 1)")]
        public float ColorRed
        {
            get => _color.X;
            set => _color.X = value;
        }
        [Category(categoryName + " Color"), DisplayName("Green (0 - 1)")]
        public float ColorGreen
        {
            get => _color.Y;
            set => _color.Y = value;
        }
        [Category(categoryName + " Color"), DisplayName("Blue (0 - 1)")]
        public float ColorBlue
        {
            get => _color.Z;
            set => _color.Z = value;
        }
        [Category(categoryName + " Color"), DisplayName("Alpha (0 - 1)")]
        public float ColorAlpha
        {
            get => _color.W;
            set => _color.W = value;
        }
        [Category(categoryName + " Color"), DisplayName("Color - (A,) R, G, B")]
        public System.Drawing.Color Color_ARGB
        {
            get => System.Drawing.Color.FromArgb(BitConverter.ToInt32(new byte[] { (byte)(ColorBlue * 255), (byte)(ColorGreen * 255), (byte)(ColorRed * 255), (byte)(ColorAlpha * 255) }, 0));
            set
            {
                ColorRed = value.R / 255f;
                ColorGreen = value.G / 255f;
                ColorBlue = value.B / 255f;
                ColorAlpha = value.A / 255f;
            }
        }

        [Category(categoryName + " Color")]
        public float ColorAlphaSpeed { get; set; }

        protected uint _modelAssetID;
        [Category(categoryName + " References")]
        public AssetID Model_AssetID
        {
            get => _modelAssetID;
            set { _modelAssetID = value; CreateTransformMatrix(); }
        }

        [Category(categoryName + " References")]
        public AssetID Surface_AssetID { get; set; }

        [Category(categoryName + " References")]
        public virtual AssetID Animation_AssetID { get; set; }

        protected int entityEndPosition => game == Game.BFBB ? 0x54 : 0x50;

        public EntityAsset(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = baseEndPosition;

            VisibilityFlags.FlagValueByte = reader.ReadByte();
            TypeFlag = reader.ReadByte();
            Flag0A.FlagValueByte = reader.ReadByte();
            SolidityFlags.FlagValueByte = reader.ReadByte();
            if (game == Game.BFBB)
                reader.ReadInt32();
            Surface_AssetID = reader.ReadUInt32();
            _yaw = reader.ReadSingle();
            _pitch = reader.ReadSingle();
            _roll = reader.ReadSingle();
            _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            _scale = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            _color = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            ColorAlphaSpeed = reader.ReadSingle();
            _modelAssetID = reader.ReadUInt32();
            Animation_AssetID = reader.ReadUInt32();

            CreateTransformMatrix();

            renderableAssets.Add(this);
        }

        // meant for use with DUPC VIL only
        protected EntityAsset(EndianBinaryReader reader) : base(reader)
        {
            VisibilityFlags.FlagValueByte = reader.ReadByte();
            TypeFlag = reader.ReadByte();
            Flag0A.FlagValueByte = reader.ReadByte();
            SolidityFlags.FlagValueByte = reader.ReadByte();
            if (game == Game.BFBB)
                reader.ReadInt32();
            Surface_AssetID = reader.ReadUInt32();
            _yaw = reader.ReadSingle();
            _pitch = reader.ReadSingle();
            _roll = reader.ReadSingle();
            _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            _scale = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            _color = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            ColorAlphaSpeed = reader.ReadSingle();
            _modelAssetID = reader.ReadUInt32();
            Animation_AssetID = reader.ReadUInt32();
        }

        public byte[] SerializeEntity(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(SerializeBase(platform));

            writer.Write(VisibilityFlags.FlagValueByte);
            writer.Write(TypeFlag);
            writer.Write(Flag0A.FlagValueByte);
            writer.Write(SolidityFlags.FlagValueByte);
            if (game == Game.BFBB)
                writer.Write(0);
            writer.Write(Surface_AssetID);
            writer.Write(_yaw);
            writer.Write(_pitch);
            writer.Write(_roll);
            writer.Write(_position.X);
            writer.Write(_position.Y);
            writer.Write(_position.Z);
            writer.Write(_scale.X);
            writer.Write(_scale.Y);
            writer.Write(_scale.Z);
            writer.Write(_color.X);
            writer.Write(_color.Y);
            writer.Write(_color.Z);
            writer.Write(_color.W);
            writer.Write(ColorAlphaSpeed);
            writer.Write(_modelAssetID);
            writer.Write(Animation_AssetID);

            return writer.ToArray();
        }

        [Browsable(false)]
        public Matrix world { get; protected set; }
        protected BoundingBox boundingBox;

        [Browsable(false)]
        public abstract bool DontRender { get; }

        public virtual void CreateTransformMatrix()
        {
            world = Matrix.Scaling(_scale)
                * Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll)
                * Matrix.Translation(_position);

            CreateBoundingBox();
        }

        protected virtual void CreateBoundingBox()
        {
            if (renderingDictionary.ContainsKey(_modelAssetID) &&
                renderingDictionary[_modelAssetID].HasRenderWareModelFile() &&
                renderingDictionary[_modelAssetID].GetRenderWareModelFile() != null)
            {
                CreateBoundingBox(renderingDictionary[_modelAssetID].GetRenderWareModelFile().vertexListG);
            }
            else
            {
                CreateBoundingBox(SharpRenderer.cubeVertices, 0.5f);
            }
        }

        protected Vector3[] vertices;
        protected RenderWareFile.Triangle[] triangles;

        protected virtual void CreateBoundingBox(List<Vector3> vertexList, float multiplier = 1f)
        {
            vertices = new Vector3[vertexList.Count];

            for (int i = 0; i < vertexList.Count; i++)
                vertices[i] = (Vector3)Vector3.Transform(vertexList[i] * multiplier, world);

            boundingBox = BoundingBox.FromPoints(vertices);

            if (renderingDictionary.ContainsKey(_modelAssetID))
            {
                IAssetWithModel assetWithModel = renderingDictionary[_modelAssetID];
                if (assetWithModel.HasRenderWareModelFile())
                    triangles = assetWithModel.GetRenderWareModelFile().triangleList.ToArray();
                else
                    triangles = null;
            }
            else
                triangles = null;
        }

        public virtual bool ShouldDraw(SharpRenderer renderer)
        {
            if (isSelected)
                return true;
            if (DontRender)
                return false;
            if (isInvisible)
                return false;
            if (movementPreview)
                return true;

            if (AssetMODL.renderBasedOnLodt)
            {
                if (GetDistanceFrom(renderer.Camera.Position) <
                    (AssetLODT.MaxDistances.ContainsKey(_modelAssetID) ?
                    AssetLODT.MaxDistances[_modelAssetID] : SharpRenderer.DefaultLODTDistance))
                    return renderer.frustum.Intersects(ref boundingBox);
                
                return false;
            }

            return renderer.frustum.Intersects(ref boundingBox);
        }

        public virtual void Draw(SharpRenderer renderer)
        {
            if (renderingDictionary.ContainsKey(_modelAssetID))
                renderingDictionary[_modelAssetID].Draw(renderer, LocalWorld(), isSelected ? renderer.selectedObjectColor * _color : _color, UvAnimOffset);
            else
                renderer.DrawCube(LocalWorld(), isSelected);
        }

        [Browsable(false)]
        public virtual bool SpecialBlendMode => !renderingDictionary.ContainsKey(_modelAssetID) || renderingDictionary[_modelAssetID].SpecialBlendMode;

        public virtual float? GetIntersectionPosition(SharpRenderer renderer, Ray ray)
        {
            if (!ShouldDraw(renderer))
                return null;

            if (ray.Intersects(ref boundingBox, out float distance))
                return TriangleIntersection(ray, distance);
            return null;
        }

        protected virtual float? TriangleIntersection(Ray r, float initialDistance)
        {
            if (triangles == null)
                return initialDistance;

            float? smallestDistance = null;

            foreach (RenderWareFile.Triangle t in triangles)
                if (r.Intersects(ref vertices[t.vertex1], ref vertices[t.vertex2], ref vertices[t.vertex3], out float distance))
                    if (smallestDistance == null || distance < smallestDistance)
                        smallestDistance = distance;
                
            return smallestDistance;
        }

        public BoundingBox GetBoundingBox()
        {
            return boundingBox;
        }

        public virtual float GetDistanceFrom(Vector3 cameraPosition)
        {
            float min = Vector3.Distance(cameraPosition, _position);
            float d;

            foreach (var v in boundingBox.GetCorners())
                if ((d = Vector3.Distance(cameraPosition, v)) < min)
                    min = d;

            return min;
        }

        public override bool HasReference(uint assetID) => Surface_AssetID == assetID || Model_AssetID == assetID || Animation_AssetID == assetID ||
            base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Verify(Surface_AssetID, ref result);
            if (Model_AssetID == 0)
                result.Add(GetType().ToString().Replace("Asset", "") + " with Model_AssetID set to 0");
            if (!(this is AssetTRIG))
                Verify(Model_AssetID, ref result);
            Verify(Animation_AssetID, ref result);
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game != Game.BFBB)
                dt.RemoveProperty("PaddingC");

            base.SetDynamicProperties(dt);
        }

        public static bool movementPreview = false;

        protected EntityAsset FindDrivenByAsset(out bool found, out bool useRotation)
        {
            foreach (Link assetEvent in _links)
            {
                uint PlatID = 0;

                if ((EventBFBB)assetEvent.EventSendID == EventBFBB.Drivenby)
                    PlatID = assetEvent.TargetAssetID;
                else if ((EventBFBB)assetEvent.EventSendID == EventBFBB.Mount)
                    PlatID = assetEvent.ArgumentAssetID;
                else continue;

                foreach (ArchiveEditor ae in Program.MainForm.archiveEditors)
                    if (ae.archive.ContainsAsset(PlatID))
                    {
                        Asset asset = ae.archive.GetFromAssetID(PlatID);
                        if (asset is EntityAsset Placeable)
                        {
                            found = true;
                            useRotation = assetEvent.FloatParameter1 != 0 || (EventBFBB)assetEvent.EventSendID == EventBFBB.Mount;
                            return Placeable;
                        }
                    }
            }

            found = false;
            useRotation = false;
            return null;
        }

        public virtual Matrix PlatLocalRotation()
        {
            return Matrix.Identity;
        }

        public virtual Matrix LocalWorld()
        {
            if (movementPreview)
            {
                EntityAsset driver = FindDrivenByAsset(out bool found, out bool useRotation);

                if (found)
                {
                    return Matrix.Scaling(_scale)
                        * Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll)
                        * Matrix.Translation(_position - driver._position)
                        * (useRotation ? driver.PlatLocalRotation() : Matrix.Identity)
                        * Matrix.Translation((Vector3)Vector3.Transform(Vector3.Zero, driver.LocalWorld()));
                }
            }

            return world;
        }

        public virtual void Reset()
        {
            localFrameCounter = -1;
            FindSurf();
        }

        protected Vector3 uvTransSpeed;
        protected int localFrameCounter = -1;

        private void FindSurf()
        {
            foreach (ArchiveEditor ae in Program.MainForm.archiveEditors)
                if (ae.archive.ContainsAsset(Surface_AssetID))
                    if (ae.archive.GetFromAssetID(Surface_AssetID) is AssetSURF SURF)
                    {
                        uvTransSpeed = new Vector3(SURF.UVEffects1_TransSpeed_X, SURF.UVEffects1_TransSpeed_Y, SURF.UVEffects1_TransSpeed_Z);
                        return;
                    }
            uvTransSpeed = Vector3.Zero;
        }

        [Browsable(false)]
        public Vector3 UvAnimOffset => movementPreview ? uvTransSpeed * localFrameCounter++ / 60f : Vector3.Zero;
    }
}