using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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
        private Matrix world;
        private BoundingBox boundingBox;

        public static bool dontRender = false;

        protected override int EventStartOffset => 0x88;

        public AssetCAM(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            _position = new Vector3(ReadFloat(0x8), ReadFloat(0xC), ReadFloat(0x10));
            CreateTransformMatrix();
            ArchiveEditorFunctions.renderableAssets.Add(this);
        }

        public override bool HasReference(uint assetID) => Marker1AssetID == assetID || Marker2AssetID == assetID ||
            (CamSpecific is CamSpecific_Path camSpecific_Path && camSpecific_Path.Unknown_AssetID == assetID) ||
            base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Verify(Marker1AssetID, ref result);
            Verify(Marker2AssetID, ref result);

            if (CamSpecific is CamSpecific_Path camSpecific_Path)
                Verify(camSpecific_Path.Unknown_AssetID, ref result);

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
            if (!ShouldDraw(renderer))
                return null;

            if (ray.Intersects(ref boundingBox, out float distance))
                return distance;
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
        
        public BoundingBox GetBoundingBox()
        {
            return boundingBox;
        }

        public float GetDistanceFrom(Vector3 cameraPosition)
        {
            return Vector3.Distance(cameraPosition, _position);
        }

        [Browsable(false)]
        public bool SpecialBlendMode => true;

        private Vector3 _position;
        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public float PositionX
        {
            get { return _position.X; }
            set
            {
                _position.X = value;
                Write(0x8, _position.X);
                CreateTransformMatrix();
            }
        }

        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public float PositionY
        {
            get { return _position.Y; }
            set
            {
                _position.Y = value;
                Write(0xC, _position.Y);
                CreateTransformMatrix();
            }
        }

        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public float PositionZ
        {
            get { return _position.Z; }
            set
            {
                _position.Z = value;
                Write(0x10, _position.Z);
                CreateTransformMatrix();
            }
        }

        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public float NormalizedForwardX
        {
            get => ReadFloat(0x14);
            set => Write(0x14, value);
        }
        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public float NormalizedForwardY
        {
            get => ReadFloat(0x18);
            set => Write(0x18, value);
        }
        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public float NormalizedForwardZ
        {
            get => ReadFloat(0x1C);
            set => Write(0x1C, value);
        }
        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public float NormalizedUpX
        {
            get => ReadFloat(0x20);
            set => Write(0x20, value);
        }
        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public float NormalizedUpY
        {
            get => ReadFloat(0x24);
            set => Write(0x24, value);
        }
        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public float NormalizedUpZ
        {
            get => ReadFloat(0x28);
            set => Write(0x28, value);
        }
        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public float NormalizedLeftX
        {
            get => ReadFloat(0x2C);
            set => Write(0x2C, value);
        }
        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public float NormalizedLeftY
        {
            get => ReadFloat(0x30);
            set => Write(0x30, value);
        }
        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public float NormalizedLeftZ
        {
            get => ReadFloat(0x34);
            set => Write(0x34, value);
        }
        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public float ViewOffsetX
        {
            get => ReadFloat(0x38);
            set => Write(0x38, value);
        }
        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public float ViewOffsetY
        {
            get => ReadFloat(0x3C);
            set => Write(0x3C, value);
        }
        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public float ViewOffsetZ
        {
            get => ReadFloat(0x40);
            set => Write(0x40, value);
        }
        [Category("Camera")]
        public short OffsetStartFrames
        {
            get => ReadShort(0x44);
            set => Write(0x44, value);
        }
        [Category("Camera")]
        public short OffsetEndFrames
        {
            get => ReadShort(0x46);
            set => Write(0x46, value);
        }
        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public float FieldOfView
        {
            get => ReadFloat(0x48);
            set => Write(0x48, value);
        }
        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public float TransitionTime
        {
            get => ReadFloat(0x4C);
            set => Write(0x4C, value);
        }
        [Category("Camera")]
        public CamTransitionType TransitionType
        {
            get => (CamTransitionType)ReadInt(0x50);
            set => Write(0x50, (int)value);
        }
        [Category("Camera")]
        public DynamicTypeDescriptor CamFlags => IntFlagsDescriptor(0x54);
        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public float FadeUp
        {
            get => ReadFloat(0x58);
            set => Write(0x58, value);
        }
        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public float FadeDown
        {
            get => ReadFloat(0x5C);
            set => Write(0x5C, value);
        }
        
        [Category("Camera"), TypeConverter(typeof(ExpandableObjectConverter))]
        public CamSpecific_Generic CamSpecific
        {
            get
            {
                switch (CamType)
                {
                    case CamType.Follow:
                        return new CamSpecific_Follow(this);
                    case CamType.Shoulder:
                        return new CamSpecific_Shoulder(this);
                    case CamType.Static:
                        return new CamSpecific_Static(this);
                    case CamType.Path:
                        return new CamSpecific_Path(this);
                    case CamType.StaticFollow:
                        return new CamSpecific_StaticFollow(this);
                    default:
                        return new CamSpecific_Generic(this);
                }
            }
        }

        [Category("Camera")]
        public DynamicTypeDescriptor Flags1 => ByteFlagsDescriptor(0x78);
        [Category("Camera")]
        public DynamicTypeDescriptor Flags2 => ByteFlagsDescriptor(0x79);
        [Category("Camera")]
        public DynamicTypeDescriptor Flags3 => ByteFlagsDescriptor(0x7A);
        [Category("Camera")]
        public DynamicTypeDescriptor Flags4 => ByteFlagsDescriptor(0x7B);
        [Category("Camera")]
        public AssetID Marker1AssetID
        {
            get => ReadUInt(0x7C);
            set => Write(0x7C, value);
        }
        [Category("Camera")]
        public AssetID Marker2AssetID
        {
            get => ReadUInt(0x80);
            set => Write(0x80, value);
        }
        [Category("Camera")]
        public CamType CamType
        {
            get => (CamType)ReadByte(0x84);
            set => Write(0x84, (byte)value);
        }
        [Category("Camera")]
        public byte Padding85
        {
            get => ReadByte(0x85);
            set => Write(0x85, value);
        }
        [Category("Camera")]
        public byte Padding86
        {
            get => ReadByte(0x86);
            set => Write(0x86, value);
        }
        [Category("Camera")]
        public byte Padding87
        {
            get => ReadByte(0x87);
            set => Write(0x87, value);
        }

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