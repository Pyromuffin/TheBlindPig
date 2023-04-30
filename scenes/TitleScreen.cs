using Godot;
using System;

public partial class TitleScreen : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		(GetNode("Panel") as Panel).Modulate = new Color(0, 0, 0, 0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
		{
			if (eventKey.Pressed && eventKey.Keycode == Key.Space)
			{
				const float alpha = 1.0f;
				const float duration = 0.8f;
				Tween tween = GetTree().CreateTween();
				tween.TweenProperty(GetNode("Panel"), "modulate:a", alpha, duration);
				tween.TweenCallback(Callable.From(this.GoToGameScene));
			}
		}
	}

	public void GoToGameScene()
	{
		GetTree().ChangeSceneToFile("res://uhhh.tscn");
	}
}
