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
        private Buffer _buffer;
        private DataStream _dataStream;
        private readonly int _dataSize;

        public Buffer Buffer
        {
            get { return _buffer; }
        }

        public ConstantBuffer(Device device)
        {
            _device = device;

            // If no specific marshalling is needed, can use
            // SharpDX.Utilities.SizeOf<T>() for better performance.
            _dataSize = Marshal.SizeOf(typeof(T));
            int bufferSize = (_dataSize + 15) & ~15;

            _buffer = new Buffer(device, new BufferDescription
            {
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.ConstantBuffer,
                SizeInBytes = bufferSize,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                StructureByteStride = 0
            });

            _dataStream = new DataStream(bufferSize, true, true);
        }

        public void UpdateValue(T value)
        {
            ObjectDisposedException.ThrowIf(_buffer == null || _dataStream == null, this);

            // If no specific marshalling is needed, can use 
            // dataStream.Write(value) for better performance.
            Marshal.StructureToPtr(value, _dataStream.DataPointer, false);

            var dataBox = new DataBox(_dataStream.DataPointer, 0, 0);
            _device.ImmediateContext.UpdateSubresource(dataBox, _buffer, 0);
        }

        public void Dispose()
        {
            Utilities.Dispose(ref _dataStream);
            Utilities.Dispose(ref _buffer);
        }
    }
}
