using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Windows.Forms;
using Application = System.Windows.Forms.Application;

namespace IndustrialPark.Updater;

/// <summary>Routes updater commands and handles release checks.</summary>
internal static class UpdaterCommands
{
    /// <summary>Dispatches worker commands or checks for a release.</summary>
    internal static int Run(string[] args, ref bool installed)
    {
        if (args.Length == 3 && args[0] == "--cleanup")
        {
            UpdateWorker.Cleanup(args[1], args[2]);
            return 0;
        }

        if (args.Length == 6 && args[0] == "--install")
        {
            UpdateWorker.Install(args[1], args[2], args[3], args[4], args[5], ref installed);
            return 0;
        }

        if (args.Length == 4 && args[0] == "--apply")
        {
            UpdateWorker.StartInstaller(Application.StartupPath, args[1], args[2], int.Parse(args[3]));
            return 0;
        }

        return CheckForUpdate(args);
    }

    /// <summary>Checks, confirms and hands off a release for the requested mode.</summary>
    private static int CheckForUpdate(string[] args)
    {
        string appDir = Application.StartupPath;
        string industrialPark = Path.Combine(appDir, "IndustrialPark.exe");
        if (!File.Exists(industrialPark))
            throw new FileNotFoundException("IndustrialPark.exe not found.", industrialPark);

        bool testCheck = args.Length == 3 && args[0] == "--test-check";
        if (args.Length != 0 && !(args.Length == 2 && args[0] == "--check") && !testCheck)
            throw new ArgumentException("Invalid updater arguments.");

        Uri testApi = testCheck ? GetTestApi(args[2]) : null;
        var service = new UpdateService(new GitHubReleaseProvider(apiUrl: testApi));
        string currentVersion = FileVersionInfo.GetVersionInfo(industrialPark).ProductVersion;
        ReleaseInfo releaseInfo = service.GetAvailableRelease(currentVersion, args.Length == 0, out int comparison);
        if (releaseInfo == null)
            return 1;
        if (testCheck)
            ValidateTestAsset(releaseInfo.DownloadUrl, testApi);
        if (!testCheck && !ConfirmUpdate(releaseInfo, currentVersion, comparison))
            return 2;

        if (args.Length != 0)
        {
            File.WriteAllText(args[1], JsonSerializer.Serialize(releaseInfo));
            return 0;
        }
        string releasePath = Path.Combine(Path.GetTempPath(), $"IndustrialPark_release_{Guid.NewGuid():N}.json");
        File.WriteAllText(releasePath, JsonSerializer.Serialize(releaseInfo));
        UpdateWorker.StartInstaller(appDir, industrialPark, releasePath, Environment.ProcessId);
        return 0;
    }

    /// <summary>Requires the test release API to be a loopback HTTP origin.</summary>
    private static Uri GetTestApi(string value)
    {
        if (!Uri.TryCreate(value, UriKind.Absolute, out Uri testApi) ||
            testApi.Scheme != Uri.UriSchemeHttp ||
            !IPAddress.TryParse(testApi.Host, out IPAddress address) || !IPAddress.IsLoopback(address) ||
            testApi.AbsolutePath != "/" || !string.IsNullOrEmpty(testApi.Query) ||
            !string.IsNullOrEmpty(testApi.Fragment) || !string.IsNullOrEmpty(testApi.UserInfo))
            throw new ArgumentException("The test release API must be a loopback HTTP origin.");
        return testApi;
    }

    /// <summary>Requires a test release asset to come from the test API origin.</summary>
    private static void ValidateTestAsset(string url, Uri testApi)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri downloadUrl) ||
            downloadUrl.Scheme != testApi.Scheme || downloadUrl.Host != testApi.Host || downloadUrl.Port != testApi.Port)
            throw new InvalidDataException("The test release asset must be served by the loopback test API.");
    }

    /// <summary>Prompts before installing the selected release.</summary>
    private static bool ConfirmUpdate(ReleaseInfo release, string currentVersion, int comparison)
    {
        string body = release.Body ?? string.Empty;
        string[] lines = body.Split('\n');
        if (lines.Length > 20)
            body = string.Join("\n", lines, 0, 20) + "\n... (truncated)";
        string introduction = comparison switch
        {
            < 0 => $"The latest release ({release.TagName}) is older than your installed version (v{currentVersion}).",
            0 => $"The latest release ({release.TagName}) has the same version as your installed version (v{currentVersion}).",
            _ => $"There is an update available: Industrial Park ({release.Name})."
        };
        string message = $"{introduction}\n\n{body}\n\nDo you wish to download and install it?";
        string title = comparison > 0 ? "Update Available" : "Latest Release";
        return MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes;
    }
}
