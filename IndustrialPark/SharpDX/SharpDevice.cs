using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using Buffer11 = SharpDX.Direct3D11.Buffer;
using Device11 = SharpDX.Direct3D11.Device;

namespace IndustrialPark;

public class SharpDevice : IDisposable
{
    //private 
    private Device11 _device;
    private DeviceContext _deviceContext;
    private SwapChain _swapchain;

    private RenderTargetView _backbufferView;
    private DepthStencilView _zbufferView;

    private RenderTargetView _selectionMaskView;
    private ShaderResourceView _selectionMaskResourceView;

    private RasterizerState _rasterState;
    private BlendState _blendState;
    private DepthStencilState _depthState;
    private SamplerState _samplerState;
    private bool _disposed;

    private DepthStencilState _defaultDepthState;
    private DepthStencilState _noDepthState;
    private DepthStencilState _readOnlyDepthState;

    private BlendState _defaultBlendState;
    private BlendState _alphaBlendState;
    private BlendState _additiveBlendState;

    private readonly Dictionary<(bool SelectionMask, bool Enabled, BlendOperation Operation, BlendOption Source, BlendOption Destination, BlendOperation AlphaOperation, BlendOption SourceAlpha, BlendOption DestinationAlpha, ColorWriteMaskFlags WriteMask), BlendState> _blendStates = new();

    private readonly Dictionary<(CullMode Cull, FillMode Fill, bool GraphicsMode), RasterizerState> _rasterStates = new();

    public readonly record struct SamplerSettings(TextureAddressMode AddressU, TextureAddressMode AddressV);

    private readonly Dictionary<(SamplerSettings Settings, bool Anisotropic, int MaximumAnisotropy), SamplerState> _samplerStates = new();

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

    /// <summary>
    /// Offscreen mask that selected assets are drawn into for the selection outline
    /// </summary>
    public ShaderResourceView SelectionMaskResourceView => _selectionMaskResourceView;

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
        DeviceCreationFlags flag = DeviceCreationFlags.BgraSupport;
        if (debug)
            flag |= DeviceCreationFlags.Debug;

        Device11.CreateWithSwapChain(DriverType.Hardware, flag, levels, desc, out _device, out _swapchain);

        //get context to device
        _deviceContext = Device.ImmediateContext;

        //Ignore all windows events
        var factory = SwapChain.GetParent<Factory>();
        factory.MakeWindowAssociation(Control.Handle, WindowAssociationFlags.IgnoreAll);

        //Setup handler on resize form
        Control.Resize += OnControlResize;

        InitializeDepthStates();
        InitializeBlendStates();

        //Set Default State
        ApplyRasterState();
        SetDefaultDepthState();
        SetDefaultBlendState();

#if DEBUG
        Font = new Sharp2D(this);
#endif

        SetDefaultSamplerState();
        //Resize all items
        Resize();
    }

    private void InitializeDepthStates()
    {
        _defaultDepthState = new DepthStencilState(Device, DepthStencilStateDescription.Default());

        var noDepthDescription = DepthStencilStateDescription.Default();
        noDepthDescription.IsDepthEnabled = false;

        _noDepthState = new DepthStencilState(Device, noDepthDescription);

        var readOnlyDescription = DepthStencilStateDescription.Default();
        readOnlyDescription.DepthWriteMask = DepthWriteMask.Zero;

        _readOnlyDepthState = new DepthStencilState(Device, readOnlyDescription);

        _depthState = _defaultDepthState;
    }

    private void InitializeBlendStates()
    {
        _defaultBlendState = CreateBlendState(BlendStateDescription.Default());

        var alphaDescription = BlendStateDescription.Default();
        var alphaTarget = alphaDescription.RenderTarget[0];

        alphaTarget.IsBlendEnabled = true;
        alphaTarget.BlendOperation = BlendOperation.Add;
        alphaTarget.SourceBlend = BlendOption.SourceAlpha;
        alphaTarget.SourceAlphaBlend = BlendOption.SourceAlpha;
        alphaTarget.DestinationBlend = BlendOption.InverseSourceAlpha;
        alphaTarget.DestinationAlphaBlend = BlendOption.InverseSourceAlpha;

        alphaDescription.RenderTarget[0] = alphaTarget;
        _alphaBlendState = CreateBlendState(alphaDescription);

        var additiveDescription = BlendStateDescription.Default();
        var additiveTarget = additiveDescription.RenderTarget[0];

        additiveTarget.IsBlendEnabled = true;
        additiveTarget.BlendOperation = BlendOperation.Add;
        additiveTarget.SourceBlend = BlendOption.SourceAlpha;
        additiveTarget.SourceAlphaBlend = BlendOption.SourceAlpha;
        additiveTarget.DestinationBlend = BlendOption.One;
        additiveTarget.DestinationAlphaBlend = BlendOption.One;

        additiveDescription.RenderTarget[0] = additiveTarget;
        _additiveBlendState = CreateBlendState(additiveDescription);

        _blendState = _defaultBlendState;
    }

    /// <summary>
    /// Create and Resize all items
    /// </summary>
    public void Resize()
    {
        if (Control.Width <= 0 || Control.Height <= 0)
            return;

#if DEBUG
        Font.Release();
#endif
        // Dispose all previous allocated resources
        Utilities.Dispose(ref _backbufferView);
        Utilities.Dispose(ref _zbufferView);
        Utilities.Dispose(ref _selectionMaskView);
        Utilities.Dispose(ref _selectionMaskResourceView);

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
                Dimension = DepthStencilViewDimension.Texture2D
            });

        // Selection mask
        using (var selectionMaskTexture = new Texture2D(Device, new Texture2DDescription
        {
            Format = Format.R8G8B8A8_UNorm,
            ArraySize = 1,
            MipLevels = 1,
            Width = Control.Width,
            Height = Control.Height,
            SampleDescription = new SampleDescription(1, 0),
            BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource
        }))
        {
            _selectionMaskView = new RenderTargetView(Device, selectionMaskTexture);
            _selectionMaskResourceView = new ShaderResourceView(Device, selectionMaskTexture);
        }

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
    /// Clear the selection mask and set it as the only render target (no depth buffer)
    /// </summary>
    public void SetSelectionMaskTarget()
    {
        DeviceContext.PixelShader.SetShaderResource(0, null);
        DeviceContext.OutputMerger.SetTargets((DepthStencilView)null, _selectionMaskView);
        DeviceContext.ClearRenderTargetView(_selectionMaskView, new Color4(0, 0, 0, 0));
    }

    /// <summary>
    /// Dispose element
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;

        Control.Resize -= OnControlResize;

#if DEBUG
        Font?.Dispose();
#endif

        foreach (var state in _blendStates.Values)
            state.Dispose();

        foreach (var state in _samplerStates.Values)
            state.Dispose();
        foreach (var state in _rasterStates.Values)
            state.Dispose();

        _rasterStates.Clear();
        _blendStates.Clear();
        _samplerStates.Clear();

        Utilities.Dispose(ref _defaultDepthState);
        Utilities.Dispose(ref _noDepthState);
        Utilities.Dispose(ref _readOnlyDepthState);

        Utilities.Dispose(ref _defaultBlendState);
        Utilities.Dispose(ref _alphaBlendState);
        Utilities.Dispose(ref _additiveBlendState);

        Utilities.Dispose(ref _samplerState);
        Utilities.Dispose(ref _backbufferView);
        Utilities.Dispose(ref _zbufferView);
        Utilities.Dispose(ref _selectionMaskView);
        Utilities.Dispose(ref _selectionMaskResourceView);
        Utilities.Dispose(ref _swapchain);
        Utilities.Dispose(ref _deviceContext);
        Utilities.Dispose(ref _device);
    }

    private void OnControlResize(object sender, EventArgs e) => MustResize = true;

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
        var key = (currentCullMode, currentFillMode, currentMode);

        if (!_rasterStates.TryGetValue(key, out var state))
        {
            state = new RasterizerState(Device, new RasterizerStateDescription()
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

            _rasterStates.Add(key, state);
        }

        _rasterState = state;
        DeviceContext.Rasterizer.State = _rasterState;
    }

    private CullMode currentCullMode = CullMode.Back;
    private FillMode currentFillMode = FillMode.Solid;

    private CullMode normalCullMode = CullMode.Back;
    private FillMode normalFillMode = FillMode.Solid;

    public bool RenderTextures = true;

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
    /// <summary>
    /// While true, blend states are converted so every visible pixel of the selection mask has a non-zero channel. See <see cref="CreateBlendState"/>.
    /// </summary>
    public bool SelectionMaskMode { get; set; }

    private BlendState CreateBlendState(BlendStateDescription description)
    {
        if (SelectionMaskMode)
        {
            // A mask pixel counts as selected if any channel is non-zero, and it must mean "this pixel is visible", which depends on the blend mode:
            // an opaque or additive blend shows the pixel even if its texture alpha is 0 (some models use alpha for other data).
            // Shaders discard pixels where all channels are zero, so accumulating the raw output marks every drawn pixel,
            // while alpha-driven blends only add colour weighted by alpha, so fully transparent texels stay out of the mask.
            var target = description.RenderTarget[0];
            var source = target.IsBlendEnabled ? target.SourceBlend : BlendOption.One;
            bool usesSourceAlpha = source == BlendOption.SourceAlpha || source == BlendOption.SourceAlphaSaturate;

            target.IsBlendEnabled = true;
            target.BlendOperation = BlendOperation.Add;
            target.AlphaBlendOperation = BlendOperation.Add;
            target.SourceBlend = source == BlendOption.Zero ? BlendOption.Zero : usesSourceAlpha ? BlendOption.SourceAlpha : BlendOption.One;
            target.SourceAlphaBlend = source == BlendOption.Zero ? BlendOption.Zero : BlendOption.One;
            target.DestinationBlend = BlendOption.One;
            target.DestinationAlphaBlend = BlendOption.One;
            target.RenderTargetWriteMask = ColorWriteMaskFlags.All;
            description.RenderTarget[0] = target;
        }

        return new BlendState(Device, description);
    }

    public void SetDefaultBlendState()
    {
        _blendState = GetOrCreateBlendState(BlendStateDescription.Default());
    }

    public void SetBlend(BlendOperation operation, BlendOption source, BlendOption destination)
    {
        var description = BlendStateDescription.Default();
        var target = description.RenderTarget[0];

        target.BlendOperation = operation;
        target.SourceBlend = source;
        target.DestinationBlend = destination;
        target.IsBlendEnabled = true;

        description.RenderTarget[0] = target;

        _blendState = GetOrCreateBlendState(description);
    }

    public void SetBlendStateAlphaBlend()
    {
        var description = BlendStateDescription.Default();
        var target = description.RenderTarget[0];

        target.IsBlendEnabled = true;
        target.BlendOperation = BlendOperation.Add;
        target.SourceBlend = BlendOption.SourceAlpha;
        target.DestinationBlend = BlendOption.InverseSourceAlpha;
        target.SourceAlphaBlend = BlendOption.One;
        target.DestinationAlphaBlend = BlendOption.InverseSourceAlpha;

        description.RenderTarget[0] = target;

        _blendState = GetOrCreateBlendState(description);
    }

    public void SetBlendStateAdditive()
    {
        var description = BlendStateDescription.Default();
        var target = description.RenderTarget[0];

        target.IsBlendEnabled = true;
        target.BlendOperation = BlendOperation.Add;
        target.SourceBlend = BlendOption.SourceAlpha;
        target.SourceAlphaBlend = BlendOption.SourceAlpha;
        target.DestinationBlend = BlendOption.One;
        target.DestinationAlphaBlend = BlendOption.One;

        description.RenderTarget[0] = target;

        _blendState = GetOrCreateBlendState(description);
    }

    private BlendState GetOrCreateBlendState(BlendStateDescription description)
    {
        var target = description.RenderTarget[0];

        var key = (
            SelectionMaskMode,
            target.IsBlendEnabled,
            target.BlendOperation,
            target.SourceBlend,
            target.DestinationBlend,
            target.AlphaBlendOperation,
            target.SourceAlphaBlend,
            target.DestinationAlphaBlend,
            target.RenderTargetWriteMask
        );

        if (!_blendStates.TryGetValue(key, out var state))
        {
            state = CreateBlendState(description);
            _blendStates.Add(key, state);
        }

        return state;
    }

    public void SetDefaultDepthState()
    {
        _depthState = _defaultDepthState;
    }

    public void SetDepthStateNone()
    {
        _depthState = _noDepthState;
    }

    public void DisableDepthBufferWrite()
    {
        _depthState = _readOnlyDepthState;
    }

    /// <summary>
    /// Set current sampler state to default
    /// </summary>
    public void SetDefaultSamplerState()
    {
        _samplerState = GetOrCreateSamplerState(new SamplerSettings(TextureAddressMode.Wrap, TextureAddressMode.Wrap), false, 1);
    }

    public int AnisotropicFiltering { get; set; } = 1;

    public void SetSamplerState(SamplerSettings settings)
    {
        bool anisotropic = AnisotropicFiltering > 1;
        _samplerState = GetOrCreateSamplerState(settings, anisotropic, anisotropic ? AnisotropicFiltering : 1);
    }

    private SamplerState GetOrCreateSamplerState(SamplerSettings settings, bool anisotropic, int maximumAnisotropy)
    {
        var key = (settings, anisotropic, maximumAnisotropy);
        if (!_samplerStates.TryGetValue(key, out SamplerState sampler))
        {
            var description = new SamplerStateDescription()
            {
                Filter = anisotropic ? Filter.Anisotropic : Filter.MinMagMipLinear,
                AddressU = settings.AddressU,
                AddressV = settings.AddressV,
                AddressW = TextureAddressMode.Wrap,
                MipLodBias = 0,
                MaximumAnisotropy = maximumAnisotropy,
                ComparisonFunction = Comparison.Never,
                BorderColor = Color4.Black,
                MinimumLod = 0,
                MaximumLod = float.MaxValue
            };

            sampler = new SamplerState(Device, description);
            _samplerStates.Add(key, sampler);
        }

        return sampler;
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
        }
    }
}