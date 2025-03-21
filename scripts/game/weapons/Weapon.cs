using Godot;
using System;
using System.Collections.Generic;

public abstract partial class Weapon : Node3D
{
    bool Enabled = false;
    List<Clock> clocks = [];
    List<EntPool> pools = [];
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        foreach (var clock in clocks)
        {
            clock.Update((float)delta);
        }
    }
    public Clock AddClock(float fullDuration, float duration = -1){
        Clock clock = new(fullDuration, duration);
        clocks.Add(clock);
        return clock;
    }
    public EntPool AddPool(Node parent, Func<Entity> fn){
        EntPool pool = new(parent, fn);
        pools.Add(pool);
        return pool;
    }
    public abstract bool CanFire(FireCommand command);
    protected abstract void Fire(FireCommand command);
    public void TryFire(FireCommand command){
        if(CanFire(command)) Fire(command);
    }
    public virtual void Mount(){
        Visible = true;
    }
    public virtual void UnMount(){
        Visible = false;
    }
    public bool IsEnabled(){return Enabled;}
    public void Enable(){Enabled = true;}
}
