using Godot;
using System;

public partial class EyeBullet : Entity
{
	[Export] public float Damage = 10;
	Clock lifetime;
	public override void _Ready()
	{
		base._Ready();
		Team = 1;
		GetNode<Area3D>("Area3D").BodyEntered += b => {
			if(b is Actor a){
				if(a.GetTeam() != GetTeam()){
					a.Damage(Damage);
					CallDeferred("Die");
				}
			}
			else CallDeferred("Die");
		};
		lifetime = AddClock(10);
		lifetime.Timeout = Die;
	}
	public override void Init()
	{
		base.Init();
		lifetime.Reset();
	}
}
