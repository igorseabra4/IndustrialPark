using HipHopFile;
using IndustrialPark.Models;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class DynaGObjectTeleport : RenderableDynaBase
    {
        private const string dynaCategoryName = "game_object:Teleport";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => $"{HexUIntTypeConverter.StringFromAssetID(Marker)} {HexUIntTypeConverter.StringFromAssetID(TargetTeleportBox)}";

        public override string Note => "Version is always 1 or 2. CameraAngle is not present in version 1, or in Movie/Incredibles regardless of version.";

        private uint _mrkr;
        [Category(dynaCategoryName), ValidReferenceRequired]
        public AssetID Marker
        {
            get => _mrkr;
            set
            {
                _mrkr = value;
                CreateTransformMatrix();
            }
        }

        [Category(dynaCategoryName)]
        public bool Opened { get; set; }

        private int _launchAngle;
        [Category(dynaCategoryName)]
        public int LaunchAngle
        {
            get => _launchAngle;
            set
            {
                _launchAngle = value;
                CreateTransformMatrix();
            }
        }

        [Category(dynaCategoryName), Description("Not used in version 1 or Movie/Incredibles.")]
        public int CameraAngle { get; set; }

        [Category(dynaCategoryName), ValidReferenceRequired]
        public AssetID TargetTeleportBox { get; set; }

        public DynaGObjectTeleport(string assetName, uint mrkrId, Func<uint, AssetMRKR> getMRKR) : base(assetName, DynaType.game_object__Teleport, Vector3.Zero)
        {
            _mrkr = mrkrId;
            GetMRKR = getMRKR;
            Version = 2;
        }

        private Func<uint, AssetMRKR> GetMRKR;

        public DynaGObjectTeleport(Section_AHDR AHDR, Game game, Endianness endianness, Func<uint, AssetMRKR> getMRKR) : base(AHDR, DynaType.game_object__Teleport, game, endianness)
        {
            GetMRKR = getMRKR;

            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                _mrkr = reader.ReadUInt32();
                Opened = reader.ReadInt32Bool();
                _launchAngle = reader.ReadInt32();
                if (game < Game.Incredibles && Version > 1)
                    CameraAngle = reader.ReadInt32();
                TargetTeleportBox = reader.ReadUInt32();

                CreateTransformMatrix();
                AddToRenderableAssets(this);
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(_mrkr);
            writer.Write(Opened ? 1 : 0);
            writer.Write(_launchAngle);
            if (game < Game.Incredibles && Version > 1)
                writer.Write(CameraAngle);
            writer.Write(TargetTeleportBox);
        }

        public override AssetSingle PositionX
        {
            get
            {
                var MRKR = GetMRKR(_mrkr);
                return MRKR != null ? MRKR.PositionX : 0;
            }
            set
            {
                var MRKR = GetMRKR(_mrkr);
                if (MRKR != null)
                    MRKR.PositionX = value;
                CreateTransformMatrix();
            }
        }

        public override AssetSingle PositionY
        {
            get
            {
                var MRKR = GetMRKR(_mrkr);
                return MRKR != null ? MRKR.PositionY : 0;
            }
            set
            {
                var MRKR = GetMRKR(_mrkr);
                if (MRKR != null)
                    MRKR.PositionY = value;
                CreateTransformMatrix();
            }
        }

        public override AssetSingle PositionZ
        {
            get
            {
                var MRKR = GetMRKR(_mrkr);
                return MRKR != null ? MRKR.PositionZ : 0;
            }
            set
            {
                var MRKR = GetMRKR(_mrkr);
                if (MRKR != null)
                    MRKR.PositionZ = value;
                CreateTransformMatrix();
            }
        }

        protected override List<Vector3> vertexSource => SharpRenderer.pyramidVertices;

        protected override List<Triangle> triangleSource => SharpRenderer.pyramidTriangles;

        private static readonly uint _modelAssetID = Functions.BKDRHash("teleportation_box_bind");

        public override void CreateTransformMatrix()
        {
            world = Matrix.RotationY(MathUtil.DegreesToRadians(LaunchAngle)) * Matrix.Translation(PositionX, PositionY, PositionZ);

            var model = GetFromRenderingDictionary(_modelAssetID);
            CreateBoundingBox((model != null) ? model.vertexListG : SharpRenderer.pyramidVertices);
            triangles = model?.triangleList.ToArray();
        }

        private void CreateBoundingBox(List<Vector3> vertexList)
        {
            vertices = new Vector3[vertexList.Count];
            for (int i = 0; i < vertexList.Count; i++)
                vertices[i] = (Vector3)Vector3.Transform(vertexList[i], world);
            boundingBox = BoundingBox.FromPoints(vertices);
        }

        public override void Draw(SharpRenderer renderer)
        {
            if (renderingDictionary.ContainsKey(_modelAssetID))
                renderingDictionary[_modelAssetID].Draw(renderer, world, Vector4.One, Vector3.Zero, isSelected);
            else
                renderer.DrawPyramid(world, isSelected);
        }

        public static bool dontRender = false;
        protected override bool DontRender => dontRender;
    }
}