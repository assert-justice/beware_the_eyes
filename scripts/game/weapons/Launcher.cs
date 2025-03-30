using System.Collections.Generic;
using Godot;

public partial class Launcher : Weapon
{
	[Export] float FireTime = 0.5f;
	[Export] PackedScene MineScene;
	[Export] int Ammo = 12;
	[Export] int MaxAmmo = 100;
	[Export] int MaxActive = 4;
	Clock fireClock;
	// EntPool minePool;
	Node root;
	AudioStreamPlayer3D fireSound;
	List<Mine> liveMines = new();
	public override void _Ready()
	{
		base._Ready();
		var temp = GetTree().GetNodesInGroup("Game");
		if(temp.Count > 0) root = temp[0];
		else root = GetTree().Root;
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
		if(liveMines.Count >= MaxActive) return false;
		return command.FireJustPressed;
	}
	bool CanAltFire(FireCommand command){
		if(liveMines.Count == 0) return false;
		return command.AltJustPressed;
	}
	void Fire(FireCommand command){
		fireClock.Reset();
		fireSound.Play();
		Ammo--;
		if(command.Ray.IsColliding()){
			var parent = command.Ray.GetCollider() as Node;
			var pos = command.Ray.GetCollisionPoint();
			var mine = MineScene.Instantiate<Mine>();
			if(parent is Actor actor){
				actor.AddChild(mine);
			}
			else{
				root.AddChild(mine);
			}
			liveMines.Add(mine);
			mine.GlobalPosition = pos;
		}
	}
	void AltFire(FireCommand command){
		foreach (var mine in liveMines)
		{
			mine.Detonate();
		}
		liveMines.Clear();
	}
}
