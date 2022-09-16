using RenderWareFile.Sections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            TopMost = true;

            foreach (var m in materials)
                listBoxMaterials.Items.Add(m);

            comboBoxMatEffects.Items.Add(MaterialEffectType.NoEffect);
            comboBoxMatEffects.Items.Add(MaterialEffectType.BumpMap);
            comboBoxMatEffects.Items.Add(MaterialEffectType.EnvironmentMap);
            comboBoxMatEffects.Items.Add(MaterialEffectType.DualTextures);
        }

        private bool OK = false;

        public static Material_0007[] GetMaterials(Material_0007[] materials)
        {
            MaterialEffectEditor materialEffectEditor = new MaterialEffectEditor(materials);
            materialEffectEditor.ShowDialog();

            if (materialEffectEditor.OK)
            {
                List<Material_0007> outMaterials = new List<Material_0007>();
                foreach (Material_0007 material in materialEffectEditor.listBoxMaterials.Items)
                    outMaterials.Add(material);

                return outMaterials.ToArray();
            }

            return null;
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

        private Material_0007[] currentMaterials => listBoxMaterials.SelectedItems.Cast<Material_0007>().ToArray();

        private bool programIsChangingStuff = false;

        private void listBoxMaterials_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (currentMaterials.Count() == 0)
                return;

            programIsChangingStuff = true;

            panelColor.BackColor = Color.FromArgb(
                currentMaterials[0].materialStruct.color.A,
                currentMaterials[0].materialStruct.color.R,
                currentMaterials[0].materialStruct.color.G,
                currentMaterials[0].materialStruct.color.B);

            propertyGridTextureInfo.SelectedObjects = (from Material_0007 mat in currentMaterials select mat.texture).ToArray();
            
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

            for (int j = 0; j < currentMaterials[0].materialExtension.extensionSectionList.Count; j++)
                if (currentMaterials[0].materialExtension.extensionSectionList[j] is MaterialEffectsPLG_0120 matEffects)
                {
                    matEffects.MaterialEffectType = (MaterialEffectType)comboBoxMatEffects.SelectedItem;
                    propertyGridMatEffects.SelectedObject = matEffects.materialEffect1;
                    foundMatEffect = true;
                    break;
                }

            if (!foundMatEffect)
            {
                var matEffects = new MaterialEffectsPLG_0120
                {
                    MaterialEffectType = (MaterialEffectType)comboBoxMatEffects.SelectedItem
                };
                propertyGridMatEffects.SelectedObject = matEffects.materialEffect1;
                currentMaterials[0].materialExtension.extensionSectionList.Add(matEffects);
            }
        }

        private void propertyGridTextureInfo_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            var selectedIndices = (from int i in listBoxMaterials.SelectedIndices select i).ToHashSet();
            for (int i = 0; i < listBoxMaterials.Items.Count; i++)
            {
                listBoxMaterials.Items[i] = listBoxMaterials.Items[i];
                listBoxMaterials.SetSelected(i, selectedIndices.Contains(i));
            }
        }
    }
}