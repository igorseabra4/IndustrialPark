using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace IndustrialPark
{
    public class EventListEditor : UITypeEditor
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
                if (value is AssetEventBFBB[])
                {
                    AssetEventBFBB[] events = EventEditor.GetEvents((AssetEventBFBB[])value, out bool success);

                    if (success)
                        value = events;
                }
                else if (value is AssetEventTSSM[])
                {
                    AssetEventTSSM[] events = EventEditor.GetEvents((AssetEventTSSM[])value, out bool success);

                    if (success)
                        value = events;
                }
                else if (value is AssetEventIncredibles[])
                {
                    AssetEventIncredibles[] events = EventEditor.GetEvents((AssetEventIncredibles[])value, out bool success);

                    if (success)
                        value = events;
                }
            }

            return value;
        }
    }
}
