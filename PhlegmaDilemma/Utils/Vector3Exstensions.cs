namespace PhlegmaDilemma.Utils;
internal static class Vector3Exstensions
{
    internal static float Distance2D(this Vector3 value1, Vector3 value2)
    {
        Vector3 diff = value1 - value2;
        return MathF.Sqrt(diff.X * diff.X + diff.Z * diff.Z);
    }

    internal static float Angle3Points(Vector3 value1, Vector3 value2, Vector3 value3)
    {
        Vector2 v1_2 = new Vector2(value2.X - value1.X, value2.Z - value1.Z);
        Vector2 v2_3 = new Vector2(value3.X - value2.X, value3.Z - value2.Z);

        float dot = v1_2.X * v2_3.X + v1_2.Y * v2_3.Y;
        float mag1_2 = MathF.Sqrt(v1_2.X * v1_2.X + v1_2.Y * v1_2.Y);
        float mag2_3 = MathF.Sqrt(v2_3.X * v2_3.X + v2_3.Y * v2_3.Y);

        float cosTheta = dot / (mag1_2 * mag2_3);
        cosTheta = MathF.Max(-1, Math.Min(1, cosTheta));
        
        float angle = MathF.Acos(cosTheta);
        return angle;
    }

    internal static Vector3 ShortestLine2D(Vector3 lineStart, Vector3 lineEnd, Vector3 point, out Vector3 linePosition, out float side)
    {
        side = 0;
        Vector2 A = new Vector2(lineStart.X, lineStart.Z);
        Vector2 B = new Vector2(lineEnd.X, lineEnd.Z);
        Vector2 C = new Vector2(point.X, point.Z);

        Vector2 AB = B - A;
        Vector2 AC = C - A;

        float dot = Vector2.Dot(AC, AB);
        float squaredLength = AB.LengthSquared();

        float distance = dot / squaredLength;
        distance = MathF.Max(0, Math.Min(1, distance));

        Vector2 position = A + distance * AB;
        float crossProduct = AB.X * AC.Y - AB.Y * AC.X;

        side = crossProduct;
        return linePosition = new Vector3(position.X, lineStart.Y, position.Y);
    }
}
