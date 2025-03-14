using Godot;
using System;
using System.Collections.Generic;

public abstract partial class Actor : CharacterBody3D
{
    List<Clock> clocks = [];
    List<EntPool> pools = [];
    float Health = 100;
    int Team = 0;
    public Clock AddClock(float fullDuration, float duration = -1){
        Clock c = new(fullDuration, duration);
        clocks.Add(c);
        return c;
    }
    public EntPool AddPool(Node parent, Func<Entity> newFn){
        EntPool e = new(parent, newFn);
        pools.Add(e);
        return e;
    }
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        foreach (var c in clocks)
        {
            c.Update((float)delta);
        }
        // MoveAndSlide();
    }
    public virtual void Damage(float value){
        Health -= value;
        if(Health < 0) Die();
    }
    public virtual void Die(){}
    public int GetTeam(){return Team;}
}
