using HipHopFile;
using SharpDX;
using System;
using System.ComponentModel;

namespace IndustrialPark
{
    public abstract class PlaceableAsset : ObjectAsset, IRenderableAsset, IClickableAsset
    {
        protected Matrix world;
        protected BoundingBox boundingBox;

        public PlaceableAsset(Section_AHDR AHDR) : base(AHDR) { }

        public virtual void Setup()
        {
            _rotation = new Vector3(ReadFloat(0x18 + Offset), ReadFloat(0x14 + Offset), ReadFloat(0x1C + Offset));
            _position = new Vector3(ReadFloat(0x20 + Offset), ReadFloat(0x24 + Offset), ReadFloat(0x28 + Offset));
            _scale = new Vector3(ReadFloat(0x2C + Offset), ReadFloat(0x30 + Offset), ReadFloat(0x34 + Offset));
            _color = new Vector4(ReadFloat(0x38 + Offset), ReadFloat(0x3c + Offset), ReadFloat(0x40 + Offset), ReadFloat(0x44 + Offset));

            _modelAssetID = ReadUInt(0x4C + Offset);

            CreateTransformMatrix();
            if (!ArchiveEditorFunctions.renderableAssetSet.Contains(this))
                ArchiveEditorFunctions.renderableAssetSet.Add(this);
        }

        public virtual void CreateTransformMatrix()
        {
            world = Matrix.Scaling(_scale)
            * Matrix.RotationY(_rotation.Y)
            * Matrix.RotationX(_rotation.X)
            * Matrix.RotationZ(_rotation.Z)
            * Matrix.Translation(_position);

            CreateBoundingBox();
        }

        protected virtual void CreateBoundingBox()
        {
            try
            {
                boundingBox = BoundingBox.FromPoints(ArchiveEditorFunctions.renderingDictionary[_modelAssetID].GetRenderWareModelFile().GetVertexList().ToArray());
            }
            catch
            {
                boundingBox = BoundingBox.FromPoints(SharpRenderer.cubeVertices.ToArray());
            }

            boundingBox.Maximum = (Vector3)Vector3.Transform(boundingBox.Maximum, world);
            boundingBox.Minimum = (Vector3)Vector3.Transform(boundingBox.Minimum, world);
        }

        protected abstract bool DontRender();

        public virtual void Draw(SharpRenderer renderer)
        {
            if (DontRender()) return;

            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(_modelAssetID))
                ArchiveEditorFunctions.renderingDictionary[_modelAssetID].Draw(renderer, world, isSelected ? renderer.selectedObjectColor * _color : _color);
            else
                renderer.DrawCube(world, isSelected);
        }

        public virtual float? IntersectsWith(Ray ray)
        {
            if (DontRender())
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

        public virtual Vector3 GetGizmoCenter()
        {
            return boundingBox.Center;
        }

        public virtual float GetGizmoRadius()
        {
            return Math.Max(Math.Max(boundingBox.Size.X, boundingBox.Size.Y), boundingBox.Size.Z) * 0.9f;
        }

        public BoundingBox GetBoundingBox()
        {
            return boundingBox;
        }

        [Category("Flags"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte VisibilityFlag
        {
            get { return ReadByte(0x8); }
            set { Write(0x8, value); }
        }

        [Category("Flags"), ReadOnly(true), TypeConverter(typeof(HexByteTypeConverter))]
        public byte TypeFlag
        {
            get { return ReadByte(0x9); }
            set { Write(0x9, value); }
        }

        [Category("Flags"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte UnknownFlag0A
        {
            get { return ReadByte(0xA); }
            set { Write(0xA, value); }
        }

        [Category("Flags"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte SolidityFlag
        {
            get { return ReadByte(0xB); }
            set { Write(0xB, value); }
        }

        [Category("Placement")]
        public AssetID UnknownAssetID_C
        {
            get { return ReadUInt(0xC); }
            set { Write(0xC, value); }
        }

        [Category("Placement")]
        public AssetID SURF_AssetID
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

        protected Vector3 _rotation;

        [Category("Placement")]
        [TypeConverter(typeof(FloatTypeConverter))]
        public float RotationX
        {
            get { return MathUtil.RadiansToDegrees(_rotation.X); }
            set
            {
                _rotation.X = MathUtil.DegreesToRadians(value);
                Write(0x18 + Offset, _rotation.X);
                CreateTransformMatrix();
            }
        }

        [Category("Placement")]
        [TypeConverter(typeof(FloatTypeConverter))]
        public float RotationY
        {
            get { return MathUtil.RadiansToDegrees(_rotation.Y); }
            set
            {
                _rotation.Y = MathUtil.DegreesToRadians(value);
                Write(0x14 + Offset, _rotation.Y);
                CreateTransformMatrix();
            }
        }

        [Category("Placement")]
        [TypeConverter(typeof(FloatTypeConverter))]
        public float RotationZ
        {
            get { return MathUtil.RadiansToDegrees(_rotation.Z); }
            set
            {
                _rotation.Z = MathUtil.DegreesToRadians(value);
                Write(0x1C + Offset, _rotation.Z);
                CreateTransformMatrix();
            }
        }

        protected Vector3 _scale;

        [Category("Placement")]
        [TypeConverter(typeof(FloatTypeConverter))]
        public float ScaleX
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
        public float ScaleY
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
        public float ScaleZ
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

        [Category("Placement")]
        public float ColorRed
        {
            get => ReadFloat(0x38 + Offset);
            set
            {
                _color.X = value;
                Write(0x38 + Offset, _color.X);
            }
        }

        [Category("Placement")]
        public float ColorGreen
        {
            get { return ReadFloat(0x3C + Offset); }
            set
            {
                _color.Y = value;
                Write(0x3C + Offset, _color.Y);
            }
        }

        [Category("Placement")]
        public float ColorBlue
        {
            get { return ReadFloat(0x40 + Offset); }
            set
            {
                _color.Z = value;
                Write(0x40 + Offset, _color.Z);
            }
        }

        [Category("Placement")]
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
        [Category("Placement")]
        public AssetID ModelAssetID
        {
            get { return _modelAssetID; }
            set
            {
                _modelAssetID = value;
                Write(0x4C + Offset, _modelAssetID);
            }
        }

        [Category("Placement")]
        public AssetID UnknownAssetID_50
        {
            get { return ReadUInt(0x50 + Offset); }
            set { Write(0x50 + Offset, value); }
        }
    }
}