using Godot;

public enum PickupType{
	Boots,
	JetPack,
	Dash,
	Shotgun,
	Zapper,
	Launcher,
	Crossbow,
	Axe,
}

public partial class Pickup : Node3D
{
	[Export] public PickupType Type;
	[Export] public float SpinSpeed = 1;
	AudioStreamPlayer3D pickupSound;
	MeshInstance3D mesh;
	Area3D area;
	bool used = false;
	public override void _Ready()
	{
		base._Ready();
		mesh = GetNode<MeshInstance3D>("Mesh");
		area = GetNode<Area3D>("Area3D");
		area.BodyEntered += body=>{
			if(used) return;
			if(body is Player p){
				used = true;
				Visible = false;
				p.AddPickup(Type);
				pickupSound.Play();
			}
		};
		pickupSound = GetNode<AudioStreamPlayer3D>("PickupSound");
		pickupSound.Finished += QueueFree;
	}
	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		mesh.Rotate(Vector3.Up, (float)delta * SpinSpeed);
	}
}
