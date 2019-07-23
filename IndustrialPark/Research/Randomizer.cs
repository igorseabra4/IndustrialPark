using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class Randomizer : Form
    {
        public Randomizer()
        {
            InitializeComponent();
        }

        private void Randomizer_Load(object sender, EventArgs e)
        {
            numericPlatTimeMin.Minimum = decimal.MinValue;
            numericPlatTimeMin.Maximum = decimal.MaxValue;
            numericPlatTimeMax.Minimum = decimal.MinValue;
            numericPlatTimeMax.Maximum = decimal.MaxValue;
            numericPlatSpeedMin.Minimum = decimal.MinValue;
            numericPlatSpeedMin.Maximum = decimal.MaxValue;
            numericPlatSpeedMax.Minimum = decimal.MinValue;
            numericPlatSpeedMax.Maximum = decimal.MaxValue;

            foreach (RandomizerFlags o in Enum.GetValues(typeof(RandomizerFlags)))
                checkedListBox1.Items.Add(o);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown) return;
            if (e.CloseReason == CloseReason.FormOwnerClosing) return;

            e.Cancel = true;
            Hide();
        }

        private string rootDir;
        private bool isDir = true;

        private void buttonChooseRoot_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog openFile = new CommonOpenFileDialog() { IsFolderPicker = true };
            if (openFile.ShowDialog() == CommonFileDialogResult.Ok)
            {
                rootDir = openFile.FileName;
                labelRootDir.Text = "Root Directory: " + rootDir;
                isDir = true;
                buttonPerform.Enabled = true;
            }
        }

        private void ButtonChooseFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                rootDir = openFile.FileName;
                labelRootDir.Text = "File: " + rootDir;
                isDir = false;
                buttonPerform.Enabled = true;
            }
        }
        
        private void buttonPerform_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 0;

            RandomizerFlags flags = 0;

            foreach (RandomizerFlags i in checkedListBox1.CheckedItems)
                flags |= i;
            
            if (isDir)
                PerformDirRandomizer(flags);
            
            else
            {
                ArchiveEditorFunctions archive = new ArchiveEditorFunctions();
                archive.OpenFile(rootDir, false, true);
                archive.Shuffle(flags, (float)numericPlatSpeedMin.Value, (float)numericPlatSpeedMax.Value, (float)numericPlatTimeMin.Value, (float)numericPlatTimeMax.Value);
                archive.Save();
            }

            MessageBox.Show("Randomizer complete");
        }

        private void PerformDirRandomizer(RandomizerFlags flags)
        {
            List<(ArchiveEditorFunctions, ArchiveEditorFunctions)> levelPairs = new List<(ArchiveEditorFunctions, ArchiveEditorFunctions)>();
            List<(string, string)> levelPathPairs = new List<(string, string)>();

            progressBar1.Minimum = 0;
            progressBar1.Maximum = 150;
            progressBar1.Step = 1;

            foreach (string dir in Directory.GetDirectories(rootDir))
                foreach (string hipPath in Directory.GetFiles(dir))
                    if (Path.GetExtension(hipPath).ToLower() == ".hip")
                    {
                        bool skip = checkBox1.Checked;

                        foreach (string s in richTextBox1.Lines)
                            if (Path.GetFileName(hipPath).ToLower().Contains(s.ToLower()))
                                skip = !checkBox1.Checked;

                        if (skip)
                            continue;

                        ArchiveEditorFunctions hip = new ArchiveEditorFunctions();
                        hip.OpenFile(hipPath, false, true);

                        ArchiveEditorFunctions hop = null;
                        string hopPath = Path.ChangeExtension(hipPath, ".HOP");

                        if (File.Exists(hopPath))
                        {
                            hop = new ArchiveEditorFunctions();
                            hop.OpenFile(hopPath, false, true);

                            levelPairs.Add((hip, hop));
                            levelPathPairs.Add((hipPath, hopPath));
                        }
                        else
                        {
                            levelPairs.Add((hip, null));
                            levelPathPairs.Add((hipPath, null));
                        }

                        progressBar1.PerformStep();
                    }

            progressBar1.Value = levelPairs.Count;
            progressBar1.Maximum = levelPairs.Count * 2;

            for (int i = 0; i < levelPairs.Count; i++)
            {
                bool item1shuffled = 
                levelPairs[i].Item1.Shuffle(flags, (float)numericPlatSpeedMin.Value, (float)numericPlatSpeedMax.Value, (float)numericPlatTimeMin.Value, (float)numericPlatTimeMax.Value);

                bool item2shuffled = false;
                if (levelPairs[i].Item2 != null)
                    item2shuffled = 
                    levelPairs[i].Item2.Shuffle(flags, (float)numericPlatSpeedMin.Value, (float)numericPlatSpeedMax.Value, (float)numericPlatTimeMin.Value, (float)numericPlatTimeMax.Value);

                if ((flags & RandomizerFlags.LevelFiles) != 0)
                {
                    Random r = new Random((int)DateTime.Now.ToBinary());

                    int newPathIndex = r.Next(0, levelPathPairs.Count);

                    levelPairs[i].Item1.Save(levelPathPairs[newPathIndex].Item1);

                    if (levelPairs[i].Item2 != null)
                        levelPairs[i].Item2.Save(levelPathPairs[newPathIndex].Item2);
                    
                    levelPathPairs.RemoveAt(newPathIndex);
                }
                else
                {
                    if (item1shuffled)
                        levelPairs[i].Item1.Save(levelPathPairs[i].Item1);

                    if (item2shuffled)
                        levelPairs[i].Item2.Save(levelPathPairs[i].Item2);
                }

                progressBar1.PerformStep();

                ArchiveEditorFunctions.renderableAssetSetCommon.Clear();
                ArchiveEditorFunctions.renderableAssetSetJSP.Clear();
                ArchiveEditorFunctions.renderableAssetSetTrans.Clear();
            }
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                label1.Text = "Patterns and files NOT to skip:";
            else
                label1.Text = "Patterns and files to skip:";

        }
    }
}