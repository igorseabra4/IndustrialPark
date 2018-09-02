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

        public virtual void Setup(SharpRenderer renderer)
        {
            _rotation = new Vector3(ReadFloat(0x18), ReadFloat(0x14), ReadFloat(0x1C));
            _position = new Vector3(ReadFloat(0x20), ReadFloat(0x24), ReadFloat(0x28));
            _scale = new Vector3(ReadFloat(0x2C), ReadFloat(0x30), ReadFloat(0x34));

            _modelAssetID = ReadUInt(0x4C);

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
                ArchiveEditorFunctions.renderingDictionary[_modelAssetID].Draw(renderer, world, isSelected);
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

        [Category("Placement")]
        public byte VisibilityFlag
        {
            get { return ReadByte(0x8); }
            set { Write(0x8, value); }
        }

        [Category("Placement")]
        public byte UnknownFlag09
        {
            get { return ReadByte(0x9); }
            set { Write(0x9, value); }
        }

        [Category("Placement")]
        public byte UnknownFlag0A
        {
            get { return ReadByte(0xA); }
            set { Write(0xA, value); }
        }

        [Category("Placement")]
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
        public AssetID UnknownAssetID_10
        {
            get { return ReadUInt(0x10); }
            set { Write(0x10, value); }
        }
        
        protected Vector3 _position;
        [Category("Placement")]
        public virtual float PositionX
        {
            get { return _position.X; }
            set
            {
                _position.X = value;
                Write(0x20, _position.X);
                CreateTransformMatrix();
            }
        }

        [Category("Placement")]
        public virtual float PositionY
        {
            get { return _position.Y; }
            set
            {
                _position.Y = value;
                Write(0x24, _position.Y);
                CreateTransformMatrix();
            }
        }

        [Category("Placement")]
        public virtual float PositionZ
        {
            get { return _position.Z; }
            set
            {
                _position.Z = value;
                Write(0x28, _position.Z);
                CreateTransformMatrix();
            }
        }

        protected Vector3 _rotation;
        [Category("Placement")]
        public float RotationX
        {
            get { return MathUtil.RadiansToDegrees(_rotation.X); }
            set
            {
                _rotation.X = MathUtil.DegreesToRadians(value);
                Write(0x18, _rotation.X);
                CreateTransformMatrix();
            }
        }

        [Category("Placement")]
        public float RotationY
        {
            get { return MathUtil.RadiansToDegrees(_rotation.Y); }
            set
            {
                _rotation.Y = MathUtil.DegreesToRadians(value);
                Write(0x14, _rotation.Y);
                CreateTransformMatrix();
            }
        }

        [Category("Placement")]
        public float RotationZ
        {
            get { return MathUtil.RadiansToDegrees(_rotation.Z); }
            set
            {
                _rotation.Z = MathUtil.DegreesToRadians(value);
                Write(0x1C, _rotation.Z);
                CreateTransformMatrix();
            }
        }

        protected Vector3 _scale;
        [Category("Placement")]
        public float ScaleX
        {
            get { return _scale.X; }
            set
            {
                _scale.X = value;
                Write(0x2C, _scale.X);
                CreateTransformMatrix();
            }
        }

        [Category("Placement")]
        public float ScaleY
        {
            get { return _scale.Y; }
            set
            {
                _scale.Y = value;
                Write(0x30, _scale.Y);
                CreateTransformMatrix();
            }
        }

        [Category("Placement")]
        public float ScaleZ
        {
            get { return _scale.Z; }
            set
            {
                _scale.Z = value;
                Write(0x34, _scale.Z);
                CreateTransformMatrix();
            }
        }

        [Category("Placement")]
        public float ColorX
        {
            get { return ReadFloat(0x38); }
            set { Write(0x38, value); }
        }

        [Category("Placement")]
        public float ColorY
        {
            get { return ReadFloat(0x3C); }
            set { Write(0x3C, value); }
        }

        [Category("Placement")]
        public float ColorZ
        {
            get { return ReadFloat(0x40); }
            set { Write(0x40, value); }
        }

        [Category("Placement")]
        public float ColorAlpha
        {
            get { return ReadFloat(0x44); }
            set { Write(0x44, value); }
        }

        [Category("Placement")]
        public float UnknownFloat48
        {
            get { return ReadFloat(0x48); }
            set { Write(0x48, value); }
        }

        protected uint _modelAssetID;
        [Category("Placement")]
        public AssetID ModelAssetID
        {
            get { return _modelAssetID; }
            set
            {
                _modelAssetID = value;
                Write(0x4C, _modelAssetID);
            }
        }

        [Category("Placement")]
        public AssetID UnknownAssetID_50
        {
            get { return ReadUInt(0x50); }
            set { Write(0x50, value); }
        }

        [Browsable(false), Category("Placement")]
        public Vector3 Position { get => _position; set { } }
        [Browsable(false), Category("Placement")]
        public Vector3 Scale { get => _scale; set { } }
    }
}