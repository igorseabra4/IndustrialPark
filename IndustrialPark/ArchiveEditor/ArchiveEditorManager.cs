using Newtonsoft.Json;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace IndustrialPark.Archive;

public class ArchiveEditorManager
{
    public readonly List<ArchiveEditor> ArchiveEditors;

    public ArchiveEditorManager()
    {
        ArchiveEditors = new List<ArchiveEditor>();
    }

    public ArchiveEditor GetTargetArchiveEditor() => ArchiveEditors.FirstOrDefault(ae => ae.Targeted);

    public void ShowArchiveEditor(ArchiveEditor ae, MainForm mainForm)
    {
        if (ae.AssetListDocked)
            return;

        if (ae.WindowState == FormWindowState.Minimized)
            ae.WindowState = FormWindowState.Normal;

        if (!ae.Visible)
            ae.Show(mainForm);
        ae.Activate();
        ae.ActiveControl = null;
    }

    public ArchiveEditor ArchiveEditorContainingAsset(Asset asset) => ArchiveEditors.FirstOrDefault(ae => ae.archive?.ContainsAsset(asset) ?? false);

    public ArchiveEditor ArchiveEditorContainingAsset(uint asset) => ArchiveEditors.FirstOrDefault(ae => ae.archive?.ContainsAsset(asset) ?? false);

    public void SetSelectedAssets(List<uint> assetIDs, bool clearIfEmpty = true)
    {
        foreach (ArchiveEditor ae in ArchiveEditors)
            if (ae.archive != null)
                ae.SetSelectedAssets(assetIDs.Select(ae.archive.GetAsset), clearIfEmpty);
    }

    public void SetSelectedAssets(List<Asset> assets, bool clearIfEmpty = true)
    {
        foreach (ArchiveEditor ae in ArchiveEditors)
            if (ae.archive != null)
                ae.SetSelectedAssets(assets, clearIfEmpty);
    }

    public int GetNumberOfSelectedAssets()
    {
        return ArchiveEditors.Sum(ae => ae.SelectedAssets.Count());
    }

    public void CopySelectedAssets(bool hierarchy)
    {
        var assets = ArchiveEditors.SelectMany(ae => ae.SelectedAssets);

        if (hierarchy)
            assets = [.. AssetService.GetAssetsAndHierarchy(assets)];

        var clipboard = new List<AssetClipboard>(assets.Count());
        foreach (var asset in assets)
        {
            var ae = ArchiveEditorContainingAsset(asset);
            clipboard.AddRange(AssetService.AssetsToClipboard([asset], ae.archive.Platform, ae.archive.GetSoundData));
        }

        if (clipboard.Count != 0)
            Clipboard.SetText(JsonConvert.SerializeObject(clipboard, Formatting.None));
    }

    public void PasteAssets()
    {
        if (GetTargetArchiveEditor() is ArchiveEditor archiveEditor)
            archiveEditor.PasteAssets();
        else
            MessageBox.Show($"Nothing was pasted because no Archive Editor is selected as the target.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    public void OpenSelectedAssetArchiveEditor(MainForm mainForm)
    {
        foreach (ArchiveEditor ae in ArchiveEditors)
            if (ae.HasSelectedAssets)
                ShowArchiveEditor(ae, mainForm);
    }

    public void OpenHeaderEditor()
    {
        var ae = ArchiveEditors.FirstOrDefault(ae => ae.HasSelectedAssets);
        ae?.EditHeader(ae?.SelectedAssets.First());
    }

    public void DeleteSelectedAssets()
    {
        foreach (ArchiveEditor ae in ArchiveEditors)
            ae.DeleteSelectedAssets();
    }

    public void DeleteAssets(IEnumerable<Asset> assets)
    {
        var beganTransaction = new HashSet<ArchiveEditor>();
        foreach (var asset in assets)
        {
            var ae = ArchiveEditorContainingAsset(asset);
            ae.RemoveAssets([asset.assetID]);
            if (beganTransaction.Add(ae))
                ae.BeginTransaction();
        }
        foreach (var ae in beganTransaction)
            ae.EndTransaction();
    }

    public void DropSelectedAssets()
    {
        Program.Renderer.Assets.DropSelectedAssets(Program.Renderer, ArchiveEditors.SelectMany(ae => ae.SelectedAssets).ToList());
    }

    public void HideSelectedAssets()
    {
        foreach (var ae in ArchiveEditors)
            ae.HideSelectedAssets();
    }

    public bool UnhideAllAssets()
    {
        bool unhid = false;
        foreach (var ae in ArchiveEditors)
            unhid |= ae.UnhideAllAssets();
        return unhid;
    }

    public void ClearTemplateFocus()
    {
        foreach (ArchiveEditor archiveEditor in ArchiveEditors)
            archiveEditor.SetTargetedOff();
    }

    public void ResetMovementPreview()
    {
        foreach (ArchiveEditor ae in ArchiveEditors)
            if (ae.archive != null)
                foreach (EntityAsset p in ae.archive.Assets.OfType<EntityAsset>())
                    p.Reset();
    }

    public bool PlaceTemplatesAt(AssetTemplate template, List<Vector3> positions)
    {
        if (template == AssetTemplate.Null || positions == null || positions.Count == 0)
            return false;
        if (GetTargetArchiveEditor() is not ArchiveEditor ae)
        {
            MessageBox.Show($"The template could not be placed because no Archive Editor is selected as the target.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        var selected = new List<Asset>();
        foreach (Vector3 position in positions)
        {
            var placed = ae.PlaceTemplate(template, position);
            if (placed == null || placed.Count == 0)
                return false;
            selected.AddRange(placed);
        }
        if (selected.Count != 0)
            SetSelectedAssets(selected);
        return true;
    }
}
