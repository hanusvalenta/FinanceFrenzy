using Godot;

public partial class BottomLeftAnimatedSprite : AnimatedSprite2D
{
	public override void _Ready()
	{
		// Připojíme se k signálům změny animace/snímku
		Connect(AnimatedSprite2D.SignalName.FrameChanged, new Callable(this, nameof(UpdateOffset)));
		Connect(AnimatedSprite2D.SignalName.AnimationChanged, new Callable(this, nameof(UpdateOffset)));
		
		// Spustíme animaci automaticky
		Play();
		
		// Nastavíme počáteční offset
		UpdateOffset();
	}

	private void UpdateOffset()
	{
		if (SpriteFrames == null) return;
		
		var texture = SpriteFrames.GetFrameTexture(Animation, Frame);
		if (texture == null) return;
		
		var size = texture.GetSize();
		
		// Offset pro levý spodní roh
		// X: -polovina šířky (posune levý okraj na střed)
		// Y: polovina výšky (posune spodní okraj na střed)
		Offset = new Vector2(-size.X / 2, size.Y / 2);
	}
}
