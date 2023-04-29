using Godot;
using System;

public partial class Trash : Sprite2D
{
	
	bool overlapped = false;
	Node2D overlapper;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(overlapped && Input.IsActionJustPressed("ui_accept")){
			var waiter = overlapper as Watier;
			waiter.Trash();
		}
	}
	
	
	private void _on_area_2d_body_entered(Node2D body)
	{
		overlapper = body;
		overlapped = true;
	}


	private void _on_area_2d_body_exited(Node2D body)
	{
		overlapped = false;
	}

	
}


