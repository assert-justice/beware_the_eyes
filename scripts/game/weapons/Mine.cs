using Godot;
using System;

public partial class Mine : Entity
{
	[Export] float Damage = 150;
	AudioStreamPlayer3D sound;
	Area3D area;
	CollisionShape3D shape;
	public override void _Ready()
	{
		base._Ready();
		sound = GetNode<AudioStreamPlayer3D>("Sound");
		sound.Finished += Die;
		area = GetNode<Area3D>("Area3D");
		area.BodyEntered += b=>{
			if(b is Actor a){
				a.Damage(Damage);
				// TODO: apply physics, damage falloff
			}
		};
		shape = GetNode<CollisionShape3D>("Area3D/CollisionShape3D");
	}
	public override void Die()
	{
		QueueFree();
	}
	// public override void Init()
	// {
	// 	base.Init();
	// 	Visible = true;
	// 	shape.Disabled = true;
	// }
	public void Detonate(){
		if(sound.Playing) return;
		shape.Disabled = false;
		sound.Play();
		Visible = false;
	}
}
