using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace IndustrialPark.Randomizer
{
    public partial class RandomizerMenu : Form
    {
        private Randomizer randomizer;

        public RandomizerMenu()
        {
            InitializeComponent();

            if (AutomaticUpdater.UpdateIndustrialPark(out _))
            {
                Close();
                System.Diagnostics.Process.Start(Application.StartupPath + "\\Randomizer.exe");
            }

            randomizer = new Randomizer();

            foreach (RandomizerFlags o in Enum.GetValues(typeof(RandomizerFlags)))
                checkedListBoxMethods.Items.Add(o);
            foreach (RandomizerFlagsP2 o in Enum.GetValues(typeof(RandomizerFlagsP2)))
                checkedListBoxNotRecommended.Items.Add(o);
            foreach (RandomizerFlagsP3 o in Enum.GetValues(typeof(RandomizerFlagsP3)))
                checkedListBoxSBINI.Items.Add(o);

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

        private void ButtonProbs_Click(object sender, EventArgs e)
        {
            new RandomizerSettingsMenu(randomizer.settings).ShowDialog();
        }

        private void ButtonClear_Click(object sender, EventArgs e)
        {
            randomizer.flags.Clear();
            randomizer.flags2.Clear();
            randomizer.flags3.Clear();

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

            for (int i = 0; i < checkedListBoxMethods.Items.Count; i++)
                checkedListBoxMethods.SetItemChecked(i, randomizer.flags.Contains((RandomizerFlags)i));
            for (int i = 0; i < checkedListBoxNotRecommended.Items.Count; i++)
                checkedListBoxNotRecommended.SetItemChecked(i, randomizer.flags2.Contains((RandomizerFlagsP2)i));
            for (int i = 0; i < checkedListBoxSBINI.Items.Count; i++)
                checkedListBoxSBINI.SetItemChecked(i, randomizer.flags3.Contains((RandomizerFlagsP3)i));
            
            programIsChangingStuff = false;
        }
        
        private void CheckedListBoxMethods_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!programIsChangingStuff)
            {
                if (e.NewValue == CheckState.Checked)
                    randomizer.flags.Add((RandomizerFlags)e.Index);
                else
                    randomizer.flags.Remove((RandomizerFlags)e.Index);
            }
        }

        private void CheckedListBoxNotRecommended_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!programIsChangingStuff)
            {
                if (e.NewValue == CheckState.Checked)
                    randomizer.flags2.Add((RandomizerFlagsP2)e.Index);
                else
                    randomizer.flags2.Remove((RandomizerFlagsP2)e.Index);
            }
        }

        private void CheckedListBoxSBINI_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!programIsChangingStuff)
            {
                if (e.NewValue == CheckState.Checked)
                    randomizer.flags3.Add((RandomizerFlagsP3)e.Index);
                else
                    randomizer.flags3.Remove((RandomizerFlagsP3)e.Index);
            }
        }
    }
}