namespace PhlegmaDilemma.Settings;

internal struct DataDynamic
{
    internal IGameObject Target { get; set; }
    internal Vector3 TargetPosition { get; set; }
    internal float TargetHitbox { get; set; }
    internal IGameObject FocusTarget { get; set; }
    internal Vector3 FocusTargetPosition { get; set; }
    internal float FocusTargetHitbox { get; set; }
    internal Vector3 PlayerPosition { get; set; }
    internal float PlayerRotation { get; set; }
    internal float PlayerHitbox { get; set; }
    internal float PlayerAutoAttackRadius { get; set; }
    // 3.6 yalms for melee, 25.6 for ranged
    internal Vector3 MousePosition { get; set; }
    internal uint ActionID { get; set; }
    internal string ActionName { get; set; }
    internal float ActionRadius { get; set; }
    internal bool DamagingAction { get; set; }
    internal bool CanTargetEnemy { get; set; }
    internal byte CastType { get; set; }
    // CastType determines the shape of the cast
    // 1 - single target (no shape)
    // 2 - circle
    // 3 - cone
    // 4 - straigt line/square
    // 7 - placebles (sacred soil, shukuchi, asylum, liturgy of the bell, etc)
    // 11 - southern cross (pvp)
    // 15 - hissatsu: soten (pvp) exclusive
    // 5, 6, 8, 9, 10, 11, 12, 13, 14 - unknown (exclusive to enemies/npc/special actions)
    internal byte CastWidth { get; set; }
    // The width of the straight line attacks. This is the total width, not a half.
    internal float ActionAngle { get; set; }
    // Variable to hold hardcoded cone angle.
    internal float DistanceToTarget3D => TargetPosition != Vector3.Zero ? (PlayerPosition - TargetPosition).Length() - TargetHitbox : 0f;
    // Probably not useful? Game seems to care more about 2D distance to a target then 3D.
    internal float DistanceToTarget2D => TargetPosition != Vector3.Zero ? PlayerPosition.Distance2D(TargetPosition) - TargetHitbox : 0f;
    internal float DistanceToFocusTarget3D => FocusTargetPosition != Vector3.Zero ? (PlayerPosition - FocusTargetPosition).Length() - FocusTargetHitbox : 0f;
    internal float DistanceToFocusTarget2D => FocusTargetPosition != Vector3.Zero ? PlayerPosition.Distance2D(FocusTargetPosition) - FocusTargetHitbox : 0f;
    internal float ActionRange => ActionManager.GetActionRange(ActionID) != 0 ? ActionManager.GetActionRange(ActionID) + PlayerHitbox : 0f;
    // Range calculation for actions start at the edge of the player hitbox, and not at the center.
    // Same applies for cone shaped AoEs.
}
