using Godot;
using System;

public partial class Item : Sprite2D
{
	
	[Export]
	public Vector2 heldOffset = new Vector2(0,1);
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		world = GetParent();
	}


	private Node world;
	private Node2D holder;
	private bool overlapped;
	private bool held = false;
	
	
	private void _on_area_2d_body_entered(Node2D body)
	{
		GD.Print("overlapped");
		overlapped = true;
		holder = body;
	}
	
	private void _on_area_2d_body_exited(Node2D body)
	{
		overlapped = false;
	}


	public override void _Process(double delta)
	{
		if(overlapped && !held){
			var selected = Input.IsActionJustPressed("ui_accept");
			if(selected){
				held = true;
				var waiter = holder as Watier;
				waiter.PickUpItem(this);
			}
		}
	}
	

	
}




