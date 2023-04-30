using Godot;
using System;
using System.Linq;

public partial class Spawners : Node
{
	
	[Export]int patronsToSpawn = 6;

	public Node2D[] spawnPoints;
	

	public static void Shuffle<T>(T[] list)  
	{  	
		uint n = (uint)list.Length;  
		while (n > 1) {  
			n--;  
			uint k = GD.Randi() % (n+1);
			T value = list[k];  
			list[k] = list[n];  
			list[n] = value;  
		}  
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		spawnPoints = GetChildren().Select(node => node as Node2D).ToArray();
		Shuffle(spawnPoints);

		var patronPrefab = GD.Load<PackedScene>("res://Patron.tscn");
		var world = GetParent();

		for(int i = 0; i < patronsToSpawn; i++){
			var point = spawnPoints[i];
			var patron = patronPrefab.Instantiate() as Patron;
			world.CallDeferred("add_child", patron); 
			patron.Position = point.Position;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
