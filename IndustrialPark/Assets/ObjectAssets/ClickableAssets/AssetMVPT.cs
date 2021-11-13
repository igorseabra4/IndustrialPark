using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetMVPT : BaseAsset, IRenderableAsset, IClickableAsset, IScalableAsset
    {
        private const string categoryName = "Move Point";

        private Vector3 _position;
        [Category(categoryName)]
        public AssetSingle PositionX
        {
            get => _position.X;
            set { _position.X = value; CreateTransformMatrix(); }
        }
        [Category(categoryName)]
        public AssetSingle PositionY
        {
            get => _position.Y;
            set { _position.Y = value; CreateTransformMatrix(); }
        }
        [Category(categoryName)]
        public AssetSingle PositionZ
        {
            get => _position.Z;
            set { _position.Z = value; CreateTransformMatrix(); }
        }

        [Category(categoryName), TypeConverter(typeof(HexUShortTypeConverter)), Description("Usually 0x2710")]
        public ushort Wt { get; set; }
        [Category(categoryName), Description("0x00 for arena (can see you), 0x01 for zone")]
        public AssetByte IsZone { get; set; }
        [Category(categoryName), Description("Usually 0x00")]
        public AssetByte BezIndex { get; set; }
        [Category(categoryName)]
        public AssetByte Flg_Props { get; set; }

        [Category(categoryName), Description("Movement Angle - Enemy will rotate around the point this amount, -1 means disabled")]
        public AssetSingle Delay { get; set; }

        private float _zoneRadius;
        [Category(categoryName), Description("Enemy will circle around the point in this distance, -1 means disabled")]
        public AssetSingle ZoneRadius
        {
            get => _zoneRadius;
            set { _zoneRadius = value; CreateTransformMatrix(); }
        }

        private float _arenaRadius;
        [Category(categoryName), Description("Enemy will be able to see you from this radius (as in a sphere trigger), -1 means disabled")]
        public AssetSingle ArenaRadius
        {
            get => _arenaRadius;
            set { _arenaRadius = value; CreateTransformMatrix(); }
        }

        [Category(categoryName)]
        public AssetID[] NextMVPTs { get; set; }

        public AssetMVPT(string assetName, Vector3 position, Game game, AssetTemplate template) : base(assetName, AssetType.MVPT, BaseAssetType.MovePoint)
        {
            _position = position;
            Wt = 0x2710;
            BezIndex = 0x00;
            NextMVPTs = new AssetID[0];

            switch (template)
            {
                case AssetTemplate.Area_MVPT:
                    IsZone = 0x00;
                    Delay = 360;
                    ZoneRadius = 4;
                    ArenaRadius = 8;
                    break;
                case AssetTemplate.Point_MVPT:
                    IsZone = 0x01;
                    Delay = game == Game.Incredibles ? 2 : 0;
                    ZoneRadius = -1;
                    ArenaRadius = -1;
                    break;
            }

            CreateTransformMatrix();
            ArchiveEditorFunctions.AddToRenderableAssets(this);
        }

        public AssetMVPT(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                Wt = reader.ReadUInt16();
                IsZone = reader.ReadByte();
                BezIndex = reader.ReadByte();
                Flg_Props = reader.ReadByte();
                reader.ReadByte(); // pad
                ushort pointCount = reader.ReadUInt16();

                if (game != Game.Scooby)
                    Delay = reader.ReadSingle();
                _zoneRadius = reader.ReadSingle();
                if (game != Game.Scooby)
                    _arenaRadius = reader.ReadSingle();

                NextMVPTs = new AssetID[pointCount];
                for (int i = 0; i < NextMVPTs.Length; i++)
                    NextMVPTs[i] = reader.ReadUInt32();

                CreateTransformMatrix();
                ArchiveEditorFunctions.AddToRenderableAssets(this);
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeBase(endianness));

                writer.Write(_position.X);
                writer.Write(_position.Y);
                writer.Write(_position.Z);
                writer.Write(Wt);
                writer.Write(IsZone);
                writer.Write(BezIndex);
                writer.Write(Flg_Props);
                writer.Write((byte)0);
                writer.Write((ushort)NextMVPTs.Length);
                if (game != Game.Scooby)
                    writer.Write(Delay);
                writer.Write(_zoneRadius);
                if (game != Game.Scooby)
                    writer.Write(_arenaRadius);
                foreach (var i in NextMVPTs)
                    writer.Write(i);

                writer.Write(SerializeLinks(endianness));
                return writer.ToArray();
            }
        }

        private Matrix world;
        private BoundingBox boundingBox;

        public static bool dontRender = false;

        [Browsable(false)]
        public bool SpecialBlendMode => true;

        public override bool HasReference(uint assetID)
        {
            foreach (AssetID a in NextMVPTs)
                if (a == assetID)
                    return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            foreach (AssetID a in NextMVPTs)
                Verify(a, ref result);
        }

        public void CreateTransformMatrix()
        {
            if (IsZone == 1 || _arenaRadius == -1f)
                world = Matrix.RotationX(MathUtil.PiOverTwo) * Matrix.Translation(_position + new Vector3(0f, 0.5f, 0f));
            else
                world = Matrix.Scaling(_arenaRadius * 2f) * Matrix.Translation(_position);

            CreateBoundingBox();
        }

        public BoundingSphere boundingSphere;

        protected void CreateBoundingBox()
        {
            if (IsZone == 1 || _arenaRadius == -1f)
            {
                var vertices = new Vector3[SharpRenderer.pyramidVertices.Count];

                for (int i = 0; i < SharpRenderer.pyramidVertices.Count; i++)
                    vertices[i] = (Vector3)Vector3.Transform(SharpRenderer.pyramidVertices[i], world);

                boundingBox = BoundingBox.FromPoints(vertices);
                boundingSphere = BoundingSphere.FromBox(boundingBox);
            }
            else
            {
                boundingSphere = new BoundingSphere(_position, _arenaRadius);
                boundingBox = BoundingBox.FromSphere(boundingSphere);
            }
        }

        public float? GetIntersectionPosition(SharpRenderer renderer, Ray ray)
        {
            if (ShouldDraw(renderer))
                if (ray.Intersects(ref boundingSphere))
                {
                    if (IsZone == 1 || _arenaRadius == -1f)
                        return TriangleIntersection(ray, SharpRenderer.pyramidTriangles, SharpRenderer.pyramidVertices, world);
                    return TriangleIntersection(ray, SharpRenderer.sphereTriangles, SharpRenderer.sphereVertices, world);
                }
            return null;
        }

        public bool ShouldDraw(SharpRenderer renderer)
        {
            if (isSelected)
                return true;
            if (dontRender)
                return false;
            if (isInvisible)
                return false;
            if (AssetMODL.renderBasedOnLodt && GetDistanceFrom(renderer.Camera.Position) > SharpRenderer.DefaultLODTDistance)
                return false;

            return renderer.frustum.Intersects(ref boundingBox);
        }

        public void Draw(SharpRenderer renderer)
        {
            if (IsZone == 1 || _arenaRadius == -1f)
                renderer.DrawPyramid(world, isSelected, 1f);
            else
                renderer.DrawSphere(world, isSelected, renderer.mvptColor);
        }

        public BoundingBox GetBoundingBox() => boundingBox;

        public float GetDistanceFrom(Vector3 cameraPosition) => Vector3.Distance(cameraPosition, _position) - (_arenaRadius == -1f ? 0 : _arenaRadius);

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game == Game.Scooby)
            {
                dt.RemoveProperty("Delay");
                dt.RemoveProperty("ArenaRadius");
            }
            base.SetDynamicProperties(dt);
        }

        [Browsable(false)]
        public AssetSingle ScaleX
        {
            get => GetScale();
            set => SetScale(value);
        }
        [Browsable(false)]
        public AssetSingle ScaleY
        {
            get => GetScale();
            set => SetScale(value);
        }
        [Browsable(false)]
        public AssetSingle ScaleZ
        {
            get => GetScale();
            set => SetScale(value);
        }

        private float GetScale()
        {
            if (IsZone == 0x00 && _arenaRadius != -1f)
                return _arenaRadius;

            return 1f;
        }

        private void SetScale(float scale)
        {
            if (IsZone == 0x00)
                ArenaRadius = scale;
        }
    }
}