using Godot;
using System;

public partial class FingerPrompt : NinePatchRect
{
	[Export] public ActManager actManager;

	public void _on_yes_pressed(){
		actManager.StartEnding();
		Hide();
	}

	public void _on_no_pressed(){
		Hide();
	}
}
