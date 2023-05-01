using Godot;
using System;





public partial class ItemSource : Sprite2D
{
	
	
	[Export]
	public ItemType itemType;

	
	PackedScene itemScene = GD.Load<PackedScene>("res://Item.tscn");
	
	Node2D overlapper;
	[Export]
	AnimationPlayer animationPlayer;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.Texture = Item.GetLargeIcon(itemType);
		animationPlayer.Play("Idle");
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(ActManager.showingActTransition){
			return;
		}

		if(overlapper != null && Input.IsActionJustPressed("ui_accept")){
			var waiter = overlapper as Waiter;
			var item = itemScene.Instantiate() as Item;
			item.Texture = Item.GetSmallIcon(itemType);
			item.itemType = itemType;
			waiter.PickUpItem(item);
		}
	}
	
	private void _on_area_2d_body_entered(Node2D body)
	{
		if(body is Waiter){
			overlapper = body;
		}
	}


	private void _on_area_2d_body_exited(Node2D body)
	{
		overlapper = null;
	}

		
}


