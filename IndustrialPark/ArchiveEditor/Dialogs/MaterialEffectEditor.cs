using Newtonsoft.Json;
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
            TopMost = true;

            foreach (var m in materials)
                listBoxMaterials.Items.Add(m);

            comboBoxMatEffects.Items.Add(MaterialEffectType.NoEffect);
            comboBoxMatEffects.Items.Add(MaterialEffectType.BumpMap);
            comboBoxMatEffects.Items.Add(MaterialEffectType.EnvironmentMap);
            comboBoxMatEffects.Items.Add(MaterialEffectType.DualTextures);
        }

        private bool OK = false;
        
        public static Material_0007[] GetMaterials(Material_0007[] materials, out bool success)
        {
            MaterialEffectEditor eventEditor = new MaterialEffectEditor(materials);
            eventEditor.ShowDialog();

            success = eventEditor.OK;

            List<Material_0007> outMaterials = new List<Material_0007>();
            foreach (Material_0007 material in eventEditor.listBoxMaterials.Items)
                outMaterials.Add(material);

            return outMaterials.ToArray();
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

        private Material_0007 currentMaterial => (Material_0007)listBoxMaterials.SelectedItem;

        private bool programIsChangingStuff = false;

        private void listBoxMaterials_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (currentMaterial == null)
                return;

            programIsChangingStuff = true;

            panelColor.BackColor = Color.FromArgb(
                currentMaterial.materialStruct.color.A,
                currentMaterial.materialStruct.color.R,
                currentMaterial.materialStruct.color.G,
                currentMaterial.materialStruct.color.B);

            propertyGridTextureInfo.SelectedObject = currentMaterial.texture;

            bool foundMatEffect = false;
            foreach (var rws in currentMaterial.materialExtension.extensionSectionList)
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

            programIsChangingStuff = false;
        }

        private void panelColor_Click(object sender, EventArgs e)
        {
            if (currentMaterial == null)
                return;

            ColorDialog dialog = new ColorDialog()
            {
                Color = Color.FromArgb(
                currentMaterial.materialStruct.color.A,
                currentMaterial.materialStruct.color.R,
                currentMaterial.materialStruct.color.G,
                currentMaterial.materialStruct.color.B)
            };

            if (dialog.ShowDialog() == DialogResult.OK) 
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
            if (programIsChangingStuff || currentMaterial == null)
                return;

            bool foundMatEffect = false;

            for (int i = 0; i < currentMaterial.materialExtension.extensionSectionList.Count; i++)
                if (currentMaterial.materialExtension.extensionSectionList[i] is MaterialEffectsPLG_0120 matEffects)
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
                currentMaterial.materialExtension.extensionSectionList.Add(matEffects);
            }
        }
    }
}