using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;
using IndustrialPark.Models;

namespace IndustrialPark
{
    public class AssetSDFX : BaseAsset, IRenderableAsset, IClickableAsset
    {
        private Matrix world;
        private Matrix world2;
        private BoundingBox boundingBox;

        public static bool dontRender = false;

        protected override int EventStartOffset => 0x20;

        public AssetSDFX(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            _position = new Vector3(ReadFloat(0x10), ReadFloat(0x14), ReadFloat(0x18));
            CreateTransformMatrix();
            ArchiveEditorFunctions.renderableAssets.Add(this);
        }
        
        public override bool HasReference(uint assetID) =>
            SoundGroup_AssetID == assetID ||
            Emitter_AssetID == assetID ||
            base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            if (SoundGroup_AssetID == 0)
                result.Add("SDFX with SoundGroup_AssetID set to 0");
            Verify(SoundGroup_AssetID, ref result);
            Verify(Emitter_AssetID, ref result);
        }

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

        [Browsable(false)]
        public bool SpecialBlendMode => true;

        public BoundingBox GetBoundingBox()
        {
            return boundingBox;
        }

        public float GetDistanceFrom(Vector3 cameraPosition)
        {
            return Vector3.Distance(cameraPosition, _position) - _radius;
        }
        
        private AssetSGRP soundGroup
        {
            get
            {
                if (Program.MainForm != null)
                    foreach (var ae in Program.MainForm.archiveEditors)
                        if (ae.archive.ContainsAsset(SoundGroup_AssetID))
                            if (ae.archive.GetFromAssetID(SoundGroup_AssetID) is AssetSGRP sgrp)
                                return sgrp;
                return null;
            }
        }

        private float _radius
        {
            get
            {
                if (soundGroup != null)
                    return soundGroup.InnerRadius;
                return 1f;
            }
        }

        private float _radius2
        {
            get
            {
                if (soundGroup != null)
                    return soundGroup.OuterRadius;
                return 1f;
            }
        }

        private const string categoryName = "Sound Effect";

        [Category(categoryName)]
        public AssetID SoundGroup_AssetID
        {
            get => ReadUInt(0x8);
            set { Write(0x8, value); CreateTransformMatrix(); }
        }

        [Category(categoryName)]
        public AssetID Emitter_AssetID
        {
            get => ReadUInt(0xC);
            set => Write(0xC, value);
        }

        private Vector3 _position;

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float PositionX
        {
            get { return _position.X; }
            set
            {
                _position.X = value;
                Write(0x10, _position.X);
                CreateTransformMatrix();
            }
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float PositionY
        {
            get { return _position.Y; }
            set
            {
                _position.Y = value;
                Write(0x14, _position.Y);
                CreateTransformMatrix();
            }
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float PositionZ
        {
            get { return _position.Z; }
            set
            {
                _position.Z = value;
                Write(0x18, _position.Z);
                CreateTransformMatrix();
            }
        }

        [Category(categoryName), Description("0 = Normal\n4 = Play from Entity")]
        public DynamicTypeDescriptor SoundEffectFlags => IntFlagsDescriptor(0x1C, null, null, "Play from Entity");
    }
}