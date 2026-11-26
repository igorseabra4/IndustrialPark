using HipHopFile;
using IndustrialPark.ArchiveEditors;
using IndustrialPark.Archives;
using IndustrialPark.Assets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace IndustrialPark;

/// <summary>
/// Opens every HIP/HOP file found in a set of paths and produces a text report with load statistics and asset errors.
/// </summary>
public static class ArchiveSoak
{
    private static readonly string[] Extensions = [".hip", ".hop"];

    private class SoakAssetError
    {
        public AssetError Error;
        public AssetType AssetType;
    }

    private class SoakFileResult
    {
        public string Path;
        public long FileSize;
        public Game Game = Game.Unknown;
        public Platform Platform = Platform.Unknown;
        public bool Opened;
        public string Exception;
        public int AssetCount;
        public int LayerCount;
        public TimeSpan LoadTime;
        public Dictionary<AssetType, int> AssetsPerType = [];
        public List<SoakAssetError> Errors = [];
    }

    /// <summary>
    /// Runs the soak over the given paths.
    /// </summary>
    /// <param name="paths">Files or directories (searched recursively) containing HIP/HOP files</param>
    /// <returns>The text report</returns>
    public static string Run(bool showProgressBar = false, params IEnumerable<string> paths)
    {
        var totalStopwatch = Stopwatch.StartNew();
        var startTime = DateTime.Now;

        var inputs = paths?.ToList() ?? [];
        var files = CollectFiles(inputs, out var invalidInputs);
        var results = new List<SoakFileResult>(files.Count);

        ProgressBar progressBar = null;
        if (showProgressBar)
        {
            progressBar = new ProgressBar("Soaking archives...");
            progressBar.SetProgressBar(0, files.Count, 1);
            progressBar.Show();
        }

        Platform knownPlatform = Platform.Unknown;

        foreach (var fileName in files)
        {
            var result = new SoakFileResult { Path = fileName };
            results.Add(result);
            progressBar?.PerformStep($"Opening {Path.GetFileName(fileName)}");

            Archive archive = null;
            var stopwatch = Stopwatch.StartNew();
            try
            {
                result.FileSize = new FileInfo(fileName).Length;

                HipFile hipFile = HipFile.FromPath(fileName);

                while (hipFile.game == Game.Unknown)
                    hipFile.game = ChooseGame.GetGame();

                if (hipFile.platform == Platform.Unknown && knownPlatform != Platform.Unknown)
                    hipFile.platform = knownPlatform;
                while (hipFile.platform == Platform.Unknown)
                    hipFile.platform = knownPlatform = ChoosePlatformDialog.GetPlatform();

                result.Game = hipFile.game;
                result.Platform = hipFile.platform;

                var assetTypes = new Dictionary<uint, AssetType>();
                foreach (var ahdr in hipFile.DICT.ATOC.AHDRList)
                    assetTypes[ahdr.assetID] = ahdr.assetType;

                archive = new Archive(hipFile, fileName, false, out List<AssetError> errors);
                stopwatch.Stop();

                result.Opened = true;
                result.LayerCount = archive.LayerCount;
                foreach (var asset in archive.Assets)
                {
                    result.AssetCount++;
                    result.AssetsPerType[asset.assetType] = result.AssetsPerType.GetValueOrDefault(asset.assetType) + 1;
                }

                foreach (var error in errors)
                    result.Errors.Add(new SoakAssetError
                    {
                        Error = error,
                        AssetType = archive.TryGetAsset(error.assetID, out Asset asset) ? asset.assetType :
                            assetTypes.GetValueOrDefault(error.assetID, AssetType.Null)
                    });
            }
            catch (Exception ex)
            {
                result.Opened = false;
                result.Exception = $"{ex.GetType().Name}: {ex.Message}";
            }
            finally
            {
                stopwatch.Stop();
                result.LoadTime = stopwatch.Elapsed;
                try
                {
                    archive?.Dispose();
                }
                catch (Exception ex)
                {
                    result.Exception ??= $"Error disposing archive: {ex.GetType().Name}: {ex.Message}";
                }
            }
        }

        progressBar?.Close();
        totalStopwatch.Stop();
        return BuildReport(inputs, invalidInputs, results, startTime, totalStopwatch.Elapsed);
    }

    private static List<string> CollectFiles(List<string> inputs, out List<string> invalidInputs)
    {
        invalidInputs = [];
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var files = new List<string>();

        var options = new EnumerationOptions
        {
            RecurseSubdirectories = true,
            IgnoreInaccessible = true,
            MatchCasing = MatchCasing.CaseInsensitive
        };

        void Add(string file)
        {
            var full = Path.GetFullPath(file);
            if (seen.Add(full))
                files.Add(full);
        }

        foreach (var input in inputs)
        {
            if (string.IsNullOrWhiteSpace(input))
                continue;

            try
            {
                if (File.Exists(input))
                {
                    if (IsHipHop(input))
                        Add(input);
                    else
                        invalidInputs.Add($"{input} (not a HIP/HOP file)");
                }
                else if (Directory.Exists(input))
                {
                    foreach (var file in Directory.EnumerateFiles(input, "*", options).Where(IsHipHop).OrderBy(f => f, StringComparer.OrdinalIgnoreCase))
                        Add(file);
                }
                else
                    invalidInputs.Add($"{input} (not found)");
            }
            catch (Exception ex)
            {
                invalidInputs.Add($"{input} ({ex.Message})");
            }
        }

        return files;
    }

    private static bool IsHipHop(string path) =>
        Extensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase);

    private static string TypeName(AssetType assetType) =>
        assetType == AssetType.Null ? "Unknown" : AssetTypeContainer.AssetTypeToString(assetType);

    private static string FormatError(SoakAssetError e) =>
        $"[{e.Error.reason}] [{e.Error.assetID:X8}] {e.Error.assetName} ({TypeName(e.AssetType)})" +
        (string.IsNullOrWhiteSpace(e.Error.ErrorMessage) ? "" : $": {e.Error.ErrorMessage}");

    private static string FormatSize(long bytes) =>
        bytes >= 1024 * 1024 ? $"{bytes / (1024.0 * 1024.0):0.00} MB" : $"{bytes / 1024.0:0.00} KB";

    private static string BuildReport(List<string> inputs, List<string> invalidInputs, List<SoakFileResult> results, DateTime startTime, TimeSpan totalTime)
    {
        var sb = new StringBuilder();
        var opened = results.Where(r => r.Opened).ToList();
        var failed = results.Where(r => !r.Opened).ToList();
        var withErrors = opened.Where(r => r.Errors.Count > 0).ToList();
        var allErrors = opened.SelectMany(r => r.Errors).ToList();

        sb.AppendLine("==================================================");
        sb.AppendLine("           INDUSTRIAL PARK SOAK REPORT");
        sb.AppendLine("==================================================");
        sb.AppendLine($"Started:        {startTime:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine($"Total duration: {totalTime:hh\\:mm\\:ss\\.fff}");
        sb.AppendLine($"Version:        {new IPversion().version}");
        sb.AppendLine();
        sb.AppendLine("Input paths:");
        foreach (var input in inputs)
            sb.AppendLine($"  {input}");
        if (invalidInputs.Count > 0)
        {
            sb.AppendLine("Skipped input paths:");
            foreach (var input in invalidInputs)
                sb.AppendLine($"  {input}");
        }
        sb.AppendLine();

        AppendSummary(sb, "SUMMARY", results);

        var hips = results.Where(r => Path.GetExtension(r.Path).Equals(".hip", StringComparison.OrdinalIgnoreCase)).ToList();
        var hops = results.Where(r => Path.GetExtension(r.Path).Equals(".hop", StringComparison.OrdinalIgnoreCase)).ToList();
        if (hips.Count > 0 && hops.Count > 0)
        {
            AppendSummary(sb, "SUMMARY (HIP ONLY)", hips);
            AppendSummary(sb, "SUMMARY (HOP ONLY)", hops);
        }

        Section(sb, "FILES PER GAME / PLATFORM");
        Table(sb, ["Game", "Platform", "Files", "Opened", "With Errors", "Failed", "Assets", "Errors"],
            results.GroupBy(r => (r.Game, r.Platform))
                .OrderBy(g => g.Key.Game).ThenBy(g => g.Key.Platform)
                .Select(g => new[]
                {
                    g.Key.Game.ToString(),
                    g.Key.Platform.ToString(),
                    g.Count().ToString(),
                    g.Count(r => r.Opened).ToString(),
                    g.Count(r => r.Opened && r.Errors.Count > 0).ToString(),
                    g.Count(r => !r.Opened).ToString(),
                    g.Sum(r => r.AssetCount).ToString(),
                    g.Sum(r => r.Errors.Count).ToString(),
                }));

        Section(sb, "ERRORS PER REASON");
        if (allErrors.Count == 0)
            sb.AppendLine("No asset errors.");
        else
            Table(sb, ["Reason", "Errors", "Files"],
                allErrors.GroupBy(e => e.Error.reason)
                    .OrderByDescending(g => g.Count())
                    .Select(g => new[]
                    {
                        g.Key.ToString(),
                        g.Count().ToString(),
                        opened.Count(r => r.Errors.Any(e => e.Error.reason == g.Key)).ToString()
                    }));

        Section(sb, "ERRORS PER ASSET TYPE");
        if (allErrors.Count == 0)
            sb.AppendLine("No asset errors.");
        else
        {
            var reasons = allErrors.Select(e => e.Error.reason).Distinct().OrderBy(r => r).ToList();
            Table(sb, ["Asset Type", "Errors", .. reasons.Select(r => r.ToString())],
                allErrors.GroupBy(e => e.AssetType)
                    .OrderByDescending(g => g.Count()).ThenBy(g => TypeName(g.Key))
                    .Select(g => (string[])[
                        TypeName(g.Key),
                        g.Count().ToString(),
                        .. reasons.Select(reason => g.Count(e => e.Error.reason == reason).ToString())
                    ]));
        }

        Section(sb, "ASSETS PER ASSET TYPE");
        var assetsPerType = new Dictionary<AssetType, int>();
        foreach (var r in opened)
            foreach (var (type, count) in r.AssetsPerType)
                assetsPerType[type] = assetsPerType.GetValueOrDefault(type) + count;
        var errorsPerType = allErrors.GroupBy(e => e.AssetType).ToDictionary(g => g.Key, g => g.Count());
        if (assetsPerType.Count == 0)
            sb.AppendLine("No assets.");
        else
            Table(sb, ["Asset Type", "Assets", "Files", "Errors", "Error Rate"],
                assetsPerType.OrderByDescending(kv => kv.Value).ThenBy(kv => TypeName(kv.Key))
                    .Select(kv =>
                    {
                        var errors = errorsPerType.GetValueOrDefault(kv.Key);
                        return new[]
                        {
                            TypeName(kv.Key),
                            kv.Value.ToString(),
                            opened.Count(r => r.AssetsPerType.ContainsKey(kv.Key)).ToString(),
                            errors.ToString(),
                            errors == 0 ? "-" : $"{100.0 * errors / kv.Value:0.##}%"
                        };
                    }));

        const int top = 10;

        Section(sb, $"FILES WITH MOST ERRORS (TOP {top})");
        if (withErrors.Count == 0)
            sb.AppendLine("No files with asset errors.");
        else
            Table(sb, ["Errors", "Assets", "File"],
                withErrors.OrderByDescending(r => r.Errors.Count).Take(top)
                    .Select(r => new[] { r.Errors.Count.ToString(), r.AssetCount.ToString(), r.Path }));

        Section(sb, $"SLOWEST FILES TO OPEN (TOP {top})");
        if (opened.Count == 0)
            sb.AppendLine("No files opened.");
        else
            Table(sb, ["Time", "Size", "Assets", "File"],
                opened.OrderByDescending(r => r.LoadTime).Take(top)
                    .Select(r => new[] { $"{r.LoadTime.TotalMilliseconds:0} ms", FormatSize(r.FileSize), r.AssetCount.ToString(), r.Path }));

        Section(sb, "FILES THAT FAILED TO OPEN");
        if (failed.Count == 0)
            sb.AppendLine("None.");
        else
            foreach (var r in failed)
            {
                sb.AppendLine(r.Path);
                sb.AppendLine($"    {r.Exception}");
            }
        sb.AppendLine();

        Section(sb, "DETAILS PER ARCHIVE");
        foreach (var r in results)
        {
            sb.AppendLine(r.Path);
            if (!r.Opened)
            {
                sb.AppendLine($"    FAILED TO OPEN: {r.Exception}");
            }
            else
            {
                sb.AppendLine($"    {r.Game} | {r.Platform} | {r.AssetCount} assets | {r.LayerCount} layers | {FormatSize(r.FileSize)} | {r.LoadTime.TotalMilliseconds:0} ms | {r.Errors.Count} error(s)");
                foreach (var e in r.Errors)
                    sb.AppendLine($"    {FormatError(e)}");
                if (r.Exception != null)
                    sb.AppendLine($"    {r.Exception}");
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private static void AppendSummary(StringBuilder sb, string title, List<SoakFileResult> results)
    {
        var opened = results.Where(r => r.Opened).ToList();
        var withErrors = opened.Count(r => r.Errors.Count > 0);
        var errorCount = opened.Sum(r => r.Errors.Count);
        var totalAssets = opened.Sum(r => r.AssetCount);

        Section(sb, title);
        Table(sb, null,
        [
            ["Files found", results.Count.ToString()],
            ["Files opened", opened.Count.ToString()],
            ["  Without asset errors", (opened.Count - withErrors).ToString()],
            ["  With asset errors", withErrors.ToString()],
            ["Files failed to open", (results.Count - opened.Count).ToString()],
            ["Total assets", totalAssets.ToString()],
            ["Total asset errors", errorCount.ToString()],
            ["Asset error rate", totalAssets == 0 ? "-" : $"{100.0 * errorCount / totalAssets:0.###}%"],
            ["Total layers", opened.Sum(r => r.LayerCount).ToString()],
            ["Average assets per file", opened.Count == 0 ? "-" : $"{(double)totalAssets / opened.Count:0.##}"],
            ["Total size of files", FormatSize(results.Sum(r => r.FileSize))],
            ["Total load time", $"{TimeSpan.FromTicks(results.Sum(r => r.LoadTime.Ticks)):hh\\:mm\\:ss\\.fff}"],
        ]);
    }

    private static void Section(StringBuilder sb, string title)
    {
        sb.AppendLine($"--- {title} " + new string('-', Math.Max(3, 46 - title.Length)));
    }

    private static void Table(StringBuilder sb, string[] header, IEnumerable<string[]> rows)
    {
        var allRows = rows.ToList();
        if (header != null)
            allRows.Insert(0, header);
        if (allRows.Count == 0)
        {
            sb.AppendLine();
            return;
        }

        var columns = allRows.Max(r => r.Length);
        var widths = new int[columns];
        foreach (var row in allRows)
            for (int i = 0; i < row.Length; i++)
                widths[i] = Math.Max(widths[i], row[i]?.Length ?? 0);

        for (int r = 0; r < allRows.Count; r++)
        {
            var row = allRows[r];
            var line = new StringBuilder();
            for (int i = 0; i < row.Length; i++)
            {
                var cell = row[i] ?? "";
                // left align the first column and the last one when it holds paths, right align numbers
                var leftAlign = i == 0 || (i == row.Length - 1 && header != null && header[i] == "File");
                line.Append(leftAlign ? cell.PadRight(widths[i]) : cell.PadLeft(widths[i]));
                if (i < row.Length - 1)
                    line.Append("  ");
            }
            sb.AppendLine(line.ToString().TrimEnd());

            if (r == 0 && header != null)
                sb.AppendLine(string.Join("  ", widths.Select(w => new string('-', w))));
        }
        sb.AppendLine();
    }
}
