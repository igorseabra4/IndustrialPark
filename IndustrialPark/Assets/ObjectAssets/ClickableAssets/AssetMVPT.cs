using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System;
using System.Linq;
using System.ComponentModel;
using IndustrialPark.Models;

namespace IndustrialPark
{
    public class AssetMVPT : ObjectAsset, IRenderableAsset, IClickableAsset
    {
        private Matrix world;
        private BoundingBox boundingBox;

        public static bool dontRender = false;
        
        protected override int getEventStartOffset()
        {
            return 0x74 + ReadShort(0x1A) * 4;
        }

        public AssetMVPT(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public void Setup(SharpRenderer renderer)
        {
            _position = new Vector3(ReadFloat(0x8), ReadFloat(0xC), ReadFloat(0x10));
            _scale = new Vector3(ReadFloat(0x1C), ReadFloat(0x20), ReadFloat(0x24));

            CreateTransformMatrix();

            if (!ArchiveEditorFunctions.renderableAssetSet.Contains(this))
                ArchiveEditorFunctions.renderableAssetSet.Add(this);
        }

        public void CreateTransformMatrix()
        {
            world = Matrix.Translation(_position);

            CreateBoundingBox();
        }

        protected void CreateBoundingBox()
        {
            boundingBox = BoundingBox.FromPoints(SharpRenderer.cubeVertices.ToArray());
            boundingBox.Maximum = (Vector3)Vector3.Transform(boundingBox.Maximum, world);
            boundingBox.Minimum = (Vector3)Vector3.Transform(boundingBox.Minimum, world);
        }

        public float? IntersectsWith(Ray ray)
        {
            if (ray.Intersects(ref boundingBox, out float distance))
                return TriangleIntersection(ray, distance, SharpRenderer.cubeTriangles, SharpRenderer.cubeVertices);
            return null;
        }
        
        private float? TriangleIntersection(Ray ray, float initialDistance, List<Triangle> triangles, List<Vector3> vertices)
        {
            bool hasIntersected = false;
            float smallestDistance = 1000f;

            foreach (Triangle t in triangles)
            {
                Vector3 v1 = (Vector3)Vector3.Transform(vertices[t.vertex1], world);
                Vector3 v2 = (Vector3)Vector3.Transform(vertices[t.vertex2], world);
                Vector3 v3 = (Vector3)Vector3.Transform(vertices[t.vertex3], world);

                if (ray.Intersects(ref v1, ref v2, ref v3, out float distance))
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

        public void Draw(SharpRenderer renderer)
        {
            if (dontRender) return;

            renderer.DrawCube(world, isSelected);
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

        private Vector3 _position;
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

        private Vector3 _scale;
        public float ScaleX
        {
            get { return _scale.X; }
            set
            {
                _scale.X = value;
                Write(0x1C, _scale.X);
                CreateTransformMatrix();
            }
        }

        public float ScaleY
        {
            get { return _position.Y; }
            set
            {
                _scale.Y = value;
                Write(0x20, _scale.Y);
                CreateTransformMatrix();
            }
        }

        public float ScaleZ
        {
            get { return _position.Z; }
            set
            {
                _scale.Z = value;
                Write(0x24, _scale.Z);
                CreateTransformMatrix();
            }
        }

        public AssetID[] OtherMVPTAssetIDs
        {
            get
            {
                List<AssetID> _otherMVPTs = new List<AssetID>();
                short amount = ReadShort(0x1A);
                for (int i = 0; i < amount; i++)
                    _otherMVPTs.Add(ReadUInt(0x28 + 4 * i));

                return _otherMVPTs.ToArray();
            }
            set
            {
                List<AssetID> newValues = value.ToList();
                short oldAmountOfPairs = ReadShort(0x1A);

                List<byte> newData = Data.Take(0x28).ToList();
                List<byte> restOfOldData = Data.Skip(0x28 + 4 * oldAmountOfPairs).ToList();

                foreach (AssetID i in newValues)
                    newData.AddRange(BitConverter.GetBytes(i).Reverse());

                newData.AddRange(restOfOldData);
                newData[0x1A] = BitConverter.GetBytes((short)newValues.Count)[1];
                newData[0x1B] = BitConverter.GetBytes((short)newValues.Count)[0];

                Data = newData.ToArray();
            }
        }
    }
}