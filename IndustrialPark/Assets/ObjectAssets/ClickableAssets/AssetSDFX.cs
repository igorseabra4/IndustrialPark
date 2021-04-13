using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;
using IndustrialPark.Models;

namespace IndustrialPark
{
    public class AssetSDFX : BaseAsset, IRenderableAsset, IClickableAsset
    {
        private const string categoryName = "Sound Effect";

        private uint _soundGroup_AssetID;
        [Category(categoryName)]
        public AssetID SoundGroup_AssetID
        {
            get => _soundGroup_AssetID;
            set { _soundGroup_AssetID = value; CreateTransformMatrix(); }
        }

        [Category(categoryName)]
        public AssetID Emitter_AssetID { get; set; }
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

        [Category(categoryName)]
        public FlagBitmask SoundEffectFlags { get; set; } = IntFlagsDescriptor(
            null,
            null,
            "Play from Entity");

        public AssetSDFX(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = baseEndPosition;

            _soundGroup_AssetID = reader.ReadUInt32();
            Emitter_AssetID = reader.ReadUInt32();
            _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            SoundEffectFlags.FlagValueInt = reader.ReadUInt32();

            CreateTransformMatrix();
            ArchiveEditorFunctions.renderableAssets.Add(this);
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(SerializeBase(platform));

            writer.Write(_soundGroup_AssetID);
            writer.Write(Emitter_AssetID);
            writer.Write(_position.X);
            writer.Write(_position.Y);
            writer.Write(_position.Z);
            writer.Write(SoundEffectFlags.FlagValueInt);

            writer.Write(SerializeLinks(platform));
            return writer.ToArray();
        }

        private Matrix world;
        private Matrix world2;
        private BoundingBox boundingBox;

        public static bool dontRender = false;

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

        public BoundingBox GetBoundingBox() => boundingBox;
        
        public float GetDistanceFrom(Vector3 cameraPosition) => Vector3.Distance(cameraPosition, _position) - _radius;
        
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
                var sg = soundGroup;
                if (sg != null)
                    return sg.InnerRadius;
                return 1f;
            }
        }

        private float _radius2
        {
            get
            {
                var sg = soundGroup;
                if (sg != null)
                    return sg.OuterRadius;
                return 1f;
            }
        }
    }
}