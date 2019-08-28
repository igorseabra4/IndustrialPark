using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows.Forms;

namespace IndustrialPark.Randomizer
{
    public partial class RandomizerMenu : Form
    {
        private Randomizer randomizer;
        private string backupDir;
        private string pathToSettings => Application.StartupPath + "/randomizer_settings.json";

        public RandomizerMenu()
        {
            InitializeComponent();

            string rootDir = null;
            bool isDir = false;
            
            if (File.Exists(pathToSettings))
            {
                Randomizer_JSON_Settings settings = JsonConvert.DeserializeObject<Randomizer_JSON_Settings>(File.ReadAllText(pathToSettings));
                backupDir = settings.backupDir;
                useBackupDirectoryToolStripMenuItem.Checked = !string.IsNullOrEmpty(settings.backupDir);
                checkForUpdatesOnStartupToolStripMenuItem.Checked = settings.checkForUpdatesOnStartup;

                if (!string.IsNullOrEmpty(settings.rootDir))
                    rootDir = settings.rootDir;
                isDir = settings.isDir;

                if (settings.checkForUpdatesOnStartup && AutomaticUpdater.UpdateIndustrialPark(out _))
                {
                    Close();
                    System.Diagnostics.Process.Start(Application.StartupPath + "/Randomizer.exe");
                }
            }
            else
            {
                MessageBox.Show("It appears this is your first time using Industrial Park's Randomizer.\nPlease consult the documentation on the BFBB Modding Wiki to understand how to use the tool if you haven't already.");

                checkForUpdatesOnStartupToolStripMenuItem.Checked = true;

                File.WriteAllText(pathToSettings, JsonConvert.SerializeObject(new Randomizer_JSON_Settings(), Formatting.Indented));
            }
            
            randomizer = new Randomizer(rootDir, isDir);

            textBoxSeed.Text = new Random().Next().ToString();

            UpdateInterfaceFromRandomizer();
        }

        private void RandomizerMenu_FormClosing(object sender, FormClosingEventArgs e)
        {
            File.WriteAllText(pathToSettings, JsonConvert.SerializeObject(new Randomizer_JSON_Settings
            {
                checkForUpdatesOnStartup = checkForUpdatesOnStartupToolStripMenuItem.Checked,
                backupDir = backupDir,
                rootDir = randomizer.rootDir,
                isDir = randomizer.isDir
            }, Formatting.Indented));
        }

        private void ChooseBackupDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool success = false;
            try
            {
                using (CommonOpenFileDialog openFile = new CommonOpenFileDialog() { Title = "Please choose your backup files directory.", IsFolderPicker = true })
                    if (openFile.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        backupDir = openFile.FileName;
                        success = true;
                    }
            }
            catch
            {
                using (FolderBrowserDialog openFile = new FolderBrowserDialog())
                    if (openFile.ShowDialog() == DialogResult.OK)
                    {
                        backupDir = openFile.SelectedPath;
                        success = true;
                    }
            }
            if (success)
            {
                useBackupDirectoryToolStripMenuItem.Checked = true;
                UpdateInterfaceFromRandomizer();
            }
        }

        private void UseBackupDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            useBackupDirectoryToolStripMenuItem.Checked = !useBackupDirectoryToolStripMenuItem.Checked;
            if (useBackupDirectoryToolStripMenuItem.Checked)
            {
                chooseBackupDirectoryToolStripMenuItem.Enabled = true;
                ChooseBackupDirectoryToolStripMenuItem_Click(null, null);
            }
            else
            {
                chooseBackupDirectoryToolStripMenuItem.Enabled = false;
                backupDir = null;
            }
            UpdateInterfaceFromRandomizer();
        }

        private void ChooseRootDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool success = false;
            try
            {
                using (CommonOpenFileDialog openFile = new CommonOpenFileDialog() { Title = "Please choose your game root (files) directory.", IsFolderPicker = true })
                    if (openFile.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        randomizer.SetRootDir(openFile.FileName);
                        success = true;
                    }
            }
            catch
            {
                using (FolderBrowserDialog openFile = new FolderBrowserDialog())
                    if (openFile.ShowDialog() == DialogResult.OK)
                    {
                        randomizer.SetRootDir(openFile.SelectedPath);
                        success = true;
                    }
            }
            if (success)
            {
                UpdateInterfaceFromRandomizer();
            }
        }

        private void ChooseSingleFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFile = new OpenFileDialog())
                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    randomizer.SetFile(openFile.FileName);
                    UpdateInterfaceFromRandomizer();
                }
        }

        private void ButtonRandomSeed_Click(object sender, EventArgs e)
        {
            randomizer.RandomSeed();
            UpdateInterfaceFromRandomizer();
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            if (!programIsChangingStuff)
            {
                randomizer.SetSeed(textBoxSeed.Text);
                UpdateInterfaceFromRandomizer();
            }
        }
                
        private void buttonPerform_Click(object sender, EventArgs e)
        {
            ProgressBar progressBar = new ProgressBar("Performing Randomizer Operation", "Step");
            progressBar.Show();
            randomizer.Perform(backupDir, progressBar.GetProgressBar());
            progressBar.Close();
            UpdateInterfaceFromRandomizer();
        }
        
        private void ButtonHelp_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://battlepedia.org/Randomizer");
        }

        private void ButtonClear_Click(object sender, EventArgs e)
        {
            randomizer.flags.Clear();
            randomizer.settings.SetAllFalse();

            UpdateInterfaceFromRandomizer();
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFile = new SaveFileDialog()
            {
                Filter = "JSON Files|*.json|All files|*.*"
            })
                if (saveFile.ShowDialog() == DialogResult.OK)
                    File.WriteAllText(saveFile.FileName, JsonConvert.SerializeObject(randomizer, Formatting.Indented));
        }

        private void LoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFile = new OpenFileDialog()
            {
                Filter = "JSON Files|*.json|All files|*.*"
            })
                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    Randomizer settings = JsonConvert.DeserializeObject<Randomizer>(File.ReadAllText(openFile.FileName));

                    if (settings.version != new Randomizer().version)
                        MessageBox.Show("Note: randomizer settings file was made with an earlier or different version of Industrial Park. " +
                            "The program will attempt to open it, but doesn't guarantee the randomization result will be the same. " +
                            "If you need the exact same result, please use the same Industrial Park version (preferably the latest one) and a settings file saved by it.");

                    randomizer = settings;

                    UpdateInterfaceFromRandomizer();
                }
        }

        private bool programIsChangingStuff = false;

        private void UpdateInterfaceFromRandomizer()
        {
            programIsChangingStuff = true;

            if (string.IsNullOrEmpty(backupDir))
                labelBackupDir.Text = "Backup Directory: None";
            else
                labelBackupDir.Text = "Backup Directory: " + backupDir;

            if (!string.IsNullOrEmpty(randomizer.rootDir))
            {
                if (randomizer.isDir)
                {
                    labelRootDir.Text = "Game Directory: " + randomizer.rootDir;
                    buttonPerform.Enabled = true;
                }
                else
                {
                    labelRootDir.Text = "File: " + randomizer.rootDir;
                    buttonPerform.Enabled = true;
                }
            }
            else
                buttonPerform.Enabled = false;

            textBoxSeed.Text = randomizer.seedText.ToString();
            labelSeed.Text = "Seed: " + randomizer.seed.ToString();

            checkedListBoxMethods.Items.Clear();
            checkedListBoxNotRecommended.Items.Clear();

            int k = 0;
            foreach (RandomizerFlags i in Enum.GetValues(typeof(RandomizerFlags)))
            {
                checkedListBoxMethods.Items.Add(i);
                checkedListBoxMethods.SetItemChecked(k++, randomizer.flags.Contains(i));
            }
            k = 0;
            foreach (RandomizerFlags2 i in Enum.GetValues(typeof(RandomizerFlags2)))
            {
                checkedListBoxNotRecommended.Items.Add(i);
                checkedListBoxNotRecommended.SetItemChecked(k++, randomizer.flags2.Contains(i));
            }

            propertyGridAsset.SelectedObject = randomizer.settings;

            programIsChangingStuff = false;
        }
        
        private void CheckedListBoxMethods_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!programIsChangingStuff)
            {
                if (e.NewValue == CheckState.Checked)
                    randomizer.flags.Add((RandomizerFlags)checkedListBoxMethods.Items[e.Index]);
                else
                    randomizer.flags.Remove((RandomizerFlags)checkedListBoxMethods.Items[e.Index]);
            }
        }

        private void CheckedListBoxNotRecommended_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!programIsChangingStuff)
            {
                if (e.NewValue == CheckState.Checked)
                    randomizer.flags2.Add((RandomizerFlags2)checkedListBoxNotRecommended.Items[e.Index]);
                else
                    randomizer.flags2.Remove((RandomizerFlags2)checkedListBoxNotRecommended.Items[e.Index]);
            }
        }

        private void CheckForUpdatesOnStartupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkForUpdatesOnStartupToolStripMenuItem.Checked = !checkForUpdatesOnStartupToolStripMenuItem.Checked;
        }
    }
}