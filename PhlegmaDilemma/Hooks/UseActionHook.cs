namespace PhlegmaDilemma.Hooks;

using static ActionManager;
using UseActionDelegate = ActionManager.Delegates.UseAction;

public unsafe class UseActionHook
{
    static readonly Hook<UseActionDelegate> _useActionHook;
    private static uint actionIdInternal = 0;
    private static uint prevActionNumber = 0;
    private static uint actionNumber = 0;
    static unsafe UseActionHook()
    {
        _useActionHook ??= Plugin.GameInteropProvider.HookFromAddress<UseActionDelegate>(
            (nint)MemberFunctionPointers.UseAction, DetourUseAction);
    }

    public static void Dispose()
    {
        _useActionHook?.Dispose();
    }

    private static bool DetourUseAction(ActionManager* self, ActionType actionType, uint actionId, ulong targetId, uint extraParam, UseActionMode mode, uint comboRouteId, bool* outOptAreaTargeted)
    {
        var r = _useActionHook!.Original(self, actionType, actionId, targetId, extraParam, mode, comboRouteId, outOptAreaTargeted);
        actionIdInternal = actionId;
        actionNumber++;
        return r;
    }

    public static uint RetrieveActionID()
    {
        return actionIdInternal;
    }

    public static uint RetrieveActionNumber()
    {
        return actionNumber;
    }

    public static void Enable() { _useActionHook.Enable(); }
    public static void Disable() { _useActionHook.Disable(); }
    public static void Check() { Plugin.PluginLog.Information($"Check: {_useActionHook?.IsEnabled}"); }
}
