using Godot;

public partial class Zapper : Weapon
{
	[Export] float Ammo = 20;
	[Export] float MaxAmmo = 100;
	[Export] float AmmoRate = 10;
	[Export] float DPS = 200;
	AudioStreamPlayer3D fireSound;
	MeshInstance3D beam;
	public override void _Ready()
	{
		base._Ready();
		fireSound = GetNode<AudioStreamPlayer3D>("FireSound");
		beam = GetNode<MeshInstance3D>("Beam");
	}
	public override string GetAmmoString()
	{
		return $"{(int)Ammo}/{MaxAmmo}";
	}
	public override bool AddAmmo()
	{
		if(Ammo == MaxAmmo) return false;
		Ammo += 20;
		if(Ammo > MaxAmmo) Ammo = MaxAmmo;
		return true;
	}
	public override void TryFire(FireCommand command)
	{
		if(CanFire(command)) Fire(command);
		else{beam.Visible = false;}
	}
	bool CanFire(FireCommand command){
		if(Ammo <= 0) return false;
		return command.FirePressed;
	}
	void Fire(FireCommand command){
		beam.Visible = true;
		if(!fireSound.Playing) fireSound.Play();
		Ammo -= command.dt * AmmoRate;
		var beamScale = beam.Scale;
		var beamPos = beam.Position;
		if(command.Ray.IsColliding()){
			if(command.Ray.GetCollider() is Actor actor){
				actor.Damage(DPS * command.dt);
			}
			var pos = command.Ray.GetCollisionPoint();
			pos = ToLocal(pos);
			beamPos.Z = Mathf.Lerp(pos.Z, Position.Z, 0.5f);
			beamScale.Z = pos.Z - Position.Z;
		}
		else{
			beamScale.Z = 1000;
		}
		beam.Scale = beamScale;
		beam.Position = beamPos;
	}
}
