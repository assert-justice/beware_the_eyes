using Godot;
using System;

public abstract partial class Weapon : Node3D
{
    bool Enabled = false;
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
