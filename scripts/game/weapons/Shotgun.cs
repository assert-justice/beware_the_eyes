using Godot;

public partial class Shotgun : Weapon
{
	[Export] float FireTime = 0.5f;
	[Export] float AltFireTime = 1.5f;
	[Export] float Damage = 50;
	[Export] float DamageMul = 2;
	[Export] PackedScene SparkleScene;
	Clock fireClock;
	Clock altFireClock;
	EntPool sparklePool;
	AudioStreamPlayer3D fireSound;
	AudioStreamPlayer3D altFireSound;
	public override void _Ready()
	{
		base._Ready();
		var temp = GetTree().GetNodesInGroup("Game");
		Node parent; 
		if(temp.Count > 0) parent = temp[0];
		else parent = GetTree().Root;
		sparklePool = AddPool(parent, ()=>SparkleScene.Instantiate<Entity>());
		fireClock = AddClock(FireTime, 0);
		altFireClock = AddClock(AltFireTime, 0);
		fireSound = GetNode<AudioStreamPlayer3D>("FireSound");
		altFireSound = GetNode<AudioStreamPlayer3D>("AltFireSound");
	}
	public override void TryFire(FireCommand command)
	{
		if(fireClock.IsRunning()) return;
		if(altFireClock.IsRunning()) return;
		if(CanFire(command)) Fire(command);
		else if(CanAltFire(command)) AltFire(command);
	}
	bool CanFire(FireCommand command){
		return command.FireJustPressed;
	}
	bool CanAltFire(FireCommand command){
		return command.AltJustPressed;
	}
	void Fire(FireCommand command){
		fireClock.Reset();
		fireSound.Play();
		if(command.Ray.IsColliding()){
			var pos = command.Ray.GetCollisionPoint();
			var s = sparklePool.GetPool().GetNew();
			s.Position = pos;
			if(command.Ray.GetCollider() is Actor actor){
				actor.Damage(Damage);
			}
		}
	}
	void AltFire(FireCommand command){
		altFireClock.Reset();
		altFireSound.Play();
		if(command.Ray.IsColliding()){
			var pos = command.Ray.GetCollisionPoint();
			var s = sparklePool.GetPool().GetNew();
			s.Position = pos;
			if(command.Ray.GetCollider() is Actor actor){
				actor.Damage(Damage * DamageMul);
			}
		}
	}
}
