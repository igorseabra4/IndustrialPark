using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System;
using System.Linq;
using System.ComponentModel;
using IndustrialPark.Models;
using static HipHopFile.Functions;

namespace IndustrialPark
{
    public class AssetMVPT : ObjectAsset, IRenderableAsset, IClickableAsset
    {
        private Matrix world;
        private BoundingBox boundingBox;

        public static bool dontRender = false;

        protected override int EventStartOffset
        {
            get => 0x28 + 4 * SiblingAmount;
        }

        public AssetMVPT(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public void Setup(SharpRenderer renderer)
        {
            _position = new Vector3(ReadFloat(0x8), ReadFloat(0xC), ReadFloat(0x10));
            _distanceICanSeeYou = ReadFloat(0x24);

            CreateTransformMatrix();

            if (!ArchiveEditorFunctions.renderableAssetSet.Contains(this))
                ArchiveEditorFunctions.renderableAssetSet.Add(this);
        }

        public void CreateTransformMatrix()
        {
            if (_distanceICanSeeYou == -1f)
                world = Matrix.RotationX(MathUtil.PiOverTwo) * Matrix.Translation(_position + new Vector3(0f, 0.5f, 0f));
            else
                world = Matrix.Scaling(_distanceICanSeeYou * 2f) * Matrix.Translation(_position);

            CreateBoundingBox();
        }

        public BoundingSphere boundingSphere;

        protected void CreateBoundingBox()
        {
            if (_distanceICanSeeYou == -1f)
            {
                boundingBox = BoundingBox.FromPoints(SharpRenderer.pyramidVertices.ToArray());
                boundingBox.Maximum = (Vector3)Vector3.Transform(boundingBox.Maximum, world);
                boundingBox.Minimum = (Vector3)Vector3.Transform(boundingBox.Minimum, world);
                boundingSphere = BoundingSphere.FromBox(boundingBox);
            }
            else
            {
                boundingSphere = new BoundingSphere(_position, _distanceICanSeeYou);
                boundingBox = BoundingBox.FromSphere(boundingSphere);
            }
        }

        public float? IntersectsWith(Ray ray)
        {
            if (dontRender)
                return null;

            if (ray.Intersects(ref boundingSphere, out float distance2))
                    return TriangleIntersection(ray, SharpRenderer.sphereTriangles, SharpRenderer.sphereVertices);
            return null;
        }

        private float? TriangleIntersection(Ray r, List<Triangle> triangles, List<Vector3> vertices)
        {
            bool hasIntersected = false;
            float smallestDistance = 1000f;

            foreach (Triangle t in triangles)
            {
                Vector3 v1 = (Vector3)Vector3.Transform(vertices[t.vertex1], world);
                Vector3 v2 = (Vector3)Vector3.Transform(vertices[t.vertex2], world);
                Vector3 v3 = (Vector3)Vector3.Transform(vertices[t.vertex3], world);

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

        public void Draw(SharpRenderer renderer)
        {
            if (dontRender) return;

            if (_distanceICanSeeYou == -1f)
                renderer.DrawPyramid(world, isSelected, 1f);
            else
                renderer.DrawSphere(world, isSelected);
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

        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte UnknownByte14
        {
            get => ReadByte(0x14);
            set => Write(0x14, value);
        }

        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte UnknownByte15
        {
            get => ReadByte(0x15);
            set => Write(0x15, value);
        }

        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte UnknownByte16
        {
            get => ReadByte(0x16);
            set => Write(0x16, value);
        }

        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte PointType
        {
            get => ReadByte(0x17);
            set => Write(0x17, value);
        }

        [ReadOnly(true)]
        public int SiblingAmount
        {
            get => ReadInt(0x18);
            set => Write(0x18, value);
        }

        public float MovementAngle
        {
            get => ReadFloat(0x1C);
            set => Write(0x1C, value);
        }

        public float MovementRadius
        {
            get => ReadFloat(0x20);
            set => Write(0x20, value);
        }

        private float _distanceICanSeeYou;
        public float DistanceICanSeeYou
        {
            get => _distanceICanSeeYou;
            set
            {
                _distanceICanSeeYou = value;
                Write(0x24, _distanceICanSeeYou);
                CreateTransformMatrix();
            }
        }

        public AssetID[] SiblingMVPTs
        {
            get
            {
                AssetID[] _otherMVPTs = new AssetID[SiblingAmount];
                for (int i = 0; i < SiblingAmount; i++)
                    _otherMVPTs[i] = ReadUInt(0x28 + 4 * i);

                return _otherMVPTs;
            }
            set
            {
                List<byte> newData = Data.Take(0x28).ToList();
                List<byte> restOfOldData = Data.Skip(0x28 + 4 * SiblingAmount).ToList();

                foreach (AssetID i in value)
                {
                    if (currentPlatform == Platform.GameCube)
                        newData.AddRange(BitConverter.GetBytes(i).Reverse());
                    else
                        newData.AddRange(BitConverter.GetBytes(i));
                }

                newData.AddRange(restOfOldData);
                
                Data = newData.ToArray();

                SiblingAmount = value.Length;
            }
        }
    }
}