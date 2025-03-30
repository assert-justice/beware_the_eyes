using Godot;

public partial class Shotgun : Weapon
{
	[Export] float FireTime = 0.5f;
	[Export] float AltFireTime = 1.5f;
	[Export] float Damage = 50;
	[Export] float DamageMul = 2;
	[Export] int Ammo = 12;
	[Export] int MaxAmmo = 100;
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
	public override string GetAmmoString()
	{
		return $"{Ammo}/{MaxAmmo}";
	}
	public override bool AddAmmo()
	{
		if(Ammo == MaxAmmo) return false;
		Ammo += 12;
		if(Ammo > MaxAmmo) Ammo = MaxAmmo;
		return true;
	}
	bool CanFire(FireCommand command){
		if(Ammo < 1) return false;
		return command.FireJustPressed;
	}
	bool CanAltFire(FireCommand command){
		if(Ammo < 2) return false;
		return command.AltJustPressed;
	}
	void Fire(FireCommand command){
		fireClock.Reset();
		fireSound.Play();
		Ammo--;
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
		Ammo -= 2;
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
