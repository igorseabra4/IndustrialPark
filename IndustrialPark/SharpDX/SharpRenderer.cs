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
        public SharpDevice device;
        public SharpCamera Camera = new SharpCamera();
        public SharpFPS sharpFPS;

        public SharpRenderer(Control control)
        {
            if (!SharpDevice.IsDirectX11Supported())
            {
                MessageBox.Show("DirectX11 Not Supported");
                return;
            }
            
            device = new SharpDevice(control, false);
            LoadModels();

            sharpFPS = new SharpFPS();
            Camera.AspectRatio = (float)control.ClientSize.Width / control.ClientSize.Height;

            SetSharpShader();
            LoadTexture();
        }

        public SharpShader basicShader;
        public SharpDX.Direct3D11.Buffer basicBuffer;

        public SharpShader defaultShader;
        public SharpDX.Direct3D11.Buffer defaultBuffer;

        public SharpShader tintedShader;
        public SharpDX.Direct3D11.Buffer tintedBuffer;
        
        public void SetSharpShader()
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

            tintedShader = new SharpShader(device, "Resources/SharpDX/Shader_Tinted.hlsl",
                new SharpShaderDescription() { VertexShaderFunction = "VS", PixelShaderFunction = "PS" },
                new InputElement[] {
                        new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                        new InputElement("COLOR", 0, Format.R8G8B8A8_UNorm, 16, 0),
                        new InputElement("TEXCOORD", 0, Format.R32G32_Float, 20, 0)
                });

            tintedBuffer = tintedShader.CreateBuffer<DefaultRenderData>();
        }

        public const string DefaultTexture = "default";
        public static ShaderResourceView whiteDefault;

        public void LoadTexture()
        {
            if (whiteDefault != null)
            {
                if (whiteDefault.IsDisposed)
                    whiteDefault = device.LoadTextureFromFile("Resources\\WhiteDefault.png");
            }
            else
                whiteDefault = device.LoadTextureFromFile("Resources\\WhiteDefault.png");
        }

        private bool showLevel = true;
        private bool showObjects = true;
        
        public void SetLevelModel(bool value)
        {
            showLevel = value;
        }

        public void SetObjects(bool value)
        {
            showObjects = value;
        }
        
        public SharpMesh Cube;
        public SharpMesh Cylinder;
        public SharpMesh Pyramid;
        
        public void LoadModels()
        {
            cubeVertices = new List<Vector3>();

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
                    if (i == 0) cubeVertices.Add(new Vector3(v.Position.X, v.Position.Y, v.Position.Z) * 0.5f);
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

        public Vector4 normalColor = new Vector4(0.2f, 0.6f, 0.8f, 0.8f);
        public Vector4 selectedColor = new Vector4(1f, 0.5f, 0.1f, 0.8f);
        public Vector4 selectedObjectColor = new Vector4(1f, 0f, 0f, 1f);

        public void DrawCube(Matrix world, bool isSelected)
        {
            DefaultRenderData renderData;

            renderData.worldViewProjection = Matrix.Scaling(0.5F) * world * viewProjection;

            if (isSelected)
                renderData.Color = selectedColor;
            else
                renderData.Color = normalColor;

            device.SetFillModeDefault();
            device.SetCullModeNone();
            device.SetBlendStateAlphaBlend();
            device.ApplyRasterState();
            device.UpdateAllStates();

            device.UpdateData(basicBuffer, renderData);
            device.DeviceContext.VertexShader.SetConstantBuffer(0, basicBuffer);
            basicShader.Apply();

            Cube.Draw(device);
        }
        
        public string TreatTextureName(string entry)
        {
            entry = (Path.GetFileNameWithoutExtension(entry).Trim('_'));
            entry = entry.Trim('_');
            return entry;
        }

        public Matrix viewProjection;
        public Color4 backgroundColor = new Color4(0.05f, 0.05f, 0.15f, 1f);
        public List<Vector3> cubeVertices;
        public BoundingFrustum frustum;

        public void RunMainLoop(Panel Panel)
        {
            RenderLoop.Run(Panel, () =>
            {
                //Resizing
                if (device.MustResize)
                {
                    device.Resize();
                    Camera.AspectRatio = (float)Panel.Width / Panel.Height;
                }

                Program.MainForm.KeyboardController();

                sharpFPS.Update();

                Program.MainForm.SetToolStripStatusLabel(Camera.GetInformation() + " FPS: " + $"{sharpFPS.FPS:0.0000}");

                device.Clear(backgroundColor);

                viewProjection = Camera.GetViewMatrix() * Camera.GetProjectionMatrix();
                frustum = new BoundingFrustum(viewProjection);

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
                            (a as AssetLevelModel).Draw(this);
                    }
                if (showObjects)
                    foreach (RenderableAsset a in ArchiveEditorFunctions.renderableAssetSet)
                    {
                        if (!(a is AssetLevelModel))
                            //if (frustum.Intersects(ref a.boundingBox))
                                a.Draw(this);
                    }

                //present
                device.Present();
            });

            //release resources
            whiteDefault.Dispose();
            TextureManager.DisposeTextures();
            
            Cube.Dispose();
            Pyramid.Dispose();
            Cylinder.Dispose();

            basicBuffer.Dispose();
            basicShader.Dispose();

            defaultBuffer.Dispose();
            defaultShader.Dispose();
            
            tintedBuffer.Dispose();
            tintedShader.Dispose();
                        
            foreach (SharpMesh mesh in RenderWareModelFile.completeMeshList)
                mesh.Dispose();
                                    
            device.Dispose();
        }
    }
}
