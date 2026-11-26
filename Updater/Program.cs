using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Windows.Forms;
using Application = System.Windows.Forms.Application;

namespace IndustrialPark.Updater;

/// <summary>Initializes the updater and reports command failures.</summary>
public static class Program
{
    /// <summary>Resolves dependencies staged in the application's Library folder.</summary>
    [ModuleInitializer]
    internal static void InitializeAssemblyResolver()
    {
        AssemblyLoadContext.Default.Resolving += (context, name) =>
        {
            string path = Path.Combine(AppContext.BaseDirectory, "Library", name.Name + ".dll");
            return File.Exists(path) ? context.LoadFromAssemblyPath(path) : null;
        };
    }

    /// <summary>Runs an updater command and returns its exit code.</summary>
    [STAThread]
    static int Main(string[] args)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        bool installed = false;

        try
        {
            return UpdaterCommands.Run(args, ref installed);
        }
        catch (Exception ex)
        {
            if (args.Length > 0 && args[0] == "--test-check")
                Console.Error.WriteLine(ex);
            else
                MessageBox.Show("There was an error updating the application: " + ex.Message, "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return 3;
        }
        finally
        {
            UpdateWorker.CleanupAfterInstall(args, installed);
        }
    }
}
