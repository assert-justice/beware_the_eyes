using Godot;

public partial class Crossbow : Weapon
{
	[Export] float FireTime = 0.5f;
	[Export] float Damage = 50;
	[Export] PackedScene SpikeScene;
	[Export] int Ammo = 6;
	[Export] int MaxAmmo = 100;
	Clock fireClock;
	AudioStreamPlayer3D fireSound;
	Node root;
	public override void _Ready()
	{
		base._Ready();
		fireClock = AddClock(FireTime, 0);
		fireSound = GetNode<AudioStreamPlayer3D>("FireSound");
		var temp = GetTree().GetNodesInGroup("Game");
		if(temp.Count > 0) root = temp[0];
		else root = GetTree().Root;
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
			var spike = SpikeScene.Instantiate<Spike>();
			if(command.Ray.GetCollider() is Actor actor){
				actor.Damage(Damage);
				actor.AddChild(spike);
			}
			else{
				root.AddChild(spike);
			}
			spike.GlobalPosition = pos;
			spike.GlobalRotation = GlobalRotation;
		}
	}	
	public override void UnMount()
	{
		base.UnMount();
		actor.IsZoomed = false;
	}
}
