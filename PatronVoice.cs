using Godot;
using System;
using System.Collections.Generic;

public partial class PatronVoice : Node2D
{
	List<AudioStream> VoiceStreams;

	[Export]
	public AudioStreamPlayer VoicePlayer;

	[Export]
	Timer VoiceCoolDown;

	[Export]
	public float MAX_PITCH = 1.1f;
	[Export]
	public float MIN_PITCH = 0.8f;

	[Export]
	public float VOLUME_RANGE = 60f;

	bool Playing = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SetPlaying( false );
	}

	AudioStream GetRandomSound()
	{
		return VoiceStreams[GD.RandRange( 0, VoiceStreams.Count - 1 )];
	}

	public void Init(PatronType type)
	{
		VoiceStreams = new List<AudioStream>();
		string voiceDirectory = "res://assets/sounds/Voices/";
		switch ( type )
		{
			case(PatronType.Journalist):
				voiceDirectory = voiceDirectory + "journalist/";
				break;
			case(PatronType.BaseballPlayer):
				voiceDirectory = voiceDirectory + "giraffe/";
				break;
			case(PatronType.Flapper):
				voiceDirectory = voiceDirectory + "rabbit/";
				break;
			case(PatronType.Spiritualist):
				voiceDirectory = voiceDirectory + "bird/";
				break;
			default:
			case(PatronType.JazzMusician):
				voiceDirectory = voiceDirectory + "lion/";
				break;
		}

		DirAccess voiceDir = DirAccess.Open( voiceDirectory );
		string[] files = voiceDir.GetFiles();
		foreach( string file in files )
		{
			if( !file.Contains( "import" ) )
			{
				AudioStream stream = (AudioStream)GD.Load( voiceDirectory + file );
				VoiceStreams.Add( stream );
			}
		}
	}

	void RandomizePitch()
	{
		float newPitch = GD.Randf() * ( MAX_PITCH - MIN_PITCH ) + MIN_PITCH;
		VoicePlayer.PitchScale = newPitch;
	}

	public void SetVolume( float fraction )
	{
		GD.Print( ( 1 - fraction * VOLUME_RANGE ) - 80 );
		VoicePlayer.VolumeDb = ( ( 1 - fraction ) * VOLUME_RANGE ) - 80;
	}

	public void SetPlaying( bool playing )
	{
		if( playing == true && !Playing )
		{
			PlayVoice();
		}

		Playing = playing;
		if( !Playing )
		{
			SetVolume( 0 );
		}
	}

	public async void PlayVoice()
	{
		for( ;; )
		{
			RandomizePitch();
			VoicePlayer.Stream = GetRandomSound();
			VoicePlayer.Play();
			await ToSignal(VoicePlayer, "finished");
			float CoolDownTime = GD.Randf() * 0.1f;
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
