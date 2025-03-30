using Godot;

public partial class Respawn : Area3D
{
	public override void _Ready()
	{
		base._Ready();
		BodyEntered += b => {
			if(b is Player p){
				p.SaveState(Position);
			}
		};
	}
}
