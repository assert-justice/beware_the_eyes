using Godot;

public partial class Shotgun : Weapon
{
	override public bool CanFire(FireCommand command){
		return true;
	}
	override protected void Fire(FireCommand command){
		GD.Print("fire!");
	}
}
