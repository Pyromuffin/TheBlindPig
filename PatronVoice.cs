using Godot;
using System;

public partial class PatronVoice : Node2D
{
	[Export]
	AudioStream[] VoiceStreams;

	[Export]
	public AudioStreamPlayer2D VoicePlayer;

	[Export]
	Timer VoiceCoolDown;

	[Export]
	public float MAX_PITCH = 1.1f;
	[Export]
	public float MIN_PITCH = 0.8f;

	bool Playing = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	AudioStream GetRandomSound()
	{
		return VoiceStreams[GD.RandRange( 0, VoiceStreams.Length - 1 )];
	}

	void RandomizePitch()
	{
		float newPitch = GD.Randf() * ( MAX_PITCH - MIN_PITCH ) + MIN_PITCH;
		VoicePlayer.PitchScale = newPitch;
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
			RandomizePitch();
			VoicePlayer.Stream = GetRandomSound();
			VoicePlayer.Play();
			await ToSignal(VoicePlayer, "finished");
			float CoolDownTime = GD.Randf() * 0.25f + 0.25f;
			VoiceCoolDown.WaitTime = CoolDownTime;
			VoiceCoolDown.Start();
			await ToSignal(VoiceCoolDown, "timeout");
			if( !Playing )
				break;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
