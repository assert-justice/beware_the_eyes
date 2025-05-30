using Godot;
using System;
using System.Collections.Generic;

public partial class Entity : Node3D
{
	protected int Team = 0;
	List<Clock> clocks = [];
	EntPool pool;
	public Vector3 Velocity;
	public Clock AddClock(float fullDuration, float duration = -1){
		Clock c = new(fullDuration, duration);
		clocks.Add(c);
		return c;
	}
	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		foreach (var c in clocks)
		{
			c.Update((float)delta);
		}
		Position += Velocity * (float)delta;
	}
	public virtual void Init(){}
	public void SetPool(EntPool entPool){
		pool = entPool;
	}
	public virtual void Die(){
		if(pool == null) return;
		pool.GetPool().Free(this);
	}
	public int GetTeam(){return Team;}
	// public void SetParent(Node parent){
	// TODO: buggy, fix it
	// 	// if(parent == this) return;
	// 	GetParent().RemoveChild(this);
	// 	parent.CallDeferred("add_child", this);
	// }
}
