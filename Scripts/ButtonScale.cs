using Godot;

public partial class ButtonScale : TextureButton
{
	[Export] public float HoverScaleMultiplier { get; set; } = 1.1f;
	[Export] public float ClickScaleMultiplier { get; set; } = 0.95f;
	[Export] public float AnimationSpeed { get; set; } = 8f;
	[Export] public float FadeDuration { get; set; } = 0.2f; // délka fade v sekundách
	[Export] public float HoverImageOpacity { get; set; } = 0.5f; // opacity při hoveru
	
	// Obrázek, který se zobrazí při hoveru
	[Export] public NodePath HoverImagePath { get; set; }
	
	private Vector2 _originalScale;
	private Vector2 _targetScale;
	private CanvasItem _hoverImage;
	
	public override void _Ready()
	{
		_originalScale = Scale;
		_targetScale = Scale;
		
		if (HoverImagePath != null && !HoverImagePath.IsEmpty)
		{
			_hoverImage = GetNode<CanvasItem>(HoverImagePath);
			_hoverImage.Modulate = new Color(1, 1, 1, 0); // start completely transparent
		}
		
		// Signály tlačítka
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
		FadeImage(HoverImageOpacity); // fade to custom opacity value
	}
	
	private void OnMouseExited()
	{
		_targetScale = _originalScale;
		FadeImage(0.0f); // fade back to transparent
	}
	
	private void OnPressed()
	{
		_targetScale = _originalScale * ClickScaleMultiplier;
	}
	
	private void OnButtonUp()
	{
		if (GetRect().HasPoint(GetLocalMousePosition()))
			_targetScale = _originalScale * HoverScaleMultiplier;
		else
			_targetScale = _originalScale;
	}
	
	private void FadeImage(float targetAlpha)
	{
		if (_hoverImage == null)
			return;
			
		// Vytvoří nový Tween pro plynulý fade
		var tween = CreateTween();
		tween.TweenProperty(_hoverImage, "modulate:a", targetAlpha, FadeDuration)
			 .SetTrans(Tween.TransitionType.Sine)
			 .SetEase(Tween.EaseType.InOut);
	}
}
