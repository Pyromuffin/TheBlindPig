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

abstract class Clue 
{
	public uint clueID;
	
	public void SetClueID(uint _clueID)
	{
		clueID = _clueID;
	}
	
	public abstract uint GetClueValue();
	public abstract string GetClueText();
}

class AlchoholClue : Clue
{
	public override uint GetClueValue() 
	{
		return (uint)clueID;
	}
	
	public override string GetClueText()
	{
		return "Alchohol Clue: " + (ItemType)clueID;
	}
}

class DietClue : Clue
{
	public override uint GetClueValue() 
	{
		return (uint)clueID;
	}
	
	public override string GetClueText()
	{
		return "Diet Clue: " + (DietType)clueID;
	}
}

class PoliticalClue : Clue
{
	public override uint GetClueValue() 
	{
		return (uint)clueID;
	}
	
	public override string GetClueText()
	{
		return "Political Clue: " + (Patron.PolitcalAffiliation)clueID;
	}
}

class CriminalClue : Clue
{
	public override uint GetClueValue() 
	{
		return (uint)clueID;
	}
	
	public override string GetClueText()
	{
		return "Criminal Clue: " + (Patron.CriminalBackground)clueID;
	}
}

class Act 
{
	public Clue clue;
	
	public Act(Clue _clue)
	{
		clue = _clue;
	}
}

public partial class ClueDirector : Node2D
{
	[Signal]
	public delegate void SendDialogEventHandler();
	
	const uint PATRON_COUNT = 6;
	const uint ACT_COUNT = 3;
	uint copIndex;
	
	Patron[] patrons = new Patron[PATRON_COUNT];
	Act[] acts = new Act[ACT_COUNT];
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		copIndex = GD.Randi() % PATRON_COUNT;
		for (uint i = 0; i < PATRON_COUNT; ++i)
		{
			uint randomPatron = GD.Randi() % PATRON_COUNT;
			patrons[i] = new Patron(i, (bool)(copIndex == i), randomPatron) ;
		}
		List<int> numberList = new List<int>();
		numberList.Add(0);
		numberList.Add(1);
		numberList.Add(2);
		numberList.Add(3);
		
		for (uint i = 0; i < ACT_COUNT; ++i)
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
				DietClue clue = new DietClue();
				clue.SetClueID((uint)patrons[copIndex].dietType);
			
				acts[i] = new Act(clue);
			}
			else if(clueType == 2)
			{
				CriminalClue clue = new CriminalClue();
				clue.SetClueID((uint)patrons[copIndex].criminalBackground);
			
				acts[i] = new Act(clue);
			}
			else if(clueType == 3)
			{
				AlchoholClue clue = new AlchoholClue();
				clue.SetClueID((uint)patrons[copIndex].hatedDrink);
			
				acts[i] = new Act(clue);
			}
			
			numberList.RemoveAt((int)clueIndex);
		}
		
		// Debug!
		for (uint i = 0; i < ACT_COUNT; ++i)
		{
			GD.Print(GetClueText(i));
		}
	}
	
	public String GetClueText(uint actIndex = 0)
	{
		return acts[actIndex].clue.GetClueText();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
