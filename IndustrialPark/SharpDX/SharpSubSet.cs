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
        /// Diffuse map
        /// </summary>
        public ShaderResourceView DiffuseMap { get; set; }
        
        /// <summary>
        /// Diffuse Color (RGBA)
        /// </summary>
        public Vector4 DiffuseColor { get; set; }
        
        /// <summary>
        /// Index Start inside IndexBuffer
        /// </summary>
        public int StartIndex { get; set; }

        /// <summary>
        /// Number of indices to draw
        /// </summary>
        public int IndexCount { get; set; }

        public SharpSubSet(int StartIndex, int IndexCount, ShaderResourceView DiffuseMap)
        {
            this.StartIndex = StartIndex;
            this.IndexCount = IndexCount;
            this.DiffuseMap = DiffuseMap;
            DiffuseColor = Vector4.One;
        }
    }
}
