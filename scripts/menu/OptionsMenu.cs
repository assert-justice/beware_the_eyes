
using Godot;

public partial class OptionsMenu: Menu{
	HSlider musicSlider;
	HSlider sfxSlider;
	CheckBox fullscreenCheckbox;
	int musicId;
	int sfxId;
	public override void _Ready()
	{
		base._Ready();
		// musicId = AudioServer.GetBusIndex("Music");
		// sfxId = AudioServer.GetBusIndex("Sfx");
		// musicSlider = GetNode<HSlider>("HBoxContainer/VBoxContainer/MusicSlider");
		// musicSlider.DragEnded += b =>{ if (b) SetVolume(musicId, (float)musicSlider.Value);};
		// sfxSlider = GetNode<HSlider>("HBoxContainer/VBoxContainer/SfxSlider");
		// sfxSlider.DragEnded += b =>{ if (b) SetVolume(sfxId, (float)sfxSlider.Value);};
		// fullscreenCheckbox = GetNode<CheckBox>("HBoxContainer/VBoxContainer/FullscreenCheckBox");
		// fullscreenCheckbox.Toggled += b => { SetFullscreen(b);};
		GetNode<Button>("HBoxContainer/VBoxContainer/Back").ButtonDown += ()=>{menuSystem.PopMenu();};
	}
    public override void OnWake()
    {
        base.OnWake();
		// musicSlider.Value = GetVolume(musicId);
		// sfxSlider.Value = GetVolume(sfxId);
		// fullscreenCheckbox.ButtonPressed = IsFullscreen();
    }
    static bool IsFullscreen(){
		return DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen;
	}
    static void SetFullscreen(bool isFullscreen){
		var mode = isFullscreen ? DisplayServer.WindowMode.Fullscreen : DisplayServer.WindowMode.Windowed;
		DisplayServer.WindowSetMode(mode); 
	}
	static void SetVolume(int id, float volume){
		AudioServer.SetBusVolumeDb(id, Mathf.LinearToDb(volume / 100));
	}
	static float GetVolume(int id){
		return Mathf.DbToLinear(AudioServer.GetBusVolumeDb(id)) * 100;
	}
}
