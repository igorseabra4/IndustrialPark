using HipHopFile;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace IndustrialPark
{
    public class DynamicTypeDescriptorCollectionEditor : CollectionEditor
    {
        private string type;
        public static Game game { get; set; }

        public DynamicTypeDescriptorCollectionEditor(Type type) : base(type)
        {
            this.type = type.Name;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (value is object[] array)
            {
                var newValue = new List<DynamicTypeDescriptor>();
                foreach (var item in array)
                {
                    DynamicTypeDescriptor dt = new DynamicTypeDescriptor(item.GetType());
                    if (item is GenericAssetDataContainer gadc)
                        gadc.SetDynamicProperties(dt);
                    newValue.Add(dt.FromComponent(item));
                }

                var result = (List<DynamicTypeDescriptor>)base.EditValue(context, provider, newValue);

                var newResult = new List<object>();
                foreach (var dt in result)
                    newResult.Add(dt.Component);

                switch (type)
                {
                    case "Shrapnel[]":
                        return newResult.Cast<Shrapnel>().ToArray();
                    case "AnimationEffect[]":
                        return newResult.Cast<AnimationEffect>().ToArray();
                    case "AnimationState[]":
                        return newResult.Cast<AnimationState>().ToArray();
                    case "PipeInfo[]":
                        return newResult.Cast<PipeInfo>().ToArray();
                    case "EntryLODT[]":
                        return newResult.Cast<EntryLODT>().ToArray();
                    case "CreditsPreset[]":
                        return newResult.Cast<CreditsPreset>().ToArray();
                }
            }

            return base.EditValue(context, provider, value);
        }

        protected override object CreateInstance(Type itemType)
        {
            Type type = CollectionType.GetElementType();
            ConstructorInfo constructor = type.GetConstructor(new[] { typeof(Game) });

            if (constructor != null)
            {
                object instance = constructor.Invoke(new object[] { game });

                DynamicTypeDescriptor dt = new DynamicTypeDescriptor(type);
                if (instance is GenericAssetDataContainer gadc)
                    gadc.SetDynamicProperties(dt);
                return dt.FromComponent(instance);
            }
            return base.CreateInstance(itemType);
        }
    }
}
