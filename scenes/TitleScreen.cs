using Godot;
using System;

public partial class TitleScreen : Control
{
	private string state = "title";
	[Export]
	AnimationPlayer animationPlayer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		(GetNode("Panel") as Panel).Modulate = new Color(0, 0, 0, 0);
		animationPlayer.Play( "Idle" );
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
					state = "instructions_to_characters";
					Tween tween = GetTree().CreateTween();
					tween.TweenProperty(GetNode("Panel"), "modulate:a", 1.0f, duration);
					tween.TweenCallback(Callable.From(this.ShowCharacters));
					tween.TweenProperty(GetNode("Panel"), "modulate:a", 0.0f, duration);
					tween.TweenCallback(Callable.From(this.GotoCharacters));
				}
				else if (state == "characters")
				{
					state = "characters_to_game";
					Tween tween = GetTree().CreateTween();
					tween.TweenProperty(GetNode("Panel"), "modulate:a", 1.0f, duration);
					tween.TweenCallback(Callable.From(this.GoToGameScene));
				}
			}
		}
	}

	public void ShowInstructions()
	{
		(GetNode("Label") as Label).Modulate = new Color(0,0,0,0);
		(GetNode("Image") as TextureRect).Texture = GD.Load("res://assets/ui/instructions.png") as Texture2D;
	}
	
	public void ShowCharacters()
	{
		(GetNode("Label") as Label).Modulate = new Color(0,0,0,0);
		(GetNode("Image") as TextureRect).Texture = GD.Load("res://assets/ui/character_screen.png") as Texture2D;
	}

	public void GotoInstructions()
	{
		this.state = "instructions";
	}
	
	public void GotoCharacters()
	{
		this.state = "characters";
	}

	public void GoToGameScene()
	{
		GetTree().ChangeSceneToFile("res://uhhh.tscn");
	}
}
