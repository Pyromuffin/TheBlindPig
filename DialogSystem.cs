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
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
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
		}

		GD.Print(debugPrefixText + debugSuffixText);
	}
}
