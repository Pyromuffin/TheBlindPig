using Godot;
using System;

public partial class Suspicion : Control
{
	[Export] public float suspicionRate;
	public static float currentSuspicion;

	public TextureRect meat;
	
	[Export] Vector2 emptyPosition;
	[Export] Vector2 fullPosition;
	[Export] float perOrderIncrease;
	


	public static void Reduce(float amount){
		currentSuspicion -= amount;
		currentSuspicion = Mathf.Clamp(currentSuspicion, 0, 100);
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		meat = GetNode<TextureRect>("Frame/Meat Mask/Meat");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(ActManager.showingActTransition){
			return;
		}
		

		float increase = 1.0f;
	
		foreach(var p in Spawners.patrons){
			if(p.currentState == Patron.State.ORDERING){
				increase += perOrderIncrease;
			}
		}

		currentSuspicion += (float)delta * increase;
		currentSuspicion = Mathf.Clamp(currentSuspicion, 0, 100);
		float fraction = currentSuspicion / 100.0f;
		meat.Position = emptyPosition.Lerp(fullPosition, fraction);
		//text.Text = "Suspicion: " + ((int)currentSuspicion).ToString();
	}
}
