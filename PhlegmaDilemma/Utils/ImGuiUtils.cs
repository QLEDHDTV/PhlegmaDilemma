namespace PhlegmaDilemma.Utils;

internal class ImGuiUtils
{
    internal static uint Vec4ToUInt(Vector4 i)
    {
        return (uint)(i.X * 255f) << 0 | (uint)(i.Y * 255f) << 8 | (uint)(i.Z * 255f) << 16 | (uint)(i.W * 255f) << 24;
    }
    internal const ImGuiWindowFlags OverlayWindowFlags = ImGuiWindowFlags.NoDecoration |
                                                        ImGuiWindowFlags.NoSavedSettings |
                                                        ImGuiWindowFlags.NoMove |
                                                        ImGuiWindowFlags.NoMouseInputs |
                                                        ImGuiWindowFlags.NoFocusOnAppearing |
                                                        ImGuiWindowFlags.NoBackground |
                                                        ImGuiWindowFlags.NoNav;
}
