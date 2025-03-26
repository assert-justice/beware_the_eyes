using Godot;
using System;

public partial class Spike : Entity
{
    Clock lifetime;
    public override void _Ready()
    {
        base._Ready();
        lifetime = AddClock(10);
        lifetime.Timeout = Die;
    }
}
