using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public static class EnumExtensions
{
	public static Enum GetRandomEnumValue(this Type t)
	{
		return Enum.GetValues(t)          // get values from Type provided
			.OfType<Enum>()               // casts to Enum
			.OrderBy(e => Guid.NewGuid()) // mess with order of results
			.FirstOrDefault();            // take first item in result
	}
}

public enum ClueType
{
	AlcoholClueType,
	DietClueType,
	PoliticalClueType,
	CriminalClueType,
	RelationshipClueType,
}

public abstract class Clue 
{
	public uint clueID;
	public ClueType clueType;
	public DialogContext dialogContext;
	
	public void SetClueID(uint _clueID)
	{
		clueID = _clueID;
	}
	
	public abstract uint GetClueValue();
	public abstract string GetClueText();
}

public class AlcoholClue : Clue
{
	public AlcoholClue()
	{
		//dialogContext = DialogContext.ItemDialog;
		clueType = ClueType.AlcoholClueType;
	}
	
	public override uint GetClueValue() 
	{
		return (uint)clueID;
	}
	
	public override string GetClueText()
	{
		return " will never order " + (ItemType)clueID;
	}
}

public class DietClue : Clue
{
	public DietClue()
	{
		//dialogContext = DialogContext.ItemDialog;
		clueType = ClueType.DietClueType;
	}
	public override uint GetClueValue() 
	{
		return (uint)clueID;
	}
	
	public override string GetClueText()
	{
		return " is a " + (DietType)clueID;
	}
}

public class PoliticalClue : Clue
{
	public PoliticalClue()
	{
		dialogContext = DialogContext.PoliticalDialog;
		clueType = ClueType.PoliticalClueType;
	}
	public override uint GetClueValue() 
	{
		return (uint)clueID;
	}
	
	public override string GetClueText()
	{
		return " is a member of the " + (PolitcalAffiliation)clueID;
	}
}

public class CriminalClue : Clue
{
	public CriminalClue()
	{
		dialogContext = DialogContext.CriminalDialog;
		clueType = ClueType.CriminalClueType;
	}
	public override uint GetClueValue() 
	{
		return (uint)clueID;
	}
	
	public override string GetClueText()
	{
		return "'s cover story is that they are a " + (CriminalBackground)clueID;
	}
}

public class RelationshipClue : Clue
{
	public RelationshipClue()
	{
		dialogContext = DialogContext.RelationshipDialog;
		clueType = ClueType.RelationshipClueType;
	}
	
	public override uint GetClueValue() 
	{
		return (uint)clueID;
	}
	
	public override string GetClueText()
	{
		return " has a " + (RelationshipType)clueID;
	}
}

static class ExtensionsClass
{
	public static void Shuffle<T>(this IList<T> list)
	{
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = (int)(GD.Randi() % (n + 1));
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}
}
	
public class Act 
{
	public Clue clue;
	
	public PatronType[,] _couples;
	
	public Act(Clue _clue)
	{
		clue = _clue;
		
		CreateCouples();
	}
	
	public bool IsActOver()
	{
		// TODO: REMOVE?
		return true;
	}
		
	public void CreateCouples()
	{
		// We do this list thing to be able to prune them out after each selection!
		// Hacky but works lol
		List<PatronType> patronList = new List<PatronType>();
		for(uint i = 0; i < 6; ++i)
		{
			patronList.Add((PatronType)i);
		}
		patronList.Shuffle();
		
		_couples = new PatronType[,] { { patronList[0], patronList[1] }, { patronList[2], patronList[3] }, { patronList[4], patronList[5] } };
	}
}

public partial class ClueDirector : Node2D
{

	[Export] float minimumOrderTime = 8;
	[Export] float maximumOrderTime = 20;

	[Signal]
	public delegate void SendDialogEventHandler();
	
	const bool DEBUG_SYSTEM = false;
	
	const uint ACT_COUNT = 3;
	const uint FLAVOR_COUNT = 6;
	
	
	
	[Export] Node2D waiterStartPos;
	[Export] Node2D waiter;

	
	const int PATRON_COUNT = 6;
	
	public uint currentAct = 0;
	uint copIndex = 0;
	int currentDialog = 0;
	
	public PatronDetails[] patrons = new PatronDetails[PATRON_COUNT];
	
	DialogSystem dialogSystem = new DialogSystem();
	List<DialogData> diaglogData = new List<DialogData>();
	
	public Act[] acts = new Act[ACT_COUNT];
	
	public void StartCurrentAct()
	{
		if(DEBUG_SYSTEM)
		{
			GD.Print("================");
			GD.Print("Start of Act " + (currentAct + 1));
			GD.Print("-------------");
		}

		var act = acts[currentAct];
		var randomPairs = Spawners.GetRandomSpawnPairs();

		for(int i = 0; i < 3; i++){
			var first = act._couples[i,0];
			var second = act._couples[i,1];

			Spawners.patrons[(int)first].Position = randomPairs[i].first.Position;
			Spawners.patrons[(int)second].Position = randomPairs[i].second.Position;
		}

		Suspicion.currentSuspicion = 0;
		foreach(var p in Spawners.patrons){
			p.ResetOrder();
		}

		waiter.Position = waiterStartPos.Position;
	}
	
	
	public void CreatePatronRelationships()
	{
		// Now make a simple list of patrons..
		List<PatronType> patronList = new List<PatronType>();
		for(uint i = 0; i < (uint)PATRON_COUNT; ++i)
		{
			patronList.Add((PatronType)i);
		}
		// Scramble it..
		patronList.Shuffle();
		
		List<RelationshipType> typeList = new List<RelationshipType>();
		typeList.Add(RelationshipType.Rival);
		typeList.Add(RelationshipType.Lover);
		typeList.Add(RelationshipType.Ex);
		//typeList.Add(RelationshipType.None); // We don't do this one because we want everyone to have a relationship right now
		typeList.Shuffle();
	
		// And use it to create the relationships
		for(int i = 0; i < (int)PATRON_COUNT; i+=2)
		{
			RelationshipType relationshipType = typeList[i / 2];
			if(relationshipType == RelationshipType.None)
			{
				continue;
			}
			
			int j = i + 1;
			
			PatronType patron1 = patronList[i];
			PatronType patron2 = patronList[j];
			
			patrons[(int)patron1].relationPatron = patron2;
			patrons[(int)patron2].relationPatron = patron1;
			
			patrons[(int)patron1].relationshipType = relationshipType;
			patrons[(int)patron2].relationshipType = relationshipType;
		}
	}
	
	public void CreateClues()
	{
		// We do this list thing to be able to prune them out after each selection!
		// Hacky but works lol
		List<int> numberList = new List<int>();
		numberList.Add(0);
		numberList.Add(1);
		//numberList.Add(2); // 2 = relationship clues, which I'm disabling for now
		
		// Every game either has a drink or food clue!
		// Decide which act it is here
		long actWithItemClue = GD.Randi() % ACT_COUNT;
		
		for (uint i = 0; i < ACT_COUNT; ++i)
		{
			// Check if this is the act with the Item clue
			if(i == actWithItemClue)
			{
				// 50/50 if its hated drink or type of diet
				bool coinFlip = (GD.Randi() % 2) == 0;
				if(coinFlip)
				{
					DietClue clue = new DietClue();
					clue.SetClueID((uint)patrons[copIndex].dietType);
					acts[i] = new Act(clue);
				}
				else
				{
					AlcoholClue clue = new AlcoholClue();
					clue.SetClueID((uint)patrons[copIndex].hatedDrink);
					acts[i] = new Act(clue);
				}
			}
			else
			{
				long clueIndex = GD.Randi() % (int)numberList.Count;
				int clueType = numberList[ (int)clueIndex ];
			
				if(clueType == 0)
				{
					PoliticalClue clue = new PoliticalClue();
					clue.SetClueID((uint)patrons[copIndex].politcalAffiliation);
				
					acts[i] = new Act(clue);
				}
				else if(clueType == 1)
				{
					CriminalClue clue = new CriminalClue();
					clue.SetClueID((uint)patrons[copIndex].criminalBackground);
				
					acts[i] = new Act(clue);
				}
				else if(clueType == 2)
				{
					RelationshipClue clue = new RelationshipClue();
					clue.SetClueID((uint)patrons[copIndex].relationshipType);
				
					acts[i] = new Act(clue);
				}
				
				numberList.RemoveAt((int)clueIndex);
			}
		}
	}
	
	public void CreateMystery()
	{
		// Which character is the undercover pig this time?
		copIndex = GD.Randi() % (uint)PATRON_COUNT;
		
		// Pick the cop's attributes randomly
		DietType copDiet = (DietType)(typeof(DietType).GetRandomEnumValue());
		ItemType copDrink = Item.alcohol[ GD.Randi() % (uint)Item.alcohol.Length ];
		PolitcalAffiliation copPolAff = (PolitcalAffiliation)(typeof(PolitcalAffiliation).GetRandomEnumValue());
		CriminalBackground copCrimBack = (CriminalBackground)(typeof(CriminalBackground).GetRandomEnumValue());
		
		// Spawn the cop!
		patrons[copIndex] = new PatronDetails(copIndex, true, copDrink, copDiet, copPolAff, copCrimBack);
		
		DialogData diaglogClueCopPolAff = new DialogData((PatronType)copIndex, DialogContext.PoliticalDialog, (uint)copPolAff);
		diaglogData.Add(diaglogClueCopPolAff);
		
		DialogData diaglogClueCrimBack = new DialogData((PatronType)copIndex, DialogContext.CriminalDialog, (uint)copCrimBack);
		diaglogData.Add(diaglogClueCrimBack);
		
		// Create the clues because
		CreateClues();
		
		List<int> uniqueFromoCopAct = new List<int>();
		uniqueFromoCopAct.Add(0);
		uniqueFromoCopAct.Add(0);
		uniqueFromoCopAct.Add(1);
		uniqueFromoCopAct.Add(1);
		uniqueFromoCopAct.Add(2);
		uniqueFromoCopAct.Add(2);
		uniqueFromoCopAct.Shuffle();
		
		for (uint i = 0; i < (uint)PATRON_COUNT; ++i)
		{
			if(i != copIndex)
			{
				// Create our desired properties
				
				DietType patronDiet = (DietType)(typeof(DietType).GetRandomEnumValue());
				while(patronDiet == copDiet)
				{
					patronDiet = (DietType)(typeof(DietType).GetRandomEnumValue());
					//GD.Print(copDiet + " - " + patronDiet);
				}
				
				ItemType patronDrink = Item.alcohol[ GD.Randi() % (uint)Item.alcohol.Length ];
				while(patronDrink == copDrink)
				{
					patronDrink = Item.alcohol[ GD.Randi() % (uint)Item.alcohol.Length ];
					//GD.Print(copDrink + " " + patronDrink);
				}
				
				PolitcalAffiliation patronPolAff = (PolitcalAffiliation)(typeof(PolitcalAffiliation).GetRandomEnumValue());
				while(patronPolAff == copPolAff)
				{
					patronPolAff = (PolitcalAffiliation)(typeof(PolitcalAffiliation).GetRandomEnumValue());
					//GD.Print(copPolAff + " " + patronPolAff);
				}
				
				CriminalBackground patronCrimBack = (CriminalBackground)(typeof(CriminalBackground).GetRandomEnumValue());
				while(patronCrimBack == copCrimBack)
				{
					patronCrimBack = (CriminalBackground)(typeof(CriminalBackground).GetRandomEnumValue());
					//GD.Print(copCrimBack + " " + patronCrimBack);
				}
				
				// Then 
				for (int j = 0; j < ACT_COUNT; ++j)
				{
					if(j == uniqueFromoCopAct[(int)i])
						continue;
					
					ClueType clueType = acts[j].clue.clueType;
					switch(clueType)
					{
						case ClueType.AlcoholClueType:
						{
							patronDrink = copDrink;
							break;
						}
						
						case ClueType.DietClueType:
						{
							patronDiet = copDiet;
							break;
						}
						
						case ClueType.PoliticalClueType:
						{
							patronPolAff = copPolAff;
							
							DialogData diaglogClue = new DialogData((PatronType)i, DialogContext.PoliticalDialog, (uint)patronPolAff);
							diaglogData.Add(diaglogClue);
							
							break;
						}
						
						case ClueType.CriminalClueType:
						{
							patronCrimBack = copCrimBack;
							 
							DialogData diaglogClue = new DialogData((PatronType)i, DialogContext.CriminalDialog, (uint)patronCrimBack);
							diaglogData.Add(diaglogClue);
							
							break;
						}
						
						case ClueType.RelationshipClueType:
						{
							// TODO!!
							
							//DialogData diaglogClue = new DialogData((PatronType)i, DialogContext.RelationshipDialog, ;
							//diaglogData.Add(diaglogClue);
							
							break;
						}
					}
				}
				
				patrons[i] = new PatronDetails(i, false, (ItemType)patronDrink, patronDiet, patronPolAff, patronCrimBack );
			}
		}

		// Make this work with the 4 on 2 off rules and move up to above the clues...
		CreatePatronRelationships();
		
		for(int i = 0; i < FLAVOR_COUNT; ++i)
		{
			PatronType patronType = (PatronType)(typeof(PatronType ).GetRandomEnumValue());
			DialogData diaglogClue = new DialogData(patronType, DialogContext.FlavorDialog, 0);
			diaglogData.Add(diaglogClue);
		}
		
		// Clue dialogs happen in a random order
		diaglogData.Shuffle();
		
		if(DEBUG_SYSTEM)
		{
			for (uint i = 0; i < ACT_COUNT; ++i)
			{
				GD.Print("The undercover cop" + acts[i].clue.GetClueText());
			}
		}

		for (uint i = 0; i < (uint)PATRON_COUNT; ++i)
		{
			patrons[i].DebugPrintDetails();
		}
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		randomOrderTime = GD.RandRange(minimumOrderTime, maximumOrderTime);
		randomDialogTime = GD.RandRange(minimumOrderTime, maximumOrderTime);
		dialogSystem.ParseDialog();
		
		if(DEBUG_SYSTEM)
		{
			for (int i = 0; i < diaglogData.Count; ++i)
			{
				PatronType patronType = (PatronType)(typeof(PatronType).GetRandomEnumValue());
				GD.Print(GeneratePatronDialog(patronType, i));
			}
		}
	}
	
	public void GenerateRadioMessage(bool _isAClue)
	{
		uint actToAbout = GD.Randi() % 3;
		
		DialogContext context;
		if(_isAClue)
		{
			context = acts[actToAbout].clue.dialogContext;
		}
		else
		{
			context = (DialogContext)(typeof(DialogContext).GetRandomEnumValue());
		}
		
		dialogSystem.GenerateRadioMessage(context);
	}
	
	public bool StillHasDialogs()
	{
		return currentDialog < diaglogData.Count;
	}
	
	public string GetContextString(DialogContext _context, uint _ID)
	{
		switch(_context)
		{
			case DialogContext.PoliticalDialog:
			{
				PolitcalAffiliation polAff = (PolitcalAffiliation)_ID;
				switch(polAff)
				{
					case PolitcalAffiliation.LeopardParty:
					{
						return "Leopard Party ";
					}
					case PolitcalAffiliation.RabbitParty:
					{
						return "Rabbit Party";
					}
					case PolitcalAffiliation.BearParty:
					{
						return "Bear Party";
					}
					default:
						return "UNKNOWN";
				}
			}
			case DialogContext.CriminalDialog:
			{
				CriminalBackground crimBackground = (CriminalBackground)_ID;
				switch(crimBackground)
				{
					case CriminalBackground.Bootlegger:
					{
						return "Bootlegger";
					}
					case CriminalBackground.RumRunner:
					{
						return "Rum Runner";
					}
					case CriminalBackground.Moonshiner:
					{
						return "Moonshiner";
					}
					case CriminalBackground.Bribery:
					{
						return "Money Launderer";
					}
					case CriminalBackground.Smuggling:
					{
						return "Smuggler";
					}
					default:
						return "UNKNOWN";
				}
			}
			case DialogContext.RelationshipDialog:
			{
				return "";
			}
			
			default:
				return "";
		}
	}
	
	public string GeneratePatronDialog(PatronType _talker, int _dialogIndex)
	{	
		PatronType listener = PatronType.EscapeArtist;
		for(int i = 0; i < 3; i++)
		{
			var first = acts[currentAct]._couples[i,0];
			var second = acts[currentAct]._couples[i,1];
		
			if( first == _talker )
			{
				listener = second;
			}
			else if( second == _talker )
			{
				listener = first;
			}
		}
		
		DialogData thisDialog =  diaglogData[_dialogIndex];
		
		PatronType  subject = thisDialog.subject;
		DialogContext context = thisDialog.dialogContext;
		
		DialogType dialogType = _talker == subject ? DialogType.TalkAboutSelf : DialogType.GossipAboutSomeoneElse;
		
		//HOOK UP CSV INTERACTION HERE
		// _talker is the person talking
		// subject is the $subject
		// dialogType is the table (brag vs gossip)
		// context is the row
		// dialogData.clueID is $object
		
		string[] dialogFormatStrings = null;

		if(dialogType == DialogType.TalkAboutSelf){
			if(context == DialogContext.CriminalDialog){
				dialogFormatStrings = dialogSystem.bragCriminalDialogs;
			}
			if(context == DialogContext.FlavorDialog){
				dialogFormatStrings = dialogSystem.bragFlavorDialogs;
			}
			if(context == DialogContext.PoliticalDialog){
				dialogFormatStrings = dialogSystem.bragPoliticalDialogs;
			}
		}
		else {
			
			if(context == DialogContext.CriminalDialog){
				dialogFormatStrings = dialogSystem.gossipCriminalDialogs;
			}
			if(context == DialogContext.FlavorDialog){
				dialogFormatStrings = dialogSystem.gossipFlavorDialogs;
			}
			if(context == DialogContext.PoliticalDialog){
				dialogFormatStrings = dialogSystem.gossipPoliticalDialogs;
			}
		}
		
		string subjectName = patrons[(int)subject].patronName;
		string objectName = GetContextString(context, thisDialog.clueID);
		
		if(DEBUG_SYSTEM)
		{
			GD.Print("Talker: " + subjectName + "(" + _talker + "); Subject: " + subject + "; Type: " + dialogType + "; Context" + context + "; Object: " + objectName + "(" + thisDialog.clueID + ")");
		}

		var formatString = dialogFormatStrings[(int)_talker];
		var s = formatString.Replace("$subject", subjectName);
		s = s.Replace("$object", objectName);

		
		return s;
	} 


	double orderTimer = 0;
	double randomOrderTime;

	double dialogTimer = 0;
	double randomDialogTime;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(ActManager.showingActTransition){
			return;
		}

		if(orderTimer > randomOrderTime){
			orderTimer = 0;
			var shuffled = Spawners.patrons.Clone() as Patron[];
			shuffled.Shuffle();
			
			for(int i = 0; i < 6; i++){
				var p = shuffled[i];
				if(p.currentState == Patron.State.IDLE){
					var item = p.GetRandomOrderableItem();
					p.CreateOrder(item);
					randomOrderTime = GD.RandRange(minimumOrderTime, maximumOrderTime);
					break;
				}
			}

		}

		if(StillHasDialogs())
		{
			if(dialogTimer > randomDialogTime){
				dialogTimer = 0;
				var shuffled = Spawners.patrons.Clone() as Patron[];
				shuffled.Shuffle();
				
				for(int i = 0; i < 6; i++){
					var p = shuffled[i];
					if(p.currentState == Patron.State.IDLE){
						var s = GeneratePatronDialog(p.details.patronType, currentDialog);
						p.CreateDialog(s);
						randomDialogTime = GD.RandRange(minimumOrderTime, maximumOrderTime);
						currentDialog++;
						break;
					}
				}
			}
		}


		orderTimer += delta;
		dialogTimer += delta;
	}
	
}
