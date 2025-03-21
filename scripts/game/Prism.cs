using Godot;
using System;

public partial class Prism : Actor
{
	public override void Die()
	{
		base.Die();
		QueueFree();
	}
}
