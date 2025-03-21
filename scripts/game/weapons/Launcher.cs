using Godot;

public partial class Launcher : Weapon
{
	override public bool CanFire(FireCommand command){
		return true;
	}
	override protected void Fire(FireCommand command){}
}
