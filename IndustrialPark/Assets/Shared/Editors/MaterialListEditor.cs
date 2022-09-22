using RenderWareFile.Sections;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace IndustrialPark
{
    public class MaterialListEditor : UITypeEditor
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
                if (value is Material_0007[] materials)
                {
                    materials = MaterialEffectEditor.GetMaterials(materials);

                    if (materials != null)
                        value = materials;
                }
            }

            return value;
        }
    }
}
