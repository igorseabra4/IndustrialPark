using System.Runtime.InteropServices;
using IndustrialPark.RenderData;
using Xunit;

namespace IndustrialParkTest
{
    public class RenderDataLayoutTests
    {
        [Fact]
        public void ConstantBufferRenderDataSizes_AreMultiplesOfSixteen()
        {
            Assert.Equal(0, Marshal.SizeOf<SharpDX.Matrix>() % 16);
            Assert.Equal(0, Marshal.SizeOf<JspRenderData>() % 16);
            Assert.Equal(0, Marshal.SizeOf<FogLightRenderData>() % 16);
        }

        [Fact]
        public void ExplicitRenderDataLayouts_MatchExpectedShaderSizes()
        {
            Assert.Equal(0x80, Marshal.SizeOf<JspRenderData>());
            Assert.Equal(0x210, Marshal.SizeOf<FogLightRenderData>());
        }
    }
}
