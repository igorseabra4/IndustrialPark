using HipHopFile;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using static IndustrialPark.ArchiveEditor;

namespace IndustrialPark
{
    public partial class ApplyScale : Form
    {
        private static Vector3 SavedValue = new Vector3(1f, 1f, 1f);

        private ApplyScale(bool hasAssets)
        {
            InitializeComponent();
            TopMost = true;

            numericUpDownX.Minimum = decimal.MinValue;
            numericUpDownY.Minimum = decimal.MinValue;
            numericUpDownZ.Minimum = decimal.MinValue;
            numericUpDownX.Maximum = decimal.MaxValue;
            numericUpDownY.Maximum = decimal.MaxValue;
            numericUpDownZ.Maximum = decimal.MaxValue;

            System.Drawing.Point SubHeight(System.Drawing.Point prevLocation) =>
                new System.Drawing.Point(prevLocation.X, prevLocation.Y - DecreaseHeight);

            if (!hasAssets)
            {
                label2.Visible = false;
                checkedListBoxAssetTypes.Visible = false;
                checkBoxBakeNpcScales.Visible = false;
                checkBoxBakeScales.Visible = false;
                Height -= DecreaseHeight;
                button1.Location = SubHeight(button1.Location);
                button2.Location = SubHeight(button2.Location);
            }

            numericUpDownX.Value = (decimal)SavedValue.X;
            numericUpDownY.Value = (decimal)SavedValue.Y;
            numericUpDownZ.Value = (decimal)SavedValue.Z;

            SetInverseLabel();
        }

        private const int DecreaseHeight = 223;

        private ApplyScale(IEnumerable<AssetTypeContainer> assetTypes) : this(true)
        {
            foreach (var assetType in assetTypes)
                checkedListBoxAssetTypes.Items.Add(assetType, true);
        }

        public static Vector3? GetVector()
        {
            ApplyScale edit = new ApplyScale(false);
            edit.ShowDialog();

            if (edit.OK)
            {
                edit.SaveValues();
                return SavedValue;
            }
            return null;
        }

        private void SaveValues()
        {
            SavedValue = GetValue();
        }

        public static (Vector3, IEnumerable<AssetType>, bool, bool)? GetScaleWithAssets(IEnumerable<AssetTypeContainer> assetTypes)
        {
            ApplyScale edit = new ApplyScale(assetTypes);
            edit.ShowDialog();

            if (edit.OK)
            {
                edit.SaveValues();
                return (SavedValue, edit.GetAssetTypes(), edit.checkBoxBakeScales.Checked, edit.checkBoxBakeNpcScales.Checked);
            }
            return null;
        }

        private Vector3 GetValue() => new Vector3((float)numericUpDownX.Value, (float)numericUpDownY.Value, (float)numericUpDownZ.Value);
        
        private IEnumerable<AssetType> GetAssetTypes() => checkedListBoxAssetTypes.CheckedItems.Cast<AssetTypeContainer>().Select(a => a.assetType);
        
        private bool OK = false;

        private void button1_Click(object sender, EventArgs e)
        {
            OK = true;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            SetInverseLabel();
        }

        private void SetInverseLabel()
        {
            var val = GetValue();
            label1.Text = $"To inverse: [{1.0 / val.X}, {1.0 / val.Y}, {1.0 / val.Z}]";
        }
    }
}
