using SharpDX;

namespace IndustrialPark
{
    public struct RendererColors
    {
        public Vector4 normalColor;
        public Vector4 trigColor;
        public Vector4 mvptColor;
        public Vector4 sfxColor;
        public Vector4 camColor;
        public Vector4 selectedColor;
        public Vector4 selectedObjectColor;

        public static RendererColors Default => new()
        {
            normalColor = new Vector4(0.2f, 0.6f, 0.8f, 1f),
            camColor = new Vector4(0.6f, 0.8f, 0.9f, 1f),
            trigColor = new Vector4(0.3f, 0.8f, 0.7f, 1f),
            mvptColor = new Vector4(0.62f, 0.28f, 0.82f, 1f),
            sfxColor = new Vector4(1f, 0.35f, 0.3f, 1f),
            selectedColor = new Vector4(1f, 0.7f, 0f, 1f),
            selectedObjectColor = new Vector4(1f, 0.7f, 0f, 1f),
        };

        public static RendererColors Classic => new()
        {
            normalColor = new Vector4(0.2f, 0.6f, 0.8f, 1f),
            camColor = new Vector4(0.2f, 0.6f, 0.8f, 1f),
            trigColor = new Vector4(0.3f, 0.8f, 0.7f, 1f),
            mvptColor = new Vector4(0.7f, 0.2f, 0.6f, 1f),
            sfxColor = new Vector4(1f, 0.2f, 0.2f, 1f),
            selectedColor = new Vector4(1f, 0.5f, 0.1f, 1f),
            selectedObjectColor = new Vector4(1f, 0f, 0f, 1f),
        };
    }
}
