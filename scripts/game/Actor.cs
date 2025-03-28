using Godot;
using System;
using System.Collections.Generic;

public abstract partial class Actor : CharacterBody3D
{
    List<Clock> clocks = [];
    List<EntPool> pools = [];
    List<Entity> entList = new();
    Queue<Entity> entQueue = new();
    protected float Health = 100;
    protected int Team = 0;
    public bool IsZoomed = false;
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
    public void AddEntChild(Entity entity){
        entQueue.Enqueue(entity);
    }
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        foreach (var c in clocks)
        {
            c.Update((float)delta);
        }
        while (entQueue.Count > 0){
            var ent = entQueue.Dequeue();
            var parent = ent.GetParent();
            if(parent != this){
                parent?.RemoveChild(ent);
                AddChild(ent);
                entList.Add(ent);
            }
        }
    }
    public virtual void Damage(float value){
        Health -= value;
        if(Health < 0) Die();
    }
    public virtual void Die(){
        foreach (var ent in entList)
        {
            ent.Die();
        }
    }
    public int GetTeam(){return Team;}
}
