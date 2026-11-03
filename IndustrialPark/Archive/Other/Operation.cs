using System;

namespace IndustrialPark.Archive;

public enum Operation
{
    Replace,
    Add,
    Subtract,
    RightHandSubtract,
    Multiply,
    Divide,
    RightHandDivide,
    Minimum,
    Maximum
}

public static class OperationExtensions
{
    public static float PerformAndClamp(this Operation op, float v1, float v2)
    {
        var value = op switch
        {
            Operation.Replace => v2,
            Operation.Add => v1 + v2,
            Operation.Subtract => v1 - v2,
            Operation.Multiply => v1 * v2,
            Operation.Divide => v1 / v2,
            Operation.RightHandSubtract => v2 - v1,
            Operation.RightHandDivide => v2 / v1,
            Operation.Minimum => Math.Min(v1, v2),
            Operation.Maximum => Math.Max(v1, v2),
            _ => throw new Exception("Unsupported operation"),
        };
        if (value < 0)
            return 0;
        if (value > 1)
            return 1;

        return value;
    }
}