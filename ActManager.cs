using Godot;
using System;

public partial class ActManager : Node
{

	[Export] public ClueDirector director;
	[Export] public float actLength;
	[Export] public float fadeTime;

	[Export] AnimatedSprite2D splash1;
	[Export] Label clueText;
	[Export] Sprite2D clueWindow;
	[Export] Suspicion suspicion;
	[Export] Sprite2D chooseCop;

	[Export] NinePatchRect confirmDialog;

	double actTimer = 0;
	public static bool showingActTransition = true;
	public static bool isEnding = false;

	public void ShowEndingConfirmation(){
		confirmDialog.Show();
	}

	void HideTransitions(){
		splash1.Hide();
		clueText.Hide();
		clueWindow.Hide();
	}

	void ShowActTransition(){

		clueWindow.Show();
		clueText.Show();
		clueText.Text = "* The cop" + director.acts[0].clue.GetClueText() + "." + 
		"\n* The cop" + director.acts[1].clue.GetClueText() +"."+
		"\n* The cop" + director.acts[2].clue.GetClueText() + "." +
		"\nCome back when you find the weasel!";


		splash1.Show();
		splash1.Play();
	}

	public void StartEnding()
	{
		isEnding = true;
		suspicion.Hide();
		chooseCop.Show();
	}

	public override void _Ready(){
		ShowActTransition();
		director.StartCurrentAct();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		/*
		if(actTimer > actLength){
			actTimer = 0;
			showingActTransition = true;
			// director.currentAct++;
			ShowActTransition();
			director.StartCurrentAct();
		}

		if(!showingActTransition){
			actTimer += delta;
		}
		*/

		if(showingActTransition && Input.IsActionJustPressed("ui_accept")){
			HideTransitions();
			showingActTransition = false;
		}


		if(!showingActTransition){
			if(Input.IsActionJustPressed("ui_focus_next")){
				showingActTransition = true;
				ShowActTransition();
			}
		} else {
			if(Input.IsActionJustPressed("ui_focus_next")){
				showingActTransition = false;
				HideTransitions();
			}
		}
		


	}
}

