namespace PhlegmaDilemma.Windows;

public class DebugWindow : Window, IDisposable
{
    private Plugin Plugin;
    private Configuration Configuration;

    // We give this window a hidden ID using ##
    // So that the user will see "My Amazing Window" as window title,
    // but for ImGui the ID is "My Amazing Window##With a hidden ID"
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
        ImGui.TextUnformatted(
            $"Player Pos: {data.PlayerPosition:F3}\n" +
            $"Player Rot: {data.PlayerRotation}\n" +
            $"Mouse Pos: {data.MousePosition:F3}\n\n" +
            $"Target: {data.Target}\n" +
            $"Target Pos: {data.TargetPosition:F3}\n" +
            $"T Distance2D: {data.DistanceToTarget2D:F3}\n" +
            $"T Distance3D: {data.DistanceToTarget3D:F3}\n" +
            $"Target HB: {data.TargetHitbox:F3}\n\n" +
            $"FTarget: {data.FocusTarget}\n" +
            $"FTarget Pos: {data.FocusTargetPosition}\n" +
            $"FT Distance2D: {data.DistanceToFocusTarget2D}\n" +
            $"FT Distance3D: {data.DistanceToFocusTarget3D}\n" +
            $"FTarget HB: {data.FocusTargetHitbox:F3}\n\n" +
            $"ActionID: {data.ActionID} ({data.ActionName})\n" +
            $"Range: {data.ActionRange}\n" +
            $"Radius: {data.ActionRadius}\n" +
            $"Damaging Action: {data.DamagingAction}\n" +
            $"Can target enemy: {data.CanTargetEnemy}\n" +
            $"Cast Type: {data.CastType}\n" +
            $"Angle: {data.ActionAngle}\n" +
            $"Cast Width: {data.CastWidth}");
    }
}
