using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using static HipHopFile.Functions;

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
                checkedListBoxMethods.Items.Add(o);
            foreach (RandomizerFlagsP2 o in Enum.GetValues(typeof(RandomizerFlagsP2)))
                checkedListBoxNotRecommended.Items.Add(o);
            foreach (RandomizerFlagsP3 o in Enum.GetValues(typeof(RandomizerFlagsP3)))
                checkedListBoxSBINI.Items.Add(o);

            textBoxSeed.Text = new Random().Next().ToString();

            SetCheckBoxes(new RandoSettings() {
                flags =
                RandomizerFlags.Warps |
                RandomizerFlags.Pickup_Positions |
                RandomizerFlags.Tiki_Types |
                RandomizerFlags.Tiki_Models |
                RandomizerFlags.Tiki_Allow_Any_Type |
                RandomizerFlags.Enemy_Types |
                RandomizerFlags.MovePoint_Radius |
                RandomizerFlags.Platform_Speeds |
                RandomizerFlags.Boulder_Settings |
                RandomizerFlags.Marker_Positions |
                RandomizerFlags.Pointer_Positions |
                RandomizerFlags.Player_Start |
                RandomizerFlags.Timers |
                RandomizerFlags.Music |
                RandomizerFlags.Disco_Floors |
                RandomizerFlags.Double_BootHip_LODT |
                RandomizerFlags.Reduce_Warps_To_HB01,
                flags2 = 0,
                flags3 = 
                RandomizerFlagsP3.BootToHB01 |
                RandomizerFlagsP3.DontShowMenuOnBoot |
                RandomizerFlagsP3.AllMenuWarpsHB01 |
                RandomizerFlagsP3.ScoobyCheat_Spring
            });
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
        private uint seed;

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
            Random r = new Random((int)seed);

            RandomizerFlags flags = GetActiveFlags();
            RandomizerFlagsP2 flags2 = GetActiveFlagsP2();
            RandomizerFlagsP3 flags3 = GetActiveFlagsP3();
            
            if (isDir)
                PerformDirRandomizer(r, flags, flags2, flags3);
            else
            {
                ArchiveEditorFunctions archive = new ArchiveEditorFunctions();
                archive.OpenFile(rootDir, false, true);
                archive.Shuffle(r, flags, flags2, (float)numericPlatSpeedMin.Value, (float)numericPlatSpeedMax.Value, (float)numericPlatTimeMin.Value, (float)numericPlatTimeMax.Value);
                archive.Save();
            }

            ArchiveEditorFunctions.renderableAssetSetCommon.Clear();
            ArchiveEditorFunctions.renderableAssetSetJSP.Clear();
            ArchiveEditorFunctions.renderableAssetSetTrans.Clear();
            ArchiveEditorFunctions.renderingDictionary.Clear();
            ArchiveEditorFunctions.nameDictionary.Clear();

            MessageBox.Show("Randomization complete.");
            progressBar1.Value = 0;
        }

        private void PerformDirRandomizer(Random r, RandomizerFlags flags, RandomizerFlagsP2 flags2, RandomizerFlagsP3 flags3)
        {
            List<(ArchiveEditorFunctions, ArchiveEditorFunctions)> levelPairs = new List<(ArchiveEditorFunctions, ArchiveEditorFunctions)>();
            List<(string, string)> levelPathPairs = new List<(string, string)>();

            List<string> folderNames = new List<string>() { rootDir };
            foreach (string dir in Directory.GetDirectories(rootDir))
                folderNames.Add(dir);

            progressBar1.Minimum = 0;
            progressBar1.Maximum = 200;
            progressBar1.Step = 1;

            List<string> warpNames = new List<string>();
            List<string> namesForBoot = new List<string>();

            List<string> toSkip = new List<string>(richTextBoxSkip.Lines.Length + richTextBox2.Lines.Length);
            toSkip.AddRange(richTextBoxSkip.Lines);
            toSkip.AddRange(richTextBox2.Lines);
            toSkip.Add("jf80");
            toSkip.Add("tl01");

            foreach (string dir in folderNames)
                foreach (string hipPath in Directory.GetFiles(dir))
                    if (Path.GetExtension(hipPath).ToLower() == ".hip")
                    {
                        if (FileInFirstBox(hipPath))
                            continue;

                        ArchiveEditorFunctions hip = new ArchiveEditorFunctions();
                        hip.OpenFile(hipPath, false, true);

                        if ((flags & RandomizerFlags.Warps) != 0 && !FileInSecondBox(hipPath))
                                hip.GetWarpNames(ref warpNames, toSkip);

                        ArchiveEditorFunctions hop = null;
                        string hopPath = Path.ChangeExtension(hipPath, ".HOP");
                        
                        if (File.Exists(hopPath))
                        {
                            hop = new ArchiveEditorFunctions();
                            hop.OpenFile(hopPath, false, true);

                            levelPairs.Add((hip, hop));

                            if (!FileInSecondBox(hipPath))
                                levelPathPairs.Add((hipPath, hopPath));
                        }
                        else
                        {
                            levelPairs.Add((hip, null));

                            if (!FileInSecondBox(hipPath))
                                levelPathPairs.Add((hipPath, null));
                        }

                        string nameForBoot = Path.GetFileNameWithoutExtension(hipPath).ToUpper();
                        if (ShouldShuffle(flags3, RandomizerFlagsP3.RandomBootLevel) && !namesForBoot.Contains(nameForBoot))
                            namesForBoot.Add(nameForBoot);

                        progressBar1.PerformStep();
                    }

            if ((flags & RandomizerFlags.Reduce_Warps_To_HB01) != 0 && currentGame == HipHopFile.Game.BFBB)
            {
                bool found = false;
                for (int i = 0; i < warpNames.Count; i++)
                    if (warpNames[i] == "HB01")
                    {
                        if (!found)
                        {
                            found = true;
                            continue;
                        }

                        int index;
                        do
                            index = r.Next(0, warpNames.Count);
                        while (warpNames[index] == "HB01");

                        warpNames[i] = warpNames[index];
                    }
            }

            if ((flags & (RandomizerFlags.Music | RandomizerFlags.Double_BootHip_LODT)) != 0)
            {
                string bootPath = rootDir + "\\boot.hip";
                if (File.Exists(bootPath))
                {
                    ArchiveEditorFunctions boot = new ArchiveEditorFunctions();
                    boot.OpenFile(bootPath, false, true);

                    bool shouldSave = false;

                    if ((flags & RandomizerFlags.Music) != 0)
                    {
                        if (currentGame == HipHopFile.Game.BFBB)
                            shouldSave |= boot.RandomizePlaylist();
                        else
                        {
                            boot.ShuffleSounds(r, false, true);
                            shouldSave = true;
                        }
                    }

                    if ((flags & RandomizerFlags.Double_BootHip_LODT) != 0)
                        shouldSave |= boot.DoubleLODT();

                    if (shouldSave)
                        boot.Save();
                }
            }

            //if (currentGame == HipHopFile.Game.Scooby && ((flags & RandomizerFlags.Music) != 0))
            //{
            //    string mnu4Path = rootDir + "\\MN\\mnu4.HIP";
            //    if (File.Exists(mnu4Path))
            //    {
            //        ArchiveEditorFunctions mnu4 = new ArchiveEditorFunctions();
            //        mnu4.OpenFile(mnu4Path, false, true);
            //        if (mnu4.RandomizePlaylist())
            //            mnu4.Save();
            //    }
            //}

            progressBar1.Value = levelPairs.Count;
            progressBar1.Maximum = levelPairs.Count * 2;
            
            for (int i = 0; i < levelPairs.Count; i++)
            {
                bool item1shuffled = 
                levelPairs[i].Item1.Shuffle(r, flags, flags2, (float)numericPlatSpeedMin.Value, (float)numericPlatSpeedMax.Value, (float)numericPlatTimeMin.Value, (float)numericPlatTimeMax.Value);

                if ((flags & RandomizerFlags.Warps) != 0 && !FileInSecondBox(levelPairs[i].Item1.currentlyOpenFilePath))
                    levelPairs[i].Item1.SetWarpNames(r, ref warpNames, toSkip);

                bool item2shuffled = false;
                if (levelPairs[i].Item2 != null)
                    item2shuffled = 
                    levelPairs[i].Item2.Shuffle(r, flags, flags2, (float)numericPlatSpeedMin.Value, (float)numericPlatSpeedMax.Value, (float)numericPlatTimeMin.Value, (float)numericPlatTimeMax.Value);

                if ((flags2 & RandomizerFlagsP2.Level_Files) != 0 && !FileInSecondBox(levelPairs[i].Item1.currentlyOpenFilePath))
                {
                    int newPathIndex = r.Next(0, levelPathPairs.Count);

                    levelPairs[i].Item1.Save(levelPathPairs[newPathIndex].Item1);

                    if (levelPairs[i].Item2 != null)
                    {
                        levelPairs[i].Item2.Save(levelPathPairs[newPathIndex].Item2);
                    }
                    
                    levelPathPairs.RemoveAt(newPathIndex);
                }
                else
                {
                    if (item1shuffled)
                    {
                        levelPairs[i].Item1.Save();
                    }

                    if (item2shuffled)
                    {
                        levelPairs[i].Item2.Save();
                    }
                }

                progressBar1.PerformStep();
            }

            if (warpNames.Count != 0)
                MessageBox.Show("There was a problem with the warp randomizer. It is likely some warps are broken.");

            if (flags3 != 0)
                ApplyINISettings(flags3, r, namesForBoot);
        }

        private bool ShouldShuffle(RandomizerFlagsP3 flags, RandomizerFlagsP3 flag)
            => (flags & flag) != 0;

        private bool FileInFirstBox(string levelName)
        {
            foreach (string s in richTextBoxSkip.Lines)
                if (Path.GetFileNameWithoutExtension(levelName).ToLower().Contains(s.ToLower()))
                    return true;
            return false;
        }

        private bool FileInSecondBox(string levelName)
        {
            foreach (string s in richTextBox2.Lines)
                if (Path.GetFileNameWithoutExtension(levelName).ToLower().Contains(s.ToLower()))
                    return true;
            return false;
        }

        private void ApplyINISettings(RandomizerFlagsP3 flags3, Random r, List<string> namesForBoot)
        {
            string[] ini;
            string filePath = rootDir + "\\sb.ini";
            if (File.Exists(filePath))
                ini = File.ReadAllLines(filePath);
            else
            {
                filePath = rootDir + "\\sd2.ini";
                if (File.Exists(filePath))
                    ini = File.ReadAllLines(filePath);
                else
                {
                    MessageBox.Show("Unable to find game settings INI.");
                    return;
                }
            }

            for (int i = 0; i < ini.Length; i++)
            {
                if (ini[i].StartsWith("BOOT=") && ShouldShuffle(flags3, RandomizerFlagsP3.RandomBootLevel))
                    ini[i] = "BOOT=" + namesForBoot[r.Next(0, namesForBoot.Count)];
                else if (ini[i].StartsWith("BOOT=") && currentGame == HipHopFile.Game.BFBB && ShouldShuffle(flags3, RandomizerFlagsP3.BootToHB01))
                    ini[i] = "BOOT=HB01";
                else if (ini[i].StartsWith("ShowMenuOnBoot") && ShouldShuffle(flags3, RandomizerFlagsP3.DontShowMenuOnBoot))
                    ini[i] = "ShowMenuOnBoot=0";
                else if (ini[i].StartsWith("Menu") && ShouldShuffle(flags3, RandomizerFlagsP3.AllMenuWarpsHB01))
                    ini[i] = ini[i].Substring(0, 9) + "HB 01 01 01 01 01 01 01 01";
                else if (ini[i].StartsWith("G.TakeDamage") && ShouldShuffle(flags3, RandomizerFlagsP3.Cheat_Invincible))
                    ini[i] = "G.TakeDamage=0";
                else if (ini[i].StartsWith("TakeDamage") && ShouldShuffle(flags3, RandomizerFlagsP3.Cheat_Invincible))
                    ini[i] = "TakeDamage=0";
                else if (ini[i].StartsWith("G.BubbleBowl") && ShouldShuffle(flags3, RandomizerFlagsP3.BobCheat_BubbleBowl))
                    ini[i] = "G.BubbleBowl=1";
                else if (ini[i].StartsWith("G.CruiseBubble") && ShouldShuffle(flags3, RandomizerFlagsP3.BobCheat_CruiseBubble))
                    ini[i] = "G.CruiseBubble=1";
                else if (ini[i].StartsWith("eSPECIAL_Spring") && currentGame == HipHopFile.Game.Scooby && ShouldShuffle(flags3, RandomizerFlagsP3.ScoobyCheat_Spring))
                    ini[i] = "eSPECIAL_Spring=1";
                else if (ini[i].StartsWith("eSPECIAL_FootballHelmet") && currentGame == HipHopFile.Game.Scooby && ShouldShuffle(flags3, RandomizerFlagsP3.ScoobyCheat_Helmet))
                    ini[i] = "eSPECIAL_FootballHelmet=1";
                else if (ini[i].StartsWith("eSPECIAL_LightningBolt") && currentGame == HipHopFile.Game.Scooby && ShouldShuffle(flags3, RandomizerFlagsP3.ScoobyCheat_Smash))
                    ini[i] = "eSPECIAL_LightningBolt=1";
                else if (ini[i].StartsWith("eSPECIAL_Umbrella") && currentGame == HipHopFile.Game.Scooby && ShouldShuffle(flags3, RandomizerFlagsP3.ScoobyCheat_Umbrella))
                    ini[i] = "eSPECIAL_Umbrella=1";
            }

            File.WriteAllLines(filePath, ini);
        }

        private void ButtonHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show("The left text box specifies files and patterns which the randomizer will not be applied to (defaults are menus and other important files). " +
                "The right one specifies files to skip for the Warps and Level Files methods only. One per line.\n" +
                "For more information please check the Randomizer page in the Battlepedia wiki.");
        }

        private void ButtonRandomSeed_Click(object sender, EventArgs e)
        {
            byte[] bytes = new byte[4];
            new Random().NextBytes(bytes);
            textBoxSeed.Text = BitConverter.ToUInt32(bytes, 0).ToString();
            UpdateSeed();
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            UpdateSeed();
        }

        private void UpdateSeed()
        {
            try
            {
                seed = Convert.ToUInt32(textBoxSeed.Text);
            }
            catch
            {
                seed = HipHopFile.Functions.BKDRHash(textBoxSeed.Text);
            }
            labelSeed.Text = "Seed: " + seed.ToString();
        }

        private RandomizerFlags GetActiveFlags()
        {
            RandomizerFlags flags = 0;
            foreach (RandomizerFlags i in checkedListBoxMethods.CheckedItems)
                flags |= i;

            return flags;
        }

        private RandomizerFlagsP2 GetActiveFlagsP2()
        {
            RandomizerFlagsP2 flags2 = 0;
            foreach (RandomizerFlagsP2 i in checkedListBoxNotRecommended.CheckedItems)
                flags2 |= i;

            return flags2;
        }

        private RandomizerFlagsP3 GetActiveFlagsP3()
        {
            RandomizerFlagsP3 flags3 = 0;
            foreach (RandomizerFlagsP3 i in checkedListBoxSBINI.CheckedItems)
                flags3 |= i;

            return flags3;
        }

        private void ButtonSaveJson_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog()
            {
                Filter = "JSON Files|*.json|All files|*.*"
            };
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                string file = JsonConvert.SerializeObject(FromInstance(), Formatting.Indented);

                File.WriteAllText(saveFile.FileName, file);
            }
        }

        private void ButtonLoadJson_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog()
            {
                Filter = "JSON Files|*.json|All files|*.*"
            };
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                RandoSettings settings = JsonConvert.DeserializeObject<RandoSettings>(File.ReadAllText(openFile.FileName));

                if (settings.version == FromInstance().version)
                {
                    if (!string.IsNullOrEmpty(settings.seedText))
                        textBoxSeed.Text = settings.seedText;
                    else
                        textBoxSeed.Text = settings.seedNum.ToString();

                    seed = settings.seedNum;

                    SetCheckBoxes(settings);

                    numericPlatSpeedMin.Value = (decimal)settings.speedMin;
                    numericPlatSpeedMax.Value = (decimal)settings.speedMax;
                    numericPlatTimeMin.Value = (decimal)settings.timeMin;
                    numericPlatTimeMax.Value = (decimal)settings.timeMax;

                    richTextBoxSkip.Lines = settings.skip1;
                    richTextBox2.Lines = settings.skip2;

                    labelRandoJson.Text = "Loaded settings: " + openFile.FileName;
                }
                else
                {
                    MessageBox.Show("Randomizer settings cannot be opened because it was made with an earlier or different version of Industrial Park.");
                }
            }
        }

        private void SetCheckBoxes(RandoSettings settings)
        {
            int k = 0;
            foreach (RandomizerFlags i in Enum.GetValues(typeof(RandomizerFlags)))
            {
                checkedListBoxMethods.SetItemChecked(k, (i & settings.flags) != 0);
                k++;
            }

            k = 0;
            foreach (RandomizerFlagsP2 i in Enum.GetValues(typeof(RandomizerFlagsP2)))
            {
                checkedListBoxNotRecommended.SetItemChecked(k, (i & settings.flags2) != 0);
                k++;
            }

            k = 0;
            foreach (RandomizerFlagsP3 i in Enum.GetValues(typeof(RandomizerFlagsP3)))
            {
                checkedListBoxSBINI.SetItemChecked(k, (i & settings.flags3) != 0);
                k++;
            }
        }

        public class RandoSettings
        {
            public int version;
            public string seedText;
            public uint seedNum;
            public RandomizerFlags flags;
            public RandomizerFlagsP2 flags2;
            public RandomizerFlagsP3 flags3;
            public float speedMin;
            public float speedMax;
            public float timeMin;
            public float timeMax;
            public string[] skip1;
            public string[] skip2;
        }

        public RandoSettings FromInstance() => new RandoSettings()
        {
            version = 46,
            seedText = textBoxSeed.Text,
            seedNum = seed,
            flags = GetActiveFlags(),
            flags2 = GetActiveFlagsP2(),
            flags3 = GetActiveFlagsP3(),
            speedMin = (float)numericPlatSpeedMin.Value,
            speedMax = (float)numericPlatSpeedMax.Value,
            timeMin = (float)numericPlatTimeMin.Value,
            timeMax = (float)numericPlatTimeMax.Value,
            skip1 = richTextBoxSkip.Lines,
            skip2 = richTextBox2.Lines
        };

        private void ButtonClear_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBoxMethods.Items.Count; i++)
                checkedListBoxMethods.SetItemChecked(i, false);
            for (int i = 0; i < checkedListBoxNotRecommended.Items.Count; i++)
                checkedListBoxNotRecommended.SetItemChecked(i, false);
            for (int i = 0; i < checkedListBoxSBINI.Items.Count; i++)
                checkedListBoxSBINI.SetItemChecked(i, false);
        }
    }
}