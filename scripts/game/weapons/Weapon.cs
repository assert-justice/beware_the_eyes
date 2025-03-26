using Godot;
using System;
using System.Collections.Generic;

public abstract partial class Weapon : Node3D
{
    bool Enabled = false;
    List<Clock> clocks = [];
    List<EntPool> pools = [];
    protected Actor actor;
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
    protected EntPool AddPool(Node parent, Func<Entity> fn){
        EntPool pool = new(parent, fn);
        pools.Add(pool);
        return pool;
    }
    // public abstract bool CanFire(FireCommand command);
    // protected abstract void Fire(FireCommand command);
    public abstract void TryFire(FireCommand command);
    public virtual void Mount(){
        Visible = true;
    }
    public virtual void UnMount(){
        Visible = false;
    }
    public virtual string GetAmmoString(){
        return "-/-";
    }
    public virtual bool AddAmmo(){return false;}
    public bool IsEnabled(){return Enabled;}
    public void Enable(){Enabled = true;}
    public void SetActor(Actor a){
        actor = a;
    }
}
