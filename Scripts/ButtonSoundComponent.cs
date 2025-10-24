using Godot;

public partial class ButtonSoundComponent : Node
{
	[Export] public AudioStream HoverSound { get; set; }
	[Export] public AudioStream ClickSound { get; set; }
	[Export] public float HoverVolume { get; set; } = 0.0f; // v dB
	[Export] public float ClickVolume { get; set; } = 0.0f; // v dB
	[Export] public float HoverPitch { get; set; } = 1.0f;
	[Export] public float ClickPitch { get; set; } = 1.0f;
	[Export] public NodePath ButtonPath { get; set; }
	
	private AudioStreamPlayer _audioPlayer;
	private BaseButton _button;
	
	public override void _Ready()
	{
		// Vytvoř AudioStreamPlayer
		_audioPlayer = new AudioStreamPlayer();
		AddChild(_audioPlayer);
		
		// Najdi button - buď z cesty nebo jako parent
		if (ButtonPath != null && !ButtonPath.IsEmpty)
		{
			_button = GetNode<BaseButton>(ButtonPath);
		}
		else
		{
			_button = GetParent<BaseButton>();
		}
		
		if (_button != null)
		{
			// Připoj signály
			_button.MouseEntered += OnMouseEntered;
			_button.Pressed += OnPressed;
		}
		else
		{
			GD.PrintErr("ButtonSoundComponent: Nepodařilo se najít button!");
		}
	}
	
	private void OnMouseEntered()
	{
		PlayHoverSound();
	}
	
	private void OnPressed()
	{
		PlayClickSound();
	}
	
	private void PlayHoverSound()
	{
		if (HoverSound != null)
		{
			_audioPlayer.Stream = HoverSound;
			_audioPlayer.VolumeDb = HoverVolume;
			_audioPlayer.PitchScale = HoverPitch;
			_audioPlayer.Play();
		}
	}
	
	private void PlayClickSound()
	{
		if (ClickSound != null)
		{
			_audioPlayer.Stream = ClickSound;
			_audioPlayer.VolumeDb = ClickVolume;
			_audioPlayer.PitchScale = ClickPitch;
			_audioPlayer.Play();
		}
	}
	
	public override void _ExitTree()
	{
		// Odpoj signály při odstranění
		if (_button != null)
		{
			_button.MouseEntered -= OnMouseEntered;
			_button.Pressed -= OnPressed;
		}
	}
}
