using HipHopFile;
using IndustrialPark.Archive;
using Newtonsoft.Json;
using SharpDX;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace IndustrialPark
{
    public class AssetSFX : BaseAsset, IRenderableAsset, IClickableAsset, IScalableAsset, IAssetCopyPasteTransformation
    {
        private const string categoryName = "SFX";
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(Sound);

        [Category(categoryName)]
        public FlagBitmask Flags08 { get; set; } = ByteFlagsDescriptor();
        [Category(categoryName)]
        public FlagBitmask Flags09 { get; set; } = ByteFlagsDescriptor();
        [Category(categoryName)]
        public short Frequency { get; set; }
        [Category(categoryName)]
        public AssetSingle MinFrequency { get; set; }
        [Category(categoryName), ValidReferenceRequired]
        public AssetID Sound { get; set; }
        [Category(categoryName)]
        public AssetID Attach { get; set; }
        [Category(categoryName)]
        public byte LoopCount { get; set; }
        [Category(categoryName)]
        public byte Priority { get; set; }
        [Category(categoryName)]
        public byte Volume { get; set; }
        private Vector3 _position;
        [Category(categoryName)]
        public AssetSingle PositionX
        {
            get => _position.X;
            set => _position.X = value;
        }
        [Category(categoryName)]
        public AssetSingle PositionY
        {
            get => _position.Y;
            set => _position.Y = value;
        }
        [Category(categoryName)]
        public AssetSingle PositionZ
        {
            get => _position.Z;
            set => _position.Z = value;
        }
        private float _radius;
        [Category(categoryName)]
        public AssetSingle InnerRadius
        {
            get => _radius;
            set => _radius = value;
        }
        private float _radius2;
        [Category(categoryName)]
        public AssetSingle OuterRadius
        {
            get => _radius2;
            set => _radius2 = value;
        }

        public AssetSFX(string assetName, Vector3 position, Game game, AssetTemplate template) : base(assetName, AssetType.SFX, BaseAssetType.SFX)
        {
            _position = position;

            if (template == AssetTemplate.SFX_OnRadius)
            {
                Flags08.FlagValueByte = 0x03;
                Flags09.FlagValueByte = 0xE6;
                _links = new Link[]
                {
                    new Link(game)
                    {
                        TargetAsset = assetID,
                        EventReceiveID = (ushort)EventBFBB.ScenePrepare,
                        EventSendID = (ushort)EventBFBB.Play
                    }
                };
            }
            else if (template == AssetTemplate.SFX_OnEvent)
            {
                Flags08.FlagValueByte = 0x01;
                Flags09.FlagValueByte = 0xC0;
            }

            MinFrequency = 1f;
            Priority = 128;
            Volume = 91;
            InnerRadius = 5f;
            OuterRadius = 10f;

            if (template == AssetTemplate.Cauldron_Sfx)
            {
                Flags08.FlagValueByte = 0x01;
                Flags09.FlagValueByte = 0xCA;
                Sound = "cauldron_loop1";
                Volume = 100;
                InnerRadius = 20f;
                OuterRadius = 0f;
                _links = new Link[]
                {
                    new Link(game)
                    {
                        TargetAsset = assetID,
                        EventReceiveID = (ushort)EventScooby.ScenePrepare,
                        EventSendID = (ushort)EventScooby.Play
                    }
                };
            }
        }

        public AssetSFX(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using var reader = new EndianBinaryReader(AHDR.data, endianness);
            reader.BaseStream.Position = baseHeaderEndPosition;

            Flags08.FlagValueByte = reader.ReadByte();
            Flags09.FlagValueByte = reader.ReadByte();
            Frequency = reader.ReadInt16();
            MinFrequency = reader.ReadSingle();
            Sound = reader.ReadUInt32();
            Attach = reader.ReadUInt32();
            LoopCount = reader.ReadByte();
            Priority = reader.ReadByte();
            Volume = reader.ReadByte();
            reader.ReadByte();
            _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            _radius = reader.ReadSingle();
            _radius2 = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Flags08.FlagValueByte);
            writer.Write(Flags09.FlagValueByte);
            writer.Write(Frequency);
            writer.Write(MinFrequency);
            writer.Write(Sound);
            writer.Write(Attach);
            writer.Write(LoopCount);
            writer.Write(Priority);
            writer.Write(Volume);
            writer.Write((byte)0);
            writer.Write(_position.X);
            writer.Write(_position.Y);
            writer.Write(_position.Z);
            writer.Write(_radius);
            writer.Write(_radius2);
            SerializeLinks(writer);
        }

        private Matrix world;
        private BoundingBox boundingBox;

        public static bool dontRender = false;

        [Browsable(false)]
        public bool SpecialBlendMode(SharpRenderer renderer) => true;

        private BoundingSphere boundingSphere;
        private BoundingSphere boundingSphere2;

        public void CreateTransformMatrix(SharpRenderer renderer)
        {
            world = Matrix.Scaling(_radius * 2f) * Matrix.Translation(_position);

            CreateBoundingBox();
        }

        protected void CreateBoundingBox()
        {
            boundingSphere = new BoundingSphere(_position, _radius);
            boundingSphere2 = new BoundingSphere(_position, _radius2);
            boundingBox = BoundingBox.FromSphere(boundingSphere);
        }

        public float? GetIntersectionPosition(SharpRenderer renderer, Ray ray)
        {
            if (ShouldDraw(renderer) && ray.Intersects(ref boundingSphere))
                return TriangleIntersection(ray, renderer.sphereTriangles, renderer.sphereVertices, world);
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
            if (AssetMODL.renderBasedOnLodt && GetDistanceFrom(renderer.Camera.Position) > SharpRenderer.DefaultLODTDistance)
                return false;

            return renderer.frustum.Intersects(ref boundingSphere);
        }

        public void Draw(SharpRenderer renderer)
        {
            renderer.DrawSphere(boundingSphere, isSelected, renderer.sfxColor);

            if (isSelected)
                renderer.DrawSphere(boundingSphere2, false, renderer.sfxColor);
        }

        public BoundingBox GetBoundingBox() => boundingBox;

        public float GetDistanceFrom(Vector3 cameraPosition) => Vector3.Distance(cameraPosition, _position) - _radius;

        [Browsable(false)]
        public AssetSingle ScaleX
        {
            get => InnerRadius;
            set
            {
                OuterRadius += value - InnerRadius;
                InnerRadius = value;
            }
        }
        [Browsable(false)]
        public AssetSingle ScaleY
        {
            get => InnerRadius;
            set
            {
                OuterRadius += value - InnerRadius;
                InnerRadius = value;
            }
        }
        [Browsable(false)]
        public AssetSingle ScaleZ
        {
            get => InnerRadius;
            set
            {
                OuterRadius += value - InnerRadius;
                InnerRadius = value;
            }
        }

        public Transformation GetTransformation() => new Transformation()
        {
            _positionX = _position.X,
            _positionY = _position.Y,
            _positionZ = _position.Z,
            _scaleSingle = _radius,
            _scaleSfxMin = _radius,
            _scaleSfxMax = _radius2,
        };

        public void SetTransformation(Transformation transformation)
        {
            _position.X = transformation._positionX;
            _position.Y = transformation._positionY;
            _position.Z = transformation._positionZ;
            _radius = transformation._scaleSfxMin;
            _radius2 = transformation._scaleSfxMax;
        }
    }
}