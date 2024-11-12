using HipHopFile;
using Newtonsoft.Json;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public abstract class DynaEnemy : AssetDYNA, IRenderableAsset, IClickableAsset, IRotatableAsset, IScalableAsset, IAssetCopyPasteTransformation
    {
        private const string dynaCategoryName = "Enemy";

        [Category(dynaCategoryName + " Base")]
        public AssetID PseudoAssetID { get; set; }

        [Category(dynaCategoryName + " Base")]
        public AssetByte PseudoAssetType { get; set; }

        [Category(dynaCategoryName + " Base")]
        public AssetByte PseudoLinkCount { get; set; }

        [Category(dynaCategoryName + " Base")]
        public FlagBitmask PseudoBaseFlags { get; set; } = ShortFlagsDescriptor(
                "Enabled On Start",
                "State Is Persistent",
                "Unknown Always True",
                "Visible During Cutscenes",
                "Receive Shadows");

        [Category(dynaCategoryName + " Entity")]
        public FlagBitmask PseudoVisibilityFlags { get; set; } = ByteFlagsDescriptor(
            "Visible",
            "Stackable");

        [Category(dynaCategoryName + " Entity")]
        public AssetByte PseudoTypeFlag { get; set; }

        [Category(dynaCategoryName + " Entity")]
        public FlagBitmask PseudoFlag0A { get; set; } = ByteFlagsDescriptor();

        [Category(dynaCategoryName + " Entity")]
        public FlagBitmask PseudoSolidityFlags { get; set; } = ByteFlagsDescriptor(
            null,
            "Precise Collision",
            null,
            null,
            "Hittable",
            "Animate Collision",
            null,
            "Ledge Grab");

        protected Vector3 _position;
        [Category(dynaCategoryName + " Entity Placement")]
        public AssetSingle PositionX
        {
            get => _position.X;
            set { _position.X = value; CreateTransformMatrix(); }
        }
        [Category(dynaCategoryName + " Entity Placement")]
        public AssetSingle PositionY
        {
            get => _position.Y;
            set { _position.Y = value; CreateTransformMatrix(); }
        }
        [Category(dynaCategoryName + " Entity Placement")]
        public AssetSingle PositionZ
        {
            get => _position.Z;
            set { _position.Z = value; CreateTransformMatrix(); }
        }

        protected float _yaw;
        [Category(dynaCategoryName + " Entity Placement")]
        public AssetSingle Yaw
        {
            get => MathUtil.RadiansToDegrees(_yaw);
            set { _yaw = MathUtil.DegreesToRadians(value); CreateTransformMatrix(); }
        }

        protected float _pitch;
        [Category(dynaCategoryName + " Entity Placement")]
        public AssetSingle Pitch
        {
            get => MathUtil.RadiansToDegrees(_pitch);
            set { _pitch = MathUtil.DegreesToRadians(value); CreateTransformMatrix(); }
        }

        protected float _roll;
        [Category(dynaCategoryName + " Entity Placement")]
        public AssetSingle Roll
        {
            get => MathUtil.RadiansToDegrees(_roll);
            set { _roll = MathUtil.DegreesToRadians(value); CreateTransformMatrix(); }
        }

        protected Vector3 _scale;
        [Category(dynaCategoryName + " Entity Placement")]
        public AssetSingle ScaleX
        {
            get => _scale.X;
            set { _scale.X = value; CreateTransformMatrix(); }
        }
        [Category(dynaCategoryName + " Entity Placement")]
        public AssetSingle ScaleY
        {
            get => _scale.Y;
            set { _scale.Y = value; CreateTransformMatrix(); }
        }
        [Category(dynaCategoryName + " Entity Placement")]
        public AssetSingle ScaleZ
        {
            get => _scale.Z;
            set { _scale.Z = value; CreateTransformMatrix(); }
        }

        protected Vector4 _color;
        [Category(dynaCategoryName + " Entity Color"), DisplayName("Red (0 - 1)")]
        public AssetSingle ColorRed
        {
            get => _color.X;
            set => _color.X = value;
        }
        [Category(dynaCategoryName + " Entity Color"), DisplayName("Green (0 - 1)")]
        public AssetSingle ColorGreen
        {
            get => _color.Y;
            set => _color.Y = value;
        }
        [Category(dynaCategoryName + " Entity Color"), DisplayName("Blue (0 - 1)")]
        public AssetSingle ColorBlue
        {
            get => _color.Z;
            set => _color.Z = value;
        }
        [Category(dynaCategoryName + " Entity Color"), DisplayName("Alpha (0 - 1)")]
        public AssetSingle ColorAlpha
        {
            get => _color.W;
            set => _color.W = value;
        }
        [Category(dynaCategoryName + " Entity Color"), DisplayName("Color - (A,) R, G, B")]
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

        [Category(dynaCategoryName + " Entity Color")]
        public AssetSingle PseudoColorAlphaSpeed { get; set; }

        protected uint _model;
        [Category(dynaCategoryName + " Entity References"), ValidReferenceRequired]
        public AssetID Model
        {
            get => _model;
            set { _model = value; CreateTransformMatrix(); }
        }

        [Category(dynaCategoryName + " Entity References")]
        public AssetID Animation { get; set; }

        [Category(dynaCategoryName + " Entity References")]
        public AssetID Surface { get; set; }

        protected int entityDynaEndPosition => dynaDataStartPosition + 0x50;

        public DynaEnemy(string assetName, DynaType type, Vector3 position) : base(assetName, type)
        {
            PseudoBaseFlags.FlagValueShort = 0x1D;
            PseudoVisibilityFlags.FlagValueByte = 1;
            PseudoSolidityFlags.FlagValueByte = 1;

            _position = position;
            _scale = new Vector3(1f);
            _color = new Vector4(1f);

            CreateTransformMatrix();
            AddToRenderableAssets(this);
        }

        public DynaEnemy(Section_AHDR AHDR, DynaType type, Game game, Endianness endianness) : base(AHDR, type, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                PseudoAssetID = reader.ReadUInt32();
                PseudoAssetType = reader.ReadByte();
                PseudoLinkCount = reader.ReadByte();
                PseudoBaseFlags.FlagValueShort = reader.ReadUInt16();
                PseudoVisibilityFlags.FlagValueByte = reader.ReadByte();
                PseudoTypeFlag = reader.ReadByte();
                PseudoFlag0A.FlagValueByte = reader.ReadByte();
                PseudoSolidityFlags.FlagValueByte = reader.ReadByte();
                Surface = reader.ReadUInt32();
                _yaw = reader.ReadSingle();
                _pitch = reader.ReadSingle();
                _roll = reader.ReadSingle();
                _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                _scale = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                _color = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                PseudoColorAlphaSpeed = reader.ReadSingle();
                _model = reader.ReadUInt32();
                Animation = reader.ReadUInt32();

                CreateTransformMatrix();
                AddToRenderableAssets(this);
            }
        }

        protected void SerializeEntityDyna(EndianBinaryWriter writer)
        {
            writer.Write(PseudoAssetID);
            writer.Write(PseudoAssetType);
            writer.Write(PseudoLinkCount);
            writer.Write(PseudoBaseFlags.FlagValueShort);
            writer.Write(PseudoVisibilityFlags.FlagValueByte);
            writer.Write(PseudoTypeFlag);
            writer.Write(PseudoFlag0A.FlagValueByte);
            writer.Write(PseudoSolidityFlags.FlagValueByte);
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
            writer.Write(PseudoColorAlphaSpeed);
            writer.Write(_model);
            writer.Write(Animation);
        }

        [Browsable(false)]
        public Matrix world { get; private set; }
        private BoundingBox boundingBox;

        public void CreateTransformMatrix()
        {
            world = (renderingDictionary.ContainsKey(_model) ? renderingDictionary[_model].TransformMatrix : Matrix.Identity)
                * Matrix.Scaling(_scale)
                * Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll)
                * Matrix.Translation(_position);

            CreateBoundingBox();
        }

        protected void CreateBoundingBox()
        {
            var model = GetFromRenderingDictionary(Model);
            if (model != null)
            {
                triangles = model.triangleList.ToArray();
                CreateBoundingBox(model.vertexListG);
            }
            else
            {
                triangles = null;
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
        }

        [Browsable(false)]
        public abstract bool DontRender { get; }

        public bool ShouldDraw(SharpRenderer renderer)
        {
            if (isSelected)
                return true;
            if (DontRender)
                return false;
            if (isInvisible)
                return false;
            if (AssetMODL.renderBasedOnLodt && GetDistanceFrom(renderer.Camera.Position) > AssetLODT.MaxDistanceTo(_model))
                return false;

            return renderer.frustum.Intersects(ref boundingBox);
        }

        [Browsable(false)]
        public bool SpecialBlendMode => !renderingDictionary.ContainsKey(_model) || renderingDictionary[_model].SpecialBlendMode;

        public void Draw(SharpRenderer renderer)
        {
            Vector4 Color = new Vector4(ColorRed, ColorGreen, ColorBlue, ColorAlpha);
            if (renderingDictionary.ContainsKey(_model))
                renderingDictionary[_model].Draw(renderer, world, Color, Vector3.Zero, isSelected);
            else
                renderer.DrawCube(world, isSelected);
        }

        public float? GetIntersectionPosition(SharpRenderer renderer, Ray ray)
        {
            if (ShouldDraw(renderer) && ray.Intersects(ref boundingBox))
                return triangles == null ? TriangleIntersection(ray, SharpRenderer.cubeTriangles, SharpRenderer.cubeVertices, world) : TriangleIntersection(ray);
            return null;
        }

        private float? TriangleIntersection(Ray ray)
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

        public float GetDistanceFrom(Vector3 cameraPosition)
        {
            return Vector3.Distance(cameraPosition, new Vector3(PositionX, PositionY, PositionZ));
        }

        public void CopyTransformation()
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

        public void PasteTransformation()
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
                _scale.X = transformation._scaleX;
                _scale.Y = transformation._scaleY;
                _scale.Z = transformation._scaleZ;
                CreateTransformMatrix();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"There was an error pasting the transformation from clipboard: ${ex.Message}. Are you sure you have a transformation copied?");
            }
        }
    }
}