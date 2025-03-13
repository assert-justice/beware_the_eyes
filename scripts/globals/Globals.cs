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
    public void SaveSettings(){}
    public static Globals Instance{get; private set;}
    private Globals(){
        UserSettings = new();
        UserSaves = [new(), new(), new()];
    }
    public override void _Ready()
    {
        base._Ready();
        Instance = new();
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