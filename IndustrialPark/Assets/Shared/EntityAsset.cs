using HipHopFile;
using IndustrialPark.Archive;
using IndustrialPark.AssetEditorColors;
using Newtonsoft.Json;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace IndustrialPark
{
    public abstract class EntityAsset : BaseAsset, IRenderableAsset, IClickableAsset, IRotatableAsset, IScalableAsset, IAssetCopyPasteTransformation
    {
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(Model);

        protected const string categoryNameReferences = "\t\t\t\tEntity References";

        protected uint _model;
        [Category(categoryNameReferences), Description("Model or Model Info asset"), ValidReferenceRequired]
        public AssetID Model
        {
            get => _model;
            set => _model = value;
        }

        [Category(categoryNameReferences)]
        public virtual AssetID Animation { get; set; }

        [Category(categoryNameReferences)]
        public AssetID Surface { get; set; }

        private const string categoryNameFlags = "\t\t\tEntity Flags";

        [Category(categoryNameFlags)]
        public FlagBitmask VisibilityFlags { get; set; } = ByteFlagsDescriptor(
            "Visible",
            "Stackable",
            null,
            null,
            null,
            null,
            "No Shadow");
        [Category(categoryNameFlags)]
        public AssetByte TypeFlag { get; set; }
        [Category(categoryNameFlags)]
        public FlagBitmask Flag0A { get; set; } = ByteFlagsDescriptor();
        [Category(categoryNameFlags)]
        public FlagBitmask SolidityFlags { get; set; } = ByteFlagsDescriptor(
            "Translucent On",
            "Precise Collision",
            null,
            "Grabbable",
            "Hittable",
            "Animate Collision",
            null,
            "Ledge Grab");

        protected const string categoryNamePlacement = "\t\tEntity Placement";

        protected Vector3 _position;
        [Category(categoryNamePlacement)]
        public virtual AssetSingle PositionX
        {
            get => _position.X;
            set => _position.X = value;
        }
        [Category(categoryNamePlacement)]
        public virtual AssetSingle PositionY
        {
            get => _position.Y;
            set => _position.Y = value;
        }
        [Category(categoryNamePlacement)]
        public virtual AssetSingle PositionZ
        {
            get => _position.Z;
            set => _position.Z = value;
        }

        protected float _yaw;
        [Category(categoryNamePlacement)]
        public AssetSingle Yaw
        {
            get => MathUtil.RadiansToDegrees(_yaw);
            set => _yaw = MathUtil.DegreesToRadians(value);
        }

        protected float _pitch;
        [Category(categoryNamePlacement)]
        public AssetSingle Pitch
        {
            get => MathUtil.RadiansToDegrees(_pitch);
            set => _pitch = MathUtil.DegreesToRadians(value);
        }

        protected float _roll;
        [Category(categoryNamePlacement)]
        public AssetSingle Roll
        {
            get => MathUtil.RadiansToDegrees(_roll);
            set => _roll = MathUtil.DegreesToRadians(value);
        }

        private const string scaleDescription = "(X, Y, Z) scale must be uniform on all 3 axes for solid objects, otherwise risking collision glitches. To circumvent this, use the 'Bake Scale' function to create a copy of the model with a non-uniform scale applied.";
        protected Vector3 _scale;

        [Category(categoryNamePlacement), Description(scaleDescription)]
        public virtual AssetSingle ScaleX
        {
            get => _scale.X;
            set => _scale.X = value;
        }
        [Category(categoryNamePlacement), Description(scaleDescription)]
        public virtual AssetSingle ScaleY
        {
            get => _scale.Y;
            set => _scale.Y = value;
        }
        [Category(categoryNamePlacement), Description(scaleDescription)]
        public virtual AssetSingle ScaleZ
        {
            get => _scale.Z;
            set => _scale.Z = value;
        }

        private const string categoryNameColor = "\tEntity Color";

        protected Vector4 _color;
        [Category(categoryNameColor), DisplayName("Red (0 - 1)")]
        public AssetSingle ColorRed
        {
            get => _color.X;
            set => _color.X = value;
        }
        [Category(categoryNameColor), DisplayName("Green (0 - 1)")]
        public AssetSingle ColorGreen
        {
            get => _color.Y;
            set => _color.Y = value;
        }
        [Category(categoryNameColor), DisplayName("Blue (0 - 1)")]
        public AssetSingle ColorBlue
        {
            get => _color.Z;
            set => _color.Z = value;
        }
        [Category(categoryNameColor), DisplayName("Alpha (0 - 1)"), Description("Alpha is turned off after the first player death on all games except Scooby. Use the SetOpacity event, with the opacity value as the first paramenter, to fix this.")]
        public AssetSingle ColorAlpha
        {
            get => _color.W;
            set => _color.W = value;
        }
        [Category(categoryNameColor)]
        public AssetColor ColorRGBA
        {
            get => AssetColor.FromVector4(ColorRed, ColorGreen, ColorBlue, ColorAlpha);
            set
            {
                ColorRed = value.R / 255f;
                ColorGreen = value.G / 255f;
                ColorBlue = value.B / 255f;
                ColorAlpha = value.A / 255f;
            }
        }

        [Category(categoryNameColor)]
        public AssetSingle ColorAlphaSpeed { get; set; }

        protected int entityHeaderEndPosition => game == Game.BFBB ? 0x54 : 0x50;

        public EntityAsset(string assetName, AssetType assetType, BaseAssetType baseAssetType, Vector3 position) : base(assetName, assetType, baseAssetType)
        {
            VisibilityFlags.FlagValueByte = 1;
            SolidityFlags.FlagValueByte = 2;
            _position = position;
            _scale = new Vector3(1f, 1f, 1f);
            _color = new Vector4(1f, 1f, 1f, 1f);
            ColorAlphaSpeed = 255f;
        }

        public EntityAsset(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using var reader = new EndianBinaryReader(AHDR.data, endianness);
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
        }

        // meant for use with DUPC VIL only
        protected EntityAsset(EndianBinaryReader reader, Game game) : base(reader, game)
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

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
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
        }

        [Browsable(false)]
        public Matrix world { get; protected set; }
        protected BoundingBox boundingBox;

        [Browsable(false)]
        public abstract bool DontRender { get; }

        public virtual void CreateTransformMatrix(SharpRenderer renderer)
        {
            world = ((renderer.Assets.GetFromRenderingDictionary(_model) is IAssetWithModel value) ? value.TransformMatrix : Matrix.Identity)
                * Matrix.Scaling(_scale)
                * Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll)
                * Matrix.Translation(_position);

            CreateBoundingBox(renderer);
            Reset();
        }

        protected virtual void CreateBoundingBox(SharpRenderer renderer)
        {
            var model = renderer.Assets.GetModelFromRenderingDictionary(_model);
            if (model != null)
                CreateBoundingBox(renderer, model.vertexListG);
            else
                CreateBoundingBox(renderer, renderer.cubeVertices, 0.5f);
        }

        protected Vector3[] vertices;
        protected List<RenderWareFile.Triangle> triangles;

        protected virtual void CreateBoundingBox(SharpRenderer renderer, List<Vector3> vertexList, float multiplier = 1f)
        {
            vertices = new Vector3[vertexList.Count];
            for (int i = 0; i < vertexList.Count; i++)
                vertices[i] = Vector3.TransformCoordinate(vertexList[i] * multiplier, world);
            boundingBox = BoundingBox.FromPoints(vertices);
            triangles = renderer.Assets.GetModelFromRenderingDictionary(_model)?.triangleList;
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

#if DEBUG
        public void DrawDebug(SharpRenderer renderer)
        {
            renderer.DrawBoundingBox(boundingBox);
            renderer.debugSelectedObjects.Add($"[{assetID:X8}] {assetName}");
        }
#endif

        public virtual void Draw(SharpRenderer renderer)
        {
#if DEBUG
            if (isSelected && !renderer.isDrawingSelectionMask)
                DrawDebug(renderer);
#endif
            if (renderer.Assets.GetFromRenderingDictionary(_model) is IAssetWithModel value)
                value.Draw(renderer, LocalWorld(), _color, UvAnimOffset, isSelected);
            else
                renderer.DrawCube(LocalWorld(), isSelected);
        }

        public virtual bool SpecialBlendMode(SharpRenderer renderer) => renderer.Assets.GetFromRenderingDictionary(_model) is not IAssetWithModel value || value.SpecialBlendMode(renderer);

        public virtual float? GetIntersectionPosition(SharpRenderer renderer, Ray ray)
        {
            if (ShouldDraw(renderer) && ray.Intersects(ref boundingBox))
                return triangles == null ? TriangleIntersection(ray, renderer.cubeTriangles, renderer.cubeVertices, world) : TriangleIntersection(ray);
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

        public virtual float GetCameraDepth(Vector3 cameraPosition, Vector3 cameraForward)
        {
            Vector3 center = (boundingBox.Minimum + boundingBox.Maximum) * 0.5f;
            return Vector3.Dot(center - cameraPosition, cameraForward);
        }

        public void ApplyBakeRotation()
        {
            var newValue = ArchiveModelTransformer.ApplyBakeRotation(_model, _yaw, _pitch, _roll);
            if (newValue != _model)
            {
                _model = newValue;
                _yaw = 0;
                _pitch = 0;
                _roll = 0;
            }
        }

        public void ApplyBakeScale()
        {
            var newValue = ArchiveModelTransformer.ApplyBakeScale(assetName, _model, _scale);
            if (newValue != _model)
            {
                _model = newValue;
                _scale = new Vector3(1f, 1f, 1f);
            }
        }

        public virtual void CopyTransformation()
        {
            var transformation = new Transformation()
            {
                _positionX = _position.X,
                _positionY = _position.Y,
                _positionZ = _position.Z,
                _yaw = _yaw,
                _pitch = _pitch,
                _roll = _roll,
                _scaleX = _scale.X,
                _scaleY = _scale.Y,
                _scaleZ = _scale.Z,
            };
            Clipboard.SetText(JsonConvert.SerializeObject(transformation));
        }

        public virtual void PasteTransformation()
        {
            try
            {
                var transformation = JsonConvert.DeserializeObject<Transformation>(Clipboard.GetText());
                _position.X = transformation._positionX;
                _position.Y = transformation._positionY;
                _position.Z = transformation._positionZ;
                _yaw = transformation._yaw;
                _pitch = transformation._pitch;
                _roll = transformation._roll;
                if (assetType != AssetType.Player && assetType != AssetType.Pickup)
                {
                    _scale.X = transformation._scaleX;
                    _scale.Y = transformation._scaleY;
                    _scale.Z = transformation._scaleZ;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"There was an error pasting the transformation from clipboard: ${ex.Message}. Are you sure you have a transformation copied?");
            }
        }

        public static bool movementPreview = false;

        /// <summary>
        /// Advances this entity's movement preview. Called once per frame while movement preview is enabled.
        /// </summary>
        /// <param name="frames">Time since the previous frame, in 60 fps frames</param>
        public virtual void UpdateMovementPreview(double frames)
        {
            localFrameCounter += frames;
        }

        protected (EntityAsset driver, bool useRotation) FindDriverAsset()
        {
            foreach (var link in _links)
            {
                uint PlatID;
                switch ((EventBFBB)link.EventSendID)
                {
                    case EventBFBB.Drivenby:
                        PlatID = link.TargetAsset;
                        break;
                    case EventBFBB.Mount when this is AssetPKUP:
                        PlatID = link.ArgumentAsset;
                        break;
                    default:
                        continue;
                }
                if (PlatID != 0)
                {
                    var ae = Program.ArchiveManager.ArchiveEditorContainingAsset(PlatID);
                    if (ae != null && ae.archive.GetAsset(PlatID) is EntityAsset entity)
                    {
                        var useRotation = link.FloatParameter1 != 0 || (EventBFBB)link.EventSendID == EventBFBB.Mount;
                        return (entity, useRotation);
                    }
                }
            }
            return (null, false);
        }

        public virtual Matrix PlatLocalRotation()
        {
            return Matrix.Identity;
        }

        protected EntityAsset driver = null;
        protected bool driverUseRotation;

        public virtual Matrix LocalWorld()
        {
            if (movementPreview)
            {
                if (driver != null)
                {
                    return Matrix.Scaling(_scale)
                        * Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll)
                        * Matrix.Translation(_position - driver._position)
                        * (driverUseRotation ? driver.PlatLocalRotation() : Matrix.Identity)
                        * Matrix.Translation(Vector3.TransformCoordinate(Vector3.Zero, driver.LocalWorld()));
                }
            }

            return world;
        }

        public virtual void Reset()
        {
            localFrameCounter = -1;
            FindSurf();
            (driver, driverUseRotation) = FindDriverAsset();
        }

        protected Vector3 uvTransSpeed;
        /// <summary>
        /// Time since the movement preview started, in 60 fps frames
        /// </summary>
        protected double localFrameCounter = -1;

        private void FindSurf()
        {
            if (Surface != 0)
            {
                var ae = Program.ArchiveManager.ArchiveEditorContainingAsset(Surface);
                if (ae != null && ae.archive.GetAsset(Surface) is AssetSURF SURF)
                {
                    uvTransSpeed = new Vector3(SURF.zSurfUVFX.TransSpeed_X, SURF.zSurfUVFX.TransSpeed_Y, SURF.zSurfUVFX.TransSpeed_Z);
                    return;
                }
            }
            uvTransSpeed = Vector3.Zero;
        }

        [Browsable(false)]
        public Vector3 UvAnimOffset => movementPreview ? uvTransSpeed * (float)localFrameCounter / 60f : Vector3.Zero;
    }
}