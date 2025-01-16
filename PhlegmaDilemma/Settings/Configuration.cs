namespace PhlegmaDilemma.Settings;

[Serializable]
public class Configuration : IPluginConfiguration
{
    private IDalamudPluginInterface PluginInterface;
    public int Version { get; set; } = 0;
    public bool EnableRangefinder { get; set; } = true;
    public bool EnableAutoAttackRange {  get; set; } = true;
    public Vector4 ColorActionRange { get; set; } = new Vector4((float)0xFF / 255f, (float)0x00 / 255f, (float)0x00 / 255f, (float)0xFF / 255f);
    public Vector4 ColorActionRadius { get; set; } = new Vector4((float)0x00 / 255f, (float)0xFF / 255f, (float)0x00 / 255f, (float)0xFF / 255f);
    public Vector4 ColorAutoAttack { get; set; } = new Vector4((float)0x00 / 255f, (float)0x00 / 255f, (float)0xFF / 255f, (float)0xFF / 255f);
    public Vector4 ColorTargetPointerOutOfRange { get; set; } = new Vector4((float)0xFF / 255f, (float)0x00 / 255f, (float)0x00 / 255f, (float)0xFF / 255f);
    public Vector4 ColorTargetPointerInRange { get; set; } = new Vector4((float)0xFF / 255f, (float)0xFF / 255f, (float)0x00 / 255f, (float)0xFF / 255f);
    public float Thickness { get; set; } = 2f;
    public int PointsNumber { get; set; } = 100;
    public bool EnableDebugCone { get; set; } = false;
    public bool EnableFadeout { get; set; } = false;
    public int FadeDelay { get; set; } = 5000; 
    public int FadeSpeed { get; set; } = 10;
    public float DebugConeRadius { get; set; } = 5f;
    public float DebugConeAngle { get; set; } = 90f;
    public bool DebugConeFollowTarget { get; set; } = false;

    // the below exist just to make saving less cumbersome
    public void Save()
    {
        PluginInterface.SavePluginConfig(this);
    }
}
