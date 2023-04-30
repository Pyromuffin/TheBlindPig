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
	None = -1,
	EscapeArtist,
	JazzMusician,
	Spiritualist,
	Journalist,
	BaseballPlayer,
	Flapper,
	PATRON_COUNT
}

public enum CriminalBackground
{
	Bootlegger,
	RumRunner,
	Moonshiner,
	Bribery,
	Smuggling,
	CRIMINALBACKGROUND_COUNT
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

public class PatronDetails {
		// Patron details
	public PatronType patronType;
	public DietType dietType;
	public ItemType hatedDrink;
	
	
	// ARE THEY THE COP!?
	public bool isTheCop;
	
	// Properties for clues and dialog
	public PolitcalAffiliation politcalAffiliation;
	public CriminalBackground criminalBackground;
	
	public uint loudness;
	
	public PatronType relationPatron;
	public RelationshipType relationshipType;
	
	public PatronDetails(int _patronIndex, bool _isCop, ItemType _hatedDrink, DietType _dietType, PolitcalAffiliation _polAff, CriminalBackground _crimBackground)
   	{
		patronType = (PatronType)_patronIndex;
		isTheCop = _isCop;
		
		loudness = GD.Randi() % 5 + 5;
		
		hatedDrink =_hatedDrink;
		dietType = _dietType;
		
		politcalAffiliation = _polAff;
		criminalBackground = _crimBackground;
		
		relationPatron = PatronType.None;
		relationshipType = RelationshipType.None;
	}
	
	public void DebugPrintDetails()
	{
		GD.Print("{ " + patronType + " }");
		
		GD.Print("Loudness: " + loudness);
		GD.Print("Diet Type: " + dietType);
		GD.Print("Hated Drink: " + hatedDrink);
		GD.Print("Politcal Affiliation: " + politcalAffiliation );
		GD.Print("Criminal Background: " + criminalBackground);
		GD.Print(relationshipType + " to " + relationPatron );
		GD.Print(isTheCop?"IM THE FUCKING COP":"Not the cop");
		GD.Print("======================");
	}
	
}


public partial class Patron : Sprite2D
{

	[Export] public Vector2 minimumDialogBoxSize;
	[Export] public Vector2 targetDialogBoxSize;
	[Export] public float beginDialogGrowDistance;
	[Export] public float endDialogGrowDistance;
	[Export] public float iconScale;
	[Export] public float iconTransitionTime;
	[Export] public float deliverySuspicionReduction;
	[Export] public float randomTime;

	[Export]
	public NinePatchRect dialogBubble;
	public Node2D waiter;
	[Export]
	public Sprite2D DialogIcon, TailIcon;

	enum State
	{
		IDLE,
		ORDERING,
		TALKING,
		HEARD_TALKING
	}
	State currentState;
	
	public ItemType desiredItem;
	string currentDialog;
	
	[Export]
	Texture2D questionIcon;
	[Export]
	Texture2D dotsIcon;

	[Export]
	PatronVoice patronVoice;
	[Export]
	Label patronText;

	public PatronDetails details;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		waiter = GetParent().GetNode<Node2D>("Waiter");
		//desiredItem = Item.GetRandomItem();
		//RandomTimedOrder(Item.GetRandomItem());
		EnterState( State.IDLE );
		//type =(PatronType)(GD.Randi() % 6);
		
	}

	public void Init(PatronDetails d) {
		details = d;
		Texture = GD.Load<Texture2D>("res://assets/characters/" + details.patronType.ToString().ToLower() + ".png");
	}

	void ResetDialog()
	{
		dialogBubble.Size = minimumDialogBoxSize;
		DialogIcon.Scale = Vector2.One * 0.4f;
		fading =  false;
		revealed = false;

		dialogBubble.Hide();
		DialogIcon.Hide();
		TailIcon.Hide();
		patronText.Hide();
	}

	void EnterState( State newState )
	{
		ResetDialog();
		switch( newState )
		{
			case( State.ORDERING ):
				EnterOrder();
				break;
			case( State.TALKING ):
				EnterTalking();
				break;
		}
		currentState = newState;
	}

	public async void RandomTimedOrder(ItemType item){

		await ToSignal(GetTree().CreateTimer(GD.Randf() * randomTime), "timeout");
		CreateOrder(item);
	}

	bool fading = false;
	bool revealed = false;

	public void ResetOrder() 
	{
		EnterState( State.IDLE );
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if( currentState == State.ORDERING ){
			growDialog();
			if(overlapper != null && Input.IsActionJustPressed("ui_accept")){
				var waiter = overlapper as Waiter;
				waiter.DeliverItem(this);
			}
		}
		else if( currentState == State.TALKING ){
			growDialog();
		}
	}

	void growDialog()
	{
		Vector2 initialIconScale = Vector2.One * 0.4f;
		float distanceToWaiter =  (waiter.Position - Position).Length();
		float totalDistance = beginDialogGrowDistance - endDialogGrowDistance;
		float fraction = (distanceToWaiter - endDialogGrowDistance) / totalDistance;
		fraction = Mathf.Clamp(fraction, 0, 1);
		var size = minimumDialogBoxSize.Lerp(targetDialogBoxSize, 1 - fraction);
		dialogBubble.Size = size;
		DialogIcon.Scale = initialIconScale.Lerp(initialIconScale * iconScale, 1 - fraction);
		if( currentState == State.TALKING )
		{
			patronVoice.SetPlaying( fraction < 1 );
			patronVoice.SetVolume( fraction );
		}
		
		if(!fading && !revealed && fraction == 0){
			fading = true;
			Fade();
		}
	}


	async void Fade(){
		var timer = 0.0;

		while(timer < iconTransitionTime){
			var fraction = timer / iconTransitionTime;
			DialogIcon.Modulate = new Color(1,1,1, 1- ((float)fraction));
			timer += GetProcessDeltaTime();
			await ToSignal(GetTree(), "process_frame");		
		}

		timer = 0.0;
		DialogIcon.Texture = Item.GetLargeIcon(desiredItem);
		patronText.Modulate = new Color(1,1,1, 0);
		patronText.Show();

		while(timer < iconTransitionTime){
			var fraction = timer / iconTransitionTime;
			if( currentState == State.ORDERING )
				DialogIcon.Modulate = new Color(1,1,1, (float)fraction);
			else if( currentState == State.TALKING )
				patronText.Modulate = new Color(1,1,1, (float)fraction);

			timer += GetProcessDeltaTime();
			await ToSignal(GetTree(), "process_frame");		
		}

		fading = false;
		revealed = true;
	}


	void EnterOrder()
	{
		dialogBubble.Show();
		DialogIcon.Show();
		TailIcon.Show();
		DialogIcon.Texture = questionIcon;
	}

	void EnterTalking()
	{
		dialogBubble.Show();
		DialogIcon.Show();
		TailIcon.Show();
		DialogIcon.Texture = dotsIcon;
	}
	
	public void CreateOrder(ItemType item){
		desiredItem = item;
		EnterState( State.ORDERING );
	}

	public void CreateDialog(string text){
		currentDialog = text;
		EnterState( State.TALKING );
	}


	public Node2D overlapper;
	
	private void Enter(Node2D body)
	{
		if(body is Waiter){
			overlapper = body;
		}
	}

	private void Exit(Node2D body)
	{
		overlapper = null;
	}
}





