using Godot;
[GlobalClass]

public partial class UserSettings: Resource{
    [Export] public float MoveDeadzone = 0.2f;
    [Export] public float AimDeadzone = 0.2f;
    [Export] public float MouseSensitivity = 0.5f;
    [Export] public float MainVolume = 1;
    [Export] public float SfxVolume = 1;
    [Export] public float MusicVolume = 1;
    [Export] public bool Fullscreen = false;
    [Export] public bool CameraRoll = true;
    [Export] public bool InvertCamera = false;
}
