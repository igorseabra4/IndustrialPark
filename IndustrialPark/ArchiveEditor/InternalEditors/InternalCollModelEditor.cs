using Assimp;
using IndustrialPark.Models;
using RenderWareFile;
using SharpDX;
using System;
using System.IO;
using System.Windows.Forms;
using static IndustrialPark.Models.Assimp_IO;

namespace IndustrialPark
{
    public partial class InternalCollModelEditor : InternalEditor
    {
        public InternalCollModelEditor(AssetRenderWareModel asset, ArchiveEditor archive) : base(asset, archive)
        {
            InitializeComponent();
            UpdateStats();
        }

        private new AssetRenderWareModel asset => (AssetRenderWareModel)base.asset;

        protected override void OnAssetReplaced()
        {
            UpdateStats();
        }

        private void exportButton_Click(object sender, EventArgs e)
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
                    else
                        ExportAssimp(Path.ChangeExtension(a.FileName, format.FileExtension), ReadFileMethods.ReadRenderWareFile(asset.Data), true, format, textureExtension, Matrix.Identity);
                }
            }
        }

        private void importButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog()
            {
                Filter = GetImportFilter(),
            };

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                asset.Data = ReadFileMethods.ExportRenderWareFile(CreateCollDFFFromAssimp(openFile.FileName), asset.RenderWareVersion);
                UpdateStats();
                asset.InvokePropertyChanged(true);
            }
        }

        private void buildCollTreeButton_Click(object sender, EventArgs e)
        {
            asset.BuildCollisionTree();
            asset.InvokePropertyChanged(true);
        }

        private void removeCollTreeButton_Click(object sender, EventArgs e)
        {
            asset.RemoveCollisionTree();
            asset.InvokePropertyChanged(true);
        }

        private void UpdateStats()
        {
            var model = asset.GetRenderWareModelFile(Program.Renderer.Assets);
            numVertsLabel.Text = model.vertexAmount.ToString();
            numTrisLabel.Text = model.triangleAmount.ToString();
        }
    }
}
