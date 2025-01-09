namespace PhlegmaDilemma.Services;

public class CameraService
{
    public unsafe CameraManager* CameraManager => FFXIVClientStructs.FFXIV.Client.Game.Control.CameraManager.Instance();
    public unsafe CameraManager* CurrentCamera => (CameraManager*)CameraManager->GetActiveCamera();
}
