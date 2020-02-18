using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using HipHopFile;
using static HipHopFile.Functions;
using System.Linq;

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
        public int version = 55;
        public string rootDir;
        public bool isDir;
        public string seedText;
        public uint seed;
        public HashSet<RandomizerFlags> flags;
        public HashSet<RandomizerFlags2> flags2;
        public RandomizerSettings settings;

        public Randomizer()
        {
            flags = new HashSet<RandomizerFlags>();
            flags2 = new HashSet<RandomizerFlags2>();
            settings = new RandomizerSettings();
        }

        public Randomizer(string rootDir, bool isDir) : this()
        {
            flags = new HashSet<RandomizerFlags>() {
                RandomizerFlags.Warps,
                RandomizerFlags.Pickup_Positions,
                RandomizerFlags.Tiki_Types,
                RandomizerFlags.Tiki_Models,
                RandomizerFlags.Tiki_Allow_Any_Type,
                RandomizerFlags.Enemy_Types,
                RandomizerFlags.Enemies_Allow_Any_Type,
                RandomizerFlags.MovePoint_Radius,
                RandomizerFlags.Platform_Speeds,
                RandomizerFlags.Boulder_Settings,
                RandomizerFlags.Marker_Positions,
                RandomizerFlags.Player_Start,
                RandomizerFlags.Shiny_Object_Gates,
                RandomizerFlags.Spatula_Gates,
                RandomizerFlags.Timers,
                RandomizerFlags.Music,
                RandomizerFlags.Texture_Animations,
                RandomizerFlags.Disco_Floors,
                RandomizerFlags.Colors
            };

            this.rootDir = rootDir;
            this.isDir = isDir;
        }
        
        public void SetRootDir(string fileName)
        {
            rootDir = fileName;
            isDir = true;
        }

        public void SetFile(string fileName)
        {
            rootDir = fileName;
            isDir = false;
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
        
        public void Perform(string backupDir, ProgressBar progressBar)
        {
            int seed = (int)this.seed;
            
            if (isDir)
                PerformDirRandomizer(backupDir, progressBar);
            else
            {
                RandomizerFlags flags = 0;
                foreach (RandomizerFlags f in this.flags)
                    flags |= f;

                RandomizerFlags2 flags2 = 0;
                foreach (RandomizerFlags2 f in this.flags2)
                    flags2 |= f;

                RandomizableArchive archive = new RandomizableArchive();
                archive.OpenFile(rootDir, false, Platform.Unknown, true);
                archive.Randomize(seed, flags, flags2, settings, new Random(seed), out bool needToAddNumbers);
                archive.Save();

                string message = "Randomization complete!";

                if (flags.HasFlag(RandomizerFlags.Enemies_Allow_Any_Type))
                {
                    HashSet<VilType> vilTypes = archive.GetEnemyTypes();

                    if (vilTypes.Count > 0)
                    {
                        string vils = "";

                        foreach (VilType v in vilTypes)
                            vils += "\n** " + v.ToString();

                        message += "\n* Due to Enemies_Allow_Any_Type, you need to import the following enemy HIPs to the HOP:" + vils;
                    }
                }

                if (needToAddNumbers)
                    message += "\n* Due to Shiny_Object_Gates or Spatula_Gates, you need to import the numbers.HIP file to the HOP";                  
                
                MessageBox.Show(message);
            }

            ArchiveEditorFunctions.renderableAssetSetCommon.Clear();
            ArchiveEditorFunctions.renderableAssetSetJSP.Clear();
            ArchiveEditorFunctions.renderableAssetSetTrans.Clear();
            ArchiveEditorFunctions.renderingDictionary.Clear();
            ArchiveEditorFunctions.nameDictionary.Clear();
        }

        private void PerformDirRandomizer(string backupDir, ProgressBar progressBar)
        {
            if (!File.Exists(Path.Combine(rootDir, "boot.hip")) && Directory.Exists(Path.Combine(rootDir, "files")) && File.Exists(Path.Combine(rootDir, "files", "boot.hip")))
            {
                MessageBox.Show("You're supposed to select the 'files' folder, not the folder which contains it. I'll fix that for you.");
                rootDir = Path.Combine(rootDir, "files");
            }

            RandomizerFlags flags = 0;
            foreach (RandomizerFlags f in this.flags)
                flags |= f;
            RandomizerFlags2 flags2 = 0;
            foreach (RandomizerFlags2 f in this.flags2)
                flags2 |= f;

            int seed = (int)this.seed;

            if (flags.HasFlag(RandomizerFlags.Player_Characters))
                settings.charsOnAnyLevel = true;

            if (settings.charsOnAnyLevel)
            {
                var skipFiles = settings.skipFiles.ToList();
                skipFiles.Add("hb05");
                settings.skipFiles = skipFiles.ToArray();
            }

            List<(RandomizableArchive, RandomizableArchive)> levelPairs = new List<(RandomizableArchive, RandomizableArchive)>(); // HIP/HOP file pairs
            List<(string, string)> levelPathPairs = new List<(string, string)>(); // HIP/HOP path pairs

            string firstDir = string.IsNullOrEmpty(backupDir) ? rootDir : backupDir;
            List<string> hipPaths = new List<string>();
            foreach (string hipPath in Directory.GetFiles(firstDir))
            {
                if (Path.GetExtension(hipPath).ToLower() != ".hip" || FileInFirstBox(hipPath))
                    continue;
                hipPaths.Add(hipPath);
            }
            foreach (string dir in Directory.GetDirectories(firstDir))
                foreach (string hipPath in Directory.GetFiles(dir))
                {
                    if (Path.GetExtension(hipPath).ToLower() != ".hip" || FileInFirstBox(hipPath))
                        continue;
                    hipPaths.Add(hipPath);
                }

            progressBar.SetProgressBar(0, hipPaths.Count * 4 + 3, 1);

            List<string> warpNames = new List<string>(); // Names of all warps in the game for the Warps randomizer
            List<string> namesForBoot = new List<string>(); // Names of all levels in the game for sb.ini boot level randomizer

            Game game = Game.Unknown;
            Platform scoobyPlatform = Platform.Unknown;

            List<string> toSkip = new List<string>(settings.skipFiles.Length + settings.skipFilesWarps.Length); // Uses these to skip warps that shouldn't be randomized
            toSkip.AddRange(settings.skipFiles);
            toSkip.AddRange(settings.skipFilesWarps);

            bool platformVerified = false; // Not all game/plaftorm combinations support all methods

            // This goes through each file in the game, loads them into memory, and also gets the warp names
            foreach (string hipPath in hipPaths)
            {
                RandomizableArchive hip = new RandomizableArchive();
                hip.OpenFile(hipPath, false, scoobyPlatform, true);

                progressBar.PerformStep();

                // Verifies if game/platform combination is ok, also checks for EditorFiles in case it needs those
                if (!platformVerified)
                {
                    game = hip.game;
                    scoobyPlatform = hip.platform;

                    if (!Directory.Exists(ArchiveEditorFunctions.editorFilesFolder))
                    {
                        DialogResult dialogResult = MessageBox.Show("The IndustrialPark-EditorFiles folder has not been found under Resources. You must download it first. Do you wish to download it?", "Note", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                        if (dialogResult == DialogResult.Yes)
                            AutomaticUpdater.DownloadEditorFiles();
                    }
                    else
                        AutomaticUpdater.VerifyEditorFiles();

                    platformVerified = true;
                }

                if (flags.HasFlag(RandomizerFlags.Warps) && !FileInSecondBox(hipPath))
                    warpNames.AddRange(hip.GetWarpNames(toSkip));

                RandomizableArchive hop = null;
                string hopPath = Path.ChangeExtension(hipPath, ".HOP");

                if (File.Exists(hopPath))
                {
                    hop = new RandomizableArchive();
                    hop.OpenFile(hopPath, false, scoobyPlatform, true);

                    levelPairs.Add((hip, hop));

                    if (!FileInSecondBox(hipPath))
                        levelPathPairs.Add((hipPath, hopPath));

                    progressBar.PerformStep();
                }
                else
                {
                    levelPairs.Add((hip, null));

                    if (!FileInSecondBox(hipPath))
                        levelPathPairs.Add((hipPath, null));

                    progressBar.PerformStep();
                }

                string nameForBoot = Path.GetFileNameWithoutExtension(hipPath).ToUpper();
                if (settings.bootLevelMode == BootLevelMode.Random && !namesForBoot.Contains(nameForBoot))
                    namesForBoot.Add(nameForBoot);
            }

            HashSet<string> uinqueWarpNames = new HashSet<string>();
            if (flags.HasFlag(RandomizerFlags.Warps))
            {
                foreach (string s in warpNames)
                    uinqueWarpNames.Add(s);
                warpNames.Clear();
                warpNames.AddRange(uinqueWarpNames);
            }
            
            // Perform things on boot.hip
            if (flags.HasFlag(RandomizerFlags.Music) || settings.bootHipLodtMulti || settings.charsOnAnyLevel || settings.randomChars)
            {
                string bootPath = (string.IsNullOrEmpty(backupDir) ? rootDir : backupDir) + "/boot.hip";
                if (File.Exists(bootPath))
                {
                    var boot = new RandomizableArchive();
                    boot.OpenFile(bootPath, false, scoobyPlatform, true);

                    bool shouldSave = false;

                    if (flags.HasFlag(RandomizerFlags.Music))
                    {
                        if (boot.game == Game.Scooby)
                            shouldSave |= boot.RandomizeSounds(seed, false, true);
                        else
                            shouldSave |= boot.RandomizePlaylist();
                    }

                    if (settings.bootHipLodtMulti)
                        shouldSave |= boot.MultiplyLODT(settings.lodtValue);

                    if (settings.charsOnAnyLevel)
                    {
                        boot.ProgImportHip("Utility", "patrick.hip");
                        boot.ProgImportHip("Utility", "sandy.hip");
                        shouldSave = true;
                    }

                    if (settings.randomChars)
                        shouldSave |= boot.RandomizePlayerOnSpawn();

                    if (shouldSave)
                    {
                        if (string.IsNullOrEmpty(backupDir))
                            boot.Save();
                        else
                            boot.Save(boot.currentlyOpenFilePath.Replace(backupDir, rootDir));
                    }
                }
            }

            progressBar.PerformStep();

            if (settings.restoreRobotLaugh)
            {
                string mnu5path = (string.IsNullOrEmpty(backupDir) ? rootDir : backupDir) + "/mn/mnu5.hip";
                if (File.Exists(mnu5path))
                {
                    var mnu5 = new RandomizableArchive();
                    mnu5.OpenFile(mnu5path, false, scoobyPlatform, true);

                    if (mnu5.RestoreRobotLaugh())
                    {
                        if (string.IsNullOrEmpty(backupDir))
                            mnu5.Save();
                        else
                            mnu5.Save(mnu5.currentlyOpenFilePath.Replace(backupDir, rootDir));
                    }
                }
            }

            progressBar.PerformStep();

            // Time for the actual randomization process!

            List<(string, string, string)> warpRandomizerOutput = new List<(string, string, string)>();
            
            Random gateRandom = new Random(seed);
            Random warpRandom = new Random(seed);
            Random levelFilesR = new Random(seed);

            while (levelPairs.Count != 0)
            {
                bool item1shuffled = false;

                if (settings.charsOnAnyLevel)
                {
                    item1shuffled |= levelPairs[0].Item1.UnimportCharacters();
                    item1shuffled |= levelPairs[0].Item1.FixTreedome();
                }

                if (settings.charsOnAnyLevel || settings.disableCutscenes)
                    item1shuffled |= levelPairs[0].Item1.KillFinalBossCutscenes();

                if (flags.HasFlag(RandomizerFlags.Warps) && !FileInSecondBox(levelPairs[0].Item1.currentlyOpenFilePath))
                    item1shuffled |= levelPairs[0].Item1.SetWarpNames(warpRandom, ref warpNames, ref warpRandomizerOutput, uinqueWarpNames);

                item1shuffled |= levelPairs[0].Item1.Randomize(seed, flags, flags2, settings, gateRandom, out bool needToAddNumbers);

                progressBar.PerformStep();

                HashSet<VilType> enemyVils = levelPairs[0].Item1.GetEnemyTypes();

                bool item2shuffled = false;
                if (levelPairs[0].Item2 != null)
                {
                    if (enemyVils.Count != 0)
                    {
                        //item1shuffled |= levelPairs[0].Item1.UnimportEnemies(enemyVils);
                        item2shuffled |= levelPairs[0].Item2.ImportEnemyTypes(enemyVils);
                    }

                    if (needToAddNumbers)
                        item2shuffled |= levelPairs[0].Item2.ImportNumbers();

                    if (settings.charsOnAnyLevel)
                        item2shuffled = levelPairs[0].Item2.UnimportCharacters();

                    item2shuffled |= levelPairs[0].Item2.Randomize(seed, flags, flags2, settings, gateRandom, out _);

                    progressBar.PerformStep();
                }
                else
                    progressBar.PerformStep();

                levelPairs[0].Item1.CollapseLayers();
                if (levelPairs[0].Item2 != null)
                    levelPairs[0].Item2.CollapseLayers();

                // Save to a random different path (level files randomizer)
                if (flags2.HasFlag(RandomizerFlags2.Level_Files) && !FileInSecondBox(levelPairs[0].Item1.currentlyOpenFilePath))
                {
                    int newPathIndex = levelFilesR.Next(0, levelPathPairs.Count);

                    levelPairs[0].Item1.Save(levelPathPairs[newPathIndex].Item1);

                    if (string.IsNullOrEmpty(backupDir))
                        levelPairs[0].Item1.Save(levelPathPairs[newPathIndex].Item1);
                    else
                        levelPairs[0].Item1.Save(levelPathPairs[newPathIndex].Item1.Replace(backupDir, rootDir));

                    if (levelPairs[0].Item2 != null)
                    {
                        if (string.IsNullOrEmpty(backupDir))
                            levelPairs[0].Item2.Save(levelPathPairs[newPathIndex].Item2);
                        else
                            levelPairs[0].Item2.Save(levelPathPairs[newPathIndex].Item2.Replace(backupDir, rootDir));
                    }

                    levelPathPairs.RemoveAt(newPathIndex);
                }
                // Save to the same path (non level files or file set to not randomize warps)
                else
                {
                    if (item1shuffled)
                    {
                        if (string.IsNullOrEmpty(backupDir))
                            levelPairs[0].Item1.Save();
                        else
                            levelPairs[0].Item1.Save(levelPairs[0].Item1.currentlyOpenFilePath.Replace(backupDir, rootDir));
                    }

                    if (item2shuffled)
                    {
                        if (string.IsNullOrEmpty(backupDir))
                            levelPairs[0].Item2.Save();
                        else
                            levelPairs[0].Item2.Save(levelPairs[0].Item2.currentlyOpenFilePath.Replace(backupDir, rootDir));
                    }
                }

                levelPairs.RemoveAt(0);
            }

            string message = "Randomization complete!";
            
            if (!ApplyINISettings(backupDir, namesForBoot, game))
                message += "\n* Unable to find game settings INI, so these were not applied.";

            progressBar.PerformStep();

            if (warpRandomizerOutput.Count != 0)
            {
                WriteLog(warpRandomizerOutput);
                message += "\n* A warps_log.txt file with the result of the Warps randomizer was saved to your root folder. Don't look at that file, it spoils the fun.";
            }

            File.WriteAllText(rootDir + "/settings.json", JsonConvert.SerializeObject(this, Formatting.Indented));
            message += "\n* The settings.json file with the settings used was saved to your root folder.";

            MessageBox.Show(message);
        }
        
        private bool FileInFirstBox(string levelName)
        {
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

        private bool ApplyINISettings(string backupDir, List<string> namesForBoot, Game game)
        {
            namesForBoot.Remove("BC05");
            namesForBoot.Remove("GY04");
            namesForBoot.Remove("B302");
            namesForBoot.Remove("B303");
            namesForBoot.Remove("PG12");
            namesForBoot.Remove("S005");
            namesForBoot.Remove("S006");

            string[] ini;
            string filePath = (string.IsNullOrEmpty(backupDir) ? rootDir : backupDir) + "/sb.ini";
            if (File.Exists(filePath))
                ini = File.ReadAllLines(filePath);
            else
            {
                filePath = (string.IsNullOrEmpty(backupDir) ? rootDir : backupDir) + "/sd2.ini";
                if (File.Exists(filePath))
                    ini = File.ReadAllLines(filePath);
                else
                    return false;
            }

            Random r = new Random((int)seed);

            for (int i = 0; i < ini.Length; i++)
            {
                if (ini[i].StartsWith("BOOT=HB00") && settings.bootLevelMode == BootLevelMode.Default && settings.charsOnAnyLevel)
                {
                    ini[i] = "BOOT=HB02";
                    settings.BootLevel = "HB02";
                }
                else if (ini[i].StartsWith("BOOT=") && settings.bootLevelMode == BootLevelMode.Random)
                    ini[i] = "BOOT=" + namesForBoot[r.Next(0, namesForBoot.Count)];
                else if (ini[i].StartsWith("BOOT=") && settings.bootLevelMode == BootLevelMode.Set)
                {
                    if (settings.BootLevel == "HB00")
                        settings.BootLevel = "HB02";
                    if (!(settings.BootLevel == "HB01" && game == Game.Scooby))
                        ini[i] = "BOOT=" + settings.BootLevel;
                }
                else if (ini[i].StartsWith("ShowMenuOnBoot") && settings.dontShowMenuOnBoot)
                    ini[i] = "ShowMenuOnBoot=0";
                else if (ini[i].StartsWith("Menu") && settings.allMenuWarpsHB01)
                    ini[i] = ini[i].Substring(0, 9) + "HB 01 01 01 01 01 01 01 01";
                else if (ini[i].StartsWith("G.TakeDamage") && settings.cheatInvincible)
                    ini[i] = "G.TakeDamage=0";
                else if (ini[i].StartsWith("TakeDamage") && settings.cheatInvincible)
                    ini[i] = "TakeDamage=0";
                else if (ini[i].StartsWith("G.CheatPlayerSwitch") && settings.cheatPlayerSwitch)
                    ini[i] = "G.CheatPlayerSwitch=1";
                else if (ini[i].StartsWith("G.BubbleBowl") && settings.cheatBubbleBowl)
                    ini[i] = "G.BubbleBowl=1";
                else if (ini[i].StartsWith("G.CruiseBubble") && settings.cheatCruiseBubble)
                    ini[i] = "G.CruiseBubble=1";
                else if (ini[i].StartsWith("eSPECIAL_Spring") && game == Game.Scooby && settings.cheatSpring)
                    ini[i] = "eSPECIAL_Spring=1";
                else if (ini[i].StartsWith("eSPECIAL_FootballHelmet") && game == Game.Scooby && settings.cheatHelmet)
                    ini[i] = "eSPECIAL_FootballHelmet=1";
                else if (ini[i].StartsWith("eSPECIAL_LightningBolt") && game == Game.Scooby && settings.cheatSmash)
                    ini[i] = "eSPECIAL_LightningBolt=1";
                else if (ini[i].StartsWith("eSPECIAL_Umbrella") && game == Game.Scooby && settings.cheatUmbrella)
                    ini[i] = "eSPECIAL_Umbrella=1";
            }

            File.WriteAllLines(string.IsNullOrEmpty(backupDir) ? filePath : filePath.Replace(backupDir, rootDir), ini);

            return true;
        }

        private void WriteLog(List<(string, string, string)> warpRandomizerOutput)
        {
            using (StreamWriter streamWriter = new StreamWriter(new FileStream(rootDir + "/warps_log.txt", FileMode.Create)))
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

        public void LogWarps(System.Windows.Forms.ProgressBar progressBar)
        {
            Platform scoobyPlatform = Platform.Unknown;
            Dictionary<string, string> levelNames = new Dictionary<string, string>();

            string[] levelNamesFiles = File.ReadAllLines("scooby_levelnames.txt");
            foreach (string s in levelNamesFiles)
            {
                string[] split = s.Split('-');
                levelNames.Add(split[0].ToUpper(), split[1]);
            }

            List<string> folderNames = new List<string>() { rootDir };
            foreach (string dir in Directory.GetDirectories(rootDir))
                folderNames.Add(dir);

            progressBar.Minimum = 0;
            progressBar.Maximum = folderNames.Count;
            progressBar.Step = 1;

            using (StreamWriter writer = new StreamWriter(new FileStream("scooby_warps.txt", FileMode.Create)))
                foreach (string dir in folderNames)
                {
                    foreach (string hipPath in Directory.GetFiles(dir))
                        if (Path.GetExtension(hipPath).ToLower() == ".hip")
                        {
                            RandomizableArchive hip = new RandomizableArchive();
                            hip.OpenFile(hipPath, false, scoobyPlatform, true);

                            if (scoobyPlatform == Platform.Unknown)
                                scoobyPlatform = hip.platform;

                            List<string> warpNames = new List<string>();
                            warpNames.AddRange(hip.GetWarpNames(new List<string>()));

                            foreach (string s in warpNames)
                            {
                                string hipPathSmall = Path.GetFileNameWithoutExtension(hipPath).ToUpper();

                                if (levelNames.ContainsKey(hipPathSmall))
                                    hipPathSmall = levelNames[hipPathSmall];

                                string a = s;
                                if (levelNames.ContainsKey(s))
                                    a = levelNames[s];

                                writer.WriteLine(hipPathSmall + " -> " + a);
                            }

                            hip.Dispose(false);
                        }
                    progressBar.PerformStep();
                }
        }
    }
}