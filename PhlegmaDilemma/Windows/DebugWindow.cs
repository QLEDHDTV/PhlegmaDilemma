namespace PhlegmaDilemma.Windows;

public class DebugWindow : Window, IDisposable
{
    private Plugin Plugin;
    private Configuration Configuration;

    // We give this window a hidden ID using ##
    // So that the user will see "My Amazing Window" as window title,
    // but for ImGui the ID is "My Amazing Window##With a hidden ID"
    public DebugWindow(Plugin plugin)
        : base("Debug##", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
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
            $"Target: {data.Target}\n" +
            $"Target Pos: {data.TargetPosition:F3}\n" +
            $"Target HB: {data.TargetHitbox:F3}\n" +
            $"ActionID: {data.ActionID} ({data.ActionName})\n" +
            $"Range: {data.ActionRange}\n" +
            $"Radius: {data.ActionRadius}\n" +
            $"Damaging Action: {data.DamagingAction}\n" +
            $"Distance3D: {data.DistanceToTarget3D:F3}\n" +
            $"Distance2D: {data.DistanceToTarget2D:F3}\n");
    }
}
