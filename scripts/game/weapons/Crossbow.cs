using Godot;

public partial class Crossbow : Weapon
{
	[Export] float FireTime = 0.5f;
	[Export] float Damage = 50;
	[Export] PackedScene SparkleScene;
	[Export] PackedScene SpikeScene;
	[Export] int Ammo = 6;
	[Export] int MaxAmmo = 100;
	Clock fireClock;
	AudioStreamPlayer3D fireSound;
	EntPool sparklePool;
	EntPool spikePool;
	public override void _Ready()
	{
		base._Ready();
		fireClock = AddClock(FireTime, 0);
		fireSound = GetNode<AudioStreamPlayer3D>("FireSound");
		var temp = GetTree().GetNodesInGroup("Game");
		Node parent; 
		if(temp.Count > 0) parent = temp[0];
		else parent = GetTree().Root;
		sparklePool = AddPool(parent, ()=>SparkleScene.Instantiate<Entity>());
		spikePool = AddPool(parent, ()=>SpikeScene.Instantiate<Entity>());
	}
    public override string GetAmmoString()
    {
		return $"{Ammo}/{MaxAmmo}";
    }
    public override bool AddAmmo()
    {
		if(Ammo == MaxAmmo) return false;
		Ammo += 6;
		if(Ammo > MaxAmmo) Ammo = MaxAmmo;
		return true;
    }
    public override void TryFire(FireCommand command)
	{
		// throw new System.NotImplementedException();
		if(CanFire(command)) Fire(command);
		if(command.AltJustPressed){
			actor.IsZoomed = !actor.IsZoomed;
		}
	}
	bool CanFire(FireCommand command){
		if(Ammo < 1) return false;
		if(fireClock.IsRunning()) return false;
		return command.FireJustPressed;
	}
	void Fire(FireCommand command){
		fireClock.Reset();
		fireSound.Play();
		Ammo--;
		if(command.Ray.IsColliding()){
			var pos = command.Ray.GetCollisionPoint();
			var s = sparklePool.GetPool().GetNew();
			var spike = spikePool.GetPool().GetNew();
			spike.Position = pos;
			spike.GlobalRotation = GlobalRotation;
			s.Position = pos;
			if(command.Ray.GetCollider() is Actor actor){
				actor.Damage(Damage);
			}
		}
	}	
	public override void UnMount()
	{
		base.UnMount();
		actor.IsZoomed = false;
	}
}
