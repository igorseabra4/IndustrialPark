using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class AssetUI : EntityAsset
    {
        private const string categoryName = "User Interface";

        public override string AssetInfo
        {
            get
            {
                if (!(this is AssetUIFT) && Texture == 0)
                    return base.AssetInfo;
                return HexUIntTypeConverter.StringFromAssetID(Texture);
            }
        }

        [Category("Entity References")]
        public AssetID Sound
        {
            get => Animation;
            set => Animation = value;
        }

        [Category(categoryName)]
        public FlagBitmask UIFlags { get; set; } = IntFlagsDescriptor(
            "Focus sets Select",
            null,
            "Screen Space",
            null,
            "Visible Allowed",
            "Invisible Allowed");

        private short _width;
        [Category(categoryName)]
        public short Width
        {
            get => _width;
            set
            {
                _width = value;
                CreateTransformMatrix();
            }
        }

        private short _height;
        [Category(categoryName)]
        public short Height
        {
            get => _height;
            set
            {
                _height = value;
                CreateTransformMatrix();
            }
        }

        [Category(categoryName)]
        public AssetID Texture { get; set; }
        [Category(categoryName)]
        public AssetSingle TextCoordTopLeftX { get; set; }
        [Category(categoryName)]
        public AssetSingle TextCoordTopLeftY { get; set; }
        [Category(categoryName)]
        public AssetSingle TextCoordTopRightX { get; set; }
        [Category(categoryName)]
        public AssetSingle TextCoordTopRightY { get; set; }
        [Category(categoryName)]
        public AssetSingle TextCoordBottomRightX { get; set; }
        [Category(categoryName)]
        public AssetSingle TextCoordBottomRightY { get; set; }
        [Category(categoryName)]
        public AssetSingle TextCoordBottomLeftX { get; set; }
        [Category(categoryName)]
        public AssetSingle TextCoordBottomLeftY { get; set; }

        public AssetUI(string assetName, Vector3 position) : this(assetName, AssetType.UserInterface, BaseAssetType.UI, position)
        {
        }

        public AssetUI(string assetName, AssetType assetType, BaseAssetType baseAssetType, Vector3 position) :
            base(assetName, assetType, baseAssetType, position)
        {
            UIFlags.FlagValueInt = 0x34;
            TextCoordTopRightX = 1f;
            TextCoordBottomRightX = 1f;
            TextCoordBottomRightY = 1f;
            TextCoordBottomLeftY = 1f;
        }

        public AssetUI(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = entityHeaderEndPosition;

                UIFlags.FlagValueInt = reader.ReadUInt32();
                _width = reader.ReadInt16();
                Height = reader.ReadInt16();

                Texture = reader.ReadUInt32();
                TextCoordTopLeftX = reader.ReadSingle();
                TextCoordTopLeftY = reader.ReadSingle();
                TextCoordTopRightX = reader.ReadSingle();
                TextCoordTopRightY = reader.ReadSingle();
                TextCoordBottomRightX = reader.ReadSingle();
                TextCoordBottomRightY = reader.ReadSingle();
                TextCoordBottomLeftX = reader.ReadSingle();
                TextCoordBottomLeftY = reader.ReadSingle();
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeEntity(game, endianness));
                writer.Write(SerializeUIData(endianness));
                writer.Write(SerializeLinks(endianness));
                return writer.ToArray();
            }
        }

        protected byte[] SerializeUIData(Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(UIFlags.FlagValueInt);
                writer.Write(Width);
                writer.Write(Height);
                writer.Write(Texture);
                writer.Write(TextCoordTopLeftX);
                writer.Write(TextCoordTopLeftY);
                writer.Write(TextCoordTopRightX);
                writer.Write(TextCoordTopRightY);
                writer.Write(TextCoordBottomRightX);
                writer.Write(TextCoordBottomRightY);
                writer.Write(TextCoordBottomLeftX);
                writer.Write(TextCoordBottomLeftY);

                return writer.ToArray();
            }
        }

        public static bool dontRender = false;

        public override bool DontRender => dontRender;

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Verify(Texture, ref result);
        }

        public override void CreateTransformMatrix()
        {
            world = renderingDictionary.ContainsKey(_model) ? renderingDictionary[_model].TransformMatrix : Matrix.Identity;

            if (!(this is AssetUIFT) && Texture == 0)
            {
                world *= Matrix.Scaling(_scale) * Matrix.Scaling(Width, Height, 1f)
                    * Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll)
                    * Matrix.Translation(PositionX, -PositionY, -PositionZ);
            }
            else
            {
                world *= Matrix.Scaling(Width, Height, 1f)
                    * Matrix.Translation(PositionX, -PositionY, -PositionZ);
            }

            CreateBoundingBox();
        }

        protected override void CreateBoundingBox()
        {
            if (!(this is AssetUIFT) && Texture == 0)
                base.CreateBoundingBox();
            else
            {
                var model = GetFromRenderingDictionary(_model);
                CreateBoundingBox(model != null ? model.vertexListG : SharpRenderer.planeVertices);
            }
        }

        public override void Draw(SharpRenderer renderer)
        {
            if (!(this is AssetUIFT) && Texture == 0)
                base.Draw(renderer);
            else
                renderer.DrawPlane(world, isSelected, Texture, UvAnimOffset);
        }

        public override bool ShouldDraw(SharpRenderer renderer)
        {
            if (isSelected)
                return true;
            if (DontRender)
                return false;
            if (isInvisible)
                return false;
            if (renderer.isDrawingUI)
                return true;

            return renderer.frustum.Intersects(ref boundingBox);
        }

        [Browsable(false)]
        public override bool SpecialBlendMode => true;

        protected override float? TriangleIntersection(Ray r)
        {
            if (Texture == 0)
                return base.TriangleIntersection(r);

            bool hasIntersected = false;
            float smallestDistance = 1000f;

            foreach (var t in SharpRenderer.planeTriangles)
            {
                var v1 = (Vector3)Vector3.Transform(SharpRenderer.planeVertices[t.vertex1], world);
                var v2 = (Vector3)Vector3.Transform(SharpRenderer.planeVertices[t.vertex2], world);
                var v3 = (Vector3)Vector3.Transform(SharpRenderer.planeVertices[t.vertex3], world);

                if (r.Intersects(ref v1, ref v2, ref v3, out float distance))
                {
                    hasIntersected = true;
                    if (distance < smallestDistance)
                        smallestDistance = distance;
                }
            }

            if (hasIntersected)
                return smallestDistance;
            return null;
        }
    }
}