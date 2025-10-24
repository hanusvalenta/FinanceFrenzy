using Godot;
using System;

public partial class Hotgun : Sprite2D
{
	[Export] private Sprite2D _sprite;
	[Export(PropertyHint.File, "*.png,*.jpg,*.tres")] private Texture2D _oneAmmoTexture; // Texture when one ammo is loaded (regardless of which hole)
	[Export(PropertyHint.File, "*.png,*.jpg,*.tres")] private Texture2D _twoAmmoTexture; // Texture when both ammo are loaded
	[Export] private Sprite2D _delayedSprite; // Sprite to make visible after 1 second delay
	[Export] private AnimatedSprite2D _animationSprite; // AnimatedSprite2D to start and loop after delay
	
	private int _ammoCount = 0; // Track how many ammo are loaded

	private void _OnHole1BodyEntered(Node2D body)
	{
		LoadAmmo(body);
	}

	private void _OnHole2BodyEntered(Node2D body)
	{
		LoadAmmo(body);
	}

	private void LoadAmmo(Node2D body)
	{
		// Only proceed if we haven't reached max ammo
		if (_ammoCount >= 2) return;

		// Increment ammo count
		_ammoCount++;

		// Update texture based on total ammo count
		if (_ammoCount == 1)
		{
			_sprite.Texture = _oneAmmoTexture;
		}
		else if (_ammoCount >= 2)
		{
			_sprite.Texture = _twoAmmoTexture;
			
			// Start the delay timer when both ammo are loaded
			StartDelayedSpriteActivation();
		}

		// Remove the ammo object
		if (body != null)
		{
			// The body is usually a CollisionShape2D/Area2D child of the actual ammo object
			Node ammoToDelete = body.GetParent();
			
			// Safety check: make sure we're not deleting the gun, hand, or main scene
			if (ammoToDelete != null && ammoToDelete != this && ammoToDelete != GetParent())
			{
				GD.Print($"Deleting ammo: {ammoToDelete.Name}"); // Debug line
				ammoToDelete.QueueFree();
			}
			else
			{
				// If parent deletion seems unsafe, try deleting the body itself
				GD.Print($"Deleting body instead: {body.Name}"); // Debug line
				body.QueueFree();
			}
		}

		// Force hand to drop
		F_ForceDrop_RNil();
	}

	private async void StartDelayedSpriteActivation()
	{
		// Wait for 1 second
		await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);
		
		// Turn on visibility for the delayed sprite
		if (_delayedSprite != null)
		{
			_delayedSprite.Visible = true;
			GD.Print("Delayed sprite is now visible!"); // Debug line
		}
		else
		{
			GD.PrintErr("DelayedSprite is not assigned in the inspector!");
		}
		
		// Start and loop the animation
		if (_animationSprite != null)
		{
			_animationSprite.Visible = true;
			_animationSprite.Play(); // Start the animation (will loop if set to loop in the animation)
			GD.Print("Animation started and looping!"); // Debug line
		}
		else
		{
			GD.PrintErr("AnimationSprite is not assigned in the inspector!");
		}
	}

	private void F_ForceDrop_RNil()
	{
		GetNode<Hand>("../Hand").F_DropObject_RNil();
	}

	// Optional: Method to reset the gun (remove all ammo)
	public void ResetGun()
	{
		_ammoCount = 0;
		_sprite.Texture = Texture; // Reset to original texture
		
		// Hide the delayed sprite when resetting
		if (_delayedSprite != null)
		{
			_delayedSprite.Visible = false;
		}
		
		// Stop and hide the animation sprite when resetting
		if (_animationSprite != null)
		{
			_animationSprite.Stop();
			_animationSprite.Visible = false;
		}
	}

	// Optional: Method to check if gun is fully loaded
	public bool IsFullyLoaded()
	{
		return _ammoCount >= 2;
	}

	// Optional: Method to get current ammo count
	public int GetAmmoCount()
	{
		return _ammoCount;
	}
}
