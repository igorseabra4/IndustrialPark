using HipHopFile;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class AssetDUPC : BaseAsset, IRenderableAsset, IClickableAsset, IRotatableAsset, IScalableAsset
    {
        protected Matrix world;
        protected BoundingBox boundingBox;

        protected override int EventStartOffset => 0xA4;
        public static bool dontRender = false;

        protected bool DontRender => dontRender;

        public AssetDUPC(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            _modelAssetID = ReadUInt(0x7C);

            CreateTransformMatrix();
            
            if (!renderableAssetSetCommon.Contains(this))
                renderableAssetSetCommon.Add(this);
        }
        
        public virtual void CreateTransformMatrix()
        {
            world = Matrix.Scaling(ScaleX, ScaleY, ScaleZ)
                * Matrix.RotationYawPitchRoll(ReadFloat(0x44), ReadFloat(0x48), ReadFloat(0x4C))
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

        public virtual void Draw(SharpRenderer renderer)
        {
            if (!isSelected && (DontRender || isInvisible))
                return;

            if (renderingDictionary.ContainsKey(_modelAssetID))
                renderingDictionary[_modelAssetID].Draw(renderer, world, isSelected ? renderer.selectedObjectColor * _color : _color, new Vector3());
            else
                renderer.DrawCube(world, isSelected);
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
        
        public BoundingBox GetBoundingBox()
        {
            return boundingBox;
        }

        public float GetDistance(Vector3 cameraPosition)
        {
            return Vector3.Distance(cameraPosition, Position);
        }

        public override bool HasReference(uint assetID) => 
            Surface_AssetID == assetID || Model_AssetID == assetID || Animation_AssetID == assetID ||
            (uint)VilType == assetID || NPCSettings_AssetID == assetID || MovePoint_AssetID == assetID ||
            TaskDYNA1_AssetID == assetID || TaskDYNA2_AssetID == assetID || NavMesh1_AssetID == assetID || 
            Settings_AssetID == assetID || NavMesh2_AssetID == assetID || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Verify(Surface_AssetID, ref result);
            if (Model_AssetID == 0)
                result.Add(AHDR.assetType.ToString() + " with Model_AssetID set to 0");
            
            Verify(Model_AssetID, ref result);
            Verify(Animation_AssetID, ref result);

            if (VilType.ToString() == ((int)VilType).ToString())
                result.Add("VIL with unknown VilType 0x" + VilType.ToString("X8"));

            Verify(NPCSettings_AssetID, ref result);
            Verify(MovePoint_AssetID, ref result);
            Verify(TaskDYNA1_AssetID, ref result);
            Verify(TaskDYNA2_AssetID, ref result);
            Verify(NavMesh1_AssetID, ref result);
            Verify(NavMesh2_AssetID, ref result);
            Verify(Settings_AssetID, ref result);
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            base.SetDynamicProperties(dt);
        }

        private const string categoryName = "Duplicator";

        public override AssetID AssetID
        {
            get => base.AssetID;
            set 
            { 
                base.AssetID = value;
                Write(0x34, value);
            }
        }

        [Category(categoryName), TypeConverter(typeof(HexUShortTypeConverter))]
        public ushort UnknownShort_08
        {
            get => ReadUShort(0x08);
            set => Write(0x08, value);
        }
        [Category(categoryName), TypeConverter(typeof(HexUShortTypeConverter))]
        public ushort UnknownShort_0A
        {
            get => ReadUShort(0x0A);
            set => Write(0x0A, value);
        }
        [Category(categoryName), TypeConverter(typeof(HexUShortTypeConverter))]
        public ushort UnknownShort_0C
        {
            get => ReadUShort(0x0C);
            set => Write(0x0C, value);
        }
        [Category(categoryName), TypeConverter(typeof(HexUShortTypeConverter))]
        public ushort UnknownShort_0E
        {
            get => ReadUShort(0x0E);
            set => Write(0x0E, value);
        }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_10
        {
            get => ReadFloat(0x10);
            set => Write(0x10, value);
        }
        [Category(categoryName)]
        public int UnknownInt_14
        {
            get => ReadInt(0x14);
            set => Write(0x14, value);
        }
        [Category(categoryName)]
        public int UnknownInt_18
        {
            get => ReadInt(0x18);
            set => Write(0x18, value);
        }
        [Category(categoryName)]
        public int UnknownInt_1C
        {
            get => ReadInt(0x1C);
            set => Write(0x1C, value);
        }
        [Category(categoryName)]
        public int UnknownInt_20
        {
            get => ReadInt(0x20);
            set => Write(0x20, value);
        }
        [Category(categoryName)]
        public int UnknownInt_24
        {
            get => ReadInt(0x24);
            set => Write(0x24, value);
        }
        [Category(categoryName)]
        public int UnknownInt_28
        {
            get => ReadInt(0x28);
            set => Write(0x28, value);
        }
        [Category(categoryName)]
        public AssetID NavMesh1_AssetID
        {
            get => ReadUInt(0x2C);
            set => Write(0x2C, value);
        }
        [Category(categoryName)]
        public int UnknownInt_30
        {
            get => ReadInt(0x30);
            set => Write(0x30, value);
        }

        private const string vilCategoryName = "VIL";

        [Category(vilCategoryName + " Base"), DisplayName("AssetType")]
        public ObjectAssetType Vil_AssetType
        {
            get => (ObjectAssetType)ReadByte(0x38);
            set => Write(0x38, (byte)value);
        }
        
        [Category(vilCategoryName + " Base"), DisplayName("AssetType")]
        public DynamicTypeDescriptor Vil_Flags
        {
            get => ShortFlagsDescriptor(0x3A,
                "Enabled On Start",
                "State Is Persistent",
                "Unknown Always True",
                "Visible During Cutscenes",
                "Receive Shadows");
        }
        
        [Category(vilCategoryName + " Entity Flags")]
        public DynamicTypeDescriptor VisibilityFlags => ByteFlagsDescriptor(0x3C, "Visible", "Stackable");

        [Category(vilCategoryName + " Entity Flags"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte TypeFlag
        {
            get => ReadByte(0x3D);
            set => Write(0x3D, value);
        }

        [Category(vilCategoryName + " Entity Flags")]
        public DynamicTypeDescriptor UnknownFlag0A => ByteFlagsDescriptor(0x3E);
        
        [Category(vilCategoryName + " Entity Flags")]
        public DynamicTypeDescriptor SolidityFlags => ByteFlagsDescriptor(0x3F, "Unused", "Precise Collision");
        
        [Category(vilCategoryName + " References")]
        public AssetID Surface_AssetID
        {
            get => ReadUInt(0x40);
            set => Write(0x40, value);
        }

        [Browsable(false)]
        public Vector3 Position
        {
            get => new Vector3(PositionX, PositionY, PositionZ);
            protected set
            {
                Write(0x50, value.X);
                Write(0x54, value.Y);
                Write(0x58, value.Z);
                CreateTransformMatrix();
            }
        }

        [Category(vilCategoryName)]
        [TypeConverter(typeof(FloatTypeConverter))]
        public float PositionX
        {
            get => ReadFloat(0x50);
            set
            {
                Write(0x50, value);
                CreateTransformMatrix();
            }
        }

        [Category(vilCategoryName)]
        [TypeConverter(typeof(FloatTypeConverter))]
        public float PositionY
        {
            get => ReadFloat(0x54);
            set
            {
                Write(0x54, value);
                CreateTransformMatrix();
            }
        }

        [Category(vilCategoryName)]
        [TypeConverter(typeof(FloatTypeConverter))]
        public float PositionZ
        {
            get => ReadFloat(0x58);
            set
            {
                Write(0x58, value);
                CreateTransformMatrix();
            }
        }
        
        [Category(vilCategoryName)]
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Yaw
        {
            get => MathUtil.RadiansToDegrees(ReadFloat(0x44));
            set
            {
                Write(0x44, MathUtil.DegreesToRadians(value));
                CreateTransformMatrix();
            }
        }

        [Category(vilCategoryName)]
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Pitch
        {
            get => MathUtil.RadiansToDegrees(ReadFloat(0x48));
            set
            {
                Write(0x48, MathUtil.DegreesToRadians(value));
                CreateTransformMatrix();
            }
        }

        [Category(vilCategoryName)]
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Roll
        {
            get => MathUtil.RadiansToDegrees(ReadFloat(0x4C));
            set
            {
                Write(0x4C, MathUtil.DegreesToRadians(value));
                CreateTransformMatrix();
            }
        }

        [Category(vilCategoryName)]
        [TypeConverter(typeof(FloatTypeConverter))]
        public float ScaleX
        {
            get => ReadFloat(0x5C);
            set
            {
                Write(0x5C, value);
                CreateTransformMatrix();
            }
        }

        [Category(vilCategoryName)]
        [TypeConverter(typeof(FloatTypeConverter))]
        public float ScaleY
        {
            get => ReadFloat(0x60);
            set
            {
                Write(0x60, value);
                CreateTransformMatrix();
            }
        }

        [Category(vilCategoryName)]
        [TypeConverter(typeof(FloatTypeConverter))]
        public float ScaleZ
        {
            get => ReadFloat(0x64);
            set
            {
                Write(0x64, value);
                CreateTransformMatrix();
            }
        }
        protected Vector4 _color => new Vector4(ColorRed, ColorGreen, ColorBlue, ColorAlpha);

        [Category(vilCategoryName + " Color"), DisplayName("Red (0 - 1)")]
        public float ColorRed
        {
            get => ReadFloat(0x68);
            set => Write(0x68, _color.X);
        }

        [Category(vilCategoryName + " Color"), DisplayName("Green (0 - 1)")]
        public float ColorGreen
        {
            get => ReadFloat(0x6C);
            set => Write(0x6C, _color.Y);
        }

        [Category(vilCategoryName + " Color"), DisplayName("Blue (0 - 1)")]
        public float ColorBlue
        {
            get => ReadFloat(0x70);
            set => Write(0x70, _color.Y);
        }

        [Category(vilCategoryName + " Color"), DisplayName("Alpha (0 - 1)")]
        public float ColorAlpha
        {
            get => ReadFloat(0x74);
            set => Write(0x74, _color.Y);
        }

        [Category(vilCategoryName + " Color"), DisplayName("Color - (A,) R, G, B")]
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

        [Category(vilCategoryName + " Color")]
        public float ColorAlphaSpeed
        {
            get => ReadFloat(0x78);
            set => Write(0x78, _color.Y);
        }

        protected uint _modelAssetID;
        [Category(vilCategoryName + " References")]
        public AssetID Model_AssetID
        {
            get => _modelAssetID;
            set
            {
                _modelAssetID = value;
                Write(0x7C, _modelAssetID);
                CreateTransformMatrix();
            }
        }

        [Category(vilCategoryName + " References")]
        public AssetID Animation_AssetID
        {
            get => ReadUInt(0x80);
            set => Write(0x80, value);
        }

        [Category(vilCategoryName)]
        public DynamicTypeDescriptor VilFlags => IntFlagsDescriptor(0x84);

        [Category(vilCategoryName)]
        public VilType VilType
        {
            get => (VilType)ReadUInt(0x88);
            set => Write(0x88, (uint)value);
        }

        [Category(vilCategoryName), DisplayName("VilType (Alphabetical)")]
        public VilType_Alphabetical VilType_Alphabetical
        {
            get
            {
                foreach (VilType_Alphabetical o in Enum.GetValues(typeof(VilType_Alphabetical)))
                    if (o.ToString() == VilType.ToString())
                        return o;

                return VilType_Alphabetical.Null;
            }
            set
            {
                foreach (VilType o in Enum.GetValues(typeof(VilType)))
                    if (o.ToString() == value.ToString())
                    {
                        Write(0x58 + Offset, (uint)o);
                        return;
                    }

                throw new Exception();
            }
        }

        [Category(vilCategoryName)]
        public AssetID NPCSettings_AssetID
        {
            get => ReadUInt(0x8C);
            set => Write(0x8C, value);
        }

        [Category(vilCategoryName)]
        public AssetID MovePoint_AssetID
        {
            get => ReadUInt(0x90);
            set => Write(0x90, value);
        }

        [Category(vilCategoryName)]
        public AssetID TaskDYNA1_AssetID
        {
            get => ReadUInt(0x94);
            set => Write(0x94, value);
        }

        [Category(vilCategoryName)]
        public AssetID TaskDYNA2_AssetID
        {
            get => ReadUInt(0x98);
            set => Write(0x98, value);
        }
        
        [Category(categoryName)]
        public AssetID NavMesh2_AssetID
        {
            get => ReadUInt(0x9C);
            set => Write(0x9C, value);
        }

        [Category(categoryName)]
        public AssetID Settings_AssetID
        {
            get => ReadUInt(0xA0);
            set => Write(0xA0, value);
        }
    }
}