using HipHopFile;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class CollectionEditor : Form
    {
        public static Array Get(Game game, Type type, Array items, AssetPropertyCollectionOptionsAttribute options)
        {
            var editor = new CollectionEditor(game, type, items, options);
            editor.ShowDialog();

            if (editor.OK)
            {
                Array result = Array.CreateInstance(editor.type, editor.allItems.Count);
                for (int i = 0; i < editor.allItems.Count; i++)
                    result.SetValue(editor.isGenericAssetData ? ((DynamicTypeDescriptor)editor.allItems[i]).Component : editor.allItems[i], i);
                return result;
            }
            return null;
        }

        private readonly Game game;
        private readonly Type type;
        private readonly bool isGenericAssetData;

        // all items, including those hidden by the search filter; listBoxItems only shows the filtered ones
        private readonly List<object> allItems = new List<object>();

        private CollectionEditor(Game game, Type type, Array items, AssetPropertyCollectionOptionsAttribute options)
        {
            InitializeComponent();

            this.game = game;
            this.type = type;
            isGenericAssetData = typeof(GenericAssetDataContainer).IsAssignableFrom(type);

            if (options != null)
            {
                if (!options.AllowAdd)
                    buttonAdd.Visible = false;
                if (!options.AllowRemove)
                    buttonRemove.Visible = false;
                if (!options.AllowReorder)
                {
                    buttonArrowUp.Visible = false;
                    buttonArrowDown.Visible = false;
                }
                if (!options.AllowCopy)
                {
                    buttonCopy.Visible = false;
                    buttonPaste.Visible = false;
                }
                if (options.AllowSearch)
                {
                    textBoxSearch.Visible = true;
                    int listBottom = listBoxItems.Bottom;
                    listBoxItems.Top = textBoxSearch.Bottom + 6;
                    listBoxItems.Height = listBottom - listBoxItems.Top;
                }
            }

            CreateNewInstance = type.Name switch
            {
                "ParticleCommand" => () =>
                {
                    (var type, bool ok) = ChooseGeneric<ParticleCommandType>.Get();
                    return ok ? AssetPARS.CreateEntry(type) : null;
                }
                ,
                "Shrapnel" => () => {
                    (var type, bool ok) = ChooseGeneric<IShrapnelType>.Get(AssetSHRP.GetValidShrpTypes(game));
                    return ok ? AssetSHRP.CreateEntry(type, game) : null;
                },
                "UIMCommand" => () => {
                    (var type, bool ok) = ChooseGeneric<UIMCommandType>.Get();
                    return ok ? AssetUIM.CreateEntry(type) : null;
                },
                _ => CreateNewInstanceDefault,
            };

            Text = GetFormTitle();

            listBoxItems.Items.Clear();
            if (isGenericAssetData)
                items = items.Cast<GenericAssetDataContainer>().Select(DynamicTypeDescriptor.Create).ToArray();
            foreach (var item in items)
            {
                allItems.Add(item);
                listBoxItems.Items.Add(item);
            }
        }

        private int programaticallyChangingSelection = 0;
        private bool OK = false;

        private Func<object> CreateNewInstance;

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            var newItem = CreateNewInstance();
            if (newItem != null)
            {
                var item = DynamicTypeDescriptor.Create(newItem);
                allItems.Add(item);
                listBoxItems.Items.Add(item);
                listBoxItems.SelectedIndices.Clear();
                listBoxItems.SelectedIndex = listBoxItems.Items.Count - 1;
            }
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if (listBoxItems.SelectedIndices.Count > 0)
            {
                listBoxItems.BeginUpdate();

                var selection = new List<int>();
                foreach (int v in listBoxItems.SelectedIndices)
                    selection.Add(v);
                selection.Reverse();

                foreach (int index in selection)
                {
                    int Temp = listBoxItems.SelectedIndices[0];
                    listBoxItems.ClearSelected();
                    allItems.RemoveAt(IndexInAllItems(listBoxItems.Items[index]));
                    listBoxItems.Items.RemoveAt(index);
                    try
                    { listBoxItems.SelectedIndex = Temp; }
                    catch { listBoxItems.SelectedIndex = Temp - 1; }
                }

                listBoxItems.EndUpdate();
                listBoxItems_SelectedIndexChanged(sender, e);
            }
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            object[] items = isGenericAssetData ?
                listBoxItems.SelectedItems.Cast<DynamicTypeDescriptor>().Select(x => ((GenericAssetDataContainer)x.Component).Serialize(Endianness.Little)).ToArray() :
                listBoxItems.SelectedItems.Cast<object>().ToArray();
            Clipboard.SetText(JsonConvert.SerializeObject(items));
        }

        private void buttonPaste_Click(object sender, EventArgs e)
        {
            try
            {
                if (isGenericAssetData)
                {
                    var clipboard = JsonConvert.DeserializeObject<byte[][]>(Clipboard.GetText());
                    listBoxItems.SelectedIndices.Clear();
                    foreach (var item in clipboard)
                    {
                        var instance = DynamicTypeDescriptor.Create(CreatePastedInstance(item));
                        allItems.Add(instance);
                        listBoxItems.Items.Add(instance);
                        listBoxItems.SetSelected(listBoxItems.Items.Count - 1, true);
                    }
                }
                else
                {
                    var clipboard = (IEnumerable)JsonConvert.DeserializeObject(Clipboard.GetText(), typeof(IEnumerable<>).MakeGenericType(type));
                    listBoxItems.SelectedIndices.Clear();
                    foreach (var item in clipboard)
                    {
                        allItems.Add(item);
                        listBoxItems.Items.Add(item);
                        listBoxItems.SetSelected(listBoxItems.Items.Count - 1, true);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to paste items: " + ex.Message + " Are you sure you have items of the correct type copied?");
            }
        }

        private void listBoxItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (programaticallyChangingSelection > 0)
            {
                programaticallyChangingSelection--;
                return;
            }
            propertyGridItem.SelectedObjects = listBoxItems.SelectedItems.Cast<object>().ToArray();
        }

        public void SwapItems(int index1, int index2)
        {
            if (index1 < 0 || index1 >= listBoxItems.Items.Count || index2 < 0 || index2 >= listBoxItems.Items.Count)
                return;

            // the visible items may not be adjacent in allItems when filtered, so swap their positions there too
            int all1 = IndexInAllItems(listBoxItems.Items[index1]);
            int all2 = IndexInAllItems(listBoxItems.Items[index2]);
            (allItems[all2], allItems[all1]) = (allItems[all1], allItems[all2]);

            (listBoxItems.Items[index2], listBoxItems.Items[index1]) = (listBoxItems.Items[index1], listBoxItems.Items[index2]);
        }

        private int IndexInAllItems(object item) => allItems.FindIndex(x => ReferenceEquals(x, item));

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            var selected = listBoxItems.SelectedItems.Cast<object>().ToList();
            string search = textBoxSearch.Text.Trim();

            listBoxItems.BeginUpdate();
            listBoxItems.Items.Clear();
            foreach (var item in allItems)
                if (search.Length == 0 || item.ToString().Contains(search, StringComparison.OrdinalIgnoreCase))
                    listBoxItems.Items.Add(item);
            for (int i = 0; i < listBoxItems.Items.Count; i++)
                if (selected.Any(x => ReferenceEquals(x, listBoxItems.Items[i])))
                    listBoxItems.SetSelected(i, true);
            listBoxItems.EndUpdate();

            propertyGridItem.SelectedObjects = listBoxItems.SelectedItems.Cast<object>().ToArray();
        }

        // pressing Enter in the search box shouldn't trigger the OK button and close the dialog
        private void textBoxSearch_Enter(object sender, EventArgs e) => AcceptButton = null;
        private void textBoxSearch_Leave(object sender, EventArgs e) => AcceptButton = buttonOK;

        private void buttonArrowUp_Click(object sender, EventArgs e)
        {
            var selectedIndices = listBoxItems.SelectedIndices.Cast<int>().ToList();
            programaticallyChangingSelection++;
            listBoxItems.ClearSelected();
            foreach (var i in selectedIndices)
            {
                if (i <= 0)
                {
                    listBoxItems.SelectedIndices.Add(0);
                    continue;
                }
                SwapItems(i, i - 1);
                listBoxItems.SelectedIndices.Add(Math.Max(i - 1, 0));
            }
        }

        private void buttonArrowDown_Click(object sender, EventArgs e)
        {
            var selectedIndices = listBoxItems.SelectedIndices.Cast<int>().Reverse().ToList();
            programaticallyChangingSelection++;
            listBoxItems.ClearSelected();
            foreach (var i in selectedIndices)
            {
                if (i >= listBoxItems.Items.Count - 1)
                {
                    listBoxItems.SelectedIndices.Add(listBoxItems.Items.Count - 1);
                    continue;
                }
                SwapItems(i, i + 1);
                listBoxItems.SelectedIndices.Add(Math.Min(i + 1, listBoxItems.Items.Count - 1));
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            OK = true;
            Close();
        }

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(AboutBox.WikiLink + "Events");
        }

        private void propertyGridItem_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            var selectedIndices = listBoxItems.SelectedIndices.Cast<int>().ToList();
            programaticallyChangingSelection++;
            listBoxItems.SelectedIndices.Clear();
            foreach (int i in selectedIndices)
            {
                programaticallyChangingSelection++;
                listBoxItems.Items[i] = listBoxItems.Items[i];
            }
            foreach (var i in selectedIndices)
                listBoxItems.SetSelected(i, true);
        }

        private string GetFormTitle()
        {
            var result = type.ToString().Replace("IndustrialPark.", "");
            if (result == "AssetIdWrapper")
                result = "Asset ID";
            return result + " Collection Editor";
        }

        private object CreateNewInstanceDefault()
        {
            ConstructorInfo constructor = isGenericAssetData
                ? type.GetConstructor([typeof(Game)]) ?? type.GetConstructor(Type.EmptyTypes)
                : type.GetConstructor(Type.EmptyTypes);
            if (constructor != null)
            {
                object[] args = constructor.GetParameters().Length == 0 ? [] : [game];
                var instance = constructor.Invoke(args);
                return instance;
            }
            throw new ArgumentException("Unable to create new item of type.");
        }

        private object CreatePastedInstance(byte[] item)
        {
            using (var reader = new EndianBinaryReader(item, Endianness.Little))
            {
                ConstructorInfo constructor = type.GetConstructor([typeof(EndianBinaryReader), typeof(Game)]);
                if (constructor != null)
                    return constructor.Invoke([reader, game]);

                constructor = type.GetConstructor([typeof(EndianBinaryReader)]);
                if (constructor != null)
                    return constructor.Invoke([reader]);
            }
            throw new InvalidOperationException("Unable to create pasted item of type.");
        }
    }
}