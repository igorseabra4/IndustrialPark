using System;

namespace IndustrialPark
{
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
            float value;
            switch (op)
            {
                case Operation.Replace:
                    value = v2;
                    break;
                case Operation.Add:
                    value = v1 + v2;
                    break;
                case Operation.Subtract:
                    value = v1 - v2;
                    break;
                case Operation.Multiply:
                    value = v1 * v2;
                    break;
                case Operation.Divide:
                    value = v1 / v2;
                    break;
                case Operation.RightHandSubtract:
                    value = v2 - v1;
                    break;
                case Operation.RightHandDivide:
                    value = v2 / v1;
                    break;
                case Operation.Minimum:
                    value = Math.Min(v1, v2);
                    break;
                case Operation.Maximum:
                    value = Math.Max(v1, v2);
                    break;
                default:
                    throw new Exception("Unsupported operation");
            }

            if (value < 0)
                return 0;
            if (value > 1)
                return 1;

            return value;
        }
    }
}