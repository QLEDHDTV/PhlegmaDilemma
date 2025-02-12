namespace PhlegmaDilemma.Windows;

internal class Rangefinder : Window , IDisposable
{
    private Plugin Plugin;
    private Configuration Configuration;
    private static System.Timers.Timer timer;
    private CancellationTokenSource cts;
    private readonly object lockObject = new();
    private bool TimerState = false;

    // Use local colors to prevent fading changing original colors
    private Vector4 localColorActionRange;
    private Vector4 localColorActionRadius;
    private Vector4 localColorAutoAttack;
    private Vector4 localColorTargetPointerOutOfRange;
    private Vector4 localColorTargetPointerInRange;

    public Rangefinder(Plugin plugin, Configuration config) :base("Rangefinder##", ImGuiUtils.OverlayWindowFlags)
    {
        Plugin = plugin;
        Configuration = config;
    }

    public void UpdateColors()
    {
        localColorActionRange = Configuration.ColorActionRange;
        localColorActionRadius = Configuration.ColorActionRadius;
        localColorAutoAttack = Configuration.ColorAutoAttack;
        localColorTargetPointerOutOfRange = Configuration.ColorTargetPointerOutOfRange;
        localColorTargetPointerInRange = Configuration.ColorTargetPointerInRange;
    }

    public void Dispose()
    {
        if (timer != null)
        {
            timer.Dispose();
        }
    }

    public override void Draw()
    {
        if (Configuration.EnableRangefinder == true)
        {
            if (Configuration.EnableFadeOut == true && TimerState == false)
            {
                InitializeTimer(Configuration.FadeOutDelay * 1000);
                TimerState = true;
            }
            else if (Configuration.EnableFadeOut == false && TimerState == true)
            {
                if (timer != null)
                {
                    timer.Dispose();
                }
                TimerState = false;
                localColorActionRange.W = Configuration.ColorActionRange.W;
                localColorActionRadius.W = Configuration.ColorActionRadius.W;
                localColorAutoAttack.W = Configuration.ColorAutoAttack.W;
                localColorTargetPointerOutOfRange.W = Configuration.ColorTargetPointerOutOfRange.W;
                localColorTargetPointerInRange.W = Configuration.ColorTargetPointerInRange.W;
            }

            DataDynamic data = Plugin.RetrieveData();
            float targetAngle = (float)Math.Atan2(data.TargetPosition.Z - data.PlayerPosition.Z, data.TargetPosition.X - data.PlayerPosition.X);
            Vector3 actionEdgeLineLeft = Vector3.Zero, actionEdgeLineRight = Vector3.Zero, actionRadiusEdgeLineLeft = Vector3.Zero, actionRadiusEdgeLineRight = Vector3.Zero;


            Vector3 actionRangeEdgePoint = new Vector3 (data.PlayerPosition.X + data.ActionRange * (float)Math.Cos(targetAngle), 
                                                        data.PlayerPosition.Y, 
                                                        data.PlayerPosition.Z + data.ActionRange * (float)Math.Sin(targetAngle));

            Vector3 actionRadiusEdgePoint = new Vector3(data.PlayerPosition.X + (data.ActionRadius + 0.5f) * (float)Math.Cos(targetAngle), 
                                                        data.PlayerPosition.Y, 
                                                        data.PlayerPosition.Z + (data.ActionRadius + 0.5f) * (float)Math.Sin(targetAngle));

            Vector3 targetHitboxEdgePoint = new Vector3(data.TargetPosition.X - data.TargetHitbox * (float)Math.Cos(targetAngle), 
                                                        data.PlayerPosition.Y, 
                                                        data.TargetPosition.Z - data.TargetHitbox * (float)Math.Sin(targetAngle));

            Vector3 playerHitboxEdgePoint = new Vector3(data.PlayerPosition.X - (-data.PlayerHitbox) * (float)Math.Cos(targetAngle), 
                                                        data.PlayerPosition.Y, 
                                                        data.PlayerPosition.Z - (-data.PlayerHitbox) * (float)Math.Sin(targetAngle));


            float focusTargetAngle = (float)Math.Atan2(data.FocusTargetPosition.Z - data.PlayerPosition.Z, data.FocusTargetPosition.X - data.PlayerPosition.X);

            Vector3 focusActionRangeEdgePoint = new Vector3(data.PlayerPosition.X + data.ActionRange * (float)Math.Cos(focusTargetAngle), 
                                                            data.PlayerPosition.Y, 
                                                            data.PlayerPosition.Z + data.ActionRange * (float)Math.Sin(focusTargetAngle));

            Vector3 focusHitboxEdgePoint = new Vector3 (data.FocusTargetPosition.X - data.FocusTargetHitbox * (float)Math.Cos(focusTargetAngle), 
                                                        data.PlayerPosition.Y, 
                                                        data.FocusTargetPosition.Z - data.FocusTargetHitbox * (float)Math.Sin(focusTargetAngle));


            Vector3 playerFrontPoint = new Vector3 (data.PlayerPosition.X + (data.ActionRadius + data.PlayerHitbox) * MathF.Sin(data.PlayerRotation), 
                                                    data.PlayerPosition.Y, 
                                                    data.PlayerPosition.Z + (data.ActionRadius + data.PlayerHitbox) * MathF.Cos(data.PlayerRotation));

            float anglePlayerToRadius = (float)Math.Atan2(actionRadiusEdgePoint.X - data.PlayerPosition.X, actionRadiusEdgePoint.Z - data.PlayerPosition.Z);
            if (data.CastType == 3)
            {
                if (data.CanTargetEnemy == true)
                {
                    actionEdgeLineLeft = new Vector3(data.PlayerPosition.X + data.ActionRange * (float)Math.Cos(targetAngle - (Math.PI / 180 * (data.ActionAngle / 2))),
                                                     data.PlayerPosition.Y,
                                                     data.PlayerPosition.Z + data.ActionRange * (float)Math.Sin(targetAngle - (Math.PI / 180 * (data.ActionAngle / 2))));

                    actionEdgeLineRight = new Vector3(data.PlayerPosition.X + data.ActionRange * (float)Math.Cos(targetAngle + (Math.PI / 180 * (data.ActionAngle / 2))),
                                                      data.PlayerPosition.Y,
                                                      data.PlayerPosition.Z + data.ActionRange * (float)Math.Sin(targetAngle + (Math.PI / 180 * (data.ActionAngle / 2))));
                }
                else // TODO: Find the reason why angle calculation is reversed when using data.PlayerRotation
                {
                    actionEdgeLineLeft = new Vector3(data.PlayerPosition.X + (data.ActionRadius + data.PlayerHitbox) * (float)Math.Sin(data.PlayerRotation - (Math.PI / 180 * (data.ActionAngle / 2))),
                                                     data.PlayerPosition.Y,
                                                     data.PlayerPosition.Z + (data.ActionRadius + data.PlayerHitbox) * (float)Math.Cos(data.PlayerRotation - (Math.PI / 180 * (data.ActionAngle / 2))));

                    actionEdgeLineRight = new Vector3(data.PlayerPosition.X + (data.ActionRadius + data.PlayerHitbox) * (float)Math.Sin(data.PlayerRotation + (Math.PI / 180 * (data.ActionAngle / 2))),
                                                      data.PlayerPosition.Y,
                                                      data.PlayerPosition.Z + (data.ActionRadius + data.PlayerHitbox) * (float)Math.Cos(data.PlayerRotation + (Math.PI / 180 * (data.ActionAngle / 2))));
                }
            }
            else if (data.CastType == 4)
            {   
                actionEdgeLineLeft = new Vector3(data.PlayerPosition.X + (data.CastWidth / 2) * (float)Math.Cos(anglePlayerToRadius),
                                                 data.PlayerPosition.Y,
                                                 data.PlayerPosition.Z - (data.CastWidth / 2) * (float)Math.Sin(anglePlayerToRadius));

                actionRadiusEdgeLineLeft = new Vector3(actionRadiusEdgePoint.X + (data.CastWidth / 2) * (float)Math.Cos(anglePlayerToRadius),
                                                       data.PlayerPosition.Y,
                                                       actionRadiusEdgePoint.Z - (data.CastWidth / 2) * (float)Math.Sin(anglePlayerToRadius));

                actionEdgeLineRight = new Vector3(data.PlayerPosition.X - (data.CastWidth / 2) * (float)Math.Cos(anglePlayerToRadius),
                                                  data.PlayerPosition.Y,
                                                  data.PlayerPosition.Z + (data.CastWidth / 2) * (float)Math.Sin(anglePlayerToRadius));

                actionRadiusEdgeLineRight = new Vector3(actionRadiusEdgePoint.X - (data.CastWidth / 2) * (float)Math.Cos(anglePlayerToRadius),
                                                        data.PlayerPosition.Y,
                                                        actionRadiusEdgePoint.Z + (data.CastWidth / 2) * (float)Math.Sin(anglePlayerToRadius));
            }

            // Target pointer
            if (data.Target != null)
            {
                if (data.ActionRange >= data.DistanceToTarget2D || (data.ActionRange == 0 && data.ActionRadius >= data.DistanceToTarget2D))
                {
                    if (Configuration.EnableRuler == true)
                    {
                        if (Configuration.RulerStartPointAtPlayer)
                        {
                            ImGui.GetForegroundDrawList().AddScale3D(
                            data.PlayerPosition,
                            targetHitboxEdgePoint,
                            data.DistanceToTarget2D,
                            0,
                            Configuration.RulerRange,
                            1,
                            localColorTargetPointerInRange,
                            Plugin.Configuration.Thickness);

                            ImGui.GetForegroundDrawList().AddLine3D(
                            data.PlayerPosition,
                            targetHitboxEdgePoint,
                            localColorTargetPointerInRange,
                            Plugin.Configuration.Thickness);

                            if (Configuration.EnableRulerText == true)
                            {
                                ImGui.GetForegroundDrawList().AddScaleText3D(
                                data.PlayerPosition,
                                targetHitboxEdgePoint,
                                data.DistanceToTarget2D,
                                0,
                                Configuration.RulerTextOffset,
                                Configuration.RulerRange,
                                1,
                                localColorTargetPointerInRange,
                                Configuration.RulerTextFrequency,
                                Configuration.RulerTexLefttSide,
                                Configuration.RulerTextSize);
                            }
                        }
                        else
                        {
                            ImGui.GetForegroundDrawList().AddScale3D(
                            playerHitboxEdgePoint,
                            targetHitboxEdgePoint,
                            data.DistanceToTarget2D,
                            data.PlayerHitbox,
                            Configuration.RulerRange,
                            1,
                            localColorTargetPointerInRange,
                            Plugin.Configuration.Thickness);

                            ImGui.GetForegroundDrawList().AddLine3D(
                            playerHitboxEdgePoint,
                            targetHitboxEdgePoint,
                            localColorTargetPointerInRange,
                            Plugin.Configuration.Thickness);

                            if (Configuration.EnableRulerText == true)
                            {
                                ImGui.GetForegroundDrawList().AddScaleText3D(
                                playerHitboxEdgePoint,
                                targetHitboxEdgePoint,
                                data.DistanceToTarget2D,
                                data.PlayerHitbox,
                                Configuration.RulerTextOffset,
                                Configuration.RulerRange,
                                1,
                                localColorTargetPointerInRange,
                                Configuration.RulerTextFrequency,
                                Configuration.RulerTexLefttSide,
                                Configuration.RulerTextSize);
                            }
                        }
                    }
                    else
                    {
                        ImGui.GetForegroundDrawList().AddLine3D(
                        actionRangeEdgePoint,
                        targetHitboxEdgePoint,
                        localColorTargetPointerInRange,
                        Plugin.Configuration.Thickness);

                    }

                    ImGui.GetForegroundDrawList().AddPolycircle3D(
                    new Vector3(data.TargetPosition.X, data.PlayerPosition.Y, data.TargetPosition.Z),
                    data.TargetHitbox,
                    Plugin.Configuration.PointsNumber,
                    localColorTargetPointerInRange,
                    Plugin.Configuration.Thickness);

                }
                else
                {
                    if (Configuration.EnableRuler == true)
                    {
                        if (Configuration.RulerStartPointAtPlayer)
                        {
                            ImGui.GetForegroundDrawList().AddScale3D(
                            data.PlayerPosition,
                            targetHitboxEdgePoint,
                            data.DistanceToTarget2D,
                            0,
                            Configuration.RulerRange,
                            1,
                            localColorTargetPointerOutOfRange,
                            Plugin.Configuration.Thickness);

                            ImGui.GetForegroundDrawList().AddLine3D(
                            data.PlayerPosition,
                            targetHitboxEdgePoint,
                            localColorTargetPointerOutOfRange,
                            Plugin.Configuration.Thickness);

                            if (Configuration.EnableRulerText == true)
                            {
                                ImGui.GetForegroundDrawList().AddScaleText3D(
                                data.PlayerPosition,
                                targetHitboxEdgePoint,
                                data.DistanceToTarget2D,
                                0,
                                Configuration.RulerTextOffset,
                                Configuration.RulerRange,
                                1,
                                localColorTargetPointerOutOfRange,
                                Configuration.RulerTextFrequency,
                                Configuration.RulerTexLefttSide,
                                Configuration.RulerTextSize);
                            }
                        }
                        else
                        {
                            ImGui.GetForegroundDrawList().AddScale3D(
                            playerHitboxEdgePoint,
                            targetHitboxEdgePoint,
                            data.DistanceToTarget2D,
                            data.PlayerHitbox,
                            Configuration.RulerRange,
                            1,
                            localColorTargetPointerOutOfRange,
                            Plugin.Configuration.Thickness);

                            ImGui.GetForegroundDrawList().AddLine3D(
                            playerHitboxEdgePoint,
                            targetHitboxEdgePoint,
                            localColorTargetPointerOutOfRange,
                            Plugin.Configuration.Thickness);

                            if (Configuration.EnableRulerText == true)
                            {
                                ImGui.GetForegroundDrawList().AddScaleText3D(
                                playerHitboxEdgePoint,
                                targetHitboxEdgePoint,
                                data.DistanceToTarget2D,
                                data.PlayerHitbox,
                                Configuration.RulerTextOffset,
                                Configuration.RulerRange,
                                1,
                                localColorTargetPointerOutOfRange,
                                Configuration.RulerTextFrequency,
                                Configuration.RulerTexLefttSide,
                                Configuration.RulerTextSize);
                            }
                        }
                    }
                    else
                    {
                        ImGui.GetForegroundDrawList().AddLine3D(
                        actionRangeEdgePoint,
                        targetHitboxEdgePoint,
                        localColorTargetPointerOutOfRange,
                        Plugin.Configuration.Thickness);
                    }
                    ImGui.GetForegroundDrawList().AddPolycircle3D(
                    new Vector3(data.TargetPosition.X, data.PlayerPosition.Y, data.TargetPosition.Z),
                    data.TargetHitbox,
                    Plugin.Configuration.PointsNumber,
                    localColorTargetPointerOutOfRange,
                    Plugin.Configuration.Thickness);
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
                    localColorTargetPointerInRange,
                    Plugin.Configuration.Thickness);

                    ImGui.GetForegroundDrawList().AddPolycircle3D(
                    new Vector3(data.FocusTargetPosition.X, data.PlayerPosition.Y, data.FocusTargetPosition.Z),
                    data.FocusTargetHitbox,
                    Plugin.Configuration.PointsNumber,
                    localColorTargetPointerInRange,
                    Plugin.Configuration.Thickness);
                }
                else
                {
                    ImGui.GetForegroundDrawList().AddLine3D(
                    focusActionRangeEdgePoint,
                    focusHitboxEdgePoint,
                    localColorTargetPointerOutOfRange,
                    Plugin.Configuration.Thickness);

                    ImGui.GetForegroundDrawList().AddPolycircle3D(
                    new Vector3(data.FocusTargetPosition.X, data.PlayerPosition.Y, data.FocusTargetPosition.Z),
                    data.FocusTargetHitbox,
                    Plugin.Configuration.PointsNumber,
                    localColorTargetPointerOutOfRange,
                    Plugin.Configuration.Thickness);
                }
            }

            // Action range
            ImGui.GetForegroundDrawList().AddPolycircle3D(
            data.PlayerPosition,
            data.ActionRange,
            Plugin.Configuration.PointsNumber,
            localColorActionRange,
            Plugin.Configuration.Thickness);

            // Auto-attack range
            if (Configuration.EnableAutoAttackRange == true)
            {
                ImGui.GetForegroundDrawList().AddPolycircle3D(
                data.PlayerPosition,
                data.PlayerAutoAttackRadius,
                Plugin.Configuration.PointsNumber,
                localColorAutoAttack,
                Plugin.Configuration.Thickness);
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
                            localColorActionRadius,
                            Plugin.Configuration.Thickness);
                        }
                    }
                    else
                    {
                        ImGui.GetForegroundDrawList().AddPolycircle3D(
                        data.PlayerPosition,
                        data.ActionRadius,
                        Plugin.Configuration.PointsNumber,
                        localColorActionRadius,
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
                        localColorActionRadius,
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
                        localColorActionRadius,
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
                            localColorActionRadius,
                            Plugin.Configuration.Thickness);
                    }
                    break;
                case 7: // Placeables
                    ImGui.GetForegroundDrawList().AddPolycircle3D(
                    data.MousePosition,
                    data.ActionRadius,
                    Plugin.Configuration.PointsNumber,
                    localColorActionRadius,
                    Plugin.Configuration.Thickness);
                    break;

                default:
                    break;
            }

            // Pointers for targets that are in radius of an action
            if (data.CastType != 1)
            {
                switch (data.CastType, data.DamagingAction, data.CanTargetEnemy)
                {
                    case (2, true, true): // Circle AoE on target
                        foreach (var potentialTarget in data.InRangeEnemyTargets)
                        {
                            if (data.TargetPosition.Distance2D(potentialTarget.Position) <= data.ActionRadius + potentialTarget.HitboxRadius)
                            {
                                ImGui.GetForegroundDrawList().AddPolycircle3D(
                                new Vector3(potentialTarget.Position.X, data.PlayerPosition.Y, potentialTarget.Position.Z),
                                potentialTarget.HitboxRadius,
                                Plugin.Configuration.PointsNumber,
                                localColorTargetPointerInRange,
                                Plugin.Configuration.Thickness);
                            }
                        }
                        break;

                    case (2, true, false): // Circle AoE around the player
                        foreach (var potentialTarget in data.InRangeEnemyTargets)
                        {
                            if (data.PlayerPosition.Distance2D(potentialTarget.Position) <= data.ActionRadius + potentialTarget.HitboxRadius)
                            {
                                ImGui.GetForegroundDrawList().AddPolycircle3D(
                                new Vector3(potentialTarget.Position.X, data.PlayerPosition.Y, potentialTarget.Position.Z),
                                potentialTarget.HitboxRadius,
                                Plugin.Configuration.PointsNumber,
                                localColorTargetPointerInRange,
                                Plugin.Configuration.Thickness);
                            }
                        }
                        break;

                    case (2, false, false): // Circle AoE for healing actions
                        foreach (var potentialTarget in data.InRangeChars)
                        {
                            if (data.PlayerPosition.Distance2D(potentialTarget.Position) <= data.ActionRadius + potentialTarget.HitboxRadius)
                            {
                                ImGui.GetForegroundDrawList().AddPolycircle3D(
                                new Vector3(potentialTarget.Position.X, data.PlayerPosition.Y, potentialTarget.Position.Z),
                                potentialTarget.HitboxRadius,
                                Plugin.Configuration.PointsNumber,
                                localColorTargetPointerInRange,
                                Plugin.Configuration.Thickness);
                            }
                        }
                        break;

                    case (3, true, true): // Cone AoE on target
                        foreach (var potentialTarget in data.InRangeEnemyTargets)
                        {
                            if (data.Target != null && data.PlayerPosition.Distance2D(potentialTarget.Position) <= data.ActionRange + potentialTarget.HitboxRadius)
                            {
                                float rightSide = 0;
                                float leftSide = 0;
                                Vector3 shortestFromPotToEdgeRight = Vector3Exstensions.ShortestLine2D(data.PlayerPosition, actionEdgeLineRight, potentialTarget.Position, out shortestFromPotToEdgeRight, out rightSide);
                                Vector3 shortestFromPotToEdgeLeft = Vector3Exstensions.ShortestLine2D(data.PlayerPosition, actionEdgeLineLeft, potentialTarget.Position, out shortestFromPotToEdgeLeft, out leftSide);

                                if ((rightSide <= 0 && leftSide >= 0) || shortestFromPotToEdgeLeft.Distance2D(potentialTarget.Position) <= potentialTarget.HitboxRadius
                                 || shortestFromPotToEdgeRight.Distance2D(potentialTarget.Position) <= potentialTarget.HitboxRadius)
                                {
                                    ImGui.GetForegroundDrawList().AddPolycircle3D(
                                    new Vector3(potentialTarget.Position.X, data.PlayerPosition.Y, potentialTarget.Position.Z),
                                    potentialTarget.HitboxRadius,
                                    Plugin.Configuration.PointsNumber,
                                    localColorTargetPointerInRange,
                                    Plugin.Configuration.Thickness);
                                }
                            }
                        }
                        break;

                    case (3, true, false): // Cone AoE without target (player direction)
                        foreach (var potentialTarget in data.InRangeEnemyTargets)
                        {
                            if (data.PlayerPosition.Distance2D(potentialTarget.Position) <= data.ActionRadius + data.PlayerHitbox + potentialTarget.HitboxRadius)
                            {
                                float rightSide = 0;
                                float leftSide = 0;
                                Vector3 shortestFromPotToEdgeRight = Vector3Exstensions.ShortestLine2D(data.PlayerPosition, actionEdgeLineRight, potentialTarget.Position, out shortestFromPotToEdgeRight, out rightSide);
                                Vector3 shortestFromPotToEdgeLeft = Vector3Exstensions.ShortestLine2D(data.PlayerPosition, actionEdgeLineLeft, potentialTarget.Position, out shortestFromPotToEdgeLeft, out leftSide);

                                // Comparison is swapped due to the reversed angle calculation (TODO in actionConeEdge)
                                if ((rightSide >= 0 && leftSide <= 0) 
                                    || shortestFromPotToEdgeLeft.Distance2D(potentialTarget.Position) <= potentialTarget.HitboxRadius
                                    || shortestFromPotToEdgeRight.Distance2D(potentialTarget.Position) <= potentialTarget.HitboxRadius)
                                {
                                    ImGui.GetForegroundDrawList().AddPolycircle3D(
                                    new Vector3(potentialTarget.Position.X, data.PlayerPosition.Y, potentialTarget.Position.Z),
                                    potentialTarget.HitboxRadius,
                                    Plugin.Configuration.PointsNumber,
                                    localColorTargetPointerInRange,
                                    Plugin.Configuration.Thickness);
                                }
                            }
                        }
                        break;

                    case (4, true, true): // Straight line/square AoE on target
                        foreach (var potentialTarget in data.InRangeEnemyTargets)
                        {
                            if (data.Target != null && data.PlayerPosition.Distance2D(potentialTarget.Position) <= data.ActionRange + potentialTarget.HitboxRadius)
                            {
                                float leftSide = 0;
                                float rightSide = 0;
                                float centerSide = 0;
                                Vector3 shortestFromPotToEdgeRight = Vector3Exstensions.ShortestLine2D(actionEdgeLineRight, actionRadiusEdgeLineRight, potentialTarget.Position, out shortestFromPotToEdgeRight, out rightSide);
                                Vector3 shortestFromPotToEdgeLeft = Vector3Exstensions.ShortestLine2D(actionEdgeLineLeft, actionRadiusEdgeLineLeft, potentialTarget.Position, out shortestFromPotToEdgeLeft, out leftSide);
                                Vector3 shortestFromLeftToRightEdge = Vector3Exstensions.ShortestLine2D(actionEdgeLineLeft, actionEdgeLineRight, potentialTarget.Position, out shortestFromLeftToRightEdge, out centerSide);
                                // ShotestFromLeftToRightEdge is used to fix the edge case where the target is in between the lines but outside of the square (behind)

                                if ((rightSide <= 0 && leftSide >= 0 && centerSide <= 0) 
                                    || shortestFromPotToEdgeLeft.Distance2D(potentialTarget.Position) <= potentialTarget.HitboxRadius
                                    || shortestFromPotToEdgeRight.Distance2D(potentialTarget.Position) <= potentialTarget.HitboxRadius 
                                    || shortestFromLeftToRightEdge.Distance2D(potentialTarget.Position) <= potentialTarget.HitboxRadius)
                                {
                                    ImGui.GetForegroundDrawList().AddPolycircle3D(
                                    new Vector3(potentialTarget.Position.X, data.PlayerPosition.Y, potentialTarget.Position.Z),
                                    potentialTarget.HitboxRadius,
                                    Plugin.Configuration.PointsNumber,
                                    localColorTargetPointerInRange,
                                    Plugin.Configuration.Thickness);
                                }
                            }
                        }
                        break;

                    case (7, false, _):
                        foreach (var potentialTarget in data.InRangeChars)
                        {
                            if (data.MousePosition.Distance2D(potentialTarget.Position) <= data.ActionRadius + potentialTarget.HitboxRadius)
                            {
                                ImGui.GetForegroundDrawList().AddPolycircle3D(
                                new Vector3(potentialTarget.Position.X, data.PlayerPosition.Y, potentialTarget.Position.Z),
                                potentialTarget.HitboxRadius,
                                Plugin.Configuration.PointsNumber,
                                localColorTargetPointerInRange,
                                Plugin.Configuration.Thickness);
                            }
                        }
                        break;
                }

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
                    Plugin.Configuration.PointsNumber,
                    Configuration.DebugConeAngle,
                    Configuration.DebugConeColor,
                    Plugin.Configuration.Thickness);
                }
                else
                {
                    ImGui.GetForegroundDrawList().AddCone3D(
                    data.PlayerPosition,
                    playerFrontPoint,
                    Configuration.DebugConeRadius,
                    Plugin.Configuration.PointsNumber,
                    Configuration.DebugConeAngle,
                    Configuration.DebugConeColor,
                    Plugin.Configuration.Thickness);
                }
            }
        }
    }

    private void InitializeTimer(float timeMS)
    {
        timer = new System.Timers.Timer(timeMS);
        timer.Elapsed += TimerElapsed;
        timer.AutoReset = false;
        timer.Start();
    }

    private async void TimerElapsed(object sender, ElapsedEventArgs e)
    {
        lock (lockObject)
        {
            // Create a cancelation token to stop the fade out process
            cts?.Cancel();
            cts?.Dispose();
            cts = new CancellationTokenSource();
        }

        var token = cts.Token;
        try
        {
            await Task.Run(() => FadeOutColors(token), token);
        }
        catch (OperationCanceledException) { }
    }

    private void FadeOutColors(CancellationToken token)
    {
        float fadeOutStrength = Configuration.FadeOutSpeed;
        for (float i = 255; i > 0; i -= fadeOutStrength)
        {
            token.ThrowIfCancellationRequested();
            if (localColorActionRange.W > 0x00 / 255f) { localColorActionRange.W = (localColorActionRange.W * 255f - fadeOutStrength) / 255f; }
            if (localColorActionRadius.W > 0x00 / 255f) { localColorActionRadius.W = (localColorActionRadius.W * 255f - fadeOutStrength) / 255f; }
            if (localColorAutoAttack.W > 0x00 / 255f) { localColorAutoAttack.W = (localColorAutoAttack.W * 255f - fadeOutStrength) / 255f; }
            if (localColorTargetPointerOutOfRange.W > 0x00 / 255f) { localColorTargetPointerOutOfRange.W = (localColorTargetPointerOutOfRange.W * 255f - fadeOutStrength) / 255f; }
            if (localColorTargetPointerInRange.W > 0x00 / 255f) { localColorTargetPointerInRange.W = (localColorTargetPointerInRange.W * 255f - fadeOutStrength) / 255f; }
            Thread.Sleep(10);
        }
    }

    public void ResetTimer()
    {
        lock (lockObject)
        {
            // Canceling for loop when reseting the timer to make sure that fade outs wont overlap
            cts?.Cancel();
            cts?.Dispose();
            cts = null; // Need to keep null so the next timer cancelation token can be created
        }

        localColorActionRange = Configuration.ColorActionRange;
        localColorActionRadius = Configuration.ColorActionRadius;
        localColorAutoAttack = Configuration.ColorAutoAttack;
        localColorTargetPointerOutOfRange = Configuration.ColorTargetPointerOutOfRange;
        localColorTargetPointerInRange = Configuration.ColorTargetPointerInRange;

        if (timer != null)
        {
            timer.Stop();
            timer.Start();
        }
    }
}
