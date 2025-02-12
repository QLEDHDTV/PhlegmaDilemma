namespace PhlegmaDilemma.Windows;

public class DebugWindow : Window, IDisposable
{
    private Plugin Plugin;
    private Configuration Configuration;
    public DebugWindow(Plugin plugin)
        : base("My Amazing Window##With a hidden ID", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(400, 400),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        Plugin = plugin;
    }

    public void Dispose() { }

    public override void Draw()
    {
        DataDynamic data = Plugin.RetrieveData();
        float targetAngle = (float)Math.Atan2(data.TargetPosition.Z - data.PlayerPosition.Z, data.TargetPosition.X - data.PlayerPosition.X);
        Vector3 actionRangeEdgePoint = new Vector3(data.PlayerPosition.X + data.ActionRange * (float)Math.Cos(targetAngle), data.PlayerPosition.Y, data.PlayerPosition.Z + data.ActionRange * (float)Math.Sin(targetAngle));

        float focusTargetAngle = (float)Math.Atan2(data.FocusTargetPosition.Z - data.PlayerPosition.Z, data.FocusTargetPosition.X - data.PlayerPosition.X);
        Vector3 focusActionRangeEdgePoint = new Vector3(data.PlayerPosition.X + data.ActionRange * (float)Math.Cos(focusTargetAngle), data.PlayerPosition.Y, data.PlayerPosition.Z + data.ActionRange * (float)Math.Sin(focusTargetAngle));

        ImGui.TextUnformatted(
            $"Player Pos: {data.PlayerPosition:F3}\n" +
            $"Player Rot: {data.PlayerRotation}\n" +
            $"Mouse Pos: {data.MousePosition:F3}\n\n" +
            $"Target: {data.Target}\n" +
            $"Target Pos: {data.TargetPosition:F3}\n" +
            $"T Distance2D: {data.DistanceToTarget2D:F3}\n" +
            $"T Distance3D: {data.DistanceToTarget3D:F3}\n" +
            $"T Angle: {(180 / MathF.PI) * targetAngle}\n" +
            $"Target HB: {data.TargetHitbox:F3}\n\n" +
            $"FTarget: {data.FocusTarget}\n" +
            $"FTarget Pos: {data.FocusTargetPosition}\n" +
            $"FT Distance2D: {data.DistanceToFocusTarget2D}\n" +
            $"FT Distance3D: {data.DistanceToFocusTarget3D}\n" +
            $"FTarget HB: {data.FocusTargetHitbox:F3}\n\n" +
            $"ActionID: {data.ActionID} ({data.ActionName})\n" +
            $"Range: {data.ActionRange}\n" +
            $"Radius: {data.ActionRadius}\n" +
            $"FT Edge, Player, T Edge Angle: {MathF.Abs((180 / MathF.PI) * Vector3Exstensions.Angle3Points(focusActionRangeEdgePoint, data.PlayerPosition, actionRangeEdgePoint) - 180)}\n" +
            $"Damaging Action: {data.DamagingAction}\n" +
            $"Can target enemy: {data.CanTargetEnemy}\n" +
            $"Cast Type: {data.CastType}\n" +
            $"Angle: {data.ActionAngle}\n" +
            $"Cast Width: {data.CastWidth}\n\n" +
            $"Party: {String.Join(", ", data.InRangeChars.Select(x => x.EntityId).ToArray())}\n");
    }
}
