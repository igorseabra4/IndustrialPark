using HipHopFile;
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
        public static uint thisAssetID { get; set; }
        public static Game game { get; set; }

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
                if (value is Link[] links)
                {
                    var newLinks = LinkEditor.GetLinks(game, links, IsTimed, thisAssetID);

                    if (newLinks != null)
                        value = newLinks;
                }
            }

            IsTimed = false;
            return value;
        }
    }
}
