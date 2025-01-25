namespace PhlegmaDilemma.Windows;

internal class Rangefinder : Window , IDisposable
{
    private Plugin Plugin;
    private Configuration Configuration;
    public Rangefinder(Plugin plugin, Configuration config):base("Rangefinder##", ImGuiUtils.OverlayWindowFlags)
    {
        Plugin = plugin;
        Configuration = config;
    }

    public void Dispose()
    {

    }

    public override void Draw()
    {
        if (Configuration.EnableRangefinder == true)
        {
            DataDynamic data = Plugin.RetrieveData();
            float targetAngle = (float)Math.Atan2(data.TargetPosition.Z - data.PlayerPosition.Z, data.TargetPosition.X - data.PlayerPosition.X);
            Vector3 actionRangeEdgePoint = new Vector3(data.PlayerPosition.X + data.ActionRange * (float)Math.Cos(targetAngle), data.PlayerPosition.Y, data.PlayerPosition.Z + data.ActionRange * (float)Math.Sin(targetAngle));
            Vector3 actionRadiusEdgePoint = new Vector3(data.PlayerPosition.X + (data.ActionRadius + 0.5f) * (float)Math.Cos(targetAngle), data.PlayerPosition.Y, data.PlayerPosition.Z + (data.ActionRadius + 0.5f) * (float)Math.Sin(targetAngle));
            Vector3 targetHitboxEdgePoint = new Vector3(data.TargetPosition.X - data.TargetHitbox * (float)Math.Cos(targetAngle), data.PlayerPosition.Y, data.TargetPosition.Z - data.TargetHitbox * (float)Math.Sin(targetAngle));
            Vector3 playerHitboxEdgePoint = new Vector3(data.PlayerPosition.X - (-data.PlayerHitbox) * (float)Math.Cos(targetAngle), data.PlayerPosition.Y, data.PlayerPosition.Z - (-data.PlayerHitbox) * (float)Math.Sin(targetAngle));

            float focusTargetAngle = (float)Math.Atan2(data.FocusTargetPosition.Z - data.PlayerPosition.Z, data.FocusTargetPosition.X - data.PlayerPosition.X);
            Vector3 focusActionRangeEdgePoint = new Vector3(data.PlayerPosition.X + data.ActionRange * (float)Math.Cos(focusTargetAngle), data.PlayerPosition.Y, data.PlayerPosition.Z + data.ActionRange * (float)Math.Sin(focusTargetAngle));
            Vector3 focusHitboxEdgePoint = new Vector3(data.FocusTargetPosition.X - data.FocusTargetHitbox * (float)Math.Cos(focusTargetAngle), data.PlayerPosition.Y, data.FocusTargetPosition.Z - data.FocusTargetHitbox * (float)Math.Sin(focusTargetAngle));

            Vector3 playerFrontPoint = new Vector3(data.PlayerPosition.X + 1 * MathF.Sin(data.PlayerRotation), data.PlayerPosition.Y, data.PlayerPosition.Z + 1 * MathF.Cos(data.PlayerRotation));

            // Target pointer
            if (data.Target != null)
            {
                if (data.ActionRange >= data.DistanceToTarget2D || (data.ActionRange == 0 && data.ActionRadius >= data.DistanceToTarget2D))
                {
                    ImGui.GetForegroundDrawList().AddLine3D(
                    actionRangeEdgePoint,
                    targetHitboxEdgePoint,
                    Plugin.Configuration.ColorTargetPointerInRange,
                    Plugin.Configuration.Thickness);

                    ImGui.GetForegroundDrawList().AddPolycircle3D(
                    new Vector3(data.TargetPosition.X, data.PlayerPosition.Y, data.TargetPosition.Z),
                    data.TargetHitbox,
                    Plugin.Configuration.PointsNumber,
                    Plugin.Configuration.ColorTargetPointerInRange,
                    Plugin.Configuration.Thickness);

                    if (Configuration.EnableRuler == true)
                    {
                        ImGui.GetForegroundDrawList().AddScale3D(
                        playerHitboxEdgePoint,
                        targetHitboxEdgePoint,
                        data.DistanceToTarget2D,
                        data.PlayerHitbox,
                        1,
                        1,
                        Plugin.Configuration.ColorTargetPointerInRange,
                        Plugin.Configuration.Thickness);
                    }

                }
                else
                {
                    ImGui.GetForegroundDrawList().AddLine3D(
                    actionRangeEdgePoint,
                    targetHitboxEdgePoint,
                    Plugin.Configuration.ColorTargetPointerOutOfRange,
                    Plugin.Configuration.Thickness);

                    ImGui.GetForegroundDrawList().AddPolycircle3D(
                    new Vector3(data.TargetPosition.X, data.PlayerPosition.Y, data.TargetPosition.Z),
                    data.TargetHitbox,
                    Plugin.Configuration.PointsNumber,
                    Plugin.Configuration.ColorTargetPointerOutOfRange,
                    Plugin.Configuration.Thickness);

                    if (Configuration.EnableRuler == true)
                    {
                        ImGui.GetForegroundDrawList().AddScale3D(
                        playerHitboxEdgePoint,
                        targetHitboxEdgePoint,
                        data.DistanceToTarget2D,
                        data.PlayerHitbox,
                        1,
                        1,
                        Plugin.Configuration.ColorTargetPointerOutOfRange,
                        Plugin.Configuration.Thickness);
                    }
                }
            }

            // Focus target pointer
            if (data.FocusTarget != null)
            {
                if (data.ActionRange >= data.DistanceToFocusTarget2D || (data.ActionRange == 0 && data.ActionRadius >= data.DistanceToFocusTarget2D))
                {
                    ImGui.GetForegroundDrawList().AddLine3D(
                    focusActionRangeEdgePoint,
                    focusHitboxEdgePoint,
                    Plugin.Configuration.ColorTargetPointerInRange,
                    Plugin.Configuration.Thickness);

                    ImGui.GetForegroundDrawList().AddPolycircle3D(
                    new Vector3(data.FocusTargetPosition.X, data.PlayerPosition.Y, data.FocusTargetPosition.Z),
                    data.FocusTargetHitbox,
                    Plugin.Configuration.PointsNumber,
                    Plugin.Configuration.ColorTargetPointerInRange,
                    Plugin.Configuration.Thickness);
                }
                else
                {
                    ImGui.GetForegroundDrawList().AddLine3D(
                    focusActionRangeEdgePoint,
                    focusHitboxEdgePoint,
                    Plugin.Configuration.ColorTargetPointerOutOfRange,
                    Plugin.Configuration.Thickness);

                    ImGui.GetForegroundDrawList().AddPolycircle3D(
                    new Vector3(data.FocusTargetPosition.X, data.PlayerPosition.Y, data.FocusTargetPosition.Z),
                    data.FocusTargetHitbox,
                    Plugin.Configuration.PointsNumber,
                    Plugin.Configuration.ColorTargetPointerOutOfRange,
                    Plugin.Configuration.Thickness);
                }
            }

            // Action range
            ImGui.GetForegroundDrawList().AddPolycircle3D(
            data.PlayerPosition, 
            data.ActionRange, 
            Plugin.Configuration.PointsNumber,
            Plugin.Configuration.ColorActionRange, 
            Plugin.Configuration.Thickness);

            // Auto-attack range
            if (Configuration.EnableAutoAttackRange == true)
            {
                ImGui.GetForegroundDrawList().AddPolycircle3D(
                data.PlayerPosition, 
                data.PlayerAutoAttackRadius, 
                Plugin.Configuration.PointsNumber,
                Plugin.Configuration.ColorAutoAttack, 
                Plugin.Configuration.Thickness);
            }

            // Debug cone
            if (Configuration.EnableDebugCone == true)
            {
                if (Configuration.DebugConeFollowTarget == true)
                {
                    ImGui.GetForegroundDrawList().AddCone3D(
                    data.PlayerPosition,
                    data.TargetPosition,
                    Configuration.DebugConeRadius,
                    Plugin.Configuration.PointsNumber / 3,
                    Configuration.DebugConeAngle,
                    Plugin.Configuration.ColorActionRadius,
                    Plugin.Configuration.Thickness);
                }
                else
                {
                    ImGui.GetForegroundDrawList().AddCone3D(
                    data.PlayerPosition,
                    playerFrontPoint,
                    Configuration.DebugConeRadius,
                    Plugin.Configuration.PointsNumber / 3,
                    Configuration.DebugConeAngle,
                    Plugin.Configuration.ColorActionRadius,
                    Plugin.Configuration.Thickness);
                }
            }

            // AoE shape
            switch (data.CastType)
            {
                case 2: // Circle AoE
                    if (data.CanTargetEnemy == true)
                    {
                        if (data.Target != null)
                        {
                            ImGui.GetForegroundDrawList().AddPolycircle3D(
                            new Vector3(data.TargetPosition.X, data.PlayerPosition.Y, data.TargetPosition.Z), 
                            data.ActionRadius,
                            Plugin.Configuration.PointsNumber, 
                            Plugin.Configuration.ColorActionRadius, 
                            Plugin.Configuration.Thickness);
                        }
                    }
                    else
                    {
                        ImGui.GetForegroundDrawList().AddPolycircle3D(
                        data.PlayerPosition, 
                        data.ActionRadius, 
                        Plugin.Configuration.PointsNumber,
                        Plugin.Configuration.ColorActionRadius, 
                        Plugin.Configuration.Thickness);
                    }
                    break;

                case 3: // Cone AoE
                    if (data.Target != null && data.CanTargetEnemy == true)
                    {
                        ImGui.GetForegroundDrawList().AddCone3D(
                        data.PlayerPosition, 
                        data.TargetPosition, 
                        data.ActionRadius + 0.5f,
                        Plugin.Configuration.PointsNumber / 3, 
                        data.ActionAngle,
                        Plugin.Configuration.ColorActionRadius, 
                        Plugin.Configuration.Thickness);
                    }
                    else if (data.CanTargetEnemy == false) // Some cone actions don't need to have a target to use it.
                    {                                                                                           
                        ImGui.GetForegroundDrawList().AddCone3D(
                        data.PlayerPosition, 
                        playerFrontPoint, 
                        data.ActionRadius + 0.5f, 
                        Plugin.Configuration.PointsNumber / 3, 
                        data.ActionAngle,
                        Plugin.Configuration.ColorActionRadius, 
                        Plugin.Configuration.Thickness);
                    }
                    break;

                case 4: // Line AoE (square)
                    if (data.Target != null)
                    {
                        ImGui.GetForegroundDrawList().AddSquare3D(
                            data.PlayerPosition, 
                            data.TargetPosition, 
                            actionRadiusEdgePoint, 
                            data.CastWidth,
                            Plugin.Configuration.ColorActionRadius, 
                            Plugin.Configuration.Thickness);
                    }
                    break;
                case 7: // Placeables
                    ImGui.GetForegroundDrawList().AddPolycircle3D(
                    data.MousePosition,
                    data.ActionRadius,
                    Plugin.Configuration.PointsNumber,
                    Plugin.Configuration.ColorActionRadius,
                    Plugin.Configuration.Thickness);
                    break;

                default:
                    break;
            }
        }
    }
}
