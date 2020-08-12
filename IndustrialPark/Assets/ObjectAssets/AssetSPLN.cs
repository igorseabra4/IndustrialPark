using HipHopFile;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public class AssetSPLN : BaseAsset, IRenderableAsset
    {
        public AssetSPLN(Section_AHDR AHDR, Game game, Platform platform, SharpRenderer renderer) : base(AHDR, game, platform)
        {
            Setup(renderer);
            CreateTransformMatrix();
            ArchiveEditorFunctions.renderableAssets.Add(this);
        }

        public override bool HasReference(uint assetID) => 
            UnknownHash_14 == assetID || UnknownHash_18 == assetID || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Verify(UnknownHash_14, ref result);
            Verify(UnknownHash_18, ref result);
        }

        [Category("Spline")]
        public int UnknownAlways3
        {
            get => ReadInt(0x08);
            set => Write(0x08, value);
        }
        [Category("Spline"), ReadOnly(true)]
        public int PointCountPlus3
        {
            get => ReadInt(0x0C);
            set => Write(0x0C, value);
        }
        [Category("Spline"), ReadOnly(true)]
        public int PointCountMinus1
        {
            get => ReadInt(0x10);
            set => Write(0x10, value);
        }
        [Category("Spline")]
        public AssetID UnknownHash_14
        {
            get => ReadUInt(0x14);
            set => Write(0x14, value);
        }
        [Category("Spline")]
        public AssetID UnknownHash_18
        {
            get => ReadUInt(0x18);
            set => Write(0x18, value);
        }
        private const int pointStart = 0x1C;
        [Category("Spline")]
        public WireVector[] Points
        {
            get
            {
                int pointCount = PointCountPlus3 - 3;
                var points = new WireVector[pointCount];
                for (int i = 0; i < pointCount; i++)
                    points[i] = new WireVector(
                        ReadFloat(pointStart + i * 0xC),
                        ReadFloat(pointStart + i * 0xC + 4),
                        ReadFloat(pointStart + i * 0xC + 8));
                return points;
            }
            set
            {
                List<byte> dataBefore = Data.Take(pointStart).ToList();
                
                foreach (var v in value)
                {
                    dataBefore.AddRange(BitConverter.GetBytes(Switch(v.X)));
                    dataBefore.AddRange(BitConverter.GetBytes(Switch(v.Y)));
                    dataBefore.AddRange(BitConverter.GetBytes(Switch(v.Z)));
                }
                dataBefore.AddRange(new byte[16]);

                float acc = 0;
                for (int i = 0; i < value.Length - 1; i++)
                {
                    acc += Distance(value[i], value[i + 1]);
                    dataBefore.AddRange(BitConverter.GetBytes(Switch(acc)));
                }
                dataBefore.AddRange(BitConverter.GetBytes(Switch(acc)));

                Data = dataBefore.ToArray();
                PointCountPlus3 = value.Length + 3;
                PointCountMinus1 = value.Length - 1;
                Setup(Program.MainForm.renderer);
            }
        }

        private static float Distance(WireVector v1, WireVector v2) =>
            Vector3.Distance(new Vector3(v1.X, v1.Y, v1.Z), new Vector3(v2.X, v2.Y, v2.Z));

        private BoundingBox boundingBox;

        public static bool dontRender = false;

        private SharpDX.Direct3D11.Buffer vertexBuffer;
        private int vertexCount;

        public void Setup(SharpRenderer renderer)
        {
            renderer.completeVertexBufferList.Remove(vertexBuffer);
            if (vertexBuffer != null)
                vertexBuffer.Dispose();
            
            vertexBuffer = SharpDX.Direct3D11.Buffer.Create(renderer.device.Device, BindFlags.VertexBuffer, Points);
            renderer.completeVertexBufferList.Add(vertexBuffer);
            vertexCount = PointCountMinus1 + 1;
        }

        public bool ShouldDraw(SharpRenderer renderer)
        {
            if (isSelected)
                return true;
            if (dontRender)
                return false;
            if (isInvisible)
                return false;
            
            return renderer.frustum.Intersects(ref boundingBox);
        }

        public void Draw(SharpRenderer renderer)
        {
            renderer.DrawSpline(vertexBuffer, vertexCount, Matrix.Identity,
                isSelected ? renderer.selectedObjectColor : Color.YellowGreen.ToVector4(), false);
        }

        public void CreateTransformMatrix()
        {
            if (Points.Length == 0)
                boundingBox = new BoundingBox();
            else
                boundingBox = new BoundingBox(
                    new Vector3(Points[0].X, Points[0].Y, Points[0].Z),
                    new Vector3(Points[0].X, Points[0].Y, Points[0].Z));

            foreach (WireVector v in Points)
            {
                if (v.X > boundingBox.Maximum.X)
                    boundingBox.Maximum.X = v.X;
                if (v.Y > boundingBox.Maximum.Y)
                    boundingBox.Maximum.Y = v.Y;
                if (v.Z > boundingBox.Maximum.Z)
                    boundingBox.Maximum.Z = v.Z;
                if (v.X < boundingBox.Minimum.X)
                    boundingBox.Minimum.X = v.X;
                if (v.Y < boundingBox.Minimum.Y)
                    boundingBox.Minimum.Y = v.Y;
                if (v.Z < boundingBox.Minimum.Z)
                    boundingBox.Minimum.Z = v.Z;
            }
        }
        
        public float GetDistanceFrom(Vector3 cameraPosition)
        {
            return Vector3.Distance(cameraPosition, boundingBox.Center);
        }

        public float? GetIntersectionPosition(SharpRenderer renderer, Ray ray) => null;

        public void Dispose()
        {
            vertexBuffer.Dispose();
        }

        [Browsable(false)]
        public bool SpecialBlendMode => false;
    }
}