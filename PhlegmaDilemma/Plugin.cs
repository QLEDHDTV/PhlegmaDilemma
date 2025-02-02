using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Party;
using System.Linq;

namespace PhlegmaDilemma;

public unsafe sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IPluginLog Log { get; private set; } = null!;
    [PluginService] internal static IGameGui GameGui { get; private set; } = null!;
    [PluginService] internal static ITargetManager TargetManager { get; private set; } = null!;
    [PluginService] internal static IClientState ClientState { get; private set; } = null!;
    [PluginService] internal static IGameInteropProvider GameInteropProvider { get; private set; } = null!;
    [PluginService] internal static IPluginLog PluginLog { get; private set; } = null!;
    [PluginService] internal static IFramework Framework { get; private set; } = null!;
    [PluginService] internal static IDataManager DataManager { get; private set; } = null!;
    [PluginService] internal static IObjectTable ObjectTable { get; private set; } = null!;
    [PluginService] internal static IPartyList PartyList { get; private set; } = null!;

    private const string CommandName = "/pd";

    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("PhlegmaDilemma");
    private ConfigWindow ConfigWindow { get; init; }
    private DebugWindow DebugWindow { get; init; }
    private Rangefinder Rangefinder { get; init; }
    internal DataDynamic[] data = new DataDynamic[1];
    internal ExcelSheet<Lumina.Excel.Sheets.Action> ActionSheet = DataManager.GetExcelSheet<Lumina.Excel.Sheets.Action>();
    
    internal uint[] Angle90 = {106, 2870, 11403};
    internal uint[] Angle180 = {24392, 24384};
    // Hardcoded angle values for cone actions.
    public Plugin(IDalamudPluginInterface pluginInterface)
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        ConfigWindow = new ConfigWindow(this);
        DebugWindow = new DebugWindow(this);
        Rangefinder = new Rangefinder(this, Configuration);

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(DebugWindow);
        WindowSystem.AddWindow(Rangefinder);

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Open the settings"
        });

        PluginInterface.UiBuilder.Draw += DrawUI;
        PluginInterface.UiBuilder.Draw += DrawRangefinder;
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;

        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;

        Framework.Update += OnFrameworkUpdate;
        EnableUseActionHook();
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        DebugWindow.Dispose();

        CommandManager.RemoveHandler(CommandName);
        UseActionHook.Dispose();
    }

    private void OnCommand(string command, string args)
    {
        ToggleConfigUI();
    }

    private void DrawUI() => WindowSystem.Draw();
    private void DrawConfig() => ConfigWindow.Draw();
    private void DrawRangefinder() => Rangefinder.Draw();
    public void EnableUseActionHook() => UseActionHook.Enable();
    public void DisableUseActionHook() => UseActionHook.Disable();
    public void CheckUseActionHook() => UseActionHook.Check();
    public void ToggleMainUI() => DebugWindow.Toggle();
    public void ToggleConfigUI() => ConfigWindow.Toggle();
    internal void OnFrameworkUpdate(IFramework framework) => GetData();
    internal DataDynamic RetrieveData()
    {
        return data[0];
    }
    internal void GetData()
    {
        if (ClientState.LocalPlayer != null)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (TargetManager.Target != null)
                {
                    data[i].Target = TargetManager.Target;
                    data[i].TargetPosition = TargetManager.Target.Position;
                    data[i].TargetHitbox = TargetManager.Target.HitboxRadius;
                }
                else
                {
                    data[i].Target = null;
                    data[i].TargetPosition = Vector3.Zero;
                    data[i].TargetHitbox = 0;
                }

                if (TargetManager.FocusTarget != null)
                {
                    data[i].FocusTarget = TargetManager.FocusTarget;
                    data[i].FocusTargetPosition = TargetManager.FocusTarget.Position;
                    data[i].FocusTargetHitbox = TargetManager.FocusTarget.HitboxRadius;
                }
                else
                {
                    data[i].FocusTarget = null;
                    data[i].FocusTargetPosition = Vector3.Zero;
                    data[i].FocusTargetHitbox = 0;
                }
                data[i].PlayerPosition = ClientState.LocalPlayer.Position;
                data[i].PlayerRotation = ClientState.LocalPlayer.Rotation;
                data[i].PlayerHitbox = ClientState.LocalPlayer.HitboxRadius;
                if (new uint[] { 5, 23, 31, 38 }.Contains(ClientState.LocalPlayer.ClassJob.Value.RowId)) // Ranged auto attack range
                {

                    data[i].PlayerAutoAttackRadius = 25.6f;
                }
                else
                {
                    data[i].PlayerAutoAttackRadius = 3.6f;
                }

                GameGui.ScreenToWorld(ImGui.GetMousePos(), out Vector3 worldSpace);
                data[i].MousePosition = worldSpace;

                if (UseActionHook.RetrieveActionID() != 0)
                {
                    data[i].ActionID = ActionManager.Instance()->GetAdjustedActionId(UseActionHook.RetrieveActionID());
                    if (ActionSheet.TryGetRow(data[i].ActionID, out var row) == true)
                    {
                        data[i].ActionName = row.Name.ExtractText();
                        data[i].ActionRadius = (float)row.EffectRange;
                        data[i].DamagingAction = row.Unknown14;                 // Unknown 14 seems to determine if the action can interact with the
                        data[i].CanTargetEnemy = row.CanTargetHostile;          // hostiles but not necesserily by directly targeting them with an action. (damaging AoEs)
                        data[i].CastType = row.CastType;
                        data[i].CastWidth = row.XAxisModifier;
                        if (data[i].CastType == 3)
                        {
                            if (Angle90.Contains(data[i].ActionID))
                            {
                                data[i].ActionAngle = 90;
                            }
                            else if (Angle180.Contains(data[i].ActionID))
                            {
                                data[i].ActionAngle = 180;
                            }
                            else
                            {
                                data[i].ActionAngle = 120;
                            }
                        }
                    }
                }

                // Search for all IBattleChara and IPlayerCharacter objects within 30 yalms
                data[i].InRangeEnemyTargets = ObjectTable.Where(obj => obj.Position.Distance2D(ClientState.LocalPlayer.Position) <= 30 && obj is IBattleNpc && obj.IsTargetable && (data[i].Target != null ? obj.EntityId != data[i].Target.EntityId : true)).ToArray();
                var nearbyCharacters = ObjectTable.Where(obj => obj != ClientState.LocalPlayer && obj.Position.Distance2D(ClientState.LocalPlayer.Position) <= 30 && obj is IPlayerCharacter && (data[i].Target != null ? obj.EntityId != data[i].Target.EntityId : true)).ToArray();
                data[i].InRangeChars = nearbyCharacters.Where(obj => PartyList.Any(x => x.ObjectId == obj.EntityId)).ToArray();
            }
        }
        else
        {
            data[0].Dispose();
        }
    }
}
