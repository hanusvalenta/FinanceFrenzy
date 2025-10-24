using Godot;

public partial class SimpleMusicManager : Node
{
[Export] private AudioStreamWav HelloSound;
	[Export] private AudioStreamWav MenuMusic;

	private AudioStreamPlayer _AudioPlayer;

	public override void _Ready()
	{
		// Audio pro hello
		_AudioPlayer		= new AudioStreamPlayer();
		_AudioPlayer.Stream = HelloSound;
		AddChild(_AudioPlayer);

		_AudioPlayer.Play();

		_AudioPlayer.Finished	+= () =>
		{
			_AudioPlayer.Stream	= MenuMusic;
			_AudioPlayer.Play();
		};
	}
}
