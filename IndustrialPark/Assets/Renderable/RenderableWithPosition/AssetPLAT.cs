using HipHopFile;
using SharpDX;
using System.Collections.Generic;

namespace IndustrialPark
{
    public class AssetPLAT : RenderableAssetWithPosition
    {
        public static bool dontRender = false;

        public AssetPLAT(Section_AHDR AHDR) : base(AHDR)
        {
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

        public override void CreateTransformMatrix()
        {
            world = Matrix.Scaling(_scale)
            * Matrix.RotationY(_rotation.Y)
            * Matrix.RotationX(_rotation.X)
            * Matrix.RotationZ(_rotation.Z)
            * Matrix.Translation(_position);

            CreateBoundingBox();
        }

        protected override void CreateBoundingBox()
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
                
        public override void Draw(SharpRenderer renderer)
        {
            if (dontRender) return;

            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(_modelAssetID))
                ArchiveEditorFunctions.renderingDictionary[_modelAssetID].Draw(renderer, world, isSelected);
            else
                renderer.DrawCube(world, isSelected);
        }
        
        protected override float? TriangleIntersection(Ray r, float initialDistance)
        {
            if (dontRender)
                return null;

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
        
        public AssetID AssetID
        {
            get { return ReadUInt(0); }
            set { Write(0, value); }
        }

        private Vector3 _position;
        public override float PositionX
        {
            get { return _position.X; }
            set
            {
                _position.X = value;
                Write(0x20, _position.X);
                CreateTransformMatrix();
            }
        }

        public override float PositionY
        {
            get { return _position.Y; }
            set
            {
                _position.Y = value;
                Write(0x24, _position.Y);
                CreateTransformMatrix();
            }
        }

        public override float PositionZ
        {
            get { return _position.Z; }
            set
            {
                _position.Z = value;
                Write(0x28, _position.Z);
                CreateTransformMatrix();
            }
        }
        
        private Vector3 _rotation;
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

        private Vector3 _scale;
        public override float ScaleX
        {
            get { return _scale.X; }
            set
            {
                _scale.X = value;
                Write(0x2C, _scale.X);
                CreateTransformMatrix();
            }
        }

        public override float ScaleY
        {
            get { return _scale.Y; }
            set
            {
                _scale.Y = value;
                Write(0x30, _scale.Y);
                CreateTransformMatrix();
            }
        }

        public override float ScaleZ
        {
            get { return _scale.Z; }
            set
            {
                _scale.Z = value;
                Write(0x34, _scale.Z);
                CreateTransformMatrix();
            }
        }
        
        private uint _modelAssetID;
        public AssetID ModelAssetID
        {
            get { return _modelAssetID; }
            set
            {
                _modelAssetID = value;
                Write(0x4C, _modelAssetID);
            }
        }
    }
}