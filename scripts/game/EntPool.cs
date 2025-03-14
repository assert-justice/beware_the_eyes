using System;
using Godot;

public class EntPool{
    Pool<Entity> pool;
    public EntPool(Node parent, Func<Entity> newFn){
        pool = new(()=>{
            var e = newFn();
            e.SetPool(this);
            return e;
        }){
            InitFn = e =>{
                parent.AddChild(e);
                e.Init();
            },
            FreeFn = e =>{
                parent.RemoveChild(e);
            }
        };
    }
    public Pool<Entity> GetPool(){
        return pool;
    }
}