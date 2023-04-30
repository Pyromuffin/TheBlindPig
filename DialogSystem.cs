using Godot;
using System;

public enum DialogType
{
	FlavorDialog,
	ItemDialog,
	PoliticalDialog,
	CriminalDialog,
}

public partial class DialogSystem : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	
	public void GenerateDialog(PatronType _patron1, PatronType _patron2, DialogType _dialogType)
	{
		
		
		GD.Print("DIALOG: "+ _dialogType);
	}
}
