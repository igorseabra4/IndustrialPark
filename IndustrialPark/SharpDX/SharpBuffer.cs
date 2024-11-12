using SharpDX.Direct3D11;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark
{
    using Buffer = SharpDX.Direct3D11.Buffer;
    public class ConstantBuffer<T> : IDisposable where T : struct
    {
        private readonly Device _device;
        private readonly Buffer _buffer;
        private readonly DataStream _dataStream;

        public Buffer Buffer
        {
            get { return _buffer; }
        }

        public ConstantBuffer(Device device)
        {
            _device = device;

            // If no specific marshalling is needed, can use
            // SharpDX.Utilities.SizeOf<T>() for better performance.
            int size = Marshal.SizeOf(typeof(T));

            _buffer = new Buffer(device, new BufferDescription
            {
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.ConstantBuffer,
                SizeInBytes = size,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                StructureByteStride = 0
            });

            _dataStream = new DataStream(size, true, true);
        }

        public void UpdateValue(T value)
        {
            // If no specific marshalling is needed, can use 
            // dataStream.Write(value) for better performance.
            Marshal.StructureToPtr(value, _dataStream.DataPointer, false);

            var dataBox = new DataBox(_dataStream.DataPointer, 0, 0);
            _device.ImmediateContext.UpdateSubresource(dataBox, _buffer, 0);
        }

        public void Dispose()
        {
            if (_dataStream != null)
                _dataStream.Dispose();
            if (_buffer != null)
                _buffer.Dispose();
        }
    }
}
