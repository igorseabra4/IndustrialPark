using HipHopFile;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class CollectionEditor : Form
    {
        public static Array Get(Game game, Type type, object[] items, AssetPropertyCollectionOptionsAttribute options)
        {
            var editor = new CollectionEditor(game, type, items, options);
            editor.ShowDialog();

            if (editor.OK)
            {
                Array result = Array.CreateInstance(editor.type, editor.listBoxItems.Items.Count);
                for (int i = 0; i < editor.listBoxItems.Items.Count; i++)
                    result.SetValue(((DynamicTypeDescriptor)editor.listBoxItems.Items[i]).Component, i);
                return result;
            }
            return null;
        }

        private readonly Game game;
        private readonly Type type;

        private CollectionEditor(Game game, Type type, object[] items, AssetPropertyCollectionOptionsAttribute options)
        {
            InitializeComponent();
            TopMost = true;

            this.game = game;
            this.type = type;

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
            }
            Text = GetFormTitle();

            listBoxItems.Items.Clear();
            foreach (var item in items)
                listBoxItems.Items.Add(item);
        }

        private int programaticallyChangingSelection = 0;
        private bool OK = false;

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            listBoxItems.Items.Add(CreateNewInstance());
            listBoxItems.SelectedIndices.Clear();
            listBoxItems.SelectedIndex = listBoxItems.Items.Count - 1;
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
            var items = listBoxItems.SelectedItems.Cast<DynamicTypeDescriptor>().Select(x => ((GenericAssetDataContainer)x.Component).Serialize(Endianness.Little)).ToArray();
            Clipboard.SetText(JsonConvert.SerializeObject(items));
        }

        private void buttonPaste_Click(object sender, EventArgs e)
        {
            try
            {
                var clipboard = JsonConvert.DeserializeObject<byte[][]>(Clipboard.GetText());
                listBoxItems.SelectedIndices.Clear();
                foreach (var item in clipboard)
                {
                    object instance = CreatePastedInstance(item);
                    listBoxItems.Items.Add(DynamicTypeDescriptor.Create(instance));
                    listBoxItems.SetSelected(listBoxItems.Items.Count - 1, true);
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
            (listBoxItems.Items[index2], listBoxItems.Items[index1]) = (listBoxItems.Items[index1], listBoxItems.Items[index2]);
        }

        private void buttonArrowUp_Click(object sender, EventArgs e)
        {
            var selectedIndices = listBoxItems.SelectedIndices.Cast<int>().ToList();
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

        private object CreateNewInstance()
        {
            //if (type.Equals(typeof(EntryLODT)))
            //{
            //    var instance = new EntryLODT(game);
            //    return DynamicTypeDescriptor.Create(instance);
            //}

            ConstructorInfo constructor = type.GetConstructor([typeof(Game)]) ?? type.GetConstructor(Type.EmptyTypes);
            if (constructor != null)
            {
                object[] args = constructor.GetParameters().Length == 0 ? [] : [game];
                var instance = constructor.Invoke(args);
                return DynamicTypeDescriptor.Create(instance);
            }
            throw new ArgumentException("Unable to create new item of type.");
        }

        private object CreatePastedInstance(byte[] item)
        {
            using (var reader = new EndianBinaryReader(item, Endianness.Little))
            {
                //if (type.Equals(typeof(EntryLODT)))
                //    return new EntryLODT(reader, game);

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