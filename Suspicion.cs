using Godot;
using System;

public partial class Suspicion : Control
{
	[Export] public float suspicionRate;
	public static float currentSuspicion;

	Label text;

	public static void Reduce(float amount){
		currentSuspicion -= amount;
		currentSuspicion = Mathf.Clamp(currentSuspicion, 0, 100);
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		text = GetNode<Label>("Label");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		currentSuspicion += (float)delta * suspicionRate;
		text.Text = "Suspicion: " + ((int)currentSuspicion).ToString();
	}
}
