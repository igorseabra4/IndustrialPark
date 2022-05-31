using HipHopFile;
using IndustrialPark.Models;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class DynaGObjectTeleport : RenderableDynaBase
    {
        private const string dynaCategoryName = "game_object:Teleport";

        public override string Note => "Version is always 1 or 2. Version 1 does not use CameraAngle.";

        [Category(dynaCategoryName)]
        private uint _mRKR_ID;
        public AssetID MRKR_ID
        {
            get => _mRKR_ID;
            set
            {
                _mRKR_ID = value;
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

        [Category(dynaCategoryName), Description("Not used in version 1 or Movie.")]
        public int CameraAngle { get; set; }

        [Category(dynaCategoryName)]
        public AssetID TargetDYNATeleportID { get; set; }

        public DynaGObjectTeleport(string assetName, uint mrkrId, DynaGObjectTeleportGetMRKR getMRKR) : base(assetName, DynaType.game_object__Teleport, 2)
        {
            _mRKR_ID = mrkrId;
            this.GetMRKR = getMRKR;
        }

        public delegate AssetMRKR DynaGObjectTeleportGetMRKR(uint MRKR_ID);
        private DynaGObjectTeleportGetMRKR GetMRKR;

        public DynaGObjectTeleport(Section_AHDR AHDR, Game game, Endianness endianness, DynaGObjectTeleportGetMRKR getMRKR) : base(AHDR, DynaType.game_object__Teleport, game, endianness)
        {
            this.GetMRKR = getMRKR;

            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                _mRKR_ID = reader.ReadUInt32();
                Opened = reader.ReadInt32Bool();
                _launchAngle = reader.ReadInt32();
                if (game != Game.Incredibles && Version > 1)
                    CameraAngle = reader.ReadInt32();
                TargetDYNATeleportID = reader.ReadUInt32();

                CreateTransformMatrix();
                AddToRenderableAssets(this);
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(_mRKR_ID);
                writer.Write(Opened ? 1 : 0);
                writer.Write(_launchAngle);
                if (game != Game.Incredibles && Version > 1)
                    writer.Write(CameraAngle);
                writer.Write(TargetDYNATeleportID);

                return writer.ToArray();
            }
        }

        public override bool HasReference(uint assetID)
        {
            if (_mRKR_ID == assetID)
                return true;
            if (TargetDYNATeleportID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            if (_mRKR_ID == 0)
                result.Add("Teleport with no MRKR reference");
            Verify(_mRKR_ID, ref result);
            if (TargetDYNATeleportID == 0)
                result.Add("Teleport with no target reference");
            Verify(TargetDYNATeleportID, ref result);
            base.Verify(ref result);
        }

        private void ValidateMRKR()
        {
            MRKR = GetMRKR(_mRKR_ID);
        }

        public override AssetSingle PositionX
        {
            get
            {
                ValidateMRKR();
                return MRKR != null ? MRKR.PositionX : 0;
            }
            set
            {
                ValidateMRKR();
                if (MRKR != null)
                    MRKR.PositionX = value;
                CreateTransformMatrix();
            }
        }

        public override AssetSingle PositionY
        {
            get
            {
                ValidateMRKR();
                return MRKR != null ? MRKR.PositionY : 0;
            }
            set
            {
                ValidateMRKR();
                if (MRKR != null)
                    MRKR.PositionY = value;
                CreateTransformMatrix();
            }
        }

        public override AssetSingle PositionZ
        {
            get
            {
                ValidateMRKR();
                return MRKR != null ? MRKR.PositionZ : 0;
            }
            set
            {
                ValidateMRKR();
                if (MRKR != null)
                    MRKR.PositionZ = value;
                CreateTransformMatrix();
            }
        }

        protected override List<Vector3> vertexSource => SharpRenderer.pyramidVertices;

        protected override List<Triangle> triangleSource => SharpRenderer.pyramidTriangles;

        private static readonly uint _modelAssetID = Functions.BKDRHash("teleportation_box_bind");

        private AssetMRKR MRKR;

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
                renderingDictionary[_modelAssetID].Draw(renderer, world, isSelected ? renderer.selectedObjectColor : Vector4.One, Vector3.Zero);
            else
                renderer.DrawPyramid(world, isSelected);
        }

        public static bool dontRender = false;
        public override bool DontRender => dontRender;
    }
}