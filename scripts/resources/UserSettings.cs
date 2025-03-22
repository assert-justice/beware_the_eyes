using Godot;

public partial class UserSettings: Resource{
    public float MoveDeadzone = 0.2f;
    public float AimDeadzone = 0.2f;
    public float MouseSensitivity = 0.5f;
    public float MainVolume = 1;
    public float SfxVolume = 1;
    public float MusicVolume = 1;
    public bool Fullscreen = false;
    public bool CameraRoll = true;
    public bool InvertCamera = false;
}
