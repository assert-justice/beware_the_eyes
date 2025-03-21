using Godot;

public partial class Shotgun : Weapon
{
	[Export] float FireTime = 0.5f;
	[Export] PackedScene SparkleScene;
	Clock fireClock;
	EntPool sparklePool;
	public override void _Ready()
	{
		base._Ready();
		var parent = GetTree().GetNodesInGroup("Game")[0];
		fireClock = AddClock(FireTime, 0);
		sparklePool = AddPool(parent, ()=>SparkleScene.Instantiate<Entity>());
	}
	override public bool CanFire(FireCommand command){
		if(fireClock.IsRunning()) return false;
		return command.JustPressed;
	}
	override protected void Fire(FireCommand command){
		GD.Print("fire!");
		fireClock.Reset();
		if(command.Ray.IsColliding()){
			var pos = command.Ray.GetCollisionPoint();
			var s = sparklePool.GetPool().GetNew();
			s.Position = pos;
		}
	}
}
