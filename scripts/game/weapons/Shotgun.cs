using Godot;

public partial class Shotgun : Weapon
{
	[Export] float FireTime = 0.5f;
	[Export] float Damage = 50;
	[Export] PackedScene SparkleScene;
	Clock fireClock;
	EntPool sparklePool;
	public override void _Ready()
	{
		base._Ready();
		var parent = GetTree().GetNodesInGroup("Game")[0];
		fireClock = AddClock(FireTime, 0);
		sparklePool = AddPool(parent, ()=>SparkleScene.Instantiate<Entity>());
	}
	public override void TryFire(FireCommand command)
	{
		if(CanFire(command)) Fire(command);
		else if(CanAltFire(command)) AltFire(command);
	}
	bool CanFire(FireCommand command){
		if(fireClock.IsRunning()) return false;
		return command.FireJustPressed;
	}
	bool CanAltFire(FireCommand command){
		if(fireClock.IsRunning()) return false;
		return command.AltJustPressed;
	}
	void Fire(FireCommand command){
		fireClock.Reset();
		if(command.Ray.IsColliding()){
			var pos = command.Ray.GetCollisionPoint();
			var s = sparklePool.GetPool().GetNew();
			s.Position = pos;
			if(command.Ray.GetCollider() is Actor actor){
				actor.Damage(Damage);
			}
		}
	}
	void AltFire(FireCommand command){}
}
