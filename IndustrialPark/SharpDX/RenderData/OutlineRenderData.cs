using SharpDX;
using System.Runtime.InteropServices;

namespace IndustrialPark.RenderData
{
    [StructLayout(LayoutKind.Sequential)]
    public struct OutlineRenderData
    {
        public Vector4 Color;
        public Vector4 Settings;
    }
}
