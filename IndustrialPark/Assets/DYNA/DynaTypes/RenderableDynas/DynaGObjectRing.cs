using HipHopFile;
using IndustrialPark.Models;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class DynaGObjectRing : RenderableRotatableDynaBase
    {
        private const string dynaCategoryName = "game_object:Ring";

        protected override int constVersion => 2;

        [Category(dynaCategoryName)]
        public int UnknownInt1 { get; set; }
        [Category(dynaCategoryName)]
        public int UnknownInt2 { get; set; }
        [Category(dynaCategoryName)]
        public int UnknownInt3 { get; set; }
        protected Vector3 _scale;
        [Category(dynaCategoryName)]
        public AssetSingle ScaleX
        {
            get => _scale.X;
            set { _scale.X = value; CreateTransformMatrix(); }
        }
        [Category(dynaCategoryName)]
        public AssetSingle ScaleY
        {
            get => _scale.Y;
            set { _scale.Y = value; CreateTransformMatrix(); }
        }
        [Category(dynaCategoryName)]
        public AssetSingle ScaleZ
        {
            get => _scale.Z;
            set { _scale.Z = value; CreateTransformMatrix(); }
        }
        [Category(dynaCategoryName)]
        public int UnknownShadowFlag { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle CollisionRadius { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat1 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat2 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle NormalTimer { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle RedTimer { get; set; }
        [Category(dynaCategoryName)]
        public AssetID DriverPLAT_AssetID { get; set; }

        public DynaGObjectRing(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, DynaType.game_object__Ring, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = dynaDataStartPosition;

            _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            _yaw = reader.ReadSingle();
            _pitch = reader.ReadSingle();
            _roll = reader.ReadSingle();
            UnknownInt1 = reader.ReadInt32();
            UnknownInt2 = reader.ReadInt32();
            UnknownInt3 = reader.ReadInt32();
            _scale = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            UnknownShadowFlag = reader.ReadInt32();
            CollisionRadius = reader.ReadSingle();
            UnknownFloat1 = reader.ReadSingle();
            UnknownFloat2 = reader.ReadSingle();
            NormalTimer = reader.ReadSingle();
            RedTimer = reader.ReadSingle();
            DriverPLAT_AssetID = reader.ReadUInt32();

            CreateTransformMatrix();
            renderableAssets.Add(this);
        }

        protected override byte[] SerializeDyna(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            writer.Write(_position.X);
            writer.Write(_position.Y);
            writer.Write(_position.Z);
            writer.Write(_yaw);
            writer.Write(_pitch);
            writer.Write(_roll);
            writer.Write(UnknownInt1);
            writer.Write(UnknownInt2);
            writer.Write(UnknownInt3);
            writer.Write(_scale.X);
            writer.Write(_scale.Y);
            writer.Write(_scale.Z);
            writer.Write(UnknownShadowFlag);
            writer.Write(CollisionRadius);
            writer.Write(UnknownFloat1);
            writer.Write(UnknownFloat2);
            writer.Write(NormalTimer);
            writer.Write(RedTimer);
            writer.Write(DriverPLAT_AssetID);

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID) => DriverPLAT_AssetID == assetID || base.HasReference(assetID);
        
        public override void Verify(ref List<string> result)
        {
            Verify(DriverPLAT_AssetID, ref result);
            base.Verify(ref result);
        }
        
        public override void CreateTransformMatrix()
        {
            world = Matrix.Scaling(_scale)
                * Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll)
                * Matrix.Translation(_position);

            CreateBoundingBox();
        }

        protected override void CreateBoundingBox()
        {
            var model = GetFromRenderingDictionary(DynaGObjectRingControl.RingModelAssetID);
            if (model != null)
                CreateBoundingBox(model.vertexListG, model.triangleList);
            else
                base.CreateBoundingBox();
        }

        private void CreateBoundingBox(List<Vector3> vertexList, List<RenderWareFile.Triangle> triangleList)
        {
            vertices = new Vector3[vertexList.Count];
            for (int i = 0; i < vertexList.Count; i++)
                vertices[i] = (Vector3)Vector3.Transform(vertexList[i], world);

            boundingBox = BoundingBox.FromPoints(vertices);

            triangles = triangleList.ToArray();
        }

        protected override List<Vector3> vertexSource => SharpRenderer.cubeVertices;

        protected override List<Triangle> triangleSource => SharpRenderer.cubeTriangles;

        public override bool ShouldDraw(SharpRenderer renderer)
        {
            if (AssetMODL.renderBasedOnLodt)
            {
                if (GetDistanceFrom(renderer.Camera.Position) < AssetLODT.MaxDistanceTo(DynaGObjectRingControl.RingModelAssetID))
                    return renderer.frustum.Intersects(ref boundingBox);

                return false;
            }

            return renderer.frustum.Intersects(ref boundingBox);
        }

        public override void Draw(SharpRenderer renderer)
        {
            if (renderingDictionary.ContainsKey(DynaGObjectRingControl.RingModelAssetID))
                renderingDictionary[DynaGObjectRingControl.RingModelAssetID].Draw(renderer, world, isSelected ? renderer.selectedObjectColor : Vector4.One, Vector3.Zero);
            else
                renderer.DrawCube(world, isSelected);
        }
    }
}