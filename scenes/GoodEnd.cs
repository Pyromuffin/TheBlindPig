using Godot;
using System;

public partial class GoodEnd : Control
{
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
		{
			if (eventKey.Pressed && eventKey.Keycode == Key.Space)
			{
				GetTree().ChangeSceneToFile("res://scenes/Credits.tscn");
			}
		}
	}
}
