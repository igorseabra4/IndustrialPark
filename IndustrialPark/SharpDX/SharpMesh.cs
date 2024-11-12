using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using Buffer11 = SharpDX.Direct3D11.Buffer;

namespace IndustrialPark
{
    public class SharpMesh : IDisposable
    {
        private Buffer11 VertexBuffer;
        private Buffer11 IndexBuffer;
        private int VertexSize;
        public List<SharpSubSet> SubSets { get; private set; }
        private int IndexCount;
        private PrimitiveTopology primitiveTopology;

        private bool isTriStrip = false;
#if DEBUG
        public int VerticesAmount = 0;
#endif

        public void Draw(SharpDevice Device)
        {
            Device.DeviceContext.InputAssembler.PrimitiveTopology = primitiveTopology;
            Device.DeviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, VertexSize, 0));
            Device.DeviceContext.InputAssembler.SetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);
            Device.DeviceContext.DrawIndexed(IndexCount, 0, 0);
#if DEBUG
            SharpRenderer.TotalVerticesDrawn += VerticesAmount;
            SharpRenderer.TotalDrawCalls++;
#endif
        }

        // indexed with subsets
        public static SharpMesh Create<VType>(SharpDevice device, VType[] vertices, int[] indices, List<SharpSubSet> SubSets, PrimitiveTopology topology = PrimitiveTopology.TriangleList) where VType : struct
        {
            return new SharpMesh()
            {
#if DEBUG
                VerticesAmount = vertices.Length,
#endif
                VertexBuffer = Buffer11.Create<VType>(device.Device, BindFlags.VertexBuffer, vertices),
                IndexBuffer = Buffer11.Create(device.Device, BindFlags.IndexBuffer, indices),
                VertexSize = Utilities.SizeOf<VType>(),
                SubSets = SubSets,
                primitiveTopology = topology,
            };
        }

        // unindexed with subsets
        public static SharpMesh Create<VType>(SharpDevice device, VType[] vertices, List<SharpSubSet> SubSets, PrimitiveTopology topology = PrimitiveTopology.TriangleStrip) where VType : struct
        {
            return new SharpMesh()
            {
#if DEBUG
                VerticesAmount = vertices.Length,
#endif
                VertexBuffer = Buffer11.Create<VType>(device.Device, BindFlags.VertexBuffer, vertices),
                IndexBuffer = null,
                isTriStrip = true,
                VertexSize = Utilities.SizeOf<VType>(),
                SubSets = SubSets,
                IndexCount = 0,
                primitiveTopology = topology
            };
        }

        // indexed without subsets
        public static SharpMesh Create<VType>(SharpDevice device, VType[] vertices, int[] indices, PrimitiveTopology topology = PrimitiveTopology.TriangleList) where VType : struct
        {
            return new SharpMesh()
            {
#if DEBUG
                VerticesAmount = vertices.Length,
#endif
                VertexBuffer = Buffer11.Create<VType>(device.Device, BindFlags.VertexBuffer, vertices),
                IndexBuffer = Buffer11.Create(device.Device, BindFlags.IndexBuffer, indices),
                VertexSize = Utilities.SizeOf<VType>(),
                SubSets = null,
                IndexCount = indices.Length,
                primitiveTopology = topology,
            };
        }

        /// <summary>
        /// Set all buffer and topology property to speed up rendering
        /// </summary>
        public void Begin(SharpDevice Device)
        {
            Device.DeviceContext.InputAssembler.PrimitiveTopology = primitiveTopology;
            Device.DeviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, VertexSize, 0));
            Device.DeviceContext.InputAssembler.SetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);
        }

        public void Draw(SharpDevice Device, int subset)
        {
            Device.DeviceContext.PixelShader.SetShaderResource(0, SubSets[subset].DiffuseMap);
            if (isTriStrip)
                Device.DeviceContext.Draw(SubSets[subset].IndexCount, SubSets[subset].StartIndex);
            else
                Device.DeviceContext.DrawIndexed(SubSets[subset].IndexCount, SubSets[subset].StartIndex, 0);
#if DEBUG
            SharpRenderer.TotalDrawCalls++;
#endif
        }

        public void DrawPoints(SharpDevice Device, int count = int.MaxValue)
        {
            Device.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.PointList;
            Device.DeviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, VertexSize, 0));
            Device.DeviceContext.InputAssembler.SetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);
            Device.DeviceContext.DrawIndexed(Math.Min(count, SubSets[0].IndexCount), 0, 0);
#if DEBUG
            SharpRenderer.TotalDrawCalls++;
#endif
        }

        public void Dispose()
        {
            if (VertexBuffer != null)
                VertexBuffer.Dispose();
            if (IndexBuffer != null)
                IndexBuffer.Dispose();
        }

        public void DisposeWithTextures()
        {
            if (VertexBuffer != null)
                VertexBuffer.Dispose();
            if (IndexBuffer != null)
                IndexBuffer.Dispose();

            foreach (var s in SubSets)
            {
                if (s.DiffuseMap != null)
                    s.DiffuseMap.Dispose();
            }
        }
    }
}