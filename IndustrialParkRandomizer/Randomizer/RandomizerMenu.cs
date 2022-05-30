using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
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

            if (File.Exists(pathToSettings))
            {
                Randomizer_JSON_Settings settings = JsonConvert.DeserializeObject<Randomizer_JSON_Settings>(File.ReadAllText(pathToSettings));
                backupDir = settings.backupDir;
                useBackupDirectoryToolStripMenuItem.Checked = !string.IsNullOrEmpty(settings.backupDir);
                checkForUpdatesOnStartupToolStripMenuItem.Checked = settings.checkForUpdatesOnStartup;

                if (settings.checkForUpdatesOnStartup && AutomaticUpdater.UpdateIndustrialPark(out _))
                {
                    Close();
                    System.Diagnostics.Process.Start(Application.StartupPath + "/Randomizer.exe");
                }
            }
            else
            {
                MessageBox.Show("It appears this is your first time using Industrial Park's Randomizer.\nPlease consult the documentation on the Heavy Iron Modding Wiki to understand how to use the tool if you haven't already.");

                checkForUpdatesOnStartupToolStripMenuItem.Checked = true;

                File.WriteAllText(pathToSettings, JsonConvert.SerializeObject(new Randomizer_JSON_Settings(), Formatting.Indented));
            }

            randomizer = new Randomizer(0);
            randomizer.RandomSeed();
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
                void WindowsFolderPicker()
                {
                    using (CommonOpenFileDialog openFile = new CommonOpenFileDialog() { Title = "Please choose your backup files directory.", IsFolderPicker = true })
                        if (openFile.ShowDialog() == CommonFileDialogResult.Ok)
                        {
                            backupDir = openFile.FileName;
                            success = true;
                        }
                }

                WindowsFolderPicker();
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
                void WindowsFolderPicker()
                {
                    using (CommonOpenFileDialog openFile = new CommonOpenFileDialog() { Title = "Please choose your game root (files) directory.", IsFolderPicker = true })
                        if (openFile.ShowDialog() == CommonFileDialogResult.Ok)
                        {
                            randomizer.SetRootDir(openFile.FileName);
                            success = true;
                        }
                }

                WindowsFolderPicker();
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
            ProgressBar progressBar = new ProgressBar("Randomizing...");
            progressBar.Show();

            Enabled = false;
            new Thread(new ThreadStart(() =>
            {
                randomizer.Perform(backupDir, progressBar);
                ThreadDone(progressBar);
            })).Start();
        }

        private void ThreadDone(ProgressBar progressBar)
        {
            progressBar.Close();
            Invoke(new Action(() =>
            {
                Enabled = true;
            }));
            UpdateInterfaceFromRandomizer();
        }

        private void ButtonHelp_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://heavyironmodding.org/wiki/Randomizer");
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            randomizer = new Randomizer(randomizer.rootDir, randomizer.isDir, comboBoxGame.SelectedIndex);
            randomizer.SetSeed(textBoxSeed.Text);
            UpdateInterfaceFromRandomizer();
        }

        private void ButtonClear_Click(object sender, EventArgs e)
        {
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

                    if (settings.version == Randomizer.currVersion)
                        randomizer = settings;
                    else
                        MessageBox.Show("Randomizer settings file was made with a different version of Industrial Park and cannot be opened.");

                    UpdateInterfaceFromRandomizer();
                }
        }

        private bool programIsChangingStuff = false;

        private void UpdateInterfaceFromRandomizer()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateInterfaceFromRandomizer));
                return;
            }
            programIsChangingStuff = true;

            comboBoxGame.SelectedIndex = randomizer.game;

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

            textBoxSeed.Text = randomizer.seedText;
            labelSeed.Text = "Seed: " + randomizer.seed.ToString();

            DynamicTypeDescriptor dt = new DynamicTypeDescriptor(randomizer.settings.GetType());
            randomizer.settings.SetDynamicProperties(dt, comboBoxGame.SelectedIndex);
            propertyGridSettings.SelectedObject = dt.FromComponent(randomizer.settings);

            programIsChangingStuff = false;
        }

        private void CheckForUpdatesOnStartupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkForUpdatesOnStartupToolStripMenuItem.Checked = !checkForUpdatesOnStartupToolStripMenuItem.Checked;
        }

        private void propertyGridAsset_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            richTextBoxHelp.Text = e.NewSelection.PropertyDescriptor == null ? "" : e.NewSelection.PropertyDescriptor.Description;
        }

        private void comboBoxGame_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!programIsChangingStuff)
            {
                randomizer.game = comboBoxGame.SelectedIndex;
                randomizer.settings.ChangeForGame(randomizer.game);
                UpdateInterfaceFromRandomizer();
            }
        }

        private void checkForUpdatesOnEditorFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!AutomaticUpdater.VerifyEditorFiles())
                MessageBox.Show("No update found.");
        }
    }
}
