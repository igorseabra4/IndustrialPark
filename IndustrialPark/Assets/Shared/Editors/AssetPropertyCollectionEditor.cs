using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;

namespace IndustrialPark
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AssetPropertyCollectionOptionsAttribute : Attribute
    {
        public bool AllowAdd { get; set; }
        public bool AllowRemove { get; set; }
        public bool AllowReorder { get; set; }
        public bool AllowCopy { get; set; }

        public AssetPropertyCollectionOptionsAttribute(bool allowAdd = true, bool allowRemove = true, bool allowCopy = true, bool allowReorder = true)
        {
            AllowAdd = allowAdd;
            AllowRemove = allowRemove;
            AllowReorder = allowReorder;
            AllowCopy = allowCopy;
        }
    }

    public class AssetPropertyCollectionEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            AssetPropertyCollectionOptionsAttribute options = null;
            if (context?.PropertyDescriptor?.Attributes[typeof(AssetPropertyCollectionOptionsAttribute)] is AssetPropertyCollectionOptionsAttribute options1)
                options = options1;

            var array = value as Array;

            var parent = (GenericAssetDataContainer)((DynamicTypeDescriptor)context.Instance).Component;

            // use wrapper for asset ids
            if (array.GetType().GetElementType().Equals(typeof(AssetID)))
                array = array.Cast<AssetID>().Select(x => new AssetIdWrapper(x)).ToArray();

            var result = CollectionEditor.Get(
                parent.game,
                array.GetType().GetElementType(),
                array.Cast<object>().Select(x => DynamicTypeDescriptor.Create(x)).ToArray(),
                options);

            // use wrapper for asset ids
            if (result != null && result is Array resultArray && resultArray.GetType().GetElementType().Equals(typeof(AssetIdWrapper)))
                return resultArray.Cast<AssetIdWrapper>().Select(x => x.Value).ToArray();

            return result ?? value;
        }
    }
}