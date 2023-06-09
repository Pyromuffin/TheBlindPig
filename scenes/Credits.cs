using Godot;
using System;

public partial class Credits : Control
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
				// GetTree().quit();
				// GetTree().ChangeSceneToFile("res://scenes/TitleScreen.tscn");
			}
		}
	}
}
