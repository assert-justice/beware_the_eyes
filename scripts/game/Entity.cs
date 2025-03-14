using Godot;
using System;
using System.Collections.Generic;

public abstract partial class Entity : Node3D
{
    List<Clock> clocks = [];
    EntPool pool;
    Vector3 Velocity;
    int Team = 0;
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
    public abstract void Init();
    public void SetPool(EntPool entPool){
        pool = entPool;
    }
    public virtual void Die(){
        if(pool == null) return;
        pool.GetPool().Free(this);
    }
    public int GetTeam(){return Team;}
}
