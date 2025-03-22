
using Godot;

public partial class OptionsMenu: Menu{
	HSlider mainSlider;
	HSlider musicSlider;
	HSlider sfxSlider;
	HSlider mouseSensitivity;
	HSlider aimDeadzone;
	HSlider moveDeadzone;
	CheckButton fullscreen;
	CheckButton invert;
	CheckButton cameraRoll;
	int mainVolumeId;
	int musicId;
	int sfxId;
	public override void _Ready()
	{
		base._Ready();
		var settings = Globals.Instance.GetSettings();
		mainVolumeId = AudioServer.GetBusIndex("Master");
		musicId = AudioServer.GetBusIndex("Music");
		sfxId = AudioServer.GetBusIndex("Sfx");
		fullscreen = GetNode<CheckButton>("HBoxContainer/VBoxContainer/Fullscreen");
		fullscreen.Toggled += b => { 
			SetFullscreen(b);
			settings.Fullscreen = b;
		};
		invert = GetNode<CheckButton>("HBoxContainer/VBoxContainer/Invert");
		invert.Toggled += b => {
			settings.InvertCamera = b;
		};
		cameraRoll = GetNode<CheckButton>("HBoxContainer/VBoxContainer/CameraRoll");
		cameraRoll.Toggled += b => {
			settings.CameraRoll = b;
		};
		mouseSensitivity = GetNode<HSlider>("HBoxContainer/VBoxContainer/MouseSensitivity");
		mouseSensitivity.DragEnded += b => {
			if(!b)return;
			float value = (float)mouseSensitivity.Value / 100;
			settings.MouseSensitivity = value;
		};
		aimDeadzone = GetNode<HSlider>("HBoxContainer/VBoxContainer/AimDeadzone");
		aimDeadzone.DragEnded += b => {
			if(!b)return;
			float value = (float)aimDeadzone.Value / 100;
			settings.AimDeadzone = value;
		};
		moveDeadzone = GetNode<HSlider>("HBoxContainer/VBoxContainer/MoveDeadzone");
		moveDeadzone.DragEnded += b => {
			if(!b)return;
			float value = (float)moveDeadzone.Value / 100;
			settings.MoveDeadzone = value;
		};
		mainSlider = GetNode<HSlider>("HBoxContainer/VBoxContainer/MainSlider");
		mainSlider.DragEnded += b =>{
			if (!b) return;
			float value = (float)mainSlider.Value / 100;
			SetVolume(mainVolumeId, value);
			settings.MainVolume = value;
		};
		musicSlider = GetNode<HSlider>("HBoxContainer/VBoxContainer/MusicSlider");
		musicSlider.DragEnded += b =>{
			if (!b) return;
			float value = (float)musicSlider.Value / 100;
			SetVolume(musicId, value);
			settings.MusicVolume = value;
		};		
		sfxSlider = GetNode<HSlider>("HBoxContainer/VBoxContainer/SfxSlider");
		sfxSlider.DragEnded += b =>{
			if (!b) return;
			float value = (float)sfxSlider.Value / 100;
			SetVolume(sfxId, value);
			settings.SfxVolume = value;
		};
		GetNode<Button>("HBoxContainer/VBoxContainer/Back").ButtonDown += ()=>{menuSystem.PopMenu();};
	}
    public override void OnWake()
    {
        base.OnWake();
		var settings = Globals.Instance.GetSettings();
		fullscreen.ButtonPressed = IsFullscreen();
		invert.ButtonPressed = settings.InvertCamera;
		cameraRoll.ButtonPressed = settings.CameraRoll;
		mouseSensitivity.Value = settings.MouseSensitivity * 100;
		aimDeadzone.Value = settings.AimDeadzone * 100;
		moveDeadzone.Value = settings.MoveDeadzone * 100;
		mainSlider.Value = GetVolume(mainVolumeId) * 100;
		musicSlider.Value = GetVolume(musicId) * 100;
		sfxSlider.Value = GetVolume(sfxId) * 100;
    }
    public override void OnSleep()
    {
        base.OnSleep();
		// Save settings
    }
    static bool IsFullscreen(){
		return DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen;
	}
    static void SetFullscreen(bool isFullscreen){
		var mode = isFullscreen ? DisplayServer.WindowMode.Fullscreen : DisplayServer.WindowMode.Windowed;
		DisplayServer.WindowSetMode(mode); 
	}
	static void SetVolume(int id, float volume){
		AudioServer.SetBusVolumeDb(id, Mathf.LinearToDb(volume));
	}
	static float GetVolume(int id){
		return Mathf.DbToLinear(AudioServer.GetBusVolumeDb(id));
	}
}
