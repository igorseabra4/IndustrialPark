﻿using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using static IndustrialPark.Models.BSP_IO_ReadOBJ;

namespace IndustrialPark
{
    public struct DefaultRenderData
    {
        public Matrix worldViewProjection;
        public Vector4 Color;
    }
    public struct UvAnimRenderData
    {
        public Matrix worldViewProjection;
        public Vector4 Color;
        public Vector4 UvAnimOffset;
    }

    public class SharpRenderer
    {
        public SharpDevice device;
        public SharpCamera Camera = new SharpCamera();
        public SharpFPS sharpFPS;

        public SharpRenderer(Control control, int msaaSampleCount = 1)
        {
            try
            {
                device = new SharpDevice(control, false, msaaSampleCount);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error setting up DirectX11 renderer: " + e.Message);
                return;
            }

            LoadModels();

            sharpFPS = new SharpFPS();
            Camera.AspectRatio = (float)control.ClientSize.Width / control.ClientSize.Height;
            Camera.Reset();
            ResetColors();
            SetSharpShader();
            LoadTexture();
            ArchiveEditorFunctions.SetUpGizmos();
        }

        public SharpRenderer()
        {
            LoadModels(true);
        }

        public SharpShader basicShader;
        public SharpDX.Direct3D11.Buffer basicBuffer;

        public SharpShader defaultShader;
        public SharpDX.Direct3D11.Buffer defaultBuffer;

        public SharpShader tintedShader;
        public SharpDX.Direct3D11.Buffer tintedBuffer;

        public void ToggleVertexColors(bool showVertexColors)
        {
            string shaderPath = showVertexColors ? "/Resources/SharpDX/Shader_Tinted.hlsl"
                : "/Resources/SharpDX/Shader_Tinted_NoVertexColor.hlsl";

            tintedShader = new SharpShader(device, Application.StartupPath + shaderPath,
            new SharpShaderDescription() { VertexShaderFunction = "VS", PixelShaderFunction = "PS" },
            new InputElement[] {
                        new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                        new InputElement("COLOR", 0, Format.R8G8B8A8_UNorm, 16, 0),
                        new InputElement("TEXCOORD", 0, Format.R32G32_Float, 20, 0)
            });
        }

        public List<int> GetSupportedMsaaSampleCounts() => device.GetSupportedMsaaSampleCounts();

        public void UpdateMsaaSampleCount(int msaaSampleCount) => device.UpdateMsaaSampleCount(msaaSampleCount);

        public void SetSharpShader()
        {
            basicShader = new SharpShader(device, Application.StartupPath + "/Resources/SharpDX/Shader_Basic.hlsl",
                new SharpShaderDescription() { VertexShaderFunction = "VS", PixelShaderFunction = "PS" },
                new InputElement[] {
                        new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0)
                });

            basicBuffer = basicShader.CreateBuffer<DefaultRenderData>();

            defaultShader = new SharpShader(device, Application.StartupPath + "/Resources/SharpDX/Shader_Default.hlsl",
                new SharpShaderDescription() { VertexShaderFunction = "VS", PixelShaderFunction = "PS" },
                new InputElement[] {
                        new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0),
                        new InputElement("COLOR", 0, Format.R8G8B8A8_UNorm, 12, 0),
                        new InputElement("TEXCOORD", 0, Format.R32G32_Float, 16, 0)
                });

            defaultBuffer = defaultShader.CreateBuffer<Matrix>();

            tintedShader = new SharpShader(device, Application.StartupPath + "/Resources/SharpDX/Shader_Tinted.hlsl",
                new SharpShaderDescription() { VertexShaderFunction = "VS", PixelShaderFunction = "PS" },
                new InputElement[] {
                        new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                        new InputElement("COLOR", 0, Format.R8G8B8A8_UNorm, 16, 0),
                        new InputElement("TEXCOORD", 0, Format.R32G32_Float, 20, 0)
                });

            tintedBuffer = tintedShader.CreateBuffer<UvAnimRenderData>();
        }

        public static ShaderResourceView whiteDefault;
        public static ShaderResourceView arrowDefault;

        public void LoadTexture()
        {
            if (whiteDefault == null || (whiteDefault != null && whiteDefault.IsDisposed))
                whiteDefault = device.LoadTextureFromFile(Application.StartupPath + "/Resources/WhiteDefault.png");
            if (arrowDefault == null || (arrowDefault != null && arrowDefault.IsDisposed))
                arrowDefault = device.LoadTextureFromFile(Application.StartupPath + "/Resources/ArrowDefault.png");
        }

        public static SharpMesh Cube { get; private set; }
        public static SharpMesh Cylinder { get; private set; }
        public static SharpMesh Pyramid { get; private set; }
        public static SharpMesh Sphere { get; private set; }
        public static SharpMesh Plane { get; private set; }
        public static SharpMesh Torus { get; private set; }

        public static List<Vector3> cubeVertices;
        public static List<Models.Triangle> cubeTriangles;
        public static List<Vector3> cylinderVertices;
        public static List<Models.Triangle> cylinderTriangles;
        public static List<Vector3> pyramidVertices;
        public static List<Models.Triangle> pyramidTriangles;
        public static List<Vector3> sphereVertices;
        public static List<Models.Triangle> sphereTriangles;
        public static List<Vector3> planeVertices;
        public static List<Models.Triangle> planeTriangles;
        public static List<Vector3> torusVertices;
        public static List<Models.Triangle> torusTriangles;

        public void LoadModels(bool tiny = false)
        {
            cubeVertices = new List<Vector3>();
            cubeTriangles = new List<Models.Triangle>();

            cylinderVertices = new List<Vector3>();
            cylinderTriangles = new List<Models.Triangle>();

            pyramidVertices = new List<Vector3>();
            pyramidTriangles = new List<Models.Triangle>();

            sphereVertices = new List<Vector3>();
            sphereTriangles = new List<Models.Triangle>();

            torusVertices = new List<Vector3>();
            torusTriangles = new List<Models.Triangle>();

            for (int i = 0; i < 5; i++)
            {
                Models.ModelConverterData objData;

                if (i == 0)
                    objData = ReadOBJFile(Application.StartupPath + "/Resources/Models/Box.obj", false);
                else if (i == 1)
                    objData = ReadOBJFile(Application.StartupPath + "/Resources/Models/Cylinder.obj", false);
                else if (i == 2)
                    objData = ReadOBJFile(Application.StartupPath + "/Resources/Models/Pyramid.obj", false);
                else if (i == 3)
                    objData = ReadOBJFile(Application.StartupPath + "/Resources/Models/Sphere.obj", false);
                else
                    objData = ReadOBJFile(Application.StartupPath + "/Resources/Models/Torus.obj", false);

                List<Vertex> vertexList = new List<Vertex>();
                foreach (Models.Vertex v in objData.VertexList)
                {
                    vertexList.Add(new Vertex(v.Position));
                    if (i == 0)
                        cubeVertices.Add(new Vector3(v.Position.X, v.Position.Y, v.Position.Z));
                    else if (i == 1)
                        cylinderVertices.Add(new Vector3(v.Position.X, v.Position.Y, v.Position.Z));
                    else if (i == 2)
                        pyramidVertices.Add(new Vector3(v.Position.X, v.Position.Y, v.Position.Z));
                    else if (i == 3)
                        sphereVertices.Add(new Vector3(v.Position.X, v.Position.Y, v.Position.Z));
                    else if (i == 4)
                        torusVertices.Add(new Vector3(v.Position.X, v.Position.Y, v.Position.Z));
                }

                List<int> indexList = new List<int>();
                foreach (Models.Triangle t in objData.TriangleList)
                {
                    indexList.Add(t.vertex1);
                    indexList.Add(t.vertex2);
                    indexList.Add(t.vertex3);
                    if (i == 0)
                        cubeTriangles.Add(t);
                    else if (i == 1)
                        cylinderTriangles.Add(t);
                    else if (i == 2)
                        pyramidTriangles.Add(t);
                    else if (i == 3)
                        sphereTriangles.Add(t);
                    else if (i == 4)
                        torusTriangles.Add(t);
                }

                if (!tiny)
                {
                    SharpMesh mesh = SharpMesh.Create(device, vertexList.ToArray(), indexList.ToArray());
                    switch (i)
                    {
                        case 0:
                            Cube = mesh;
                            break;
                        case 1:
                            Cylinder = mesh;
                            break;
                        case 2:
                            Pyramid = mesh;
                            break;
                        case 3:
                            Sphere = mesh;
                            break;
                        case 4:
                            Torus = mesh;
                            break;
                    }
                }
            }

            CreatePlaneMesh(tiny);
        }

        public void CreatePlaneMesh(bool tiny)
        {
            planeVertices = new List<Vector3>();
            planeTriangles = new List<Models.Triangle>();

            List<VertexColoredTextured> vertexList = new List<VertexColoredTextured>
            {
                new VertexColoredTextured(new Vector3(0f, 0f, 0), new Vector2(0, -1), new Color(255, 255, 255, 255)),
                new VertexColoredTextured(new Vector3(0f, -1, 0), new Vector2(0, 0), new Color(255, 255, 255, 255)),
                new VertexColoredTextured(new Vector3(1f, 0f, 0), new Vector2(1, -1), new Color(255, 255, 255, 255)),
                new VertexColoredTextured(new Vector3(1f, -1f, 0), new Vector2(1, 0), new Color(255, 255, 255, 255))
            };

            foreach (VertexColoredTextured v in vertexList)
                planeVertices.Add((Vector3)v.Position);

            List<int> indexList = new List<int>
            {
                0, 1, 2, 3, 2, 1
            };

            planeTriangles.Add(new Models.Triangle() { vertex1 = 0, vertex2 = 1, vertex3 = 2, UVCoord1 = 0, UVCoord2 = 1, UVCoord3 = 2 });
            planeTriangles.Add(new Models.Triangle() { vertex1 = 3, vertex2 = 2, vertex3 = 1, UVCoord1 = 3, UVCoord2 = 2, UVCoord3 = 1 });

            if (!tiny)
                Plane = SharpMesh.Create(device, vertexList.ToArray(), indexList.ToArray());
        }

        public void SetSelectionColor(System.Drawing.Color color)
        {
            selectedColor = new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, selectedColor.W);
            selectedObjectColor = new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, selectedObjectColor.W);
        }

        public void SetWidgetColor(System.Drawing.Color color)
        {
            SetWidgetColor(new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, normalColor.W));
        }

        public void SetWidgetColor(Vector4 widgetColor)
        {
            widgetColor.W = normalColor.W;
            normalColor = widgetColor;
        }

        public void SetTrigColor(System.Drawing.Color color)
        {
            SetTrigColor(new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, trigColor.W));
        }

        public void SetTrigColor(Vector4 widgetColor)
        {
            widgetColor.W = trigColor.W;
            trigColor = widgetColor;
        }

        public void SetMvptColor(System.Drawing.Color color)
        {
            SetMvptColor(new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, mvptColor.W));
        }

        public void SetMvptColor(Vector4 widgetColor)
        {
            widgetColor.W = mvptColor.W;
            mvptColor = widgetColor;
        }

        public void SetSfxColor(System.Drawing.Color color)
        {
            SetSfxColor(new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, sfxColor.W));
        }

        public void SetSfxColor(Vector4 widgetColor)
        {
            widgetColor.W = sfxColor.W;
            sfxColor = widgetColor;
        }

        public void ResetColors()
        {
            var defaultAlpha = 0.5f;

            backgroundColor = new Color4(0.05f, 0.05f, 0.15f, 1f);
            normalColor = new Vector4(0.2f, 0.6f, 0.8f, defaultAlpha);
            trigColor = new Vector4(0.3f, 0.8f, 0.7f, defaultAlpha);
            mvptColor = new Vector4(0.7f, 0.2f, 0.6f, defaultAlpha);
            sfxColor = new Vector4(1f, 0.2f, 0.2f, defaultAlpha);
            selectedColor = new Vector4(1f, 0.5f, 0.1f, defaultAlpha - 0.1f);
            selectedObjectColor = new Vector4(1f, 0f, 0f, 1f);
        }

        public Vector4 normalColor;
        public Vector4 trigColor;
        public Vector4 mvptColor;
        public Vector4 sfxColor;
        public Vector4 selectedColor;
        public Vector4 selectedObjectColor;

        DefaultRenderData renderData;

        public void DrawCube(Matrix world, bool isSelected, float multiplier = 0.5f)
        {
            renderData.worldViewProjection = Matrix.Scaling(multiplier) * world * viewProjection;
            renderData.Color = isSelected ? selectedColor : normalColor;

            device.SetCullModeNone();
            device.ApplyRasterState();
            device.SetBlendStateAlphaBlend();
            device.UpdateAllStates();

            device.UpdateData(basicBuffer, renderData);
            device.DeviceContext.VertexShader.SetConstantBuffer(0, basicBuffer);
            basicShader.Apply();

            Cube.Draw(device);
        }

        public void DrawPyramid(Matrix world, bool isSelected, float multiplier = 0.5f)
        {
            renderData.worldViewProjection = Matrix.Scaling(multiplier) * world * viewProjection;
            renderData.Color = isSelected ? selectedColor : normalColor;

            device.SetCullModeNone();
            device.ApplyRasterState();
            device.SetBlendStateAlphaBlend();
            device.UpdateAllStates();

            device.UpdateData(basicBuffer, renderData);
            device.DeviceContext.VertexShader.SetConstantBuffer(0, basicBuffer);
            basicShader.Apply();

            Pyramid.Draw(device);
        }

        public void DrawSphere(Matrix world, bool isSelected, Vector4 normalColor)
        {
            renderData.worldViewProjection = world * viewProjection;
            renderData.Color = isSelected ? selectedColor : normalColor;

            device.SetCullModeNone();
            device.ApplyRasterState();
            device.SetBlendStateAlphaBlend();
            device.UpdateAllStates();

            device.UpdateData(basicBuffer, renderData);
            device.DeviceContext.VertexShader.SetConstantBuffer(0, basicBuffer);
            basicShader.Apply();

            Sphere.Draw(device);
        }

        public void DrawCylinder(Matrix world, bool isSelected, Vector4 normalColor)
        {
            renderData.worldViewProjection = world * viewProjection;
            renderData.Color = isSelected ? selectedColor : normalColor;

            device.SetCullModeNone();
            device.ApplyRasterState();
            device.SetBlendStateAlphaBlend();
            device.UpdateAllStates();

            device.UpdateData(basicBuffer, renderData);
            device.DeviceContext.VertexShader.SetConstantBuffer(0, basicBuffer);
            basicShader.Apply();

            Cylinder.Draw(device);
        }

        public void DrawPlane(Matrix world, bool isSelected, uint textureAssetID, Vector3 uvAnimOffset)
        {
            UvAnimRenderData renderData;
            renderData.worldViewProjection = world * viewProjection;
            renderData.Color = isSelected ? selectedColor : Vector4.One;
            renderData.UvAnimOffset = (Vector4)uvAnimOffset;

            device.UpdateData(tintedBuffer, renderData);
            device.DeviceContext.VertexShader.SetConstantBuffer(0, tintedBuffer);
            tintedShader.Apply();

            device.DeviceContext.PixelShader.SetShaderResource(0, TextureManager.GetTextureFromDictionary(textureAssetID));

            Plane.Draw(device);
        }

        public void DrawPlaneText(Matrix world, bool isSelected, Vector3 uvAnimOffset, ShaderResourceView texture = null)
        {
            UvAnimRenderData renderData;
            renderData.worldViewProjection = world * viewProjection;
            renderData.Color = isSelected ? selectedColor : Vector4.One;
            renderData.UvAnimOffset = (Vector4)uvAnimOffset;

            device.UpdateData(tintedBuffer, renderData);
            device.DeviceContext.VertexShader.SetConstantBuffer(0, tintedBuffer);
            tintedShader.Apply();

            if (texture == null)
                texture = whiteDefault;

            device.DeviceContext.PixelShader.SetShaderResource(0, texture);

            Plane.Draw(device);
        }

        public List<SharpDX.Direct3D11.Buffer> completeVertexBufferList = new List<SharpDX.Direct3D11.Buffer>();

        public void DrawSpline(SharpDX.Direct3D11.Buffer VertexBuffer, int vertexCount, Matrix world, Vector4 color, bool lineList)
        {
            renderData.worldViewProjection = world * viewProjection;
            renderData.Color = color;

            device.UpdateData(basicBuffer, renderData);
            device.DeviceContext.VertexShader.SetConstantBuffer(0, basicBuffer);
            basicShader.Apply();

            device.DeviceContext.InputAssembler.PrimitiveTopology =
                lineList ? SharpDX.Direct3D.PrimitiveTopology.LineList : SharpDX.Direct3D.PrimitiveTopology.LineStrip;
            device.DeviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, 12, 0));
            device.DeviceContext.Draw(vertexCount, 0);
        }

        private bool playingFly = false;
        private bool recordingFly = false;
        private InternalFlyEditor flyToPlay;

        public void PlayFly(InternalFlyEditor internalFlyEditor)
        {
            playingFly = true;
            flyToPlay = internalFlyEditor;
        }

        public void RecordFly(InternalFlyEditor internalFlyEditor)
        {
            recordingFly = true;
            flyToPlay = internalFlyEditor;
        }

        public void StopFly()
        {
            playingFly = false;
            recordingFly = false;
            flyToPlay = null;
        }

        public Matrix viewProjection;
        public Color4 backgroundColor;
        public BoundingFrustum frustum;

        public bool isDrawingUI = false;
        public HashSet<IRenderableAsset> renderableAssets = new HashSet<IRenderableAsset>();
        public const float DefaultLODTDistance = 100f;

        public bool allowRender = true;

        private void MainLoop(System.Drawing.Size controlSize)
        {
            //Resizing
            if (device.MustResize)
            {
                device.Resize();
                Camera.AspectRatio = (float)controlSize.Width / controlSize.Height;
            }

            Program.MainForm.KeyboardController();

            sharpFPS.Update();

            device.Clear(backgroundColor);

            if (allowRender)
                lock (renderableAssets)
                    if (isDrawingUI)
                    {
                        viewProjection = Matrix.OrthoOffCenterRH(0, 640, -480, 0, -Camera.FarPlane, Camera.FarPlane);

                        device.SetFillModeDefault();
                        device.SetCullModeDefault();
                        device.ApplyRasterState();
                        device.SetBlendStateAlphaBlend();
                        device.SetDefaultDepthState();
                        device.UpdateAllStates();

                        lock (ArchiveEditorFunctions.renderableAssets)
                            foreach (IRenderableAsset a in
                            (from IRenderableAsset asset in ArchiveEditorFunctions.renderableAssets
                             where (asset is AssetUI || asset is AssetUIFT) && asset.ShouldDraw(this)
                             select (IClickableAsset)asset).OrderBy(f => -f.PositionZ))
                                a.Draw(this);
                    }
                    else
                    {
                        if (recordingFly)
                            flyToPlay.Record();
                        else if (playingFly)
                            flyToPlay.Play();

                        Program.MainForm.SetToolStripStatusLabel(Camera.ToString() + " FPS: " + $"{sharpFPS.FPS:0.0000}");

                        Matrix view = Camera.ViewMatrix;
                        viewProjection = view * Camera.ProjectionMatrix;
                        frustum = new BoundingFrustum(view * Camera.BiggerFovProjectionMatrix);

                        device.SetFillModeDefault();
                        device.SetCullModeDefault();
                        device.ApplyRasterState();
                        device.SetDefaultDepthState();
                        device.UpdateAllStates();

                        lock (ArchiveEditorFunctions.renderableJSPs)
                            foreach (var a in ArchiveEditorFunctions.renderableJSPs)
                                if (a.ShouldDraw(this))
                                    a.Draw(this);

                        lock (ArchiveEditorFunctions.renderableAssets)
                            foreach (IRenderableAsset a in ArchiveEditorFunctions.renderableAssets)
                            {
                                if (a.ShouldDraw(this))
                                    renderableAssets.Add(a);
                                else
                                    renderableAssets.Remove(a);
                            }

                        //foreach (IRenderableAsset a in renderableAssets.OrderByDescending(a => a.GetDistanceFrom(Camera.Position)))
                        //    a.Draw(this);

                        var renderableAssetsTrans = new HashSet<IRenderableAsset>();

                        foreach (IRenderableAsset a in renderableAssets)
                            if (a.SpecialBlendMode)
                                renderableAssetsTrans.Add(a);
                            else
                                a.Draw(this);

                        foreach (IRenderableAsset a in renderableAssetsTrans.OrderByDescending(a => a.GetDistanceFrom(Camera.Position)))
                            a.Draw(this);
                    }

            device.SetCullModeNone();
            device.ApplyRasterState();
            device.SetBlendStateAlphaBlend();
            device.UpdateAllStates();

            ArchiveEditorFunctions.RenderGizmos(this);

            device.Present();
        }

        public void RunMainLoop(Control control)
        {
            using (var loop = new RenderLoop(control))
                while (loop.NextFrame())
                    MainLoop(control.Size);

            // main loop is done; release resources

            SoundUtility_vgmstream.Dispose();

            arrowDefault.Dispose();
            whiteDefault.Dispose();
            TextureManager.DisposeTextures();

            Cube.Dispose();
            Pyramid.Dispose();
            Cylinder.Dispose();
            Sphere.Dispose();
            Plane.Dispose();

            basicBuffer.Dispose();
            basicShader.Dispose();

            defaultBuffer.Dispose();
            defaultShader.Dispose();

            tintedBuffer.Dispose();
            tintedShader.Dispose();

            foreach (SharpMesh mesh in RenderWareModelFile.completeMeshList)
                if (mesh != null)
                    mesh.Dispose();

            foreach (var bf in completeVertexBufferList)
                if (bf != null)
                    bf.Dispose();

            device.Dispose();
        }
    }
}
