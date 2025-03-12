using Godot;
using System;

public partial class Player : Entity
{
	[Export] public float Speed = 5.0f;
	[Export] public float TurnSpeed = 5.0f;
	[Export] public float JumpVelocity = 4.5f;
	readonly PlayerInput playerInput = new();
	Camera3D camera;
	[Export] float minCameraAngle = -1;
	[Export] float maxCameraAngle = 1;
	[Export] float mouseSensitivity = 1;
	[Export] float coyoteTime = 0.1f;
	[Export] float jumpBuffer = 0.1f;
	[Export] int maxAirJumps = 1;
	[Export] float dashTime = 0.5f;
	[Export] float dashSpeed = 10.0f;
	[Export] int maxAirDashes = 1;
	// [Export] float fov = 75.0f;
	// [Export] float dashFov = 70.0f;
	int airDashes = 0;
	int airJumps = 0;
	Clock groundClock;
	Clock jumpClock;
	Clock dashClock;

	public override void _Ready()
	{
		base._Ready();
		camera = GetNode<Camera3D>("Camera3D");
		groundClock = AddClock(coyoteTime, 0);
		jumpClock = AddClock(jumpBuffer, 0);
		dashClock = AddClock(dashTime, 0);
		// dashClock.Timeout = ()=>{camera.Fov = fov;};
		// TODO: handle elsewhere
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}
	public override void _Input(InputEvent @event)
	{
		if(Input.MouseMode == Input.MouseModeEnum.Captured){
			if (@event is InputEventMouseMotion mouseMotion){
				var motion = mouseMotion.Relative * mouseSensitivity * 0.001f;
				ChangeCameraYaw(-motion.X);
				ChangeCameraPitch(-motion.Y);
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		float dt = (float)delta;
		if(IsOnFloor()) {
			groundClock.Reset();
			airJumps = maxAirJumps;
			airDashes = maxAirDashes;
		}
		playerInput.Poll();
		if(playerInput.JumpJustPressed()) jumpClock.Reset();
		if(playerInput.PauseJustPressed()) GetTree().Quit();
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor() && !dashClock.IsRunning())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		bool shouldJump = false;
		if(jumpClock.IsRunning()){
			if(groundClock.IsRunning()) shouldJump = true;
			else if(airJumps > 0){
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
		if(playerInput.DashJustPressed()){
			if(groundClock.IsRunning()) shouldDash = true;
			else if(airDashes > 0){
				airDashes--;
				shouldDash = true;
			}
		}
		if(shouldDash){
			Vector3 dir = (camera.GlobalTransform.Basis * Vector3.Forward).Normalized();
			velocity = dir * dashSpeed;
			dashClock.Reset();
			// camera.Fov = dashFov;
		}

		Vector2 aim = playerInput.GetAim();
		ChangeCameraPitch(-aim.Y * TurnSpeed * dt);
		ChangeCameraYaw(-aim.X * TurnSpeed * dt);
		Vector2 inputDir = playerInput.GetMove();
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
}
