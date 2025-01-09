namespace PhlegmaDilemma.Utils;
internal static class Vector3Exstensions
{
    internal static float Distance2D(this Vector3 value1, Vector3 value2)
    {
        Vector3 diff = value1 - value2;
        return MathF.Sqrt(diff.X * diff.X + diff.Z * diff.Z);
    }
}
