using Godot;

public partial class SimpleMusicManager : Node
{
[Export] private AudioStreamWav HelloSound;
	[Export] private AudioStreamWav MenuMusic;

	private AudioStreamPlayer _helloPlayer;
	private AudioStreamPlayer _menuPlayer;

	public override void _Ready()
	{
		// Audio pro hello
		_helloPlayer = new AudioStreamPlayer();
		_helloPlayer.Stream = HelloSound;
		AddChild(_helloPlayer);

		// Audio pro menu hudbu
		_menuPlayer = new AudioStreamPlayer();
		if (MenuMusic != null)
		{
			// Nastavení smyčky přímo na streamu
			MenuMusic.LoopMode = AudioStreamWav.LoopModeEnum.Forward;
			_menuPlayer.Stream = MenuMusic;
		}
		AddChild(_menuPlayer);

		// Finished signál pro hello
		_helloPlayer.Finished += OnHelloFinished;

		// Spusť hello
		_helloPlayer.Play();
		GD.Print("Přehrávám hello...");
	}

	private void OnHelloFinished()
	{
		GD.Print("Hello skončilo, start menu hudby...");
		_menuPlayer.Play();
	}
}
