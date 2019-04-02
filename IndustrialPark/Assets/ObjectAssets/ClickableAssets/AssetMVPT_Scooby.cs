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
    public class AssetMVPT_Scooby : ObjectAsset, IRenderableAsset, IClickableAsset, IScalableAsset
    {
        private Matrix world;
        private BoundingBox boundingBox;

        public static bool dontRender = false;

        protected override int EventStartOffset => 0x20 + 4 * SiblingAmount;

        public AssetMVPT_Scooby(Section_AHDR AHDR) : base(AHDR)
        {
            _position = new Vector3(ReadFloat(0x8), ReadFloat(0xC), ReadFloat(0x10));
            _arenaRadius = ReadFloat(0x1C);

            CreateTransformMatrix();

            if (!ArchiveEditorFunctions.renderableAssetSetTrans.Contains(this))
                ArchiveEditorFunctions.renderableAssetSetTrans.Add(this);
        }

        public override bool HasReference(uint assetID)
        {
            foreach (AssetID a in NextMVPTs)
                if (a == assetID)
                    return true;

            return base.HasReference(assetID);
        }

        public void CreateTransformMatrix()
        {
            if (IsZone == 1 || _arenaRadius == -1f)
                world = Matrix.RotationX(MathUtil.PiOverTwo) * Matrix.Translation(_position + new Vector3(0f, 0.5f, 0f));
            else
                world = Matrix.Scaling(_arenaRadius * 2f) * Matrix.Translation(_position);

            CreateBoundingBox();
        }

        public BoundingSphere boundingSphere;

        protected void CreateBoundingBox()
        {
            if (IsZone == 1 || _arenaRadius == -1f)
            {
                Vector3[] vertices = new Vector3[SharpRenderer.pyramidVertices.Count];

                for (int i = 0; i < SharpRenderer.pyramidVertices.Count; i++)
                    vertices[i] = (Vector3)Vector3.Transform(SharpRenderer.pyramidVertices[i], world);

                boundingBox = BoundingBox.FromPoints(vertices);
                boundingSphere = BoundingSphere.FromBox(boundingBox);
            }
            else
            {
                boundingSphere = new BoundingSphere(_position, _arenaRadius);
                boundingBox = BoundingBox.FromSphere(boundingSphere);
            }
        }

        public float? IntersectsWith(Ray ray)
        {
            if (dontRender || isInvisible)
                return null;

            if (ray.Intersects(ref boundingSphere))
            {
                if (IsZone == 1 || _arenaRadius == -1f)
                    return TriangleIntersection(ray, SharpRenderer.pyramidTriangles, SharpRenderer.pyramidVertices);
                return TriangleIntersection(ray, SharpRenderer.sphereTriangles, SharpRenderer.sphereVertices);
            }
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
            if (dontRender || isInvisible) return;

            if (IsZone == 1 || _arenaRadius == -1f)
                renderer.DrawPyramid(world, isSelected, 1f);
            else
                renderer.DrawSphere(world, isSelected, renderer.mvptColor);
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

        public float GetDistance(Vector3 cameraPosition)
        {
            return Vector3.Distance(cameraPosition, _position) - (_arenaRadius == -1f ? 0 : _arenaRadius);
        }
        
        protected Vector3 _position;
        [Category("Move Point"), TypeConverter(typeof(FloatTypeConverter))]
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

        [Category("Move Point"), TypeConverter(typeof(FloatTypeConverter))]
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

        [Category("Move Point"), TypeConverter(typeof(FloatTypeConverter))]
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

        [Category("Move Point")]
        [TypeConverter(typeof(HexShortTypeConverter))]
        [Description("Usually 0x2710")]
        public short Wt
        {
            get => ReadShort(0x14);
            set => Write(0x14, value);
        }
        
        [Category("Move Point")]
        [TypeConverter(typeof(HexByteTypeConverter))]
        [Description("0x00 for arena (can see you), 0x01 for zone")]
        public byte IsZone
        {
            get => ReadByte(0x16);
            set => Write(0x16, value);
        }

        [Category("Move Point")]
        [TypeConverter(typeof(HexByteTypeConverter))]
        [Description("Usually 0x00")]
        public byte BezIndex
        {
            get => ReadByte(0x17);
            set => Write(0x17, value);
        }

        [Category("Move Point")]
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flg_Props
        {
            get => ReadByte(0x18);
            set => Write(0x18, value);
        }

        [Category("Move Point")]
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Padding19
        {
            get => ReadByte(0x19);
            set => Write(0x19, value);
        }

        [Category("Move Point")]
        [ReadOnly(true)]
        public short SiblingAmount
        {
            get => ReadShort(0x1A);
            set => Write(0x1A, value);
        }

        protected float _arenaRadius;
        [Category("Move Point"), TypeConverter(typeof(FloatTypeConverter))]
        public virtual float ArenaRadius
        {
            get => _arenaRadius;
            set
            {
                _arenaRadius = value;
                Write(0x1C, _arenaRadius);
                CreateTransformMatrix();
            }
        }

        protected virtual int NextStartOffset => 0x20;

        [Category("Move Point")]
        public AssetID[] NextMVPTs
        {
            get
            {
                try
                {
                    AssetID[] _otherMVPTs = new AssetID[SiblingAmount];
                    for (int i = 0; i < SiblingAmount; i++)
                        _otherMVPTs[i] = ReadUInt(NextStartOffset + 4 * i);

                    return _otherMVPTs;
                }
                catch
                {
                    return new AssetID[0];
                }
            }
            set
            {
                List<byte> newData = Data.Take(NextStartOffset).ToList();
                List<byte> restOfOldData = Data.Skip(NextStartOffset + 4 * SiblingAmount).ToList();

                foreach (AssetID i in value)
                {
                    if (currentPlatform == Platform.GameCube)
                        newData.AddRange(BitConverter.GetBytes(i).Reverse());
                    else
                        newData.AddRange(BitConverter.GetBytes(i));
                }

                newData.AddRange(restOfOldData);
                
                Data = newData.ToArray();

                SiblingAmount = (short)value.Length;
            }
        }

        [Browsable(false)]
        public float ScaleX
        {
            get => GetScale();
            set => SetScale(value);
        }
        [Browsable(false)]
        public float ScaleY
        {
            get => GetScale();
            set => SetScale(value);
        }
        [Browsable(false)]
        public float ScaleZ
        {
            get => GetScale();
            set => SetScale(value);
        }

        private float GetScale()
        {
            if (IsZone == 0x00 && _arenaRadius != -1f)
                return _arenaRadius;

            return 1f;
        }

        private void SetScale(float scale)
        {
            if (IsZone == 0x00)
                ArenaRadius = scale;
        }
    }
}