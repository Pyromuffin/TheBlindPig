using Godot;
using System;

public partial class BadEnd : Control
{
	public override void _UnhandledInput(InputEvent @event)
	{
		GetTree().ChangeSceneToFile("res://scenes/Credits.tscn");
	}
}
