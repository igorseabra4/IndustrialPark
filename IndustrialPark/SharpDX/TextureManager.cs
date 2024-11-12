using HipHopFile;
using RenderWareFile;
using RenderWareFile.Sections;
using SharpDX.Direct3D11;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IndustrialPark
{
    public static class TextureManager
    {
        public static string TreatTextureName(string entry)
        {
            entry = Path.GetFileNameWithoutExtension(entry).Trim('_');
            entry = entry.Trim('_');
            return entry;
        }

        public const string DefaultTexture = "default";
        private static Dictionary<string, ShaderResourceView> Textures = new Dictionary<string, ShaderResourceView>();

        public static HashSet<string> OpenTextureFolders { get; private set; } = new HashSet<string>();

        public static bool HasTexture(string textureName)
        {
            return Textures.ContainsKey(textureName);
        }

        public static ShaderResourceView GetTextureFromDictionary(Material_0007 material)
        {
            if (material != null && material.texture != null && Textures.ContainsKey(material.texture.DiffuseTextureName))
                return Textures[material.texture.DiffuseTextureName];
            return SharpRenderer.whiteDefault;
        }

        public static ShaderResourceView GetTextureFromDictionary(uint assetID)
        {
            foreach (string s in Textures.Keys)
                if (Functions.BKDRHash(s) == assetID)
                    return Textures[s];
            return SharpRenderer.whiteDefault;
        }

        public static void LoadTexturesFromTXD(string textureFile, bool reapply = true)
        {
            foreach (RWSection rw in ReadFileMethods.ReadRenderWareFile(textureFile))
                if (rw is TextureDictionary_0016 td)
                    foreach (TextureNative_0015 tn in td.textureNativeList)
                        AddTextureNative(tn.textureNativeStruct);

            if (reapply)
                ReapplyTextures();
        }

        private static void AddTextureNative(TextureNativeStruct_0001 tnStruct)
        {
            ShaderResourceView texture;

            try
            { texture = Program.MainForm.renderer.device.LoadTextureFromRenderWareNative(tnStruct); }
            catch { return; }

            DisposeTexture(tnStruct.textureName);
            Textures[tnStruct.textureName] = texture;
        }

        public static void RemoveTexture(string textureName, ArchiveEditorFunctions sender, uint assetID)
        {
            foreach (var ae in Program.MainForm.archiveEditors)
                if (ae.archive != sender && ae.archive.ContainsAsset(assetID))
                    return;

            DisposeTexture(textureName);
            Textures.Remove(textureName);
            ReapplyTextures();
        }

        public static void LoadTexturesFromFolder(IEnumerable<string> folderNames)
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

        private static void AddTextureBitmap(string path)
        {
            string textureName = TreatTextureName(Path.GetFileNameWithoutExtension(path));
            DisposeTexture(textureName);
            Textures[textureName] = Program.MainForm.renderer.device.LoadTextureFromFile(path);
        }

        public static void ReapplyTextures()
        {
            foreach (SharpMesh mesh in RenderWareModelFile.completeMeshList)
                if (mesh != null)
                    foreach (SharpSubSet sub in mesh.SubSets)
                    {
                        if (Textures.ContainsKey(sub.DiffuseMapName))
                        {
                            if (sub.DiffuseMap != Textures[sub.DiffuseMapName])
                            {
                                DisposeIfNotWhiteDefault(sub.DiffuseMap);
                                sub.DiffuseMap = Textures[sub.DiffuseMapName];
                            }
                        }
                        else
                        {
                            DisposeIfNotWhiteDefault(sub.DiffuseMap);
                            sub.DiffuseMap = SharpRenderer.whiteDefault;
                        }
                    }
        }

        private static void DisposeIfNotWhiteDefault(ShaderResourceView texture)
        {
            if (texture != null && !texture.IsDisposed && texture != SharpRenderer.whiteDefault)
                texture.Dispose();
        }

        public static void SetTextureForAnimation(string diffuseMapName, string newMapName)
        {
            foreach (SharpMesh mesh in RenderWareModelFile.completeMeshList)
                foreach (SharpSubSet sub in mesh.SubSets)
                {
                    if (sub.DiffuseMapName == diffuseMapName)
                        sub.DiffuseMap = Textures[newMapName];
                }
        }

        public static void DisposeTexture(string textureName)
        {
            if (Textures.ContainsKey(textureName) && Textures[textureName] != null && !Textures[textureName].IsDisposed)
                Textures[textureName].Dispose();
        }

        public static void DisposeTextures()
        {
            foreach (ShaderResourceView texture in Textures.Values)
                if (texture != null)
                    texture.Dispose();
        }

        public static void ClearTextures()
        {
            OpenTextureFolders.Clear();
            DisposeTextures();
            Textures.Clear();
        }
    }
}