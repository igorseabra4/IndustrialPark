using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using IndustrialPark.RenderData;
using static IndustrialPark.Models.BSP_IO_ReadOBJ;

namespace IndustrialPark
{
    public class SharpRenderer
    {
        public SharpDevice device;
        public SharpCamera Camera = new SharpCamera();
        public SharpFPS sharpFPS;

        public SharpRenderer(Control control)
        {
#if !DEBUG
            try
            {
#endif
                device = new SharpDevice(control, false);
#if !DEBUG
        }
            catch (Exception e)
            {
                MessageBox.Show("Error setting up DirectX11 renderer: " + e.Message);
                return;
            }
#endif

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
            device = new SharpDevice(new Control(), false);
            LoadModels(true);
        }

        public SharpShader basicShader;
        public ConstantBuffer<DefaultRenderData> basicBuffer;

        public SharpShader defaultShader;
        public ConstantBuffer<Matrix> defaultBuffer;

        public SharpShader tintedShader;
        public ConstantBuffer<UvAnimRenderData> tintedBuffer;

        public SharpShader fogLightShader;
        public ConstantBuffer<FogLightRenderData> fogLightBuffer;

        public VertexShader outlineVertexShader;
        public PixelShader outlinePixelShader;
        public ConstantBuffer<OutlineRenderData> outlineBuffer;

        public bool showSelectionOutline = true;
        /// <summary>
        /// True while selected assets are being drawn into the selection mask, so they can skip drawing extras (e.g. debug bounding boxes) that shouldn't be outlined
        /// </summary>
        public bool isDrawingSelectionMask { get; private set; }
        public int selectionOutlineThickness = 2;

        public SharpShader jspShader;
        public ConstantBuffer<JspRenderData> jspBuffer;

        public void SetSharpShader()
        {
            basicShader = new SharpShader(device, Application.StartupPath + "/Resources/SharpDX/Shader_Basic.hlsl",
                new SharpShaderDescription() { VertexShaderFunction = "VS", PixelShaderFunction = "PS" },
                new InputElement[] {
                        new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0)
                });

            basicBuffer = new ConstantBuffer<DefaultRenderData>(device.Device);

            defaultShader = new SharpShader(device, Application.StartupPath + "/Resources/SharpDX/Shader_Default.hlsl",
                new SharpShaderDescription() { VertexShaderFunction = "VS", PixelShaderFunction = "PS" },
                new InputElement[] {
                        new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0),
                        new InputElement("COLOR", 0, Format.R8G8B8A8_UNorm, 12, 0),
                        new InputElement("TEXCOORD", 0, Format.R32G32_Float, 16, 0)
                });

            defaultBuffer = new ConstantBuffer<Matrix>(device.Device);

            tintedShader = new SharpShader(device, Application.StartupPath + "/Resources/SharpDX/Shader_Tinted.hlsl",
                new SharpShaderDescription() { VertexShaderFunction = "VS", PixelShaderFunction = "PS" },
                new InputElement[] {
                        new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                        new InputElement("COLOR", 0, Format.R8G8B8A8_UNorm, 16, 0),
                        new InputElement("TEXCOORD", 0, Format.R32G32_Float, 20, 0)
                });

            tintedBuffer = new ConstantBuffer<UvAnimRenderData>(device.Device);

            fogLightShader = new SharpShader(device, Application.StartupPath + "/Resources/SharpDX/Shader_FogLight.hlsl",
                new SharpShaderDescription() { VertexShaderFunction = "VS", PixelShaderFunction = "PS" },
                new InputElement[] {
                        new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                        new InputElement("COLOR", 0, Format.R8G8B8A8_UNorm, 16, 0),
                        new InputElement("TEXCOORD", 0, Format.R32G32_Float, 20, 0),
                        new InputElement("NORMAL", 0, Format.R32G32B32_Float, 28, 0)
                });
            fogLightBuffer = new ConstantBuffer<FogLightRenderData>(device.Device);


            jspShader = new SharpShader(device, Application.StartupPath + "/Resources/SharpDX/Shader_JSP.hlsl",
                new SharpShaderDescription() { VertexShaderFunction = "VS", PixelShaderFunction = "PS" },
                new InputElement[] {
                        new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                        new InputElement("COLOR", 0, Format.R8G8B8A8_UNorm, 16, 0),
                        new InputElement("TEXCOORD", 0, Format.R32G32_Float, 20, 0),
                });
            jspBuffer = new ConstantBuffer<JspRenderData>(device.Device);

            // The outline is a fullscreen pass with no vertex input, so it doesn't use SharpShader (which needs an input layout)
            string outlinePath = Application.StartupPath + "/Resources/SharpDX/Shader_Outline.hlsl";
            using (var vsByteCode = SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile(outlinePath, "VS", "vs_5_0"))
                outlineVertexShader = new VertexShader(device.Device, vsByteCode);
            using (var psByteCode = SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile(outlinePath, "PS", "ps_5_0"))
                outlinePixelShader = new PixelShader(device.Device, psByteCode);
            outlineBuffer = new ConstantBuffer<OutlineRenderData>(device.Device);
        }

        /// <summary>
        /// Draws the selected assets into an offscreen mask, then draws an outline around the mask onto the back buffer.
        /// </summary>
        private void DrawSelectionOutline()
        {
            var selectedAssets = new List<IRenderableAsset>();

            lock (ArchiveEditorFunctions.renderableJSPs)
                foreach (var jsp in ArchiveEditorFunctions.renderableJSPs)
                    if (jsp.isSelected && jsp.ShouldDraw(this))
                        selectedAssets.Add(jsp);

            foreach (IRenderableAsset a in renderableAssets)
                if (a is Asset asset && asset.isSelected)
                    selectedAssets.Add(a);

            if (selectedAssets.Count == 0)
                return;

            device.SetSelectionMaskTarget();
            device.SelectionMaskMode = true;

            device.SetFillModeSolid();
            device.SetCullModeDefault();
            device.ApplyRasterState();
            device.SetDefaultDepthState();
            device.SetBlendStateAlphaBlend();
            device.UpdateAllStates();

            isDrawingSelectionMask = true;
            foreach (IRenderableAsset a in selectedAssets)
                a.Draw(this);
            isDrawingSelectionMask = false;
            device.SelectionMaskMode = false;

            device.SetDefaultTargers();

            device.SetCullModeNone();
            device.SetFillModeDefault();
            device.ApplyRasterState();
            device.SetDepthStateNone();
            device.SetBlendStateAlphaBlend();
            device.UpdateAllStates();

            outlineBuffer.UpdateValue(new OutlineRenderData()
            {
                Color = new Vector4(selectedColor.X, selectedColor.Y, selectedColor.Z, 1f),
                Settings = new Vector4(selectionOutlineThickness, 0, 0, 0)
            });

            var context = device.DeviceContext;
            context.InputAssembler.InputLayout = null;
            context.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            context.VertexShader.Set(outlineVertexShader);
            context.PixelShader.Set(outlinePixelShader);
            context.GeometryShader.Set(null);
            context.DomainShader.Set(null);
            context.HullShader.Set(null);
            context.PixelShader.SetConstantBuffer(0, outlineBuffer.Buffer);
            context.PixelShader.SetShaderResource(0, device.SelectionMaskResourceView);

            context.Draw(3, 0);

            // unbind the mask so it can be used as a render target again
            context.PixelShader.SetShaderResource(0, null);
            device.SetDefaultDepthState();
            device.UpdateAllStates();
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
#if DEBUG
        public static SharpMesh BoundingBox { get; private set; }
        public static List<Vector3> boundingBoxVertices;
#endif

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
            cubeVertices = [];
            cubeTriangles = [];
            Cube = LoadMesh(tiny, "Box", ref cubeVertices, ref cubeTriangles);

            cylinderVertices = [];
            cylinderTriangles = [];
            Cylinder = LoadMesh(tiny, "Cylinder", ref cylinderVertices, ref cylinderTriangles);

            pyramidVertices = [];
            pyramidTriangles = [];
            Pyramid = LoadMesh(tiny, "Pyramid", ref pyramidVertices, ref pyramidTriangles);

            sphereVertices = [];
            sphereTriangles = [];
            Sphere = LoadMesh(tiny, "Sphere", ref sphereVertices, ref sphereTriangles);

            torusVertices = [];
            torusTriangles = [];
            Torus = LoadMesh(tiny, "Torus", ref torusVertices, ref torusTriangles);

            CreatePlaneMesh(tiny);
#if DEBUG
            CreateBoundingBoxMesh();
#endif
            AssetCAM.CamVertexBuffer = SharpDX.Direct3D11.Buffer.Create(device.Device, BindFlags.VertexBuffer, [Vector3.Zero, Vector3.Up * 0.8f]);
        }

        private SharpMesh LoadMesh(bool tiny, string meshName, ref List<Vector3> vertices, ref List<Models.Triangle> triangles)
        {
            var objData = ReadOBJFile(Application.StartupPath + $"/Resources/Models/{meshName}.obj", false);
            List<Vertex> vertexList = [];
            foreach (Models.Vertex v in objData.VertexList)
            {
                vertexList.Add(new Vertex(v.Position));
                vertices.Add(new Vector3(v.Position.X, v.Position.Y, v.Position.Z));
            }
            List<int> indexList = [];
            foreach (Models.Triangle t in objData.TriangleList)
            {
                indexList.Add(t.vertex1);
                indexList.Add(t.vertex2);
                indexList.Add(t.vertex3);
                triangles.Add(t);
            }

            if (!tiny)
                return SharpMesh.Create(device, vertexList.ToArray(), indexList.ToArray());
            return null;
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

#if DEBUG
        private void CreateBoundingBoxMesh()
        {
            boundingBoxVertices = new()
            {
                new Vector3(-0.5f, 0.5f, -0.5f),
                new Vector3(-0.5f, 0.5f, 0.5f),
                new Vector3(0.5f, 0.5f, 0.5f),
                new Vector3(0.5f, 0.5f, -0.5f),
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(-0.5f, -0.5f, 0.5f),
                new Vector3(0.5f, -0.5f, 0.5f),
                new Vector3(0.5f, -0.5f, -0.5f),
            };

            List<int> bboxLines = new()
            {
               0, 1, 1, 2, 2, 3, 3, 0, 0, 4, 4, 5, 5, 6, 6, 7, 7, 4, 1, 5, 2, 6, 3, 7
            };

            BoundingBox = SharpMesh.Create(device, boundingBoxVertices.Select(v => new Vertex(v)).ToArray(), bboxLines.ToArray(), SharpDX.Direct3D.PrimitiveTopology.LineList);
        }
#endif

        public void SetSelectionColor(System.Drawing.Color color)
        {
            colors.selectedColor = new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, selectedColor.W);
            colors.selectedObjectColor = new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, selectedObjectColor.W);
        }

        public void SetWidgetColor(System.Drawing.Color color)
        {
            SetWidgetColor(new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, normalColor.W));
        }

        public void SetWidgetColor(Vector4 widgetColor)
        {
            widgetColor.W = normalColor.W;
            colors.normalColor = widgetColor;
        }

        public void SetTrigColor(System.Drawing.Color color)
        {
            SetTrigColor(new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, trigColor.W));
        }

        public void SetTrigColor(Vector4 widgetColor)
        {
            widgetColor.W = trigColor.W;
            colors.trigColor = widgetColor;
        }

        public void SetMvptColor(System.Drawing.Color color)
        {
            SetMvptColor(new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, mvptColor.W));
        }

        public void SetMvptColor(Vector4 widgetColor)
        {
            widgetColor.W = mvptColor.W;
            colors.mvptColor = widgetColor;
        }

        public void SetSfxColor(System.Drawing.Color color)
        {
            SetSfxColor(new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, sfxColor.W));
        }

        public void SetSfxColor(Vector4 widgetColor)
        {
            widgetColor.W = sfxColor.W;
            colors.sfxColor = widgetColor;
        }

        public void ResetColors()
        {
            backgroundColor = defaultBackgroundColor;
            colors = RendererColors.Default;
        }

        public RendererColors colors = new RendererColors();
        public Vector4 normalColor => colors.normalColor;
        public Vector4 trigColor => colors.trigColor;
        public Vector4 mvptColor => colors.mvptColor;
        public Vector4 sfxColor => colors.sfxColor;
        public Vector4 selectedColor => colors.selectedColor;
        public Vector4 selectedObjectColor => colors.selectedObjectColor;

        DefaultRenderData renderData;

#if DEBUG
        public void DrawBoundingBox(BoundingBox bbox)
        {
            float height = bbox.Maximum.Y - bbox.Minimum.Y;
            float width = bbox.Maximum.X - bbox.Minimum.X;
            float depth = bbox.Maximum.Z - bbox.Minimum.Z;

            renderData.worldViewProjection = Matrix.Scaling(width, height, depth) * Matrix.Translation(bbox.Center) * viewProjection;
            renderData.Color = new Vector4(1f, 0f, 0f, 1f);

            device.SetCullModeNone();
            device.ApplyRasterState();
            device.SetBlendStateAlphaBlend();
            device.SetDefaultDepthState();
            device.UpdateAllStates();

            basicBuffer.UpdateValue(renderData);
            device.DeviceContext.VertexShader.SetConstantBuffer(0, basicBuffer.Buffer);
            basicShader.Apply();

            BoundingBox.Draw(device);
        }
#endif

        public void DrawCube(Matrix world, bool isSelected, float multiplier = 0.5f)
        {
            renderData.worldViewProjection = Matrix.Scaling(multiplier) * world * viewProjection;
            renderData.Color = isSelected ? selectedColor : normalColor;

            device.SetCullModeNone();
            device.ApplyRasterState();
            device.SetBlendStateAlphaBlend();
            device.SetDefaultDepthState();
            device.UpdateAllStates();

            basicBuffer.UpdateValue(renderData);
            device.DeviceContext.VertexShader.SetConstantBuffer(0, basicBuffer.Buffer);
            basicShader.Apply();

            Cube.Draw(device);
        }

        public void DrawPyramid(Matrix world, bool isSelected, float multiplier = 0.5f, Vector4? color = null)
        {
            renderData.worldViewProjection = Matrix.Scaling(multiplier) * world * viewProjection;
            renderData.Color = isSelected ? selectedColor : (color ?? normalColor);

            device.SetCullModeNone();
            device.ApplyRasterState();
            device.SetBlendStateAlphaBlend();
            device.SetDefaultDepthState();
            device.UpdateAllStates();

            basicBuffer.UpdateValue(renderData);
            device.DeviceContext.VertexShader.SetConstantBuffer(0, basicBuffer.Buffer);
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
            device.SetDefaultDepthState();
            device.UpdateAllStates();

            basicBuffer.UpdateValue(renderData);
            device.DeviceContext.VertexShader.SetConstantBuffer(0, basicBuffer.Buffer);
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
            device.SetDefaultDepthState();
            device.UpdateAllStates();

            basicBuffer.UpdateValue(renderData);
            device.DeviceContext.VertexShader.SetConstantBuffer(0, basicBuffer.Buffer);
            basicShader.Apply();

            Cylinder.Draw(device);
        }

        public void DrawPlane(Matrix world, bool isSelected, uint textureAssetID, Vector3 uvAnimOffset)
        {
            UvAnimRenderData renderData;
            renderData.worldViewProjection = world * viewProjection;
            renderData.Color = isSelected ? selectedColor : Vector4.One;
            renderData.UvAnimOffset = (Vector4)uvAnimOffset;

            tintedBuffer.UpdateValue(renderData);
            device.DeviceContext.VertexShader.SetConstantBuffer(0, tintedBuffer.Buffer);
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

            tintedBuffer.UpdateValue(renderData);
            device.DeviceContext.VertexShader.SetConstantBuffer(0, tintedBuffer.Buffer);
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

            basicBuffer.UpdateValue(renderData);
            device.DeviceContext.VertexShader.SetConstantBuffer(0, basicBuffer.Buffer);
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
        public static Color4 backgroundColor;
        public static Color4 defaultBackgroundColor => new Color4(0.05f, 0.05f, 0.15f, 1f);

        public BoundingFrustum frustum;

        public bool isDrawingUI = false;
        public HashSet<IRenderableAsset> renderableAssets = new HashSet<IRenderableAsset>();
        public const float DefaultLODTDistance = 10000f;
        public static bool RenderVertexColors = true;

        public bool allowRender = true;

        private static AssetFOG _fog;
        public static AssetFOG Fog
        {
            get => _fog;
            set
            {
                _fog = value;
                if (value != null)
                    backgroundColor = new Color4(value.BackgroundColor.ToVector4());
                else
                    backgroundColor = defaultBackgroundColor;
            }

        }

        private Stopwatch stopwatch = new Stopwatch();
        private const float TARGET_FRAME_TIME = 1.0f / 60.0f;
#if DEBUG
        public static int TotalDrawCalls = 0;
        public static int TotalVerticesDrawn = 0;
        public static int TotalObjectsDrawn = 0;
        public static int TotalAtomicsDrawn = 0;
        public List<string> debugSelectedObjects = new();
#endif

        public float TransformScaleFactor { get; private set; } = 1.0f;

        private void MainLoop(System.Drawing.Size controlSize)
        {
            if (!stopwatch.IsRunning)
            {
                stopwatch.Start(); // Start the stopwatch for the first frame
            }

            //Resizing
            if (device.MustResize)
            {
                device.Resize();
                Camera.AspectRatio = (float)controlSize.Width / controlSize.Height;
            }

            // Calculate the time elapsed since the last frame
            float elapsedSeconds = Convert.ToSingle(stopwatch.Elapsed.TotalSeconds);
            stopwatch.Restart(); // Restart the stopwatch for the next frame

            // Calculate the scaling factor
            TransformScaleFactor = elapsedSeconds / TARGET_FRAME_TIME;

            Program.MainForm.KeyboardController();
            Camera.Update();

            sharpFPS.Update();

            device.Clear(backgroundColor);
#if DEBUG
            TotalDrawCalls = 0;
            TotalVerticesDrawn = 0;
            TotalObjectsDrawn = 0;
            TotalAtomicsDrawn = 0;
            debugSelectedObjects.Clear();
#endif

            if (!AssetFOG.DontRender && SharpRenderer.Fog != null)
                Camera.FarPlane = (float)Fog.EndDistance;

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

                        Program.MainForm.SetToolStripStatusLabel($"{Camera} | FPS: {sharpFPS.FPS:0.0000}");

                        Matrix view = Camera.ViewMatrix;
                        viewProjection = view * Camera.ProjectionMatrix;
                        frustum = new BoundingFrustum(view * Camera.BiggerFovProjectionMatrix);

                        device.SetFillModeDefault();

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
                                if (EntityAsset.movementPreview && a is AssetWithMotion entity)
                                      entity.Motion.Update();
                            }

                        var renderableAssetsTrans = new HashSet<IRenderableAsset>();

                        foreach (IRenderableAsset a in renderableAssets)
                            if (a.SpecialBlendMode)
                                renderableAssetsTrans.Add(a);
                            else
                                a.Draw(this);

                        foreach (IRenderableAsset a in renderableAssetsTrans.OrderByDescending(a => a.GetDistanceFrom(Camera.Position)))
                            a.Draw(this);

                        if (showSelectionOutline)
                            DrawSelectionOutline();
                    }
#if DEBUG
            device.Font.Begin();
            device.Font.DrawString($"Draw Calls: {TotalDrawCalls}", 0, 0);
            device.Font.DrawString($"Vertices: {TotalVerticesDrawn}", 0, 20);
            device.Font.DrawString($"Frame Time (ms): {elapsedSeconds * 1000}", 0, 40);
            device.Font.DrawString($"Objects: {TotalObjectsDrawn}", 0, 60);
            device.Font.DrawString($"Atomics: {TotalAtomicsDrawn}", 0, 80);
            for (int i = 0; i < debugSelectedObjects.Count; i++)
                device.Font.DrawString(debugSelectedObjects[i], 1000, i * 15);
            device.Font.End();
#endif
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

            AssetCAM.CamVertexBuffer.Dispose();

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

            fogLightBuffer.Dispose();
            fogLightShader.Dispose();

            jspBuffer.Dispose();
            jspShader.Dispose();

            outlineBuffer.Dispose();
            outlineVertexShader.Dispose();
            outlinePixelShader.Dispose();

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
