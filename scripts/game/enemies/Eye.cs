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
	[Export] float WakeRadius = 100;
	[Export] float FOV = 90;
	[Export] PackedScene BulletScene;
	EyeState lastState = EyeState.Idle;
	Vector3 lookPoint;
	Player player;
	Clock fireClock;
	EntPool bulletPool;
	RayCast3D ray;
	PhysicsDirectSpaceState3D spaceState;
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
		ray = GetNode<RayCast3D>("RayCast3D");
		ray.TargetPosition = Vector3.Forward * WakeRadius;
	}
	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		float dt = (float)delta;
		spaceState = GetWorld3D().DirectSpaceState;
		var tempState = state;
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
		lastState = tempState;
	}
	public override void Damage(float value)
	{
		base.Damage(value);
		if(Health >= 0) GetNode<AudioStreamPlayer3D>("PainSound").Play();
	}
	public override void Die()
	{
		base.Die();
		// QueueFree();
		var sound = GetNode<AudioStreamPlayer3D>("DeathSound");
		if(sound.Playing) return;
		sound.Play();
		sound.Finished += QueueFree;
		state = EyeState.Idle;
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
	bool CanSeePlayer(){
		if((player.Position - Position).Length() > WakeRadius) return false;
		if(AngleToTarget(player.Position) > Mathf.DegToRad(FOV / 2)) return false;
		var query = PhysicsRayQueryParameters3D.Create(Position, player.Position);
		var result = spaceState.IntersectRay(query);
		if(result.Count == 0) return false;
		var collider = (GodotObject)result["collider"];
		if(collider is Actor a && a == player){
			return true;
		}
		return false;
	}
	void Idle(float dt){}
	void Spinning(float dt){
		Rotate(Vector3.Up, TurnSpeed * dt);
	}
	void Seeking(float dt){
		if(lastState != EyeState.Seeking){
			RandomizeLookPoint();
			GetNode<AudioStreamPlayer3D>("SeekSound").Play();
		}
		float angle = AngleToTarget(lookPoint);
		if(angle < 0.1f){
			RandomizeLookPoint();
		}
		LookToTarget(dt, lookPoint);
		if(CanSeePlayer()) state = EyeState.Attacking;
	}
	void Attacking(float dt){
		if(lastState != EyeState.Attacking){
			GetNode<AudioStreamPlayer3D>("WakeSound").Play();
		}
		LookToTarget(dt, player.Position);
		Fire();
		if(!CanSeePlayer()) state = EyeState.Seeking;
	}
}
