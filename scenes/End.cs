using Godot;
using System;

public partial class End : Control
{


	bool Released = false;
	[Export] Sprite2D cop;


	public override void _Ready(){

		PatronType copType = (PatronType)0;

		foreach(var p in Spawners.patrons){
			if(p.details.isTheCop){
				copType = p.details.patronType;
				break;
			}
		}
		
		cop.Texture = GD.Load<Texture2D>("res://assets/characters/" + copType.ToString().ToLower() + ".png");
	}

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
