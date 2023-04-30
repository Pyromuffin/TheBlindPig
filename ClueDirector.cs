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

abstract class Clue 
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

class AlcoholClue : Clue
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

class DietClue : Clue
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

class PoliticalClue : Clue
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
		return " is a member of " + (PolitcalAffiliation)clueID;
	}
}

class CriminalClue : Clue
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
		return " pretends to be a " + (CriminalBackground)clueID + " on the side";
	}
}

class RelationshipClue : Clue
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
	
class Act 
{
	public Clue clue;
	public uint clueCount = 5;
	
	public PatronType[,] _couples;
	
	public Act(Clue _clue)
	{
		clue = _clue;
		
		CreateCouples();
	}
	
	public bool IsActOver()
	{
		return clueCount <= 0;
	}
		
	public void CreateCouples()
	{
		// We do this list thing to be able to prune them out after each selection!
		// Hacky but works lol
		List<PatronType> patronList = new List<PatronType>();
		for(uint i = 0; i < (uint)PatronType.PATRON_COUNT; ++i)
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
	
	const uint ACT_COUNT = 3;
	
	uint currentAct = 0;
	uint copIndex = 0;
	
	public PatronDetails[] patrons = new PatronDetails[(int)PatronType.PATRON_COUNT];
	
	DialogSystem dialogSystem = new DialogSystem();
	
	Act[] acts = new Act[ACT_COUNT];
	
	public void StartCurrentAct()
	{
		GD.Print("================");
		GD.Print("Start of Act " + (currentAct + 1));
		GD.Print("-------------");

		var act = acts[currentAct];
		var randomPairs = Spawners.GetRandomSpawnPairs();

		for(int i = 0; i < 3; i++){
			var first = act._couples[i,0];
			var second = act._couples[i,1];

			Spawners.patrons[(int)first].Position = randomPairs[i].first.Position;
			Spawners.patrons[(int)second].Position = randomPairs[i].second.Position;
		}

	}
	
	public void GoToNextAct()
	{
		GD.Print("-------------");
		GD.Print("End of Act " + (currentAct + 1));
		GD.Print("================");
		
		currentAct++;
	}
	
	public void CreatePatronRelationships()
	{
		// Now make a simple list of patrons..
		List<PatronType> patronList = new List<PatronType>();
		for(uint i = 0; i < (uint)PatronType.PATRON_COUNT; ++i)
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
		for(int i = 0; i < (int)PatronType.PATRON_COUNT; i+=2)
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
		copIndex = GD.Randi() % (uint)PatronType.PATRON_COUNT;
		
		// Pick the cop's attributes randomly
		DietType copDiet = (DietType)(typeof(DietType).GetRandomEnumValue());
		ItemType copDrink = Item.alcohol[ GD.Randi() % (uint)Item.alcohol.Length ];
		PolitcalAffiliation copPolAff = (PolitcalAffiliation)(typeof(PolitcalAffiliation).GetRandomEnumValue());
		CriminalBackground copCrimBack = (CriminalBackground)(typeof(CriminalBackground).GetRandomEnumValue());
		
		// Spawn the cop!
		patrons[copIndex] = new PatronDetails(copIndex, true, copDrink, copDiet, copPolAff, copCrimBack);
		
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
		
		for (uint i = 0; i < (uint)PatronType.PATRON_COUNT; ++i)
		{
			if(i != copIndex)
			{
				// Create our desired properties
				// TODO: NEED TO REDO IF THE SAME AS COP
				
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
							break;
						}
						
						case ClueType.CriminalClueType:
						{
							patronCrimBack = copCrimBack; 
							break;
						}
						
						case ClueType.RelationshipClueType:
						{
							// TODO!!
							break;
						}
					}
				}
				
				patrons[i] = new PatronDetails(i, false, (ItemType)patronDrink, patronDiet, patronPolAff, patronCrimBack );
			}
		}

		CreatePatronRelationships();
		
		// Debug!
		for (uint i = 0; i < ACT_COUNT; ++i)
		{
			GD.Print("Act " + (i+1) + ": The undercover cop" + acts[i].clue.GetClueText());
		}

		for (uint i = 0; i < (uint)PatronType.PATRON_COUNT; ++i)
		{
			patrons[i].DebugPrintDetails();
		}
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		randomOrderTime = GD.RandRange(minimumOrderTime, maximumOrderTime);
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
	
	public void GeneratePatronDialog(bool _isAClue)
	{
		uint randomCouple = GD.Randi() % 3; // Will be one of the three couples!
		uint talkerIndex = GD.Randi() % 2; // Either 0 or 1
		uint listenerIndex = talkerIndex == 0u ? 1u : 0u;
		
		uint actToAbout = GD.Randi() % 3;
		
		PatronType talker = acts[currentAct]._couples[randomCouple, talkerIndex];
		PatronType listener = acts[currentAct]._couples[randomCouple, listenerIndex];
		
		PatronType subject;
		DialogContext context;
		
		DialogType dialogType = (DialogType)(typeof(DialogType).GetRandomEnumValue());
		
		if(dialogType == DialogType.TalkAboutSelf)
		{
			// If I'm talking about myself, the subjust is ME!
			subject = talker;
		}	
		else
		{
			// This means we are talking about someone else
			// Figure out which of the remaining four we are talking about!
			List<PatronType> patronList = new List<PatronType>();
			for(uint i = 0; i < (uint)PatronType.PATRON_COUNT; ++i)
			{
				if( acts[currentAct]._couples[randomCouple, 0] != (PatronType)i &&
					acts[currentAct]._couples[randomCouple, 1] != (PatronType)i )
				{
					patronList.Add((PatronType)i);
				}
			}
			long subjectIndex = GD.Randi() % (int)patronList.Count;
			subject = patronList[ (int)subjectIndex ];
		}
		
		if(_isAClue)
		{
			context = acts[actToAbout].clue.dialogContext;
		}
		else
		{
			context = (DialogContext)(typeof(DialogContext).GetRandomEnumValue());
		}
		
		
		dialogSystem.GeneratePatronDialog(talker, listener, subject, dialogType, context, acts[actToAbout].clue.clueID);
	}


	double timer = 0;
	double randomOrderTime;
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(timer > randomOrderTime){

			var shuffled = Spawners.patrons.Clone() as Patron[];
			shuffled.Shuffle();
			
			for(int i = 0; i < 6; i++){
				var p = shuffled[i];
				if(p.currentState == Patron.State.IDLE){
					var item = p.GetRandomOrderableItem();
					p.CreateOrder(item);
					timer = 0;
					randomOrderTime = GD.RandRange(minimumOrderTime, maximumOrderTime);
					break;
				}
			}
		}
		

		timer += delta;
		

		/*
		while(currentAct < ACT_COUNT)
		{
			StartCurrentAct();
		
			while(!acts[currentAct].IsActOver())
			{
				// When we dish out clues is controlled by Game Flow
				bool isClue = GD.Randi() % 3 == 0;
				
				if(isClue)
				{
					acts[currentAct].clueCount--;
				}
				
				// The radio will be controlled by the Game Flow
				if(GD.Randi() % 6 == 0)
				{
					GenerateRadioMessage(isClue);
				}
				else
				{
					GeneratePatronDialog(isClue);
				}
			}
		
			GoToNextAct();
		 }
		*/
	}
	
}
