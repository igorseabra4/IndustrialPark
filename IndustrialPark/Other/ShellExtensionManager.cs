using SharpShell.ServerRegistration;
using ShellExtensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IndustrialPark
{
    /// <summary>
    /// Manages the GameCube thumbnail handler shell extension.
    /// </summary>
    public static class ShellExtensionManager
    {
        /// <summary>
        /// Restarts explorer.exe so that the thumbnail handler becomes active
        /// </summary>
        private static void RestartExplorer()
        {
            // Kill all instances of explorer.exe
            var explorerProcesses = Process.GetProcessesByName("explorer");
            foreach (var process in explorerProcesses)
            {
                try
                {
                    process.Kill();
                    process.WaitForExit();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to kill explorer.exe: {ex.Message}", "Failed to kill explorer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            // Wait for a moment to ensure explorer.exe is completely closed
            Thread.Sleep(1000);

            // Start a new instance of explorer.exe
            try
            {
                Process.Start("explorer.exe");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start explorer.exe: {ex.Message}", "Failed to kill explorer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Gets the registration type, depending on the bitness of the user's system
        /// </summary>
        private static RegistrationType GetSystemRegistrationType()
        {
            SharpShell.ServerRegistration.RegistrationType type = RegistrationType.OS32Bit;

            if (Environment.Is64BitOperatingSystem)
            {
                type = RegistrationType.OS64Bit;
            }

            return type;
        }

        /// <summary>
        /// Registers the GameCube banner thumbnail handler extension
        /// Requires administrative priveleges
        /// </summary>
        public static void RegisterBannerExtension()
        {
            var type = GetSystemRegistrationType();
            var handler = new GameCubeBannerThumbnailHandler();
            bool codeBase = true;

            try
            {
                SharpShell.ServerRegistration.ServerRegistrationManager.InstallServer(handler, type, codeBase);
                SharpShell.ServerRegistration.ServerRegistrationManager.RegisterServer(handler, type);
            }
            catch (Exception ex)
            {
                MessageBox.Show("GameCube Banner Thumbnail Handler was not registered. " + ex.Message, "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult restartExplorer = MessageBox.Show("GameCube Banner Thumbnail Handler has been registered. Would you like to restart explorer.exe?", "Registration Successful", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (restartExplorer == DialogResult.Yes)
            {
                RestartExplorer();
            }
        }

        /// <summary>
        /// Unregisters the GameCube banner thumbnail handler extension
        /// Requires administrative priveleges
        /// </summary>
        public static void UnregisterBannerExtension()
        {
            var type = GetSystemRegistrationType();
            var handler = new GameCubeBannerThumbnailHandler();

            SharpShell.ServerRegistration.ServerRegistrationManager.UnregisterServer(handler, type);
            bool uninstalled = SharpShell.ServerRegistration.ServerRegistrationManager.UninstallServer(handler, type);

            if (uninstalled)
            {
                MessageBox.Show("GameCube Banner Thumbnail Handler has been uninstalled.", "Uninstall Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("GameCube Banner Thumbnail Handler was not uninstalled. The COM server could not be found.", "Uninstall Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
