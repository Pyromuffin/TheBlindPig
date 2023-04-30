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

	public NinePatchRect dialogBubble;
	public Node2D waiter;
	public Sprite2D icon, tail;
	
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
	
	PatronType futureRelationshipMechanic;

	public bool hasOrder = false;
	public ItemType desiredItem;
	
	Texture2D questionIcon;

	public Patron(uint _patronIndex, bool _isCop, uint _randomPatron)
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
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		dialogBubble = GetNode<NinePatchRect>("Request text");
		icon = GetNode<Sprite2D>("IconQuestionMark24");
		questionIcon = icon.Texture;
		waiter = GetParent().GetNode<Node2D>("Waiter");
		desiredItem = Item.GetRandomItem();
		tail = GetNode<Sprite2D>("BubbleTail");

		dialogBubble.Hide();
		icon.Hide();
		tail.Hide();

		RandomTimedOrder(Item.GetRandomItem());
	}

	public async void RandomTimedOrder(ItemType item){

		await ToSignal(GetTree().CreateTimer(GD.Randf() * randomTime), "timeout");
		CreateOrder(item);
	}

	bool fading = false;
	bool revealed = false;

	public void ResetOrder(){
		dialogBubble.Size = minimumDialogBoxSize;
		icon.Scale = Vector2.One * 0.4f;
		fading =  false;
		revealed = false;
		hasOrder = false;
		dialogBubble.Hide();
		icon.Hide();
		tail.Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 initialIconScale = Vector2.One * 0.4f;

		if(hasOrder){
			float distanceToWaiter =  (waiter.Position - Position).Length();
			float totalDistance = beginDialogGrowDistance - endDialogGrowDistance;
			float fraction = (distanceToWaiter - endDialogGrowDistance) / totalDistance;
			fraction = Mathf.Clamp(fraction, 0, 1);
			var size = minimumDialogBoxSize.Lerp(targetDialogBoxSize, 1 - fraction);
			dialogBubble.Size = size;
			icon.Scale = initialIconScale.Lerp(initialIconScale * iconScale, 1 - fraction);

			if(!fading && !revealed && fraction == 0){
				fading = true;
				Fade();
			}

			if(overlapper != null && Input.IsActionJustPressed("ui_accept")){
				var waiter = overlapper as Waiter;
				waiter.DeliverItem(this);
			}
		}
	}


	async void Fade(){
		var timer = 0.0;

		while(timer < iconTransitionTime){
			var fraction = timer / iconTransitionTime;
			icon.Modulate = new Color(1,1,1, 1- ((float)fraction));
			timer += GetProcessDeltaTime();
			await ToSignal(GetTree(), "process_frame");		
		}

		timer = 0.0;
		icon.Texture = Item.GetLargeIcon(desiredItem);

		while(timer < iconTransitionTime){
			var fraction = timer / iconTransitionTime;
			icon.Modulate = new Color(1,1,1, (float)fraction);
			timer += GetProcessDeltaTime();
			await ToSignal(GetTree(), "process_frame");		
		}

		fading = false;
		revealed = true;
	}
	
	
	
	public void CreateOrder(ItemType item){
		dialogBubble.Show();
		icon.Show();
		tail.Show();
		icon.Texture = questionIcon;
		desiredItem = item;
		hasOrder = true;
	}


	public Node2D overlapper;
	
	private void _on_area_2d_body_entered(Node2D body)
	{
		overlapper = body;
	}


	private void _on_area_2d_body_exited(Node2D body)
	{
		overlapper = null;
	}
}



