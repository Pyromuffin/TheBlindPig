using Godot;
using System;

public partial class Watier : CharacterBody2D
{
	[Export]
	public float Speed = 300.0f;
	[Export]
	public float animationCutoffSpeed = 0.01f;

	[Export]
	public Vector2 firstItemPosition;
	[Export]
	public Vector2 secondItemPosition;

	private bool facingDown = true;
	private bool facingRight = false;

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
		
		
		
		if(velocity.Length() > animationCutoffSpeed){
			
			if( Mathf.Abs(velocity.Y) > 0 ){
				facingDown = velocity.Y > 0;
			}
			
			if( Mathf.Abs(velocity.X) > 0 ){
				facingRight = velocity.X > 0;
			}
			
			sprite.FlipH = facingRight ^ !facingDown;
			
			
			if(facingDown){
				sprite.Play("forward_walk");
			} else {
				sprite.Play("backward_walk");
			}
		}
		else{		
			if(facingDown){
				sprite.Play("forward_idle");
			} else {
				sprite.Play("back_idle");
			}
		}
		

		Velocity = velocity;
		MoveAndSlide();
	}
	
	
	
	public Node2D firstItem;
	public Node2D secondItem;

	public void PickUpItem(Node2D item){
		
		GD.Print("Picked up");
		
		if(firstItem == null){
			firstItem = item;
			AddChild(firstItem);
			firstItem.Position = firstItemPosition;
		} else if(secondItem == null) {
			secondItem = item;
			AddChild(secondItem);
			secondItem.Position = secondItemPosition;
		}
		
	}
	
	
	public void Trash(){
		
		if(secondItem != null){
			secondItem.QueueFree();
			secondItem = null;
		}
		
		if(firstItem != null){
			firstItem.QueueFree();
			firstItem = null;
		}
	}
	
	
}
