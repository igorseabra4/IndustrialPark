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

        public RandomizerMenu()
        {
            InitializeComponent();

            if (File.Exists(MainForm.pathToSettings))
            {
                IPSettings settings = JsonConvert.DeserializeObject<IPSettings>(File.ReadAllText(MainForm.pathToSettings));

                if (settings.CheckForUpdatesOnStartup && AutomaticUpdater.UpdateIndustrialPark(out _))
                {
                    Close();
                    System.Diagnostics.Process.Start(Application.StartupPath + "\\Randomizer.exe");
                }
            }
            else
            {
                MessageBox.Show("It appears this is your first time using Industrial Park's Randomizer.\nPlease consult the documentation on the BFBB Modding Wiki to understand how to use the tool if you haven't already.");

                File.WriteAllText(MainForm.pathToSettings, JsonConvert.SerializeObject(new IPSettings
                {
                    AutosaveOnClose = true,
                    AutoloadOnStartup = true,
                    LastProjectPath = null,
                    CheckForUpdatesOnStartup = true
                }, Formatting.Indented));
            }
            
            randomizer = new Randomizer();

            textBoxSeed.Text = new Random().Next().ToString();

            UpdateInterfaceFromRandomizer();
        }

        private void buttonChooseRoot_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog openFile = new CommonOpenFileDialog() { IsFolderPicker = true };
            if (openFile.ShowDialog() == CommonFileDialogResult.Ok)
            {
                randomizer.SetRootDir(openFile.FileName);
                UpdateInterfaceFromRandomizer();
            }
        }

        private void ButtonChooseFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
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
            randomizer.Perform(progressBar1);
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

        private void ButtonSaveJson_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog()
            {
                Filter = "JSON Files|*.json|All files|*.*"
            };
            if (saveFile.ShowDialog() == DialogResult.OK)
                File.WriteAllText(saveFile.FileName, JsonConvert.SerializeObject(randomizer, Formatting.Indented));
        }

        private void ButtonLoadJson_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog()
            {
                Filter = "JSON Files|*.json|All files|*.*"
            };
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                Randomizer settings = JsonConvert.DeserializeObject<Randomizer>(File.ReadAllText(openFile.FileName));

                if (settings.version != new Randomizer().version)
                    MessageBox.Show("Note: randomizer settings file was made with an earlier or different version of Industrial Park. " +
                        "The program will attempt to open it, but doesn't guarantee the randomization result will be the same. " +
                        "If you need the exact same result, please use the same Industrial Park version (preferably the latest one) and a settings file saved by it.");

                randomizer = settings;

                UpdateInterfaceFromRandomizer();

                labelRandoJson.Text = "Loaded settings: " + openFile.FileName;
            }
        }

        private bool programIsChangingStuff = false;

        private void UpdateInterfaceFromRandomizer()
        {
            programIsChangingStuff = true;

            if (!string.IsNullOrEmpty(randomizer.rootDir))
            {
                if (randomizer.isDir)
                {
                    labelRootDir.Text = "Root Directory: " + randomizer.rootDir;
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
            for (RandomizerFlags i = RandomizerFlags.Warps; i <= RandomizerFlags.Sounds; i = (RandomizerFlags)((int)i * 2))
            {
                checkedListBoxMethods.Items.Add(i);
                checkedListBoxMethods.SetItemChecked(k++, randomizer.flags.Contains(i));
            }
            k = 0;
            for (RandomizerFlags2 i = RandomizerFlags2.Level_Files; i <= RandomizerFlags2.Models; i = (RandomizerFlags2)((int)i * 2))
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
    }
}