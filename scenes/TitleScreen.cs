using Godot;
using System;

public partial class TitleScreen : Control
{
	private string state = "title";

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
		const float duration = 0.8f;
		if (@event is InputEventKey eventKey)
		{
			if (eventKey.Pressed && eventKey.Keycode == Key.Space)
			{
				if (state == "title")
				{
					state = "title_to_instructions";
					Tween tween = GetTree().CreateTween();
					tween.TweenProperty(GetNode("Panel"), "modulate:a", 1.0f, duration);
					tween.TweenCallback(Callable.From(this.ShowInstructions));
					tween.TweenProperty(GetNode("Panel"), "modulate:a", 0.0f, duration);
					tween.TweenCallback(Callable.From(this.GotoInstructions));
				}
				else if (state == "instructions")
				{
					state = "instructions_to_game";
					Tween tween = GetTree().CreateTween();
					tween.TweenProperty(GetNode("Panel"), "modulate:a", 1.0f, duration);
					tween.TweenCallback(Callable.From(this.GoToGameScene));
				}
			}
		}
	}

	public void ShowInstructions()
	{
		(GetNode("Image") as TextureRect).Texture = GD.Load("res://assets/ui/instructions.png") as Texture2D;
	}

	public void GotoInstructions()
	{
		this.state = "instructions";
	}

	public void GoToGameScene()
	{
		GetTree().ChangeSceneToFile("res://uhhh.tscn");
	}
}
