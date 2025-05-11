using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IndustrialPark
{
    public static class RemoteControl
    {
        // This method attempts to close all open Dolphin instances, then launch the DOL of the game.
        // The process is canceled if it takes more than 10 seconds.
        public static void TryToRunGame(string dolPath)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;

            // First check if any program is associated with .dol (preferably dolphin :)) to prevent a Win32Exception
            bool isAssociated;
            using (RegistryKey extKey = Registry.ClassesRoot.OpenSubKey(".dol"))
            {
                isAssociated = extKey != null && !string.IsNullOrEmpty(extKey.GetValue("") as string);
            }

            if (!isAssociated)
            {
                MessageBox.Show("Please associate the .dol extension with Dolphin Emulator", "Cannot launch dol", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

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
    }
}
