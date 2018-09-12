using SharpDX;
using System.Windows.Forms;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using System.Collections.Generic;
using static IndustrialPark.Models.OBJFunctions;

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
            Camera.Reset();

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

        public static SharpMesh Cube { get; private set; }
        public static SharpMesh Cylinder { get; private set; }
        public static SharpMesh Pyramid { get; private set; }
        public static SharpMesh Sphere { get; private set; }

        public static List<Vector3> cubeVertices;
        public static List<Models.Triangle> cubeTriangles;
        public static List<Vector3> cylinderVertices;
        public static List<Models.Triangle> cylinderTriangles;
        public static List<Vector3> pyramidVertices;
        public static List<Models.Triangle> pyramidTriangles;
        public static List<Vector3> sphereVertices;
        public static List<Models.Triangle> sphereTriangles;

        public void LoadModels()
        {
            cubeVertices = new List<Vector3>();
            cubeTriangles = new List<Models.Triangle>();

            cylinderVertices = new List<Vector3>();
            cylinderTriangles = new List<Models.Triangle>();

            pyramidVertices = new List<Vector3>();
            pyramidTriangles = new List<Models.Triangle>();

            sphereVertices = new List<Vector3>();
            sphereTriangles = new List<Models.Triangle>();

            for (int i = 0; i < 4; i++)// 3; i++)
            {
                Models.ModelConverterData objData;

                if (i == 0) objData = ReadOBJFile("Resources/Models/Box.obj", false);
                else if (i == 1) objData = ReadOBJFile("Resources/Models/Cylinder.obj", false);
                else if (i == 2) objData = ReadOBJFile("Resources/Models/Pyramid.obj", false);
                else objData = ReadOBJFile("Resources/Models/Sphere.obj", false);

                List<Vertex> vertexList = new List<Vertex>();
                foreach (Models.Vertex v in objData.VertexList)
                {
                    vertexList.Add(new Vertex(v.Position));
                    if (i == 0) cubeVertices.Add(new Vector3(v.Position.X, v.Position.Y, v.Position.Z));
                    else if (i == 1) cylinderVertices.Add(new Vector3(v.Position.X, v.Position.Y, v.Position.Z));
                    else if (i == 2) pyramidVertices.Add(new Vector3(v.Position.X, v.Position.Y, v.Position.Z));
                    else if (i == 3) sphereVertices.Add(new Vector3(v.Position.X, v.Position.Y, v.Position.Z));
                }

                List<int> indexList = new List<int>();
                foreach (Models.Triangle t in objData.TriangleList)
                {
                    indexList.Add(t.vertex1);
                    indexList.Add(t.vertex2);
                    indexList.Add(t.vertex3);
                    if (i == 0) cubeTriangles.Add(t);
                    else if (i == 1) cylinderTriangles.Add(t);
                    else if (i == 2) pyramidTriangles.Add(t);
                    else if (i == 3) sphereTriangles.Add(t);
                }

                if (i == 0) Cube = SharpMesh.Create(device, vertexList.ToArray(), indexList.ToArray(), new List<SharpSubSet>() { new SharpSubSet(0, indexList.Count, null) });
                else if (i == 1) Cylinder = SharpMesh.Create(device, vertexList.ToArray(), indexList.ToArray(), new List<SharpSubSet>() { new SharpSubSet(0, indexList.Count, null) });
                else if (i == 2) Pyramid = SharpMesh.Create(device, vertexList.ToArray(), indexList.ToArray(), new List<SharpSubSet>() { new SharpSubSet(0, indexList.Count, null) });
                else Sphere = SharpMesh.Create(device, vertexList.ToArray(), indexList.ToArray(), new List<SharpSubSet>() { new SharpSubSet(0, indexList.Count, null) });
            }
        }
        
        public Vector4 normalColor = new Vector4(0.2f, 0.6f, 0.8f, 0.8f);
        public Vector4 selectedColor = new Vector4(1f, 0.5f, 0.1f, 0.8f);
        public Vector4 selectedObjectColor = new Vector4(1f, 0f, 0f, 1f);
        DefaultRenderData renderData;

        public void DrawCube(Matrix world, bool isSelected, float multiplier = 0.5f)
        {
            renderData.worldViewProjection = Matrix.Scaling(multiplier) * world * viewProjection;

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

        public void DrawPyramid(Matrix world, bool isSelected, float multiplier = 0.5f)
        {
            renderData.worldViewProjection = Matrix.Scaling(multiplier) * world * viewProjection;

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

            Pyramid.Draw(device);
        }

        public void DrawSphere(Matrix world, bool isSelected)
        {
            renderData.worldViewProjection = world * viewProjection;

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

            Sphere.Draw(device);
        }

        public Matrix viewProjection;
        public Color4 backgroundColor = new Color4(0.05f, 0.05f, 0.15f, 1f);
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

                Matrix view = Camera.GetViewMatrix();
                viewProjection = view * Camera.GetProjectionMatrix();
                frustum = new BoundingFrustum(view * Camera.GetBiggerFovProjectionMatrix());

                device.SetFillModeDefault();
                device.SetCullModeDefault();
                device.SetBlendStateAlphaBlend();
                device.SetDefaultDepthState();
                device.ApplyRasterState();
                device.UpdateAllStates();

                List<IRenderableAsset> renderableAssetSet = new List<IRenderableAsset>();
                try
                {
                    renderableAssetSet.AddRange(ArchiveEditorFunctions.renderableAssetSet);
                }
                catch
                {
                    return;
                }

                foreach (IRenderableAsset a in renderableAssetSet)
                {
                    if (a is AssetJSP)
                        (a as AssetJSP).Draw(this);
                }
                foreach (IRenderableAsset a in renderableAssetSet)
                {
                    if (!(a is AssetJSP))
                    {
                        BoundingBox bb = a.GetBoundingBox();
                        if (frustum.Intersects(ref bb))
                            a.Draw(this);
                    }
                }
                ArchiveEditorFunctions.RenderGizmos(this);
                
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
