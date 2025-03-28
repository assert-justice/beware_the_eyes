using Godot;

public static class GameMath{
    public static Vector3 PointOnSphere(){
        while(true){
            Vector3 vec = new(GD.Randf() * 2 - 1, GD.Randf() * 2 - 1, GD.Randf() * 2 - 1);
            if(vec.LengthSquared() > 1) continue;
            return vec.Normalized();
        }
    } 
}