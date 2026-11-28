using RenderWareFile;
using SharpDX;
using System;
using System.Collections.Generic;

namespace IndustrialPark;

public class ShapeGeometry
{
    public List<Vector3> Vertices { get; } = [];
    public List<Triangle> Triangles { get; } = [];
    public List<int> WireframeIndices { get; } = [];

    internal int AddVertex(Vector3 vertex)
    {
        if (Vertices.Count > ushort.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(vertex), "Shape exceeds the supported vertex count.");
        Vertices.Add(vertex);
        return Vertices.Count - 1;
    }

    internal void AddTriangle(int a, int b, int c, ushort material = 0) =>
        Triangles.Add(new Triangle(material, (ushort)a, (ushort)b, (ushort)c));

    internal void AddEdge(int a, int b)
    {
        WireframeIndices.Add(a);
        WireframeIndices.Add(b);
    }
}

public static class WidgetShapes
{
    private static void CheckDimension(float value, string name)
    {
        if (!float.IsFinite(value) || value <= 0)
            throw new ArgumentOutOfRangeException(name);
    }

    private static void CheckSegments(int segments, string name)
    {
        if (segments < 3)
            throw new ArgumentOutOfRangeException(name);
    }

    public static ShapeGeometry Box(float width = 1, float height = 1, float depth = 1)
    {
        CheckDimension(width, nameof(width));
        CheckDimension(height, nameof(height));
        CheckDimension(depth, nameof(depth));
        var shape = new ShapeGeometry();
        float x = width / 2, y = height / 2, z = depth / 2;
        foreach (var vertex in new[]
        {
            new Vector3(-x, -y, -z), new Vector3(x, -y, -z),
            new Vector3(x, -y, z), new Vector3(-x, -y, z),
            new Vector3(x, y, z), new Vector3(x, y, -z),
            new Vector3(-x, y, -z), new Vector3(-x, y, z)
        })
            shape.AddVertex(vertex);
        foreach (var (a, b, c, d) in new[]
        {
            (0, 1, 2, 3), (4, 5, 6, 7), (2, 4, 7, 3),
            (1, 5, 4, 2), (0, 6, 5, 1), (3, 7, 6, 0)
        })
        {
            shape.AddTriangle(a, b, c);
            shape.AddTriangle(c, d, a);
        }
        foreach (var (a, b) in new[]
        {
            (0, 1), (1, 2), (2, 3), (3, 0),
            (4, 5), (5, 6), (6, 7), (7, 4),
            (0, 6), (1, 5), (2, 4), (3, 7)
        })
            shape.AddEdge(a, b);
        return shape;
    }

    public static ShapeGeometry Pyramid(float width = 1, float height = 1, float depth = 1)
    {
        CheckDimension(width, nameof(width));
        CheckDimension(height, nameof(height));
        CheckDimension(depth, nameof(depth));
        var shape = new ShapeGeometry();
        float x = width / 2, y = height / 2, z = depth / 2;
        foreach (var vertex in new[]
        {
            new Vector3(-x, -y, -z), new Vector3(x, -y, -z),
            new Vector3(0, 0, z), new Vector3(x, y, -z), new Vector3(-x, y, -z)
        })
            shape.AddVertex(vertex);
        foreach (var (a, b, c) in new[]
        {
            (0, 1, 2), (2, 3, 4), (1, 3, 2),
            (0, 4, 3), (3, 1, 0), (4, 0, 2)
        })
            shape.AddTriangle(a, b, c);
        return shape;
    }

    public static ShapeGeometry Sphere(float radius, int segments, int stacks)
    {
        CheckDimension(radius, nameof(radius));
        CheckSegments(segments, nameof(segments));
        if (stacks < 2 || (long)(stacks - 1) * segments + 2 > ushort.MaxValue + 1L)
            throw new ArgumentOutOfRangeException(nameof(stacks));
        var shape = new ShapeGeometry();
        shape.AddVertex(new Vector3(0, radius, 0));
        for (int ring = 1; ring < stacks; ring++)
        {
            float latitude = MathUtil.Pi * ring / stacks;
            for (int i = 0; i < segments; i++)
            {
                float angle = 2 * MathUtil.Pi * i / segments;
                float x = -radius * (float)Math.Sin(latitude) * (float)Math.Sin(angle);
                float y = radius * (float)Math.Cos(latitude);
                float z = -radius * (float)Math.Sin(latitude) * (float)Math.Cos(angle);
                shape.AddVertex(new Vector3(x, y, z));
            }
        }
        int bottom = shape.AddVertex(new Vector3(0, -radius, 0));
        for (int i = 0; i < segments; i++)
        {
            int next = (i + 1) % segments;
            shape.AddTriangle(0, 1 + i, 1 + next);
            shape.AddEdge(0, 1 + i);
        }
        for (int ring = 0; ring < stacks - 2; ring++)
            for (int i = 0; i < segments; i++)
            {
                int next = (i + 1) % segments;
                int a = 1 + ring * segments + i;
                int b = 1 + (ring + 1) * segments + i;
                int c = 1 + (ring + 1) * segments + next;
                int d = 1 + ring * segments + next;
                shape.AddTriangle(a, b, c);
                shape.AddTriangle(a, c, d);
                shape.AddEdge(a, b);
            }
        for (int ring = 0; ring < stacks - 1; ring++)
            for (int i = 0; i < segments; i++)
                shape.AddEdge(1 + ring * segments + i, 1 + ring * segments + (i + 1) % segments);
        int lastRing = 1 + (stacks - 2) * segments;
        for (int i = 0; i < segments; i++)
        {
            int next = (i + 1) % segments;
            shape.AddTriangle(bottom, lastRing + next, lastRing + i);
            shape.AddEdge(bottom, lastRing + i);
        }
        return shape;
    }

    public static ShapeGeometry Cylinder(float radius = 0.5f, float height = 1, int segments = 12)
    {
        CheckDimension(radius, nameof(radius));
        CheckDimension(height, nameof(height));
        CheckSegments(segments, nameof(segments));
        if ((long)segments * 2 + 2 > ushort.MaxValue + 1L)
            throw new ArgumentOutOfRangeException(nameof(segments));
        var shape = new ShapeGeometry();
        int bottomCenter = shape.AddVertex(new Vector3(0, -height / 2, 0));
        for (int ring = 0; ring < 2; ring++)
            for (int i = 0; i < segments; i++)
            {
                float angle = -2 * MathUtil.Pi * i / segments;
                float x = radius * (float)Math.Cos(angle);
                float z = radius * (float)Math.Sin(angle);
                shape.AddVertex(new Vector3(x, ring == 0 ? -height / 2 : height / 2, z));
            }
        int topCenter = shape.AddVertex(new Vector3(0, height / 2, 0));
        int top = 1 + segments;
        for (int i = 0; i < segments; i++)
        {
            int next = (i + 1) % segments;
            int a = 1 + i, b = 1 + next, c = top + next, d = top + i;
            shape.AddTriangle(bottomCenter, b, a);
            shape.AddTriangle(a, c, d);
            shape.AddTriangle(a, b, c);
            shape.AddTriangle(topCenter, d, c);
            shape.AddEdge(a, b);
            shape.AddEdge(d, c);
            shape.AddEdge(a, d);
            shape.AddEdge(bottomCenter, a);
            shape.AddEdge(topCenter, d);
        }
        return shape;
    }

    public static ShapeGeometry Torus(float majorRadius = 1, float minorRadius = 0.1f, int segments = 24, int sides = 4)
    {
        CheckDimension(majorRadius, nameof(majorRadius));
        CheckDimension(minorRadius, nameof(minorRadius));
        CheckSegments(segments, nameof(segments));
        CheckSegments(sides, nameof(sides));
        if ((long)segments * sides > ushort.MaxValue + 1L)
            throw new ArgumentOutOfRangeException(nameof(segments));
        var shape = new ShapeGeometry();
        for (int i = 0; i < segments; i++)
            for (int j = 0; j < sides; j++)
            {
                float angle = 2 * MathUtil.Pi * i / segments;
                float cross = 2 * MathUtil.Pi * j / sides;
                float radial = majorRadius + minorRadius * (float)Math.Cos(cross);
                float x = radial * (float)Math.Sin(angle), y = minorRadius * (float)Math.Sin(cross), z = -radial * (float)Math.Cos(angle);
                shape.AddVertex(new Vector3(x, y, z));
            }
        for (int i = 0; i < segments; i++)
            for (int j = 0; j < sides; j++)
            {
                int a = i * sides + j;
                int b = i * sides + (j + 1) % sides;
                int c = (i + 1) % segments * sides + (j + 1) % sides;
                int d = (i + 1) % segments * sides + j;
                shape.AddTriangle(a, c, d);
                shape.AddTriangle(a, b, c);
            }
        return shape;
    }

    public static ShapeGeometry Cone(float radius = 0.5f, float height = 1, int segments = 32)
    {
        CheckDimension(radius, nameof(radius));
        CheckDimension(height, nameof(height));
        CheckSegments(segments, nameof(segments));
        if ((long)segments + 1 > ushort.MaxValue + 1L)
            throw new ArgumentOutOfRangeException(nameof(segments));
        var shape = new ShapeGeometry();
        for (int i = 0; i < segments; i++)
        {
            float angle = 2 * MathUtil.Pi * i / segments;
            float x = radius * (float)Math.Sin(angle), y = radius * (float)Math.Cos(angle);
            shape.AddVertex(new Vector3(x, y, -height / 2));
        }
        int tip = shape.AddVertex(new Vector3(0, 0, height / 2));
        for (int i = 0; i < segments; i++)
            shape.AddTriangle(i, tip, (i + 1) % segments, ushort.MaxValue);
        shape.AddTriangle(0, 1, 2, ushort.MaxValue);
        for (int i = 2; i < segments - 1; i++)
            shape.AddTriangle(0, i, i + 1, ushort.MaxValue);
        return shape;
    }
}
