using Godot;

public enum PickupType{
	Boots,
}

public partial class Pickup : Node3D
{
	[Export] public PickupType Type;
	[Export] public float SpinSpeed = 1;
	MeshInstance3D mesh;
	Area3D area;
	public override void _Ready()
	{
		base._Ready();
		mesh = GetNode<MeshInstance3D>("Mesh");
		area = GetNode<Area3D>("Area3D");
		area.BodyEntered += body=>{
			if(body is Player p){
				p.AddPickup(Type);
				QueueFree();
			}
		};
	}
	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		mesh.Rotate(Vector3.Up, (float)delta * SpinSpeed);
	}
}
