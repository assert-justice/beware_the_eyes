using Godot;

public partial class Launcher : Weapon
{
	[Export] float FireTime = 0.5f;
	[Export] PackedScene MineScene;
	[Export] int Ammo = 12;
	[Export] int MaxAmmo = 100;
	[Export] int MaxActive = 4;
	Clock fireClock;
	EntPool minePool;
	AudioStreamPlayer3D fireSound;
	public override void _Ready()
	{
		base._Ready();
		var temp = GetTree().GetNodesInGroup("Game");
		Node parent; 
		if(temp.Count > 0) parent = temp[0];
		else parent = GetTree().Root;
		minePool = AddPool(parent, ()=>MineScene.Instantiate<Entity>());
		fireClock = AddClock(FireTime, 0);
		fireSound = GetNode<AudioStreamPlayer3D>("FireSound");
	}
	public override void TryFire(FireCommand command)
	{
		if(CanFire(command)) Fire(command);
		if(CanAltFire(command)) AltFire(command);
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
		if(fireClock.IsRunning()) return false;
		if(minePool.GetPool().CountAlive() >= MaxActive) return false;
		return command.FireJustPressed;
	}
	bool CanAltFire(FireCommand command){
		if(minePool.GetPool().CountAlive() == 0) return false;
		return command.AltJustPressed;
	}
	void Fire(FireCommand command){
		fireClock.Reset();
		fireSound.Play();
		Ammo--;
		if(command.Ray.IsColliding()){
			// var parent = command.Ray.GetCollider() as Node;
			var pos = command.Ray.GetCollisionPoint();
			var mine = minePool.GetPool().GetNew();
			// mine.SetParent(parent);
			mine.Position = pos;
		}
	}
	void AltFire(FireCommand command){
		foreach (var item in minePool.GetPool().GetAlive())
		{
			var mine = item as Mine;
			mine.Detonate();
		}
	}
}
