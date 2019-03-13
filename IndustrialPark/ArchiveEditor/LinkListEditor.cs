using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace IndustrialPark
{
    public class LinkListEditor : UITypeEditor
    {
        private IWindowsFormsEditorService service;

        public static bool IsTimed { get; internal set; }

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
                if (value is LinkBFBB[])
                {
                    LinkBFBB[] events = LinkEditor.GetEvents((LinkBFBB[])value, out bool success, IsTimed);

                    if (success)
                        value = events;
                }
                else if (value is LinkTSSM[])
                {
                    LinkTSSM[] events = LinkEditor.GetEvents((LinkTSSM[])value, out bool success, IsTimed);

                    if (success)
                        value = events;
                }
                else if (value is LinkIncredibles[])
                {
                    LinkIncredibles[] events = LinkEditor.GetEvents((LinkIncredibles[])value, out bool success, IsTimed);

                    if (success)
                        value = events;
                }
            }

            IsTimed = false;
            return value;
        }
    }
}
