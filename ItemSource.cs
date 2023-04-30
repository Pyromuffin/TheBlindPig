using Godot;
using System;

public enum ItemType {
	Absinthe,
	Bourbon,
	Cake,
	Carrot,
	Cocktail,
	Meat,
	Wine,
}





public partial class ItemSource : Sprite2D
{
	
	/*
	public static ItemType[] foods = {
		ItemType.Cake, ItemType.Carrot, ItemType.Meat
	}
	
	public static ItemType[] drinks = {
		ItemType.Absinthe, ItemType.Bourbon, ItemType.Cocktail, ItemType.Wine
	}
	*/
	
	
	[Export]
	public ItemType itemType;
	
	[Export]
	public Resource[] icons;
	
	[Export]
	public Resource[] smallIcons;
	
	
	PackedScene itemScene = GD.Load<PackedScene>("res://Item.tscn");
	
	Node2D overlapper;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.Texture = icons[(int)itemType] as Texture2D;
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(overlapper != null && Input.IsActionJustPressed("ui_accept")){
			var waiter = overlapper as Waiter;
			var item = itemScene.Instantiate() as Item;
			item.Texture = smallIcons[(int)itemType] as Texture2D;
			item.itemType = itemType;
			
			waiter.PickUpItem(item);
		}
	}
	
	private void _on_area_2d_body_entered(Node2D body)
	{
		overlapper = body;
	}


	private void _on_area_2d_body_exited(Node2D body)
	{
		overlapper = null;
	}

		
}


