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

public enum DietType {
	Carnivore,
	Herbivore,
	Omnivore,
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
	
	public static ItemType[] foods = {
		ItemType.Meat, ItemType.Carrot, ItemType.Cake,
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
		var index = GD.Randi() % 7;
		return (ItemType)index;
	}
	
	public static ItemType GetRandomDrink(){
		var index = GD.Randi() % alcohol.Length;
		return (ItemType)alcohol[index];
	}
	
	public static DietType GetRandomDiet(){
		return (DietType)(GD.Randi() % 3);
	}
	
	public static ItemType GetCarnivoreFood(){
		var index = GD.Randi() % meats.Length;
		return (ItemType)meats[index];
	}
	public static ItemType GetOmnivoreFood(){
		var index = GD.Randi() % foods.Length;
		return (ItemType)foods[index];
	}
	public static ItemType GetHerbivoreFood(){
		var index = GD.Randi() % veggies.Length;
		return (ItemType)veggies[index];
	}
	
	public static ItemType GetRandomFoodIEat(DietType _diet)
	{ 
		switch(_diet)
		{
			case DietType.Carnivore:
			{
				return GetCarnivoreFood();
			}
			
			case DietType.Herbivore:
			{
				return GetHerbivoreFood();
			}
			
			case DietType.Omnivore:
			{
				return GetOmnivoreFood();
			}
		}
		
		return (ItemType)0;
	}
	
	public ItemType itemType;

	public override void _Process(double delta)
	{
		
	}
	

	
}




