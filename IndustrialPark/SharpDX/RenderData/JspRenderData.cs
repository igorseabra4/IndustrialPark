using IndustrialPark.RenderData;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark.RenderData
{
    [StructLayout(LayoutKind.Explicit, Size = 0x80)]
    public struct JspRenderData
    {
        [FieldOffset(0)]
        public Matrix worldViewProjection;

        [FieldOffset(0x40)]
        public Vector4 MaterialColor;

        [FieldOffset(0x50)]
        public Vector4 FogColor;

        [FieldOffset(0x60)]
        public float FogStart;

        [FieldOffset(0x64)]
        public float FogEnd;

        [FieldOffset(0x68)]
        public bool FogEnable;

        [FieldOffset(0x6C)]
        public bool VertexColorEnable;

        [FieldOffset(0x70)]
        public Vector4 SelectedObjectColor;
    }
}
