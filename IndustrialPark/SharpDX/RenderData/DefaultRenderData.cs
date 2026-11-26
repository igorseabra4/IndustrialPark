using SharpDX;
using System.Runtime.InteropServices;

namespace IndustrialPark.RenderData;

[StructLayout(LayoutKind.Sequential)]
public struct DefaultRenderData
{
    public Matrix worldViewProjection;
    public Vector4 Color;
}
