using Godot;
using System;

public partial class End : Control
{

	bool Released = false;

	public override void _Process(double delta)
	{
		if (!Input.IsKeyPressed(Key.Space))
		{
			Released = true;
		}
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (!Released)
		{
			return;
		}
		if (@event is InputEventKey eventKey)
		{
			if (eventKey.Pressed && eventKey.Keycode == Key.Space)
			{
				GetTree().ChangeSceneToFile("res://scenes/Credits.tscn");
			}
		}
	}
}
