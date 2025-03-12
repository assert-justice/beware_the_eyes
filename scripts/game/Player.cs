using Godot;
using System;

public partial class Player : Entity
{
	[Export] public float Speed = 5.0f;
	[Export] public float TurnSpeed = 5.0f;
	[Export] public float JumpVelocity = 4.5f;
	PlayerInput playerInput = new();
	Camera3D camera;
	[Export] float minCameraAngle = -1;
	[Export] float maxCameraAngle = 1;
	[Export] float mouseSensitivity = 1;
	[Export] float coyoteTime = 0.1f;
	[Export] float jumpBuffer = 0.1f;
	[Export] int maxAirJumps = 1;
	int airJumps = 0;
	Clock groundClock;
	Clock jumpClock;

	public override void _Ready()
	{
		base._Ready();
		camera = GetNode<Camera3D>("Camera3D");
		groundClock = AddClock(coyoteTime, 0);
		jumpClock = AddClock(jumpBuffer, 0);
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
		}
		playerInput.Poll();
		if(playerInput.JumpJustPressed()) jumpClock.Reset();
		if(playerInput.PauseJustPressed()) GetTree().Quit();
		Vector3 velocity = Velocity;
		// Vector3 rotation = Rotation;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		// GD.Print(jumpClock.GetDuration());
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

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		// Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Vector2 aim = playerInput.GetAim();
		// rotation.Y -= aim.X * TurnSpeed * dt;
		ChangeCameraPitch(-aim.Y * TurnSpeed * dt);
		ChangeCameraYaw(-aim.X * TurnSpeed * dt);
		Vector2 inputDir = playerInput.GetMove();
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
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
