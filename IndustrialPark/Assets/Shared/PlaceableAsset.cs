using HipHopFile;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public abstract class PlaceableAsset : ObjectAsset, IRenderableAsset, IClickableAsset, IRotatableAsset, IScalableAsset
    {
        protected Matrix world;
        protected BoundingBox boundingBox;

        protected abstract bool DontRender { get; }

        public PlaceableAsset(Section_AHDR AHDR) : base(AHDR)
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
            {
                CreateBoundingBox(ArchiveEditorFunctions.renderingDictionary[_modelAssetID].GetRenderWareModelFile().vertexListG);
            }
            else
            {
                CreateBoundingBox(SharpRenderer.cubeVertices, 0.5f);
            }
        }

        protected Vector3[] vertices;
        protected RenderWareFile.Triangle[] triangles;

        protected virtual void CreateBoundingBox(List<Vector3> vertexList, float multiplier = 1f)
        {
            vertices = new Vector3[vertexList.Count];

            for (int i = 0; i < vertexList.Count; i++)
                vertices[i] = (Vector3)Vector3.Transform(vertexList[i] * multiplier, world);

            boundingBox = BoundingBox.FromPoints(vertices);

            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(_modelAssetID))
            {
                if (ArchiveEditorFunctions.renderingDictionary[_modelAssetID] is AssetMINF MINF)
                {
                    if (MINF.HasRenderWareModelFile())
                        triangles = ArchiveEditorFunctions.renderingDictionary[_modelAssetID].GetRenderWareModelFile().triangleList.ToArray();
                    else
                        triangles = null;
                }
                else
                    triangles = ArchiveEditorFunctions.renderingDictionary[_modelAssetID].GetRenderWareModelFile().triangleList.ToArray();
            }
            else
                triangles = null;
        }

        public virtual void Draw(SharpRenderer renderer)
        {
            if (DontRender || isInvisible)
                return;

            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(_modelAssetID))
                ArchiveEditorFunctions.renderingDictionary[_modelAssetID].Draw(renderer, LocalWorld(), isSelected ? renderer.selectedObjectColor * _color : _color);
            else
                renderer.DrawCube(LocalWorld(), isSelected);
        }

        public virtual float? IntersectsWith(Ray ray)
        {
            if (DontRender || isInvisible)
                return null;

            if (ray.Intersects(ref boundingBox, out float distance))
                return TriangleIntersection(ray, distance);
            return null;
        }
        
        protected virtual float? TriangleIntersection(Ray r, float initialDistance)
        {
            if (triangles == null)
                return initialDistance;

            bool hasIntersected = false;
            float smallestDistance = 1000f;

            foreach (RenderWareFile.Triangle t in triangles)
                if (r.Intersects(ref vertices[t.vertex1], ref vertices[t.vertex2], ref vertices[t.vertex3], out float distance))
                {
                    hasIntersected = true;

                    if (distance < smallestDistance)
                        smallestDistance = distance;
                }

            if (hasIntersected)
                return smallestDistance;
            return null;
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
            if (Surface_AssetID == assetID)
                return true;
            if (Model_AssetID == assetID)
                return true;
            if (Animation_AssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        [Category("Placement Flags"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte VisibilityFlag
        {
            get => ReadByte(0x8);
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
            get => ReadByte(0x9);
            set => Write(0x9, value);
        }

        [Category("Placement Flags"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte UnknownFlag0A
        {
            get => ReadByte(0xA);
            set => Write(0xA, value);
        }

        [Category("Placement Flags"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte SolidityFlag
        {
            get => ReadByte(0xB);
            set => Write(0xB, value);
        }

        [Category("Placement Flags")]
        public bool PreciseCollision
        {
            get => (SolidityFlag & Mask(1)) != 0;
            set => SolidityFlag = (byte)(value ? (VisibilityFlag | Mask(1)) : (VisibilityFlag & InvMask(1)));
        }

        [Category("Placement")]
        public int PaddingC
        {
            get => ReadInt(0xC);
            set => Write(0xC, value);
        }

        [Category("Placement References")]
        public AssetID Surface_AssetID
        {
            get => ReadUInt(0x10 + Offset);
            set => Write(0x10 + Offset, value);
        }
        
        public Vector3 _position;

        [Category("Placement")]
        [TypeConverter(typeof(FloatTypeConverter))]
        public virtual float PositionX
        {
            get => _position.X;
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
            get => _position.Y;
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
            get => _position.Z;
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
            get => MathUtil.RadiansToDegrees(_yaw);
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
            get => MathUtil.RadiansToDegrees(_pitch);
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
            get => MathUtil.RadiansToDegrees(_roll);
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
            get => _scale.X;
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
            get => _scale.Y;
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
            get => _scale.Z;
            set
            {
                _scale.Z = value;
                Write(0x34 + Offset, _scale.Z);
                CreateTransformMatrix();
            }
        }

        protected Vector4 _color;

        [Category("Placement Color"), DisplayName("Red (0 - 1)")]
        public float ColorRed
        {
            get => ReadFloat(0x38 + Offset);
            set
            {
                _color.X = value;
                Write(0x38 + Offset, _color.X);
            }
        }

        [Category("Placement Color"), DisplayName("Green (0 - 1)")]
        public float ColorGreen
        {
            get => ReadFloat(0x3C + Offset);
            set
            {
                _color.Y = value;
                Write(0x3C + Offset, _color.Y);
            }
        }

        [Category("Placement Color"), DisplayName("Blue (0 - 1)")]
        public float ColorBlue
        {
            get => ReadFloat(0x40 + Offset);
            set
            {
                _color.Z = value;
                Write(0x40 + Offset, _color.Z);
            }
        }

        [Category("Placement Color"), DisplayName("Alpha (0 - 1)")]
        public float ColorAlpha
        {
            get => ReadFloat(0x44 + Offset);
            set
            {
                _color.W = value;
                Write(0x44 + Offset, _color.W);
            }
        }

        [Category("Placement Color"), DisplayName("Color - (A,) R, G, B")]
        public System.Drawing.Color Color_ARGB
        {
            get => System.Drawing.Color.FromArgb(BitConverter.ToInt32(new byte[] { (byte)(ColorBlue * 255), (byte)(ColorGreen * 255), (byte)(ColorRed * 255), (byte)(ColorAlpha * 255) }, 0));
            set
            {
                ColorRed = value.R / 255f;
                ColorGreen = value.G / 255f;
                ColorBlue = value.B / 255f;
                ColorAlpha = value.A / 255f;
            }
        }

        [Category("Placement")]
        public float ColorAlphaSpeed
        {
            get => ReadFloat(0x48 + Offset);
            set { Write(0x48 + Offset, value); }
        }

        protected uint _modelAssetID;
        [Category("Placement References")]
        public AssetID Model_AssetID
        {
            get => _modelAssetID;
            set
            {
                _modelAssetID = value;
                Write(0x4C + Offset, _modelAssetID);
            }
        }

        [Category("Placement References")]
        public virtual AssetID Animation_AssetID
        {
            get => ReadUInt(0x50 + Offset);
            set { Write(0x50 + Offset, value); }
        }

        public static bool movementPreview = false;

        protected PlaceableAsset FindDrivenByAsset(out bool found, out bool useRotation)
        {
            foreach (LinkBFBB assetEvent in LinksBFBB)
                if (assetEvent.EventSendID == EventBFBB.Drivenby)
                {
                    uint PlatID = assetEvent.TargetAssetID;

                    foreach (ArchiveEditor ae in Program.MainForm.archiveEditors)
                        if (ae.archive.ContainsAsset(PlatID))
                        {
                            Asset asset = ae.archive.GetFromAssetID(PlatID);
                            if (asset is PlaceableAsset Placeable)
                            {
                                found = true;
                                useRotation = assetEvent.Arguments_Float[0] != 0;
                                return Placeable;
                            }
                        }
                }
            found = false;
            useRotation = false;
            return null;
        }

        public virtual Matrix PlatLocalRotation()
        {
            return Matrix.Identity;
        }

        public virtual Matrix LocalWorld()
        {
            if (movementPreview)
            {
                PlaceableAsset driver = FindDrivenByAsset(out bool found, out bool useRotation);

                if (found)
                {
                    return Matrix.Scaling(_scale)
                        * Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll)
                        * Matrix.Translation(_position - driver._position)
                        * (useRotation ? driver.PlatLocalRotation() : Matrix.Identity)
                        * Matrix.Translation((Vector3)Vector3.Transform(Vector3.Zero, driver.LocalWorld()));
                }
            }

            return world;
        }
    }
}