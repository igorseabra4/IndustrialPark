using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace IndustrialPark.AssetEditorColors;

public class AssetColorEditor : UITypeEditor
{
    private static ColorDialog ColorDialog;

    public static (DialogResult Result, Color Color) GetColorFromDialog()
    {
        ColorDialog ??= new ColorDialog
        {
            AnyColor = true,
            FullOpen = true,
            SolidColorOnly = false,
            ShowHelp = true,
        };

        return (ColorDialog.ShowDialog(), ColorDialog.Color);
    }

    public static (DialogResult Result, Color Color) GetColorFromDialog(Color color)
    {
        ColorDialog ??= new ColorDialog
        {
            AnyColor = true,
            Color = color,
            FullOpen = true,
            SolidColorOnly = false,
            ShowHelp = true,
        };

        ColorDialog.Color = color;

        return (ColorDialog.ShowDialog(), ColorDialog.Color);
    }

    public static (DialogResult Result, Color Color) GetColorFromDialog(SharpDX.Vector4 color)
    {
        return GetColorFromDialog(color.ToColor());
    }

    public static (DialogResult Result, Color Color) GetColorFromDialog(AssetColor color)
    {
        return GetColorFromDialog(Color.FromArgb(color.GetARGB()));
    }

    private IWindowsFormsEditorService service;

    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
    {
        return UITypeEditorEditStyle.Modal;
    }

    public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
    {
        if (provider != null)
            service = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

        if (service != null)
        {
            if (value is AssetColor color)
            {
                var result = GetColorFromDialog(color);
                if (result.Result == DialogResult.OK)
                    value = new AssetColor(result.Color.ToArgb());
            }
        }

        return value;
    }

    public override bool GetPaintValueSupported(ITypeDescriptorContext context)
    {
        return true;
    }

    public override void PaintValue(PaintValueEventArgs e)
    {
        if (e.Value is AssetColor)
        {
            var b = new SolidBrush(Color.FromArgb(((AssetColor)e.Value).GetARGB()));
            e.Graphics.FillRectangle(b, e.Bounds);
            b.Dispose();
        }
    }
}
