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
        [Category(categoryName)]
        public CamType CamType
        {
            get => _camType;
            set
            {
                if (_camType != value)
                {
                    _camType = value;
                    CamSpecific = _camType switch
                    {
                        CamType.Follow => new CamSpecific_Follow(),
                        CamType.Shoulder => new CamSpecific_Shoulder(),
                        CamType.Static => new CamSpecific_Static(),
                        CamType.Path => new CamSpecific_Path(),
                        CamType.StaticFollow => new CamSpecific_StaticFollow(),
                        _ => new CamSpecific_Other(),
                    };
                }
            }
        }
        [Category(categoryName), DisplayName("Data")]
        public CamSpecific_Generic CamSpecific { get; set; }
        [Category(categoryName)]
        public FlagBitmask ValidFlags { get; set; } = IntFlagsDescriptor();
        [Category(categoryName)]
        public AssetID Marker1 { get; set; }
        [Category(categoryName)]
        public AssetID Marker2 { get; set; }
        private CamType _camType;

        public AssetCAM(string assetName, Vector3 position, AssetTemplate template) : base(assetName, AssetType.Camera, BaseAssetType.Camera)
        {
            _position = position;

            OffsetStartFrames = 30;
            OffsetEndFrames = 45;
            FieldOfView = 100;
            ValidFlags.FlagValueInt = 0x000101C0;

            CamType = CamType.Static;

            if (template == AssetTemplate.Start_Camera)
            {
                NormalizedForwardZ = 1;
                NormalizedUpY = 1;
                NormalizedLeftX = 1;
                FieldOfView = 85;
                ValidFlags.FlagValueInt = 0x0001018F;
                CamType = CamType.Follow;
                var camSpecific = (CamSpecific_Follow)CamSpecific;
                camSpecific.Distance = -2;
                camSpecific.Height = 1;
                camSpecific.RubberBand = 1;
            }
            else if (template == AssetTemplate.Bus_Stop_Camera)
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
                ValidFlags.FlagValueInt = 0x0001018F;
                CamType = CamType.Static;
            }

            CreateTransformMatrix();
            ArchiveEditorFunctions.AddToRenderableAssets(this);
        }

        public AssetCAM(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
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
                ValidFlags.FlagValueInt = reader.ReadUInt32();
                Marker1 = reader.ReadUInt32();
                Marker2 = reader.ReadUInt32();
                _camType = (CamType)reader.ReadByte();

                reader.BaseStream.Position = 0x60;
                CamSpecific = _camType switch
                {
                    CamType.Follow => new CamSpecific_Follow(reader),
                    CamType.Shoulder => new CamSpecific_Shoulder(reader),
                    CamType.Static => new CamSpecific_Static(reader),
                    CamType.Path => new CamSpecific_Path(reader),
                    CamType.StaticFollow => new CamSpecific_StaticFollow(reader),
                    _ => new CamSpecific_Other(reader),
                };

                CreateTransformMatrix();
                ArchiveEditorFunctions.AddToRenderableAssets(this);
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);

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
            CamSpecific.Serialize(writer);
            while (writer.BaseStream.Length < 0x78)
                writer.Write((byte)0);
            writer.Write(ValidFlags.FlagValueInt);
            writer.Write(Marker1);
            writer.Write(Marker2);
            writer.Write((byte)_camType);
            while (writer.BaseStream.Length < 0x88)
                writer.Write((byte)0);

            SerializeLinks(writer);
        }

        private Matrix world;
        private BoundingBox boundingBox;

        public static bool dontRender = false;

        [Browsable(false)]
        public bool SpecialBlendMode => true;

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Vector3 nForward = new Vector3(NormalizedForwardX, NormalizedForwardY, NormalizedForwardZ);
            if (!MathUtil.IsOne(nForward.Length()))
                result.Add("Forward vector seems to not be normalized.");

            Vector3 nUp = new Vector3(NormalizedUpX, NormalizedUpY, NormalizedUpZ);
            if (!MathUtil.IsOne(nUp.Length()))
                result.Add("Up vector seems to not be normalized.");

            Vector3 nLeft = new Vector3(NormalizedLeftX, NormalizedLeftY, NormalizedLeftZ);
            if (!MathUtil.IsOne(nLeft.Length()))
                result.Add("Left vector seems to not be normalized.");
        }

        public Matrix GetCameraRotationMatrix()
        {
            Vector3 forward = Vector3.Normalize(new Vector3(NormalizedForwardX, NormalizedForwardY, NormalizedForwardZ));
            Vector3 up = Vector3.Normalize(new Vector3(NormalizedUpX, NormalizedUpY, NormalizedUpZ));
            Vector3 left = Vector3.Normalize(Vector3.Cross(up, forward));

            return new Matrix(
                left.X, left.Y, left.Z, 0f,
                up.X, up.Y, up.Z, 0f,
                forward.X, forward.Y, forward.Z, 0f,
                0f, 0f, 0f, 1f);
        }

        public void CreateTransformMatrix()
        {
            var left = new Vector3(NormalizedLeftX, NormalizedLeftY, NormalizedLeftZ);
            left.Normalize();
            var up = new Vector3(NormalizedUpX, NormalizedUpY, NormalizedUpZ);
            up.Normalize();
            var forward = new Vector3(NormalizedForwardX, NormalizedForwardY, NormalizedForwardZ);
            forward.Normalize();

            var rotation = new Matrix(
                left.X, left.Y, left.Z, 0,
                up.X, up.Y, up.Z, 0,
                forward.X, forward.Y, forward.Z, 0,
                0, 0, 0, 1
            );

            world = Matrix.Translation(0f, 0f, -0.5f) *
                Matrix.Scaling(new Vector3(4f, 3f, 3f) / 8f) *
                rotation *
                Matrix.Translation(_position);
            CreateBoundingBox();
        }

        protected void CreateBoundingBox()
        {
            var vertices = new Vector3[SharpRenderer.pyramidVertices.Count];

            for (int i = 0; i < SharpRenderer.pyramidVertices.Count; i++)
                vertices[i] = (Vector3)Vector3.Transform(SharpRenderer.pyramidVertices[i], world);

            boundingBox = BoundingBox.FromPoints(vertices);
        }

        public float? GetIntersectionPosition(SharpRenderer renderer, Ray ray)
        {
            if (ShouldDraw(renderer) && ray.Intersects(ref boundingBox))
                return TriangleIntersection(ray, SharpRenderer.pyramidTriangles, SharpRenderer.pyramidVertices, world);
            return null;
        }

        public static float CameraWidgetDrawMinDistance = 0.5f;

        public bool ShouldDraw(SharpRenderer renderer)
        {
            if (isSelected)
                return GetDistanceFrom(renderer.Camera.Position) > CameraWidgetDrawMinDistance;
            if (dontRender)
                return false;
            if (isInvisible)
                return false;
            if (AssetMODL.renderBasedOnLodt && GetDistanceFrom(renderer.Camera.Position) > SharpRenderer.DefaultLODTDistance)
                return false;
            if (renderer.frustum.Intersects(ref boundingBox))
                return GetDistanceFrom(renderer.Camera.Position) > CameraWidgetDrawMinDistance;

            return false;
        }

        public static SharpDX.Direct3D11.Buffer CamVertexBuffer;

        public void Draw(SharpRenderer renderer) 
        {
            renderer.DrawPyramid(world, isSelected, 1f, renderer.colors.camColor);
            renderer.DrawSpline(CamVertexBuffer, 2, world, isSelected ? Vector4.One : renderer.colors.camColor, true);
        }

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

        public (Vector3 Forward, Vector3 Up, Vector3 Left) GetCameraOrientation() => (
            new Vector3(NormalizedForwardX, NormalizedForwardY, NormalizedForwardZ),
            new Vector3(NormalizedUpX, NormalizedUpY, NormalizedUpZ),
            new Vector3(NormalizedLeftX, NormalizedLeftY, NormalizedLeftZ)
        );

        public void ApplyRotation(Vector3 Forward, Vector3 Up, Vector3 axis, float angleDegrees)
        {
            Matrix rotation = Matrix.RotationAxis(Vector3.Normalize(axis), MathUtil.DegreesToRadians(angleDegrees));

            Vector3 forward = Vector3.Normalize(Vector3.TransformNormal(Forward, rotation));
            Vector3 up = Vector3.Normalize(Vector3.TransformNormal(Up, rotation));
            Vector3 left = Vector3.Normalize(Vector3.Cross(up, forward));

            NormalizedForwardX = forward.X;
            NormalizedForwardY = forward.Y;
            NormalizedForwardZ = forward.Z;

            NormalizedUpX = up.X;
            NormalizedUpY = up.Y;
            NormalizedUpZ = up.Z;

            NormalizedLeftX = left.X;
            NormalizedLeftY = left.Y;
            NormalizedLeftZ = left.Z;

            CreateTransformMatrix();
        }
    }
}