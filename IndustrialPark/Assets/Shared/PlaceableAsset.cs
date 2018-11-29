using HipHopFile;
using SharpDX;
using System;
using System.ComponentModel;

namespace IndustrialPark
{
    public abstract class PlaceableAsset : ObjectAsset, IRenderableAsset, IClickableAsset, IRotatableAsset, IScalableAsset
    {
        protected Matrix world;
        protected BoundingBox boundingBox;

        protected abstract bool DontRender { get; }

        public PlaceableAsset(Section_AHDR AHDR) : base(AHDR) { }

        public virtual void Setup()
        {
            _yaw = ReadFloat(0x14 + Offset);
            _pitch = ReadFloat(0x18 + Offset);
            _roll = ReadFloat(0x1C + Offset);
            _position = new Vector3(ReadFloat(0x20 + Offset), ReadFloat(0x24 + Offset), ReadFloat(0x28 + Offset));
            _scale = new Vector3(ReadFloat(0x2C + Offset), ReadFloat(0x30 + Offset), ReadFloat(0x34 + Offset));
            _color = new Vector4(ReadFloat(0x38 + Offset), ReadFloat(0x3c + Offset), ReadFloat(0x40 + Offset), ReadFloat(0x44 + Offset));

            _modelAssetID = ReadUInt(0x4C + Offset);

            CreateTransformMatrix();
            if (!ArchiveEditorFunctions.renderableAssetSetCommon.Contains(this))
                ArchiveEditorFunctions.renderableAssetSetCommon.Add(this);
        }

        public virtual void CreateTransformMatrix()
        {
            world = Matrix.Scaling(_scale)
                * Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll)
                * Matrix.Translation(_position);

            CreateBoundingBox();
        }

        protected virtual void CreateBoundingBox()
        {
            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(_modelAssetID) &&
                ArchiveEditorFunctions.renderingDictionary[_modelAssetID].HasRenderWareModelFile() &&
                ArchiveEditorFunctions.renderingDictionary[_modelAssetID].GetRenderWareModelFile() != null)
                boundingBox = BoundingBox.FromPoints(ArchiveEditorFunctions.renderingDictionary[_modelAssetID].GetRenderWareModelFile().vertexListG.ToArray());
            else
                boundingBox = BoundingBox.FromPoints(SharpRenderer.cubeVertices.ToArray());
            
            boundingBox.Maximum = (Vector3)Vector3.Transform(boundingBox.Maximum, world);
            boundingBox.Minimum = (Vector3)Vector3.Transform(boundingBox.Minimum, world);
        }

        public virtual void Draw(SharpRenderer renderer)
        {
            if (DontRender) return;

            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(_modelAssetID))
                ArchiveEditorFunctions.renderingDictionary[_modelAssetID].Draw(renderer, world, isSelected ? renderer.selectedObjectColor * _color : _color);
            else
                renderer.DrawCube(world, isSelected);
        }

        public virtual float? IntersectsWith(Ray ray)
        {
            if (DontRender)
                return null;

            if (ray.Intersects(ref boundingBox, out float distance))
                return TriangleIntersection(ray, distance);
            return null;
        }
        
        protected virtual float? TriangleIntersection(Ray r, float initialDistance)
        {
            bool hasIntersected = false;
            float smallestDistance = 1000f;

            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(_modelAssetID))
            {
                RenderWareModelFile rwmf;

                if (ArchiveEditorFunctions.renderingDictionary[_modelAssetID] is AssetMINF MINF)
                {
                    if (MINF.HasRenderWareModelFile())
                        rwmf = ArchiveEditorFunctions.renderingDictionary[_modelAssetID].GetRenderWareModelFile();
                    else return initialDistance;
                }
                else rwmf = ArchiveEditorFunctions.renderingDictionary[_modelAssetID].GetRenderWareModelFile();

                foreach (RenderWareFile.Triangle t in rwmf.triangleList)
                {
                    Vector3 v1 = (Vector3)Vector3.Transform(rwmf.vertexListG[t.vertex1], world);
                    Vector3 v2 = (Vector3)Vector3.Transform(rwmf.vertexListG[t.vertex2], world);
                    Vector3 v3 = (Vector3)Vector3.Transform(rwmf.vertexListG[t.vertex3], world);

                    if (r.Intersects(ref v1, ref v2, ref v3, out float distance))
                    {
                        hasIntersected = true;

                        if (distance < smallestDistance)
                            smallestDistance = distance;
                    }
                }

                if (hasIntersected)
                    return smallestDistance;
                else return null;
            }

            return initialDistance;
        }

        public BoundingSphere GetObjectCenter()
        {
            BoundingSphere boundingSphere = new BoundingSphere(_position, boundingBox.Size.Length());
            boundingSphere.Radius *= 0.9f;
            return boundingSphere;
        }

        public BoundingSphere GetGizmoCenter()
        {
            BoundingSphere boundingSphere = BoundingSphere.FromBox(boundingBox);
            boundingSphere.Radius *= 0.9f;
            return boundingSphere;
        }

        public BoundingBox GetBoundingBox()
        {
            return boundingBox;
        }

        public virtual float GetDistance(Vector3 cameraPosition)
        {
            return Vector3.Distance(cameraPosition, _position);
        }

        public override bool HasReference(uint assetID)
        {
            if (SurfaceAssetID == assetID)
                return true;
            if (ModelAssetID == assetID)
                return true;
            if (ReferenceAssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        [Category("Placement Flags"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte VisibilityFlag
        {
            get { return ReadByte(0x8); }
            set { Write(0x8, value); }
        }

        [Category("Placement Flags")]
        public bool Visible
        {
            get => (VisibilityFlag & Mask(0)) != 0;
            set => VisibilityFlag = (byte)(value ? (VisibilityFlag | Mask(0)) : (VisibilityFlag & InvMask(0)));
        }

        [Category("Placement Flags")]
        public bool UseGravity
        {
            get => (VisibilityFlag & Mask(1)) != 0;
            set => VisibilityFlag = (byte)(value ? (VisibilityFlag | Mask(1)) : (VisibilityFlag & InvMask(1)));
        }

        [Category("Placement Flags"), ReadOnly(true), TypeConverter(typeof(HexByteTypeConverter))]
        public byte TypeFlag
        {
            get { return ReadByte(0x9); }
            set { Write(0x9, value); }
        }

        [Category("Placement Flags"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte UnknownFlag0A
        {
            get { return ReadByte(0xA); }
            set { Write(0xA, value); }
        }

        [Category("Placement Flags"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte SolidityFlag
        {
            get { return ReadByte(0xB); }
            set { Write(0xB, value); }
        }

        [Category("Placement Flags")]
        public bool PreciseCollision
        {
            get => (SolidityFlag & Mask(1)) != 0;
            set => SolidityFlag = (byte)(value ? (VisibilityFlag | Mask(1)) : (VisibilityFlag & InvMask(1)));
        }

        [Category("Placement")]
        public int UnknownIntC
        {
            get { return ReadInt(0xC); }
            set { Write(0xC, value); }
        }

        [Category("Placement References")]
        public AssetID SurfaceAssetID
        {
            get { return ReadUInt(0x10 + Offset); }
            set { Write(0x10 + Offset, value); }
        }
        
        protected Vector3 _position;

        [Category("Placement")]
        [TypeConverter(typeof(FloatTypeConverter))]
        public virtual float PositionX
        {
            get { return _position.X; }
            set
            {
                _position.X = value;
                Write(0x20 + Offset, _position.X);
                CreateTransformMatrix();
            }
        }

        [Category("Placement")]
        [TypeConverter(typeof(FloatTypeConverter))]
        public virtual float PositionY
        {
            get { return _position.Y; }
            set
            {
                _position.Y = value;
                Write(0x24 + Offset, _position.Y);
                CreateTransformMatrix();
            }
        }

        [Category("Placement")]
        [TypeConverter(typeof(FloatTypeConverter))]
        public virtual float PositionZ
        {
            get { return _position.Z; }
            set
            {
                _position.Z = value;
                Write(0x28 + Offset, _position.Z);
                CreateTransformMatrix();
            }
        }

        protected float _yaw;
        protected float _pitch;
        protected float _roll;

        [Category("Placement")]
        [TypeConverter(typeof(FloatTypeConverter))]
        public virtual float Yaw
        {
            get { return MathUtil.RadiansToDegrees(_yaw); }
            set
            {
                _yaw = MathUtil.DegreesToRadians(value);
                Write(0x14 + Offset, _yaw);
                CreateTransformMatrix();
            }
        }

        [Category("Placement")]
        [TypeConverter(typeof(FloatTypeConverter))]
        public virtual float Pitch
        {
            get { return MathUtil.RadiansToDegrees(_pitch); }
            set
            {
                _pitch = MathUtil.DegreesToRadians(value);
                Write(0x18 + Offset, _pitch);
                CreateTransformMatrix();
            }
        }

        [Category("Placement")]
        [TypeConverter(typeof(FloatTypeConverter))]
        public virtual float Roll
        {
            get { return MathUtil.RadiansToDegrees(_roll); }
            set
            {
                _roll = MathUtil.DegreesToRadians(value);
                Write(0x1C + Offset, _roll);
                CreateTransformMatrix();
            }
        }

        protected Vector3 _scale;

        [Category("Placement")]
        [TypeConverter(typeof(FloatTypeConverter))]
        public virtual float ScaleX
        {
            get { return _scale.X; }
            set
            {
                _scale.X = value;
                Write(0x2C + Offset, _scale.X);
                CreateTransformMatrix();
            }
        }

        [Category("Placement")]
        [TypeConverter(typeof(FloatTypeConverter))]
        public virtual float ScaleY
        {
            get { return _scale.Y; }
            set
            {
                _scale.Y = value;
                Write(0x30 + Offset, _scale.Y);
                CreateTransformMatrix();
            }
        }

        [Category("Placement")]
        [TypeConverter(typeof(FloatTypeConverter))]
        public virtual float ScaleZ
        {
            get { return _scale.Z; }
            set
            {
                _scale.Z = value;
                Write(0x34 + Offset, _scale.Z);
                CreateTransformMatrix();
            }
        }

        protected Vector4 _color;

        [Category("Placement Color")]
        public float ColorRed
        {
            get => ReadFloat(0x38 + Offset);
            set
            {
                _color.X = value;
                Write(0x38 + Offset, _color.X);
            }
        }

        [Category("Placement Color")]
        public float ColorGreen
        {
            get { return ReadFloat(0x3C + Offset); }
            set
            {
                _color.Y = value;
                Write(0x3C + Offset, _color.Y);
            }
        }

        [Category("Placement Color")]
        public float ColorBlue
        {
            get { return ReadFloat(0x40 + Offset); }
            set
            {
                _color.Z = value;
                Write(0x40 + Offset, _color.Z);
            }
        }

        [Category("Placement Color")]
        public float ColorAlpha
        {
            get { return ReadFloat(0x44 + Offset); }
            set
            {
                _color.W = value;
                Write(0x44 + Offset, _color.W);
            }
        }

        [Category("Placement")]
        public float UnknownFloat48
        {
            get { return ReadFloat(0x48 + Offset); }
            set { Write(0x48 + Offset, value); }
        }

        protected uint _modelAssetID;
        [Category("Placement References")]
        public AssetID ModelAssetID
        {
            get { return _modelAssetID; }
            set
            {
                _modelAssetID = value;
                Write(0x4C + Offset, _modelAssetID);
            }
        }

        [Category("Placement References")]
        public virtual AssetID ReferenceAssetID
        {
            get { return ReadUInt(0x50 + Offset); }
            set { Write(0x50 + Offset, value); }
        }
    }
}