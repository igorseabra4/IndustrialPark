using IndustrialPark.Models;
using RenderWareFile;
using SharpDX;
using System;
using System.IO;
using System.Windows.Forms;
using static IndustrialPark.Models.Assimp_IO;
using static IndustrialPark.Models.BSP_IO;
using static IndustrialPark.Models.BSP_IO_Shared;
using HipHopFile;
using Assimp;
using System.Linq;
using RenderWareFile.Sections;
using System.ComponentModel;

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
                comboBoxPiptPreset.Items.Clear();
                foreach (PiptPreset v in Enum.GetValues(typeof(PiptPreset)))
                    comboBoxPiptPreset.Items.Add(v);

                SetupPiptBox();
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
            var code = asset.assetType.GetCode();
            if (asset.assetType.IsDyna())
                code += $"/{asset.TypeString}";
            System.Diagnostics.Process.Start(AboutBox.WikiLink + code);
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

        private void SetupPiptBox()
        {
            var pipt = archive.GetPIPT();
            if (pipt != null)
            {
                foreach (var entry in pipt.Entries)
                    if (entry.Model == asset.assetID)
                    {
                        buttonCreatePipeInfo.Text = "Remove";
                        propertyGridPipeInfo.Enabled = true;
                        comboBoxPiptPreset.Enabled = true;
                        propertyGridPipeInfo.SelectedObject = new PipeInfoWrapper(entry);
                        return;
                    }
            }
            propertyGridPipeInfo.SelectedObject = null;
            comboBoxPiptPreset.Enabled = false;
        }

        private void buttonCreatePipeInfo_Click(object sender, EventArgs e)
        {
            var pipt = archive.GetPIPT(true);
            if (propertyGridPipeInfo.SelectedObject == null)
            {
                var entry = new PipeInfo()
                {
                    Model = asset.assetID
                };
                entry.SetGame(asset.game);
                pipt.AddEntry(entry);
                SetupPiptBox();
            }
            else
            {
                pipt.RemoveEntry(asset.assetID);
                buttonCreatePipeInfo.Text = "Create";
                propertyGridPipeInfo.SelectedObject = null;
                propertyGridPipeInfo.Enabled = false;
                comboBoxPiptPreset.Enabled = false;
            }
        }

        private void propertyGridPipeInfo_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (propertyGridPipeInfo.SelectedObject == null)
                return;
            var pipt = archive.GetPIPT();
            pipt.AddEntry(((PipeInfoWrapper)propertyGridPipeInfo.SelectedObject).Entry);
        }

        private void comboBoxPiptPreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (propertyGridPipeInfo.SelectedObject == null)
                return;
            ((PipeInfoWrapper)propertyGridPipeInfo.SelectedObject).PipeFlags_Preset = (PiptPreset)comboBoxPiptPreset.SelectedItem;
            propertyGridPipeInfo_PropertyValueChanged(sender, null);
            propertyGridPipeInfo.Refresh();
        }

        private void SetupLodtBox()
        {
            var lodt = archive.GetLODT();
            if (lodt != null)
            {
                foreach (var entry in lodt.Entries)
                    if (entry.BaseModel == asset.assetID)
                    {
                        buttonCreateLevelOfDetail.Text = "Remove";
                        propertyGridLevelOfDetail.Enabled = true;
                        propertyGridLevelOfDetail.SelectedObject = new LodtWrapper(entry);
                        return;
                    }
            }
            propertyGridLevelOfDetail.SelectedObject = null;
        }

        private void buttonCreateLevelOfDetail_Click(object sender, EventArgs e)
        {
            var lodt = archive.GetLODT(true);
            if (propertyGridLevelOfDetail.SelectedObject == null)
            {
                var entry = new EntryLODT()
                {
                    BaseModel = asset.assetID
                };
                entry.SetGame(asset.game);
                lodt.AddEntry(entry);
                SetupLodtBox();
            }
            else
            {
                lodt.RemoveEntry(asset.assetID);
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
                    if (entry.Model == asset.assetID)
                    {
                        buttonCreateCollision.Text = "Remove";
                        propertyGridCollision.Enabled = true;
                        propertyGridCollision.SelectedObject = new CollWrapper(entry);
                        return;
                    }
            }
            propertyGridCollision.SelectedObject = null;
        }

        private void buttonCreateCollision_Click(object sender, EventArgs e)
        {
            var coll = archive.GetCOLL(true);
            if (propertyGridCollision.SelectedObject == null)
            {
                var entry = new EntryCOLL()
                {
                    Model = asset.assetID
                };
                entry.SetGame(asset.game);
                coll.AddEntry(entry);
                SetupCollBox();
            }
            else
            {
                coll.RemoveEntry(asset.assetID);
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
                    if (entry.Model == asset.assetID)
                    {
                        buttonCreateShadow.Text = "Remove";
                        propertyGridShadow.Enabled = true;
                        propertyGridShadow.SelectedObject = new ShdwWrapper(entry);
                        return;
                    }
            }
            propertyGridShadow.SelectedObject = null;
        }

        private void buttonCreateShadow_Click(object sender, EventArgs e)
        {
            var shdw = archive.GetSHDW(true);
            if (propertyGridShadow.SelectedObject == null)
            {
                var entry = new EntrySHDW()
                {
                    Model = asset.assetID
                };
                entry.SetGame(asset.game);
                shdw.AddEntry(entry);
                SetupShdwBox();
            }
            else
            {
                shdw.RemoveEntry(asset.assetID);
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
    }

    public class PipeInfoWrapper
    {
        public PipeInfo Entry;

        public PipeInfoWrapper(PipeInfo entry)
        {
            Entry = entry;
        }

        public FlagBitmask SubObjectBits
        {
            get => Entry.SubObjectBits;
            set => Entry.SubObjectBits = value;
        }

        public int PipeFlags
        {
            get => Entry.PipeFlags;
            set => Entry.PipeFlags = value;
        }

        [ReadOnly(true)]
        public PiptPreset PipeFlags_Preset
        {
            get => Entry.PipeFlags_Preset;
            set => Entry.PipeFlags_Preset = value;
        }

        [DisplayName("AlphaCompareValue (0 - 255)")]
        public byte AlphaCompareValue
        {
            get => Entry.AlphaCompareValue;
            set => Entry.AlphaCompareValue = value;
        }

        [DisplayName("UnknownFlagB (0 - 15)")]
        public byte UnknownFlagB
        {
            get => Entry.UnknownFlagB;
            set => Entry.UnknownFlagB = value;
        }

        [DisplayName("UnknownFlagC (0 - 7)")]
        public byte UnknownFlagC
        {
            get => Entry.UnknownFlagC;
            set => Entry.UnknownFlagC = value;
        }

        public bool IgnoreFog
        {
            get => Entry.IgnoreFog;
            set => Entry.IgnoreFog = value;
        }

        public BlendFactorType DestinationBlend
        {
            get => Entry.DestinationBlend;
            set => Entry.DestinationBlend = value;
        }

        public BlendFactorType SourceBlend
        {
            get => Entry.SourceBlend;
            set => Entry.SourceBlend = value;
        }

        public LightingMode LightingMode
        {
            get => Entry.LightingMode;
            set => Entry.LightingMode = value;
        }

        public PiptCullMode CullMode
        {
            get => Entry.CullMode;
            set => Entry.CullMode = value;
        }

        public ZWriteMode ZWriteMode
        {
            get => Entry.ZWriteMode;
            set => Entry.ZWriteMode = value;
        }

        [DisplayName("UnknownFlagJ (0 - 3)")]
        public byte UnknownFlagJ
        {
            get => Entry.UnknownFlagJ;
            set => Entry.UnknownFlagJ = value;
        }

        [DisplayName("Unknown (Movie/Incredibles only)")]
        public int Unknown
        {
            get => Entry.Unknown;
            set => Entry.Unknown = value;
        }
    }

    public class LodtWrapper
    {
        public EntryLODT Entry;

        public LodtWrapper(EntryLODT entry)
        {
            Entry = entry;
        }

        public AssetSingle MaxDistance
        {
            get => Entry.MaxDistance;
            set => Entry.MaxDistance = value;
        }
        public AssetID LOD1_Model
        {
            get => Entry.LOD1_Model;
            set => Entry.LOD1_Model = value;
        }
        public AssetSingle LOD1_MinDistance
        {
            get => Entry.LOD1_MinDistance;
            set => Entry.LOD1_MinDistance = value;
        }
        public AssetID LOD2_Model
        {
            get => Entry.LOD2_Model;
            set => Entry.LOD2_Model = value;
        }
        public AssetSingle LOD2_MinDistance
        {
            get => Entry.LOD2_MinDistance;
            set => Entry.LOD2_MinDistance = value;
        }
        public AssetID LOD3_Model
        {
            get => Entry.LOD3_Model;
            set => Entry.LOD3_Model = value;
        }
        public AssetSingle LOD3_MinDistance
        {
            get => Entry.LOD3_MinDistance;
            set => Entry.LOD3_MinDistance = value;
        }
        [DisplayName("Unknown - Movie/Incredibles only")]
        public AssetSingle Unknown
        {
            get => Entry.Unknown;
            set => Entry.Unknown = value;
        }
    }

    public class CollWrapper
    {
        public EntryCOLL Entry;

        public CollWrapper(EntryCOLL entry)
        {
            Entry = entry;
        }

        public AssetID CollisionModel
        {
            get => Entry.CollisionModel;
            set => Entry.CollisionModel = value;
        }

        public AssetID CameraCollisionModel
        {
            get => Entry.CameraCollisionModel;
            set => Entry.CameraCollisionModel = value;
        }
    }

    public class ShdwWrapper
    {
        public EntrySHDW Entry;

        public ShdwWrapper(EntrySHDW entry)
        {
            Entry = entry;
        }

        public AssetID ShadowModel
        {
            get => Entry.ShadowModel;
            set => Entry.ShadowModel = value;
        }

        public int Unknown
        {
            get => Entry.Unknown;
            set => Entry.Unknown = value;
        }
    }
}
