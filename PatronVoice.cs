using Godot;
using System;

public partial class PatronVoice : Node2D
{
	[Export]
	AudioStream[] VoiceStreams;

	[Export]
	public AudioStreamPlayer2D VoicePlayer;

	bool Playing = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SetPlaying( true );
	}

	AudioStream GetRandomSound()
	{
		return VoiceStreams[GD.RandRange( 0, VoiceStreams.Length - 1 )];
	}

	public void SetPlaying( bool playing )
	{
		if( playing == true && !Playing )
		{
			PlayVoice();
		}

		Playing = playing;
	}

	public async void PlayVoice()
	{
		for( ;; )
		{
			VoicePlayer.Stream = GetRandomSound();
			VoicePlayer.Play();
			await ToSignal(VoicePlayer, "finished");

			if( !Playing )
				break;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
