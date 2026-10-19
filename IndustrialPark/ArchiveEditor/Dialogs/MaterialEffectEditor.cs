using RenderWareFile.Sections;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class MaterialEffectEditor : Form
    {
        private MaterialEffectEditor(Material_0007[] materials)
        {
            InitializeComponent();

            foreach (var m in materials)
                listBoxMaterials.Items.Add(m);

            comboBoxMatEffects.Items.Add(MaterialEffectType.NoEffect);
            comboBoxMatEffects.Items.Add(MaterialEffectType.BumpMap);
            comboBoxMatEffects.Items.Add(MaterialEffectType.EnvironmentMap);
            comboBoxMatEffects.Items.Add(MaterialEffectType.DualTextures);

            toolTip = new ToolTip();
        }

        public static Material_0007[] GetMaterials(Material_0007[] materials)
        {
            MaterialEffectEditor materialEffectEditor = new MaterialEffectEditor(materials);
            if (materialEffectEditor.ShowDialog() == DialogResult.OK)
                return [.. materialEffectEditor.listBoxMaterials.Items.Cast<Material_0007>()];
            return null;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private Material_0007[] currentMaterials => listBoxMaterials.SelectedItems.Cast<Material_0007>().ToArray();

        private bool programIsChangingStuff = false;

        private void listBoxMaterials_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (programIsChangingStuff)
                return;

            programIsChangingStuff = true;

            if (currentMaterials.Length > 0)
                panelColor.BackColor = Color.FromArgb(
                    currentMaterials[0].materialStruct.color.A,
                    currentMaterials[0].materialStruct.color.R,
                    currentMaterials[0].materialStruct.color.G,
                    currentMaterials[0].materialStruct.color.B);

            propertyGridTextureInfo.SelectedObjects = [.. currentMaterials.Where(m => m.texture != null).Select(m => m.texture)];

            // Using PropertyGrid with multiple objects is broken with "Categorized" sorting after switching to .NET 10
            propertyGridTextureInfo.PropertySort = PropertySort.CategorizedAlphabetical;

            if (currentMaterials.Count() == 1)
            {
                comboBoxMatEffects.Enabled = true;
                propertyGridMatEffects.Enabled = true;

                bool foundMatEffect = false;
                foreach (var rws in currentMaterials[0].materialExtension.extensionSectionList)
                    if (rws is MaterialEffectsPLG_0120 matEffects)
                    {
                        comboBoxMatEffects.SelectedItem = matEffects.MaterialEffectType;
                        propertyGridMatEffects.SelectedObject = matEffects.materialEffect1;
                        foundMatEffect = true;
                        break;
                    }
                if (!foundMatEffect)
                {
                    comboBoxMatEffects.SelectedItem = null;
                    propertyGridMatEffects.SelectedObject = null;
                }
            }
            else
            {
                comboBoxMatEffects.Enabled = false;
                propertyGridMatEffects.Enabled = false;
                comboBoxMatEffects.SelectedItem = null;
                propertyGridMatEffects.SelectedObject = null;
            }

            programIsChangingStuff = false;
        }

        private void panelColor_Click(object sender, EventArgs e)
        {
            if (currentMaterials.Count() == 0)
                return;

            ColorDialog dialog = new ColorDialog()
            {
                Color = Color.FromArgb(
                currentMaterials[0].materialStruct.color.A,
                currentMaterials[0].materialStruct.color.R,
                currentMaterials[0].materialStruct.color.G,
                currentMaterials[0].materialStruct.color.B)
            };

            if (dialog.ShowDialog() == DialogResult.OK)
                foreach (var currentMaterial in currentMaterials)
                {
                    currentMaterial.materialStruct.color = new RenderWareFile.Color(
                        dialog.Color.R,
                        dialog.Color.G,
                        dialog.Color.B,
                        dialog.Color.A);
                    panelColor.BackColor = dialog.Color;
                }
        }

        private void comboBoxMatEffects_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (programIsChangingStuff || currentMaterials.Count() != 1)
                return;

            bool foundMatEffect = false;

            for (int j = 0; j < currentMaterials[0].materialExtension.extensionSectionList.Length; j++)
                if (currentMaterials[0].materialExtension.extensionSectionList[j] is MaterialEffectsPLG_0120 matEffects)
                {
                    matEffects.MaterialEffectType = (MaterialEffectType)comboBoxMatEffects.SelectedItem;
                    propertyGridMatEffects.SelectedObject = matEffects.materialEffect1;
                    foundMatEffect = true;
                    break;
                }

            if (!foundMatEffect)
            {
                var matEffects = new MaterialEffectsPLG_0120()
                {
                    MaterialEffectType = (MaterialEffectType)comboBoxMatEffects.SelectedItem
                };
                propertyGridMatEffects.SelectedObject = matEffects.materialEffect1;
                currentMaterials[0].materialExtension.Add(matEffects);
            }
        }

        private void propertyGridTextureInfo_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            programIsChangingStuff = true;

            var selectedIndices = listBoxMaterials.SelectedIndices.Cast<int>().ToList();
            listBoxMaterials.SelectedIndices.Clear();
            foreach (int i in selectedIndices)
            {
                listBoxMaterials.Items[i] = listBoxMaterials.Items[i];
                listBoxMaterials.SetSelected(i, true);
            }

            programIsChangingStuff = false;
        }

        private void buttonAddTexture_Click(object sender, EventArgs e)
        {
            foreach (var m in currentMaterials)
                if (!m.IsTextured)
                    m.IsTextured = true;
        }

        private void buttonSelectSimilar_Click(object sender, EventArgs e)
        {
            var names = new HashSet<string>();
            foreach (var m in currentMaterials)
                names.Add(m.ToString());
            for (int i = 0; i < listBoxMaterials.Items.Count; i++)
                if (names.Contains(listBoxMaterials.Items[i].ToString()))
                    listBoxMaterials.SetSelected(i, true);
        }

        ToolTip toolTip;

        private void buttonSelectSimilar_MouseHover(object sender, EventArgs e)
        {
            toolTip.Show("Select all materials which have the same name as the currently selected materials", buttonSelectSimilar);
        }
    }
}