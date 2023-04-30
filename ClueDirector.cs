using Godot;
using System;
using System.Linq;

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

public enum Drink {
	Wine,
	OldFashioned,
	Cocktail,
	Absinthe,
	DRINK_COUNT = Absinthe,
}

public enum Food  {
	Meat,
	StrawberryCake,
	Carrot,
	FOOD_COUNT = Carrot
}

public enum PatronType
{
	EscapeArtist,
	JazzMusician,
	Spritualist,
	Journalist,
	BaseballPlayer,
	Flapper,
}

public enum CriminalBackground
{
	Bootlegger,
	RumRunner,
	Moonshiner,
	Bribery,
	Smuggling,
}

public enum PolitcalAffiliation
{
	HippoParty,
	BearParty,
	RabbitParty,
}

class Patron
{	
	PatronType patronType;
	bool isTheCop;
	PatronType futureRelationshipMechanic;
	
	uint loudness;
	
	// Ordering Observation Task
	public Drink hatedDrink;
	public Food hatedFood;
	//public Drink favoriteDrink;
	//public Food favoriteFood;
	
	// Converstation Task
	public PolitcalAffiliation polAff;
	public CriminalBackground criminalBackground;
	
	public Patron(uint _patronIndex, bool _isCop, uint _randomPatron)
   	{
		patronType = (PatronType)_patronIndex;
		isTheCop = _isCop;
		
		loudness = GD.Randi() % 5 + 5;
		
		hatedDrink = (Drink)(typeof(Drink).GetRandomEnumValue());
		hatedFood = (Food)(typeof(Food).GetRandomEnumValue());
		
		polAff = (PolitcalAffiliation)(typeof(PolitcalAffiliation).GetRandomEnumValue());
		criminalBackground = (CriminalBackground)(typeof(CriminalBackground).GetRandomEnumValue());
		
		futureRelationshipMechanic = (PatronType)_randomPatron;
		
		//int hatedOffset = GD.Randi() % ((int)Drink.DRINK_COUNT - 1);
		//hatedDrink = (Drink)((int)(favoriteDrink + hatedOffset + 1) % (int)Drink.DRINK_COUNT);
		//int hatedFoodOffset  = rnd.Next((int)Food.FOOD_COUNT - 1);
		//hatedFood = (Food)((int)(favoriteDrink + hatedFoodOffset + 1) % (int)Food.FOOD_COUNT);
		
		GD.Print("{ " + patronType + " }");
		
		GD.Print("Loudness: " + loudness);
		GD.Print("Hated Food: " + hatedFood);
		GD.Print("Hated Food: " + hatedDrink);
		GD.Print("Politcal Affiliation: " + polAff);
		GD.Print("Criminal Background: " + criminalBackground);
		GD.Print("Connection to: " + (futureRelationshipMechanic != patronType ? futureRelationshipMechanic : "NONE"));
		GD.Print(isTheCop?"IM THE FUCKING COP":"Not the cop");
		GD.Print("======================");
	}
}

abstract class Clue 
{
	public uint clueID;
	
	public  void SetClueID(uint _clueID)
	{
		clueID = _clueID;
	}
	
	public abstract uint GetClueValue();
	public abstract string GetClueText();
}

class DietaryClue : Clue
{
	public override uint GetClueValue() 
	{
		return (uint)clueID;
	}
	
	public override string GetClueText()
	{
		return "The cop will never order " + (Food)clueID;
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
	const uint ACT_COUNT = 2;
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
		
		for (uint i = 0; i < ACT_COUNT; ++i)
		{
			DietaryClue clue = new DietaryClue();
			clue.SetClueID((uint)patrons[copIndex].hatedFood);
			
			acts[i] = new Act(clue);
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
