using Godot;
using System;

public partial class Player : Actor
{
	[Export] PackedScene SparkleScene;
	[ExportGroup("PowerUps")]
	[Export] bool doubleJumpEnabled = false;
	[Export] bool dashEnabled = false;
	[Export] bool jetPackEnabled = false;
	[Export] int maxAirJumps = 1;
	[Export] float dashTime = 0.5f;
	[Export] float dashSpeed = 15.0f;
	[Export] int maxAirDashes = 1;
	[Export] float jetPackPower = 1;
	[Export] float maxJetPackFuel = 1;
	[ExportGroup("Movement")]
	[Export] public float Speed = 5.0f;
	[Export] public float TurnSpeed = 5.0f;
	[Export] public float JumpVelocity = 4.5f;
	[Export] float coyoteTime = 0.1f;
	[Export] float jumpBuffer = 0.1f;
	[ExportGroup("Camera")]
	[Export] float minCameraAngle = -1;
	[Export] float maxCameraAngle = 1;
	[Export] float fov = 75.0f;
	[Export] float zoomFov = 30.0f;
	[Export] float zoomSensitivityMul = 0.3f;
	[Export] float dashFovScale = 0.9f;
	[Export] float maxTiltAngle = 0.04f;
	[Export] float tiltAngleSpeed = 0.5f;
	readonly PlayerInput playerInput = new();
	Camera3D camera;
	RayCast3D rayCast;
	float jetPackFuel = 0;
	int airDashes = 0;
	int airJumps = 0;
	Clock groundClock;
	Clock jumpClock;
	Clock dashClock;
	Label hudLabel;
	Notification notification;
	Vector3 nextVelocity = new();
	Node3D hardPoint;
	Weapon CurrentWeapon = null;
	MenuSystem menuSystem;
	AudioStreamPlayer3D jumpSound;
	AudioStreamPlayer3D dieSound;
	AudioStreamPlayer3D hurtSound;
	Control hud;
	Vector3 respawnPos;
	SpotLight3D flashlight;
	Axe axe;
	bool hasAxe = false;

	public override void _Ready()
	{
		base._Ready();
		respawnPos = Position;
		var temp = GetTree().GetNodesInGroup("MenuSystem");
		if(temp.Count > 0) {
			menuSystem = temp[0] as MenuSystem;
			menuSystem.resumeCallback = ()=>hud.Visible = true;
		}
		camera = GetNode<Camera3D>("Camera3D");
		rayCast = GetNode<RayCast3D>("Camera3D/RayCast3D");
		hardPoint = GetNode<Node3D>("Camera3D/Hardpoint");
		foreach (var c in hardPoint.GetChildren())
		{
			if(c is Weapon w)w.SetActor(this);
		}
		hudLabel = GetNode<Label>("Camera3D/Hud/Label");
		hud = GetNode<Control>("Camera3D/Hud");
		flashlight = GetNode<SpotLight3D>("Camera3D/SpotLight3D");
		axe = GetNode<Axe>("Camera3D/Axe");
		jumpSound = GetNode<AudioStreamPlayer3D>("JumpSound");
		dieSound = GetNode<AudioStreamPlayer3D>("DieSound");
		hurtSound = GetNode<AudioStreamPlayer3D>("HurtSound");
		notification = GetNode<Notification>("Camera3D/Hud/Notification");
		groundClock = AddClock(coyoteTime, 0);
		jumpClock = AddClock(jumpBuffer, 0);
		dashClock = AddClock(dashTime, 0);
		if(menuSystem == null) Input.MouseMode = Input.MouseModeEnum.Captured;
	}
	public override void _Input(InputEvent @event)
	{
		if(Input.MouseMode == Input.MouseModeEnum.Captured && Globals.Instance.UseKeyboard){
			if (@event is InputEventMouseMotion mouseMotion){
				var settings = Globals.Instance.GetSettings();
				float mouseSensitivity = settings.MouseSensitivity;
				var motion = mouseMotion.Relative * mouseSensitivity * 0.01f;
				ChangeCameraYaw(-motion.X);
				ChangeCameraPitch(-motion.Y);
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		float dt = (float)delta;
		if(IsZoomed){
			camera.Fov = zoomFov;
		}
		else{
			float minFov = fov * dashFovScale;
			float fovDiff = (fov - minFov) * dashClock.GetProgress();
			camera.Fov = minFov + fovDiff;
		}
		if(IsOnFloor()) {
			groundClock.Reset();
			airJumps = maxAirJumps;
			airDashes = maxAirDashes;
			jetPackFuel = maxJetPackFuel;
		}
		playerInput.Poll();
		if(playerInput.FlashlightJustPressed()){
			flashlight.Visible = !flashlight.Visible;
		}
		var weaponSelected = playerInput.GetWeaponMask();
		for (int i = 0; i < weaponSelected.Length; i++)
		{
			if(weaponSelected[i]) SetWeapon(i);
		}
		Vector2 inputDir = playerInput.GetMove();
		FireCommand command = new()
		{
			Team = Team,
			Ray = rayCast,
			FirePressed = playerInput.FirePressed(),
			FireJustPressed = playerInput.FireJustPressed(),
			AltPressed = playerInput.AltPressed(),
			AltJustPressed = playerInput.AltJustPressed(),
			MeleePressed = playerInput.MeleePressed(),
			MeleeJustPressed = playerInput.MeleeJustPressed(),
			Player = this,
			dt = dt,
		};
		CurrentWeapon?.TryFire(command);
		if(hasAxe) axe.TryFire(command);
		if(playerInput.JumpJustPressed()) jumpClock.Reset();
		if(playerInput.PauseJustPressed()){
			if(menuSystem != null) {
				menuSystem.CallDeferred("Pause", true);
				hud.Visible = false;
			}
			else GetTree().Quit();
		}
		Vector3 velocity = Velocity;
		if(IsOnFloor()){}
		else if(dashClock.IsRunning()){}
		else if(jetPackEnabled && playerInput.JumpPressed() && jetPackFuel > 0){
			jetPackFuel -= dt;
			if(jetPackFuel < 0) jetPackFuel = 0;
			velocity += Vector3.Up * jetPackPower * dt;
		}
		else{
			velocity += GetGravity() * dt;
		}
		// Handle Jump.
		bool shouldJump = false;
		if(jumpClock.IsRunning()){
			if(groundClock.IsRunning()) shouldJump = true;
			else if(doubleJumpEnabled && airJumps > 0){
				airJumps--;
				shouldJump = true;
			}
		}
		if(shouldJump){
			velocity.Y = JumpVelocity;
			groundClock.Finish();
			jumpClock.Finish();
			jumpSound.Play();
		}
		// Handle Dash
		bool shouldDash = false;
		if(dashEnabled && playerInput.DashJustPressed()){
			if(groundClock.IsRunning()) shouldDash = true;
			else if(airDashes > 0){
				airDashes--;
				shouldDash = true;
			}
		}
		if(shouldDash){
			Vector3 dir = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
			if(inputDir.Length() == 0) dir = (Transform.Basis * Vector3.Forward).Normalized();
			velocity = dir * dashSpeed;
			dashClock.Reset();
			IsZoomed = false;
		}

		Vector2 aim = playerInput.GetAim();
		ChangeCameraPitch(-aim.Y * TurnSpeed * dt);
		ChangeCameraYaw(-aim.X * TurnSpeed * dt);
		var angle = Mathf.MoveToward(camera.Rotation.Z, inputDir.X * maxTiltAngle, dt * tiltAngleSpeed);
		if(Globals.Instance.GetSettings().CameraRoll) SetCameraRoll(angle);
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if(!dashClock.IsRunning()){
			if (direction != Vector3.Zero)
			{
				velocity.X = direction.X * Speed;
				velocity.Z = direction.Z * Speed;
			}
			else
			{
				velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
				velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
			}
		}
		Velocity = velocity;
		base._PhysicsProcess(delta);
		SetHud();
		MoveAndSlide();
	}
	public override void Damage(float value)
	{
		base.Damage(value);
		if(Health >= 0)hurtSound.Play();
	}
	public override void Die()
	{
		base.Die();
		dieSound.Play();
		Position = respawnPos;
		Health = 100;
	}
	public void SaveState(Vector3 spawnPos){
		notification.AddMessage("Saved!");
		respawnPos = spawnPos;
	}
	public bool AddPickup(PickupType pickupType){
		switch (pickupType)
		{
			case PickupType.Boots:
			doubleJumpEnabled = true;
			notification.AddMessage("Double Jump Acquired!");
			break;
			case PickupType.JetPack:
			jetPackEnabled = true;
			notification.AddMessage("Jetpack Acquired!");
			break;
			case PickupType.Dash:
			dashEnabled = true;
			notification.AddMessage("Dash Acquired!");
			break;
			case PickupType.Shotgun:
			EnableWeapon(0);
			notification.AddMessage("Shotgun Acquired!");
			break;
			case PickupType.ShotgunAmmo:
			if(GetWeapon(0).AddAmmo()){
				notification.AddMessage("Shells Acquired!");
			}
			break;
			case PickupType.Zapper:
			EnableWeapon(3);
			notification.AddMessage("Zapper Acquired!");
			break;
			case PickupType.ZapperAmmo:
			if(GetWeapon(3).AddAmmo()){
				notification.AddMessage("Charge Acquired!");
			}
			break;
			case PickupType.Crossbow:
			EnableWeapon(2);
			notification.AddMessage("Crossbow Acquired!");
			break;
			case PickupType.CrossbowAmmo:
			if(GetWeapon(2).AddAmmo()){
				notification.AddMessage("Bolts Acquired!");
			}
			break;
			case PickupType.Launcher:
			EnableWeapon(1);
			notification.AddMessage("Launcher Acquired!");
			break;
			case PickupType.Axe:
			notification.AddMessage("Axe Acquired!");
			hasAxe = true;
			break;
			default:
			GD.PrintErr("Unhandled pickup type!");
			notification.AddMessage("Unhandled pickup type!");
			break;
		}
		return true;
	}
	Weapon GetWeapon(int idx){
		return hardPoint.GetChild<Weapon>(idx);
	}
	void EnableWeapon(int idx){
		var weapon = hardPoint.GetChild<Weapon>(idx);
		weapon.Enable();
		SetWeapon(idx);
	}
	void SetWeapon(int idx){
		var weapon = hardPoint.GetChild<Weapon>(idx);
		if(!weapon.IsEnabled()) return;
		CurrentWeapon?.UnMount();
		weapon.Mount();
		CurrentWeapon = weapon;
	}
	void ChangeCameraPitch(float da){
		if(IsZoomed) da *= zoomSensitivityMul;
		if(Globals.Instance.GetSettings().InvertCamera) da = -da;
		Vector3 cameraRotation = camera.Rotation;
		cameraRotation.X = Mathf.Clamp(cameraRotation.X + da, minCameraAngle, maxCameraAngle);
		camera.Rotation = cameraRotation;
	}
	void ChangeCameraYaw(float da){
		if(IsZoomed) da *= zoomSensitivityMul;
		Vector3 cameraRotation = Rotation;
		cameraRotation.Y += da;
		Rotation = cameraRotation;
	}
	void SetCameraRoll(float da){
		Vector3 cameraRotation = camera.Rotation;
		cameraRotation.Z = da;
		camera.Rotation = cameraRotation;
	}
	void SetHud(){
		int fuel = Mathf.FloorToInt(jetPackFuel * 100);
		string ammoStr = "-/-";
		if(CurrentWeapon != null) ammoStr = CurrentWeapon.GetAmmoString();
		hudLabel.Text = $"Health: {Health}\nAmmo: {ammoStr}\nFuel: {fuel}";
	}
}
