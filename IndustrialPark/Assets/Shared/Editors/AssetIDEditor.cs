using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;

namespace IndustrialPark
{
    public class AssetIDEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (value is AssetID assetID && Program.MainForm != null && assetID != 0)
                foreach (ArchiveEditor archiveEditor in Program.MainForm.archiveEditors)
                    archiveEditor.OpenInternalEditors(new List<uint>() { assetID }, true);

            return value;
        }
    }
}
