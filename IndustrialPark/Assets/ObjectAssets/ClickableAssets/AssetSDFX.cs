using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetSDFX : BaseAsset, IRenderableAsset, IClickableAsset
    {
        private const string categoryName = "SDFX";
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(SoundGroup);

        private uint _soundGroup;
        [Category(categoryName), ValidReferenceRequired]
        public AssetID SoundGroup
        {
            get => _soundGroup;
            set { _soundGroup = value; CreateTransformMatrix(); }
        }

        [Category(categoryName)]
        public AssetID Emitter { get; set; }
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
        public FlagBitmask SDFXFlags { get; set; } = IntFlagsDescriptor(
            null,
            null,
            "Play from Entity");

        public AssetSDFX(string assetName, Vector3 position) : base(assetName, AssetType.SDFX, BaseAssetType.SDFX)
        {
            _position = position;

            CreateTransformMatrix();
            ArchiveEditorFunctions.AddToRenderableAssets(this);
        }

        public AssetSDFX(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                _soundGroup = reader.ReadUInt32();
                Emitter = reader.ReadUInt32();
                _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                SDFXFlags.FlagValueInt = reader.ReadUInt32();

                CreateTransformMatrix();
                ArchiveEditorFunctions.AddToRenderableAssets(this);
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeBase(endianness));
                writer.Write(_soundGroup);
                writer.Write(Emitter);
                writer.Write(_position.X);
                writer.Write(_position.Y);
                writer.Write(_position.Z);
                writer.Write(SDFXFlags.FlagValueInt);
                writer.Write(SerializeLinks(endianness));
                return writer.ToArray();
            }
        }

        private Matrix world;
        private Matrix world2;
        private BoundingBox boundingBox;

        public static bool dontRender = false;

        public void CreateTransformMatrix()
        {
            world = Matrix.Scaling(_radius * 2f) * Matrix.Translation(_position);
            world2 = Matrix.Scaling(_radius2 * 2f) * Matrix.Translation(_position);

            CreateBoundingBox();
        }

        public BoundingSphere boundingSphere;

        protected void CreateBoundingBox()
        {
            boundingSphere = new BoundingSphere(_position, _radius);
            boundingBox = BoundingBox.FromSphere(boundingSphere);
        }

        public float? GetIntersectionPosition(SharpRenderer renderer, Ray ray)
        {
            if (ShouldDraw(renderer) && ray.Intersects(ref boundingSphere))
                return TriangleIntersection(ray, SharpRenderer.sphereTriangles, SharpRenderer.sphereVertices, world);
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
            renderer.DrawSphere(world, isSelected, renderer.sfxColor);

            if (isSelected)
                renderer.DrawSphere(world2, false, renderer.sfxColor);
        }

        [Browsable(false)]
        public bool SpecialBlendMode => true;

        public BoundingBox GetBoundingBox() => boundingBox;

        public float GetDistanceFrom(Vector3 cameraPosition) => Vector3.Distance(cameraPosition, _position) - _radius;

        private AssetSGRP soundGroup
        {
            get
            {
                if (Program.MainForm != null)
                    foreach (var ae in Program.MainForm.archiveEditors)
                        if (ae.archive.ContainsAsset(SoundGroup))
                            if (ae.archive.GetFromAssetID(SoundGroup) is AssetSGRP sgrp)
                                return sgrp;
                return null;
            }
        }

        private float _radius
        {
            get
            {
                var sg = soundGroup;
                return sg == null ? 1f : (float)sg.InnerRadius;
            }
        }

        private float _radius2
        {
            get
            {
                var sg = soundGroup;
                return sg == null ? 1f : (float)sg.OuterRadius;
            }
        }
    }
}