using Godot;
using System;
using System.Threading.Tasks;

public partial class AudioDirector : Node
{
	[Export]
	public AudioStreamPlayer2D Bass;
	
	[Export]
	public AudioStreamPlayer2D Treble;
	
	AudioStream BassSound = ResourceLoader.Load("res://assets/music/JamRagBass.mp3") as AudioStream;
	AudioStream TrebleSound = ResourceLoader.Load("res://assets/music/JamRagTreble.mp3") as AudioStream;
	AudioStream IntroSound = ResourceLoader.Load("res://assets/music/JamRagIntro.mp3") as AudioStream;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CallDeferred( "SetupMusic" );
	}

	public async void SetupMusic()
	{
		await ToSignal(Treble, "finished");
		Treble.Stream = TrebleSound;
		Treble.Play();
		Bass.Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
