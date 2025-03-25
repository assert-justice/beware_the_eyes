using Godot;
public partial class Globals: Node{
    public bool UseKeyboard{get; private set;} = true;
    public UserSettings UserSettings;
    public UserSave[] UserSaves;
    public int SaveId = 0;
    public UserSave GetSave(){
        return UserSaves[SaveId];
    }
    public UserSettings GetSettings(){
        return UserSettings;
    }
    public void SaveSettings(){
        ResourceSaver.Save(UserSettings, "user://settings.tres");
    }
    public static Globals Instance{get; private set;}
    private Globals(){
        var temp = GD.Load("user://settings.tres");
        if(temp == null) {
            UserSettings = new();
            SaveSettings();
        }
        else {
            UserSettings = temp as UserSettings;
            // Apply settings
            // if(UserSettings.Fullscreen) DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
            if(UserSettings.Fullscreen) OptionsMenu.SetFullscreen(true);
            int mainVolumeId = AudioServer.GetBusIndex("Master");
            int musicId = AudioServer.GetBusIndex("Music");
            int sfxId = AudioServer.GetBusIndex("Sfx");
            OptionsMenu.SetVolume(mainVolumeId, UserSettings.MainVolume);
            OptionsMenu.SetVolume(musicId, UserSettings.MusicVolume);
            OptionsMenu.SetVolume(sfxId, UserSettings.SfxVolume);
        }
        UserSaves = [new(), new(), new()];
    }
    public override void _Ready()
    {
        base._Ready();
        Instance = this;
    }
    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if(@event is InputEventJoypadButton) {
            Instance.UseKeyboard = false;
        }
        else if(@event is InputEventKey || @event is InputEventMouseButton) {
            Instance.UseKeyboard = true;
        }
    }
}