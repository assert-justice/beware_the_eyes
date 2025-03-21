
using Godot;

public partial class PauseMenu: Menu{
	public override void _Ready()
	{
		base._Ready();
		GetNode<Button>("HBoxContainer/VBoxContainer/Resume").ButtonDown += ()=>{
			menuSystem.Pause(false);
		};
		GetNode<Button>("HBoxContainer/VBoxContainer/Options").ButtonDown += ()=>{
			menuSystem.PushMenu("Options");
		};
		GetNode<Button>("HBoxContainer/VBoxContainer/Quit").ButtonDown += ()=>{
			GetTree().Quit();
		};
	}
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if(Input.IsActionJustPressed("pad_pause") || Input.IsActionJustPressed("kb_pause")){
            menuSystem.Pause(false);
        }
    }
}
