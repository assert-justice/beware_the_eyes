using Godot;
using System.Collections.Generic;

public partial class MenuSystem : Control
{
	[Export] PackedScene GameScene;
	Stack<string> menuStack = new();
	Node gameHolder;
	public override void _Ready()
	{
		gameHolder = GetNode<Node>("GameHolder");
		Pause(true);
		menuStack.Push("Main");
		SetMenu();
	}
	void HideMenus(){
		foreach (var child in GetChildren())
		{
			if (child is Control c){
				c.Visible = false;
			}
		}
	}
	void SetMenu(){
		HideMenus();
		GetNode<Control>(menuStack.Peek()).Visible = true;
	}
	public void PushMenu(string menuName){
		menuStack.Push(menuName);
		SetMenu();
	}
	public string PopMenu(){
		var name = menuStack.Peek();
		if(menuStack.Count > 1) menuStack.Pop();
		SetMenu();
		return name;
	}
	public void Launch(){
		Pause(false);
		if(gameHolder.GetChildCount() > 0) gameHolder.GetChild(0).QueueFree();
		gameHolder.AddChild(GameScene.Instantiate());
	}
	public bool IsPaused(){
		return GetTree().Paused;
	}
	public void Pause(bool shouldPause){
		if(IsPaused() == shouldPause) return;
		GetTree().Paused = !IsPaused();
		if(IsPaused()){
			PushMenu("Pause");
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
		else{
			HideMenus();
			Input.MouseMode = Input.MouseModeEnum.Captured;
		}
	}
}
