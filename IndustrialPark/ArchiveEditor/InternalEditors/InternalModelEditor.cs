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

namespace IndustrialPark
{
    public partial class InternalModelEditor : InternalEditor
    {
        public InternalModelEditor(AssetRenderWareModel asset, ArchiveEditorFunctions archive, Action<Asset> updateListView)
        {
            InitializeComponent();

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
                SetupCollButton();
            }
            else
            {
                groupBoxPipeInfo.Enabled = false;
                groupBoxLevelOfDetail.Enabled = false;
                groupBoxCollisionModel.Enabled = false;
                groupBoxShadow.Enabled = false;
                checkBoxUseTemplates.Enabled = false;
            }

            if (asset.IsNativeData && archive.platform == Platform.PS2)
                buttonExport.Enabled = false;
        }

        public override void RefreshPropertyGrid()
        {
            propertyGridCollision.Refresh();
            propertyGridPipeInfo.Refresh();
            propertyGridShadow.Refresh();
            propertyGridLevelOfDetail.Refresh();
        }

        private void InternalAssetEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            asset.GetRenderWareModelFile()?.highlightAtomic = -1;
            archive.CloseInternalEditor(this);
        }

        private readonly AssetRenderWareModel asset;

        public override uint GetAssetID()
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
                List<IReversibleAction> actions = [archive.GetAssetRemovedAction(asset)];
                var flags = asset.AtomicFlags;
                flags[index] = (AtomicFlags)cb.SelectedItem;
                asset.AtomicFlags = flags;
                actions.Add(archive.GetAssetAddedAction(asset));
                Program.UndoBuffer.AddAction(actions);
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
                List<IReversibleAction> actions = [archive.GetAssetRemovedAction(asset)];
                asset.ApplyVertexColors(performOperation);
                actions.Add(archive.GetAssetAddedAction(asset));
                Program.UndoBuffer.AddAction(actions);
                archive.UnsavedChanges = true;
                updateListView(asset);
            }
        }

        private void buttonApplyRotation_Click(object sender, EventArgs e)
        {
            var factor = ApplyScale.GetVector(true);
            if (factor.HasValue)
            {
                List<IReversibleAction> actions = [archive.GetAssetRemovedAction(asset)];
                asset.ApplyRotation(MathUtil.DegreesToRadians(factor.Value.X), MathUtil.DegreesToRadians(factor.Value.Y), MathUtil.DegreesToRadians(factor.Value.Z));
                actions.Add(archive.GetAssetAddedAction(asset));
                Program.UndoBuffer.AddAction(actions);
                archive.UnsavedChanges = true;
                updateListView(asset);
            }
        }

        private void buttonApplyScale_Click(object sender, EventArgs e)
        {
            var factor = ApplyScale.GetVector(false);
            if (factor.HasValue)
            {
                List<IReversibleAction> actions = [archive.GetAssetRemovedAction(asset)];
                asset.ApplyScale(factor.Value);
                actions.Add(archive.GetAssetAddedAction(asset));
                Program.UndoBuffer.AddAction(actions);
                archive.UnsavedChanges = true;
                updateListView(asset);
            }
        }

        private void buttonImport_Click(object sender, EventArgs e)
        {
            if (ImportModel.GetSingleModel(archive.game, archive.platform, asset.assetType) is byte[] newModel)
            {
                List<IReversibleAction> actions = [archive.GetAssetRemovedAction(asset)];
                asset.Data = newModel;
                asset.Setup(Program.MainForm.renderer);
                actions.Add(archive.GetAssetAddedAction(asset));
                Program.UndoBuffer.AddAction(actions);
                ArchiveEditorFunctions.RecalculateAllMatrices();
                archive.UnsavedChanges = true;
                updateListView(asset);
                SetupTexturesBox();
                SetupCollButton();
            }
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            (bool ok, ExportFormatDescription format, string textureExtension) = ChooseTarget.GetTarget();

            if (ok)
            {
                SaveFileDialog a = new SaveFileDialog()
                {
                    FileName = asset.assetName,
                    Filter = format == null ? "RenderWare DFF|*.dff" : format.Description + "|*." + format.FileExtension,
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
            List<IReversibleAction> actions = [archive.GetAssetRemovedAction(asset)];
            var materials = MaterialEffectEditor.GetMaterials(asset.Materials);
            if (materials != null)
            {
                asset.Materials = materials;
                actions.Add(archive.GetAssetAddedAction(asset));
                Program.UndoBuffer.AddAction(actions);
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
            var hasPipt = archive.ContainsAssetWithType(AssetType.PipeInfoTable);
            var actions = new List<IReversibleAction>();
            var pipt = archive.GetPIPT(true);
            if (hasPipt)
                actions.Add(archive.GetAssetRemovedAction(pipt));
            var entry = new PipeInfo(asset.game)
            {
                Model = GetAssetID()
            };
            entry.SetGame(asset.game);
            pipt.AddEntry(entry);
            actions.Add(archive.GetAssetAddedAction(pipt));
            SetupPiptBox(pipeInfos.Count);
            Program.UndoBuffer.AddAction(actions);
        }

        private void buttonDeletePipeInfo_Click(object sender, EventArgs e)
        {
            if (pipeInfos.Count > 0 && currentPipeInfo > -1 && currentPipeInfo < pipeInfos.Count)
            {
                var actions = new List<IReversibleAction>();
                pipeInfos.RemoveAt(currentPipeInfo);
                var pipt = archive.GetPIPT();
                actions.Add(archive.GetAssetRemovedAction(pipt));
                pipt.RemoveEntry(GetAssetID());
                pipt.AddEntries(pipeInfos);
                actions.Add(archive.GetAssetAddedAction(pipt));
                SetupPiptBox(currentPipeInfo < 1 ? 0 : currentPipeInfo - 1);
                Program.UndoBuffer.AddAction(actions);
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
            List<IReversibleAction> actions = [archive.GetAssetRemovedAction(pipt)];
            pipt.RemoveEntry(GetAssetID());
            pipt.AddEntries(pipeInfos);
            actions.Add(archive.GetAssetAddedAction(pipt));
            Program.UndoBuffer.AddAction(actions);
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
            var hasLodt = archive.ContainsAssetWithType(AssetType.LevelOfDetailTable);
            var lodt = archive.GetLODT(true);
            List<IReversibleAction> actions = [];
            if (hasLodt)
                actions.Add(archive.GetAssetRemovedAction(lodt));
            if (propertyGridLevelOfDetail.SelectedObject == null)
            {
                var entry = new EntryLODT(archive.game)
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
            actions.Add(archive.GetAssetAddedAction(lodt));
            Program.UndoBuffer.AddAction(actions);
        }

        private void propertyGridLevelOfDetail_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (propertyGridLevelOfDetail.SelectedObject == null)
                return;
            var lodt = archive.GetLODT();
            List<IReversibleAction> actions = [archive.GetAssetRemovedAction(lodt)];
            lodt.AddEntry(((LodtWrapper)propertyGridLevelOfDetail.SelectedObject).Entry);
            actions.Add(archive.GetAssetAddedAction(lodt));
            Program.UndoBuffer.AddAction(actions);
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
                        buttonImportColl.Enabled = true;
                        return;
                    }
            }
            propertyGridCollision.SelectedObject = null;
            propertyGridCollision.Enabled = false;
        }

        private void buttonCreateCollision_Click(object sender, EventArgs e)
        {
            var hasColl = archive.ContainsAssetWithType(AssetType.CollisionTable);
            var coll = archive.GetCOLL(true);
            List<IReversibleAction> actions = [];
            if (hasColl)
                actions.Add(archive.GetAssetRemovedAction(coll));
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
                buttonImportColl.Enabled = false;
            }
            actions.Add(archive.GetAssetAddedAction(coll));
            Program.UndoBuffer.AddAction(actions);
        }

        private void propertyGridCollision_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (propertyGridCollision.SelectedObject == null)
                return;
            var coll = archive.GetCOLL();
            List<IReversibleAction> actions = [archive.GetAssetRemovedAction(coll)];
            coll.AddEntry(((CollWrapper)propertyGridCollision.SelectedObject).Entry);
            actions.Add(archive.GetAssetAddedAction(coll));
            Program.UndoBuffer.AddAction(actions);
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
            var hasShdw = archive.ContainsAssetWithType(AssetType.ShadowTable);
            var shdw = archive.GetSHDW(true);
            List<IReversibleAction> actions = [];
            if (hasShdw)
                actions.Add(archive.GetAssetRemovedAction(shdw));
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
            actions.Add(archive.GetAssetAddedAction(shdw));
            Program.UndoBuffer.AddAction(actions);
        }

        private void propertyGridShadow_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (propertyGridShadow.SelectedObject == null)
                return;
            var shdw = archive.GetSHDW();
            List<IReversibleAction> actions = [archive.GetAssetRemovedAction(shdw)];
            shdw.AddEntry(((ShdwWrapper)propertyGridShadow.SelectedObject).Entry);
            actions.Add(archive.GetAssetAddedAction(shdw));
            Program.UndoBuffer.AddAction(actions);
        }

        public bool CheckedForTemplate => checkBoxUseTemplates.Checked;

        private void checkBoxUseTemplates_Click(object sender, EventArgs e)
        {
            Program.MainForm.ClearModelTemplateFocus();
            checkBoxUseTemplates.Checked = true;
        }

        public void ClearModelTemplateFocus()
        {
            checkBoxUseTemplates.Checked = false;
        }

        private void SetupCollButton()
        {
            if (asset.IsNativeData)
                buildCollTreeButton.Enabled = false;
            if (asset.HasCollisionPLG)
                removeCollPlgButton.Enabled = true;
        }

        private void buildCollTreeButton_Click(object sender, EventArgs e)
        {
            List<IReversibleAction> actions = [archive.GetAssetRemovedAction(asset)];
            asset.BuildCollisionTree();
            SetupCollButton();
            actions.Add(archive.GetAssetAddedAction(asset));
            Program.UndoBuffer.AddAction(actions);
        }

        private void removeCollPlgButton_Click(object sender, EventArgs e)
        {
            List<IReversibleAction> actions = [archive.GetAssetRemovedAction(asset)];
            asset.RemoveCollisionTree();
            removeCollPlgButton.Enabled = false;
            actions.Add(archive.GetAssetAddedAction(asset));
            Program.UndoBuffer.AddAction(actions);
        }

        private void buttonImportColl_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog()
            {
                Filter = GetImportFilter(),
            };

            if (openFile.ShowDialog() != DialogResult.OK)
                return;

            RWSection rwmodel = CreateCollDFFFromAssimp(openFile.FileName);

            string collName = asset.assetName + "_COLL";
            Section_AHDR collmodelAhdr = new Section_AHDR(
                Functions.BKDRHash(collName),
                AssetType.Model,
                ArchiveEditorFunctions.AHDRFlagsFromAssetType(AssetType.Model),
                new Section_ADBG(0, collName, "", 0),
                rwmodel.GetBytes(asset.RenderWareVersion));
            Asset collModel = archive.AddAssetWithUniqueID(collmodelAhdr, archive.game, archive.platform.Endianness(), archive.IndexOfLayerOfType(LayerType.MODEL));

            List<IReversibleAction> actions = [archive.GetAssetAddedAction(collModel)];

            var coll = archive.GetCOLL();
            actions.Add(archive.GetAssetRemovedAction(coll));
            foreach (var item in coll.Entries)
                if (item.Model == asset.assetID)
                {
                    if (item.CollisionModel != 0 && archive.ContainsAsset(item.CollisionModel))
                    {
                        actions.Add(archive.GetAssetRemovedAction(item.CollisionModel));
                        archive.RemoveAsset(item.CollisionModel);
                    }
                    item.CollisionModel = collModel.assetID;
                    propertyGridCollision.SelectedObject = new CollWrapper(item);
                }
            actions.Add(archive.GetAssetAddedAction(coll));

            actions.Add(archive.GetAssetRemovedAction(asset));
            asset.RemoveCollisionTree();
            actions.Add(archive.GetAssetAddedAction(asset));
            foreach (var ae in Program.MainForm.archiveEditors)
                if (ae.archive == archive)
                    ae.PopulateAssetListAndComboBox();
            Program.UndoBuffer.AddAction(actions);
        }

        private void numericHighlightAtomic_ValueChanged(object sender, EventArgs e)
        {
            asset.GetRenderWareModelFile()?.highlightAtomic = (int)numericHighlightAtomic.Value;
        }

        private void numericHighlightMaterial_ValueChanged(object sender, EventArgs e)
        {
            numericHighlightAtomic.Value = -1;
        }

        private RWAnalyze rwAnalyze;

        private void buttonRWanalyze_Click(object sender, EventArgs e)
        {
            if (this.rwAnalyze != null)
            {
                this.rwAnalyze.Close();
                this.rwAnalyze.Dispose();
            }
            this.rwAnalyze = new RWAnalyze(asset.ModelAsRWSections);
            this.rwAnalyze.Show(this);
        }

        private void buttonEditHeader_Click(object sender, EventArgs e)
        {
            var ae = archive.GetArchiveEditor();
            if (ae.EditHeader(asset.assetID, out uint newAssetID))
                ae.OpenInternalEditor(newAssetID);
        }
    }
}
