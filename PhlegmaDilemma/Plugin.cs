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

    private const string CommandName = "/pmycommand";

    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("PhlegmaDilemma");
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }
    private Rangefinder Rangefinder { get; init; }
    internal DataDynamic[] data = new DataDynamic[1];
    internal ExcelSheet<Lumina.Excel.Sheets.Action> ActionSheet = DataManager.GetExcelSheet<Lumina.Excel.Sheets.Action>();
    
    internal uint[] Angle90 = new uint[] {106, 11403};
    internal uint[] Angle180 = new uint[] {24392, 24384};
    // Hardcoded angle values for cone actions.
    public Plugin(IDalamudPluginInterface pluginInterface)
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this);
        Rangefinder = new Rangefinder(this, Configuration);

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);
        WindowSystem.AddWindow(Rangefinder);

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "A useful message to display in /xlhelp"
        });

        PluginInterface.UiBuilder.Draw += DrawUI;
        PluginInterface.UiBuilder.Draw += DrawRangefinder;
        // This adds a button to the plugin installer entry of this plugin which allows
        // to toggle the display status of the configuration ui
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;

        // Adds another button that is doing the same but for the main ui of the plugin
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;

        Framework.Update += OnFrameworkUpdate;
        EnableUseActionHook();
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();

        CommandManager.RemoveHandler(CommandName);
        UseActionHook.Dispose();
    }

    private void OnCommand(string command, string args)
    {
        // in response to the slash command, just toggle the display status of our main ui
        ToggleMainUI();
    }

    private void DrawUI() => WindowSystem.Draw();
    private void DrawConfig() => ConfigWindow.Draw();
    private void DrawRangefinder() => Rangefinder.Draw();
    public void EnableUseActionHook() => UseActionHook.Enable();
    public void DisableUseActionHook() => UseActionHook.Disable();
    public void CheckUseActionHook() => UseActionHook.Check();
    public void ToggleMainUI()
    {
        MainWindow.Toggle();
    }
    public void ToggleConfigUI()
    {
        ConfigWindow.Toggle();
    }
    internal void OnFrameworkUpdate(IFramework framework)
    {
        GetData();
    }
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
            }
        }
    }
}
