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
	[Export] Label clueText;

	double actTimer = 0;
	public static bool showingActTransition = true;
	public static bool isEnding = false;

	void HideTransitions(){
		splash1.Hide();
		splash2.Hide();
		splash3.Hide();
		clueText.Hide();
	}

	void ShowActTransition(){

		clueText.Show();
		clueText.Text = "The cop" + director.acts[0].clue.GetClueText() + 
		"\nThe cop" + director.acts[1].clue.GetClueText() +
		"\nThe cop" + director.acts[2].clue.GetClueText();


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

	public void StartEnding()
	{
		isEnding = true;
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

