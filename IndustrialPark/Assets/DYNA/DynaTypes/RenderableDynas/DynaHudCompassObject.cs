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
    public class DynaHudCompassObject : RenderableDynaBase
    {
        private const string dynaCategoryName = "HUD Compass Object";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(ObjectId);
        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetID ObjectId { get; set; }
        [Category(dynaCategoryName)]
        public AssetID TextureId { get; set; }
        [Category(dynaCategoryName)]
        public AssetID CompassSystemID { get; set; }

        public DynaHudCompassObject(string assetName, Vector3 position) : base(assetName, DynaType.HUD_Compass_Object, position)
        {
            CreateTransformMatrix();
            AddToRenderableAssets(this);
        }
        public DynaHudCompassObject(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.HUD_Compass_Object, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                ObjectId = reader.ReadUInt32();
                _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                TextureId = reader.ReadUInt32();
                CompassSystemID = reader.ReadUInt32();

                CreateTransformMatrix();
                AddToRenderableAssets(this);
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(ObjectId);
            writer.Write(_position.X);
            writer.Write(_position.Y);
            writer.Write(_position.Z);
            writer.Write(TextureId);
            writer.Write(CompassSystemID);
        }

        protected override List<Vector3> vertexSource => SharpRenderer.cubeVertices;

        protected override List<Triangle> triangleSource => SharpRenderer.cubeTriangles;

        public override void Draw(SharpRenderer renderer)
        {
            renderer.DrawCube(world, isSelected);
        }

        public static bool dontRender = true;
        protected override bool DontRender => dontRender;
    }
}