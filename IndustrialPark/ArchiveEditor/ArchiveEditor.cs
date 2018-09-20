using HipHopFile;
using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class ArchiveEditor : Form
    {
        public ArchiveEditorFunctions archive;
        public bool UnsavedChanges { get; private set; } = false;

        public ArchiveEditor()
        {
            InitializeComponent();
            TopMost = true;
            archive = new ArchiveEditorFunctions();
            defaultColor = textBoxFindAsset.BackColor;

            programIsChangingStuff = true;

            foreach (LayerType o in Enum.GetValues(typeof(LayerType)))
            {
                comboBoxLayerTypes.Items.Add(o.ToString());
            }

            programIsChangingStuff = false;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown) return;
            if (e.CloseReason == CloseReason.FormOwnerClosing) return;

            e.Cancel = true;
            Hide();
        }
        
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog()
            {
                Filter = "HIP/HOP Files|*.hip;*.hop",
                Title = "Please choose a HIP or HOP file"
            };
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                Thread t = new Thread(() => OpenFile(openFile.FileName));
                t.Start();
            }
        }

        private void OpenFile(string fileName)
        {
            archive.OpenFile(fileName);

            if (!InvokeRequired)
                FinishedOpening(fileName);
            else
                Invoke(new Action<string>(FinishedOpening), fileName);
        }

        private void FinishedOpening(string fileName)
        {
            toolStripStatusLabel1.Text = "File: " + fileName;
            Text = Path.GetFileName(fileName);
            Program.MainForm.SetToolStripItemName(this, Text);
            saveToolStripMenuItem.Enabled = true;
            saveAsToolStripMenuItem.Enabled = true;
            UnsavedChanges = false;
            PopulateLayerComboBox();
            PopulateAssetList();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (UnsavedChanges)
            {
                DialogResult result = MessageBox.Show("You have unsaved changes. Do you wish to save them before closing?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                if (result == DialogResult.Cancel) return;
                if (result == DialogResult.Yes) archive.Save();
            }

            archive.Dispose();
            Program.MainForm.CloseAssetEditor(this);
            Close();
        }

        private void exportTexturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ArchiveEditor editor in Program.MainForm.archiveEditors)
                foreach (Asset asset in editor.archive.GetAllAssets())
                {
                    if (asset is AssetRWTX texture)
                        ArchiveEditorFunctions.ExportTextureAsset(texture, editor.archive.fileNamePrefix);
                }
        }

        private bool programIsChangingStuff = false;

        private string LayerToString(int index)
        {
            return "Layer " + index.ToString() + ": "
                + archive.DICT.LTOC.LHDRList[index].layerType.ToString()
                + " [" + archive.DICT.LTOC.LHDRList[index].assetIDlist.Count() + "]";
        }

        private void PopulateLayerComboBox()
        {
            programIsChangingStuff = true;

            comboBoxLayers.Items.Clear();
            for (int i = 0; i < archive.DICT.LTOC.LHDRList.Count; i++)
            {
                comboBoxLayers.Items.Add(LayerToString(i));
            }

            programIsChangingStuff = false;
        }

        private void comboBoxLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            programIsChangingStuff = true;

            comboBoxLayerTypes.SelectedItem = archive.DICT.LTOC.LHDRList[comboBoxLayers.SelectedIndex].layerType.ToString();

            PopulateAssetListAndComboBox();

            programIsChangingStuff = false;
        }
        
        private void comboBoxLayerTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!programIsChangingStuff)
            {
                foreach (LayerType o in Enum.GetValues(typeof(LayerType)))
                {
                    if(o.ToString() == (string)comboBoxLayerTypes.SelectedItem)
                    {
                        archive.DICT.LTOC.LHDRList[comboBoxLayers.SelectedIndex].layerType = o;
                        comboBoxLayers.Items[comboBoxLayers.SelectedIndex] = LayerToString(comboBoxLayers.SelectedIndex);
                        return;
                    }
                }
                throw new Exception("Invalid layer type");
            }
        }

        private void buttonAddLayer_Click(object sender, EventArgs e)
        {
            programIsChangingStuff = true;
            try
            { 
                archive.DICT.LTOC.LHDRList.Add(new Section_LHDR()
                {
                    assetIDlist = new List<uint>(),
                    LDBG = new Section_LDBG(-1)
                });
                comboBoxLayers.Items.Add(LayerToString(archive.DICT.LTOC.LHDRList.Count - 1));
                comboBoxLayers.SelectedIndex = comboBoxLayers.Items.Count - 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to add layer: " + ex.Message);
            }

            programIsChangingStuff = false;
        }

        private void buttonRemoveLayer_Click(object sender, EventArgs e)
        {
            programIsChangingStuff = true;

            try
            {
                archive.RemoveLayer(comboBoxLayers.SelectedIndex);

                comboBoxLayers.Items.RemoveAt(comboBoxLayers.SelectedIndex);

                for (int i = 0; i < archive.DICT.LTOC.LHDRList.Count; i++)
                {
                    comboBoxLayers.Items[i] = LayerToString(i);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to remove layer: " + ex.Message);
            }
            programIsChangingStuff = false;
        }
        
        private void PopulateAssetListAndComboBox()
        {
            PopulateAssetList();

            programIsChangingStuff = true;

            List<uint> assetIDs = archive.DICT.LTOC.LHDRList[comboBoxLayers.SelectedIndex].assetIDlist;
            List <AssetType> assetTypeList = new List<AssetType>();
            for (int i = 0; i < assetIDs.Count(); i++)
            {
                if (!assetTypeList.Contains(archive.GetFromAssetID(assetIDs[i]).AHDR.assetType))
                    assetTypeList.Add(archive.GetFromAssetID(assetIDs[i]).AHDR.assetType);
            }
            assetTypeList.Sort();

            comboBoxAssetTypes.Items.Clear();
            comboBoxAssetTypes.Items.Add("All");
            comboBoxAssetTypes.Items.AddRange(assetTypeList.ToArray().Cast<object>().ToArray());

            comboBoxAssetTypes.SelectedIndex = 0;

            programIsChangingStuff = false;
        }

        private void PopulateAssetList(AssetType type = AssetType.Null)
        {
            listBoxAssets.Items.Clear();
            if (comboBoxLayers.SelectedItem == null) return;

            List<uint> assetIDs = archive.DICT.LTOC.LHDRList[comboBoxLayers.SelectedIndex].assetIDlist;
            List<string> assetList = new List<string>();
            for (int i = 0; i < assetIDs.Count(); i++)
            {
                if (type == AssetType.Null)
                    assetList.Add(archive.GetFromAssetID(assetIDs[i]).ToString());
                else
                {
                    if (archive.GetFromAssetID(assetIDs[i]).AHDR.assetType == type)
                        assetList.Add(archive.GetFromAssetID(assetIDs[i]).ToString());
                }
            }

            assetList.Sort();
            listBoxAssets.Items.AddRange(assetList.ToArray());
        }

        private void comboBoxAssetTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!programIsChangingStuff)
            {
                if (comboBoxAssetTypes.SelectedIndex == 0)
                    PopulateAssetList();
                else
                    PopulateAssetList((AssetType)comboBoxAssetTypes.SelectedItem);
            }
        }

        private void buttonAddAsset_Click(object sender, EventArgs e)
        {
            Section_AHDR AHDR = AddAssetDialog.GetAsset(new AddAssetDialog(), out bool success);

            if (success)
            {
                try
                {
                    while (archive.ContainsAsset(AHDR.assetID))
                    {
                        MessageBox.Show($"Archive already contains asset id [{AHDR.assetID.ToString("X8")}]. Will change it to [{(AHDR.assetID + 1).ToString("X8")}].");
                        AHDR.assetID++;
                    }
                    UnsavedChanges = true;
                    archive.AddAsset(comboBoxLayers.SelectedIndex, AHDR);
                    PopulateAssetListAndComboBox();
                    SetSelectedIndex(AHDR.assetID);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to add asset: " + ex.Message);
                }
            }
        }

        private int numCopies = 0;

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            if (listBoxAssets.SelectedIndex < 0) return;

            UnsavedChanges = true;

            Section_AHDR AHDR = archive.GetFromAssetID(CurrentlySelectedAssetID()).AHDR;

            string newAssetName;

            if (AHDR.ADBG.assetName.Contains("_COPY"))            
                newAssetName = AHDR.ADBG.assetName.Substring(0, AHDR.ADBG.assetName.LastIndexOf("_COPY")) + "_COPY" + numCopies.ToString();            
            else
                newAssetName = AHDR.ADBG.assetName + "_COPY" + numCopies.ToString();

            numCopies++;

            uint newAssetId = Functions.BKDRHash(newAssetName);
            while (archive.ContainsAsset(newAssetId))
                newAssetId++;

            Section_AHDR newAHDR = new Section_AHDR(newAssetId, AHDR.assetType, AHDR.flags, new Section_ADBG(AHDR.ADBG.alignment, newAssetName, AHDR.ADBG.assetFileName, AHDR.ADBG.checksum))
            {
                fileOffset = AHDR.fileOffset,
                fileSize = AHDR.fileSize,
                plusValue = AHDR.plusValue
            };

            newAHDR.data = new byte[AHDR.data.Length];
            for (int i = 0; i < newAHDR.data.Length; i++)
                newAHDR.data[i] = AHDR.data[i];

            archive.AddAsset(comboBoxLayers.SelectedIndex, newAHDR);
            PopulateAssetListAndComboBox();

            SetSelectedIndex(newAssetId);
        }

        private void buttonRemoveAsset_Click(object sender, EventArgs e)
        {
            if (listBoxAssets.SelectedIndex < 0) return;

            RemoveInternalEditor(CurrentlySelectedAssetID());
            archive.RemoveAsset(CurrentlySelectedAssetID());

            UnsavedChanges = true;

            listBoxAssets.Items.Remove(listBoxAssets.SelectedItem);
        }

        private void buttonView_Click(object sender, EventArgs e)
        {
            if (listBoxAssets.SelectedIndex < 0) return;

            if (archive.GetFromAssetID(CurrentlySelectedAssetID()) is AssetCAM cam)
                Program.MainForm.renderer.Camera.SetPositionCamera(cam);
            else if (archive.GetFromAssetID(CurrentlySelectedAssetID()) is IClickableAsset a)
                Program.MainForm.renderer.Camera.SetPosition(new Vector3(a.PositionX, a.PositionY, a.PositionZ) - 8 * Program.MainForm.renderer.Camera.GetForward());
        }

        private void buttonEditAsset_Click(object sender, EventArgs e)
        {
            try
            {
                uint aid = CurrentlySelectedAssetID();
                Section_AHDR AHDR = AddAssetDialog.GetAsset(archive.GetFromAssetID(aid).AHDR, out bool success);

                if (success)
                {
                    UnsavedChanges = true;
                    RemoveInternalEditor(aid);
                    archive.RemoveAsset(aid);

                    while (archive.ContainsAsset(AHDR.assetID))
                    {
                        MessageBox.Show($"Archive already contains asset id [{AHDR.assetID.ToString("X8")}]. Will change it to [{(AHDR.assetID + 1).ToString("X8")}].");
                        AHDR.assetID++;
                    }

                    archive.AddAsset(comboBoxLayers.SelectedIndex, AHDR);
                    PopulateAssetListAndComboBox();
                    SetSelectedIndex(AHDR.assetID);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to edit asset: " + ex.Message);
            }
        }

        private List<IInternalEditor> internalEditors = new List<IInternalEditor>();

        public void RemoveInternalEditor(IInternalEditor i)
        {
            internalEditors.Remove(i);
        }

        public void RemoveInternalEditor(uint assetID)
        {
            for (int i = 0; i < internalEditors.Count; i++)
                if (internalEditors[i].GetAssetID() == assetID)
                    internalEditors[i].Close();
        }

        private void buttonInternalEdit_Click(object sender, EventArgs e)
        {
            if (listBoxAssets.SelectedItem != null)
            {
                Asset asset = archive.GetFromAssetID(CurrentlySelectedAssetID());

                for (int i = 0; i < internalEditors.Count; i++)
                    if (internalEditors[i].GetAssetID() == asset.AHDR.assetID)
                        internalEditors[i].Close();

                if (asset is AssetCAM CAM)
                    internalEditors.Add(new InternalCamEditor(CAM, this));
                else if (asset is AssetDYNA DYNA)
                    internalEditors.Add(new InternalDynaEditor(DYNA, this));
                else if (asset is AssetTEXT TEXT)
                    internalEditors.Add(new InternalTextEditor(TEXT, this));
                else if (asset.AHDR.assetType == AssetType.SND | asset.AHDR.assetType == AssetType.SNDS)
                    internalEditors.Add(new InternalSoundEditor(asset, this));
                else
                    internalEditors.Add(new InternalAssetEditor(asset, this));

                internalEditors.Last().Show();

                UnsavedChanges = true;
            }
        }

        private void buttonExportRaw_Click(object sender, EventArgs e)
        {
            if (listBoxAssets.SelectedItem == null)
                return;

            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                FileName = archive.GetFromAssetID(CurrentlySelectedAssetID()).AHDR.ADBG.assetName
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.WriteAllBytes(saveFileDialog.FileName, archive.GetFromAssetID(CurrentlySelectedAssetID()).AHDR.data);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to export asset raw data: " + ex.Message);
                }
            }
        }

        public void AddSoundToSNDI(byte[] headerData, uint assetID)
        {
            archive.AddSoundToSNDI(headerData, assetID);
        }

        public byte[] GetHeaderFromSNDI(uint assetID)
        {
            return archive.GetHeaderFromSNDI(assetID);
        }

        private uint CurrentlySelectedAssetID()
        {
            if (listBoxAssets.SelectedItem != null)
                return GetAssetIDFromName(listBoxAssets.SelectedItem as string);
            else return 0;
        }

        private uint GetAssetIDFromName(string name)
        {
            return Convert.ToUInt32(name.Substring(name.IndexOf('[') + 1, 8), 16);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(archive.Save);
            t.Start();
            UnsavedChanges = false;
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                archive.currentlyOpenFilePath = saveFileDialog.FileName;
                archive.Save();
                toolStripStatusLabel1.Text = "File: " + saveFileDialog.FileName;
                UnsavedChanges = false;
            }
        }
        
        private void listBoxAssets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxAssets.SelectedItem != null)
            {
                archive.SelectAsset(CurrentlySelectedAssetID());

                if (archive.GetFromAssetID(CurrentlySelectedAssetID()) is IClickableAsset a)
                    buttonView.Visible = true;
                else
                    buttonView.Visible = false;
            }
        }

        public void ScreenClicked(Ray ray, bool isMouseDown)
        {
            if (isMouseDown)
                archive.GizmoSelect(ray);
            else if (archive.FinishedMovingGizmo)
                archive.FinishedMovingGizmo = false;
            else
            {
                uint assetID = archive.GetClickedAssetID(ray);
                if (assetID != 0)
                    SetSelectedIndex(assetID);
            }
        }

        public void ScreenUnclicked()
        {
            archive.ScreenUnclicked();
        }

        public void MouseMoveX(SharpCamera camera, int deltaX)
        {
            archive.MouseMoveX(camera, deltaX);
        }

        public void MouseMoveY(SharpCamera camera, int deltaY)
        {
            archive.MouseMoveY(camera, deltaY);
        }

        private void SetSelectedIndex(uint assetID)
        {
            if (!archive.ContainsAsset(assetID))
                return;

            try
            {
                comboBoxLayers.SelectedIndex = archive.GetLayerFromAssetID(assetID);
                comboBoxAssetTypes.SelectedItem = archive.GetFromAssetID(assetID).AHDR.assetType;
            }
            catch
            {
                PopulateAssetList();
            }

            for (int i = 0; i < listBoxAssets.Items.Count; i++)
            {
                if (GetAssetIDFromName(listBoxAssets.Items[i] as string) == assetID)
                {
                    listBoxAssets.SelectedIndex = i;
                    return;
                }
            }
        }

        System.Drawing.Color defaultColor;
        private void textBoxFindAsset_TextChanged(object sender, EventArgs e)
        {
            uint assetID = 0;
            try
            {
                textBoxFindAsset.BackColor = defaultColor;
                assetID = Convert.ToUInt32(textBoxFindAsset.Text, 16);
                SetSelectedIndex(assetID);
            }
            catch
            {
                textBoxFindAsset.BackColor = System.Drawing.Color.Red;
            }
        }
        
        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clickThisToolStripMenuItem.Visible = false;
            if (MainForm.alternateNamingMode)
            {
                assetIDAssetNameToolStripMenuItem.Checked = true;
                assetNameAssetIDToolStripMenuItem.Checked = false;
            }
            else
            {
                assetIDAssetNameToolStripMenuItem.Checked = false;
                assetNameAssetIDToolStripMenuItem.Checked = true;
            }
        }
        
        private void assetNameAssetIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            assetIDAssetNameToolStripMenuItem.Checked = false;
            assetNameAssetIDToolStripMenuItem.Checked = true;
            MainForm.alternateNamingMode = false;
        }

        private void assetIDAssetNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            assetIDAssetNameToolStripMenuItem.Checked = true;
            assetNameAssetIDToolStripMenuItem.Checked = false;
            MainForm.alternateNamingMode = true;
        }

        private void importTXDArchiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openTXD = new OpenFileDialog() { Filter = "TXD archives|*.txd" };
            if (openTXD.ShowDialog() == DialogResult.OK)
            {
                archive.AddTextureDictionary(openTXD.FileName);
            }
        }

        public bool HasAsset(uint assetID)
        {
            return archive.ContainsAsset(assetID);
        }

        public string GetAssetNameFromID(uint assetID)
        {
            return archive.GetFromAssetID(assetID).AHDR.ADBG.assetName;
        }
    }
}