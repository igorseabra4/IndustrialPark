using HipHopFile;
using IndustrialPark.Models;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetSFX : BaseAsset, IRenderableAsset, IClickableAsset, IScalableAsset
    {
        private const string categoryName = "Sound Effect";

        [Category(categoryName)]
        public FlagBitmask Flags08 { get; set; } = ByteFlagsDescriptor();
        [Category(categoryName)]
        public FlagBitmask Flags09 { get; set; } = ByteFlagsDescriptor();
        [Category(categoryName)]
        public short Frequency { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float MinFrequency { get; set; }
        [Category(categoryName)]
        public AssetID Sound_AssetID { get; set; }
        [Category(categoryName)]
        public AssetID AttachAssetID { get; set; }
        [Category(categoryName)]
        public byte LoopCount { get; set; }
        [Category(categoryName)]
        public byte Priority { get; set; }
        [Category(categoryName)]
        public byte Volume { get; set; }
        private Vector3 _position;
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
        private float _radius;
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float InnerRadius
        {
            get => _radius;
            set { _radius = value; CreateTransformMatrix(); }
        }
        private float _radius2;
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float OuterRadius
        {
            get => _radius2;
            set { _radius2 = value; CreateTransformMatrix(); }
        }

        public AssetSFX(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = baseEndPosition;

            Flags08.FlagValueByte = reader.ReadByte();
            Flags09.FlagValueByte = reader.ReadByte();
            Frequency = reader.ReadInt16();
            MinFrequency = reader.ReadSingle();
            Sound_AssetID = reader.ReadUInt32();
            AttachAssetID = reader.ReadUInt32();
            LoopCount = reader.ReadByte();
            Priority = reader.ReadByte();
            Volume = reader.ReadByte();
            reader.ReadByte();
            _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            _radius = reader.ReadSingle();
            _radius2 = reader.ReadSingle();
            
            CreateTransformMatrix();
            ArchiveEditorFunctions.renderableAssets.Add(this);
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(SerializeBase(platform));

            writer.Write(Flags08.FlagValueByte);
            writer.Write(Flags09.FlagValueByte);
            writer.Write(Frequency);
            writer.Write(MinFrequency);
            writer.Write(Sound_AssetID);
            writer.Write(AttachAssetID);
            writer.Write(LoopCount);
            writer.Write(Priority);
            writer.Write(Volume);
            writer.Write((byte)0);
            writer.Write(_position.X);
            writer.Write(_position.Y);
            writer.Write(_position.Z);
            writer.Write(_radius);
            writer.Write(_radius2);

            writer.Write(SerializeLinks(platform));
            return writer.ToArray();
        }

        public override bool HasReference(uint assetID) => Sound_AssetID == assetID || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            if (Sound_AssetID == 0)
                result.Add("SFX with Sound_AssetID set to 0");
            Verify(Sound_AssetID, ref result);
        }

        private Matrix world;
        private Matrix world2;
        private BoundingBox boundingBox;

        public static bool dontRender = false;

        [Browsable(false)]
        public bool SpecialBlendMode => true;

        public BoundingSphere boundingSphere;

        public void CreateTransformMatrix()
        {
            world = Matrix.Scaling(_radius * 2f) * Matrix.Translation(_position);
            world2 = Matrix.Scaling(_radius2 * 2f) * Matrix.Translation(_position);

            CreateBoundingBox();
        }

        protected void CreateBoundingBox()
        {
            boundingSphere = new BoundingSphere(_position, _radius);
            boundingBox = BoundingBox.FromSphere(boundingSphere);
        }

        public float? GetIntersectionPosition(SharpRenderer renderer, Ray ray)
        {
            if (!ShouldDraw(renderer))
                return null;

            if (ray.Intersects(ref boundingSphere))
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

            if (AssetMODL.renderBasedOnLodt)
            {
                if (GetDistanceFrom(renderer.Camera.Position) < SharpRenderer.DefaultLODTDistance)
                    return renderer.frustum.Intersects(ref boundingBox);
                return false;
            }

            return renderer.frustum.Intersects(ref boundingBox);
        }

        public void Draw(SharpRenderer renderer)
        {
            renderer.DrawSphere(world, isSelected, renderer.sfxColor);

            if (isSelected)
                renderer.DrawSphere(world2, false, renderer.sfxColor);
        }

        public BoundingBox GetBoundingBox() => boundingBox;
        
        public float GetDistanceFrom(Vector3 cameraPosition) => Vector3.Distance(cameraPosition, _position) - _radius;
        
        [Browsable(false)]
        public float ScaleX
        {
            get => InnerRadius;
            set
            {
                OuterRadius += value - InnerRadius;
                InnerRadius = value;
            }
        }
        [Browsable(false)]
        public float ScaleY
        {
            get => InnerRadius;
            set
            {
                OuterRadius += value - InnerRadius;
                InnerRadius = value;
            }
        }
        [Browsable(false)]
        public float ScaleZ
        {
            get => InnerRadius;
            set
            {
                OuterRadius += value - InnerRadius;
                InnerRadius = value;
            }
        }
    }
}