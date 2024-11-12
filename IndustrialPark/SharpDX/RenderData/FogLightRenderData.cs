using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark.RenderData
{
    [StructLayout(LayoutKind.Explicit, Size = 0x210)]
    public struct FogLightRenderData
    {
        [FieldOffset(0)]
        public Matrix worldViewProjection;

        [FieldOffset(0x40)]
        public Matrix world;

        [FieldOffset(0x80)]
        public Vector4 ColorMultiplier;

        [FieldOffset(0x90)]
        public Vector4 MaterialColor;

        [FieldOffset(0xA0)]
        public Vector4 UvAnimOffset;

        [FieldOffset(0xB0)]
        public Vector4 FogColor;

        [FieldOffset(0xC0)]
        public float FogStart;

        [FieldOffset(0xC4)]
        public float FogEnd;

        [FieldOffset(0xC8)]
        public bool FogEnable;

        [FieldOffset(0xCC)]
        public bool VertexColorEnable;

        [FieldOffset(0xD0), MarshalAs(UnmanagedType.ByValArray, SizeConst = AssetLKIT.MAX_DIRECTIONAL_LIGHTS)]
        public DirectionalLight[] DirectionalLights;

        [FieldOffset(0x1D0)]
        public Vector4 AmbientColor;

        [FieldOffset(0x1E0)]
        public bool LightingEnable;

        [FieldOffset(0x1E4)]
        public float DiffuseMult;

        [FieldOffset(0x1E8)]
        public float AmbientMult;

        [FieldOffset(0x1F0)]
        public Vector4 SelectedObjectColor;

        [FieldOffset(0x200)]
        public float AlphaDiscard;

    }

    [StructLayout(LayoutKind.Sequential, Size = 0x20)]
    public struct DirectionalLight
    {
        public Vector4 Direction;
        public Vector4 Color;
    }
}
