using SharpDX;

namespace IndustrialPark.Models
{
    public struct Vertex
    {
        public Vector3 Position;
        public Color Color;
        public Vector2 TexCoord;
        public Vector3 Normal;

        public bool HasUV;
        public bool HasColor;
    }
}