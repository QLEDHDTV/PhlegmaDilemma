namespace PhlegmaDilemma;

internal static class ImGuiExtensions
{
    public static unsafe void AddPolycircle3D(this ImDrawListPtr self, Vector3 centerPos, float radius, int numPoints, Vector4 color, float thickness)
    {
        Vector2[] points = new Vector2[numPoints];
        float step = (float)Math.Tau / numPoints;
        float theta = 0f;
        uint colorConverted = ImGuiUtils.Vec4ToUInt(color);

        for (int i = 0; i < numPoints; i++, theta += step)
        {
            Vector3 worldSpace = new Vector3(centerPos.X + (radius * (float)Math.Cos(theta)), centerPos.Y, centerPos.Z - (radius * (float)Math.Sin(theta)));
            Plugin.GameGui.WorldToScreen(worldSpace, out Vector2 screenSpace);

            points[i] = screenSpace;
        }
        self.AddPolyline(ref points[0], numPoints, colorConverted, ImDrawFlags.Closed, thickness);
    }

    public static void AddLine3D(this ImDrawListPtr self, Vector3 startPos, Vector3 endPos, Vector4 color, float thickness)
    {
        uint colorConverted = ImGuiUtils.Vec4ToUInt(color);
        Plugin.GameGui.WorldToScreen(startPos, out Vector2 screenSpaceStart);
        Plugin.GameGui.WorldToScreen(endPos, out Vector2 screenSpaceEnd);
        self.AddLine(screenSpaceStart, screenSpaceEnd, colorConverted, thickness);
    }

    public static void AddCone3D(this ImDrawListPtr self, Vector3 startPos, Vector3 directionPos, float radius, int numPoints, float arcAngle, Vector4 color, float thickness)
    {
        float angleStartToEnd = (float)Math.Atan2(directionPos.X - startPos.X, directionPos.Z - startPos.Z);
        Vector2[] points = new Vector2[numPoints + 2];
        float step = (arcAngle * MathF.PI / 180) / numPoints;
        float theta = angleStartToEnd - ((arcAngle - 180) * 0.5f * MathF.PI / 180);
        uint colorConverted = ImGuiUtils.Vec4ToUInt(color);
        Vector3 worldSpace = Vector3.Zero;
        for (int i = 0; i <= numPoints + 1; i++, theta += step)
        {
            if (i == numPoints + 1)
            {
                worldSpace = new Vector3(startPos.X, startPos.Y, startPos.Z);
            }
            else
            {
                worldSpace = new Vector3(startPos.X - (radius * (float)Math.Cos(theta)), startPos.Y, startPos.Z + (radius * (float)Math.Sin(theta)));
            }
            Plugin.GameGui.WorldToScreen(worldSpace, out Vector2 screenSpace);
            points[i] = screenSpace;
        }
        self.AddPolyline(ref points[0], numPoints + 2, colorConverted, ImDrawFlags.Closed, thickness);
    }

    public static void AddSquare3D(this ImDrawListPtr self, Vector3 startPos, Vector3 directionPos, Vector3 endPos, float width, Vector4 color, float thickness)
    {
        float angleStartToEnd = (float)Math.Atan2(endPos.X - startPos.X, endPos.Z - startPos.Z);
        uint colorConverted = ImGuiUtils.Vec4ToUInt(color);

        Plugin.GameGui.WorldToScreen(new Vector3(startPos.X + (width / 2) * (float)Math.Cos(angleStartToEnd), startPos.Y, startPos.Z - (width / 2) * (float)Math.Sin(angleStartToEnd)), out Vector2 leftSideStart);
        Plugin.GameGui.WorldToScreen(new Vector3(endPos.X + (width / 2) * (float)Math.Cos(angleStartToEnd), startPos.Y, endPos.Z - (width / 2) * (float)Math.Sin(angleStartToEnd)), out Vector2 leftSideEnd);
        Plugin.GameGui.WorldToScreen(new Vector3(startPos.X - (width / 2) * (float)Math.Cos(angleStartToEnd), startPos.Y, startPos.Z + (width / 2) * (float)Math.Sin(angleStartToEnd)), out Vector2 rightSideStart);
        Plugin.GameGui.WorldToScreen(new Vector3(endPos.X - (width / 2) * (float)Math.Cos(angleStartToEnd), startPos.Y, endPos.Z + (width / 2) * (float)Math.Sin(angleStartToEnd)), out Vector2 rightSideEnd);
        self.AddLine(leftSideStart, leftSideEnd, colorConverted, thickness);
        self.AddLine(rightSideStart, rightSideEnd, colorConverted, thickness);
        self.AddLine(rightSideStart, leftSideStart, colorConverted, thickness);
        self.AddLine(rightSideEnd, leftSideEnd, colorConverted, thickness);
    }

    public static void AddScale3D(this ImDrawListPtr self, Vector3 startPos, Vector3 endPos, float distance, float offset, int range, float width, Vector4 color, float thickness)
    {
        uint colorConverted = ImGuiUtils.Vec4ToUInt(color);
        float angle = (float)Math.Atan2(endPos.X - startPos.X, endPos.Z - startPos.Z);
        Plugin.GameGui.WorldToScreen(startPos, out Vector2 screenSpaceStart);
        Plugin.GameGui.WorldToScreen(endPos, out Vector2 screenSpaceEnd);
        Vector3 direction = Vector3.Normalize(endPos - startPos);
        for (float i = 0; i < distance - offset; i += range)
        {
            Plugin.GameGui.WorldToScreen(new Vector3(startPos.X + (direction.X * i) + (width / 2) * (float)Math.Cos(angle), startPos.Y, startPos.Z + (direction.Z * i) - (width / 2) * (float)Math.Sin(angle)), out Vector2 leftPoint);
            Plugin.GameGui.WorldToScreen(new Vector3(startPos.X + (direction.X * i) - (width / 2) * (float)Math.Cos(angle), startPos.Y, startPos.Z + (direction.Z * i) + (width / 2) * (float)Math.Sin(angle)), out Vector2 rightPoint);
            self.AddLine(leftPoint, rightPoint, colorConverted, thickness);
        }
    }
}
