using Godot;
using System;

public enum DialogType
{
	TalkAboutSelf,
	GossipAboutSomeoneElse,
}

public enum DialogContext
{
	FlavorDialog,
	//ItemDialog, // Don't talk about food or drinks, you must observe it.
	PoliticalDialog,
	CriminalDialog,
	RelationshipDialog,
}




public partial class DialogSystem : Node
{


	string[] gossipFlavorDialogs;
	string[] gossipPoliticalDialogs;
	string[] gossipCriminalDialogs;
	string[] bragFlavorDialogs;
	string[] bragPoliticalDialogs;
	string[] bragCriminalDialogs;

	void ParseDialog() {
		var file = FileAccess.Open("res://assets/DialogText.csv", FileAccess.ModeFlags.Read);
		var text = file.GetAsText();
		var lines = text.Split('\n');
		// first three lines are gossips
		// second three lines are brags
		gossipFlavorDialogs = lines[0].Split(',');
		gossipPoliticalDialogs = lines[1].Split(',');
		gossipCriminalDialogs = lines[2].Split(',');
		bragFlavorDialogs = lines[3].Split(',');
		bragPoliticalDialogs = lines[4].Split(',');
		bragCriminalDialogs = lines[5].Split(',');

		file.Close();
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ParseDialog();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void GenerateRadioMessage(DialogContext _dialogContext)
	{
		GD.Print("The radio broadcasts information about " + _dialogContext);
	}
	
	public void GeneratePatronDialog(PatronType _patron1, PatronType _patron2, PatronType _patronSubject, DialogType _dialogType, DialogContext _dialogContext, uint _clueID)
	{
		string debugPrefixText = "";
		switch(_dialogType)
		{
			case DialogType.TalkAboutSelf:
			{					
				debugPrefixText = _patron1 + " talks abouts themselves to " + _patron2 + " about";
				break;
			}
			
			case DialogType.GossipAboutSomeoneElse:
			{
				debugPrefixText = _patron1 + " talks to " + _patron2 + " about " + _patronSubject;
				break;
			}
		}
		
		string debugSuffixText = "";
		switch(_dialogContext)
		{
			case DialogContext.FlavorDialog:
			{					
				debugSuffixText = " a random tidbit!";
				break;
			}
			
			case DialogContext.CriminalDialog:
			{
				CriminalBackground criminalID = (CriminalBackground)_clueID;
				debugSuffixText = " " + criminalID;
				break;
			}
			
			case DialogContext.PoliticalDialog:
			{
				PolitcalAffiliation politcalID = (PolitcalAffiliation)_clueID;
				debugSuffixText = " " + politcalID;
				break;
			}
			
			case DialogContext.RelationshipDialog:
			{
				RelationshipType relationshipType = (RelationshipType)_clueID;
				debugSuffixText = " their " + relationshipType;
				break;
			}
		}

		GD.Print(debugPrefixText + debugSuffixText);
	}
}
