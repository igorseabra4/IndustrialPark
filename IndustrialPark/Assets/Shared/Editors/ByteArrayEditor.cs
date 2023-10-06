using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;

namespace IndustrialPark
{
    public class ByteArrayEditor : UITypeEditor
    {
        public static bool IsImport;
        public static byte[] Data;

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (IsImport)
            {
                var openFile = new OpenFileDialog();
                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    Data = File.ReadAllBytes(openFile.FileName);
                    return "imported_replace";
                }
            }
            else
            {
                var saveFile = new SaveFileDialog();
                if (saveFile.ShowDialog() == DialogResult.OK)
                    File.WriteAllBytes(saveFile.FileName, Data);
            }
            return null;
        }
    }
}
