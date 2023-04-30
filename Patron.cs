using Godot;
using System;

public partial class Patron : Sprite2D
{
	[Export]
	public Vector2 minimumDialogBoxSize;
	[Export]
	public Vector2 targetDialogBoxSize;
	
	[Export]
	public float beginDialogGrowDistance;
	[Export]
	public float endDialogGrowDistance;
	
	
	public NinePatchRect dialogBubble;
	public Node2D waiter;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		dialogBubble = GetNode<NinePatchRect>("Request text");
		waiter = GetParent().GetNode<Node2D>("Waiter");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		float distanceToWaiter =  (waiter.Position - Position).Length();
		float totalDistance = beginDialogGrowDistance - endDialogGrowDistance;
		float fraction = (distanceToWaiter - beginDialogGrowDistance) / totalDistance;
		fraction = Mathf.Clamp(fraction, 0, 1);
		var size = minimumDialogBoxSize.Lerp(targetDialogBoxSize, 1 - fraction);
		dialogBubble.Size = size;
		GD.Print(fraction);
		GD.Print(distanceToWaiter);
	}
}
