using Godot;
using System.Collections.Generic;

public partial class Notification : Label
{
    [Export] float Delay = 1;
    Queue<string> messages;
    Clock clock;

    public override void _Ready()
    {
        base._Ready();
        messages = new();
        clock = new(Delay, 0);
        clock.Timeout = ()=>{
            if(messages.Count > 0){
                SetMessage(messages.Dequeue());
                clock.Reset();
            }
            else{Text = "";}
        };
        Text = "";
    }
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        clock.Update((float)delta);
        if(clock.GetDuration() == 0 && messages.Count > 0){
            SetMessage(messages.Dequeue());
            clock.Reset();
        }
    }
    public void AddMessage(string message){
        messages.Enqueue(message);
    }
    void SetMessage(string message){
        Text = message;
    }
}
