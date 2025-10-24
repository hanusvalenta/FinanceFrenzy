using Godot;

public partial class DualAnimatedSprites : Node2D
{
	[Export] private AnimatedSprite2D spriteA;
	[Export] private AnimatedSprite2D spriteB;

	public override void _Ready()
	{
		if (spriteA == null || spriteB == null)
		{
			GD.PrintErr("❌ Missing sprite references! Assign them in the Inspector.");
			return;
		}

		// Ensure their current animations loop
		if (spriteA.SpriteFrames != null && spriteA.Animation != "")
			spriteA.SpriteFrames.SetAnimationLoop(spriteA.Animation, true);

		if (spriteB.SpriteFrames != null && spriteB.Animation != "")
			spriteB.SpriteFrames.SetAnimationLoop(spriteB.Animation, true);

		// Play both animations
		spriteA.Play();
		spriteB.Play();

		GD.Print("✅ Both AnimatedSprite2D are now playing and looping.");
	}
}
