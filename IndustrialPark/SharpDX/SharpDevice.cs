using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using Buffer11 = SharpDX.Direct3D11.Buffer;
using Device11 = SharpDX.Direct3D11.Device;

namespace IndustrialPark
{
    public class SharpDevice : IDisposable
    {
        //private 
        private Device11 _device;
        private DeviceContext _deviceContext;
        private SwapChain _swapchain;

        private RenderTargetView _backbufferView;
        private DepthStencilView _zbufferView;

        private RasterizerState _rasterState;
        private BlendState _blendState;
        private DepthStencilState _depthState;
        private SamplerState _samplerState;

        /// <summary>
        /// Device
        /// </summary>
        public Device11 Device => _device;

        /// <summary>
        /// Device Context
        /// </summary>
        public DeviceContext DeviceContext => _deviceContext;

        /// <summary>
        /// Swapchain
        /// </summary>
        public SwapChain SwapChain => _swapchain;

        /// <summary>
        /// Rendering Form
        /// </summary>
        public System.Windows.Forms.Control Control { get; private set; }

        /// <summary>
        /// Indicate that device must be resized
        /// </summary>
        public bool MustResize { get; private set; }

        /// <summary>
        /// View to BackBuffer
        /// </summary>
        public RenderTargetView BackBufferView => _backbufferView;

        /// <summary>
        /// View to Depth Buffer
        /// </summary>
        public DepthStencilView ZBufferView => _zbufferView;

        SampleDescription multisampleDesc = new SampleDescription(1, 0);

        private static FeatureLevel featureLevel = FeatureLevel.Level_11_0;

        /// <summary>
        /// Font batch
        /// </summary>
#if DEBUG
        public Sharp2D Font { get; private set; }
#endif

        /// <summary>
        /// Init all object to start rendering
        /// </summary>
        /// <param name="form">Rendering form</param>
        /// <param name="debug">Active the debug mode</param>
        public SharpDevice(System.Windows.Forms.Control control, bool debug = false)
        {
            Control = control;

            // SwapChain description
            SwapChainDescription desc = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription = new ModeDescription(Control.ClientSize.Width, Control.ClientSize.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = Control.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput,
            };

            //if (GetSupportedFeatureLevel() >= FeatureLevel.Level_10_0)
            //    featureLevel = GetSupportedFeatureLevel();

            FeatureLevel[] levels = new FeatureLevel[] { featureLevel };

            //create device and swapchain
            DeviceCreationFlags flag = DeviceCreationFlags.None | DeviceCreationFlags.BgraSupport;
            if (debug)
                flag = DeviceCreationFlags.Debug;

            Device11.CreateWithSwapChain(DriverType.Hardware, flag, levels, desc, out _device, out _swapchain);

            //get context to device
            _deviceContext = Device.ImmediateContext;

            //Ignore all windows events
            var factory = SwapChain.GetParent<Factory>();
            factory.MakeWindowAssociation(Control.Handle, WindowAssociationFlags.IgnoreAll);

            //Setup handler on resize form
            Control.Resize += (sender, args) => MustResize = true;

            //Set Default State
            ApplyRasterState();
            SetDefaultDepthState();
            SetDefaultBlendState();

#if DEBUG
            Font = new Sharp2D(this);
#endif

            //Resize all items
            Resize();
        }

        /// <summary>
        /// Create and Resize all items
        /// </summary>
        public void Resize()
        {
#if DEBUG
            Font.Release();
#endif
            SetDefaultSamplerState();
            // Dispose all previous allocated resources
            Utilities.Dispose(ref _backbufferView);
            Utilities.Dispose(ref _zbufferView);

            if (Control.Width == 0 || Control.Height == 0)
                return;

            // Resize the backbuffer
            SwapChain.ResizeBuffers(1, Control.Width, Control.Height, Format.R8G8B8A8_UNorm, SwapChainFlags.None);

            // Get the backbuffer from the swapchain
            var _backBufferTexture = SwapChain.GetBackBuffer<Texture2D>(0);

            // update font
#if DEBUG
            Font.UpdateResources(_backBufferTexture);
#endif

            // Backbuffer
            _backbufferView = new RenderTargetView(Device, _backBufferTexture);
            _backBufferTexture.Dispose();

            // Depth buffer

            // Create a descriptor for the depth/stencil buffer.
            // Allocate a 2-D surface as the depth/stencil buffer.
            // Create a DepthStencil view on this surface to use on bind.
            var depthBuffer = new Texture2D(Device, new Texture2DDescription
            {
                Format = currentMode ? Format.D32_Float : Format.D16_UNorm,
                ArraySize = 1,
                MipLevels = 1,
                Width = Control.Width,
                Height = Control.Height,
                SampleDescription = new SampleDescription(1, 0),
                BindFlags = BindFlags.DepthStencil
            });

            // Create the view for binding to the device.
            _zbufferView = new DepthStencilView(Device, depthBuffer,
                new DepthStencilViewDescription()
                {
                    Format = currentMode ? Format.D32_Float : Format.D16_UNorm,
                    //Dimension = DepthStencilViewDimension.Texture2D
                    Dimension = currentMode ? DepthStencilViewDimension.Texture2DMultisampled : DepthStencilViewDimension.Texture2D
                });

            SetDefaultTargers();

            // End resize
            MustResize = false;
        }

        /// <summary>
        /// Set default render and depth buffer inside device context
        /// </summary>
        public void SetDefaultTargers()
        {
            // Setup targets and viewport for rendering
            DeviceContext.Rasterizer.SetViewport(0, 0, Control.Width, Control.Height);
            DeviceContext.OutputMerger.SetTargets(_zbufferView, _backbufferView);
        }

        /// <summary>
        /// Dispose element
        /// </summary>
        public void Dispose()
        {
#if DEBUG
            Font.Dispose();
#endif
            _rasterState.Dispose();
            _blendState.Dispose();
            _depthState.Dispose();
            _samplerState.Dispose();

            _backbufferView.Dispose();
            _zbufferView.Dispose();
            _swapchain.Dispose();
            _deviceContext.Dispose();
            _device.Dispose();
        }

        /// <summary>
        /// Clear backbuffer and zbuffer
        /// </summary>
        /// <param name="color">background color</param>
        public void Clear(Color4 color)
        {
            DeviceContext.ClearRenderTargetView(_backbufferView, color);
            DeviceContext.ClearDepthStencilView(_zbufferView, DepthStencilClearFlags.Depth, 1.0F, 0);
        }

        /// <summary>
        /// Present scene to screen
        /// </summary>
        public void Present()
        {
            SwapChain.Present(_vSync, PresentFlags.UseDuration);
        }

        /// <summary>
        /// Set current rasterizer state to default
        /// </summary>
        public void ApplyRasterState()
        {
            Utilities.Dispose(ref _rasterState);
            //Rasterize state
            _rasterState = new RasterizerState(Device, new RasterizerStateDescription()
            {
                CullMode = currentCullMode,
                FillMode = currentFillMode,
                IsFrontCounterClockwise = true,
                DepthBias = 0,
                DepthBiasClamp = 0,
                IsAntialiasedLineEnabled = currentMode,
                SlopeScaledDepthBias = 0,
                IsDepthClipEnabled = currentMode,
                IsScissorEnabled = false,
                IsMultisampleEnabled = currentMode
            });
            DeviceContext.Rasterizer.State = _rasterState;
        }

        private CullMode currentCullMode = CullMode.Back;
        private FillMode currentFillMode = FillMode.Solid;

        private CullMode normalCullMode = CullMode.Back;
        private FillMode normalFillMode = FillMode.Solid;
        
        public CullMode NormalCullMode
        {
            get => normalCullMode;
        }

        public void SetNormalCullMode(CullMode cullMode)
        {
            normalCullMode = cullMode;
        }

        public void SetCullModeDefault()
        {
            currentCullMode = normalCullMode;
        }

        public void SetCullModeNormal()
        {
            currentCullMode = CullMode.Back;
        }

        public void SetCullModeReverse()
        {
            currentCullMode = CullMode.Front;
        }

        public void SetCullModeNone()
        {
            currentCullMode = CullMode.None;
        }

        public void SetNormalFillMode(FillMode fillMode)
        {
            normalFillMode = fillMode;
        }

        public void SetFillModeDefault()
        {
            currentFillMode = normalFillMode;
        }

        public void SetFillModeSolid()
        {
            currentFillMode = FillMode.Solid;
        }

        public void SetFillModeWireframe()
        {
            currentFillMode = FillMode.Wireframe;
        }

        public FillMode GetFillMode()
        {
            return normalFillMode;
        }

        /// <summary>
        /// Set current blending state to default
        /// </summary>
        public void SetDefaultBlendState()
        {
            Utilities.Dispose(ref _blendState);
            BlendStateDescription description = BlendStateDescription.Default();

            _blendState = new BlendState(Device, description);
        }

        /// <summary>
        /// Set current blending state
        /// </summary>
        /// <param name="operation">Blend Operation</param>
        /// <param name="source">Source Option</param>
        /// <param name="destination">Destination Option</param>
        public void SetBlend(BlendOperation operation, BlendOption source, BlendOption destination)
        {
            Utilities.Dispose(ref _blendState);
            BlendStateDescription description = BlendStateDescription.Default();

            description.RenderTarget[0].BlendOperation = operation;
            description.RenderTarget[0].SourceBlend = source;
            description.RenderTarget[0].DestinationBlend = destination;
            description.RenderTarget[0].IsBlendEnabled = true;

            _blendState = new BlendState(Device, description);
        }

        public void SetBlendStateAlphaBlend()
        {
            Utilities.Dispose(ref _blendState);
            BlendStateDescription description = BlendStateDescription.Default();

            description.RenderTarget[0].IsBlendEnabled = true;
            description.RenderTarget[0].BlendOperation = BlendOperation.Add;
            description.RenderTarget[0].SourceBlend = BlendOption.SourceAlpha;
            description.RenderTarget[0].SourceAlphaBlend = BlendOption.SourceAlpha;
            description.RenderTarget[0].DestinationBlend = BlendOption.InverseSourceAlpha;
            description.RenderTarget[0].DestinationAlphaBlend = BlendOption.InverseSourceAlpha;

            _blendState = new BlendState(Device, description);
        }

        public void SetBlendStateAdditive()
        {
            Utilities.Dispose(ref _blendState);
            BlendStateDescription description = BlendStateDescription.Default();

            description.RenderTarget[0].IsBlendEnabled = true;
            description.RenderTarget[0].BlendOperation = BlendOperation.Add;
            description.RenderTarget[0].SourceBlend = BlendOption.SourceAlpha;
            description.RenderTarget[0].SourceAlphaBlend = BlendOption.SourceAlpha;
            description.RenderTarget[0].DestinationBlend = BlendOption.One;
            description.RenderTarget[0].DestinationAlphaBlend = BlendOption.One;

            _blendState = new BlendState(Device, description);
        }

        /// <summary>
        /// Set current depth state to default
        /// </summary>
        public void SetDefaultDepthState()
        {
            Utilities.Dispose(ref _depthState);
            DepthStencilStateDescription description = DepthStencilStateDescription.Default();

            _depthState = new DepthStencilState(Device, description);
        }

        /// <summary>
        /// Set current depth state to none
        /// </summary>
        public void SetDepthStateNone()
        {
            Utilities.Dispose(ref _depthState);
            DepthStencilStateDescription description = DepthStencilStateDescription.Default();
            description.IsDepthEnabled = false;
            _depthState = new DepthStencilState(Device, description);
        }

        public void DisableDepthBufferWrite()
        {
            Utilities.Dispose(ref _depthState);
            DepthStencilStateDescription description = DepthStencilStateDescription.Default();
            description.DepthWriteMask = DepthWriteMask.Zero;
            _depthState = new DepthStencilState(Device, description);
        }

        /// <summary>
        /// Set current sampler state to default
        /// </summary>
        public void SetDefaultSamplerState()
        {
            Utilities.Dispose(ref _samplerState);
            SamplerStateDescription description = SamplerStateDescription.Default();
            description.Filter = currentMode ? Filter.Anisotropic : Filter.MinMagMipPoint;
            description.AddressU = TextureAddressMode.Wrap;
            description.AddressV = TextureAddressMode.Wrap;
            _samplerState = new SamplerState(Device, description);
        }

        /// <summary>
        /// Set current states inside context
        /// </summary>
        public void UpdateAllStates()
        {
            DeviceContext.OutputMerger.SetBlendState(_blendState);
            DeviceContext.OutputMerger.SetDepthStencilState(_depthState);
            DeviceContext.PixelShader.SetSampler(0, _samplerState);
        }

        /// <summary>
        /// Update constant buffer
        /// </summary>
        /// <typeparam name="T">Data Type</typeparam>
        /// <param name="buffer">Buffer to update</param>
        /// <param name="data">Data to write inside buffer</param>
        public void UpdateData<T>(Buffer11 buffer, T data) where T : struct
        {
            DeviceContext.UpdateSubresource(ref data, buffer);
        }

        /// <summary>
        /// DirectX11 Support Avaiable
        /// </summary>
        /// <returns>Supported</returns>
        public static bool IsDirectX11Supported()
        {
#if DEBUG
            featureLevel = SharpDX.Direct3D11.Device.GetSupportedFeatureLevel();
#endif
            return Device11.GetSupportedFeatureLevel() >= FeatureLevel.Level_10_0;
        }

        public static FeatureLevel GetSupportedFeatureLevel()
        {
            return Device11.GetSupportedFeatureLevel();
        }

        public bool currentMode { get; private set; } = true;

        public void SetGraphicsMode(bool value)
        {
            currentMode = value;
            Resize();
        }

        private int _vSync = 1;

        public bool VSync
        {
            get => _vSync == 1;
            set
            {
                if (value)
                    _vSync = 1;
                else
                    _vSync = 0;
                Resize();
            }
        }
    }
}