using IndustrialPark.Archive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ArchiveModel = IndustrialPark.Archive.Archive;

namespace IndustrialPark;

public partial class ArchiveComparerForm : Form
{
    private readonly ArchiveEditorManager archiveEditorManager;

    public ArchiveComparerForm(ArchiveEditorManager archiveEditorManager)
    {
        this.archiveEditorManager = archiveEditorManager ?? throw new ArgumentNullException(nameof(archiveEditorManager));
        InitializeComponent();
        PopulateArchiveSelectors();
    }

    private void PopulateArchiveSelectors()
    {
        var editors = archiveEditorManager.ArchiveEditors
            .Where(archiveEditor => archiveEditor.archive != null)
            .Select(archiveEditor => new ArchiveEditorItem(archiveEditor))
            .ToArray();

        comboBoxSource.Items.AddRange(editors);
        comboBoxDestination.Items.AddRange(editors.Select(item => new ArchiveEditorItem(item.Editor)).ToArray());

        if (comboBoxSource.Items.Count > 0)
            comboBoxSource.SelectedIndex = 0;
        if (comboBoxDestination.Items.Count > 1)
            comboBoxDestination.SelectedIndex = 1;
        else if (comboBoxDestination.Items.Count > 0)
            comboBoxDestination.SelectedIndex = 0;

        buttonGenerate.Enabled = comboBoxSource.Items.Count > 1;
        buttonSaveDelta.Enabled = comboBoxSource.Items.Count > 1;
    }

    private void buttonGenerate_Click(object sender, EventArgs e)
    {
        if (!TryGetSelectedArchives(out var source, out var destination))
            return;

        var comparison = new ArchiveComparer(source, destination).Compare();
        textBoxReport.Text = BuildReport(comparison);
        buttonSaveDelta.Enabled = true;
    }

    private void buttonSaveDelta_Click(object sender, EventArgs e)
    {
        if (!TryGetSelectedArchives(out var source, out var destination))
            return;

        using var saveFileDialog = new SaveFileDialog
        {
            AddExtension = true,
            DefaultExt = Path.GetExtension(destination.CurrentFilePath).Trim('.'),
            Filter = "HIP/HOP files|*.hip;*.hop|All files|*.*",
            FileName = $"{destination.LevelName}_delta"
        };

        if (saveFileDialog.ShowDialog(this) != DialogResult.OK)
            return;

        try
        {
            var deltaArchive = new ArchiveComparer(source, destination).CreateDeltaArchive();
            try
            {
                deltaArchive.Save(saveFileDialog.FileName);
            }
            finally
            {
                deltaArchive.Dispose();
            }
            System.Windows.Forms.MessageBox.Show(this, "The delta archive was saved successfully.", "Archive Comparer", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show(this, $"The delta archive could not be saved: {ex.Message}", "Archive Comparer", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private bool TryGetSelectedArchives(out ArchiveModel source, out ArchiveModel destination)
    {
        source = (comboBoxSource.SelectedItem as ArchiveEditorItem)?.Editor.archive;
        destination = (comboBoxDestination.SelectedItem as ArchiveEditorItem)?.Editor.archive;

        if (source == null || destination == null)
        {
            System.Windows.Forms.MessageBox.Show(this, "Select both a source archive and a destination archive.", "Archive Comparer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        if (ReferenceEquals(source, destination))
        {
            System.Windows.Forms.MessageBox.Show(this, "The source and destination archives must be different.", "Archive Comparer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        return true;
    }

    private static string BuildReport(ArchiveComparisonResult comparison)
    {
        var report = new StringBuilder();
        AppendAssetSection(report, "Assets present in source but not destination", comparison.AssetsOnlyInSource,
            entry => FormatAsset(entry.SourceAsset));
        AppendAssetSection(report, "Assets present in destination but not source", comparison.AssetsOnlyInDestination,
            entry => FormatAsset(entry.DestinationAsset));
        AppendAssetSection(report, "Assets present in both with modified data", comparison.ModifiedAssets,
            entry => FormatAsset(entry.SourceAsset));
        AppendAssetSection(report, "Assets present in both with identical data", comparison.IdenticalAssets,
            entry => FormatAsset(entry.SourceAsset));
        return report.ToString();
    }

    private static void AppendAssetSection(
        StringBuilder report,
        string title,
        IEnumerable<ArchiveAssetComparison> assets,
        Func<ArchiveAssetComparison, string> format)
    {
        var entries = assets.ToArray();
        report.AppendLine($"{title} ({entries.Length})");
        report.AppendLine(new string('-', title.Length + entries.Length.ToString().Length + 3));
        foreach (var entry in entries)
            report.AppendLine(format(entry));
        report.AppendLine();
    }

    private static string FormatAsset(Asset asset) => $"{asset.assetID:X8} [{asset.assetType}] {asset.assetName}";

    private sealed class ArchiveEditorItem
    {
        public ArchiveEditor Editor { get; }

        public ArchiveEditorItem(ArchiveEditor editor) => Editor = editor;

        public override string ToString() => $"[{Editor.archive.LevelName}] { Editor.archive.CurrentFilePath ?? "" }";
    }
}
