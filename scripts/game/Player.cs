using Godot;
using System;

public partial class Player : Actor
{
	[Export] PackedScene SparkleScene;
	[ExportGroup("PowerUps")]
	[Export] bool doubleJumpEnabled = true;
	[Export] bool dashEnabled = true;
	[Export] bool jetPackEnabled = true;
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
	EntPool sparklePool;
	Label hud;
	Notification notification;
	Vector3 nextVelocity = new();

	public override void _Ready()
	{
		base._Ready();
		camera = GetNode<Camera3D>("Camera3D");
		rayCast = GetNode<RayCast3D>("Camera3D/RayCast3D");
		hud = GetNode<Label>("Camera3D/Control/Label");
		notification = GetNode<Notification>("Camera3D/Control/Notification");
		groundClock = AddClock(coyoteTime, 0);
		jumpClock = AddClock(jumpBuffer, 0);
		dashClock = AddClock(dashTime, 0);
		sparklePool = AddPool(GetParent(), ()=>{return SparkleScene.Instantiate<Entity>();});
		// TODO: handle elsewhere
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}
	public override void _Input(InputEvent @event)
	{
		if(Input.MouseMode == Input.MouseModeEnum.Captured && Globals.Instance.UseKeyboard){
			if (@event is InputEventMouseMotion mouseMotion){
				float mouseSensitivity = Globals.Instance.GetSettings().MouseSensitivity;
				var motion = mouseMotion.Relative * mouseSensitivity * 0.001f;
				ChangeCameraYaw(-motion.X);
				ChangeCameraPitch(-motion.Y);
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		float dt = (float)delta;
		float minFov = fov * dashFovScale;
		float fovDiff = (fov - minFov) * dashClock.GetProgress();
		camera.Fov = minFov + fovDiff;
		if(IsOnFloor()) {
			groundClock.Reset();
			airJumps = maxAirJumps;
			airDashes = maxAirDashes;
			jetPackFuel = maxJetPackFuel;
		}
		playerInput.Poll();
		Vector2 inputDir = playerInput.GetMove();
		if(playerInput.FireJustPressed()){
			if(rayCast.IsColliding()){
				// var target = rayCast.GetCollider();
				var point = rayCast.GetCollisionPoint();
				var s = sparklePool.GetPool().GetNew();
				s.Position = point;
				notification.AddMessage("fire!");
			}
		}
		if(playerInput.JumpJustPressed()) jumpClock.Reset();
		if(playerInput.PauseJustPressed()) GetTree().Quit();
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
		}

		Vector2 aim = playerInput.GetAim();
		ChangeCameraPitch(-aim.Y * TurnSpeed * dt);
		ChangeCameraYaw(-aim.X * TurnSpeed * dt);
		var angle = Mathf.MoveToward(camera.Rotation.Z, inputDir.X * maxTiltAngle, dt * tiltAngleSpeed);
		SetCameraRoll(angle);
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
	void ChangeCameraPitch(float da){
		Vector3 cameraRotation = camera.Rotation;
		cameraRotation.X = Mathf.Clamp(cameraRotation.X + da, minCameraAngle, maxCameraAngle);
		camera.Rotation = cameraRotation;
	}
	void ChangeCameraYaw(float da){
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
		hud.Text = $"Health: {Health}\nAmmo: {100}\nFuel: {fuel}";
	}
}
