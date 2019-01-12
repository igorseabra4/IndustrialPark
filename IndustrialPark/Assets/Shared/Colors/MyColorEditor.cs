using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace AssetEditorColors
{
    public class MyColorEditor : UITypeEditor
    {
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
                ColorDialog selectionControl = new ColorDialog
                {
                    AnyColor = true,
                    Color = Color.FromArgb(((MyColor)value).GetARGB()),
                    FullOpen = true,
                    SolidColorOnly = false
                };

                if (selectionControl.ShowDialog() == DialogResult.OK)
                    value = new MyColor(selectionControl.Color.ToArgb());
            }

            return value;
        }
    }
}
