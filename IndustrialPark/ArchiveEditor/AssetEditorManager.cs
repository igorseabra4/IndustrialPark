using HipHopFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace IndustrialPark.Archive;

public class AssetEditorManager
{
    private readonly List<InternalEditor> internalEditors;
    private readonly List<InternalMultiAssetEditor> multiInternalEditors;

    public bool AutoDockAssetEditors;
    public bool AutoDockSelectedAssetEditors;

    public AssetEditorManager()
    {
        internalEditors = new List<InternalEditor>();
        multiInternalEditors = new List<InternalMultiAssetEditor>();
    }

    public void OpenInternalEditor(uint assetID)
    {
        foreach (ArchiveEditor ae in Program.ArchiveManager.ArchiveEditors)
            if (ae.archive != null && ae.archive.TryGetAsset(assetID, out Asset asset))
                OpenAssetEditor(asset, Control.MousePosition);
    }

    public void OpenInternalEditors()
    {
        OpenAssetEditors(Program.ArchiveManager.ArchiveEditors.SelectMany(ae => ae.SelectedAssets), location: Control.MousePosition);
    }

    public void OpenAssetEditors(IEnumerable<Asset> assets, bool openAnyway = false, System.Drawing.Point? location = null, bool forceDock = false)
    {
        bool willOpen = true;
        if (assets.Count() > MainForm.MaxDockedEditors && !openAnyway)
            willOpen = MessageBox.Show($"Warning: you're going to open {assets.Count()} Asset Data Editor windows. Are you sure you want to do that?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;

        if (willOpen)
            foreach (var asset in assets)
                OpenAssetEditor(asset, assets.Count() == 1 ? location : null, forceDock);
    }

    public void OpenAssetEditor(Asset asset, System.Drawing.Point? location = null, bool forceDock = false)
    {
        if (internalEditors.FirstOrDefault(e => e.GetAsset().Equals(asset)) is InternalEditor existing)
        {
            Program.MainForm.SelectDockedTab(existing);
            existing.Activate();
            return;
        }

        CloseEditor(asset);

        var editor = CreateAssetEditor(asset, Program.ArchiveManager.ArchiveEditorContainingAsset(asset));
        editor.Text = asset.assetName;
        editor.Activated += (sender, e) =>
        {
            editor.Opacity = 1f;
            foreach (var child in editor.OwnedForms)
                child.SendToBack();
            editor.BringToFront();
        };
        editor.Deactivate += (sender, e) =>
        {
            if (editor.IsDisposed || editor.Disposing)
                return;
            if (Program.MainForm.TranslucentWhenOutOfFocus && editor.CanFocus && !editor.OwnedForms.Any())
                editor.Opacity = 0.5f;
        };
        editor.KeyPreview = true;
        editor.KeyDown += (sender, e) =>
        {
            if ((e.KeyCode == Keys.Y && e.Modifiers == Keys.Control) || (e.KeyCode == Keys.Z && e.Modifiers == (Keys.Control | Keys.Shift)))
                Program.UndoBuffer.Redo();
            else if (e.KeyCode == Keys.Z && e.Modifiers == Keys.Control)
                Program.UndoBuffer.Undo();
        };
        editor.FormClosing += (sender, e) => { if (!e.Cancel) Program.EditorManager.CloseEditor(editor); };
        if (location.HasValue)
        {
            editor.StartPosition = FormStartPosition.Manual;
            editor.Location = new System.Drawing.Point(Math.Max(0, location.Value.X - editor.Size.Width / 2), Math.Max(0, location.Value.Y - editor.Size.Height / 2));
        }
        editor.Owner = Program.MainForm;
        if (editor.CanBeDocked)
        {
            editor.FormDropped += Program.MainForm.editor_FormDropped;
            if (forceDock || (AutoDockAssetEditors && Program.MainForm.dockedEditorsCount < MainForm.MaxDockedEditors))
                Program.MainForm.DockAssetEditor(editor);
        }
        editor.Show();
        internalEditors.Add(editor);
    }

    private static InternalEditor CreateAssetEditor(Asset asset, ArchiveEditor ae)
    {
        switch (asset.assetType)
        {
            case AssetType.Model when ((AssetMODL)asset).IsCollisionModel:
                return new InternalCollModelEditor((AssetRenderWareModel)asset, ae);
            case AssetType.Model:
            case AssetType.BSP:
            case AssetType.JSP:
                return new InternalModelEditor((AssetRenderWareModel)asset, ae);
            case AssetType.Flythrough:
                return new InternalFlyEditor((AssetFLY)asset, ae);
            case AssetType.Texture:
            case AssetType.TextureStream:
                if (asset is AssetRWTX rwtx)
                    return new InternalTextureEditor(rwtx, ae);
                break;
            case AssetType.Sound:
            case AssetType.SoundStream:
                return new InternalSoundEditor((AssetSound)asset, ae);
            case AssetType.Text:
                return new InternalTextEditor((AssetTEXT)asset, ae);
        }
        return new InternalAssetEditor(asset, ae);
    }

    public void CloseEditor(InternalEditor editor)
    {
        Program.MainForm.CloseTab(editor);
        internalEditors.Remove(editor);
    }

    public void CloseEditor(Asset asset)
    {
        for (int i = 0; i < internalEditors.Count; i++)
            if (internalEditors[i].GetAsset() == asset)
                internalEditors[i].Close();
        for (int i = 0; i < multiInternalEditors.Count; i++)
            if (multiInternalEditors[i].AssetIDs.Contains(asset.assetID))
            {
                multiInternalEditors[i].Close();
                multiInternalEditors.RemoveAt(i--);
            }
    }

    public void OpenMultiAssetEditor(IEnumerable<Asset> list, System.Drawing.Point? location)
    {
        foreach (var asset in list)
            CloseEditor(asset);

        var editor = new InternalMultiAssetEditor([.. list]);
        if (location.HasValue)
        {
            editor.StartPosition = FormStartPosition.Manual;
            editor.Location = new System.Drawing.Point(Math.Max(0, location.Value.X - editor.Size.Width / 2), Math.Max(0, location.Value.Y - editor.Size.Height / 2));
        }
        multiInternalEditors.Add(editor);
        editor.Show(Program.MainForm);
    }

    public void SetAllOpacity(bool translucentEnabled)
    {
        foreach (var ie in internalEditors)
            ie.Opacity = (translucentEnabled && !ie.OwnedForms.Any() && !ie.Focused) ? 0.5f : 1.0f;
    }

    public void OpenAutoDockedEditors(IEnumerable<Asset> assets, ArchiveEditor archive)
    {
        if (!AutoDockSelectedAssetEditors || !assets.Any(InternalEditor.CanDockAssetEditor))
            return;

        Program.MainForm.ClearDockedAssetEditors(except: assets);
        OpenAssetEditors(assets, true, forceDock: true);
        foreach (var item in internalEditors.Where(ie => assets.Any(u => u.assetID == ie.GetAssetID())))
        {
            Program.MainForm.SelectDockedTab(item);
            item.Activate();
        }
        Program.MainForm.CloseDockPanelIfEmpty();
    }

    public void ClearModelTemplateFocus(InternalModelEditor except)
    {
        foreach (var ime in internalEditors.OfType<InternalModelEditor>().Where(ime => ime != except))
            ime.ClearModelTemplateFocus();
    }

    public void OpenWhoTargetsAssetEditors(uint assetID)
    {
        OpenAssetEditors(AssetService.WhoTargets(assetID));
    }
}
