using Godot;
using System;

public partial class Watier : CharacterBody2D
{
	[Export]
	public float Speed = 300.0f;
	[Export]
	public float animationCutoffSpeed = 0.01f;

  	private AnimatedSprite2D sprite;

	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}


	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;


		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Y = direction.Y * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);
		}
		
		GD.Print(velocity);
		
		
		if(velocity.Y > animationCutoffSpeed){
			sprite.Play("down");
		}
		else if(velocity.Y < -animationCutoffSpeed){
			sprite.Play("up");
		}
		else if(velocity.X > animationCutoffSpeed){
			sprite.Play("right");
		}
		else if(velocity.X < -animationCutoffSpeed){
			sprite.Play("left");
		}
		
		if(Mathf.Abs(velocity.X) <= animationCutoffSpeed && Mathf.Abs(velocity.Y) <= animationCutoffSpeed){
			sprite.Stop();
		}
		

		Velocity = velocity;
		MoveAndSlide();
	}
}
