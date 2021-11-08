using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;
using System;
using SharpDX;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class DynaGObjectTrainCar : AssetDYNA, IRenderableAsset, IClickableAsset, IRotatableAsset, IScalableAsset
    {
        private const string dynaCategoryName = "game_object:train_car";

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

        protected uint _modelAssetID;
        [Category(dynaCategoryName + " Entity References")]
        public AssetID Model_AssetID
        {
            get => _modelAssetID;
            set { _modelAssetID = value; CreateTransformMatrix(); }
        }

        [Category(dynaCategoryName + " Entity References")]
        public AssetID PseudoAnimation_AssetID { get; set; }

        [Category(dynaCategoryName + " Entity References")]
        public AssetID Surface_AssetID { get; set; }

        protected int entityDynaEndPosition => dynaDataStartPosition + 0x50;

        [Category(dynaCategoryName)]
        public AssetID ParentCar_AssetID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID StartSpline_AssetID { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle InitialU { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle front_axle_dist { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle rear_axle_dist { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle front_hitch_dist { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle rear_hitch_dist { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle axle_width { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte have_sparks { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte isdestructible { get; set; }
        [Category(dynaCategoryName)]
        public AssetID NavMesh_AssetID { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte NavMeshGroupIndex { get; set; }

        protected override short constVersion => 7;

        public DynaGObjectTrainCar(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__train_car, game, endianness)
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
                Surface_AssetID = reader.ReadUInt32();
                _yaw = reader.ReadSingle();
                _pitch = reader.ReadSingle();
                _roll = reader.ReadSingle();
                _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                _scale = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                _color = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                PseudoColorAlphaSpeed = reader.ReadSingle();
                _modelAssetID = reader.ReadUInt32();
                PseudoAnimation_AssetID = reader.ReadUInt32();

                ParentCar_AssetID = reader.ReadUInt32();
                StartSpline_AssetID = reader.ReadUInt32();
                InitialU = reader.ReadSingle();
                front_axle_dist = reader.ReadSingle();
                rear_axle_dist = reader.ReadSingle();
                front_hitch_dist = reader.ReadSingle();
                rear_hitch_dist = reader.ReadSingle();
                axle_width = reader.ReadSingle();
                have_sparks = reader.ReadByte();
                isdestructible = reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                NavMesh_AssetID = reader.ReadUInt32();
                NavMeshGroupIndex = reader.ReadByte();

                CreateTransformMatrix();
                AddToRenderableAssets(this);
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(PseudoAssetID);
                writer.Write(PseudoAssetType);
                writer.Write(PseudoLinkCount);
                writer.Write(PseudoBaseFlags.FlagValueShort);
                writer.Write(PseudoVisibilityFlags.FlagValueByte);
                writer.Write(PseudoTypeFlag);
                writer.Write(PseudoFlag0A.FlagValueByte);
                writer.Write(PseudoSolidityFlags.FlagValueByte);
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
                writer.Write(PseudoColorAlphaSpeed);
                writer.Write(_modelAssetID);
                writer.Write(PseudoAnimation_AssetID);

                writer.Write(ParentCar_AssetID);
                writer.Write(StartSpline_AssetID);
                writer.Write(InitialU);
                writer.Write(front_axle_dist);
                writer.Write(rear_axle_dist);
                writer.Write(front_hitch_dist);
                writer.Write(rear_hitch_dist);
                writer.Write(axle_width);
                writer.Write(have_sparks);
                writer.Write(isdestructible);
                writer.Write((short)0);
                writer.Write(NavMesh_AssetID);
                writer.Write(NavMeshGroupIndex);
                writer.Write((byte)0);
                writer.Write((short)0);

                return writer.ToArray();
            }
        }

        [Browsable(false)]
        public Matrix world { get; private set; }
        private BoundingBox boundingBox;

        public void CreateTransformMatrix()
        {
            world = Matrix.Scaling(_scale)
                * Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll)
                * Matrix.Translation(_position);

            CreateBoundingBox();
        }

        protected void CreateBoundingBox()
        {
            var model = GetFromRenderingDictionary(Model_AssetID);
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

        public bool ShouldDraw(SharpRenderer renderer)
        {
            if (isSelected)
                return true;
            if (dontRender)
                return false;
            if (isInvisible)
                return false;
            if (AssetMODL.renderBasedOnLodt && GetDistanceFrom(renderer.Camera.Position) > AssetLODT.MaxDistanceTo(_modelAssetID))
                return false;

            return renderer.frustum.Intersects(ref boundingBox);
        }

        [Browsable(false)]
        public bool SpecialBlendMode => !renderingDictionary.ContainsKey(_modelAssetID) || renderingDictionary[_modelAssetID].SpecialBlendMode;

        public void Draw(SharpRenderer renderer)
        {
            Vector4 Color = new Vector4(ColorRed, ColorGreen, ColorBlue, ColorAlpha);
            if (renderingDictionary.ContainsKey(_modelAssetID))
                renderingDictionary[_modelAssetID].Draw(renderer, world, isSelected ? renderer.selectedObjectColor * Color : Color, Vector3.Zero);
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

        public BoundingSphere GetObjectCenter()
        {
            BoundingSphere boundingSphere = new BoundingSphere(new Vector3(PositionX, PositionY, PositionZ), boundingBox.Size.Length());
            boundingSphere.Radius *= 0.9f;
            return boundingSphere;
        }

        public override bool HasReference(uint assetID)
        {
            if (Surface_AssetID == assetID)
                return true;
            if (Model_AssetID == assetID)
                return true;
            if (PseudoAnimation_AssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            Verify(Surface_AssetID, ref result);
            Verify(Model_AssetID, ref result);
            Verify(PseudoAnimation_AssetID, ref result);

            base.Verify(ref result);
        }
    }
}