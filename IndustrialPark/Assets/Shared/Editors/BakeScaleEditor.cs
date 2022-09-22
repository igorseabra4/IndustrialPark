using IndustrialPark;
using SharpDX;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace AssetEditorColors
{
    public class BakeScaleEditor : UITypeEditor
    {
        public static uint modelAssetId;
        public static Vector3 scale;

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
                return ArchiveEditorFunctions.ApplyBakeScale(modelAssetId, scale).ToString();
            
            return "unchanged";
        }
    }
}
