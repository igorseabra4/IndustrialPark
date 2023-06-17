using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace IndustrialPark
{
    public class AtblAddEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            return "add_state_effect";
        }
    }
}
