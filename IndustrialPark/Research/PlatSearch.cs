using HipHopFile;
using Microsoft.WindowsAPICodePack.Dialogs;
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

namespace IndustrialPark
{
    public partial class PlatSearch : Form
    {
        public PlatSearch()
        {
            InitializeComponent();
        }

        private void EventSearch_Load(object sender, EventArgs e)
        {

        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown) return;
            if (e.CloseReason == CloseReason.FormOwnerClosing) return;

            e.Cancel = true;
            Hide();
        }

        private string rootDir;

        private void buttonChooseRoot_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog openFile = new CommonOpenFileDialog() { IsFolderPicker = true };
            if (openFile.ShowDialog() == CommonFileDialogResult.Ok)
            {
                rootDir = openFile.FileName;
                labelRootDir.Text = "Root Directory: " + rootDir;
            }
        }

        int total = 0;

        HashSet<PlatType> PlatformType_Header = new HashSet<PlatType>();
        HashSet<PlatTypeSpecific> PlatformType_Specific = new HashSet<PlatTypeSpecific>();
        HashSet<byte> UnknownByte_55 = new HashSet<byte>();
        HashSet<short> CollisionType = new HashSet<short>();
        HashSet<int> MovementLockMode = new HashSet<int>();
        HashSet<byte> UnknownByte_90 = new HashSet<byte>();
        HashSet<byte> UnknownByte_91 = new HashSet<byte>();
        HashSet<byte> UnknownByte_92 = new HashSet<byte>();
        HashSet<byte> UnknownByte_93 = new HashSet<byte>();
        HashSet<byte> MovementMode = new HashSet<byte>();
        HashSet<byte> MovementLoopType = new HashSet<byte>();
        HashSet<byte> MovementTranslation_Direction = new HashSet<byte>();
        HashSet<byte> MovementRotation_Axis = new HashSet<byte>();

        private void buttonPerform_Click(object sender, EventArgs e)
        {
            total = 0;

            PlatformType_Header = new HashSet<PlatType>();
            PlatformType_Specific = new HashSet<PlatTypeSpecific>();
            UnknownByte_55 = new HashSet<byte>();
            CollisionType = new HashSet<short>();
            MovementLockMode = new HashSet<int>();
            UnknownByte_90 = new HashSet<byte>();
            UnknownByte_91 = new HashSet<byte>();
            UnknownByte_92 = new HashSet<byte>();
            UnknownByte_93 = new HashSet<byte>();
            MovementMode = new HashSet<byte>();
            MovementLoopType = new HashSet<byte>();
            MovementTranslation_Direction = new HashSet<byte>();
            MovementRotation_Axis = new HashSet<byte>();

            AddFolder(rootDir);

            richTextBox2.Text = $"Found a total of {total} PLATs with \n {PlatformType_Header.Count} PlatformType_Header types: ";
            foreach (PlatType type in PlatformType_Header)
                richTextBox2.Text += type.ToString() + ", ";

            richTextBox2.Text += $"\n with {PlatformType_Specific.Count} PlatformType_Specific types: ";
            foreach (PlatTypeSpecific type in PlatformType_Specific)
                richTextBox2.Text += type.ToString() + ", ";

            richTextBox2.Text += $"\n with {UnknownByte_55.Count} UnknownByte_55 types: ";
            foreach (byte type in UnknownByte_55)
                richTextBox2.Text += type.ToString() + ", ";

            richTextBox2.Text += $"\n with {CollisionType.Count} CollisionType types: ";
            foreach (short type in CollisionType)
                richTextBox2.Text += type.ToString() + ", ";

            richTextBox2.Text += $"\n with {MovementLockMode.Count} MovementLockMode types: ";
            foreach (int type in MovementLockMode)
                richTextBox2.Text += type.ToString() + ", ";

            richTextBox2.Text += $"\n with {UnknownByte_90.Count} UnknownByte_90 types: ";
            foreach (byte type in UnknownByte_90)
                richTextBox2.Text += type.ToString() + ", ";

            richTextBox2.Text += $"\n with {UnknownByte_91.Count} UnknownByte_91 types: ";
            foreach (byte type in UnknownByte_91)
                richTextBox2.Text += type.ToString() + ", ";

            richTextBox2.Text += $"\n with {UnknownByte_92.Count} UnknownByte_92 types: ";
            foreach (byte type in UnknownByte_92)
                richTextBox2.Text += type.ToString() + ", ";

            richTextBox2.Text += $"\n with {UnknownByte_93.Count} UnknownByte_93 types: ";
            foreach (byte type in UnknownByte_93)
                richTextBox2.Text += type.ToString() + ", ";

            richTextBox2.Text += $"\n with {UnknownByte_93.Count} UnknownByte_93 types: ";
            foreach (byte type in UnknownByte_93)
                richTextBox2.Text += type.ToString() + ", ";

            richTextBox2.Text += $"\n with {MovementMode.Count} MovementMode types: ";
            foreach (byte type in MovementMode)
                richTextBox2.Text += type.ToString() + ", ";

            richTextBox2.Text += $"\n with {MovementLoopType.Count} MovementLoopType types: ";
            foreach (byte type in MovementLoopType)
                richTextBox2.Text += type.ToString() + ", ";

            richTextBox2.Text += $"\n with {MovementTranslation_Direction.Count} MovementTranslation_Direction types: ";
            foreach (byte type in MovementTranslation_Direction)
                richTextBox2.Text += type.ToString() + ", ";

            richTextBox2.Text += $"\n with {MovementRotation_Axis.Count} MovementRotation_Axis types: ";
            foreach (byte type in MovementRotation_Axis)
                richTextBox2.Text += type.ToString() + ", ";
        }

        private void AddFolder(string folderPath)
        {
            foreach (string s in Directory.GetFiles(folderPath))
            {
                if (Path.GetExtension(s).ToLower() == ".hip" || Path.GetExtension(s).ToLower() == ".hop")
                {
                    ArchiveEditorFunctions archive = new ArchiveEditorFunctions();
                    archive.OpenFile(s);
                    WriteWhatIFound(archive);
                    archive.Dispose();
                }
            }
            foreach (string s in Directory.GetDirectories(folderPath))
                AddFolder(s);
        }

        private void WriteWhatIFound(ArchiveEditorFunctions archive)
        {
            progressBar1.Minimum = 0;
            progressBar1.Maximum = archive.AssetCount;
            progressBar1.Step = 1;

            foreach (Asset asset in archive.GetAllAssets())
            {
                progressBar1.PerformStep();

                if (asset.AHDR.assetType != AssetType.PLAT)
                    continue;

                AssetPLAT PLAT = (AssetPLAT)asset;

                PlatformType_Header.Add(PLAT.PlatformType);
                PlatformType_Specific.Add(PLAT.PlatformSubtype);
                UnknownByte_55.Add(PLAT.UnknownByte_55);
                CollisionType.Add(PLAT.CollisionType);
                MovementLockMode.Add(PLAT.MovementLockMode);
                UnknownByte_90.Add(PLAT.UnknownByte_90);
                UnknownByte_91.Add(PLAT.UnknownByte_91);
                UnknownByte_92.Add(PLAT.UnknownByte_92);
                UnknownByte_93.Add(PLAT.UnknownByte_93);
                MovementMode.Add(PLAT.MovementMode);
                MovementLoopType.Add(PLAT.MovementLoopType);
                MovementTranslation_Direction.Add(PLAT.MovementTranslation_Direction);
                MovementRotation_Axis.Add(PLAT.MovementRotation_Axis);

                total++;
            }
        }
    }
}
