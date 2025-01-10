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

    // the below exist just to make saving less cumbersome
    public void Save()
    {
        PluginInterface.SavePluginConfig(this);
    }
}
