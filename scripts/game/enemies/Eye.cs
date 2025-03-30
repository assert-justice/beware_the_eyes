using System;
using Godot;

public enum EyeState{
	Idle,
	Spinning,
	Seeking,
	Attacking,
}

public partial class Eye : Actor
{
	[Export] public EyeState state = EyeState.Idle;
	[Export] float TurnSpeed = 1;
	[Export] float BulletSpeed = 30;
	[Export] PackedScene BulletScene;
	EyeState lastState = EyeState.Idle;
	Vector3 lookPoint;
	Player player;
	Clock fireClock;
	EntPool bulletPool;
	public override void _Ready()
	{
		base._Ready();
		Team = 1;
		Node parent;
		var temp = GetTree().GetNodesInGroup("Game");
		if(temp.Count > 0) parent = temp[0];
		else parent = GetTree().Root;
		player = GetTree().GetNodesInGroup("Player")[0] as Player;
		fireClock = AddClock(0.3f);
		bulletPool = AddPool(parent, ()=>BulletScene.Instantiate<Entity>());
	}
	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		float dt = (float)delta;
		switch (state)
		{
			case EyeState.Idle:
			Idle(dt);
			break;
			case EyeState.Spinning:
			Spinning(dt);
			break;
			case EyeState.Seeking:
			Seeking(dt);
			break;
			case EyeState.Attacking:
			Attacking(dt);
			break;
			default:
			GD.PrintErr("Invalid eye state");
			break;
		}
		lastState = state;
	}
	public override void Die()
	{
		base.Die();
		QueueFree();
	}
	float AngleToTarget(Vector3 target){
		var a = GlobalTransform.Basis.GetRotationQuaternion();
		var b = GlobalTransform.LookingAt(target, Vector3.Up).Basis.GetRotationQuaternion();
		return a.AngleTo(b);
	}
	void LookToTarget(float dt, Vector3 target){
		var a = GlobalTransform.Basis.GetRotationQuaternion();
		var b = GlobalTransform.LookingAt(target, Vector3.Up).Basis.GetRotationQuaternion();
		float da = TurnSpeed * dt;
		float angle = a.AngleTo(b);
		float weight = da / angle;
		if(weight > 1) weight = 1;
		var c = a.Slerp(b, weight);
		var transform = Transform;
		transform.Basis = new(c);
		Transform = transform;
	}
	void RandomizeLookPoint(){
		lookPoint = GameMath.PointOnSphere() + Position;
	}
	void Fire(){
		if(fireClock.IsRunning()) return;
		fireClock.Reset();
		var bullet = bulletPool.GetPool().GetNew();
		bullet.Position = Position;
		bullet.Velocity = (Transform.Basis * Vector3.Forward).Normalized() * BulletSpeed;
	}
	void Idle(float dt){}
	void Spinning(float dt){
		Rotate(Vector3.Up, TurnSpeed * dt);
	}
	void Seeking(float dt){
		if(lastState != EyeState.Seeking){
			RandomizeLookPoint();
		}
		float angle = AngleToTarget(lookPoint);
		if(angle < 0.1f){
			RandomizeLookPoint();
		}
		LookToTarget(dt, lookPoint);
	}
	void Attacking(float dt){
		LookToTarget(dt, player.Position);
		Fire();
	}
}
