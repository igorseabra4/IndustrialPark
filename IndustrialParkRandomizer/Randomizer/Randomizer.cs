using HipHopFile;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static HipHopFile.Functions;

namespace IndustrialPark.Randomizer
{
    public enum RandomizerActMode
    {
        Backup,
        Directory,
        SingleFile
    }

    public class Randomizer
    {
        public static int currVersion = 60;
        public int version = currVersion;
        public string seedText;
        public uint seed;
        public int game;
        public RandomizerSettings settings;

        private Randomizer()
        {
        }

        public Randomizer(int game)
        {
            this.game = game;
            settings = new RandomizerSettings(game);
        }

        public void SetSeed(string seed)
        {
            seedText = seed;
            try
            {
                this.seed = Convert.ToUInt32(seed);
            }
            catch
            {
                this.seed = BKDRHash(seed);
            }
        }

        public void RandomSeed()
        {
            byte[] bytes = new byte[4];
            new Random().NextBytes(bytes);
            seed = BitConverter.ToUInt32(bytes, 0);
            seedText = seed.ToString();
        }

        public void Perform(string backupDir, string destinationDir, ProgressBar progressBar)
        {
            RandomizableArchive.random = new Random((int)this.seed);

            //if (isDir)
            PerformDirRandomizer(backupDir, destinationDir, progressBar);
            //else
            //{
            //RandomizableArchive archive = new RandomizableArchive();
            //archive.OpenFile(rootDir, false, Platform.Unknown);
            //archive.Randomize(settings);
            //archive.Save();

            //string message = "Randomization complete!";

            //if (settings.Enemies_Allow_Any_Type)
            //{
            //    HashSet<NpcType_BFBB> vilTypes = archive.GetEnemyTypes();

            //    if (vilTypes.Count > 0)
            //    {
            //        string vils = "";

            //        foreach (NpcType_BFBB v in vilTypes)
            //            vils += "\n** " + v.ToString();

            //        message += "\n* Due to Enemies_Allow_Any_Type, you need to import the following enemy HIPs to the HOP:" + vils;
            //    }
            //}

            //if (settings.Shiny_Object_Gates || settings.Spatula_Gates)
            //    message += "\n* Due to Shiny_Object_Gates or Spatula_Gates, you need to import the numbers.HIP file to the HOP";

            //MessageBox.Show(message);
            //}

            ArchiveEditorFunctions.renderableAssets.Clear();
            ArchiveEditorFunctions.renderableJSPs.Clear();
            ArchiveEditorFunctions.renderingDictionary.Clear();
            ArchiveEditorFunctions.ClearNameDictionary();
        }

        private void PerformDirRandomizer(string backupDir, string destinationDir, ProgressBar progressBar)
        {
            if (settings.PlayerCharacters)
                settings.UnlockCharacters = true;

            if (settings.UnlockCharacters)
            {
                var skipFiles = settings.skipFiles.ToList();
                skipFiles.Add("hb05");
                settings.skipFiles = skipFiles.ToArray();
            }

            string firstDir = string.IsNullOrEmpty(backupDir) ? destinationDir : backupDir;
            List<string> hipPaths = new List<string>();
            foreach (string dir in Directory.GetDirectories(firstDir))
                foreach (string hipPath in Directory.GetFiles(dir))
                {
                    if (Path.GetExtension(hipPath).ToLower() != ".hip" || FileInFirstBox(hipPath) || Path.GetFileNameWithoutExtension(hipPath).ToLower().Contains("us"))
                        continue;
                    hipPaths.Add(hipPath);
                }

            progressBar.SetProgressBar(0, hipPaths.Count * 3 + 3, 1);

            List<string> warpNames = new List<string>(); // Names of all warps in the game for the Warps randomizer
            List<string> namesForBoot = new List<string>(); // Names of all levels in the game for sb.ini boot level randomizer

            Game game = Game.Unknown;
            Platform scoobyPlatform = Platform.Unknown;

            List<string> toSkip = new List<string>(settings.skipFiles.Length + settings.skipFilesWarps.Length); // Uses these to skip warps that shouldn't be randomized
            toSkip.AddRange(settings.skipFiles);
            toSkip.AddRange(settings.skipFilesWarps);

            bool platformVerified = false; // Not all game/plaftorm combinations support all methods

            // This goes through each file in the game and gets the warp names
            foreach (string hipPath in hipPaths)
            {
                RandomizableArchive hip = new RandomizableArchive();
                hip.OpenFile(hipPath, false, scoobyPlatform);

                progressBar.PerformStep($" | Opening {Path.GetFileNameWithoutExtension(hip.currentlyOpenFilePath)}...");

                // Verifies if game/platform combination is ok, also checks for EditorFiles in case it needs those
                if (!platformVerified)
                {
                    game = hip.game;
                    scoobyPlatform = hip.platform;

                    if (Directory.Exists(ArchiveEditorFunctions.editorFilesFolder))
                        AutomaticUpdater.VerifyEditorFiles();
                    else
                    {
                        DialogResult dialogResult = MessageBox.Show("The IndustrialPark-EditorFiles folder has not been found under Resources. You must download it first. Do you wish to download it?", "Note", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                        if (dialogResult == DialogResult.Yes)
                            AutomaticUpdater.DownloadEditorFiles();
                    }

                    platformVerified = true;
                }

                if (settings.Warps && !FileInSecondBox(hipPath))
                    warpNames.AddRange(hip.GetWarpNames(toSkip));

                string nameForBoot = Path.GetFileNameWithoutExtension(hipPath).ToUpper();
                if (settings.bootLevelMode == BootLevelMode.Random && !namesForBoot.Contains(nameForBoot))
                    namesForBoot.Add(nameForBoot);
            }

            HashSet<string> uinqueWarpNames = new HashSet<string>();
            if (settings.Warps)
            {
                foreach (string s in warpNames)
                    uinqueWarpNames.Add(s);
                warpNames.Clear();
                warpNames.AddRange(uinqueWarpNames);
            }

            progressBar.PerformStep(" | Patching boot.hip...");

            // Perform things on boot.hip
            if (settings.Music || settings.bootHipLodtMulti || settings.UnlockCharacters || settings.RandomCharacters)
            {
                string bootPath = (string.IsNullOrEmpty(backupDir) ? destinationDir : backupDir) + "/boot.hip";
                if (File.Exists(bootPath))
                {
                    var boot = new RandomizableArchive();
                    boot.OpenFile(bootPath, false, scoobyPlatform);
                    boot.NoLayers = true;

                    bool shouldSave = false;

                    if (settings.Music)
                    {
                        if (boot.game == Game.Scooby)
                            shouldSave |= boot.RandomizeSounds(false, true);
                        else
                            shouldSave |= boot.RandomizePlaylist();
                    }

                    if (settings.bootHipLodtMulti)
                        shouldSave |= boot.MultiplyLODT(settings.lodtValue);

                    if (settings.UnlockCharacters)
                    {
                        boot.ProgImportHip("Utility", "patrick.hip");
                        boot.ProgImportHip("Utility", "sandy.hip");
                        shouldSave = true;
                    }

                    if (settings.RandomCharacters)
                        shouldSave |= boot.RandomizePlayerOnSpawn();

                    if (shouldSave)
                    {
                        var destPath = boot.currentlyOpenFilePath.Replace(backupDir, destinationDir);
                        var destDir = Path.GetDirectoryName(destPath);
                        if (!Directory.Exists(destDir))
                            Directory.CreateDirectory(destDir);
                        boot.Save(destPath);
                    }
                }
            }

            progressBar.PerformStep(" | Patching mnu5.hip...");

            if (settings.restoreRobotLaugh)
            {
                string mnu5path = (string.IsNullOrEmpty(backupDir) ? destinationDir : backupDir) + "/mn/mnu5.hip";
                if (File.Exists(mnu5path))
                {
                    var mnu5 = new RandomizableArchive();
                    mnu5.OpenFile(mnu5path, false, scoobyPlatform);
                    mnu5.NoLayers = true;

                    if (mnu5.RestoreRobotLaugh())
                    {
                        var destPath = mnu5.currentlyOpenFilePath.Replace(backupDir, destinationDir);
                        var destDir = Path.GetDirectoryName(destPath);
                        if (!Directory.Exists(destDir))
                            Directory.CreateDirectory(destDir);
                        mnu5.Save(destPath);
                    }
                }
            }

            progressBar.PerformStep(" | Patching mnu4.hip...");

            if (settings.widescreen)
            {
                string mnu4path = (string.IsNullOrEmpty(backupDir) ? destinationDir : backupDir) + "/mn/mnu4.hip";
                if (File.Exists(mnu4path))
                {
                    var mnu4 = new RandomizableArchive();
                    mnu4.OpenFile(mnu4path, false, scoobyPlatform);
                    mnu4.NoLayers = true;

                    if (mnu4.WidescreenMenu())
                    {
                        var destPath = mnu4.currentlyOpenFilePath.Replace(backupDir, destinationDir);
                        var destDir = Path.GetDirectoryName(destPath);
                        if (!Directory.Exists(destDir))
                            Directory.CreateDirectory(destDir);
                        mnu4.Save(destPath);
                    }
                }
            }

            // Time for the actual randomization process!

            List<(string, string, string)> warpRandomizerOutput = new List<(string, string, string)>();

            var hops = new List<string>();

            foreach (string hipPath in hipPaths)
            {
                RandomizableArchive hip = new RandomizableArchive();
                hip.OpenFile(hipPath, false, scoobyPlatform);
                hip.NoLayers = true;

                progressBar.PerformStep($" | Randomizing {Path.GetFileNameWithoutExtension(hip.currentlyOpenFilePath)}...");

                string hopPath = Path.ChangeExtension(hipPath, ".HOP");

                if (File.Exists(hopPath))
                {
                    hip.ImportHip(hopPath, true);
                    hops.Add(hopPath.Replace(backupDir, "").Substring(1));
                }

                if (settings.UnlockCharacters)
                {
                    hip.UnimportCharacters();
                    hip.FixTreedome();
                }

                if (settings.UnlockCharacters || settings.disableCutscenes)
                    hip.KillFinalBossCutscenes();

                if (settings.Warps && !FileInSecondBox(hip.currentlyOpenFilePath))
                    hip.SetWarpNames(toSkip, ref warpNames, ref warpRandomizerOutput, uinqueWarpNames);

                hip.Randomize(settings);

                progressBar.PerformStep($" | Saving {Path.GetFileNameWithoutExtension(hip.currentlyOpenFilePath)}...");

                var destPath = hip.currentlyOpenFilePath.Replace(backupDir, destinationDir);
                var destDir = Path.GetDirectoryName(destPath);
                if (!Directory.Exists(destDir))
                    Directory.CreateDirectory(destDir);
                hip.Save(destPath);
            }

            RemoveFiles = string.Join("\n", hops);

            string message = "Randomization complete!";

            ApplyINISettings(namesForBoot, game);

            progressBar.PerformStep();

            if (warpRandomizerOutput.Count != 0)
            {
                WriteLog(warpRandomizerOutput, destinationDir);
                message += "\n* A warps_log.txt file with the result of the Warps randomizer was saved to your root folder. Don't look at that file, it spoils the fun.";
            }

            File.WriteAllText(destinationDir + "/settings.json", JsonConvert.SerializeObject(this, Formatting.Indented));
            message += "\n* The settings.json file with the settings used was saved to your root folder.";

            MessageBox.Show(message);
        }

        private readonly string[] skipHips = new string[] { "mn", "sp", "pl" };

        private bool FileInFirstBox(string levelName)
        {
            foreach (string s in skipHips)
                if (Path.GetFileNameWithoutExtension(levelName).ToLower().Contains(s.ToLower()))
                    return true;
            foreach (string s in settings.skipFiles)
                if (Path.GetFileNameWithoutExtension(levelName).ToLower().Contains(s.ToLower()))
                    return true;
            return false;
        }

        private bool FileInSecondBox(string levelName)
        {
            foreach (string s in settings.skipFilesWarps)
                if (Path.GetFileNameWithoutExtension(levelName).ToLower().Contains(s.ToLower()))
                    return true;
            return false;
        }

        private void ApplyINISettings(List<string> namesForBoot, Game game)
        {
            namesForBoot.Remove("BC05");
            namesForBoot.Remove("GY04");
            namesForBoot.Remove("B302");
            namesForBoot.Remove("B303");
            namesForBoot.Remove("PG12");
            namesForBoot.Remove("S005");
            namesForBoot.Remove("S006");

            var replacements = new Dictionary<string, string>
            {
                ["ShowMenuOnBoot"] = settings.dontShowMenuOnBoot ? "0" : "1",
                ["G.TakeDamage"] = settings.cheatInvincible ? "0" : "1",
                ["TakeDamage"] = settings.cheatInvincible ? "0" : "1",
                ["G.CheatPlayerSwitch"] = settings.PowerupCheatsBFBB.PlayerSwitch ? "1" : "0",
                ["G.BubbleBowl"] = settings.PowerupCheatsBFBB.BubbleBowl ? "1" : "0",
                ["G.CruiseBubble"] = settings.PowerupCheatsBFBB.CruiseBubble ? "1" : "0",
                ["eSPECIAL_Spring"] = settings.PowerupCheatsScooby.Spring ? "1" : "0",
                ["eSPECIAL_FootballHelmet"] = settings.PowerupCheatsScooby.Helmet ? "1" : "0",
                ["eSPECIAL_LightningBolt"] = settings.PowerupCheatsScooby.Smash ? "1" : "0",
                ["eSPECIAL_Umbrella"] = settings.PowerupCheatsScooby.Umbrella ? "1" : "0",
                ["G.KarateSpin"] = GetLine(settings.PowerupCheatsMovie.SB_KarateSpin),
                ["G.Bash"] = GetLine(settings.PowerupCheatsMovie.SB_Bash),
                ["G.MachoBowl"] = GetLine(settings.PowerupCheatsMovie.SB_Bowl),
                ["G.WaveGuitar"] = GetLine(settings.PowerupCheatsMovie.SB_Guitar),
                ["G.BellyBump"] = GetLine(settings.PowerupCheatsMovie.Pat_StarSpin),
                ["G.Cartwheel"] = GetLine(settings.PowerupCheatsMovie.Pat_Cartwheel),
                ["G.BellyFlop"] = GetLine(settings.PowerupCheatsMovie.Pat_BellyFlop),
                ["G.Throw"] = GetLine(settings.PowerupCheatsMovie.Pat_Throw),
                ["G.HealthSB"] = GetLine(settings.PowerupCheatsMovie.SB_Health),
                ["G.HealthPat"] = GetLine(settings.PowerupCheatsMovie.Pat_Health),
                //["G.BBashHeight"] = (0.315 * (settings.Set_Scale_Physics ? settings.scaleFactor : 1.0)).ToString(),
                //["G.Gravity"] = (30.0 / (settings.Set_Scale_Physics ? settings.scaleFactor : 1.0)).ToString(),
                //["Carry.ThrowMaxDist"] = (12.0 * (settings.Set_Scale_Physics ? settings.scaleFactor : 1.0)).ToString(),
                //["SB.MoveSpeed"] = $"{0.6 * (settings.Set_Scale_Physics ? settings.scaleFactor : 1.0)},{4.0 * (settings.Set_Scale_Physics ? settings.scaleFactor : 1.0)},{5.0 * (settings.Set_Scale_Physics ? settings.scaleFactor : 1.0)},  {0.1 * (settings.Set_Scale_Physics ? settings.scaleFactor : 1.0)},{0.8 * (settings.Set_Scale_Physics ? settings.scaleFactor : 1.0)},{1.0 * (settings.Set_Scale_Physics ? settings.scaleFactor : 1.0)}",
                //["Patrick.MoveSpeed"] = $"{0.6 * (settings.Set_Scale_Physics ? settings.scaleFactor : 1.0)},{2.5 * (settings.Set_Scale_Physics ? settings.scaleFactor : 1.0)},{5.0 * (settings.Set_Scale_Physics ? settings.scaleFactor : 1.0)},  {0.1 * (settings.Set_Scale_Physics ? settings.scaleFactor : 1.0)},{0.6 * (settings.Set_Scale_Physics ? settings.scaleFactor : 1.0)},{1.0 * (settings.Set_Scale_Physics ? settings.scaleFactor : 1.0)}",
                //["Sandy.MoveSpeed"] = $"{0.6 * (settings.Set_Scale_Physics ? settings.scaleFactor : 1.0)},{2.5 * (settings.Set_Scale_Physics ? settings.scaleFactor : 1.0)},{5.0 * (settings.Set_Scale_Physics ? settings.scaleFactor : 1.0)},  {0.1 * (settings.Set_Scale_Physics ? settings.scaleFactor : 1.0)},{0.6 * (settings.Set_Scale_Physics ? settings.scaleFactor : 1.0)},{1.0 * (settings.Set_Scale_Physics ? settings.scaleFactor : 1.0)}",
                //["SB.cb.fly.live_time"] = (6.0 * (settings.Set_Scale_Physics ? settings.scaleFactor : 1.0)).ToString(),
            };

            var ini = new List<string>();

            foreach (var entry in replacements)
                ini.Add($"{entry.Key}={entry.Value}");

            if (game == Game.BFBB && settings.bootLevelMode == BootLevelMode.Default && settings.UnlockCharacters)
            {
                ini.Add("BOOT=HB02");
                settings.BootLevel = "HB02";
            }
            else if (settings.bootLevelMode == BootLevelMode.Random)
                ini.Add("BOOT=" + namesForBoot[RandomizableArchive.random.Next(0, namesForBoot.Count)]);
            else if (settings.bootLevelMode == BootLevelMode.Set)
            {
                if (settings.BootLevel == "HB00")
                    settings.BootLevel = "HB02";
                if (game == Game.Incredibles && settings.BootLevel == "HB01")
                    settings.BootLevel = "BB02";
                if (settings.BootLevel != "HB01" || game == Game.BFBB)
                    ini.Add("BOOT=" + settings.BootLevel);
            }

            if (settings.allMenuWarpsHB01 && game == Game.BFBB)
                for (int i = 1; i <= 15; i++)
                    ini.Add($"Menu{i:D2}=HB 01 01 01 01 01 01 01 01");

            INI = string.Join("\n", ini);
        }

        [JsonIgnore]
        public string INI { get; private set; }

        [JsonIgnore]
        public string RemoveFiles { get; private set; }

        private string GetLine<T>(T status) where T : struct, IConvertible
        {
            return Convert.ToInt32(Enum.Parse(typeof(T), status.ToString()) as Enum).ToString();
        }

        private void WriteLog(List<(string, string, string)> warpRandomizerOutput, string destinationDir)
        {
            using (StreamWriter streamWriter = new StreamWriter(new FileStream(destinationDir + "/warps_log.txt", FileMode.Create)))
            {
                HashSet<string> uniqueNames = new HashSet<string>();
                foreach (var s in warpRandomizerOutput)
                    uniqueNames.Add(s.Item1);

                streamWriter.WriteLine($"// Warps randomizer log:");
                streamWriter.WriteLine($"// Format is: (Level)-[Default Warp Destination]->(New Warp Destination)");
                streamWriter.WriteLine($"// This log is actually a script in the Cypher Query Language, which can be used to generate a graph in Neo4j.");
                streamWriter.WriteLine();

                streamWriter.WriteLine($"CREATE");
                foreach (var s in uniqueNames)
                    streamWriter.WriteLine($"({s}: Level {{ name: '{s}'}} ),");
                for (int i = 0; i < warpRandomizerOutput.Count - 1; i++)
                {
                    var s = warpRandomizerOutput[i];
                    streamWriter.WriteLine($"({s.Item1})-[:{s.Item2}]->({s.Item3}),");
                }
                var c = warpRandomizerOutput[warpRandomizerOutput.Count - 1];
                streamWriter.WriteLine($"({c.Item1})-[:{c.Item2}]->({c.Item3})");
            }
        }
    }
}