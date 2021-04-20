using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public enum CamType : byte
    {
        Follow = 0,
        Shoulder = 1,
        Static = 2,
        Path = 3,
        StaticFollow = 4,
    }

    public enum CamTransitionType
    {
        None = 0,
        Interp1 = 1,
        Interp2 = 2,
        Interp3 = 3,
        Interp4 = 4,
        Linear = 5,
        Interp1Rev = 6,
        Interp2Rev = 7,
        Interp3Rev = 8,
        Interp4Rev = 9,
    }

    public class AssetCAM : BaseAsset, IRenderableAsset, IClickableAsset
    {
        private const string categoryName = "Camera";

        private Vector3 _position;
        [Category(categoryName)]
        public AssetSingle PositionX
        {
            get => _position.X;
            set { _position.X = value; CreateTransformMatrix(); }
        }
        [Category(categoryName)]
        public AssetSingle PositionY
        {
            get => _position.Y;
            set { _position.Y = value; CreateTransformMatrix(); }
        }
        [Category(categoryName)]
        public AssetSingle PositionZ
        {
            get => _position.Z;
            set { _position.Z = value; CreateTransformMatrix(); }
        }
        [Category(categoryName)]
        public AssetSingle NormalizedForwardX { get; set; }
        [Category(categoryName)]
        public AssetSingle NormalizedForwardY { get; set; }
        [Category(categoryName)]
        public AssetSingle NormalizedForwardZ { get; set; }
        [Category(categoryName)]
        public AssetSingle NormalizedUpX { get; set; }
        [Category(categoryName)]
        public AssetSingle NormalizedUpY { get; set; }
        [Category(categoryName)]
        public AssetSingle NormalizedUpZ { get; set; }
        [Category(categoryName)]
        public AssetSingle NormalizedLeftX { get; set; }
        [Category(categoryName)]
        public AssetSingle NormalizedLeftY { get; set; }
        [Category(categoryName)]
        public AssetSingle NormalizedLeftZ { get; set; }
        [Category(categoryName)]
        public AssetSingle ViewOffsetX { get; set; }
        [Category(categoryName)]
        public AssetSingle ViewOffsetY { get; set; }
        [Category(categoryName)]
        public AssetSingle ViewOffsetZ { get; set; }
        [Category(categoryName)]
        public short OffsetStartFrames { get; set; }
        [Category(categoryName)]
        public short OffsetEndFrames { get; set; }
        [Category(categoryName)]
        public AssetSingle FieldOfView { get; set; }
        [Category(categoryName)]
        public AssetSingle TransitionTime { get; set; }
        [Category(categoryName)]
        public CamTransitionType TransitionType { get; set; }
        [Category(categoryName)]
        public FlagBitmask CamFlags { get; set; } = IntFlagsDescriptor();
        [Category(categoryName)]
        public AssetSingle FadeUp { get; set; }
        [Category(categoryName)]
        public AssetSingle FadeDown { get; set; }
        [Category(categoryName), TypeConverter(typeof(ExpandableObjectConverter))]
        public CamSpecific_Generic CamSpecific { get; set; }
        [Category(categoryName)]
        public FlagBitmask Flags1 { get; set; } = ByteFlagsDescriptor();
        [Category(categoryName)]
        public FlagBitmask Flags2 { get; set; } = ByteFlagsDescriptor();
        [Category(categoryName)]
        public FlagBitmask Flags3 { get; set; } = ByteFlagsDescriptor();
        [Category(categoryName)]
        public FlagBitmask Flags4 { get; set; } = ByteFlagsDescriptor();
        [Category(categoryName)]
        public AssetID Marker1AssetID { get; set; }
        [Category(categoryName)]
        public AssetID Marker2AssetID { get; set; }
        private CamType _camType;
        [Category(categoryName)]
        public CamType CamType 
        { 
            get => _camType;
            set
            {
                _camType = value;
                switch (_camType)
                {
                    case CamType.Follow:
                        CamSpecific = new CamSpecific_Follow();
                        break;
                    case CamType.Shoulder:
                        CamSpecific = new CamSpecific_Shoulder();
                        break;
                    case CamType.Static:
                        CamSpecific = new CamSpecific_Static();
                        break;
                    case CamType.Path:
                        CamSpecific = new CamSpecific_Path();
                        break;
                    case CamType.StaticFollow:
                        CamSpecific = new CamSpecific_StaticFollow();
                        break;
                    default:
                        CamSpecific = new CamSpecific_Generic();
                        break;
                }
            }
        }

        public AssetCAM(string assetName, Vector3 position, AssetTemplate template) : base(assetName, AssetType.CAM, BaseAssetType.Camera)
        {
            _position = position;

            OffsetStartFrames = 30;
            OffsetEndFrames = 45;
            FieldOfView = 100;
            Flags1.FlagValueByte = 0;
            Flags2.FlagValueByte = 1;
            Flags3.FlagValueByte = 1;
            Flags4.FlagValueByte = 0xC0;

            CamType = CamType.Static;

            if (template == AssetTemplate.StartCamera)
            {
                NormalizedForwardZ = 1;
                NormalizedUpY = 1;
                NormalizedLeftX = 1;
                FieldOfView = 85;
                Flags4.FlagValueByte = 0x8F;
                CamType = CamType.Follow;
                var camSpecific = (CamSpecific_Follow)CamSpecific;
                camSpecific.Distance = -2;
                camSpecific.Height = 1;
                camSpecific.RubberBand = 1;
            }
            else if (template == AssetTemplate.BusStop_Camera)
            {
                PositionX -= 7f;
                PositionY += 2f;
                NormalizedForwardX = 1f;
                NormalizedForwardY = 0f;
                NormalizedForwardZ = 0f;
                NormalizedUpX = 0f;
                NormalizedUpX = 1f;
                NormalizedUpX = 0f;
                NormalizedLeftX = 0f;
                NormalizedLeftY = 0f;
                NormalizedLeftZ = 1f;
                OffsetStartFrames = 30;
                OffsetEndFrames = 45;
                FieldOfView = 60f;
                TransitionTime = 0.5f;
                Flags4.FlagValueByte = 0x8F;
                CamType = CamType.Static;
            }

            CreateTransformMatrix();
            ArchiveEditorFunctions.renderableAssets.Add(this);
        }

        public AssetCAM(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = baseHeaderEndPosition;

            _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            NormalizedForwardX = reader.ReadSingle();
            NormalizedForwardY = reader.ReadSingle();
            NormalizedForwardZ = reader.ReadSingle();
            NormalizedUpX = reader.ReadSingle();
            NormalizedUpY = reader.ReadSingle();
            NormalizedUpZ = reader.ReadSingle();
            NormalizedLeftX = reader.ReadSingle();
            NormalizedLeftY = reader.ReadSingle();
            NormalizedLeftZ = reader.ReadSingle();
            ViewOffsetX = reader.ReadSingle();
            ViewOffsetY = reader.ReadSingle();
            ViewOffsetZ = reader.ReadSingle();
            OffsetStartFrames = reader.ReadInt16();
            OffsetEndFrames = reader.ReadInt16();
            FieldOfView = reader.ReadSingle();
            TransitionTime = reader.ReadSingle();
            TransitionType = (CamTransitionType)reader.ReadInt32();
            CamFlags.FlagValueInt = reader.ReadUInt32();
            FadeUp = reader.ReadSingle();
            FadeDown = reader.ReadSingle();

            reader.BaseStream.Position = 0x78;
            Flags1.FlagValueByte = reader.ReadByte();
            Flags2.FlagValueByte = reader.ReadByte();
            Flags3.FlagValueByte = reader.ReadByte();
            Flags4.FlagValueByte = reader.ReadByte();
            Marker1AssetID = reader.ReadUInt32();
            Marker2AssetID = reader.ReadUInt32();
            _camType = (CamType)reader.ReadByte();

            reader.BaseStream.Position = 0x60;
            switch (_camType)
            {
                case CamType.Follow:
                    CamSpecific = new CamSpecific_Follow(reader);
                    break;
                case CamType.Shoulder:
                    CamSpecific = new CamSpecific_Shoulder(reader);
                    break;
                case CamType.Static:
                    CamSpecific = new CamSpecific_Static(reader);
                    break;
                case CamType.Path:
                    CamSpecific = new CamSpecific_Path(reader);
                    break;
                case CamType.StaticFollow:
                    CamSpecific = new CamSpecific_StaticFollow(reader);
                    break;
                default:
                    CamSpecific = new CamSpecific_Generic();
                    break;
            }

            CreateTransformMatrix();
            ArchiveEditorFunctions.renderableAssets.Add(this);
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(SerializeBase(platform));

            writer.Write(_position.X);
            writer.Write(_position.Y);
            writer.Write(_position.Z);
            writer.Write(NormalizedForwardX);
            writer.Write(NormalizedForwardY);
            writer.Write(NormalizedForwardZ);
            writer.Write(NormalizedUpX);
            writer.Write(NormalizedUpY);
            writer.Write(NormalizedUpZ);
            writer.Write(NormalizedLeftX);
            writer.Write(NormalizedLeftY);
            writer.Write(NormalizedLeftZ);
            writer.Write(ViewOffsetX);
            writer.Write(ViewOffsetY);
            writer.Write(ViewOffsetZ);
            writer.Write(OffsetStartFrames);
            writer.Write(OffsetEndFrames);
            writer.Write(FieldOfView);
            writer.Write(TransitionTime);
            writer.Write((int)TransitionType);
            writer.Write(CamFlags.FlagValueInt);
            writer.Write(FadeUp);
            writer.Write(FadeDown);
            writer.Write(CamSpecific.Serialize(game, platform));
            while (writer.BaseStream.Length < 0x78)
                writer.Write((byte)0);
            writer.Write(Flags1.FlagValueByte);
            writer.Write(Flags2.FlagValueByte);
            writer.Write(Flags3.FlagValueByte);
            writer.Write(Flags4.FlagValueByte);
            writer.Write(Marker1AssetID);
            writer.Write(Marker2AssetID);
            writer.Write((byte)_camType);
            while (writer.BaseStream.Length < 0x88)
                writer.Write((byte)0);

            writer.Write(SerializeLinks(platform));
            return writer.ToArray();
        }

        private Matrix world;
        private BoundingBox boundingBox;

        public static bool dontRender = false;

        [Browsable(false)]
        public bool SpecialBlendMode => true;

        public override bool HasReference(uint assetID) => Marker1AssetID == assetID || Marker2AssetID == assetID ||
            CamSpecific.HasReference(assetID) || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Verify(Marker1AssetID, ref result);
            Verify(Marker2AssetID, ref result);

            CamSpecific.Verify(ref result);

            Vector3 nForward = new Vector3(NormalizedForwardX, NormalizedForwardY, NormalizedForwardZ);
            if (nForward != Vector3.Normalize(nForward))
                result.Add("Camera forward vector seems to not be normalized.");

            Vector3 nUp = new Vector3(NormalizedUpX, NormalizedUpY, NormalizedUpZ);
            if (nUp != Vector3.Normalize(nUp))
                result.Add("Camera up vector seems to not be normalized.");

            Vector3 nLeft = new Vector3(NormalizedLeftX, NormalizedLeftY, NormalizedLeftZ);
            if (nLeft != Vector3.Normalize(nLeft))
                result.Add("Camera left vector seems to not be normalized.");
        }

        public void CreateTransformMatrix()
        {
            world = Matrix.Translation(_position);
            CreateBoundingBox();
        }

        protected void CreateBoundingBox()
        {
            var vertices = new Vector3[SharpRenderer.cubeVertices.Count];

            for (int i = 0; i < SharpRenderer.cubeVertices.Count; i++)
                vertices[i] = (Vector3)Vector3.Transform(SharpRenderer.cubeVertices[i] * 0.5f, world);

            boundingBox = BoundingBox.FromPoints(vertices);
        }

        public float? GetIntersectionPosition(SharpRenderer renderer, Ray ray)
        {
            if (ShouldDraw(renderer) && ray.Intersects(ref boundingBox))
                return TriangleIntersection(ray, SharpRenderer.cubeTriangles, SharpRenderer.cubeVertices, world);
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

        public void Draw(SharpRenderer renderer) => renderer.DrawCube(world, isSelected);

        public BoundingBox GetBoundingBox() => boundingBox;

        public float GetDistanceFrom(Vector3 cameraPosition) => Vector3.Distance(cameraPosition, _position);

        public void SetPosition(Vector3 position)
        {
            PositionX = position.X;
            PositionY = position.Y;
            PositionZ = position.Z;
        }

        public void SetNormalizedForward(Vector3 forward)
        {
            NormalizedForwardX = forward.X;
            NormalizedForwardY = forward.Y;
            NormalizedForwardZ = forward.Z;
        }

        public void SetNormalizedUp(Vector3 up)
        {
            NormalizedUpX = up.X;
            NormalizedUpY = up.Y;
            NormalizedUpZ = up.Z;
        }

        public void SetNormalizedLeft(Vector3 right)
        {
            NormalizedLeftX = -right.X;
            NormalizedLeftY = -right.Y;
            NormalizedLeftZ = -right.Z;
        }
    }
}