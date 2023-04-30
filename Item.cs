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



public partial class Item : Sprite2D
{

	/*
	meat, veggie, alcohol
	icons, get random drink, get random food,
	*/

	public static ItemType[] meats = {
		ItemType.Meat
	};

	public static ItemType[] veggies = {
		ItemType.Carrot, ItemType.Cake,
	};

	public static ItemType[] alcohol = {
		ItemType.Absinthe, ItemType.Bourbon, ItemType.Cocktail, ItemType.Wine
	};

	public static Texture2D GetSmallIcon(ItemType item){
		var name = item.ToString().ToLower();
		return GD.Load<Texture2D>("res://assets/ui/icon_" + name + "_16.png");
	}


	public static Texture2D GetLargeIcon(ItemType item){
		var name = item.ToString().ToLower();
		return GD.Load<Texture2D>("res://assets/ui/icon_" + name + "_24.png");
	}

	public static ItemType GetRandomItem(){
		var index = Random.Shared.Next() % 7;
		return (ItemType)index;
	}

	
	public ItemType itemType;

	public override void _Process(double delta)
	{
		
	}
	

	
}




