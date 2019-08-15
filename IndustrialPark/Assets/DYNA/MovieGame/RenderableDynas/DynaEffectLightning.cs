using AssetEditorColors;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaEffectLightning : DynaBase
    {
        public override string Note => "Version is always 2";

        public DynaEffectLightning() : base() { }

        public override bool HasReference(uint assetID)
        {
            return LightningTexture_AssetID == assetID || GlowTexture_AssetID == assetID || SIMP1_AssetID == assetID || SIMP2_AssetID == assetID;
        }

        public override void Verify(ref List<string> result)
        {
            Asset.Verify(LightningTexture_AssetID, ref result);
            Asset.Verify(GlowTexture_AssetID, ref result);
            Asset.Verify(SIMP1_AssetID, ref result);
            Asset.Verify(SIMP2_AssetID, ref result);
        }

        public DynaEffectLightning(IEnumerable<byte> enumerable) : base (enumerable)
        {
            _position = new Vector3(
                Switch(BitConverter.ToSingle(Data, 0x00)),
                Switch(BitConverter.ToSingle(Data, 0x04)),
                Switch(BitConverter.ToSingle(Data, 0x08)));
            _position2 = new Vector3(
                Switch(BitConverter.ToSingle(Data, 0x0C)),
                Switch(BitConverter.ToSingle(Data, 0x10)),
                Switch(BitConverter.ToSingle(Data, 0x14)));
            ColorR = Data[0x18];
            ColorG = Data[0x19];
            ColorB = Data[0x1A];
            ColorAlpha = Data[0x1B];
            Width = Switch(BitConverter.ToSingle(Data, 0x1C));
            UnknownFloat = Switch(BitConverter.ToSingle(Data, 0x20));
            LightningTexture_AssetID = Switch(BitConverter.ToUInt32(Data, 0x24));
            GlowTexture_AssetID = Switch(BitConverter.ToUInt32(Data, 0x28));
            UnknownInt1 = Switch(BitConverter.ToInt32(Data, 0x2C));
            KnockbackSpeed = Switch(BitConverter.ToSingle(Data, 0x30));
            SoundGroupID_AssetID = Switch(BitConverter.ToUInt32(Data, 0x34));
            UnknownInt2 = Switch(BitConverter.ToInt32(Data, 0x38));
            UnknownInt3 = Switch(BitConverter.ToInt32(Data, 0x3C));
            SIMP1_AssetID = Switch(BitConverter.ToUInt32(Data, 0x40));
            SIMP2_AssetID = Switch(BitConverter.ToUInt32(Data, 0x44));
            DamagePlayer = Switch(BitConverter.ToInt32(Data, 0x48));
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(_position.X)));
            list.AddRange(BitConverter.GetBytes(Switch(_position.Y)));
            list.AddRange(BitConverter.GetBytes(Switch(_position.Z)));
            list.AddRange(BitConverter.GetBytes(Switch(_position2.X)));
            list.AddRange(BitConverter.GetBytes(Switch(_position2.Y)));
            list.AddRange(BitConverter.GetBytes(Switch(_position2.Z)));
            list.Add(ColorR);
            list.Add(ColorG);
            list.Add(ColorB);
            list.Add(ColorAlpha);
            list.AddRange(BitConverter.GetBytes(Switch(Width)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat)));
            list.AddRange(BitConverter.GetBytes(Switch(LightningTexture_AssetID)));
            list.AddRange(BitConverter.GetBytes(Switch(GlowTexture_AssetID)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt1)));
            list.AddRange(BitConverter.GetBytes(Switch(KnockbackSpeed)));
            list.AddRange(BitConverter.GetBytes(Switch(SoundGroupID_AssetID)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt2)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt3)));
            list.AddRange(BitConverter.GetBytes(Switch(SIMP1_AssetID)));
            list.AddRange(BitConverter.GetBytes(Switch(SIMP2_AssetID)));
            list.AddRange(BitConverter.GetBytes(Switch(DamagePlayer)));
            return list.ToArray();
        }

        private Vector3 _position;
        private Vector3 _position2;

        [Category("Effect Lightning"), Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionX
        {
            get => _position.X;
            set
            {
                _position.X = value;
                CreateTransformMatrix();
            }
        }
        [Category("Effect Lightning"), Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionY
        {
            get => _position.Y;
            set
            {
                _position.Y = value;
                CreateTransformMatrix();
            }
        }
        [Category("Effect Lightning"), Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionZ
        {
            get => _position.Z;
            set
            {
                _position.Z = value;
                CreateTransformMatrix();
            }
        }

        [Category("Effect Lightning"), TypeConverter(typeof(FloatTypeConverter))]
        public float PositionEndX
        {
            get => _position2.X;
            set
            {
                _position2.X = value;
                CreateTransformMatrix();
            }
        }
        [Category("Effect Lightning"), Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public float PositionEndY
        {
            get => _position2.Y;
            set
            {
                _position2.Y = value;
                CreateTransformMatrix();
            }
        }
        [Category("Effect Lightning"), Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public float PositionEndZ
        {
            get => _position2.Z;
            set
            {
                _position2.Z = value;
                CreateTransformMatrix();
            }
        }

        private byte ColorR;
        private byte ColorG;
        private byte ColorB;

        [Category("Effect Lightning"), Editor(typeof(MyColorEditor), typeof(UITypeEditor)), DisplayName("Color 1 (R, G, B)")]
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
        [Category("Effect Lightning"), DisplayName("Color 1 Alpha (0 - 255)")]
        public byte ColorAlpha { get; set; }

        [Category("Effect Lightning"), TypeConverter(typeof(FloatTypeConverter))]
        public float Width { get; set; }

        [Category("Effect Lightning"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat { get; set; }

        [Category("Effect Lightning")]
        public AssetID LightningTexture_AssetID { get; set; }

        [Category("Effect Lightning")]
        public AssetID GlowTexture_AssetID { get; set; }

        [Category("Effect Lightning")]
        public int UnknownInt1 { get; set; }

        [Category("Effect Lightning"), TypeConverter(typeof(FloatTypeConverter))]
        public float KnockbackSpeed { get; set; }

        [Category("Effect Lightning")]
        public AssetID SoundGroupID_AssetID { get; set; }

        [Category("Effect Lightning")]
        public int UnknownInt2 { get; set; }

        [Category("Effect Lightning")]
        public int UnknownInt3 { get; set; }

        [Category("Effect Lightning")]
        public AssetID SIMP1_AssetID { get; set; }

        [Category("Effect Lightning")]
        public AssetID SIMP2_AssetID { get; set; }

        [Category("Effect Lightning")]
        public int DamagePlayer { get; set; }
        
        public override bool IsRenderableClickable => true;

        private Matrix world;
        private Matrix world2;
        private BoundingBox boundingBox;
        private Vector3[] vertices;
        protected RenderWareFile.Triangle[] triangles;

        public override void CreateTransformMatrix()
        {
            world = Matrix.Translation(_position);
            world2 = Matrix.Translation(_position2);

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
            if (ray.Intersects(ref boundingBox, out float distance))
                return TriangleIntersection(ray, distance);
            return null;
        }

        private float? TriangleIntersection(Ray r, float initialDistance)
        {
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
            else return null;
        }

        public override BoundingSphere GetGizmoCenter()
        {
            BoundingSphere boundingSphere = BoundingSphere.FromBox(boundingBox);
            boundingSphere.Radius *= 0.9f;
            return boundingSphere;
        }
    }
}