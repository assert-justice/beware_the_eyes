using System;
using Godot;

public enum EyeState{
	Idle,
	Spinning,
	Seeking,
}

public partial class Eye : Actor
{
	[Export] public EyeState state = EyeState.Idle;
	[Export] float TurnSpeed = 1;
	EyeState lastState = EyeState.Idle;
	Vector3 lookPoint;
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
			default:
			break;
		}
		lastState = state;
	}
	public override void Die()
	{
		base.Die();
		QueueFree();
	}
	void Idle(float dt){}
	void Spinning(float dt){
		Rotate(Vector3.Up, TurnSpeed * dt);
	}
	void Seeking(float dt){
		if(lastState != EyeState.Seeking){
			lookPoint = GameMath.PointOnSphere() + Position;
		}
		var a = GlobalTransform.Basis.GetRotationQuaternion();
		var b = GlobalTransform.LookingAt(lookPoint, Vector3.Up).Basis.GetRotationQuaternion();
		float angle = a.AngleTo(b);
		if(angle < 0.1f){
			lookPoint = GameMath.PointOnSphere() * 100 + Position;
		}
		else{
			float da = TurnSpeed * dt;
			float weight = da / angle;
			if(weight > 1) weight = 1;
			var c = a.Slerp(b, weight);
			var transform = Transform;
			transform.Basis = new(c);
			Transform = transform;
		}
	}
}
