using SharpDX;
using SharpDX.Direct3D11;

namespace IndustrialPark
{
    /// <summary>
    /// Describe a mesh subset
    /// </summary>
    public class SharpSubSet
    {
        /// <summary>
        /// Diffuse map name
        /// </summary>
        public string DiffuseMapName { get; set; }

        /// <summary>
        /// Diffuse map
        /// </summary>
        public ShaderResourceView DiffuseMap { get; set; }

        /// <summary>
        /// Diffuse Color (RGBA)
        /// </summary>
        public Vector4 DiffuseColor { get; set; }

        /// <summary>
        /// Diffuse color multiplier
        /// </summary>
        public float DiffuseMult { get; set; }

        /// <summary>
        /// Ambient color multiplier
        /// </summary>
        public float AmbientMult { get; set; }

        /// <summary>
        /// Has vertex color
        /// </summary>
        public bool EnablePrelight { get; set; }

        /// <summary>
        /// Should render light kits
        /// </summary>
        public bool EnableLights { get; set; }

        /// <summary>
        /// Index Start inside IndexBuffer
        /// </summary>
        public int StartIndex { get; set; }

        /// <summary>
        /// Number of indices to draw
        /// </summary>
        public int IndexCount { get; set; }

        public SharpSubSet(int StartIndex, int IndexCount, ShaderResourceView DiffuseMap, Vector4 DiffuseColor, string DiffuseMapName = "", float DiffuseMult = 1f, float AmbientMult = 1f,
            bool EnablePrelight = true, bool EnableLights = true)
        {
            this.StartIndex = StartIndex;
            this.IndexCount = IndexCount;
            this.DiffuseMap = DiffuseMap;
            this.DiffuseMapName = DiffuseMapName;
            this.DiffuseColor = DiffuseColor;
            this.DiffuseMult = DiffuseMult;
            this.AmbientMult = AmbientMult;
            this.EnablePrelight = EnablePrelight;
            this.EnableLights = EnableLights;
        }

        public SharpSubSet(int StartIndex, int IndexCount, ShaderResourceView DiffuseMap) : this(StartIndex, IndexCount, DiffuseMap, Vector4.One) { }
    }
}
