using SharpDX;
using System.Windows.Forms;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using System.Collections.Generic;
using static IndustrialPark.Models.BSP_IO_ReadOBJ;
using System.Linq;
using System.IO;
using System;

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

        public SharpRenderer(Control control)
        {
            try
            {
                device = new SharpDevice(control, false);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error setting up DirectX11 renderer: " + e.Message);
                return;
            }

            LoadModels();

            sharpFPS = new SharpFPS();
            Camera.AspectRatio = (float)control.ClientSize.Width / control.ClientSize.Height;
            Camera.ConditionalAspect = 16f / 9f;
            Camera.Reset();
            ResetColors();
            SetSharpShader();
            LoadTexture();
            ArchiveEditorFunctions.SetUpGizmos();
        }

        public SharpShader basicShader;
        public SharpDX.Direct3D11.Buffer basicBuffer;

        public SharpShader defaultShader;
        public SharpDX.Direct3D11.Buffer defaultBuffer;

        public SharpShader tintedShader;
        public SharpDX.Direct3D11.Buffer tintedBuffer;

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

        public const string DefaultTexture = "default";
        public static ShaderResourceView whiteDefault;

        public void LoadTexture()
        {
            if (whiteDefault != null)
            {
                if (whiteDefault.IsDisposed)
                    whiteDefault = device.LoadTextureFromFile(Application.StartupPath + "\\Resources\\WhiteDefault.png");
            }
            else
                whiteDefault = device.LoadTextureFromFile(Application.StartupPath + "\\Resources\\WhiteDefault.png");
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

            for (int i = 0; i < 4; i++)
            {
                Models.ModelConverterData objData;

                if (i == 0) objData = ReadOBJFile(Application.StartupPath + "/Resources/Models/Box.obj", false);
                else if (i == 1) objData = ReadOBJFile(Application.StartupPath + "/Resources/Models/Cylinder.obj", false);
                else if (i == 2) objData = ReadOBJFile(Application.StartupPath + "/Resources/Models/Pyramid.obj", false);
                else  objData = ReadOBJFile(Application.StartupPath + "/Resources/Models/Sphere.obj", false);

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
                else if (i == 3) Sphere = SharpMesh.Create(device, vertexList.ToArray(), indexList.ToArray(), new List<SharpSubSet>() { new SharpSubSet(0, indexList.Count, null) });
            }

            RenderWareModelFile torusModel = new RenderWareModelFile("Torus");
            torusModel.SetForRendering(device, RenderWareFile.ReadFileMethods.ReadRenderWareFile(Application.StartupPath + "/Resources/Models/Torus.DFF"), File.ReadAllBytes(Application.StartupPath + "/Resources/Models/Torus.DFF"));
            Torus = torusModel.meshList[0];
            torusTriangles = new List<Models.Triangle>();
            foreach (RenderWareFile.Triangle t in torusModel.triangleList)
                torusTriangles.Add(new Models.Triangle() { vertex1 = t.vertex1, vertex2 = t.vertex2, vertex3 = t.vertex3 });
            torusVertices = torusModel.vertexListG;

            CreatePlaneMesh();
        }

        public void CreatePlaneMesh()
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

            Plane = SharpMesh.Create(device, vertexList.ToArray(), indexList.ToArray(), new List<SharpSubSet>() { new SharpSubSet(0, indexList.Count, null) });
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
            backgroundColor = new Color4(0.05f, 0.05f, 0.15f, 1f);
            normalColor = new Vector4(0.2f, 0.6f, 0.8f, 0.55f);
            trigColor = new Vector4(0.3f, 0.8f, 0.7f, 0.4f);
            mvptColor = new Vector4(0.7f, 0.2f, 0.6f, 0.5f);
            sfxColor = new Vector4(1f, 0.2f, 0.2f, 0.35f);
            selectedColor = new Vector4(1f, 0.5f, 0.1f, 0.5f);
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
            if (AssetMODL.renderBasedOnLodt && Vector3.Distance(Camera.Position, (Vector3)world.Row4) > 100f)
                return;

            renderData.worldViewProjection = Matrix.Scaling(multiplier) * world * viewProjection;
            renderData.Color = isSelected ? selectedColor : normalColor;

            device.UpdateData(basicBuffer, renderData);
            device.DeviceContext.VertexShader.SetConstantBuffer(0, basicBuffer);
            basicShader.Apply();

            Cube.Draw(device);
        }

        public void DrawPyramid(Matrix world, bool isSelected, float multiplier = 0.5f)
        {
            if (AssetMODL.renderBasedOnLodt && Vector3.Distance(Camera.Position, (Vector3)world.Row4) > 100f)
                return;

            renderData.worldViewProjection = Matrix.Scaling(multiplier) * world * viewProjection;
            renderData.Color = isSelected ? selectedColor : normalColor;

            device.UpdateData(basicBuffer, renderData);
            device.DeviceContext.VertexShader.SetConstantBuffer(0, basicBuffer);
            basicShader.Apply();

            Pyramid.Draw(device);
        }

        public void DrawSphere(Matrix world, bool isSelected, Vector4 normalColor)
        {
            if (AssetMODL.renderBasedOnLodt && Vector3.Distance(Camera.Position, (Vector3)world.Row4) > 100f)
                return;

            renderData.worldViewProjection = world * viewProjection;
            renderData.Color = isSelected ? selectedColor : normalColor;

            device.UpdateData(basicBuffer, renderData);
            device.DeviceContext.VertexShader.SetConstantBuffer(0, basicBuffer);
            basicShader.Apply();

            Sphere.Draw(device);
        }

        public void DrawCylinder(Matrix world, bool isSelected, Vector4 normalColor)
        {
            if (AssetMODL.renderBasedOnLodt && Vector3.Distance(Camera.Position, (Vector3)world.Row4) > 100f)
                return;

            renderData.worldViewProjection = world * viewProjection;
            renderData.Color = isSelected ? selectedColor : normalColor;

            device.UpdateData(basicBuffer, renderData);
            device.DeviceContext.VertexShader.SetConstantBuffer(0, basicBuffer);
            basicShader.Apply();

            Cylinder.Draw(device);
        }

        public void DrawPlane(Matrix world, bool isSelected, uint textureAssetID, Vector3 uvAnimOffset)
        {
            if (AssetMODL.renderBasedOnLodt && Vector3.Distance(Camera.Position, (Vector3)world.Row4) > 100f)
                return;

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

        private bool playingFly = false;
        private InternalFlyEditor flyToPlay;

        public void PlayFly(InternalFlyEditor internalFlyEditor)
        {
            playingFly = true;
            flyToPlay = internalFlyEditor;
        }

        public void StopFly()
        {
            playingFly = false;
            flyToPlay = null;
        }

        public Matrix viewProjection;
        public Color4 backgroundColor;
        public BoundingFrustum frustum;

        public bool isDrawingUI = false;

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

                device.Clear(backgroundColor);

                if (ArchiveEditorFunctions.allowRender)
                    if (isDrawingUI)
                    {
                        viewProjection = Matrix.OrthoOffCenterRH(-480f / (Camera.AspectRatioScreenScale(Camera.AspectRatio, Camera.ConditionalAspect) / Camera.ConditionalAspect) * Camera.AspectRatio / 2f - -480f * (4f / 3f) / 2f, 480f / (Camera.AspectRatioScreenScale(Camera.AspectRatio, Camera.ConditionalAspect) / Camera.ConditionalAspect) * Camera.AspectRatio / 2f + 480f * (4f / 3f) / 2f, (-480f / (Camera.AspectRatioScreenScale(Camera.AspectRatio, Camera.ConditionalAspect) / Camera.ConditionalAspect) / 2f) + (-480f / 2f), (480f / (Camera.AspectRatioScreenScale(Camera.AspectRatio, Camera.ConditionalAspect) / Camera.ConditionalAspect) / 2f) - (480f / 2f), -Camera.FarPlane, Camera.FarPlane);

                        device.SetFillModeDefault();
                        device.SetCullModeDefault();
                        device.ApplyRasterState();
                        device.SetBlendStateAlphaBlend();
                        device.SetDefaultDepthState();
                        device.UpdateAllStates();

                        List<AssetUI> renderCommon = new List<AssetUI>(ArchiveEditorFunctions.renderableAssetSetCommon.Count);

                        foreach (IRenderableAsset a in ArchiveEditorFunctions.renderableAssetSetCommon)
                            if (a is AssetUI ui)
                                renderCommon.Add(ui);
                            else if (a is AssetUIFT uift)
                                renderCommon.Add(uift);

                        renderCommon = renderCommon.OrderBy(f => -f.PositionZ).ToList();

                        foreach (IRenderableAsset a in renderCommon)
                            a.Draw(this);
                    }
                    else
                    {
                        if (playingFly)
                            flyToPlay.Play();

                        Program.MainForm.SetToolStripStatusLabel(Camera.ToString() + " FPS: " + $"{sharpFPS.FPS:0.0000}");

                        Matrix view = Camera.ViewMatrix;
                        viewProjection = view * Camera.ProjectionMatrix;
                        frustum = new BoundingFrustum(view * Camera.BiggerFovProjectionMatrix);

                        device.SetFillModeDefault();
                        device.SetCullModeDefault();
                        device.ApplyRasterState();
                        device.SetBlendStateAlphaBlend();
                        device.SetDefaultDepthState();
                        device.UpdateAllStates();

                        List<IRenderableAsset> renderJSP = new List<IRenderableAsset>(ArchiveEditorFunctions.renderableAssetSetJSP.Count);
                        List<IRenderableAsset> renderCommon = new List<IRenderableAsset>(ArchiveEditorFunctions.renderableAssetSetCommon.Count);
                        List<IRenderableAsset> renderTrans = new List<IRenderableAsset>(ArchiveEditorFunctions.renderableAssetSetTrans.Count);
                        List<IRenderableAsset> renderLarge = new List<IRenderableAsset>();

                        renderJSP.AddRange(ArchiveEditorFunctions.renderableAssetSetJSP);
                        renderCommon.AddRange(ArchiveEditorFunctions.renderableAssetSetCommon);
                        renderTrans.AddRange(ArchiveEditorFunctions.renderableAssetSetTrans);

                        renderCommon = renderCommon.OrderBy(f => f.GetDistance(Camera.Position)).Reverse().ToList();
                        renderTrans = renderTrans.OrderBy(f => f.GetDistance(Camera.Position)).Reverse().ToList();

                        foreach (IRenderableAsset a in renderJSP)
                            a.Draw(this);

                        foreach (IRenderableAsset a in renderCommon)
                        {
                            BoundingBox bb = a.GetBoundingBox();
                            if (bb.Width > 100)
                                renderLarge.Add(a);
                            else if (a is AssetPKUP assetPKUP && AssetPICK.pickEntries.Count == 0)
                                renderTrans.Add(a);
                            else if (PlaceableAsset.movementPreview || frustum.Intersects(ref bb))
                                a.Draw(this);
                        }

                        //device.SetFillModeSolid();
                        device.SetCullModeNone();
                        device.ApplyRasterState();
                        //device.SetBlendStateAlphaBlend();
                        //device.SetDefaultDepthState();
                        //device.UpdateAllStates();

                        foreach (IRenderableAsset a in renderTrans)
                        {
                            BoundingBox bb = a.GetBoundingBox();
                            if (PlaceableAsset.movementPreview || frustum.Intersects(ref bb))
                                a.Draw(this);
                        }

                        //device.SetFillModeDefault();
                        device.SetCullModeDefault();
                        device.ApplyRasterState();
                        //device.SetBlendStateAlphaBlend();
                        //device.SetDefaultDepthState();
                        //device.UpdateAllStates();

                        foreach (IRenderableAsset a in renderLarge)
                        {
                            a.Draw(this);
                        }

                        ArchiveEditorFunctions.RenderGizmos(this);
                    }

                device.Present();
            });

            Program.MainForm.DisposeAllArchiveEditors();

            //release resources
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
                mesh.Dispose();

            device.Dispose();
        }
    }
}
