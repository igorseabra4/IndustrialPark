using HipHopFile;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public abstract class EntityAsset : BaseAsset, IRenderableAsset, IClickableAsset, IRotatableAsset, IScalableAsset
    {
        [Browsable(false)]
        public Matrix world { get; protected set; }
        protected BoundingBox boundingBox;


        [Browsable(false)]
        public abstract bool DontRender { get; }

        public EntityAsset(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            _yaw = ReadFloat(0x14 + Offset);
            _pitch = ReadFloat(0x18 + Offset);
            _roll = ReadFloat(0x1C + Offset);
            _position = new Vector3(ReadFloat(0x20 + Offset), ReadFloat(0x24 + Offset), ReadFloat(0x28 + Offset));
            _scale = new Vector3(ReadFloat(0x2C + Offset), ReadFloat(0x30 + Offset), ReadFloat(0x34 + Offset));
            _color = new Vector4(ReadFloat(0x38 + Offset), ReadFloat(0x3c + Offset), ReadFloat(0x40 + Offset), ReadFloat(0x44 + Offset));

            _modelAssetID = ReadUInt(0x4C + Offset);

            CreateTransformMatrix();
            
            renderableAssets.Add(this);
        }
        
        public virtual void CreateTransformMatrix()
        {
            world = Matrix.Scaling(_scale)
                * Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll)
                * Matrix.Translation(Position);

            CreateBoundingBox();
        }

        protected virtual void CreateBoundingBox()
        {
            if (renderingDictionary.ContainsKey(_modelAssetID) &&
                renderingDictionary[_modelAssetID].HasRenderWareModelFile() &&
                renderingDictionary[_modelAssetID].GetRenderWareModelFile() != null)
            {
                CreateBoundingBox(renderingDictionary[_modelAssetID].GetRenderWareModelFile().vertexListG);
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

            if (renderingDictionary.ContainsKey(_modelAssetID))
            {
                IAssetWithModel assetWithModel = renderingDictionary[_modelAssetID];
                if (assetWithModel.HasRenderWareModelFile())
                    triangles = assetWithModel.GetRenderWareModelFile().triangleList.ToArray();
                else
                    triangles = null;
            }
            else
                triangles = null;
        }

        public bool ShouldDraw(SharpRenderer renderer)
        {
            if (isSelected)
                return true;
            if (DontRender)
                return false;
            if (isInvisible)
                return false;
            if (movementPreview)
                return true;

            if (AssetMODL.renderBasedOnLodt)
            {
                if (GetDistanceFrom(renderer.Camera.Position) <
                    (AssetLODT.MaxDistances.ContainsKey(_modelAssetID) ?
                    AssetLODT.MaxDistances[_modelAssetID] : SharpRenderer.DefaultLODTDistance))
                    return renderer.frustum.Intersects(ref boundingBox);
                
                return false;
            }

            return renderer.frustum.Intersects(ref boundingBox);
        }

        public virtual void Draw(SharpRenderer renderer)
        {
            if (renderingDictionary.ContainsKey(_modelAssetID))
                renderingDictionary[_modelAssetID].Draw(renderer, LocalWorld(), isSelected ? renderer.selectedObjectColor * _color : _color, UvAnimOffset);
            else
                renderer.DrawCube(LocalWorld(), isSelected);
        }

        [Browsable(false)]
        public virtual bool SpecialBlendMode => !renderingDictionary.ContainsKey(_modelAssetID) || renderingDictionary[_modelAssetID].SpecialBlendMode;

        public virtual float? GetIntersectionPosition(SharpRenderer renderer, Ray ray)
        {
            if (!ShouldDraw(renderer))
                return null;

            if (ray.Intersects(ref boundingBox, out float distance))
                return TriangleIntersection(ray, distance);
            return null;
        }

        protected virtual float? TriangleIntersection(Ray r, float initialDistance)
        {
            if (triangles == null)
                return initialDistance;

            float? smallestDistance = null;

            foreach (RenderWareFile.Triangle t in triangles)
                if (r.Intersects(ref vertices[t.vertex1], ref vertices[t.vertex2], ref vertices[t.vertex3], out float distance))
                    if (smallestDistance == null || distance < smallestDistance)
                        smallestDistance = distance;
                
            return smallestDistance;
        }

        public BoundingBox GetBoundingBox()
        {
            return boundingBox;
        }

        public virtual float GetDistanceFrom(Vector3 cameraPosition)
        {
            float min = Vector3.Distance(cameraPosition, Position);
            float d;

            foreach (var v in boundingBox.GetCorners())
                if ((d = Vector3.Distance(cameraPosition, v)) < min)
                    min = d;

            return min;
        }

        public override bool HasReference(uint assetID) => Surface_AssetID == assetID || Model_AssetID == assetID || Animation_AssetID == assetID ||
            base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Verify(Surface_AssetID, ref result);
            if (Model_AssetID == 0)
                result.Add(AHDR.assetType.ToString() + " with Model_AssetID set to 0");
            if (!(this is AssetTRIG))
                Verify(Model_AssetID, ref result);
            Verify(Animation_AssetID, ref result);
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game != Game.BFBB)
                dt.RemoveProperty("PaddingC");

            base.SetDynamicProperties(dt);
        }

        private const string categoryName = "Entity";

        [Category(categoryName)]
        public DynamicTypeDescriptor VisibilityFlags => ByteFlagsDescriptor(0x8, "Visible", "Stackable");

        [Category(categoryName), TypeConverter(typeof(HexByteTypeConverter))]
        public byte TypeFlag
        {
            get => ReadByte(0x9);
            set => Write(0x9, value);
        }

        [Category(categoryName)]
        public DynamicTypeDescriptor UnknownFlag0A => ByteFlagsDescriptor(0xA);
        
        [Category(categoryName)]
        public DynamicTypeDescriptor SolidityFlags => ByteFlagsDescriptor(0xB, null, "Precise Collision", null, null, "Hittable", "Animate Collision", null, "Ledge Grab");
        
        [Category(categoryName)]
        public int PaddingC
        {
            get
            {
                if (game == Game.BFBB)
                    return ReadInt(0xC);
                return 0;
            }
            set
            {
                if (game == Game.BFBB)
                    Write(0xC, value);
            }
        }

        [Category(categoryName + " References")]
        public AssetID Surface_AssetID
        {
            get => ReadUInt(0x10 + Offset);
            set => Write(0x10 + Offset, value);
        }

        protected Vector3 _position;
        [Browsable(false)]
        public Vector3 Position
        {
            get => _position;
            protected set
            {
                _position = value;
                Write(0x20 + Offset, _position.X);
                Write(0x24 + Offset, _position.Y);
                Write(0x28 + Offset, _position.Z);
                CreateTransformMatrix();
            }
        }

        [Category(categoryName)]
        [TypeConverter(typeof(FloatTypeConverter))]
        public virtual float PositionX
        {
            get => Position.X;
            set
            {
                _position.X = value;
                Write(0x20 + Offset, Position.X);
                CreateTransformMatrix();
            }
        }

        [Category(categoryName)]
        [TypeConverter(typeof(FloatTypeConverter))]
        public virtual float PositionY
        {
            get => Position.Y;
            set
            {
                _position.Y = value;
                Write(0x24 + Offset, Position.Y);
                CreateTransformMatrix();
            }
        }

        [Category(categoryName)]
        [TypeConverter(typeof(FloatTypeConverter))]
        public virtual float PositionZ
        {
            get => Position.Z;
            set
            {
                _position.Z = value;
                Write(0x28 + Offset, Position.Z);
                CreateTransformMatrix();
            }
        }

        protected float _yaw;
        protected float _pitch;
        protected float _roll;

        [Category(categoryName)]
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

        [Category(categoryName)]
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

        [Category(categoryName)]
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

        [Category(categoryName)]
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

        [Category(categoryName)]
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

        [Category(categoryName)]
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

        [Category(categoryName + " Color"), DisplayName("Red (0 - 1)")]
        public float ColorRed
        {
            get => ReadFloat(0x38 + Offset);
            set
            {
                _color.X = value;
                Write(0x38 + Offset, _color.X);
            }
        }

        [Category(categoryName + " Color"), DisplayName("Green (0 - 1)")]
        public float ColorGreen
        {
            get => ReadFloat(0x3C + Offset);
            set
            {
                _color.Y = value;
                Write(0x3C + Offset, _color.Y);
            }
        }

        [Category(categoryName + " Color"), DisplayName("Blue (0 - 1)")]
        public float ColorBlue
        {
            get => ReadFloat(0x40 + Offset);
            set
            {
                _color.Z = value;
                Write(0x40 + Offset, _color.Z);
            }
        }

        [Category(categoryName + " Color"), DisplayName("Alpha (0 - 1)")]
        public float ColorAlpha
        {
            get => ReadFloat(0x44 + Offset);
            set
            {
                _color.W = value;
                Write(0x44 + Offset, _color.W);
            }
        }

        [Category(categoryName + " Color"), DisplayName("Color - (A,) R, G, B")]
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

        [Category(categoryName + " Color")]
        public float ColorAlphaSpeed
        {
            get => ReadFloat(0x48 + Offset);
            set => Write(0x48 + Offset, value);
        }

        protected uint _modelAssetID;
        [Category(categoryName + " References")]
        public AssetID Model_AssetID
        {
            get => _modelAssetID;
            set
            {
                _modelAssetID = value;
                Write(0x4C + Offset, _modelAssetID);
                CreateTransformMatrix();
            }
        }

        [Category(categoryName + " References")]
        public virtual AssetID Animation_AssetID
        {
            get => ReadUInt(0x50 + Offset);
            set => Write(0x50 + Offset, value);
        }

        public static bool movementPreview = false;

        protected EntityAsset FindDrivenByAsset(out bool found, out bool useRotation)
        {
            foreach (LinkBFBB assetEvent in LinksBFBB)
            {
                uint PlatID = 0;

                if (assetEvent.EventSendID == EventBFBB.Drivenby)
                    PlatID = assetEvent.TargetAssetID;
                else if (assetEvent.EventSendID == EventBFBB.Mount)
                    PlatID = assetEvent.ArgumentAssetID;
                else continue;

                foreach (ArchiveEditor ae in Program.MainForm.archiveEditors)
                    if (ae.archive.ContainsAsset(PlatID))
                    {
                        Asset asset = ae.archive.GetFromAssetID(PlatID);
                        if (asset is EntityAsset Placeable)
                        {
                            found = true;
                            useRotation = assetEvent.Arguments_Float[0] != 0 || assetEvent.EventSendID == EventBFBB.Mount;
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
                EntityAsset driver = FindDrivenByAsset(out bool found, out bool useRotation);

                if (found)
                {
                    return Matrix.Scaling(_scale)
                        * Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll)
                        * Matrix.Translation(Position - driver.Position)
                        * (useRotation ? driver.PlatLocalRotation() : Matrix.Identity)
                        * Matrix.Translation((Vector3)Vector3.Transform(Vector3.Zero, driver.LocalWorld()));
                }
            }

            return world;
        }

        public virtual void Reset()
        {
            localFrameCounter = -1;
            FindSurf();
        }

        protected Vector3 uvTransSpeed;
        protected int localFrameCounter = -1;

        private void FindSurf()
        {
            foreach (ArchiveEditor ae in Program.MainForm.archiveEditors)
                if (ae.archive.ContainsAsset(Surface_AssetID))
                    if (ae.archive.GetFromAssetID(Surface_AssetID) is AssetSURF SURF)
                    {
                        uvTransSpeed = new Vector3(SURF.UVEffects1_TransSpeed_X, SURF.UVEffects1_TransSpeed_Y, SURF.UVEffects1_TransSpeed_Z);
                        return;
                    }
            uvTransSpeed = Vector3.Zero;
        }

        [Browsable(false)]
        public Vector3 UvAnimOffset => movementPreview ? uvTransSpeed * localFrameCounter++ / 60f : Vector3.Zero;
    }
}