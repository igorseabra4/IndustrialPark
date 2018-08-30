using HipHopFile;
using SharpDX;
using System.Collections.Generic;

namespace IndustrialPark
{
    public class RenderableAsset : Asset
    {
        public RenderableAsset(Section_AHDR AHDR) : base(AHDR)
        {
        }

        protected Vector3 _position;
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

        public Vector3 Position
        {
            get { return _position; }
            set
            {
                PositionX = value.X;
                PositionY = value.Y;
                PositionZ = value.Z;
            }
        }

        protected Vector3 _rotation;
        public virtual Vector3 Rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;
                Write(0x18, _rotation.X);
                Write(0x14, _rotation.Y);
                Write(0x1C, _rotation.Z);
                CreateTransformMatrix();
            }
        }

        protected Vector3 _scale;
        public virtual Vector3 Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                Write(0x2C, _scale.X);
                Write(0x30, _scale.Y);
                Write(0x34, _scale.Z);
                CreateTransformMatrix();
            }
        }
        
        public Matrix world;

        protected uint _modelAssetID;
        public virtual uint ModelAssetID
        {
            get { return _modelAssetID; }
            set
            {
                _modelAssetID = value;
                Write(0x4C, value);
            }
        }
        
        public override void Setup(SharpRenderer renderer, bool defaultMode = true)
        {
            _modelAssetID = ReadUInt(0x4C);

            _position = new Vector3(ReadFloat(0x20), ReadFloat(0x24), ReadFloat(0x28));
            _rotation = new Vector3(ReadFloat(0x18), ReadFloat(0x14), ReadFloat(0x1C));
            _scale = new Vector3(ReadFloat(0x2C), ReadFloat(0x30), ReadFloat(0x34));

            CreateTransformMatrix();

            ArchiveEditorFunctions.renderableAssetSet.Add(this);
        }

        public virtual void CreateTransformMatrix()
        {
            world = Matrix.Scaling(_scale)
            * Matrix.RotationY(_rotation.Y)
            * Matrix.RotationX(_rotation.X)
            * Matrix.RotationZ(_rotation.Z)
            * Matrix.Translation(_position);

            boundingBox = CreateBoundingBox();
            boundingBox.Maximum = (Vector3)Vector3.Transform(boundingBox.Maximum, world);
            boundingBox.Minimum = (Vector3)Vector3.Transform(boundingBox.Minimum, world);
        }

        protected virtual BoundingBox CreateBoundingBox()
        {
            List<Vector3> list = new List<Vector3>();
            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(_modelAssetID))
            {
                try
                {
                    list.AddRange(ArchiveEditorFunctions.renderingDictionary[_modelAssetID].GetRenderWareModelFile().GetVertexList());
                }
                catch
                {
                    return BoundingBox.FromPoints(SharpRenderer.cubeVertices.ToArray());
                }
            }
            else
                return BoundingBox.FromPoints(SharpRenderer.cubeVertices.ToArray());

            return BoundingBox.FromPoints(list.ToArray());
        }

        public virtual void Draw(SharpRenderer renderer)
        {
            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(_modelAssetID))
            {
                ArchiveEditorFunctions.renderingDictionary[_modelAssetID].Draw(renderer, world, isSelected);
            }
            else
            {
                renderer.DrawCube(world, isSelected);
            }
        }

        public BoundingBox boundingBox;

        public float? IntersectsWith(Ray ray)
        {
            if (ray.Intersects(ref boundingBox, out float distance))
            {
                return TriangleIntersection(ray, distance);
            }
            else return null;
        }

        private float? TriangleIntersection(Ray r, float initialDistance)
        {
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
                        return distance;
                }
            }
            else
                return initialDistance;

            return null;
        }
    }
}