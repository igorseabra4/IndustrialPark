using SharpDX;
using SharpDX.DXGI;
using System;
using System.Runtime.InteropServices;
using SharpDX.Direct3D11;
using static IndustrialPark.SharpDevice;
using Device = SharpDX.Direct3D11.Device;
using RWTextureAddressMode = RenderWareFile.Sections.TextureAddressMode;
using SharpTextureAddressMode = SharpDX.Direct3D11.TextureAddressMode;

namespace IndustrialPark;

public static class TextureRenderer
{
    public static ShaderResourceView CreateWhiteDefault(Device device)
    {
        var pixels = new byte[] { 255, 255, 255, 255 };
        return CreateGeneratedTexture(device, 1, 1, 1, pixels);
    }

    public static ShaderResourceView CreateArrowDefault(Device device, int size = 128)
    {
        var pixels = new byte[size * size * 4];
        for (int y = 0; y < size; y++)
        {
            int row = y % (size / 2);
            for (int x = 0; x < size; x++)
            {
                bool arrow = x < size / 2
                    ? x >= row && x < row + 4
                    : x >= size - row - 4 && x < size - row;
                int offset = (y * size + x) * 4;
                byte color = arrow ? (byte)0 : (byte)255;
                pixels[offset] = pixels[offset + 1] = pixels[offset + 2] = color;
                pixels[offset + 3] = 255;
            }
        }
        return CreateGeneratedTexture(device, size, size, (int)Math.Log2(size), pixels);
    }

    private static ShaderResourceView CreateGeneratedTexture(Device device, int width, int height, int mipLevels, byte[] pixels)
    {
        var description = new Texture2DDescription
        {
            Width = width,
            Height = height,
            MipLevels = mipLevels,
            ArraySize = 1,
            Format = Format.B8G8R8A8_UNorm,
            BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
            Usage = ResourceUsage.Default,
            SampleDescription = new SampleDescription(1, 0),
            OptionFlags = ResourceOptionFlags.GenerateMipMaps
        };
        using var buffer = new Texture2D(device, description);
        var handle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
        try
        {
            device.ImmediateContext.UpdateSubresource(new DataBox(handle.AddrOfPinnedObject(), width * 4, 0), buffer, 0);
        }
        finally
        {
            handle.Free();
        }

        var view = new ShaderResourceView(device, buffer);
        try
        {
            device.ImmediateContext.GenerateMips(view);
            return view;
        }
        catch
        {
            view.Dispose();
            throw;
        }
    }

    public static ShaderResourceView CreateTextureFromBitmap(Device device, System.Drawing.Bitmap bitmap)
    {
        // Describe and create a Texture2D.
        Texture2DDescription textureDesc = new Texture2DDescription()
        {
            MipLevels = (int)Math.Log(Math.Max(bitmap.Width, bitmap.Height), 2),
            Format = Format.B8G8R8A8_UNorm,
            Width = bitmap.Width,
            Height = bitmap.Height,
            ArraySize = 1,
            BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
            Usage = ResourceUsage.Default,
            SampleDescription = new SampleDescription(1, 0),
            OptionFlags = ResourceOptionFlags.GenerateMipMaps
        };

        System.Drawing.Imaging.BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        DataRectangle[] dataRectangles = new DataRectangle[textureDesc.MipLevels];

        for (int i = 0; i < textureDesc.MipLevels; i++)
            dataRectangles[i] = new DataRectangle(data.Scan0, data.Stride);

        Texture2D buffer = null;
        while (buffer == null)
        {
            try
            {
                buffer = new Texture2D(device, textureDesc, dataRectangles);
            }
            catch { }
        }
        bitmap.UnlockBits(data);

        ShaderResourceView resourceView = new ShaderResourceView(device, buffer);
        device.ImmediateContext.GenerateMips(resourceView);
        buffer.Dispose();
        bitmap.Dispose();

        return resourceView;
    }

    public static ShaderResourceView CreateTextureFromBgraMipmaps(this SharpDevice device, int width, int height,
        byte[][] mipmaps, RWTextureAddressMode addressU, RWTextureAddressMode addressV)
    {
        var description = new Texture2DDescription
        {
            MipLevels = mipmaps.Length,
            Format = Format.B8G8R8A8_UNorm,
            Width = width,
            Height = height,
            ArraySize = 1,
            BindFlags = BindFlags.ShaderResource,
            Usage = ResourceUsage.Default,
            SampleDescription = new SampleDescription(1, 0)
        };
        using var buffer = new Texture2D(device.Device, description);
        for (int i = 0; i < mipmaps.Length; i++)
        {
            var handle = GCHandle.Alloc(mipmaps[i], GCHandleType.Pinned);
            try
            {
                device.DeviceContext.UpdateSubresource(new DataBox(handle.AddrOfPinnedObject(), Math.Max(1, width >> i) * 4, 0), buffer, i);
            }
            finally
            {
                handle.Free();
            }
        }
        return new ShaderResourceView(device.Device, buffer)
        {
            Tag = new SamplerSettings(AddressModeConvert(addressU), AddressModeConvert(addressV))
        };
    }

    private static SharpTextureAddressMode AddressModeConvert(RWTextureAddressMode addressMode) =>
        addressMode switch
    {
        RWTextureAddressMode.TEXTUREADDRESSBORDER => SharpTextureAddressMode.Border,
        RWTextureAddressMode.TEXTUREADDRESSMIRROR => SharpTextureAddressMode.Mirror,
        RWTextureAddressMode.TEXTUREADDRESSCLAMP => SharpTextureAddressMode.Clamp,
        _ => SharpTextureAddressMode.Wrap,
    };
}
