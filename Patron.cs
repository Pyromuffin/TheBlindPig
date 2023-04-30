using Godot;
using System;

public partial class Patron : Sprite2D
{
	[Export] public Vector2 minimumDialogBoxSize;
	[Export] public Vector2 targetDialogBoxSize;
	[Export] public float beginDialogGrowDistance;
	[Export] public float endDialogGrowDistance;
	[Export] public float iconScale;
	[Export] public float iconTransitionTime;
	
	public NinePatchRect dialogBubble;
	public Node2D waiter;
	public Sprite2D icon;

	[Export]
	public Texture2D desired;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		dialogBubble = GetNode<NinePatchRect>("Request text");
		icon = GetNode<Sprite2D>("IconQuestionMark24");
		waiter = GetParent().GetNode<Node2D>("Waiter");
	}

	bool fading = false;
	bool revealed = false;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 initialIconScale = Vector2.One * 0.4f;


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
		icon.Texture = desired;

		while(timer < iconTransitionTime){
			var fraction = timer / iconTransitionTime;
			icon.Modulate = new Color(1,1,1, (float)fraction);
			timer += GetProcessDeltaTime();
			await ToSignal(GetTree(), "process_frame");		
		}

		fading = false;
		revealed = true;
	}
}
