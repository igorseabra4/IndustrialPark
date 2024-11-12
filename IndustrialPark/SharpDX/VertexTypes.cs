using SharpDX;

namespace IndustrialPark
{
    /// <summary>
    /// Vertex
    /// </summary>
    public struct Vertex
    {
        /// <summary>
        /// Position
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="position">Position</param>
        public Vertex(Vector3 position)
        {
            Position = position;
        }

        public Vertex(float x, float y, float z)
        {
            Position = new Vector3(x, y, z);
        }
    }

    /// <summary>
    /// Textured Vertex
    /// </summary>
    public struct VertexTextured
    {
        /// <summary>
        /// Position
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// Texture coordinate
        /// </summary>
        public Vector2 TextureCoordinate;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="position">Position</param>
        /// <param name="textureCoordinate">Texture Coordinate</param>
        public VertexTextured(Vector3 position, Vector2 textureCoordinate)
        {
            Position = position;
            TextureCoordinate = textureCoordinate;
        }
    }

    /// <summary>
    /// Colored Vertex
    /// </summary>
    public struct VertexColored
    {
        /// <summary>
        /// Position
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// Color
        /// </summary>
        public Color Color;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="position">Position XYZ</param>
        /// <param name="color">Vertex Color</param>
        public VertexColored(Vector3 position, Color color)
        {
            Position = position;
            Color = color;
        }
    }

    /// <summary>
    /// Colored Textured Vertex
    /// </summary>
    public struct VertexColoredTextured
    {
        /// <summary>
        /// Position
        /// </summary>
        public Vector4 Position;

        /// <summary>
        /// Color
        /// </summary>
        public Color Color;

        /// <summary>
        /// Texture coordinate
        /// </summary>
        public Vector2 TextureCoordinate;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="position">Position XYZ</param>
        /// <param name="color">Vertex Color</param>
        public VertexColoredTextured(Vector3 position, Vector2 textureCoordinate, Color color)
        {
            Position = (Vector4)position;
            Position.W = 1f;
            TextureCoordinate = textureCoordinate;
            Color = color;
        }
    }

    /// <summary>
    /// Colored Textured Vertex
    /// </summary>
    public struct VertexColoredTexturedNormalized
    {
        /// <summary>
        /// Position
        /// </summary>
        public Vector4 Position;

        /// <summary>
        /// Color
        /// </summary>
        public Color Color;

        /// <summary>
        /// Texture coordinate
        /// </summary>
        public Vector2 TextureCoordinate;

        /// <summary>
        /// Normal
        /// </summary>
        public Vector3 Normal;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="position">Position XYZ</param>
        /// <param name="color">Vertex Color</param>
        public VertexColoredTexturedNormalized(Vector3 position, Vector2 textureCoordinate, Color color, Vector3 normal)
        {
            Position = (Vector4)position;
            Position.W = 1f;
            TextureCoordinate = textureCoordinate;
            Color = color;
            Normal = normal;
        }
    }

    /// <summary>
    /// Colored Textured Vertex
    /// </summary>
    public struct VertexTexturedNormalized
    {
        /// <summary>
        /// Position
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// Texture coordinate
        /// </summary>
        public Vector2 TextureCoordinate;

        /// <summary>
        /// Normal
        /// </summary>
        public Vector4 Normal;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="position">Position XYZ</param>
        /// <param name="color">Vertex Color</param>
        public VertexTexturedNormalized(Vector3 position, Vector2 textureCoordinate, Vector3 normal)
        {
            Position = position;
            TextureCoordinate = textureCoordinate;
            Normal = (Vector4)normal;
            Normal.W = 1F;
        }
    }

    /// <summary>
    /// Vertex with Color and Normal
    /// </summary>
    public struct VertexColoredNormalized
    {
        /// <summary>
        /// Position
        /// </summary>
        public Vector4 Position;

        /// <summary>
        /// Normal
        /// </summary>
        public Vector3 Normal;

        /// <summary>
        /// Color
        /// </summary>
        public Color Color;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="position">Position XYZ</param>
        /// <param name="color">Vertex Color</param>
        public VertexColoredNormalized(Vector3 position, Vector3 normal, Color color)
        {
            Position = new Vector4(position, 1f);
            Normal = Vector3.Normalize(normal);
            Color = color;
        }
    }
}
