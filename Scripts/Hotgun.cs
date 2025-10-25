using Godot;
using System;

public partial class Hotgun : Sprite2D
{
	[Export] private Sprite2D _sprite;
	[Export(PropertyHint.File, "*.png,*.jpg,*.tres")] private Texture2D _oneAmmoTexture;
	[Export(PropertyHint.File, "*.png,*.jpg,*.tres")] private Texture2D _twoAmmoTexture;
	[Export] private Sprite2D _delayedSprite;
	[Export] private AnimationManager _delayedSprite2;
	[Export] private AnimatedSprite2D _animationSprite;
	
	// 🔊 DIRECT AUDIO FILE EXPORTS
	[Export(PropertyHint.File, "*.wav,*.ogg,*.mp3")] private AudioStream _firstSpriteAudio; // Audio soubor pro první sprite
	[Export(PropertyHint.File, "*.wav,*.ogg,*.mp3")] private AudioStream _secondSpriteAudio; // Audio soubor pro druhý sprite
	[Export] private float _audioVolume = 0.0f; // Volume v dB (0 = default, záporné = tišší)
	
	private int _ammoCount = 0;
	private AudioStreamPlayer _firstAudioPlayer; // Dedicated player pro první audio
	private AudioStreamPlayer _secondAudioPlayer; // Dedicated player pro druhé audio

	public override void _Ready()
	{
		// Vytvořit dva AudioStreamPlayer pro nezávislé přehrávání
		_firstAudioPlayer = new AudioStreamPlayer();
		_secondAudioPlayer = new AudioStreamPlayer();
		
		// Nastavit volume
		_firstAudioPlayer.VolumeDb = _audioVolume;
		_secondAudioPlayer.VolumeDb = _audioVolume;
		
		// Přidat jako child nodes
		AddChild(_firstAudioPlayer);
		AddChild(_secondAudioPlayer);
		
		// Nastavit audio streams pokud jsou přiřazeny
		if (_firstSpriteAudio != null)
			_firstAudioPlayer.Stream = _firstSpriteAudio;
		if (_secondSpriteAudio != null)
			_secondAudioPlayer.Stream = _secondSpriteAudio;
			
		GD.Print("🎵 Audio playery inicializovány");
	}

	private void _OnHole1BodyEntered(Node2D body) { LoadAmmo(body); }
	private void _OnHole2BodyEntered(Node2D body) { LoadAmmo(body); }

	private void LoadAmmo(Node2D body)
	{
		if (_ammoCount >= 2) return;
		_ammoCount++;

		if (_ammoCount == 1)
		{
			_sprite.Texture = _oneAmmoTexture;
		}
		else if (_ammoCount >= 2)
		{
			_sprite.Texture = _twoAmmoTexture;
			StartDelayedSpriteActivation();
		}

		// Remove ammo logic
		if (body != null)
		{
			Node ammoToDelete = body.GetParent();
			if (ammoToDelete != null && ammoToDelete != this && ammoToDelete != GetParent())
			{
				GD.Print($"Deleting ammo: {ammoToDelete.Name}");
				ammoToDelete.QueueFree();
			}
			else
			{
				GD.Print($"Deleting body instead: {body.Name}");
				body.QueueFree();
			}
		}

		F_ForceDrop_RNil();
	}

	private async void StartDelayedSpriteActivation()
	{
		// ⏰ Čekání 1 sekunda
		await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);
		
		// 👁️ První sprite viditelný + 🔊 PŘEHRÁT PRVNÍ AUDIO
		if (_delayedSprite != null)
		{
			_delayedSprite.Visible = true;
			GD.Print("First delayed sprite is now visible!");
			
			// Přehrát první audio
			PlayFirstAudio();
		}
		
		// 🎬 Spustit animaci
		if (_animationSprite != null)
		{
			_animationSprite.Visible = true;
			_animationSprite.Play();
			GD.Print("Animation started and looping!");
		}

		// ⏰ Čekání dalších 2 sekundy
		await ToSignal(GetTree().CreateTimer(2.0), SceneTreeTimer.SignalName.Timeout);
		
		// 👁️ Druhý sprite viditelný + 🔊 PŘEHRÁT DRUHÉ AUDIO
		if (_delayedSprite2 != null)
		{
			_delayedSprite2.Visible = true;
			GD.Print("Second delayed sprite is now visible!");
			
			// Přehrát druhé audio
			PlaySecondAudio();
		}
	}

	public void PlayFirstAudio()
	{
		if (_firstAudioPlayer != null && _firstSpriteAudio != null)
		{
			_firstAudioPlayer.Play();
			GD.Print("🔊 První sprite audio přehráno!");
		}
		else
		{
			if (_firstSpriteAudio == null)
				GD.PrintErr("❌ První audio soubor není přiřazen v inspektoru!");
			if (_firstAudioPlayer == null)
				GD.PrintErr("❌ První AudioPlayer není inicializován!");
		}
	}

	public void PlaySecondAudio()
	{
		if (_secondAudioPlayer != null && _secondSpriteAudio != null)
		{
			_secondAudioPlayer.Play();
			GD.Print("🔊 Druhé sprite audio přehráno!");
		}
		else
		{
			if (_secondSpriteAudio == null)
				GD.PrintErr("❌ Druhé audio soubor není přiřazen v inspektoru!");
			if (_secondAudioPlayer == null)
				GD.PrintErr("❌ Druhý AudioPlayer není inicializován!");
		}
	}

	private void F_ForceDrop_RNil()
	{
		GetNode<Hand>("../Hand").F_DropObject_RNil();
	}

	public void ResetGun()
	{
		_ammoCount = 0;
		_sprite.Texture = Texture;
		
		// Skrýt sprite
		if (_delayedSprite != null) _delayedSprite.Visible = false;
		if (_delayedSprite2 != null) _delayedSprite2.Visible = false;
		
		// Zastavit animaci
		if (_animationSprite != null)
		{
			_animationSprite.Stop();
			_animationSprite.Visible = false;
		}
		
		// 🔇 Zastavit všechna audio
		StopAllAudio();
	}

	public void StopAllAudio()
	{
		if (_firstAudioPlayer != null && _firstAudioPlayer.Playing)
		{
			_firstAudioPlayer.Stop();
			GD.Print("🔇 První audio zastaveno");
		}
		
		if (_secondAudioPlayer != null && _secondAudioPlayer.Playing)
		{
			_secondAudioPlayer.Stop();
			GD.Print("🔇 Druhé audio zastaveno");
		}
	}

	// Utility metody
	public bool IsFullyLoaded() => _ammoCount >= 2;
	public int GetAmmoCount() => _ammoCount;
	
	public bool IsFirstAudioPlaying() => _firstAudioPlayer != null && _firstAudioPlayer.Playing;
	public bool IsSecondAudioPlaying() => _secondAudioPlayer != null && _secondAudioPlayer.Playing;
	public bool IsAnyAudioPlaying() => IsFirstAudioPlaying() || IsSecondAudioPlaying();

	// Nastavení volume za běhu
	public void SetAudioVolume(float volumeDb)
	{
		_audioVolume = volumeDb;
		if (_firstAudioPlayer != null) _firstAudioPlayer.VolumeDb = volumeDb;
		if (_secondAudioPlayer != null) _secondAudioPlayer.VolumeDb = volumeDb;
		GD.Print($"🔊 Audio volume nastaveno na: {volumeDb} dB");
	}
}
