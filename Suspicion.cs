using Godot;
using System;

public partial class Suspicion : Control
{
	[Export] public float suspicionRate;
	public static float currentSuspicion;

	public TextureRect meat;
	
	[Export] Vector2 emptyPosition;
	[Export] Vector2 fullPosition;

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
		
		currentSuspicion += (float)delta * suspicionRate;
		currentSuspicion = Mathf.Clamp(currentSuspicion, 0, 100);
		float fraction = currentSuspicion / 100.0f;
		meat.Position = emptyPosition.Lerp(fullPosition, fraction);
		//text.Text = "Suspicion: " + ((int)currentSuspicion).ToString();
	}
}
