using PhlegmaDilemma.Settings;

namespace PhlegmaDilemma.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration Configuration;
    public ConfigWindow(Plugin plugin) : base("Rangefinder Configuration###")
    {
        Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(400, 600);
        SizeCondition = ImGuiCond.Always;

        Configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void PreDraw() { }

    public override void Draw()
    {
        var rangefinderEnabler = Configuration.EnableRangefinder;
        if (ImGui.Checkbox("Enable rangefinder", ref rangefinderEnabler))
        {
            Configuration.EnableRangefinder = rangefinderEnabler;
            Configuration.Save();
        }

        var autoAttackRangeEnabler = Configuration.EnableAutoAttackRange;
        if (ImGui.Checkbox("Enable auto-attack range circle", ref autoAttackRangeEnabler))
        {
            Configuration.EnableAutoAttackRange = autoAttackRangeEnabler;
            Configuration.Save();
        }

        ImGui.Separator();

        var thickness = Configuration.Thickness;
        if (ImGui.InputFloat("Line thickness", ref thickness))
        {
            if (thickness <= 15)
            {
                Configuration.Thickness = thickness;
            }
            Configuration.Save();
        }

        var resolution = Configuration.PointsNumber;
        if (ImGui.InputInt("Resolution", ref resolution))
        {
            if (resolution <= 250) 
            {
                Configuration.PointsNumber = resolution;
            }
            Configuration.Save();
        }

        ImGui.Separator();

        var actionRangeColor = Configuration.ColorActionRange;
        if (ImGui.CollapsingHeader("Action range color"))
        {
            if (ImGui.ColorPicker4("##range", ref actionRangeColor))
            {
                Configuration.ColorActionRange = actionRangeColor;
                Configuration.Save();
            }
        }

        var actionRadiusColor = Configuration.ColorActionRadius;
        if (ImGui.CollapsingHeader("Action radius color"))
        {
            if (ImGui.ColorPicker4("##radius", ref actionRadiusColor))
            {
                Configuration.ColorActionRadius = actionRadiusColor;
                Configuration.Save();
            }
        }

        var autoAttackRangeColor = Configuration.ColorAutoAttack;
        if (ImGui.CollapsingHeader("Auto-attack range color"))
        {
            if (ImGui.ColorPicker4("##autoattack", ref autoAttackRangeColor))
            {
                Configuration.ColorAutoAttack = autoAttackRangeColor;
                Configuration.Save();
            }
        }

        var targetOutofRangePointerColor = Configuration.ColorTargetPointerOutOfRange;
        if (ImGui.CollapsingHeader("Target out-of-range pointer color"))
        {
            if (ImGui.ColorPicker4("##targetofr", ref targetOutofRangePointerColor))
            {
                Configuration.ColorTargetPointerOutOfRange = targetOutofRangePointerColor;
                Configuration.Save();
            }
        }

        var targetInRangePointerColor = Configuration.ColorTargetPointerInRange;
        if (ImGui.CollapsingHeader("Target in-range pointer color"))
        {
            if (ImGui.ColorPicker4("##targetofr", ref targetInRangePointerColor))
            {
                Configuration.ColorTargetPointerInRange = targetInRangePointerColor;
                Configuration.Save();
            }
        }

        ImGui.Separator();

        var debugCone = Configuration.EnableDebugCone;
        if (ImGui.Checkbox("Debug cone", ref debugCone))
        {
            Configuration.EnableDebugCone = debugCone;
            Configuration.Save();
        }

        var debugConeRadius = Configuration.DebugConeRadius;
        if (ImGui.InputFloat("Radius", ref debugConeRadius))
        {
            Configuration.DebugConeRadius = debugConeRadius;
            Configuration.Save();
        }

        var debugConeAngle = Configuration.DebugConeAngle;
        if (ImGui.InputFloat("Angle", ref debugConeAngle))
        {
            Configuration.DebugConeAngle = debugConeAngle;
            Configuration.Save();
        }

        var debugConeFollowTarget = Configuration.DebugConeFollowTarget;
        if (ImGui.Checkbox("Follow target", ref debugConeFollowTarget))
        {
            Configuration.DebugConeFollowTarget = debugConeFollowTarget;
            Configuration.Save();
        }
    }
}
