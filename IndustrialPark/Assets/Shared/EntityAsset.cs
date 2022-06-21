using AssetEditorColors;
using HipHopFile;
using SharpDX;
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
        [Category(categoryName)]
        public AssetByte TypeFlag { get; set; }
        [Category(categoryName)]
        public FlagBitmask Flag0A { get; set; } = ByteFlagsDescriptor();
        [Category(categoryName)]
        public FlagBitmask SolidityFlags { get; set; } = ByteFlagsDescriptor(
            "Diggable",
            "Precise Collision",
            null,
            null,
            "Hittable",
            "Animate Collision",
            null,
            "Ledge Grab");

        protected Vector3 _position;
        [Category(categoryName)]
        public virtual AssetSingle PositionX
        {
            get => _position.X;
            set { _position.X = value; CreateTransformMatrix(); }
        }
        [Category(categoryName)]
        public virtual AssetSingle PositionY
        {
            get => _position.Y;
            set { _position.Y = value; CreateTransformMatrix(); }
        }
        [Category(categoryName)]
        public virtual AssetSingle PositionZ
        {
            get => _position.Z;
            set { _position.Z = value; CreateTransformMatrix(); }
        }

        protected AssetSingle _yaw;
        [Category(categoryName)]
        public AssetSingle Yaw
        {
            get => MathUtil.RadiansToDegrees(_yaw);
            set { _yaw = MathUtil.DegreesToRadians(value); CreateTransformMatrix(); }
        }

        protected AssetSingle _pitch;
        [Category(categoryName)]
        public AssetSingle Pitch
        {
            get => MathUtil.RadiansToDegrees(_pitch);
            set { _pitch = MathUtil.DegreesToRadians(value); CreateTransformMatrix(); }
        }

        protected AssetSingle _roll;
        [Category(categoryName)]
        public AssetSingle Roll
        {
            get => MathUtil.RadiansToDegrees(_roll);
            set { _roll = MathUtil.DegreesToRadians(value); CreateTransformMatrix(); }
        }

        protected Vector3 _scale;
        [Category(categoryName)]
        public virtual AssetSingle ScaleX
        {
            get => _scale.X;
            set { _scale.X = value; CreateTransformMatrix(); }
        }
        [Category(categoryName)]
        public virtual AssetSingle ScaleY
        {
            get => _scale.Y;
            set { _scale.Y = value; CreateTransformMatrix(); }
        }
        [Category(categoryName)]
        public virtual AssetSingle ScaleZ
        {
            get => _scale.Z;
            set { _scale.Z = value; CreateTransformMatrix(); }
        }

        protected Vector4 _color;
        [Category(categoryName + " Color"), DisplayName("Red (0 - 1)")]
        public AssetSingle ColorRed
        {
            get => _color.X;
            set => _color.X = value;
        }
        [Category(categoryName + " Color"), DisplayName("Green (0 - 1)")]
        public AssetSingle ColorGreen
        {
            get => _color.Y;
            set => _color.Y = value;
        }
        [Category(categoryName + " Color"), DisplayName("Blue (0 - 1)")]
        public AssetSingle ColorBlue
        {
            get => _color.Z;
            set => _color.Z = value;
        }
        [Category(categoryName + " Color"), DisplayName("Alpha (0 - 1)")]
        public AssetSingle ColorAlpha
        {
            get => _color.W;
            set => _color.W = value;
        }
        [Category(categoryName + " Color")]
        public AssetColor Color_RBGA
        {
            get => new AssetColor((byte)(ColorRed * 255), (byte)(ColorGreen * 255), (byte)(ColorBlue * 255), (byte)(ColorAlpha * 255));
            set
            {
                ColorRed = value.R / 255f;
                ColorGreen = value.G / 255f;
                ColorBlue = value.B / 255f;
                ColorAlpha = value.A / 255f;
            }
        }

        [Category(categoryName + " Color")]
        public AssetSingle ColorAlphaSpeed { get; set; }

        protected uint _model;
        [Category(categoryName + " References"), Description("Model or ModelInfo asset")]
        public AssetID Model
        {
            get => _model;
            set { _model = value; CreateTransformMatrix(); }
        }

        [Category(categoryName + " References")]
        public virtual AssetID Animation { get; set; }

        [Category(categoryName + " References")]
        public AssetID Surface { get; set; }

        protected int entityHeaderEndPosition => game == Game.BFBB ? 0x54 : 0x50;

        public EntityAsset(string assetName, AssetType assetType, BaseAssetType baseAssetType, Vector3 position) : base(assetName, assetType, baseAssetType)
        {
            VisibilityFlags.FlagValueByte = 1;
            SolidityFlags.FlagValueByte = 2;
            _position = position;
            _scale = new Vector3(1f, 1f, 1f);
            _color = new Vector4(1f, 1f, 1f, 1f);
            ColorAlphaSpeed = 255f;

            CreateTransformMatrix();
            AddToRenderableAssets(this);
        }

        public EntityAsset(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                VisibilityFlags.FlagValueByte = reader.ReadByte();
                TypeFlag = reader.ReadByte();
                Flag0A.FlagValueByte = reader.ReadByte();
                SolidityFlags.FlagValueByte = reader.ReadByte();
                if (game == Game.BFBB)
                    reader.ReadInt32();
                Surface = reader.ReadUInt32();
                _yaw = reader.ReadSingle();
                _pitch = reader.ReadSingle();
                _roll = reader.ReadSingle();
                _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                _scale = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                _color = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                ColorAlphaSpeed = reader.ReadSingle();
                _model = reader.ReadUInt32();
                Animation = reader.ReadUInt32();

                CreateTransformMatrix();
                AddToRenderableAssets(this);
            }
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
            Surface = reader.ReadUInt32();
            _yaw = reader.ReadSingle();
            _pitch = reader.ReadSingle();
            _roll = reader.ReadSingle();
            _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            _scale = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            _color = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            ColorAlphaSpeed = reader.ReadSingle();
            _model = reader.ReadUInt32();
            Animation = reader.ReadUInt32();
        }

        protected byte[] SerializeEntity(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeBase(endianness));

                writer.Write(VisibilityFlags.FlagValueByte);
                writer.Write(TypeFlag);
                writer.Write(Flag0A.FlagValueByte);
                writer.Write(SolidityFlags.FlagValueByte);
                if (game == Game.BFBB)
                    writer.Write(0);
                writer.Write(Surface);
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
                writer.Write(_model);
                writer.Write(Animation);

                return writer.ToArray();
            }
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
            Reset();
        }

        protected virtual void CreateBoundingBox()
        {
            var model = GetFromRenderingDictionary(_model);
            if (model != null)
                CreateBoundingBox(model.vertexListG);
            else
                CreateBoundingBox(SharpRenderer.cubeVertices, 0.5f);
        }

        protected Vector3[] vertices;
        protected RenderWareFile.Triangle[] triangles;

        protected virtual void CreateBoundingBox(List<Vector3> vertexList, float multiplier = 1f)
        {
            vertices = new Vector3[vertexList.Count];
            for (int i = 0; i < vertexList.Count; i++)
                vertices[i] = (Vector3)Vector3.Transform(vertexList[i] * multiplier, world);
            boundingBox = BoundingBox.FromPoints(vertices);
            triangles = GetFromRenderingDictionary(_model)?.triangleList.ToArray();
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
            if (AssetMODL.renderBasedOnLodt && GetDistanceFrom(renderer.Camera.Position) > AssetLODT.MaxDistanceTo(_model))
                return false;

            return renderer.frustum.Intersects(ref boundingBox);
        }

        public virtual void Draw(SharpRenderer renderer)
        {
            if (renderingDictionary.ContainsKey(_model))
                renderingDictionary[_model].Draw(renderer, LocalWorld(), isSelected ? renderer.selectedObjectColor * _color : _color, UvAnimOffset);
            else
                renderer.DrawCube(LocalWorld(), isSelected);
        }

        [Browsable(false)]
        public virtual bool SpecialBlendMode => !renderingDictionary.ContainsKey(_model) || renderingDictionary[_model].SpecialBlendMode;

        public virtual float? GetIntersectionPosition(SharpRenderer renderer, Ray ray)
        {
            if (ShouldDraw(renderer) && ray.Intersects(ref boundingBox))
                return triangles == null ? TriangleIntersection(ray, SharpRenderer.cubeTriangles, SharpRenderer.cubeVertices, world) : TriangleIntersection(ray);
            return null;
        }

        protected virtual float? TriangleIntersection(Ray ray)
        {
            float? smallestDistance = null;

            foreach (RenderWareFile.Triangle t in triangles)
                if (ray.Intersects(ref vertices[t.vertex1], ref vertices[t.vertex2], ref vertices[t.vertex3], out float distance))
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

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Verify(Surface, ref result);
            if (Model == 0)
                result.Add(GetType().ToString().Replace("Asset", "") + " with Model set to 0");
            if (!(this is AssetTRIG))
                Verify(Model, ref result);
            Verify(Animation, ref result);
        }

        public static bool movementPreview = false;

        protected EntityAsset FindDrivenByAsset(out bool useRotation)
        {
            foreach (var link in _links)
            {
                uint PlatID = 0;

                if ((EventBFBB)link.EventSendID == EventBFBB.Drivenby)
                    PlatID = link.TargetAsset;
                else if ((EventBFBB)link.EventSendID == EventBFBB.Mount)
                    PlatID = link.ArgumentAsset;
                else continue;

                foreach (var ae in Program.MainForm.archiveEditors)
                    if (ae.archive.ContainsAsset(PlatID))
                    {
                        var asset = ae.archive.GetFromAssetID(PlatID);
                        if (asset is EntityAsset entity)
                        {
                            useRotation = link.FloatParameter1 != 0 || (EventBFBB)link.EventSendID == EventBFBB.Mount;
                            return entity;
                        }
                    }
            }

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
                var driver = FindDrivenByAsset(out bool useRotation);

                if (driver != null)
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
            if (Program.MainForm != null && Surface != 0)
                foreach (ArchiveEditor ae in Program.MainForm.archiveEditors)
                    if (ae.archive.ContainsAsset(Surface))
                        if (ae.archive.GetFromAssetID(Surface) is AssetSURF SURF)
                        {
                            uvTransSpeed = new Vector3(SURF.zSurfUVFX.TransSpeed_X, SURF.zSurfUVFX.TransSpeed_Y, SURF.zSurfUVFX.TransSpeed_Z);
                            return;
                        }
            uvTransSpeed = Vector3.Zero;
        }

        [Browsable(false)]
        public Vector3 UvAnimOffset => movementPreview ? uvTransSpeed * localFrameCounter++ / 60f : Vector3.Zero;
    }
}