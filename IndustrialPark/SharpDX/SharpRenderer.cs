using SharpDX;
using System.Windows.Forms;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using System.Collections.Generic;
using static IndustrialPark.OBJFunctions;
using System.IO;

namespace IndustrialPark
{
    public struct DefaultRenderData
    {
        public Matrix worldViewProjection;
        public Vector4 Color;
    }

    public class SharpRenderer
    {
        public static SharpDevice device;
        public static SharpCamera Camera = new SharpCamera();
        public static SharpFPS sharpFPS;

        public static float fovAngle;
        private static float aspectRatio;
        private static float near = 0.1f;
        public static float far;
        
        public SharpRenderer(Control control)
        {
            if (!SharpDevice.IsDirectX11Supported())
            {
                MessageBox.Show("DirectX11 Not Supported");
                return;
            }

            device = new SharpDevice(control);
            LoadModels();

            aspectRatio = (float)control.ClientSize.Width / control.ClientSize.Height;

            sharpFPS = new SharpFPS();
            sharpFPS.Reset();

            SetSharpShaders(device);
        }

        public static SharpShader basicShader;
        public static SharpDX.Direct3D11.Buffer basicBuffer;
        
        public static SharpShader defaultShader;
        public static SharpDX.Direct3D11.Buffer defaultBuffer;

        public static void SetSharpShaders(SharpDevice device)
        {
            basicShader = new SharpShader(device, "Resources/SharpDX/Shader_Basic.hlsl",
                new SharpShaderDescription() { VertexShaderFunction = "VS", PixelShaderFunction = "PS" },
                new InputElement[] {
                        new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0)
                });

            basicBuffer = basicShader.CreateBuffer<DefaultRenderData>();

            defaultShader = new SharpShader(device, "Resources/SharpDX/Shader_Default.hlsl",
                new SharpShaderDescription() { VertexShaderFunction = "VS", PixelShaderFunction = "PS" },
                new InputElement[] {
                        new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0),
                        new InputElement("COLOR", 0, Format.R8G8B8A8_UNorm, 12, 0),
                        new InputElement("TEXCOORD", 0, Format.R32G32_Float, 16, 0)
                });

            defaultBuffer = defaultShader.CreateBuffer<Matrix>();
        }

        private static bool showLevel = true;
        private static bool showObjects = true;
        
        public static void SetLevelModel(bool value)
        {
            showLevel = value;
        }

        public static void SetObjects(bool value)
        {
            showObjects = value;
        }
        
        public static SharpMesh Cube;
        public static SharpMesh Cylinder;
        public static SharpMesh Pyramid;
        
        public static void LoadModels()
        {
            for (int i = 0; i < 3; i++)// 3; i++)
            {
                ModelConverterData objData;

                if (i == 0) objData = ReadOBJFile("Resources/Models/Box.obj");
                else if (i == 1) objData = ReadOBJFile("Resources/Models/Cylinder.obj");
                else objData = ReadOBJFile("Resources/Models/Pyramid.obj");

                List<Vertex> vertexList = new List<Vertex>();
                foreach (OBJFunctions.Vertex v in objData.VertexStream)
                {
                    vertexList.Add(new Vertex(v.Position));
                }

                List<int> indexList = new List<int>();
                foreach (Triangle t in objData.TriangleStream)
                {
                    indexList.Add(t.Vertex1);
                    indexList.Add(t.Vertex2);
                    indexList.Add(t.Vertex3);
                }

                if (i == 0) Cube = SharpMesh.Create(device, vertexList.ToArray(), indexList.ToArray(), new List<SharpSubSet>() { new SharpSubSet(0, indexList.Count, null) });
                else if (i == 1) Cylinder = SharpMesh.Create(device, vertexList.ToArray(), indexList.ToArray(), new List<SharpSubSet>() { new SharpSubSet(0, indexList.Count, null) });
                else Pyramid = SharpMesh.Create(device, vertexList.ToArray(), indexList.ToArray(), new List<SharpSubSet>() { new SharpSubSet(0, indexList.Count, null) });
            }
        }
        
        public static void DrawCube(Matrix world)
        {
            DefaultRenderData renderData;

            renderData.worldViewProjection = world * viewProjection;
            renderData.Color = new Vector4(0.75f, 0.75f, 1f, 0.01f);
            //renderData.Color = new Vector4(0.75f, 1f, 0.75f, 0.01f);

            device.SetFillModeDefault();
            device.SetCullModeReverse();
            device.SetBlend(BlendOperation.Subtract, BlendOption.SourceColor, BlendOption.InverseSourceColor);
            device.ApplyRasterState();
            device.UpdateAllStates();

            device.UpdateData(basicBuffer, renderData);
            device.DeviceContext.VertexShader.SetConstantBuffer(0, basicBuffer);
            basicShader.Apply();

            Cube.Draw();

            device.SetCullModeDefault();
            device.SetBlendStateAlphaBlend();
            device.ApplyRasterState();
            device.UpdateAllStates();

            device.UpdateData(defaultBuffer, renderData);
            device.DeviceContext.VertexShader.SetConstantBuffer(0, defaultBuffer);
            defaultShader.Apply();
        }

        public static Dictionary<string, ShaderResourceView> TextureStream = new Dictionary<string, ShaderResourceView>();
        public static ShaderResourceView whiteDefault;

        public static void LoadTextures(string fileNamePrefix)
        {
            if (whiteDefault == null)
                whiteDefault = device.LoadTextureFromFile("Resources\\WhiteDefault.png");
            
            List<string> FilesToLoad = new List<string>();
            if (Directory.Exists(Application.StartupPath + "\\Textures\\" + fileNamePrefix))
                FilesToLoad.AddRange(Directory.GetFiles(Application.StartupPath + "\\Textures\\" + fileNamePrefix));

            foreach (string i in FilesToLoad)
            {
                string textureName = TreatTextureName(i);

                if (TextureStream.ContainsKey(textureName))
                {
                    TextureStream[textureName].Dispose();
                    TextureStream[textureName] = device.LoadTextureFromFile(i);
                }
                else
                    TextureStream.Add(textureName, device.LoadTextureFromFile(i));
            }
        }

        public static string TreatTextureName(string entry)
        {
            entry = (Path.GetFileNameWithoutExtension(entry).Trim('_'));
            entry = entry.Trim('_');
            return entry;
        }

        public static Matrix viewProjection;
        public static Color4 backgroundColor = new Color4(0.05f, 0.05f, 0.15f, 1f);

        public static void RunMainLoop(Panel Panel)
        {
            RenderLoop.Run(Panel, () =>
            {
                //Resizing
                if (device.MustResize)
                {
                    device.Resize();
                    aspectRatio = (float)Panel.Width / Panel.Height;
                }

                Program.mainForm.KeyboardController();

                sharpFPS.Update();

                Program.mainForm.SetToolStripStatusLabel(Camera.GetInformation() + " FPS: " + sharpFPS.FPS.ToString());

                device.Clear(backgroundColor);

                viewProjection = Camera.GenerateLookAtRH() * Matrix.PerspectiveFovRH(fovAngle, aspectRatio, near, far);

                device.SetFillModeDefault();
                device.SetCullModeDefault();
                device.SetBlendStateAlphaBlend();
                device.SetDefaultDepthState();
                device.ApplyRasterState();
                device.UpdateAllStates();

                if (showLevel)
                    foreach (RenderableAsset a in ArchiveEditorFunctions.renderableAssetSet)
                    {
                        if (a is AssetLevelModel)
                            (a as AssetLevelModel).Draw();
                    }
                if (showObjects)
                    foreach (RenderableAsset a in ArchiveEditorFunctions.renderableAssetSet)
                    {
                        if (!(a is AssetLevelModel))
                            a.Draw();
                    }

                //present
                device.Present();
            });

            //release resources
            if (whiteDefault != null)
                whiteDefault.Dispose();

            foreach (ShaderResourceView texture in TextureStream.Values)
                texture.Dispose();

            foreach (SharpMesh mesh in RenderWareModelFile.completeMeshList)
                mesh.Dispose();

            Cube.Dispose();
            Pyramid.Dispose();
            Cylinder.Dispose();

            defaultBuffer.Dispose();
            defaultShader.Dispose();

            basicBuffer.Dispose();
            basicShader.Dispose();
                        
            device.Dispose();
        }
    }
}
