using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace IndustrialPark
{
    public class SoundHeaderEditor : UITypeEditor
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
                byte[] array = (byte[])value;

                OpenFileDialog openFile = new OpenFileDialog()
                {
                    Title = "Select sound file with header to import"
                };

                if (openFile.ShowDialog() == DialogResult.OK)
                    value = File.ReadAllBytes(openFile.FileName).Take(0x60).ToArray();
            }

            return value;
        }
    }
}
