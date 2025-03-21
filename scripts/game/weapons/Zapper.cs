using Godot;

public partial class Zapper : Weapon
{
	override public bool CanFire(FireCommand command){
		return true;
	}
	override protected void Fire(FireCommand command){}
}
