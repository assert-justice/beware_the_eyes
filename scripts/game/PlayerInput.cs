using Godot;

public class PlayerInput{
    Vector2 move;
    Vector2 aim;
    bool jumpJustPressed;
    bool jumpPressed;
    bool pauseJustPressed;
    bool dashJustPressed;
    bool firePressed;
    bool fireJustPressed;
    bool[] weaponSelected = [false, false, false, false];
    public void Poll(){
        if(Globals.Instance.UseKeyboard){
            move = Input.GetVector("kb_move_left", "kb_move_right", "kb_move_up", "kb_move_down");
            if(move.Length() > 0) move = move.Normalized();
            aim = Input.GetVector("kb_aim_left", "kb_aim_right", "kb_aim_up", "kb_aim_down");
            if(aim.Length() > 0) aim = aim.Normalized();
            jumpJustPressed = Input.IsActionJustPressed("kb_jump");
            jumpPressed = Input.IsActionPressed("kb_jump");
            pauseJustPressed = Input.IsActionJustPressed("kb_pause");
            dashJustPressed = Input.IsActionJustPressed("kb_dash");
            firePressed = Input.IsActionPressed("kb_fire");
            fireJustPressed = Input.IsActionJustPressed("kb_fire");
            weaponSelected[0] = Input.IsActionJustPressed("kb_wep_1");
            weaponSelected[1] = Input.IsActionJustPressed("kb_wep_2");
            weaponSelected[2] = Input.IsActionJustPressed("kb_wep_3");
            weaponSelected[3] = Input.IsActionJustPressed("kb_wep_4");
        }
        else{
            move = Input.GetVector("pad_move_left", "pad_move_right", "pad_move_up", "pad_move_down");
            aim = Input.GetVector("pad_aim_left", "pad_aim_right", "pad_aim_up", "pad_aim_down");
            jumpJustPressed = Input.IsActionJustPressed("pad_jump");
            jumpPressed = Input.IsActionPressed("pad_jump");
            pauseJustPressed = Input.IsActionJustPressed("pad_pause");
            dashJustPressed = Input.IsActionJustPressed("pad_dash");
            firePressed = Input.IsActionPressed("pad_fire");
            fireJustPressed = Input.IsActionJustPressed("pad_fire");
            weaponSelected[0] = Input.IsActionJustPressed("pad_wep_1");
            weaponSelected[1] = Input.IsActionJustPressed("pad_wep_2");
            weaponSelected[2] = Input.IsActionJustPressed("pad_wep_3");
            weaponSelected[3] = Input.IsActionJustPressed("pad_wep_4");
        }
    }
    public Vector2 GetMove(){return move;}
    public Vector2 GetAim(){return aim;}
    public bool JumpJustPressed(){return jumpJustPressed;}
    public bool JumpPressed(){return jumpPressed;}
    public bool PauseJustPressed(){return pauseJustPressed;}
    public bool DashJustPressed(){return dashJustPressed;}
    public bool FirePressed(){return firePressed;}
    public bool FireJustPressed(){return fireJustPressed;}
    public bool[] GetWeaponMask(){
        return weaponSelected;
    }
}
