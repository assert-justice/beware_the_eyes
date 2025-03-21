
using Godot;

public abstract partial class Menu: Control{
    protected MenuSystem menuSystem;
    public override void _Ready()
    {
        menuSystem = GetParent<MenuSystem>();
        VisibilityChanged += () => {
            if(Visible) OnWake();
            else OnSleep();
        };
    }
    static bool FocusHelp(Node n){
        if (n is Control c){
                if(c.FocusMode == FocusModeEnum.All){
                    c.CallDeferred("grab_focus");
                    return true;
                }
            }
            foreach (var child in n.GetChildren())
            {
                if(FocusHelp(child)) return true;
            }
            return false;
    }
    public virtual void OnWake(){
        // Find and focus on first thing that can be focused
        FocusHelp(this);
    }
    public virtual void OnSleep(){}
}