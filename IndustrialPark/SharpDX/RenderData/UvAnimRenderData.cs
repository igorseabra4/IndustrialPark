using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark.RenderData
{
    [StructLayout(LayoutKind.Sequential)]
    public struct UvAnimRenderData
    {
        public Matrix worldViewProjection;
        public Vector4 Color;
        public Vector4 UvAnimOffset;
    }
}
