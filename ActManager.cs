using Godot;
using System;

public partial class ActManager : Node
{

	[Export] public ClueDirector director;
	[Export] public float actLength;
	[Export] public float fadeTime;

	[Export] Sprite2D splash1;
	[Export] Sprite2D splash2;
	[Export] Sprite2D splash3;

	double actTimer = 0;
	public static bool showingActTransition = true;

	void HideTransitions(){
		splash1.Hide();
		splash2.Hide();
		splash3.Hide();
	}

	void ShowActTransition(){
		if(director.currentAct == 0){
			splash1.Show();
		}
		if(director.currentAct == 1){
			splash2.Show();
		}
		if(director.currentAct == 2){
			splash3.Show();
		}
	}

	public override void _Ready(){
		ShowActTransition();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(actTimer > actLength){
			actTimer = 0;
			showingActTransition = true;
			director.currentAct++;
			ShowActTransition();
			director.StartCurrentAct();
		}

		if(!showingActTransition){
			actTimer += delta;
		}

		if(showingActTransition && Input.IsActionJustPressed("ui_accept")){
			HideTransitions();
			showingActTransition = false;
		}
	}
}

