using Assimp;
using HipHopFile;
using IndustrialPark.Models;
using RenderWareFile;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static IndustrialPark.Models.Assimp_IO;

namespace IndustrialPark
{
    public partial class InternalCollModelEditor : Form, IInternalEditor
    {
        public InternalCollModelEditor(AssetRenderWareModel asset, ArchiveEditorFunctions archive, Action<Asset> updateListView)
        {
            InitializeComponent();

            this.asset = asset;
            this.archive = archive;
            this.updateListView = updateListView;

            UpdateStats();
        }

        private readonly AssetRenderWareModel asset;
        private readonly ArchiveEditorFunctions archive;
        private readonly Action<Asset> updateListView;

        public uint GetAssetID()
        {
            return asset.assetID;
        }

        public void RefreshPropertyGrid()
        {
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
                archive.UnsavedChanges = true;
                asset.Setup(Program.MainForm.renderer);
                UpdateStats();
                updateListView(asset);
            }
        }

        private void buildCollTreeButton_Click(object sender, EventArgs e)
        {
            asset.BuildCollisionTree();
        }

        private void removeCollTreeButton_Click(object sender, EventArgs e)
        {
            asset.RemoveCollisionTree();
        }

        private void UpdateStats()
        {
            var model = asset.GetRenderWareModelFile();
            numVertsLabel.Text = model.vertexAmount.ToString();
            numTrisLabel.Text = model.triangleAmount.ToString();
        }
    }
}
