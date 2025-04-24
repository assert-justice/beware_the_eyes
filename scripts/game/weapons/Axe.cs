using Godot;
using System;

public partial class Axe : Weapon
{
	[Export] float FireTime = 0.5f;
	[Export] float Damage = 10;
	[Export] float Range = 2;
	Clock fireClock;
	public override void _Ready()
	{
		base._Ready();
		fireClock = AddClock(FireTime, 0);
	}
	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		Visible = fireClock.IsRunning();
	}
	public override void TryFire(FireCommand command)
	{
		if (CanFire(command)) Fire(command);
	}
	bool CanFire(FireCommand command){
		return !fireClock.IsRunning() && command.MeleeJustPressed;
	}
	void Fire(FireCommand command){
		fireClock.Reset();
		// fireSound.Play();
		if(command.Ray.IsColliding()){
			var pos = command.Ray.GetCollisionPoint();
			if((pos - GlobalPosition).Length() > Range) return;
			if(command.Ray.GetCollider() is Actor actor){
				actor.Damage(Damage);
				// GD.Print("axe damage!");
			}
		}
	}
}
