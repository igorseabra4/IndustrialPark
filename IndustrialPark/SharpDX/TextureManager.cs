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

        public HashSet<string> OpenTextureFolders { get; private set; } = new HashSet<string>();

        public bool HasTexture(string textureName)
        {
            return Textures.ContainsKey(textureName);
        }

        public ShaderResourceView GetTextureFromDictionary(Material_0007 material)
        {
            if (material != null && material.texture != null && Textures.ContainsKey(material.texture.DiffuseTextureName))
                return Textures[material.texture.DiffuseTextureName];
            return Program.Renderer.whiteDefault;
        }

        public ShaderResourceView GetTextureFromDictionary(uint assetID)
        {
            foreach (string s in Textures.Keys)
                if (Functions.BKDRHash(s) == assetID)
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

            DisposeTexture(tnStruct.textureName);
            Textures[tnStruct.textureName] = texture;
        }

        public void RemoveTexture(string textureName, Archive.Archive sender, uint assetID)
        {
            foreach (var ae in Program.ArchiveEditors)
                if (ae.archive != null && ae.archive != sender && ae.archive.ContainsAsset(assetID))
                    return;
            
            DisposeTexture(textureName);
            Textures.Remove(textureName);
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
            DisposeTexture(textureName);
            Textures[textureName] = Program.Renderer.device.LoadTextureFromFile(path);
        }

        public void ReapplyTextures()
        {
            foreach (SharpMesh mesh in RenderWareModelFile.completeMeshList)
                if (mesh != null)
                    foreach (SharpSubSet sub in mesh.SubSets)
                    {
                        if (Textures.TryGetValue(sub.DiffuseMapName, out ShaderResourceView texture))
                        {
                            if (sub.DiffuseMap != texture)
                            {
                                DisposeIfNotWhiteDefault(sub.DiffuseMap);
                                sub.DiffuseMap = texture;
                            }
                        }
                        else
                        {
                            DisposeIfNotWhiteDefault(sub.DiffuseMap);
                            sub.DiffuseMap = Program.Renderer.whiteDefault;
                        }
                    }
        }

        private void DisposeIfNotWhiteDefault(ShaderResourceView texture)
        {
            if (texture != null && !texture.IsDisposed && texture != Program.Renderer.whiteDefault)
                texture.Dispose();
        }

        public void SetTextureForAnimation(string diffuseMapName, string newMapName)
        {
            foreach (SharpMesh mesh in RenderWareModelFile.completeMeshList)
                foreach (SharpSubSet sub in mesh.SubSets)
                {
                    if (sub.DiffuseMapName == diffuseMapName)
                        sub.DiffuseMap = Textures[newMapName];
                }
        }

        public void DisposeTexture(string textureName)
        {
            if (Textures.TryGetValue(textureName, out ShaderResourceView value) && value != null && !value.IsDisposed)
                value.Dispose();
        }

        public void DisposeTextures()
        {
            foreach (ShaderResourceView texture in Textures.Values)
                texture?.Dispose();
        }

        public void ClearTextures()
        {
            OpenTextureFolders.Clear();
            DisposeTextures();
            Textures.Clear();
        }

        public HashSet<string> SelectedTextures = new HashSet<string>();

        public bool IsSelected(string texture) => SelectedTextures.Contains(texture);
    }
}