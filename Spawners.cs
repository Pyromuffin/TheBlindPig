using Godot;
using System;
using System.Linq;


public struct SpawnPair{
	public Node2D first, second;	
}

public partial class Spawners : Node
{
	
	[Export]int patronsToSpawn = 6;

	static Node2D[] spawnPoints;
	public static Patron[] patrons = new Patron[6];
	public static SpawnPair[] GetRandomSpawnPairs() {
		var indices = new int[] {0,1,2,3,4,5,6,7};
		indices.Shuffle();
		var pairs = new SpawnPair[3];
		pairs[0].first = spawnPoints[indices[0] * 2];
		pairs[0].second = spawnPoints[indices[0] * 2 + 1];
		pairs[1].first = spawnPoints[indices[1] * 2];
		pairs[1].second = spawnPoints[indices[1] * 2 + 1];
		pairs[2].first = spawnPoints[indices[2] * 2];
		pairs[2].second = spawnPoints[indices[2] * 2 + 1];

		return pairs;
	}	

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
		var patronPrefab = GD.Load<PackedScene>("res://Patron.tscn");
		var world = GetParent();

		for(int i = 0; i < patronsToSpawn; i++){
			var point = spawnPoints[i];
			var patron = patronPrefab.Instantiate() as Patron;
			world.CallDeferred("add_child", patron); 
			patron.Position = point.Position;
			patrons[i] = patron;
		}

		var director = GetParent().GetNode<ClueDirector>("director");
		director.CreateMystery();
		for(int i = 0; i < 6; i ++){
			patrons[i].Init(director.patrons[i]);
		}
		director.StartCurrentAct();

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
