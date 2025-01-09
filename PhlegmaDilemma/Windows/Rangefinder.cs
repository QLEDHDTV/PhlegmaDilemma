namespace PhlegmaDilemma.Windows;

internal class Rangefinder : Window , IDisposable
{
    private Plugin Plugin;
    public Rangefinder(Plugin plugin):base("Rangefinder##", ImGuiUtils.OverlayWindowFlags)
    {
        Plugin = plugin;
    }

    public void Dispose()
    {

    }

    public override void Draw()
    {
        DataDynamic data = Plugin.RetrieveData();
        float targetAngle = (float)Math.Atan2(data.TargetPosition.Z - data.PlayerPosition.Z, data.TargetPosition.X - data.PlayerPosition.X);
        Vector3 actionRangeEdgePoint = new Vector3(data.PlayerPosition.X + data.ActionRange * (float)Math.Cos(targetAngle), data.PlayerPosition.Y, data.PlayerPosition.Z + data.ActionRange * (float)Math.Sin(targetAngle));
        Vector3 actionRadiusEdgePoint = new Vector3(data.PlayerPosition.X + (data.ActionRadius + 0.5f) * (float)Math.Cos(targetAngle), data.PlayerPosition.Y, data.PlayerPosition.Z + (data.ActionRadius + 0.5f) * (float)Math.Sin(targetAngle));
        Vector3 targetHitboxEdgePoint = new Vector3(data.TargetPosition.X - data.TargetHitbox * (float)Math.Cos(targetAngle), data.PlayerPosition.Y, data.TargetPosition.Z - data.TargetHitbox * (float)Math.Sin(targetAngle));

        float focusTargetAngle = (float)Math.Atan2(data.FocusTargetPosition.Z - data.PlayerPosition.Z, data.FocusTargetPosition.X - data.PlayerPosition.X);
        Vector3 focusActionRangeEdgePoint = new Vector3(data.PlayerPosition.X + data.ActionRange * (float)Math.Cos(focusTargetAngle), data.PlayerPosition.Y, data.PlayerPosition.Z + data.ActionRange * (float)Math.Sin(focusTargetAngle));
        Vector3 focusHitboxEdgePoint = new Vector3(data.FocusTargetPosition.X - data.FocusTargetHitbox * (float)Math.Cos(focusTargetAngle), data.PlayerPosition.Y, data.FocusTargetPosition.Z - data.FocusTargetHitbox * (float)Math.Sin(focusTargetAngle));


        // Target pointer
        if (data.Target != null)
        {
            ImGui.GetForegroundDrawList().AddLine3D(actionRangeEdgePoint, targetHitboxEdgePoint, Plugin.Configuration.ColorTargetPointer, Plugin.Configuration.Thickness);
            ImGui.GetForegroundDrawList().AddPolycircle3D(new Vector3(data.TargetPosition.X, data.PlayerPosition.Y, data.TargetPosition.Z), data.TargetHitbox, Plugin.Configuration.PointsNumber, Plugin.Configuration.ColorTargetPointer, Plugin.Configuration.Thickness);
        }

        // Focus target pointer
        if (data.FocusTarget != null)
        {
            ImGui.GetForegroundDrawList().AddLine3D(focusActionRangeEdgePoint, focusHitboxEdgePoint, Plugin.Configuration.ColorTargetPointer, Plugin.Configuration.Thickness);
            ImGui.GetForegroundDrawList().AddPolycircle3D(new Vector3(data.FocusTargetPosition.X, data.PlayerPosition.Y, data.FocusTargetPosition.Z), data.FocusTargetHitbox, Plugin.Configuration.PointsNumber, Plugin.Configuration.ColorTargetPointer, Plugin.Configuration.Thickness);
        }

        // Action range
        ImGui.GetForegroundDrawList().AddPolycircle3D(
        data.PlayerPosition, data.ActionRange, Plugin.Configuration.PointsNumber,
        Plugin.Configuration.ColorActionRange, Plugin.Configuration.Thickness);

        // Auto-attack range (~3.6 yalms)
        ImGui.GetForegroundDrawList().AddPolycircle3D(
        data.PlayerPosition, 3.6f, Plugin.Configuration.PointsNumber,
        Plugin.Configuration.ColorAutoAttack, Plugin.Configuration.Thickness);
        
        // AoE shape
        switch (data.CastType)
        {
            case 2: // Circle AoE
                if (data.CanTargetEnemy == true)
                {
                    if (data.Target != null)
                    {
                        ImGui.GetForegroundDrawList().AddPolycircle3D(
                        data.TargetPosition, data.ActionRadius, Plugin.Configuration.PointsNumber,
                        Plugin.Configuration.ColorActionRadius, Plugin.Configuration.Thickness);
                    }
                }
                else
                {
                    ImGui.GetForegroundDrawList().AddPolycircle3D(
                    data.PlayerPosition, data.ActionRadius, Plugin.Configuration.PointsNumber,
                    Plugin.Configuration.ColorActionRadius, Plugin.Configuration.Thickness);
                }
                break;

            case 3: // Cone AoE
                if (data.Target != null)
                {
                    ImGui.GetForegroundDrawList().AddCone3D(
                    data.PlayerPosition, data.TargetPosition, data.ActionRadius + 0.5f,                     // From what i seen, the cones attack have a 120 degrees
                    Plugin.Configuration.PointsNumber / 3, 120,                                             // cones. Maybe there are exceptions? Need to look more into it.
                    Plugin.Configuration.ColorActionRadius, Plugin.Configuration.Thickness);
                }
                break;

            case 4: // Line AoE (square)
                if (data.Target != null)
                {
                    ImGui.GetForegroundDrawList().AddSquare3D(
                        data.PlayerPosition, data.TargetPosition, actionRadiusEdgePoint, data.CastWidth,
                        Plugin.Configuration.ColorActionRadius, Plugin.Configuration.Thickness);
                }
                break;

            default:
                break;
        }
    }
}
