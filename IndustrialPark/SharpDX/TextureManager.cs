using HipHopFile;
using IndustrialPark.Archives;
using RenderWareFile.Sections;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;

namespace IndustrialPark;

public class TextureManager
{
    private Dictionary<string, ShaderResourceView> Textures = new Dictionary<string, ShaderResourceView>();
    private readonly HashSet<ShaderResourceView> retiredTextures = new(ReferenceEqualityComparer.Instance);
    private readonly Dictionary<ShaderResourceView, ShaderResourceView> pendingReplacements = new(ReferenceEqualityComparer.Instance);
    private readonly HashSet<string> pendingNames = new();
    private int updateDepth;

    public ShaderResourceView GetTextureFromDictionary(Material_0007 material)
    {
        if (material != null && material.texture != null && Textures.TryGetValue(material.texture.DiffuseTextureName, out ShaderResourceView texture) && !texture.IsDisposed)
            return texture;
        return Program.Renderer.whiteDefault;
    }

    public ShaderResourceView GetTextureFromDictionary(uint assetID)
    {
        foreach (string s in Textures.Keys)
            if (Functions.BKDRHash(s) == assetID && Textures[s] != null && !Textures[s].IsDisposed)
                return Textures[s];
        return Program.Renderer.whiteDefault;
    }

    public void RemoveTexture(string textureName, Archive sender, uint assetID)
    {
        foreach (var ae in Program.ArchiveEditors)
            if (ae.archive != null && ae.archive != sender && ae.archive.ContainsAsset(assetID))
                return;

        DisposeTexture(textureName);
        ReapplyTextures();
    }

    public void AddTextureMipmaps(string name, TextureNativeStruct_0001 texture, int width, int height, byte[][] mipmaps)
    {
        var view = Program.Renderer.device.CreateTextureFromBgraMipmaps(width, height, mipmaps, texture.addressModeU, texture.addressModeV);
        ReplaceTexture(name, view);
    }

    public void BeginTextureUpdates() => updateDepth++;

    public void EndTextureUpdates()
    {
        if (updateDepth == 0)
            throw new InvalidOperationException("No texture update batch is active.");
        if (--updateDepth == 0)
            ApplyPendingTextures();
    }

    public void ReapplyTextures()
    {
        foreach (SharpMesh mesh in RenderWareModelFile.completeMeshList)
            if (mesh?.SubSets != null)
                foreach (SharpSubSet sub in mesh.SubSets)
                {
                    if (sub == null)
                        continue;

                    sub.AnimatedTextureName = null;
                    if (Textures.TryGetValue(sub.DiffuseMapName, out ShaderResourceView texture) && texture != null && !texture.IsDisposed)
                    {
                        if (sub.DiffuseMap != texture)
                            sub.DiffuseMap = texture;
                    }
                    else
                        sub.DiffuseMap = Program.Renderer.whiteDefault;
                }

        DisposeRetiredTextures();
        pendingReplacements.Clear();
        pendingNames.Clear();
    }

    private void ReplaceTexture(string textureName, ShaderResourceView texture)
    {
        Textures.TryGetValue(textureName, out ShaderResourceView oldTexture);
        if (oldTexture != texture)
            RetireTexture(oldTexture);

        Textures[textureName] = texture;
        if (updateDepth != 0)
        {
            pendingNames.Add(textureName);
            if (oldTexture != null && oldTexture != texture)
                pendingReplacements[oldTexture] = texture;
            return;
        }

        foreach (SharpMesh mesh in RenderWareModelFile.completeMeshList)
            if (mesh?.SubSets != null)
                foreach (SharpSubSet sub in mesh.SubSets)
                    if (sub != null &&
                        ((oldTexture != null && sub.DiffuseMap == oldTexture) ||
                         ((sub.AnimatedTextureName ?? sub.DiffuseMapName) == textureName &&
                          sub.DiffuseMap == Program.Renderer.whiteDefault)))
                        sub.DiffuseMap = texture;
        DisposeRetiredTextures();
    }

    private void ApplyPendingTextures()
    {
        if (pendingNames.Count == 0)
            return;

        foreach (SharpMesh mesh in RenderWareModelFile.completeMeshList)
            if (mesh?.SubSets != null)
                foreach (SharpSubSet sub in mesh.SubSets)
                {
                    if (sub == null)
                        continue;
                    var texture = sub.DiffuseMap;
                    while (texture != null && pendingReplacements.TryGetValue(texture, out var replacement))
                        texture = replacement;
                    string name = sub.AnimatedTextureName ?? sub.DiffuseMapName;
                    if (texture == Program.Renderer.whiteDefault && name != null &&
                        pendingNames.Contains(name) && Textures.TryGetValue(name, out var added))
                        texture = added;
                    sub.DiffuseMap = texture;
                }

        DisposeRetiredTextures();
        pendingReplacements.Clear();
        pendingNames.Clear();
    }

    private void RetireTexture(ShaderResourceView texture)
    {
        if (texture != null && texture != Program.Renderer.whiteDefault && !texture.IsDisposed)
            retiredTextures.Add(texture);
    }

    private void DisposeRetiredTextures()
    {
        if (retiredTextures.Count == 0)
            return;
        var currentTextures = new HashSet<ShaderResourceView>(Textures.Values, ReferenceEqualityComparer.Instance);
        retiredTextures.RemoveWhere(texture =>
        {
            if (currentTextures.Contains(texture))
                return false;

            if (texture != null && !texture.IsDisposed)
                texture.Dispose();
            return true;
        });
    }

    public void SetTextureForAnimation(string diffuseMapName, string newMapName)
    {
        ShaderResourceView texture = Textures.TryGetValue(newMapName, out ShaderResourceView foundTexture) && !foundTexture.IsDisposed
            ? foundTexture
            : Program.Renderer.whiteDefault;

        foreach (SharpMesh mesh in RenderWareModelFile.completeMeshList)
            if (mesh?.SubSets != null)
                foreach (SharpSubSet sub in mesh.SubSets)
                {
                    if (sub != null && sub.DiffuseMapName == diffuseMapName)
                    {
                        sub.AnimatedTextureName = newMapName;
                        sub.DiffuseMap = texture;
                    }
                }
    }

    public void DisposeTexture(string textureName)
    {
        if (Textures.Remove(textureName, out ShaderResourceView texture))
            RetireTexture(texture);
    }

    public void DisposeTextures()
    {
        foreach (ShaderResourceView texture in Textures.Values)
            RetireTexture(texture);

        Textures.Clear();
        ReapplyTextures();
    }

    public void ClearTextures()
    {
        DisposeTextures();
    }

    public HashSet<string> SelectedTextures = new HashSet<string>();

    public bool IsSelected(string texture) => SelectedTextures.Contains(texture);
}