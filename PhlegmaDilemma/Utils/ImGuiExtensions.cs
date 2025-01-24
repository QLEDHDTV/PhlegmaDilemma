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

    public static void AddLine3D(this ImDrawListPtr self, Vector3 centerPos, Vector3 targetPos, Vector4 color, float thickness)
    {
        uint colorConverted = ImGuiUtils.Vec4ToUInt(color);
        Plugin.GameGui.WorldToScreen(centerPos, out Vector2 screenSpacePlayer);
        Plugin.GameGui.WorldToScreen(targetPos, out Vector2 screenSpaceTarget);
        self.AddLine(screenSpaceTarget, screenSpacePlayer, colorConverted, thickness);
    }

    public static void AddCone3D(this ImDrawListPtr self, Vector3 centerPos, Vector3 targetPos, float radius, int numPoints, float angle, Vector4 color, float thickness)
    {
        float angleCenterToTarget = (float)Math.Atan2(targetPos.X - centerPos.X, targetPos.Z - centerPos.Z);
        Vector2[] points = new Vector2[numPoints + 2];
        float step = (angle * MathF.PI / 180) / numPoints;
        float theta = angleCenterToTarget + (Math.Abs(angle - 180) / 2 * MathF.PI / 180);
        uint colorConverted = ImGuiUtils.Vec4ToUInt(color);
        Vector3 worldSpace = Vector3.Zero;
        for (int i = 0; i <= numPoints + 1; i++, theta += step)
        {
            if (i == numPoints + 1)
            {
                worldSpace = new Vector3(centerPos.X, centerPos.Y, centerPos.Z);
            }
            else
            {
                worldSpace = new Vector3(centerPos.X - (radius * (float)Math.Cos(theta)), centerPos.Y, centerPos.Z + (radius * (float)Math.Sin(theta)));
            }
            Plugin.GameGui.WorldToScreen(worldSpace, out Vector2 screenSpace);
            points[i] = screenSpace;
        }
        self.AddPolyline(ref points[0], numPoints + 2, colorConverted, ImDrawFlags.Closed, thickness);
    }
    public static void AddSquare3D(this ImDrawListPtr self, Vector3 centerPos, Vector3 targetPos, Vector3 radiusEdge, float width, Vector4 color, float thickness)
    {
        float angleCenterToTarget = (float)Math.Atan2(radiusEdge.X - centerPos.X, radiusEdge.Z - centerPos.Z);
        uint colorConverted = ImGuiUtils.Vec4ToUInt(color);

        Plugin.GameGui.WorldToScreen(new Vector3(centerPos.X + (width / 2) * (float)Math.Cos(angleCenterToTarget), centerPos.Y, centerPos.Z - (width / 2) * (float)Math.Sin(angleCenterToTarget)), out Vector2 leftSideCenter);
        Plugin.GameGui.WorldToScreen(new Vector3(radiusEdge.X + (width / 2) * (float)Math.Cos(angleCenterToTarget), centerPos.Y, radiusEdge.Z - (width / 2) * (float)Math.Sin(angleCenterToTarget)), out Vector2 leftSideEdge);
        Plugin.GameGui.WorldToScreen(new Vector3(centerPos.X - (width / 2) * (float)Math.Cos(angleCenterToTarget), centerPos.Y, centerPos.Z + (width / 2) * (float)Math.Sin(angleCenterToTarget)), out Vector2 rightSideCenter);
        Plugin.GameGui.WorldToScreen(new Vector3(radiusEdge.X - (width / 2) * (float)Math.Cos(angleCenterToTarget), centerPos.Y, radiusEdge.Z + (width / 2) * (float)Math.Sin(angleCenterToTarget)), out Vector2 rightSideEdge);
        self.AddLine(leftSideCenter, leftSideEdge, colorConverted, thickness);
        self.AddLine(rightSideCenter, rightSideEdge, colorConverted, thickness);
        self.AddLine(rightSideCenter, leftSideCenter, colorConverted, thickness);
        self.AddLine(rightSideEdge, leftSideEdge, colorConverted, thickness);
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
