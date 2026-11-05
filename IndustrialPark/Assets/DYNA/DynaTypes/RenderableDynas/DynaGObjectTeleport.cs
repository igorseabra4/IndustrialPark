using HipHopFile;
using IndustrialPark.Models;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaGObjectTeleport : RenderableDynaBase
    {
        private const string dynaCategoryName = "game_object:Teleport";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => $"{HexUIntTypeConverter.StringFromAssetID(Marker)} {HexUIntTypeConverter.StringFromAssetID(TargetTeleportBox)}";

        public override string Note => "Version is always 1 or 2.";

        [Category(dynaCategoryName), ValidReferenceRequired]
        public AssetID Marker { get; set; }

        [Category(dynaCategoryName)]
        public bool Opened { get; set; }

        [Category(dynaCategoryName)]
        public int LaunchAngle { get; set; }

        [Category(dynaCategoryName), Description("Only used in BFBB, Version 2")]
        public int CameraAngle { get; set; }

        [Category(dynaCategoryName), ValidReferenceRequired]
        public AssetID TargetTeleportBox { get; set; }

        public DynaGObjectTeleport(string assetName, uint mrkrId) : base(assetName, DynaType.game_object__Teleport, Vector3.Zero)
        {
            Marker = mrkrId;
            Version = 2;
        }

        public Func<uint, AssetMRKR> GetMRKR;

        public DynaGObjectTeleport(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__Teleport, game, endianness)
        {
            using var reader = new EndianBinaryReader(AHDR.data, endianness);
            reader.BaseStream.Position = dynaDataStartPosition;

            Marker = reader.ReadUInt32();
            Opened = reader.ReadInt32Bool();
            LaunchAngle = reader.ReadInt32();
            if (game < Game.Incredibles && Version > 1)
                CameraAngle = reader.ReadInt32();
            TargetTeleportBox = reader.ReadUInt32();
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(Marker);
            writer.Write(Opened ? 1 : 0);
            writer.Write(LaunchAngle);
            if (game < Game.Incredibles && Version > 1)
                writer.Write(CameraAngle);
            writer.Write(TargetTeleportBox);
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game == Game.Incredibles)
                dt.RemoveProperty("CameraAngle");
            base.SetDynamicProperties(dt);
        }

        public const string PositionDescription = "The position values are defined in the Marker asset. I will update them for you if you define them here or with the gizmo, and the marker will turn invisible.";

        [Description(PositionDescription)]
        public override AssetSingle PositionX
        {
            get
            {
                var MRKR = GetMRKR?.Invoke(Marker);
                return _position.X = MRKR != null ? MRKR.PositionX : 0;
            }
            set
            {
                var MRKR = GetMRKR?.Invoke(Marker);
                MRKR?.PositionX = value;
                MRKR?.isInvisible = true;
                MRKR?.CreateTransformMatrix(Program.Renderer);
                _position.X = value;
            }
        }

        [Description(PositionDescription)]
        public override AssetSingle PositionY
        {
            get
            {
                var MRKR = GetMRKR?.Invoke(Marker);
                return _position.Z = MRKR != null ? MRKR.PositionY : 0;
            }
            set
            {
                var MRKR = GetMRKR?.Invoke(Marker);
                MRKR?.PositionY = value;
                MRKR?.isInvisible = true;
                MRKR?.CreateTransformMatrix(Program.Renderer);
                _position.Y = value;
            }
        }

        [Description(PositionDescription)]
        public override AssetSingle PositionZ
        {
            get
            {
                var MRKR = GetMRKR?.Invoke(Marker);
                return _position.Z = MRKR != null ? MRKR.PositionZ : 0;
            }
            set
            {
                var MRKR = GetMRKR?.Invoke(Marker);
                MRKR?.PositionZ = value;
                MRKR?.isInvisible = true;
                MRKR?.CreateTransformMatrix(Program.Renderer);
                _position.Z = value;
            }
        }

        protected override List<Vector3> vertexSource(SharpRenderer renderer) => renderer.pyramidVertices;

        protected override List<Triangle> triangleSource(SharpRenderer renderer) => renderer.pyramidTriangles;

        private static readonly uint _modelAssetID = new AssetID("teleportation_box_bind");

        public override void CreateTransformMatrix(SharpRenderer renderer)
        {
            world = Matrix.RotationY(MathUtil.DegreesToRadians(LaunchAngle)) * Matrix.Translation(PositionX, PositionY, PositionZ);

            var model = renderer.Assets.GetModelFromRenderingDictionary(_modelAssetID);
            CreateBoundingBox((model != null) ? model.vertexListG : renderer.pyramidVertices);
            triangles = model?.triangleList;
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
            if ((renderer.Assets.GetFromRenderingDictionary(_modelAssetID) is IAssetWithModel value))
                value.Draw(renderer, world, Vector4.One, Vector3.Zero, isSelected);
            else
                renderer.DrawPyramid(world, isSelected);
        }

        public static bool dontRender = false;
        protected override bool DontRender => dontRender;
    }
}