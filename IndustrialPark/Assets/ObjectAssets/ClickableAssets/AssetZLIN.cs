using HipHopFile;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetZLIN: BaseAsset, IRenderableAsset, IClickableAsset
    {
        private const string categoryName = "Zip Line";
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(Spline);

        [Category(categoryName)]
        public AssetByte DismountType { get; set; }

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

        [Category(categoryName)]
        public AssetSingle HangLength { get; set; }

        [Category(categoryName), ValidReferenceRequired]
        public AssetID Spline { get; set; }

        [Category(categoryName), ValidReferenceRequired]
        public AssetID Entity { get; set; }

        [Category(categoryName)]
        public AssetSingle Speed { get; set; }

        [Category(categoryName)]
        public FlagBitmask ZipLineFlags { get; set; } = IntFlagsDescriptor();

        public AssetZLIN(string assetName, Vector3 position) : base(assetName, AssetType.ZipLine, BaseAssetType.ZipLine)
        {
            _position = position;

            CreateTransformMatrix();
            ArchiveEditorFunctions.AddToRenderableAssets(this);
        }

        public AssetZLIN(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                DismountType = reader.ReadByte();
                reader.BaseStream.Position += 3;
                _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                HangLength = reader.ReadSingle();
                Spline = reader.ReadUInt32();
                Entity = reader.ReadUInt32();
                Speed = reader.ReadSingle();
                ZipLineFlags.FlagValueInt = reader.ReadUInt32();

                CreateTransformMatrix();
                ArchiveEditorFunctions.AddToRenderableAssets(this);
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {

                base.Serialize(writer);

                writer.Write(DismountType);
                writer.Write(new byte[3]);
                writer.Write(_position.X);
                writer.Write(_position.Y);
                writer.Write(_position.Z);
                writer.Write(HangLength);
                writer.Write(Spline);
                writer.Write(Entity);
                writer.Write(Speed);
                writer.Write(ZipLineFlags.FlagValueInt);

                SerializeLinks(writer);
                
        }

        private Matrix world;
        private BoundingBox boundingBox;

        public static bool dontRender = false;

        [Browsable(false)]
        public bool SpecialBlendMode => true;

        public void CreateTransformMatrix()
        {
            world = Matrix.Translation(_position);
            CreateBoundingBox();
        }

        protected void CreateBoundingBox()
        {
            var vertices = new Vector3[SharpRenderer.cubeVertices.Count];

            for (int i = 0; i < SharpRenderer.cubeVertices.Count; i++)
                vertices[i] = (Vector3)Vector3.Transform(SharpRenderer.cubeVertices[i] * 0.5f, world);

            boundingBox = BoundingBox.FromPoints(vertices);
        }

        public float? GetIntersectionPosition(SharpRenderer renderer, Ray ray)
        {
            if (ShouldDraw(renderer) && ray.Intersects(ref boundingBox))
                return TriangleIntersection(ray, SharpRenderer.cubeTriangles, SharpRenderer.cubeVertices, world);
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

        public void Draw(SharpRenderer renderer) => renderer.DrawCube(world, isSelected);

        public BoundingBox GetBoundingBox() => boundingBox;

        public float GetDistanceFrom(Vector3 cameraPosition) => Vector3.Distance(cameraPosition, _position);
    }
}