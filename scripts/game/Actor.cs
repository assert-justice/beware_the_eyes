using Godot;
using System;
using System.Collections.Generic;

public partial class Actor : CharacterBody3D
{
    List<Clock> clocks = [];
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
        // MoveAndSlide();
    }
}
