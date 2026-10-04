using System.Diagnostics;
using System.Threading.Tasks;

namespace IndustrialPark
{
    public static class RemoteControl
    {
        private const int CloseTimeoutMilliseconds = 10000;

        /// <summary>
        /// Closes all open Dolphin instances, then launches the DOL of the game through its file association.
        /// </summary>
        /// <param name="dolPath">The path to the game's main.dol.</param>
        /// <returns><c>false</c> if an open Dolphin instance didn't close in time, in which case the game isn't launched.</returns>
        /// <exception cref="System.ComponentModel.Win32Exception">The DOL couldn't be opened, such as when no program is associated with .dol files.</exception>
        public static async Task<bool> TryToRunGame(string dolPath)
        {
            if (!await Task.Run(CloseDolphin))
                return false;

            Process.Start(dolPath);
            return true;
        }

        public static bool CloseDolphin()
        {
            foreach (var p in Process.GetProcessesByName("Dolphin"))
                if (!p.HasExited)
                {
                    p.CloseMainWindow();
                    if (!p.WaitForExit(CloseTimeoutMilliseconds))
                        return false;
                }

            return true;
        }
    }
}
