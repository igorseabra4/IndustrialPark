using Assimp;
using DiscordRPC;
using HipHopFile;
using IndustrialPark.Archive;
using IndustrialPark.Models;
using RenderWareFile;
using RenderWareFile.Sections;
using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Windows.Forms;
using static IndustrialPark.Models.Assimp_IO;
using static IndustrialPark.Models.BSP_IO;

namespace IndustrialPark
{
    public partial class InternalModelEditor : InternalEditor
    {
        public InternalModelEditor(AssetRenderWareModel asset, ArchiveEditor archive) : base (asset, archive)
        {
            InitializeComponent();

            SetupTexturesBox();
            if (asset.assetType == AssetType.Model)
            {
                SetupPiptBox(0);
                SetupLodtBox();
                SetupCollBox();
                SetupShdwBox();
                SetupCollButton();
                if (AssetFactoryService.TemplateModelAssetID == GetAssetID())
                    checkBoxUseTemplates.Checked = true;
            }
            else
            {
                groupBoxPipeInfo.Enabled = false;
                groupBoxLevelOfDetail.Enabled = false;
                groupBoxCollisionModel.Enabled = false;
                groupBoxShadow.Enabled = false;
                checkBoxUseTemplates.Enabled = false;
            }

            if (asset.IsNativeData && archive.archive.Platform == Platform.PS2)
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
            asset.GetRenderWareModelFile(Program.Renderer.Assets)?.highlightAtomic = -1;
        }

        private new AssetRenderWareModel asset => (AssetRenderWareModel)base.asset;

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
                asset.InvokePropertyChanged(true);
            };
            return cb;
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
                asset.InvokePropertyChanged(true);
            }
        }

        private void buttonApplyRotation_Click(object sender, EventArgs e)
        {
            var factor = ApplyScale.GetVector(true);
            if (factor.HasValue)
            {
                asset.ApplyRotation(MathUtil.DegreesToRadians(factor.Value.X), MathUtil.DegreesToRadians(factor.Value.Y), MathUtil.DegreesToRadians(factor.Value.Z));
                asset.InvokePropertyChanged(true);
            }
        }

        private void buttonApplyScale_Click(object sender, EventArgs e)
        {
            var factor = ApplyScale.GetVector(false);
            if (factor.HasValue)
            {
                asset.ApplyScale(factor.Value);
                asset.InvokePropertyChanged(true);
            }
        }

        private void buttonImport_Click(object sender, EventArgs e)
        {
            if (ImportModel.GetSingleModel(archiveEditor.archive.Game, archiveEditor.archive.Platform, asset.assetType) is byte[] newModel)
            {
                asset.Data = newModel;
                asset.InvokePropertyChanged(true);
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
                        var bitmaps = TextureConverter.GetTexturesAsBitmaps(archiveEditor.archive, asset.Textures);
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
                asset.InvokePropertyChanged(true);
                SetupTexturesBox();
            }
        }


        private List<PipeInfo> pipeInfos = null;
        private int currentPipeInfo = -1;

        private void GetPipeInfos()
        {
            var pipt = archiveEditor.archive.TableManager.GetPIPT(false);
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
            archiveEditor.BeginTransaction();

            var pipt = archiveEditor.archive.TableManager.GetPIPT(true);
            var entry = new PipeInfo(asset.game)
            {
                Model = GetAssetID()
            };
            entry.SetGame(asset.game);
            pipt.AddEntry(entry);

            SetupPiptBox(pipeInfos.Count);

            pipt.InvokePropertyChanged(false);
            archiveEditor.EndTransaction();
        }

        private void buttonDeletePipeInfo_Click(object sender, EventArgs e)
        {
            if (pipeInfos.Count > 0 && currentPipeInfo > -1 && currentPipeInfo < pipeInfos.Count)
            {
                archiveEditor.BeginTransaction();

                pipeInfos.RemoveAt(currentPipeInfo);
                var pipt = archiveEditor.archive.TableManager.GetPIPT();
                pipt.RemoveEntry(GetAssetID());
                pipt.AddEntries(pipeInfos);
                SetupPiptBox(currentPipeInfo < 1 ? 0 : currentPipeInfo - 1);

                pipt.InvokePropertyChanged(false);
                archiveEditor.EndTransaction();
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
            var pipt = archiveEditor.archive.TableManager.GetPIPT();
            pipt.RemoveEntry(GetAssetID());
            pipt.AddEntries(pipeInfos);
            pipt.InvokePropertyChanged(true);
        }

        private void SetupLodtBox()
        {
            var lodt = archiveEditor.archive.TableManager.GetLODT();
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
            archiveEditor.BeginTransaction();

            var lodt = archiveEditor.archive.TableManager.GetLODT(true);
            if (propertyGridLevelOfDetail.SelectedObject == null)
            {
                var entry = new EntryLODT(archiveEditor.archive.Game)
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

            lodt.InvokePropertyChanged(false);
            archiveEditor.EndTransaction();
        }

        private void propertyGridLevelOfDetail_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (propertyGridLevelOfDetail.SelectedObject == null)
                return;
            var lodt = archiveEditor.archive.TableManager.GetLODT();
            lodt.AddEntry(((LodtWrapper)propertyGridLevelOfDetail.SelectedObject).Entry);
            lodt.InvokePropertyChanged(true);
        }

        private void SetupCollBox()
        {
            var coll = archiveEditor.archive.TableManager.GetCOLL();
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
            archiveEditor.BeginTransaction();

            var coll = archiveEditor.archive.TableManager.GetCOLL(true);
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

            coll.InvokePropertyChanged(false);
            archiveEditor.EndTransaction();
        }

        private void propertyGridCollision_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (propertyGridCollision.SelectedObject == null)
                return;
            var coll = archiveEditor.archive.TableManager.GetCOLL();
            coll.AddEntry(((CollWrapper)propertyGridCollision.SelectedObject).Entry);
            coll.InvokePropertyChanged(true);
        }

        private void SetupShdwBox()
        {
            var shdw = archiveEditor.archive.TableManager.GetSHDW();
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
            archiveEditor.BeginTransaction();

            var shdw = archiveEditor.archive.TableManager.GetSHDW(true);
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

            shdw.InvokePropertyChanged(false);
            archiveEditor.EndTransaction();
        }

        private void propertyGridShadow_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (propertyGridShadow.SelectedObject == null)
                return;
            var shdw = archiveEditor.archive.TableManager.GetSHDW();
            shdw.AddEntry(((ShdwWrapper)propertyGridShadow.SelectedObject).Entry);
            shdw.InvokePropertyChanged(true);
        }

        private void checkBoxUseTemplates_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxUseTemplates.Checked)
            {
                Program.EditorManager.ClearModelTemplateFocus(this);
                AssetFactoryService.TemplateModelAssetID = GetAssetID();
            }
            else if (AssetFactoryService.TemplateModelAssetID == GetAssetID())
            {
                AssetFactoryService.TemplateModelAssetID = 0;
            }
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
            asset.BuildCollisionTree();
            SetupCollButton();
            asset.InvokePropertyChanged(true);
        }

        private void removeCollPlgButton_Click(object sender, EventArgs e)
        {
            asset.RemoveCollisionTree();
            removeCollPlgButton.Enabled = false;
            asset.InvokePropertyChanged(true);
        }

        private void buttonImportColl_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog()
            {
                Filter = GetImportFilter(),
            };

            if (openFile.ShowDialog() != DialogResult.OK)
                return;

            archiveEditor.BeginTransaction();

            var ac = archiveEditor.archive;

            RWSection rwmodel = CreateCollDFFFromAssimp(openFile.FileName);

            string collName = asset.assetName + "_COLL";

            Section_AHDR collmodelAhdr = new Section_AHDR(
                Functions.BKDRHash(collName),
                AssetType.Model,
                AssetService.AHDRFlagsFromAssetType(AssetType.Model),
                new Section_ADBG(0, collName, "", 0),
                rwmodel.GetBytes(asset.RenderWareVersion));

            Asset collModel = ac.ImportAssets([collmodelAhdr], ac.GetOrCreateLayerOfType(LayerType.MODEL), false)[0];

            var coll = ac.TableManager.GetCOLL();
            foreach (var item in coll.Entries)
                if (item.Model == asset.assetID)
                {
                    if (item.CollisionModel != 0 && ac.ContainsAsset(item.CollisionModel))
                        ac.RemoveAsset(item.CollisionModel);
                    item.CollisionModel = collModel.assetID;
                    propertyGridCollision.SelectedObject = new CollWrapper(item);
                }
            asset.RemoveCollisionTree();

            archiveEditor.PopulateAssetListAndComboBox();

            asset.InvokePropertyChanged(false);
            coll.InvokePropertyChanged(false);
            archiveEditor.EndTransaction();
        }

        private void numericHighlightAtomic_ValueChanged(object sender, EventArgs e)
        {
            asset.GetRenderWareModelFile(Program.Renderer.Assets)?.highlightAtomic = (int)numericHighlightAtomic.Value;
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
            if (archiveEditor.EditHeader(asset) is Asset newAsset)
                Program.EditorManager.OpenAssetEditor(newAsset, MousePosition);
        }
    }
}
