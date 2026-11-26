using HipHopFile;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace IndustrialPark
{
    public static class RemoteControl
    {
        // try to run game
        public static void RunGame(Game game, string hipFileName)
        {
            var levelName = Path.GetFileNameWithoutExtension(hipFileName).ToUpper();
            var rootFolderName = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(hipFileName)));
            var filesPath = Path.Combine(rootFolderName, "files");
            var dolPath = Path.Combine(rootFolderName, "sys", "main.dol");
            var modJsonExists = File.Exists(Path.Combine(rootFolderName, "mod.json"));

            string hmmMessage;

            if (modJsonExists)
            {
                var modId = Path.GetFileNameWithoutExtension(rootFolderName);
                var message = $"run {game} {levelName} {modId}";
                bool hmm = SendMessageToHeavyModManager(message);
                if (hmm)
                    return;
                hmmMessage = "Heavy Mod Manager is not running.";
            }
            else
            {
                hmmMessage = "Heavy Mod Manager mod not identified.";
            }

            if (!Directory.Exists(filesPath) || !File.Exists(dolPath))
                throw new Exception($"{hmmMessage}\nCould not find necessary game files for launching Dolphin.");

            var validIniNames = new string[] { "sb.ini", "sb04.ini", "sd2.ini", "in.ini", "in2.ini", "rats.ini" };
            var iniFileName = Directory.GetFiles(filesPath).FirstOrDefault(f => validIniNames.Contains(Path.GetFileName(f).ToLower()));
            if (string.IsNullOrEmpty(iniFileName) || !File.Exists(iniFileName))
                throw new Exception($"{hmmMessage}\nCould not find necessary game files for launching Dolphin.");

            if (!string.IsNullOrEmpty(DolphinPath) && File.Exists(DolphinPath))
            {
                ApplyBootLevelToIni(levelName, iniFileName);
                CloseDolphin();
                var result = Process.Start(DolphinPath, [dolPath]);
                if (result != null)
                    return;
                throw new Exception($"{hmmMessage}\nError launching Dolphin from path.");
            }

            if (IsAssociated())
            {
                ApplyBootLevelToIni(levelName, iniFileName);
                ExecuteDol(dolPath);
                return;
            }
            
            throw new Exception($"{hmmMessage}\nDolphin path not set.\n.dol extension has no file association.");
        }

        private static void ApplyBootLevelToIni(string levelName, string iniFileName)
        {
            string[] ini = File.ReadAllLines(iniFileName);
            for (int i = 0; i < ini.Length; i++)
                if (ini[i].StartsWith("BOOT"))
                {
                    ini[i] = "BOOT=" + levelName;
                    break;
                }
            File.WriteAllLines(iniFileName, ini);
        }

        private static bool IsAssociated()
        {
            // Check if any program is associated with .dol (preferably dolphin :)) to prevent a Win32Exception
            using RegistryKey extKey = Registry.ClassesRoot.OpenSubKey(".dol");
            return extKey != null && !string.IsNullOrEmpty(extKey.GetValue("") as string);
        }

        // This method attempts to close all open Dolphin instances, then launch the DOL of the game.
        // The process is canceled if it takes more than 10 seconds.
        public static void ExecuteDol(string dolPath)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;

            Thread t = new Thread(() =>
            {
                token.ThrowIfCancellationRequested();
                CloseDolphin();

                ProcessStartInfo startInfo = new ProcessStartInfo()
                {
                    FileName = dolPath,
                    UseShellExecute = true,
                };

                Process process = new Process() { StartInfo = startInfo };
                process.Start();
            });

            ScheduleAction(cts.Cancel, 10000);

            t.Start();
        }

        public static async void ScheduleAction(Action action, int ms)
        {
            await Task.Delay(ms);
            action();
        }

        public static bool CloseDolphin()
        {
            foreach (var p in Process.GetProcessesByName("Dolphin"))
                if (!p.HasExited)
                {
                    p.CloseMainWindow();
                    p.WaitForExit();
                }

            return true;
        }

        private const int WM_COPYDATA = 0x004A;

        [StructLayout(LayoutKind.Sequential)]
        private struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            public IntPtr lpData;
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref COPYDATASTRUCT lParam);

        public static bool SendMessageToHeavyModManager(string message)
        {
            IntPtr window = FindWindow(null, "Heavy Mod Manager");

            if (window == IntPtr.Zero)
                return false;

            IntPtr textPtr = Marshal.StringToHGlobalUni(message);

            try
            {
                var data = new COPYDATASTRUCT
                {
                    dwData = IntPtr.Zero,
                    cbData = (message.Length + 1) * sizeof(char),
                    lpData = textPtr
                };

                return SendMessage(window, WM_COPYDATA, IntPtr.Zero, ref data) != IntPtr.Zero;
            }
            finally
            {
                Marshal.FreeHGlobal(textPtr);
            }
        }

        public static string DolphinPath { get; set; }
    }
}
