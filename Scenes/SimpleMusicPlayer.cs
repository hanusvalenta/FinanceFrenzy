using Godot;

public partial class SimpleMusicPlayer : Node
{
	[Export] public AudioStream FirstAudio { get; set; }  // První zvuk
	[Export] public AudioStream SecondAudio { get; set; } // Druhý zvuk (menu music)
	[Export] public float Volume { get; set; } = 0.0f;    // Hlasitost pro oba
	[Export] public bool LoopSecond { get; set; } = true; // Opakuj druhý zvuk
	
	private AudioStreamPlayer _player;
	
	public override void _Ready()
	{
		// Vytvoř audio player
		_player = new AudioStreamPlayer();
		AddChild(_player);
		_player.VolumeDb = Volume;
		
		// Připoj signál pro dokončení
		_player.Finished += OnFinished;
		
		// Spusť první audio
		PlayFirst();
	}
	
	private void PlayFirst()
	{
		if (FirstAudio != null)
		{
			_player.Stream = FirstAudio;
			_player.Play();
		}
		else
		{
			PlaySecond(); // Pokud není první, jdi na druhý
		}
	}
	
	private void PlaySecond()
	{
		if (SecondAudio != null)
		{
			_player.Stream = SecondAudio;
			
			// Nastav loop
			if (_player.Stream is AudioStreamOggVorbis ogg)
				ogg.Loop = LoopSecond;
			else if (_player.Stream is AudioStreamWav wav)
				wav.LoopMode = LoopSecond ? AudioStreamWav.LoopModeEnum.Forward : AudioStreamWav.LoopModeEnum.Disabled;
			
			_player.Play();
		}
	}
	
	private void OnFinished()
	{
		// Když první skončí, spusť druhý
		if (_player.Stream == FirstAudio)
		{
			PlaySecond();
		}
	}
}
