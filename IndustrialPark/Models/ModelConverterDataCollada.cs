using Collada141;
using SharpDX;
using System.Collections.Generic;

namespace IndustrialPark.Models
{
    public class TriangleListCollada
    {
        public string TextureName;
        public List<Triangle> TriangleList = new List<Triangle>();
    }

    public class ModelConverterDataCollada
    {
        public string ObjectName;

        public List<Vector3> PositionVertexList = new List<Vector3>();
        public List<Vector3> NormalList = new List<Vector3>();
        public List<Vector2> TexCoordList = new List<Vector2>();
        public List<Color> VColorList = new List<Color>();
        public List<TriangleListCollada> TriangleListList = new List<TriangleListCollada>();

        public Matrix TransformMatrix = new Matrix();

        public void GetMatrix(matrix m)
        {
            TransformMatrix = new Matrix
            {
                M11 = (float)m.Values[0],
                M21 = (float)m.Values[1],
                M31 = (float)m.Values[2],
                M41 = (float)m.Values[3],
                M12 = (float)m.Values[4],
                M22 = (float)m.Values[5],
                M32 = (float)m.Values[6],
                M42 = (float)m.Values[7],
                M13 = (float)m.Values[8],
                M23 = (float)m.Values[9],
                M33 = (float)m.Values[10],
                M43 = (float)m.Values[11],
                M14 = (float)m.Values[12],
                M24 = (float)m.Values[13],
                M34 = (float)m.Values[14],
                M44 = (float)m.Values[15]
            };
        }
    }
}
