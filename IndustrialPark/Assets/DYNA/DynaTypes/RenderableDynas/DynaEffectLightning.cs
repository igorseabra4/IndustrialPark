using AssetEditorColors;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;

namespace IndustrialPark
{
    public class DynaEffectLightning : DynaBase
    {
        public string Note => "Version is always 2";

        public override int StructSize => 0x4C;

        public DynaEffectLightning(AssetDYNA asset) : base(asset) { }

        public override bool HasReference(uint assetID) =>
            LightningTexture_AssetID == assetID || GlowTexture_AssetID == assetID || SIMP1_AssetID == assetID || SIMP2_AssetID == assetID;
        
        public override void Verify(ref List<string> result)
        {
            Asset.Verify(LightningTexture_AssetID, ref result);
            Asset.Verify(GlowTexture_AssetID, ref result);
            Asset.Verify(SIMP1_AssetID, ref result);
            Asset.Verify(SIMP2_AssetID, ref result);
        }
        
        [Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionX
        {
            get => ReadFloat(0x00);
            set
            {
                Write(0x00, value);
                CreateTransformMatrix();
            }
        }
        [Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionY
        {
            get => ReadFloat(0x04);
            set
            {
                Write(0x04, value);
                CreateTransformMatrix();
            }
        }
        [Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionZ
        {
            get => ReadFloat(0x08);
            set
            {
                Write(0x08, value);
                CreateTransformMatrix();
            }
        }

        [TypeConverter(typeof(FloatTypeConverter))]
        public float PositionEndX
        {
            get => ReadFloat(0x0C);
            set
            {
                Write(0x0C, value);
                CreateTransformMatrix();
            }
        }
        [Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public float PositionEndY
        {
            get => ReadFloat(0x10);
            set
            {
                Write(0x10, value);
                CreateTransformMatrix();
            }
        }
        [Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public float PositionEndZ
        {
            get => ReadFloat(0x14);
            set
            {
                Write(0x14, value);
                CreateTransformMatrix();
            }
        }

        private byte ColorR
        {
            get => ReadByte(0x18);
            set => Write(0x18, value);
        }
        private byte ColorG
        {
            get => ReadByte(0x19);
            set => Write(0x19, value);
        }
        private byte ColorB
        {
            get => ReadByte(0x1A);
            set => Write(0x1A, value);
        }

        [Editor(typeof(MyColorEditor), typeof(UITypeEditor)), DisplayName("Color 1 (R, G, B)")]
        public MyColor Color
        {
            get => new MyColor(ColorR, ColorG, ColorB, ColorAlpha);
            set
            {
                ColorR = value.R;
                ColorG = value.G;
                ColorB = value.B;
            }
        }
        [DisplayName("Color 1 Alpha (0 - 255)")]
        public byte ColorAlpha
        {
            get => ReadByte(0x1B);
            set => Write(0x1B, value);
        }

        [TypeConverter(typeof(FloatTypeConverter))]
        public float Width
        {
            get => ReadFloat(0x1C);
            set => Write(0x1C, value);
        }

        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat
        {
            get => ReadFloat(0x20);
            set => Write(0x20, value);
        }

        public AssetID LightningTexture_AssetID
        {
            get => ReadUInt(0x24);
            set => Write(0x24, value);
        }

        public AssetID GlowTexture_AssetID
        {
            get => ReadUInt(0x28);
            set => Write(0x28, value);
        }
        
        public int UnknownInt1
        {
            get => ReadInt(0x2C);
            set => Write(0x2C, value);
        }

        [TypeConverter(typeof(FloatTypeConverter))]
        public float KnockbackSpeed
        {
            get => ReadFloat(0x30);
            set => Write(0x30, value);
        }

        public AssetID SoundGroupID_AssetID
        {
            get => ReadUInt(0x34);
            set => Write(0x34, value);
        }

        public int UnknownInt2
        {
            get => ReadInt(0x38);
            set => Write(0x38, value);
        }

        public int UnknownInt3
        {
            get => ReadInt(0x3C);
            set => Write(0x3C, value);
        }

        public AssetID SIMP1_AssetID
        {
            get => ReadUInt(0x40);
            set => Write(0x40, value);
        }

        public AssetID SIMP2_AssetID
        {
            get => ReadUInt(0x44);
            set => Write(0x44, value);
        }

        public int DamagePlayer
        {
            get => ReadInt(0x48);
            set => Write(0x48, value);
        }

        public override bool IsRenderableClickable => true;

        private Matrix world;
        private Matrix world2;
        private BoundingBox boundingBox;
        private Vector3[] vertices;
        protected RenderWareFile.Triangle[] triangles;

        public override void CreateTransformMatrix()
        {
            world = Matrix.Translation(PositionX, PositionY, PositionZ);
            world2 = Matrix.Translation(PositionEndX, PositionEndY, PositionEndZ);

            vertices = new Vector3[SharpRenderer.cubeVertices.Count];

            for (int i = 0; i < SharpRenderer.cubeVertices.Count; i++)
                vertices[i] = (Vector3)Vector3.Transform(SharpRenderer.cubeVertices[i], world);

            boundingBox = BoundingBox.FromPoints(vertices);

            triangles = new RenderWareFile.Triangle[SharpRenderer.cubeTriangles.Count];
            for (int i = 0; i < SharpRenderer.cubeTriangles.Count; i++)
            {
                triangles[i] = new RenderWareFile.Triangle((ushort)SharpRenderer.cubeTriangles[i].materialIndex,
                    (ushort)SharpRenderer.cubeTriangles[i].vertex1, (ushort)SharpRenderer.cubeTriangles[i].vertex2, (ushort)SharpRenderer.cubeTriangles[i].vertex3);
            }
        }

        public override void Draw(SharpRenderer renderer, bool isSelected)
        {
            renderer.DrawCube(world, isSelected);
            renderer.DrawCube(world2, isSelected);
        }

        public override BoundingBox GetBoundingBox()
        {
            return boundingBox;
        }

        public override float GetDistance(Vector3 cameraPosition)
        {
            return Vector3.Distance(cameraPosition, new Vector3(PositionX, PositionY, PositionZ));
        }

        public override float? IntersectsWith(Ray ray)
        {
            float? smallestDistance = null;

            if (ray.Intersects(ref boundingBox))
                foreach (RenderWareFile.Triangle t in triangles)
                    if (ray.Intersects(ref vertices[t.vertex1], ref vertices[t.vertex2], ref vertices[t.vertex3], out float distance))
                        if (smallestDistance == null || distance < smallestDistance)
                            smallestDistance = distance;

            return smallestDistance;
        }
    }
}