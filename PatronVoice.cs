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

	AudioStream BassSound = ResourceLoader.Load("res://assets/music/JamRagBass.mp3") as AudioStream;
	AudioStream TrebleSound = ResourceLoader.Load("res://assets/music/JamRagTreble.mp3") as AudioStream;
	AudioStream IntroSound = ResourceLoader.Load("res://assets/music/JamRagIntro.mp3") as AudioStream;

	AudioStream Bird01 = ResourceLoader.Load("res://assets/sounds/Voices/bird/Bird-01.mp3") as AudioStream;
	AudioStream Bird02 = ResourceLoader.Load("res://assets/sounds/Voices/bird/Bird-02.mp3") as AudioStream;
	AudioStream Bird03 = ResourceLoader.Load("res://assets/sounds/Voices/bird/Bird-03.mp3") as AudioStream;
	AudioStream Bird04 = ResourceLoader.Load("res://assets/sounds/Voices/bird/Bird-04.mp3") as AudioStream;
	AudioStream Bird05 = ResourceLoader.Load("res://assets/sounds/Voices/bird/Bird-05.mp3") as AudioStream;
	AudioStream Bird06 = ResourceLoader.Load("res://assets/sounds/Voices/bird/Bird-06.mp3") as AudioStream;
	AudioStream Bird07 = ResourceLoader.Load("res://assets/sounds/Voices/bird/Bird-07.mp3") as AudioStream;
	AudioStream Bird08 = ResourceLoader.Load("res://assets/sounds/Voices/bird/Bird-08.mp3") as AudioStream;
	AudioStream Bird09 = ResourceLoader.Load("res://assets/sounds/Voices/bird/Bird-09.mp3") as AudioStream;

	AudioStream Giraffe01 = ResourceLoader.Load("res://assets/sounds/Voices/giraffe/Giraffe-01.mp3") as AudioStream;
	AudioStream Giraffe02 = ResourceLoader.Load("res://assets/sounds/Voices/giraffe/Giraffe-02.mp3") as AudioStream;
	AudioStream Giraffe03 = ResourceLoader.Load("res://assets/sounds/Voices/giraffe/Giraffe-03.mp3") as AudioStream;
	AudioStream Giraffe04 = ResourceLoader.Load("res://assets/sounds/Voices/giraffe/Giraffe-04.mp3") as AudioStream;
	AudioStream Giraffe05 = ResourceLoader.Load("res://assets/sounds/Voices/giraffe/Giraffe-05.mp3") as AudioStream;
	AudioStream Giraffe06 = ResourceLoader.Load("res://assets/sounds/Voices/giraffe/Giraffe-06.mp3") as AudioStream;
	AudioStream Giraffe07 = ResourceLoader.Load("res://assets/sounds/Voices/giraffe/Giraffe-07.mp3") as AudioStream;
	AudioStream Giraffe08 = ResourceLoader.Load("res://assets/sounds/Voices/giraffe/Giraffe-08.mp3") as AudioStream;
	AudioStream Giraffe09 = ResourceLoader.Load("res://assets/sounds/Voices/giraffe/Giraffe-09.mp3") as AudioStream;
	AudioStream Giraffe10 = ResourceLoader.Load("res://assets/sounds/Voices/giraffe/Giraffe-10.mp3") as AudioStream;
	AudioStream Giraffe11 = ResourceLoader.Load("res://assets/sounds/Voices/giraffe/Giraffe-11.mp3") as AudioStream;
	AudioStream Giraffe12 = ResourceLoader.Load("res://assets/sounds/Voices/giraffe/Giraffe-12.mp3") as AudioStream;
	AudioStream Giraffe13 = ResourceLoader.Load("res://assets/sounds/Voices/giraffe/Giraffe-13.mp3") as AudioStream;

	AudioStream Pepp01 = ResourceLoader.Load("res://assets/sounds/Voices/journalist/Pepp-01.mp3") as AudioStream;
	AudioStream Pepp02 = ResourceLoader.Load("res://assets/sounds/Voices/journalist/Pepp-02.mp3") as AudioStream;
	AudioStream Pepp03 = ResourceLoader.Load("res://assets/sounds/Voices/journalist/Pepp-03.mp3") as AudioStream;
	AudioStream Pepp04 = ResourceLoader.Load("res://assets/sounds/Voices/journalist/Pepp-04.mp3") as AudioStream;
	AudioStream Pepp05 = ResourceLoader.Load("res://assets/sounds/Voices/journalist/Pepp-05.mp3") as AudioStream;
	AudioStream Pepp06 = ResourceLoader.Load("res://assets/sounds/Voices/journalist/Pepp-06.mp3") as AudioStream;
	AudioStream Pepp07 = ResourceLoader.Load("res://assets/sounds/Voices/journalist/Pepp-07.mp3") as AudioStream;
	AudioStream Pepp08 = ResourceLoader.Load("res://assets/sounds/Voices/journalist/Pepp-08.mp3") as AudioStream;
	// AudioStream Pepp09 = ResourceLoader.Load("res://assets/sounds/Voices/journalist/Pepp-09.mp3") as AudioStream;
	AudioStream Pepp10 = ResourceLoader.Load("res://assets/sounds/Voices/journalist/Pepp-10.mp3") as AudioStream;
	AudioStream Pepp11 = ResourceLoader.Load("res://assets/sounds/Voices/journalist/Pepp-11.mp3") as AudioStream;

	AudioStream Lion01 = ResourceLoader.Load("res://assets/sounds/Voices/lion/Lion-01.mp3") as AudioStream;
	AudioStream Lion02 = ResourceLoader.Load("res://assets/sounds/Voices/lion/Lion-02.mp3") as AudioStream;
	AudioStream Lion03 = ResourceLoader.Load("res://assets/sounds/Voices/lion/Lion-03.mp3") as AudioStream;
	AudioStream Lion04 = ResourceLoader.Load("res://assets/sounds/Voices/lion/Lion-04.mp3") as AudioStream;
	AudioStream Lion05 = ResourceLoader.Load("res://assets/sounds/Voices/lion/Lion-05.mp3") as AudioStream;
	AudioStream Lion06 = ResourceLoader.Load("res://assets/sounds/Voices/lion/Lion-06.mp3") as AudioStream;
	AudioStream Lion07 = ResourceLoader.Load("res://assets/sounds/Voices/lion/Lion-07.mp3") as AudioStream;
	AudioStream Lion08 = ResourceLoader.Load("res://assets/sounds/Voices/lion/Lion-08.mp3") as AudioStream;
	AudioStream Lion09 = ResourceLoader.Load("res://assets/sounds/Voices/lion/Lion-09.mp3") as AudioStream;

	AudioStream Rabbit01 = ResourceLoader.Load("res://assets/sounds/Voices/rabbit/Rabbit-01.mp3") as AudioStream;
	AudioStream Rabbit02 = ResourceLoader.Load("res://assets/sounds/Voices/rabbit/Rabbit-02.mp3") as AudioStream;
	AudioStream Rabbit03 = ResourceLoader.Load("res://assets/sounds/Voices/rabbit/Rabbit-03.mp3") as AudioStream;
	AudioStream Rabbit04 = ResourceLoader.Load("res://assets/sounds/Voices/rabbit/Rabbit-04.mp3") as AudioStream;
	AudioStream Rabbit05 = ResourceLoader.Load("res://assets/sounds/Voices/rabbit/Rabbit-05.mp3") as AudioStream;
	AudioStream Rabbit06 = ResourceLoader.Load("res://assets/sounds/Voices/rabbit/Rabbit-06.mp3") as AudioStream;
	AudioStream Rabbit07 = ResourceLoader.Load("res://assets/sounds/Voices/rabbit/Rabbit-07.mp3") as AudioStream;
	AudioStream Rabbit08 = ResourceLoader.Load("res://assets/sounds/Voices/rabbit/Rabbit-08.mp3") as AudioStream;
	AudioStream Rabbit09 = ResourceLoader.Load("res://assets/sounds/Voices/rabbit/Rabbit-09.mp3") as AudioStream;
	AudioStream Rabbit10 = ResourceLoader.Load("res://assets/sounds/Voices/rabbit/Rabbit-10.mp3") as AudioStream;

	bool Playing = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SetPlaying(false);
	}

	AudioStream GetRandomSound()
	{
		return VoiceStreams[GD.RandRange(0, VoiceStreams.Count - 1)];
	}

	public void Init(PatronType type)
	{
		VoiceStreams = new List<AudioStream>();
		string voiceDirectory = "res://assets/sounds/Voices/";
		switch (type)
		{
			case (PatronType.Journalist):
				voiceDirectory = voiceDirectory + "journalist/";
				break;
			case (PatronType.BaseballPlayer):
				voiceDirectory = voiceDirectory + "giraffe/";
				break;
			case (PatronType.Flapper):
				voiceDirectory = voiceDirectory + "rabbit/";
				break;
			case (PatronType.Spiritualist):
				voiceDirectory = voiceDirectory + "bird/";
				break;
			default:
			case (PatronType.JazzMusician):
				voiceDirectory = voiceDirectory + "lion/";
				break;
		}

		DirAccess voiceDir = DirAccess.Open(voiceDirectory);
		string[] files = voiceDir.GetFiles();
		foreach (string file in files)
		{
			if (!file.Contains("import"))
			{
				AudioStream stream = (AudioStream)GD.Load(voiceDirectory + file);
				VoiceStreams.Add(stream);
			}
		}
	}

	void RandomizePitch()
	{
		float newPitch = GD.Randf() * (MAX_PITCH - MIN_PITCH) + MIN_PITCH;
		VoicePlayer.PitchScale = newPitch;
	}

	public void SetVolume(float fraction)
	{
		VoicePlayer.VolumeDb = ((1 - fraction) * VOLUME_RANGE) - 80;
	}

	public void SetPlaying(bool playing)
	{
		if (playing == true && !Playing)
		{
			PlayVoice();
		}

		Playing = playing;
		if (!Playing)
		{
			SetVolume(0);
		}
	}

	public async void PlayVoice()
	{
		for (; ; )
		{
			RandomizePitch();
			VoicePlayer.Stream = GetRandomSound();
			VoicePlayer.Play();
			await ToSignal(VoicePlayer, "finished");
			float CoolDownTime = GD.Randf() * 0.1f;
			VoiceCoolDown.WaitTime = CoolDownTime;
			VoiceCoolDown.Start();
			await ToSignal(VoiceCoolDown, "timeout");
			if (!Playing)
				break;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
