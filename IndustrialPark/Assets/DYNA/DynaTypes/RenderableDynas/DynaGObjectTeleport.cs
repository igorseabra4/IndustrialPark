using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;
using HipHopFile;
using static IndustrialPark.ArchiveEditorFunctions;
using IndustrialPark.Models;

namespace IndustrialPark
{
    public class DynaGObjectTeleport : RenderableDynaBase
    {
        private const string dynaCategoryName = "game_object:Teleport";

        public override string Note => "Version is always 1 or 2. Version 1 does not use CameraAngle.";

        [Category(dynaCategoryName)]
        public AssetID MRKR_ID { get; set; }
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

        public DynaGObjectTeleport(string assetName, uint mrkrId) : base(assetName, DynaType.game_object__Teleport, 2)
        {
            MRKR_ID = mrkrId;
        }

        public DynaGObjectTeleport(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__Teleport, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                MRKR_ID = reader.ReadUInt32();
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
                writer.Write(MRKR_ID);
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
            if (MRKR_ID == assetID)
                return true;
            if (TargetDYNATeleportID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            if (MRKR_ID == 0)
                result.Add("Teleport with no MRKR reference");
            Verify(MRKR_ID, ref result);
            if (TargetDYNATeleportID == 0)
                result.Add("Teleport with no target reference");
            Verify(TargetDYNATeleportID, ref result);
        }
        
        private void ValidateMRKR()
        {
            if (Program.MainForm != null)
                foreach (ArchiveEditor ae in Program.MainForm.archiveEditors)
                    if (ae.archive.ContainsAsset(MRKR_ID) && ae.archive.GetFromAssetID(MRKR_ID) is AssetMRKR MRKR)
                    {
                        this.MRKR = MRKR;
                        this.MRKR.isInvisible = true;
                        return;
                    }
            MRKR = null;
        }

        public override AssetSingle PositionX
        {
            get
            {
                ValidateMRKR();
                if (MRKR != null)
                    return MRKR.PositionX;
                return 0;
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
                if (MRKR != null)
                    return MRKR.PositionY;
                return 0;
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
                if (MRKR != null)
                    return MRKR.PositionZ;
                return 0;
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
            ValidateMRKR();
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
    }
}