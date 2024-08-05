using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

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

            Thread t = new Thread(() =>
            {
                token.ThrowIfCancellationRequested();
                CloseDolphin();
                // this might throw a win32exception if .dol is not associated with Dolphin
                Process.Start(dolPath);
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
