using HipHopFile;
using RenderWareFile;
using RenderWareFile.Sections;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IndustrialPark
{
    public class TextureManager
    {
        public string TreatTextureName(string entry)
        {
            entry = Path.GetFileNameWithoutExtension(entry).Trim('_');
            entry = entry.Trim('_');
            return entry;
        }

        public const string DefaultTexture = "default";
        private Dictionary<string, ShaderResourceView> Textures = new Dictionary<string, ShaderResourceView>();
        private readonly List<ShaderResourceView> retiredTextures = new();

        public HashSet<string> OpenTextureFolders { get; private set; } = new HashSet<string>();

        public bool HasTexture(string textureName)
        {
            return Textures.TryGetValue(textureName, out ShaderResourceView texture) && texture != null && !texture.IsDisposed;
        }

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

        public void LoadTexturesFromTXD(string textureFile, bool reapply = true, IProgressBar progressBar = null)
        {
            foreach (RWSection rw in ReadFileMethods.ReadRenderWareFile(textureFile))
                if (rw is TextureDictionary_0016 td)
                    foreach (TextureNative_0015 tn in td.textureNativeList)
                    {
                        progressBar?.PerformStep();
                        AddTextureNative(tn.textureNativeStruct);
                    }

            if (reapply)
                ReapplyTextures();
        }

        private void AddTextureNative(TextureNativeStruct_0001 tnStruct)
        {
            ShaderResourceView texture;

            try
            { texture = Program.Renderer.device.LoadTextureFromRenderWareNative(tnStruct); }
            catch { return; }

            ReplaceTexture(tnStruct.textureName, texture);
        }

        public void RemoveTexture(string textureName, Archive.Archive sender, uint assetID)
        {
            foreach (var ae in Program.ArchiveEditors)
                if (ae.archive != null && ae.archive != sender && ae.archive.ContainsAsset(assetID))
                    return;
            
            DisposeTexture(textureName);
            ReapplyTextures();
        }

        public void LoadTexturesFromFolder(IEnumerable<string> folderNames)
        {
            string[] extensions = new string[] { ".png", ".bmp", ".gif", ".jpg", ".jpeg", ".jpe", ".jif", ".jfif", ".jfi", ".tif", ".tiff" };

            foreach (string folderName in folderNames)
                if (Directory.Exists(folderName))
                {
                    OpenTextureFolders.Add(folderName);
                    foreach (string i in Directory.GetFiles(folderName))
                        if (extensions.Contains(Path.GetExtension(i).ToLower()))
                            AddTextureBitmap(i);
                }
                else
                    MessageBox.Show("Error loading textures from " + folderName + ": folder not found");

            ReapplyTextures();
        }

        private void AddTextureBitmap(string path)
        {
            string textureName = TreatTextureName(Path.GetFileNameWithoutExtension(path));
            ReplaceTexture(textureName, Program.Renderer.device.LoadTextureFromFile(path));
        }

        public void ReapplyTextures()
        {
            foreach (SharpMesh mesh in RenderWareModelFile.completeMeshList)
                if (mesh?.SubSets != null)
                    foreach (SharpSubSet sub in mesh.SubSets)
                    {
                        if (sub == null)
                            continue;

                        if (Textures.TryGetValue(sub.DiffuseMapName, out ShaderResourceView texture) && texture != null && !texture.IsDisposed)
                        {
                            if (sub.DiffuseMap != texture)
                                sub.DiffuseMap = texture;
                        }
                        else
                            sub.DiffuseMap = Program.Renderer.whiteDefault;
                    }

            DisposeRetiredTextures();
        }

        private void ReplaceTexture(string textureName, ShaderResourceView texture)
        {
            if (Textures.TryGetValue(textureName, out ShaderResourceView oldTexture) && oldTexture != texture)
                RetireTexture(oldTexture);

            Textures[textureName] = texture;
        }

        private void RetireTexture(ShaderResourceView texture)
        {
            if (texture != null && texture != Program.Renderer.whiteDefault && !texture.IsDisposed && !retiredTextures.Any(retired => ReferenceEquals(retired, texture)))
                retiredTextures.Add(texture);
        }

        private void DisposeRetiredTextures()
        {
            for (int i = retiredTextures.Count - 1; i >= 0; i--)
            {
                ShaderResourceView texture = retiredTextures[i];
                if (Textures.Values.Any(current => ReferenceEquals(current, texture)))
                    continue;

                if (texture != null && !texture.IsDisposed)
                    texture.Dispose();
                retiredTextures.RemoveAt(i);
            }
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
                            sub.DiffuseMap = texture;
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
            OpenTextureFolders.Clear();
            DisposeTextures();
        }

        public HashSet<string> SelectedTextures = new HashSet<string>();

        public bool IsSelected(string texture) => SelectedTextures.Contains(texture);
    }
}