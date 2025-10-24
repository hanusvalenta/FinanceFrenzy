using Godot;

public partial class ButtonScale : TextureButton
{
	[Export] public float HoverScaleMultiplier { get; set; } = 1.1f;
	[Export] public float ClickScaleMultiplier { get; set; } = 0.95f;
	[Export] public float AnimationSpeed { get; set; } = 8f;

	[Export] public NodePath HoverImagePath { get; set; }
	[Export] public float FadeDuration { get; set; } = 0.2f; // délka fade v sekundách

	private Vector2 _originalScale;
	private Vector2 _targetScale;
	private CanvasItem _hoverImage;
	private Tween _tween;

	public override void _Ready()
	{
		_originalScale = Scale;
		_targetScale = Scale;

		if (HoverImagePath != null && !HoverImagePath.IsEmpty)
		{
			_hoverImage = GetNode<CanvasItem>(HoverImagePath);
			_hoverImage.Modulate = new Color(1, 1, 1, 0); // start transparent
		}

		// Tween pro plynulý fade
		_tween = new Tween();
		AddChild(_tween);

		// Signály
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
		Pressed += OnPressed;
		ButtonUp += OnButtonUp;
	}

	public override void _Process(double delta)
	{
		Scale = Scale.Lerp(_targetScale, (float)(delta * AnimationSpeed));
	}

	private void OnMouseEntered()
	{
		_targetScale = _originalScale * HoverScaleMultiplier;
		FadeImage(1.0f); // fade in
	}

	private void OnMouseExited()
	{
		_targetScale = _originalScale;
		FadeImage(0.0f); // fade out
	}

	private void OnPressed()
	{
		_targetScale = _originalScale * ClickScaleMultiplier;
	}

	private void OnButtonUp()
	{
		_targetScale = GetRect().HasPoint(GetLocalMousePosition())
			? _originalScale * HoverScaleMultiplier
			: _originalScale;
	}

	private void FadeImage(float targetAlpha)
	{
		if (_hoverImage == null || _tween == null) return;

		// Zruší probíhající tweeny
		_tween.Kill();

		// Spustí nový tween pro alpha
		_tween.TweenProperty(_hoverImage, "modulate:a", targetAlpha, FadeDuration)
			  .SetTrans(Tween.TransitionType.Sine)
			  .SetEase(Tween.EaseType.InOut);
	}
}
