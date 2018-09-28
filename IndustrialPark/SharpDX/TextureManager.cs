using System.Collections.Generic;
using System.IO;
using SharpDX.Direct3D11;
using RenderWareFile;
using RenderWareFile.Sections;
using System;

namespace IndustrialPark
{
    public static class TextureManager
    {
        public static string TreatTextureName(string entry)
        {
            entry = (Path.GetFileNameWithoutExtension(entry).Trim('_'));
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

        public static ShaderResourceView GetTextureFromDictionary(string textureName)
        {
            if (Textures.ContainsKey(textureName))
                return Textures[textureName];
            return SharpRenderer.whiteDefault;
        }

        public static void LoadTexturesFromTXD(byte[] txdData, string textureName = null)
        {
            RWSection[] file = ReadFileMethods.ReadRenderWareFile(txdData);
            LoadTexturesFromTXD(file, textureName);
        }

        public static void LoadTexturesFromTXD(RWSection[] txdFile, string textureName = null)
        {
            foreach (RWSection rw in txdFile)
                if (rw is TextureDictionary_0016 td)
                    foreach (TextureNative_0015 tn in td.textureNativeList)
                        AddTextureNative(tn.textureNativeStruct, textureName);

            ReapplyTextures();
        }

        private static void AddTextureNative(TextureNativeStruct_0001 tnStruct, string textureName = null)
        {
            if (textureName == null)
                textureName = tnStruct.textureName;

            ShaderResourceView texture;

            try { texture = Program.MainForm.renderer.device.LoadTextureFromRenderWareNative(tnStruct); }
            catch { return; }

            if (Textures.ContainsKey(textureName))
            {
                if (Textures[textureName] != null)
                    if (!Textures[textureName].IsDisposed)
                        Textures[textureName].Dispose();

                Textures[textureName] = texture;
            }
            else
                Textures.Add(textureName, texture);
        }

        public static void RemoveTexture(string textureName)
        {
            if (Textures.ContainsKey(textureName))
            {
                if (Textures[textureName] != null)
                    if (!Textures[textureName].IsDisposed)
                        Textures[textureName].Dispose();

                Textures.Remove(textureName);
            }
        }

        public static void LoadTexturesFromFolder(string folderName)
        {
            OpenTextureFolders.Add(folderName);

            foreach (string i in Directory.GetFiles(folderName))
                if (Path.GetExtension(i).ToLower().Equals(".png"))
                    AddTexturePNG(i);

            ReapplyTextures();
        }

        private static void AddTexturePNG(string path)
        {
            string textureName = TreatTextureName(Path.GetFileNameWithoutExtension(path));

            if (Textures.ContainsKey(textureName))
            {
                if (Textures[textureName] != null)
                    if (!Textures[textureName].IsDisposed)
                        Textures[textureName].Dispose();

                Textures[textureName] = Program.MainForm.renderer.device.LoadTextureFromFile(path);
            }
            else
                Textures.Add(textureName, Program.MainForm.renderer.device.LoadTextureFromFile(path));
        }

        public static void ReapplyTextures()
        {
            List<RenderWareModelFile> models = new List<RenderWareModelFile>();
            foreach (IAssetWithModel awm in ArchiveEditorFunctions.renderingDictionary.Values)
                try
                {
                    models.Add(awm.GetRenderWareModelFile());
                }
                catch { }
            foreach (IRenderableAsset awm in ArchiveEditorFunctions.renderableAssetSetJSP)
                if (awm is AssetJSP alm)
                    try
                    {
                        models.Add(alm.model);
                    }
                    catch { }

            foreach (RenderWareModelFile m in models)
                foreach (SharpMesh mesh in m.meshList)
                    foreach (SharpSubSet sub in mesh.SubSets)
                    {
                        if (Textures.ContainsKey(sub.DiffuseMapName))
                        {
                            if (sub.DiffuseMap != Textures[sub.DiffuseMapName])
                            {
                                if (sub.DiffuseMap != null)
                                    if (!sub.DiffuseMap.IsDisposed)
                                        if (sub.DiffuseMap != SharpRenderer.whiteDefault)
                                            sub.DiffuseMap.Dispose();

                                sub.DiffuseMap = Textures[sub.DiffuseMapName];
                            }
                        }
                        else
                        {
                            if (sub.DiffuseMap != null)
                                if (!sub.DiffuseMap.IsDisposed)
                                    if (sub.DiffuseMap != SharpRenderer.whiteDefault)
                                        sub.DiffuseMap.Dispose();

                            sub.DiffuseMap = SharpRenderer.whiteDefault;
                        }
                    }
        }

        public static void SetTextureForAnimation(string diffuseMapName, string newMapName)
        {
            List<RenderWareModelFile> models = new List<RenderWareModelFile>();
            foreach (IAssetWithModel awm in ArchiveEditorFunctions.renderingDictionary.Values)
                models.Add(awm.GetRenderWareModelFile());

            foreach (RenderWareModelFile m in models)
                foreach (SharpMesh mesh in m.meshList)
                    foreach (SharpSubSet sub in mesh.SubSets)
                    {
                        if (sub.DiffuseMapName == diffuseMapName)
                            sub.DiffuseMap = Textures[newMapName];
                    }
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
            ReapplyTextures();
        }
    }
}
