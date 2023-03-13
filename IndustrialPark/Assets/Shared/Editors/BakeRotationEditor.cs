using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace IndustrialPark
{
    public class BakeRotationEditor : UITypeEditor
    {
        public static uint modelAssetId;
        public static float yaw;
        public static float pitch;
        public static float roll;

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
                return ArchiveEditorFunctions.ApplyBakeRotation(modelAssetId, yaw, pitch, roll).ToString();
            
            return "unchanged";
        }
    }
}
