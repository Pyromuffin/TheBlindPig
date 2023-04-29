using Godot;
using System;

public partial class Item : Sprite2D
{
	
	[Export]
	public Vector2 heldOffset = new Vector2(0,1);
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}


	private Node2D holder;
	private bool overlapped;
	
	private void _on_area_2d_body_entered(Node2D body)
	{
		overlapped = true;
		holder = body;
		GD.Print("entered");
	}
	
	private void _on_area_2d_body_exited(Node2D body)
	{
		overlapped = false;
		GD.Print("exited");
	}


	public override void _Process(double delta)
	{
		
		if(overlapped){
			var selected = Input.IsActionJustPressed("ui_accept");
			if(selected){
				GD.Print("item taken");
				GetParent().RemoveChild(this);
				holder.AddChild(this);
				Position = heldOffset;
			}
		}
		 
	}
	

	
}




