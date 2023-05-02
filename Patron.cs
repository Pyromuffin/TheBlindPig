using Godot;
using System;

// Random Enum function
/*public static class EnumExtensions
{
	public static Enum GetRandomEnumValue(this Type t)
	{
		return Enum.GetValues(t)          // get values from Type provided
			.OfType<Enum>()               // casts to Enum
			.OrderBy(e => Guid.NewGuid()) // mess with order of results
			.FirstOrDefault();            // take first item in result
	}
}*/

public enum PatronType
{
	EscapeArtist,
	JazzMusician,
	Spiritualist,
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
	BearParty,
	RabbitParty,
	LeopardParty,
}


public enum RelationshipType
{
	Rival,
	Lover,
	Ex,
	None,
}

public class PatronDetails
{
	// Patron details
	public PatronType patronType;
	public DietType dietType;
	public ItemType hatedDrink;
	public string patronName;

	// ARE THEY THE COP!?
	public bool isTheCop;

	// Properties for clues and dialog
	public PolitcalAffiliation politcalAffiliation;
	public CriminalBackground criminalBackground;

	public uint loudness;

	public PatronType relationPatron;
	public RelationshipType relationshipType;

	public float idleSpeedScale;

	const bool DEBUG_SYSTEM = false;

	public PatronDetails(uint _patronIndex, bool _isCop, ItemType _hatedDrink, DietType _dietType, PolitcalAffiliation _polAff, CriminalBackground _crimBackground)
	{
		patronType = (PatronType)_patronIndex;
		isTheCop = _isCop;
		loudness = GD.Randi() % 5 + 5;

		hatedDrink = _hatedDrink;
		dietType = _dietType;

		politcalAffiliation = _polAff;
		criminalBackground = _crimBackground;

		relationPatron = PatronType.EscapeArtist;
		relationshipType = RelationshipType.None;

		switch (patronType)
		{
			case PatronType.EscapeArtist:
				{
					patronName = "Sharksy";
					break;
				}
			case PatronType.JazzMusician:
				{
					patronName = "Lionel";
					break;
				}
			case PatronType.Spiritualist:
				{
					patronName = "Birdie";
					break;
				}
			case PatronType.Journalist:
				{
					patronName = "Puppi";
					break;
				}
			case PatronType.BaseballPlayer:
				{
					patronName = "Gee Raffe";
					break;
				}
			case PatronType.Flapper:
				{
					patronName = "Bunnie";
					break;
				}
			default:
				patronName = "";
				break;
		}
	}

	public void DebugPrintDetails()
	{
		if (DEBUG_SYSTEM)
		{
			GD.Print("{ " + patronType + " }");

			GD.Print("Loudness: " + loudness);
			GD.Print("Diet Type: " + dietType);
			GD.Print("Hated Drink: " + hatedDrink);
			GD.Print("Politcal Affiliation: " + politcalAffiliation);
			GD.Print("Criminal Background: " + criminalBackground);
			GD.Print(relationshipType + " to " + relationPatron);
			GD.Print(isTheCop ? "IM THE FUCKING COP" : "Not the cop");
			GD.Print("======================");
		}
	}
}


public partial class Patron : Sprite2D
{

	[Export] public Vector2 minimumDialogBoxSize;
	[Export] public float beginDialogGrowDistance;
	[Export] public float endDialogGrowDistance;
	[Export] public float iconScale;
	[Export] public float iconTransitionTime;
	[Export] public float deliverySuspicionReduction;

	[Export]
	public NinePatchRect dialogBubble;
	public Node2D waiter;
	[Export]
	public Sprite2D DialogIcon, TailIcon;

	public enum State
	{
		IDLE,
		ORDERING,
		TALKING
	}
	public State currentState;

	public ItemType desiredItem;
	string currentDialog;

	[Export] float dialogAdvanceTime;

	[Export]
	Texture2D questionIcon;
	[Export]
	Texture2D dotsIcon;

	[Export]
	PatronVoice patronVoice;
	[Export]
	Label patronText;

	[Export]
	AnimationPlayer animationPlayer;
	[Export]
	AnimationPlayer characterAnimation;


	public PatronDetails details;
	[Export]
	public float characterAnimationSpeedScale;
	[Export]
	public bool isPig;
	[Export]
	public ActManager actManager;
	bool hasStartedAnimating = false;
	float animStartTime;
	[Export] public double crisisModeTransitionTime = 5.0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		waiter = GetParent().GetNode<Node2D>("Waiter");
		EnterState(State.IDLE);
		characterAnimation.SpeedScale = characterAnimationSpeedScale;
	}

	public void Init(PatronDetails d)
	{
		details = d;
		Texture = GD.Load<Texture2D>("res://assets/characters/" + details.patronType.ToString().ToLower() + ".png");
		patronVoice.Init(details.patronType);
		switch (details.patronType)
		{
			case (PatronType.Journalist):
				characterAnimationSpeedScale = 1.1f;
				break;
			case (PatronType.BaseballPlayer):
				characterAnimationSpeedScale = 0.67f;
				break;
			case (PatronType.Flapper):
				characterAnimationSpeedScale = 1.3f;
				break;
			case (PatronType.Spiritualist):
				characterAnimationSpeedScale = 0.43f;
				break;
			case (PatronType.EscapeArtist):
				characterAnimationSpeedScale = 0.33f;
				break;
			case (PatronType.JazzMusician):
				characterAnimationSpeedScale = 0.53f;
				break;
		}
		characterAnimation.SpeedScale = characterAnimationSpeedScale;
	}

	void ResetDialog()
	{
		dialogBubble.Size = minimumDialogBoxSize;
		DialogIcon.Scale = Vector2.One * 0.4f;
		DialogIcon.Modulate = new Color(1, 1, 1, 1);
		fading = false;
		revealed = false;

		dialogBubble.Hide();
		DialogIcon.Hide();
		TailIcon.Hide();
		patronText.Hide();
		hasStartedAnimating = false;
	}

	void EnterState(State newState)
	{
		ResetDialog();
		patronVoice.SetPlaying(false);

		switch (newState)
		{
			case (State.ORDERING):
				EnterOrder();
				break;
			case (State.TALKING):
				EnterTalking();
				break;
			case (State.IDLE):
				EnterIdle();
				break;
		}
		currentState = newState;
	}



	bool fading = false;
	bool revealed = false;

	public void ResetOrder()
	{
		EnterState(State.IDLE);
	}

	bool dialogHeard = false;
	double endingTimer = 0;
	double dialogMissTimer, dialogFadeTimer;


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (ActManager.showingActTransition)
		{
			return;
		}

		if (ActManager.isEnding)
		{
			endingTimer += delta;

			if (currentState != State.IDLE)
				EnterState( State.IDLE );

			if ( !isPig && overlapper != null && Input.IsActionJustPressed("ui_accept") && endingTimer > crisisModeTransitionTime)
			{
				if( details.isTheCop )
				{
					GetTree().ChangeSceneToFile("res://scenes/GoodEnd.tscn");
				}
				else
				{
					GetTree().ChangeSceneToFile("res://scenes/BadEnd.tscn");
				}
			}
			return;
		}

		if( isPig )
		{
			if (overlapper != null )
			{
				if (Input.IsActionJustPressed("ui_accept"))
				{
					actManager.ShowEndingConfirmation();
				}
			}
			
			return;
		}

		if (currentState == State.ORDERING)
		{
			growDialog();
			if (overlapper != null && Input.IsActionJustPressed("ui_accept"))
			{
				var waiter = overlapper as Waiter;
				waiter.DeliverItem(this);
			}
		}
		else if (currentState == State.TALKING)
		{
			growDialog();
			if (dialogHeard)
			{
				if (dialogFadeTimer > 5)
				{
					ResetOrder();
					dialogHeard = false;
					dialogFadeTimer = 0;
				}
				dialogFadeTimer += delta;
			}
			else
			{
				if (dialogMissTimer > 15)
				{
					ResetOrder();
					dialogHeard = false;
					dialogMissTimer = 0;
				}
				dialogMissTimer += delta;
			}
		}
	}

	void growDialog()
	{
		Vector2 initialIconScale = Vector2.One * 0.4f;
		float distanceToWaiter = (waiter.Position - Position).Length();
		float totalDistance = beginDialogGrowDistance - endDialogGrowDistance;
		float fraction = (distanceToWaiter - endDialogGrowDistance) / totalDistance;
		fraction = Mathf.Clamp(fraction, 0, 1);

		if (currentState == State.TALKING)
		{
			patronVoice.SetPlaying(fraction < 1);
			patronVoice.SetVolume(fraction);
			if (Position.X < -72)
				animationPlayer.Play("TalkBoxRight");
			else
				animationPlayer.Play("TalkBoxLeft");

			animationPlayer.Seek((1 - fraction) * 1.8f);
			if (1 - fraction == 1)
			{
				dialogHeard = true;
				characterAnimation.SpeedScale = 2.5f;
			}
			else
			{
				characterAnimation.SpeedScale = characterAnimationSpeedScale;
			}
		}
		else if (currentState == State.ORDERING)
		{
			animationPlayer.Play("IconBoxRight");
			animationPlayer.Seek(1 - fraction);
			if (1 - fraction == 1)
				DialogIcon.Texture = Item.GetLargeIcon(desiredItem);
		}

		if( animStartTime != 0 )
		{
			animStartTime = 0;
			ShowDialog(currentState == State.TALKING);
		}

		if( !hasStartedAnimating )
		{
			animStartTime = Time.GetTicksMsec();
			hasStartedAnimating = true;
		}	
	}


	async void Fade()
	{
		var timer = 0.0;

		while (timer < iconTransitionTime)
		{
			var fraction = timer / iconTransitionTime;
			DialogIcon.Modulate = new Color(1, 1, 1, 1 - ((float)fraction));
			timer += GetProcessDeltaTime();
			await ToSignal(GetTree(), "process_frame");
		}

		timer = 0.0;
		DialogIcon.Texture = Item.GetLargeIcon(desiredItem);
		patronText.Modulate = new Color(1, 1, 1, 0);
		patronText.Show();

		while (timer < iconTransitionTime)
		{
			var fraction = timer / iconTransitionTime;
			if (currentState == State.ORDERING)
				DialogIcon.Modulate = new Color(1, 1, 1, (float)fraction);
			else if (currentState == State.TALKING)
				patronText.Modulate = new Color(1, 1, 1, (float)fraction);

			timer += GetProcessDeltaTime();
			await ToSignal(GetTree(), "process_frame");
		}

		fading = false;
		revealed = true;
	}

	void ShowDialog( bool showText )
	{
		dialogBubble.Show();
		DialogIcon.Show();
		TailIcon.Show();

		if ( showText )
			patronText.Show();
	}


	void EnterOrder()
	{
		if( hasStartedAnimating )
			ShowDialog( false );

		DialogIcon.Texture = questionIcon;
	}

	void EnterTalking()
	{
		if( hasStartedAnimating )
			ShowDialog( true );

		DialogIcon.Texture = dotsIcon;
	}

	void EnterIdle()
	{
		characterAnimation.Play("Idle");
	}


	bool IsItemDrink(ItemType item)
	{
		return item == ItemType.Absinthe || item == ItemType.Bourbon || item == ItemType.Cocktail || item == ItemType.Wine;
	}

	bool IsAcceptableForDiet(ItemType item, DietType diet)
	{

		if (IsItemDrink(item))
		{
			return true;
		}

		if (diet == DietType.Carnivore)
		{
			return item == ItemType.Meat;
		}

		if (diet == DietType.Herbivore)
		{
			return item == ItemType.Cake || item == ItemType.Carrot;
		}

		return true;
	}

	public ItemType GetRandomOrderableItem(bool orderDrink)
	{
		var diet = details.dietType;
		var hated = details.hatedDrink;


		var items = orderDrink ? Item.alcohol : Item.foods;

		items.Shuffle();

		foreach (var i in items)
		{
			if (IsAcceptableForDiet(i, diet) && i != hated)
			{
				return i;
			}
		}

		throw new InvalidOperationException();
	}

	public void CreateOrder(ItemType item)
	{
		desiredItem = item;
		EnterState(State.ORDERING);
	}

	public void CreateDialog(string text)
	{
		currentDialog = text;
		patronText.Text = text;
		EnterState(State.TALKING);
	}

	public void StartIdle()
	{
		EnterState(State.IDLE);
	}


	public Node2D overlapper;

	private void Enter(Node2D body)
	{
		if (body is Waiter)
		{
			overlapper = body;
		}
	}

	private void Exit(Node2D body)
	{
		overlapper = null;
	}
}





