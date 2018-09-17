using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class InternalSoundEditor : Form, IInternalEditor
    {
        public InternalSoundEditor(Asset asset, ArchiveEditor archiveEditor)
        {
            InitializeComponent();
            TopMost = true;

            this.asset = asset;
            this.archiveEditor = archiveEditor;

            labelAssetName.Text = $"[{asset.AHDR.assetType.ToString()}] {asset.ToString()}";
        }

        private void InternalAssetEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            archiveEditor.RemoveInternalEditor(this);
        }

        private Asset asset;
        private ArchiveEditor archiveEditor;

        public uint GetAssetID()
        {
            return asset.AHDR.assetID;
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Select your GameCube DSP audio file"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                byte[] file = File.ReadAllBytes(openFileDialog.FileName);
                if (checkBoxSendToSNDI.Checked)
                {
                    archiveEditor.AddSoundToSNDI(file.Take(0x60).ToArray(), asset.AHDR.assetID);
                    asset.AHDR.data = file.Skip(0x60).ToArray();
                }
                else
                {
                    asset.AHDR.data = file;
                }
            }
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                FileName = asset.AHDR.ADBG.assetName + ".DSP",
                Filter = "DSP Files|*.dsp|All files|*"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                List<byte> file = new List<byte>();
                file.AddRange(archiveEditor.GetHeaderFromSNDI(asset.AHDR.assetID));
                file.AddRange(asset.AHDR.data);

                File.WriteAllBytes(saveFileDialog.FileName, file.ToArray());
            }
        }
    }
}
