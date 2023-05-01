using Godot;
using System;

public partial class Waiter : CharacterBody2D
{
	[Export]
	public float Speed = 300.0f;
	[Export]
	public float animationCutoffSpeed = 0.01f;

	[Export]
	public Vector2 firstItemPosition;
	[Export]
	public Vector2 secondItemPosition;

	[Export] public AudioStream pickUpSound, deliverSound, trashSound;
	[Export] public AudioStreamPlayer audioPlayer;
	[Export] public AudioStreamPlayer stepPlayer;
	[Export]public double stepRate, stepRateVariation;



	private bool facingDown = true;
	private bool facingRight = false;

  	private AnimatedSprite2D sprite;

	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		nextStepTime = stepRate + GD.RandRange(-stepRateVariation, stepRateVariation);
	}


	double stepTimer = 0;
	double nextStepTime;

	public override void _PhysicsProcess(double delta)
	{

		if(ActManager.showingActTransition){
			return;
		}



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
			
			if(stepTimer > nextStepTime){
				stepPlayer.PitchScale = (float)GD.RandRange(0.8, 1.2);
				stepPlayer.Play();
				stepTimer = 0;
				nextStepTime = stepRate + GD.RandRange(-stepRateVariation, stepRateVariation);
			}
			stepTimer += delta;

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
	
	
	
	public Item firstItem;
	public Item secondItem;

	public void PickUpItem(Node2D item){
		
		if(firstItem == null){
			audioPlayer.PitchScale = (float)GD.RandRange(.8, 1.2);
			audioPlayer.Stream = pickUpSound;
			audioPlayer.Play();

			firstItem = item as Item;
			AddChild(firstItem);
			firstItem.Position = firstItemPosition;
		} else if(secondItem == null) {

			audioPlayer.PitchScale = (float)GD.RandRange(.8, 1.2);
			audioPlayer.Stream = pickUpSound;
			audioPlayer.Play();

			secondItem = item as Item;
			AddChild(secondItem);
			secondItem.Position = secondItemPosition;
		}
		
	}
	
	
	public void DeliverItem(Patron p){
		if(firstItem?.itemType == p.desiredItem){
			firstItem.QueueFree();
			firstItem = null;

			if(secondItem != null){
				firstItem = secondItem;
				firstItem.Position = firstItemPosition;
				secondItem = null;
			}

			audioPlayer.Stream = deliverSound;
			audioPlayer.PitchScale = (float)GD.RandRange(.8, 1.2);
			audioPlayer.Play();
			p.ResetOrder();
			Suspicion.Reduce(p.deliverySuspicionReduction);
		}

		else if(secondItem?.itemType == p.desiredItem){
			secondItem.QueueFree();
			secondItem = null;

			audioPlayer.PitchScale = (float)GD.RandRange(.8, 1.2);
			audioPlayer.Stream = deliverSound;
			audioPlayer.Play();
			p.ResetOrder();
			Suspicion.Reduce(p.deliverySuspicionReduction);
		}
	}


	public void Trash(){

		if(secondItem != null || firstItem != null){
			audioPlayer.PitchScale = (float)GD.RandRange(.8, 1.2);
			audioPlayer.Stream = trashSound;
			audioPlayer.Play();
		}

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
