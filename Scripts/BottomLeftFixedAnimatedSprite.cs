using Godot;

public partial class BottomLeftFixedAnimatedSprite : AnimatedSprite2D
{
	[Export]
	public bool AutoPlay = true; // Automatically start animation
	
	[Export]
	public string DefaultAnimation = "default"; // Default animation to play
	
	public override void _Ready()
	{
		// Connect to frame and animation change signals
		Connect(AnimatedSprite2D.SignalName.FrameChanged, new Callable(this, nameof(OnFrameChanged)));
		Connect(AnimatedSprite2D.SignalName.AnimationChanged, new Callable(this, nameof(OnAnimationChanged)));
		
		// Set initial offset
		UpdateBottomLeftOffset();
		
		// Auto-play if enabled
		if (AutoPlay)
		{
			StartDefaultAnimation();
		}
	}

	private void OnFrameChanged()
	{
		UpdateBottomLeftOffset();
	}

	private void OnAnimationChanged()
	{
		UpdateBottomLeftOffset();
	}

	private void UpdateBottomLeftOffset()
	{
		// Skip if no sprite frames are available
		if (SpriteFrames == null)
			return;

		// Get current frame texture
		var texture = SpriteFrames.GetFrameTexture(Animation, Frame);
		if (texture == null)
			return;

		var textureSize = texture.GetSize();
		
		// Calculate offset to keep bottom-left corner fixed
		// X offset: negative half width (moves left edge to center)
		// Y offset: positive half height (moves bottom edge to center)
		var offsetX = -textureSize.X / 2.0f;
		var offsetY = textureSize.Y / 2.0f;
		
		Offset = new Vector2(offsetX, offsetY);
		
		// Debug output (remove in production if not needed)
		GD.Print($"Frame: {Frame}, Size: {textureSize}, Offset: {Offset}");
	}

	private void StartDefaultAnimation()
	{
		if (SpriteFrames == null)
			return;

		// Try to play specified default animation
		if (!string.IsNullOrEmpty(DefaultAnimation) && SpriteFrames.HasAnimation(DefaultAnimation))
		{
			Play(DefaultAnimation);
		}
		else
		{
			// Play any available animation
			var animations = SpriteFrames.GetAnimationNames();
			if (animations.Length > 0)
			{
				Play(animations[0]);
			}
		}
	}

	// Method to play specific animation while maintaining bottom-left positioning
	public void PlayAnimationBottomLeft(string animationName)
	{
		if (SpriteFrames != null && SpriteFrames.HasAnimation(animationName))
		{
			Play(animationName);
			// Offset will be automatically updated via signal
		}
		else
		{
			GD.PrintErr($"Animation '{animationName}' not found in SpriteFrames!");
		}
	}

	// Method to set position of bottom-left corner directly
	public void SetBottomLeftPosition(Vector2 bottomLeftPos)
	{
		// Calculate where the sprite center should be based on desired bottom-left position
		if (SpriteFrames == null)
		{
			Position = bottomLeftPos;
			return;
		}

		var texture = SpriteFrames.GetFrameTexture(Animation, Frame);
		if (texture == null)
		{
			Position = bottomLeftPos;
			return;
		}

		var textureSize = texture.GetSize();
		
		// Center position = bottom-left + half width right + half height up
		var centerX = bottomLeftPos.X + textureSize.X / 2.0f;
		var centerY = bottomLeftPos.Y - textureSize.Y / 2.0f;
		
		Position = new Vector2(centerX, centerY);
		UpdateBottomLeftOffset();
	}

	// Method to get current bottom-left position
	public Vector2 GetBottomLeftPosition()
	{
		if (SpriteFrames == null)
			return Position;

		var texture = SpriteFrames.GetFrameTexture(Animation, Frame);
		if (texture == null)
			return Position;

		var textureSize = texture.GetSize();
		
		// Bottom-left = center - half width left - half height down
		var bottomLeftX = Position.X - textureSize.X / 2.0f;
		var bottomLeftY = Position.Y + textureSize.Y / 2.0f;
		
		return new Vector2(bottomLeftX, bottomLeftY);
	}
}
