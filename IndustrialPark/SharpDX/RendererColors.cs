using SharpDX;

namespace IndustrialPark
{
    public struct RendererColors
    {
        public static float defaultAlpha = 0.45f;

        public Vector4 normalColor;
        public Vector4 trigColor;
        public Vector4 mvptColor;
        public Vector4 sfxColor;
        public Vector4 selectedColor;
        public Vector4 selectedObjectColor;

        public static RendererColors Default => new()
        {
            normalColor = new Vector4(0.2f, 0.6f, 0.8f, defaultAlpha),
            trigColor = new Vector4(0.3f, 0.8f, 0.7f, defaultAlpha),
            mvptColor = new Vector4(0.62f, 0.28f, 0.82f, defaultAlpha),
            sfxColor = new Vector4(1f, 0.35f, 0.3f, defaultAlpha),
            selectedColor = new Vector4(1f, 0.7f, 0.05f, defaultAlpha - 0.15f),
            selectedObjectColor = new Vector4(1f, 0.7f, 0.05f, 1f),
        };

        public static float classicDefaultAlpha = 0.5f;
        public static RendererColors Classic => new()
        {
            normalColor = new Vector4(0.2f, 0.6f, 0.8f, classicDefaultAlpha),
            trigColor = new Vector4(0.3f, 0.8f, 0.7f, classicDefaultAlpha),
            mvptColor = new Vector4(0.7f, 0.2f, 0.6f, classicDefaultAlpha),
            sfxColor = new Vector4(1f, 0.2f, 0.2f, classicDefaultAlpha),
            selectedColor = new Vector4(1f, 0.5f, 0.1f, classicDefaultAlpha - 0.1f),
            selectedObjectColor = new Vector4(1f, 0f, 0f, 1f),
        };
    }
}
