using HipHopFile;
using IndustrialPark.Models;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public enum DynaSpringBoardType
    {
        Idle = 0,
        Compressing = 1,
        Compressed = 2,
        Launching = 3
    }

    public class DynaCObjectSpringBoard : RenderableRotatableDynaBase
    {
        private const string dynaCategoryName = "Context Object:SpringBoard";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(ModelID);

        protected override short constVersion => 4;

        [Category(dynaCategoryName)]
        public AssetID ModelID { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Adjustment { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte Top { get; set; }
        [Category(dynaCategoryName)]
        public DynaSpringBoardType SpringboardType { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte Size { get; set; }

        public DynaCObjectSpringBoard(string assetName, Vector3 position) : base(assetName, DynaType.ContextObject_Springboard, position)
        {
            CreateTransformMatrix();
            AddToRenderableAssets(this);
        }
        public DynaCObjectSpringBoard(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.ContextObject_Springboard, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                _yaw = reader.ReadSingle();
                _pitch = reader.ReadSingle();
                _roll = reader.ReadSingle();
                ModelID = reader.ReadUInt32();
                Adjustment = reader.ReadSingle();
                Top = reader.ReadByte();
                SpringboardType = (DynaSpringBoardType)reader.ReadByte();
                Size = reader.ReadByte();

                reader.BaseStream.Position += 1;

                CreateTransformMatrix();
                AddToRenderableAssets(this);
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(_position.X);
            writer.Write(_position.Y);
            writer.Write(_position.Z);
            writer.Write(_yaw);
            writer.Write(_pitch);
            writer.Write(_roll);
            writer.Write(ModelID);
            writer.Write(Adjustment);
            writer.Write(Top);
            writer.Write((byte)SpringboardType);
            writer.Write(Size);
            writer.Write((byte)0);
        }
        protected override List<Vector3> vertexSource => SharpRenderer.pyramidVertices;

        protected override List<Triangle> triangleSource => SharpRenderer.pyramidTriangles;

        public override void CreateTransformMatrix()
        {
            world = (renderingDictionary.ContainsKey(ModelID) ?
                renderingDictionary[ModelID].TransformMatrix : Matrix.Identity)
                * Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll)
                * Matrix.Translation(_position);

            CreateBoundingBox();
        }

        protected override void CreateBoundingBox()
        {
            var model = GetFromRenderingDictionary(ModelID);
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

        public override bool ShouldDraw(SharpRenderer renderer)
        {
            if (isSelected)
                return true;
            if (isInvisible)
                return false;
            if (dontRender)
                return false;

            if (AssetMODL.renderBasedOnLodt && GetDistanceFrom(renderer.Camera.Position) > AssetLODT.MaxDistanceTo(ModelID))
                return false;

            return renderer.frustum.Intersects(ref boundingBox);
        }

        public override void Draw(SharpRenderer renderer)
        {

            if (renderingDictionary.ContainsKey(ModelID))
                renderingDictionary[ModelID].Draw(renderer, world, Vector4.One, Vector3.Zero, isSelected);
            else
                renderer.DrawCube(world, isSelected);
        }

        public static bool dontRender = false;
        protected override bool DontRender => dontRender;
    }
}