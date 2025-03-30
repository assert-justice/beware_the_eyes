using Godot;
using System;

public partial class EyeBullet : Entity
{
    [Export] public float Damage = 10;
    public override void _Ready()
    {
        base._Ready();
        Team = 1;
        GetNode<Area3D>("Area3D").BodyEntered += b => {
            if(b is Actor a && a.GetTeam() != GetTeam()){
                a.Damage(Damage);
            }
        };
    }
}
