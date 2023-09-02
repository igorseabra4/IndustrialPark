using Assimp;
using HipHopFile;
using IndustrialPark.Models;
using RenderWareFile;
using RenderWareFile.Sections;
using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static IndustrialPark.Models.Assimp_IO;
using static IndustrialPark.Models.BSP_IO;
using static IndustrialPark.Models.BSP_IO_Shared;

namespace IndustrialPark
{
    public partial class InternalModelEditor : Form, IInternalEditor
    {
        public InternalModelEditor(AssetRenderWareModel asset, ArchiveEditorFunctions archive, Action<Asset> updateListView)
        {
            InitializeComponent();
            TopMost = true;

            this.asset = asset;
            this.archive = archive;
            this.updateListView = updateListView;

            Text = $"[{asset.assetType}] {asset}";

            SetupTexturesBox();
            if (asset.assetType == AssetType.Model)
            {
                SetupPiptBox(0);
                SetupLodtBox();
                SetupCollBox();
                SetupShdwBox();
            }
            else
            {
                groupBoxPipeInfo.Enabled = false;
                groupBoxLevelOfDetail.Enabled = false;
                groupBoxCollisionModel.Enabled = false;
                groupBoxShadow.Enabled = false;
                checkBoxUseTemplates.Enabled = false;
            }
        }

        public void RefreshPropertyGrid()
        {
            propertyGridCollision.Refresh();
            propertyGridPipeInfo.Refresh();
            propertyGridShadow.Refresh();
            propertyGridLevelOfDetail.Refresh();
        }

        private void InternalAssetEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            archive.CloseInternalEditor(this);
        }

        private readonly AssetRenderWareModel asset;
        private readonly ArchiveEditorFunctions archive;
        private readonly Action<Asset> updateListView;

        public uint GetAssetID()
        {
            return asset.assetID;
        }

        private void SetupTexturesBox()
        {
            flowLayoutPanelTextures.Controls.Clear();
            foreach (var s in asset.Textures.OrderBy(s => s))
                flowLayoutPanelTextures.Controls.Add(new Label() { Text = $"- {s}", Height = 16, AutoSize = true });
        }

        private void buttonEditAtomics_Click(object sender, EventArgs e)
        {
            buttonEditAtomics.Visible = false;
            SetupAtomicsBox();
        }

        private void SetupAtomicsBox()
        {
            tableLayoutPanelAtomics.Controls.Clear();
            var atomics = asset.AtomicFlags;
            tableLayoutPanelAtomics.RowCount = atomics.Length;
            for (int i = 0; i < atomics.Length; i++)
            {
                tableLayoutPanelAtomics.Controls.Add(new Label()
                {
                    Text = i.ToString(),
                    AutoSize = true,
                    Dock = DockStyle.Fill,
                    TextAlign = System.Drawing.ContentAlignment.MiddleLeft
                }, 0, i);
                tableLayoutPanelAtomics.Controls.Add(CreateComboBox(atomics[i], i), 1, i);
            }
        }

        private ComboBox CreateComboBox(AtomicFlags a, int index)
        {
            var cb = new ComboBox()
            {
                Width = 134
            };
            foreach (var af in Enum.GetValues(typeof(AtomicFlags)))
                cb.Items.Add(af);
            cb.SelectedItem = a;
            cb.SelectedIndexChanged += (object sender, EventArgs e) =>
            {
                var flags = asset.AtomicFlags;
                flags[index] = (AtomicFlags)cb.SelectedItem;
                asset.AtomicFlags = flags;
            };
            return cb;
        }

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            ArchiveEditorFunctions.OpenWikiPage(asset);
        }

        private void buttonFindCallers_Click(object sender, EventArgs e)
        {
            Program.MainForm.FindWhoTargets(GetAssetID());
        }

        private void buttonApplyVertexColors_Click(object sender, EventArgs e)
        {
            var (color, operation) = ApplyVertexColors.GetColor();
            if (color.HasValue)
            {
                var performOperation = new Func<Vector4, Vector4>((Vector4 oldColor) => new Vector4(
                    operation.PerformAndClamp(oldColor.X, color.Value.X),
                    operation.PerformAndClamp(oldColor.Y, color.Value.Y),
                    operation.PerformAndClamp(oldColor.Z, color.Value.Z),
                    operation.PerformAndClamp(oldColor.W, color.Value.W)));

                asset.ApplyVertexColors(performOperation);
                archive.UnsavedChanges = true;
                updateListView(asset);
            }
        }

        private void buttonApplyRotation_Click(object sender, EventArgs e)
        {
            var factor = ApplyScale.GetVector(true);
            if (factor.HasValue)
            {
                asset.ApplyRotation(MathUtil.DegreesToRadians(factor.Value.X), MathUtil.DegreesToRadians(factor.Value.Y), MathUtil.DegreesToRadians(factor.Value.Z));
                archive.UnsavedChanges = true;
                updateListView(asset);
            }
        }

        private void buttonApplyScale_Click(object sender, EventArgs e)
        {
            var factor = ApplyScale.GetVector(false);
            if (factor.HasValue)
            {
                asset.ApplyScale(factor.Value);
                archive.UnsavedChanges = true;
                updateListView(asset);
            }
        }

        private void buttonImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog()
            {
                Filter = GetImportFilter(), // "All supported types|*.dae;*.obj;*.bsp|DAE Files|*.dae|OBJ Files|*.obj|BSP Files|*.bsp|All files|*.*",
            };

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                if (asset.assetType == AssetType.Model)
                    asset.Data = Path.GetExtension(openFile.FileName).ToLower().Equals(".dff") ?
                    File.ReadAllBytes(openFile.FileName) :
                    ReadFileMethods.ExportRenderWareFile(CreateDFFFromAssimp(openFile.FileName, checkBoxFilpUvs.Checked, checkBoxIgnoreMeshColors.Checked), modelRenderWareVersion(asset.game));
                else if (asset.assetType == AssetType.BSP)
                    asset.Data = Path.GetExtension(openFile.FileName).ToLower().Equals(".bsp") ?
                    File.ReadAllBytes(openFile.FileName) :
                    ReadFileMethods.ExportRenderWareFile(CreateBSPFromAssimp(openFile.FileName, checkBoxFilpUvs.Checked, checkBoxIgnoreMeshColors.Checked), modelRenderWareVersion(asset.game));

                asset.Setup(Program.MainForm.renderer);
                archive.UnsavedChanges = true;
                updateListView(asset);
                SetupTexturesBox();
            }
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            (bool ok, ExportFormatDescription format, string textureExtension) = ChooseTarget.GetTarget();

            if (ok)
            {
                SaveFileDialog a = new SaveFileDialog()
                {
                    Filter = format == null ? "RenderWare BSP/DFF|*.bsp;*.dff" : format.Description + "|*." + format.FileExtension,
                };
                if (a.ShowDialog() == DialogResult.OK)
                {
                    if (format == null)
                        File.WriteAllBytes(a.FileName, asset.Data);
                    else if (format.FileExtension.ToLower().Equals("obj") && asset.assetType == AssetType.BSP)
                        ConvertBSPtoOBJ(a.FileName, ReadFileMethods.ReadRenderWareFile(asset.Data), true);
                    else
                        ExportAssimp(Path.ChangeExtension(a.FileName, format.FileExtension), ReadFileMethods.ReadRenderWareFile(asset.Data), true, format, textureExtension, Matrix.Identity);

                    if (checkBoxExportTextures.Checked)
                    {
                        string folderName = Path.GetDirectoryName(a.FileName);
                        var bitmaps = archive.GetTexturesAsBitmaps(asset.Textures);
                        ReadFileMethods.treatStuffAsByteArray = false;
                        foreach (string textureName in bitmaps.Keys)
                            bitmaps[textureName].Save(folderName + "/" + textureName + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    }
                }
            }
        }

        private void buttonMaterialEditor_Click(object sender, EventArgs e)
        {
            var materials = MaterialEffectEditor.GetMaterials(asset.Materials);
            if (materials != null)
            {
                asset.Materials = materials;
                archive.UnsavedChanges = true;
                updateListView(asset);
                SetupTexturesBox();
            }
        }


        private List<PipeInfo> pipeInfos = null;
        private int currentPipeInfo = -1;

        private void GetPipeInfos()
        {
            var pipt = archive.GetPIPT(false);
            pipeInfos = new List<PipeInfo>();
            if (pipt != null)
                foreach (var pipeInfo in pipt.Entries)
                    if (pipeInfo.Model == GetAssetID())
                        pipeInfos.Add(pipeInfo);
        }

        private void SetupPiptBox(int index)
        {
            GetPipeInfos();
            if (pipeInfos.Count > 0 && index > -1 && index < pipeInfos.Count)
            {
                propertyGridPipeInfo.Enabled = true;
                propertyGridPipeInfo.SelectedObject = new PipeInfoWrapper(pipeInfos[index]);
                currentPipeInfo = index;
                labelPipeInfos.Text = $"{index + 1}/{pipeInfos.Count} entries";
            }
            else
            {
                currentPipeInfo = -1;
                labelPipeInfos.Text = $"0/0 entries";
                propertyGridPipeInfo.SelectedObject = null;
                propertyGridPipeInfo.Enabled = false;
            }
            SetupArrowVisibility();
        }

        private void buttonCreatePipeInfo_Click(object sender, EventArgs e)
        {
            var pipt = archive.GetPIPT(true);
            var entry = new PipeInfo()
            {
                Model = GetAssetID()
            };
            entry.SetGame(asset.game);
            pipt.AddEntry(entry);
            SetupPiptBox(pipeInfos.Count);
        }

        private void buttonDeletePipeInfo_Click(object sender, EventArgs e)
        {
            if (pipeInfos.Count > 0 && currentPipeInfo > -1 && currentPipeInfo < pipeInfos.Count)
            {
                pipeInfos.RemoveAt(currentPipeInfo);
                var pipt = archive.GetPIPT();
                pipt.RemoveEntry(GetAssetID());
                pipt.AddEntries(pipeInfos);
                SetupPiptBox(currentPipeInfo < 1 ? 0 : currentPipeInfo - 1);
            }
        }

        private void SetupArrowVisibility()
        {
            buttonArrowUp.Enabled = currentPipeInfo != 0;
            buttonArrowDown.Enabled = currentPipeInfo < pipeInfos.Count - 1;
            buttonDeletePipeInfo.Enabled = pipeInfos.Any();
        }

        private void buttonArrowUp_Click(object sender, EventArgs e)
        {
            SetupPiptBox(currentPipeInfo - 1);
        }

        private void buttonArrowDown_Click(object sender, EventArgs e)
        {
            SetupPiptBox(currentPipeInfo + 1);
        }

        private void propertyGridPipeInfo_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            var pipt = archive.GetPIPT();
            pipt.RemoveEntry(GetAssetID());
            pipt.AddEntries(pipeInfos);
        }

        private void SetupLodtBox()
        {
            var lodt = archive.GetLODT();
            if (lodt != null)
            {
                foreach (var entry in lodt.Entries)
                    if (entry.BaseModel == GetAssetID())
                    {
                        buttonCreateLevelOfDetail.Text = "Remove";
                        propertyGridLevelOfDetail.Enabled = true;
                        propertyGridLevelOfDetail.SelectedObject = new LodtWrapper(entry);
                        return;
                    }
            }
            propertyGridLevelOfDetail.SelectedObject = null;
            propertyGridLevelOfDetail.Enabled = false;
        }

        private void buttonCreateLevelOfDetail_Click(object sender, EventArgs e)
        {
            var lodt = archive.GetLODT(true);
            if (propertyGridLevelOfDetail.SelectedObject == null)
            {
                var entry = new EntryLODT()
                {
                    BaseModel = GetAssetID()
                };
                entry.SetGame(asset.game);
                lodt.AddEntry(entry);
                SetupLodtBox();
            }
            else
            {
                lodt.RemoveEntry(GetAssetID());
                buttonCreateLevelOfDetail.Text = "Create";
                propertyGridLevelOfDetail.SelectedObject = null;
                propertyGridLevelOfDetail.Enabled = false;
            }
        }

        private void propertyGridLevelOfDetail_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (propertyGridLevelOfDetail.SelectedObject == null)
                return;
            var lodt = archive.GetLODT();
            lodt.AddEntry(((LodtWrapper)propertyGridLevelOfDetail.SelectedObject).Entry);
        }

        private void SetupCollBox()
        {
            var coll = archive.GetCOLL();
            if (coll != null)
            {
                foreach (var entry in coll.Entries)
                    if (entry.Model == GetAssetID())
                    {
                        buttonCreateCollision.Text = "Remove";
                        propertyGridCollision.Enabled = true;
                        propertyGridCollision.SelectedObject = new CollWrapper(entry);
                        return;
                    }
            }
            propertyGridCollision.SelectedObject = null;
            propertyGridCollision.Enabled = false;
        }

        private void buttonCreateCollision_Click(object sender, EventArgs e)
        {
            var coll = archive.GetCOLL(true);
            if (propertyGridCollision.SelectedObject == null)
            {
                var entry = new EntryCOLL()
                {
                    Model = GetAssetID()
                };
                entry.SetGame(asset.game);
                coll.AddEntry(entry);
                SetupCollBox();
            }
            else
            {
                coll.RemoveEntry(GetAssetID());
                buttonCreateCollision.Text = "Create";
                propertyGridCollision.SelectedObject = null;
                propertyGridCollision.Enabled = false;
            }
        }

        private void propertyGridCollision_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (propertyGridCollision.SelectedObject == null)
                return;
            var coll = archive.GetCOLL();
            coll.AddEntry(((CollWrapper)propertyGridCollision.SelectedObject).Entry);
        }

        private void SetupShdwBox()
        {
            var shdw = archive.GetSHDW();
            if (shdw != null)
            {
                foreach (var entry in shdw.Entries)
                    if (entry.Model == GetAssetID())
                    {
                        buttonCreateShadow.Text = "Remove";
                        propertyGridShadow.Enabled = true;
                        propertyGridShadow.SelectedObject = new ShdwWrapper(entry);
                        return;
                    }
            }
            propertyGridShadow.SelectedObject = null;
            propertyGridShadow.Enabled = false;
        }

        private void buttonCreateShadow_Click(object sender, EventArgs e)
        {
            var shdw = archive.GetSHDW(true);
            if (propertyGridShadow.SelectedObject == null)
            {
                var entry = new EntrySHDW()
                {
                    Model = GetAssetID()
                };
                entry.SetGame(asset.game);
                shdw.AddEntry(entry);
                SetupShdwBox();
            }
            else
            {
                shdw.RemoveEntry(GetAssetID());
                buttonCreateShadow.Text = "Create";
                propertyGridShadow.SelectedObject = null;
                propertyGridShadow.Enabled = false;
            }
        }

        private void propertyGridShadow_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (propertyGridShadow.SelectedObject == null)
                return;
            var shdw = archive.GetSHDW();
            shdw.AddEntry(((ShdwWrapper)propertyGridShadow.SelectedObject).Entry);
        }

        public bool CheckedForTemplate => checkBoxUseTemplates.Checked;

        private void checkBoxUseTemplates_Click(object sender, EventArgs e)
        {
            if (!archive.standalone)
                Program.MainForm.ClearModelTemplateFocus();
            checkBoxUseTemplates.Checked = true;
        }

        public void ClearModelTemplateFocus()
        {
            checkBoxUseTemplates.Checked = false;
        }
    }
}
