using PhlegmaDilemma.Settings;

namespace PhlegmaDilemma.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Plugin Plugin;
    private Configuration Configuration;
    public ConfigWindow(Plugin plugin) : base("Rangefinder Configuration###")
    {
        Flags = ImGuiWindowFlags.NoCollapse;
        SizeCondition = ImGuiCond.Always;
        Plugin = plugin;
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
            if (thickness <= 15 && thickness > 0)
            {
                Configuration.Thickness = thickness;
            }
            Configuration.Save();
        }

        var resolution = Configuration.PointsNumber;
        if (ImGui.InputInt("Resolution", ref resolution))
        {
            if (resolution <= 250 && resolution >= 3) 
            {
                Configuration.PointsNumber = resolution;
            }
            Configuration.Save();
        }

        ImGui.Separator();

        var rulerEnabler = Configuration.EnableRuler;
        if (ImGui.Checkbox("Enable range ruler", ref rulerEnabler))
        {
            Configuration.EnableRuler = rulerEnabler;
            Configuration.Save();
        }

        var rulerStartPointAtPlayer = Configuration.RulerStartPointAtPlayer;
        if (ImGui.Checkbox("Rules start position at player position", ref rulerStartPointAtPlayer))
        {
            Configuration.RulerStartPointAtPlayer = rulerStartPointAtPlayer;
            Configuration.Save();
        }

        var enableRulerText = Configuration.EnableRulerText;
        if (ImGui.Checkbox("Enable ruler text", ref enableRulerText))
        {
            Configuration.EnableRulerText = enableRulerText;
            Configuration.Save();
        }

        var rulerTextSize = Configuration.RulerTextSize;
        if (ImGui.InputInt("Font size", ref rulerTextSize))
        {
            if (rulerTextSize > 0)
            {
                Configuration.RulerTextSize = rulerTextSize;
                Configuration.Save();
            }
        }

        var rulerTextLeftSide = Configuration.RulerTexLefttSide;
        if (ImGui.Checkbox("Should text be on the left side?", ref rulerTextLeftSide))
        {
            Configuration.RulerTexLefttSide = rulerTextLeftSide;
            Configuration.Save();
        }

        var rulerTextFrequency = Configuration.RulerTextFrequency;
        if (ImGui.InputInt("Text frequency", ref rulerTextFrequency))
        {
            if (rulerTextFrequency > 0)
            {
                Configuration.RulerTextFrequency = rulerTextFrequency;
                Configuration.Save();
            }
        }

        // TODO: Properly implement offset in ImGuiExtensions
        //var rulerTextOffset = Configuration.RulerTextOffset;
        //if (ImGui.InputFloat2("Text offset", ref rulerTextOffset))
        //{
        //    Configuration.RulerTextOffset = rulerTextOffset;
        //    Configuration.Save();
        //}

        ImGui.Separator();

        var fadeOutEnabler = Configuration.EnableFadeOut;
        if (ImGui.Checkbox("Enable fade-out", ref fadeOutEnabler))
        {
            Configuration.EnableFadeOut = fadeOutEnabler;
            Configuration.Save();
        }

        ImGui.TextUnformatted("Re-enable fade-out to apply new delay.");

        var fadeOutDelay = Configuration.FadeOutDelay;
        if (ImGui.InputFloat("Fade-out delay", ref fadeOutDelay))
        {
            if (fadeOutDelay > 0)
            {
                Configuration.FadeOutDelay = fadeOutDelay;
                Configuration.Save();
            }
        }

        var fadeOutSpeed = Configuration.FadeOutSpeed;
        if (ImGui.InputFloat("Fade-out speed", ref fadeOutSpeed))
        {
            if (fadeOutSpeed > 0)
            {
                Configuration.FadeOutSpeed = fadeOutSpeed;
                Configuration.Save();
            }
        }

        ImGui.Separator();

        var actionRangeColor = Configuration.ColorActionRange;
        if (ImGui.CollapsingHeader("Action range color"))
        {
            if (ImGui.ColorPicker4("##range", ref actionRangeColor))
            {
                Configuration.ColorActionRange = actionRangeColor;
                Configuration.Save();
                Plugin.UpdateRangefinderColors();
            }
        }

        var actionRadiusColor = Configuration.ColorActionRadius;
        if (ImGui.CollapsingHeader("Action radius color"))
        {
            if (ImGui.ColorPicker4("##radius", ref actionRadiusColor))
            {
                Configuration.ColorActionRadius = actionRadiusColor;
                Configuration.Save();
                Plugin.UpdateRangefinderColors();
            }
        }

        var autoAttackRangeColor = Configuration.ColorAutoAttack;
        if (ImGui.CollapsingHeader("Auto-attack range color"))
        {
            if (ImGui.ColorPicker4("##autoattack", ref autoAttackRangeColor))
            {
                Configuration.ColorAutoAttack = autoAttackRangeColor;
                Configuration.Save();
                Plugin.UpdateRangefinderColors();
            }
        }

        var targetOutofRangePointerColor = Configuration.ColorTargetPointerOutOfRange;
        if (ImGui.CollapsingHeader("Target out-of-range pointer color"))
        {
            if (ImGui.ColorPicker4("##targetofr", ref targetOutofRangePointerColor))
            {
                Configuration.ColorTargetPointerOutOfRange = targetOutofRangePointerColor;
                Configuration.Save();
                Plugin.UpdateRangefinderColors();
            }
        }

        var targetInRangePointerColor = Configuration.ColorTargetPointerInRange;
        if (ImGui.CollapsingHeader("Target in-range pointer color"))
        {
            if (ImGui.ColorPicker4("##targetofr", ref targetInRangePointerColor))
            {
                Configuration.ColorTargetPointerInRange = targetInRangePointerColor;
                Configuration.Save();
                Plugin.UpdateRangefinderColors();
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
            if (debugConeRadius > 0)
            {
                Configuration.DebugConeRadius = debugConeRadius;
                Configuration.Save();
            }
        }

        var debugConeAngle = Configuration.DebugConeAngle;
        if (ImGui.InputFloat("Angle", ref debugConeAngle))
        {
            if (debugConeAngle > 0)
            {
                Configuration.DebugConeAngle = debugConeAngle;
                Configuration.Save();
            }
        }

        var debugConeFollowTarget = Configuration.DebugConeFollowTarget;
        if (ImGui.Checkbox("Follow target", ref debugConeFollowTarget))
        {
            Configuration.DebugConeFollowTarget = debugConeFollowTarget;
            Configuration.Save();
        }
    }
}
