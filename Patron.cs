using Godot;
using System;


public enum PatronType
{
	None = -1,
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
	LeopardParty,
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
	
	public PatronType futureRelationshipMechanic;
	
	public PatronDetails(uint _patronIndex, bool _isCop, uint _randomPatron)
   	{
		patronType = (PatronType)_patronIndex;
		isTheCop = _isCop;
		
		loudness = GD.Randi() % 5 + 5;
		
		hatedDrink = Item.GetRandomDrink();
		dietType = Item.GetRandomDiet();
		
		politcalAffiliation = (PolitcalAffiliation)(typeof(PolitcalAffiliation).GetRandomEnumValue());
		criminalBackground = (CriminalBackground)(typeof(CriminalBackground).GetRandomEnumValue());
		
		futureRelationshipMechanic = (PatronType)_randomPatron;
				
		GD.Print("{ " + patronType + " }");
		
		GD.Print("Loudness: " + loudness);
		GD.Print("Diet Type: " + dietType);
		GD.Print("Hated Drink: " + hatedDrink);
		GD.Print("Politcal Affiliation: " + politcalAffiliation );
		GD.Print("Criminal Background: " + criminalBackground);
		GD.Print("Connection to: " + (futureRelationshipMechanic != patronType ? futureRelationshipMechanic : "NONE"));
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
	public Sprite2D PreviewIcon, TailIcon;

	enum State
	{
		IDLE,
		ORDERING,
		TALKING
	}
	State currentState;
	
	public ItemType desiredItem;
	string currentDialog;
	
	[Export]
	Texture2D questionIcon;
	[Export]
	Texture2D dotsIcon;

	[Export]
	PatronType type;

	public PatronDetails details;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		waiter = GetParent().GetNode<Node2D>("Waiter");
		desiredItem = Item.GetRandomItem();
		RandomTimedOrder(Item.GetRandomItem());
		EnterState( State.IDLE );
		type =(PatronType)(GD.Randi() % 6);
		Texture = GD.Load<Texture2D>("res://assets/characters/" + type.ToString().ToLower() + ".png");
	}

	void ResetDialog()
	{
		dialogBubble.Size = minimumDialogBoxSize;
		PreviewIcon.Scale = Vector2.One * 0.4f;
		fading =  false;
		revealed = false;

		dialogBubble.Hide();
		PreviewIcon.Hide();
		TailIcon.Hide();
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
		PreviewIcon.Scale = initialIconScale.Lerp(initialIconScale * iconScale, 1 - fraction);

		if(!fading && !revealed && fraction == 0){
			fading = true;
			Fade();
		}
	}


	async void Fade(){
		var timer = 0.0;

		while(timer < iconTransitionTime){
			var fraction = timer / iconTransitionTime;
			PreviewIcon.Modulate = new Color(1,1,1, 1- ((float)fraction));
			timer += GetProcessDeltaTime();
			await ToSignal(GetTree(), "process_frame");		
		}

		timer = 0.0;
		PreviewIcon.Texture = Item.GetLargeIcon(desiredItem);

		while(timer < iconTransitionTime){
			var fraction = timer / iconTransitionTime;
			PreviewIcon.Modulate = new Color(1,1,1, (float)fraction);
			timer += GetProcessDeltaTime();
			await ToSignal(GetTree(), "process_frame");		
		}

		fading = false;
		revealed = true;
	}


	void EnterOrder()
	{
		dialogBubble.Show();
		PreviewIcon.Show();
		TailIcon.Show();
		PreviewIcon.Texture = questionIcon;
	}

	void EnterTalking()
	{
		dialogBubble.Show();
		PreviewIcon.Show();
		TailIcon.Show();
		PreviewIcon.Texture = dotsIcon;
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
		overlapper = body;
	}

	private void Exit(Node2D body)
	{
		overlapper = null;
	}
}





