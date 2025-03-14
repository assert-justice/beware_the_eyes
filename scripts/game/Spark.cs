using Godot;
using System;

public partial class Spark : Entity
{
	CpuParticles3D particles;
	public override void _Ready()
	{
		base._Ready();
		particles = GetNode<CpuParticles3D>("CPUParticles3D");
		particles.Finished += Die;
	}
	public override void Init()
	{
		particles.Restart();
	}
}
