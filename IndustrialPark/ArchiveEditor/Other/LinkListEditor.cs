using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace IndustrialPark
{
    public class LinkListEditor : UITypeEditor
    {
        private IWindowsFormsEditorService service;

        public static bool IsTimed { get; set; }
        public static Endianness endianness { get; set; }
        public static uint thisAssetID { get; set; }

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
                    LinkBFBB[] events = LinkEditor.GetEvents((LinkBFBB[])value, endianness, IsTimed, thisAssetID);

                    if (events != null)
                        value = events;
                }
                else if (value is LinkTSSM[])
                {
                    LinkTSSM[] events = LinkEditor.GetEvents((LinkTSSM[])value, endianness, IsTimed, thisAssetID);

                    if (events != null)
                        value = events;
                }
            }

            IsTimed = false;
            return value;
        }
    }
}
