using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace IndustrialPark
{
    /// <summary>
    /// The default ToolStrip renderer, but item images are resized with high quality interpolation
    /// instead of the blocky default when they don't match the strip's image size.
    /// </summary>
    public class SmoothImageToolStripRenderer : ToolStripProfessionalRenderer
    {
        protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
        {
            var interpolationMode = e.Graphics.InterpolationMode;
            var pixelOffsetMode = e.Graphics.PixelOffsetMode;

            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            base.OnRenderItemImage(e);

            e.Graphics.InterpolationMode = interpolationMode;
            e.Graphics.PixelOffsetMode = pixelOffsetMode;
        }
    }
}
