using Godot;

public partial class ButtonScale : TextureButton
{
	[Export] public float HoverScaleMultiplier { get; set; } = 1.1f;
	[Export] public float ClickScaleMultiplier { get; set; } = 0.95f;
	[Export] public float AnimationSpeed { get; set; } = 8f;
	[Export] public float FadeDuration { get; set; } = 0.2f; // délka fade v sekundách
	[Export] public float HoverImageOpacity { get; set; } = 0.5f; // opacity při hoveru
	[Export] public bool StayVisibleAfterClick { get; set; } = true; // zůstane viditelný po kliknutí
	[Export] public float ClickImageOpacity { get; set; } = 1.0f; // opacity po kliknutí
	
	// Obrázek, který se zobrazí při hoveru
	[Export] public NodePath HoverImagePath { get; set; }
	
	private Vector2 _originalScale;
	private Vector2 _targetScale;
	private CanvasItem _hoverImage;
	private bool _wasClicked = false; // sleduje jestli byl button kliknut
	
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
		
		// Pokud už byl kliknut a má zůstat viditelný, neměň opacity
		if (_wasClicked && StayVisibleAfterClick)
			return;
			
		FadeImage(HoverImageOpacity); // fade to custom opacity value
	}
	
	private void OnMouseExited()
	{
		_targetScale = _originalScale;
		
		// Pokud byl kliknut a má zůstat viditelný, neubírej opacity
		if (_wasClicked && StayVisibleAfterClick)
			return;
			
		FadeImage(0.0f); // fade back to transparent
	}
	
	private void OnPressed()
	{
		_targetScale = _originalScale * ClickScaleMultiplier;
		
		// Označ že byl kliknut a nastav opacity
		if (StayVisibleAfterClick)
		{
			_wasClicked = true;
			FadeImage(ClickImageOpacity); // nastav na click opacity
		}
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
	
	// Veřejné metody pro externí ovládání
	public void SetHoverImageVisible(bool visible, float opacity = 1.0f)
	{
		if (_hoverImage == null)
			return;
			
		if (visible)
		{
			FadeImage(opacity);
		}
		else
		{
			FadeImage(0.0f);
		}
	}
	
	public void SetHoverImagePermanentlyVisible(float opacity = 1.0f)
	{
		if (_hoverImage == null)
			return;
			
		// Odpoj mouse události aby se hover image neschovával
		MouseEntered -= OnMouseEntered;
		MouseExited -= OnMouseExited;
		
		// Nastav na požadovanou opacity
		FadeImage(opacity);
	}
	
	public void RestoreHoverBehavior()
	{
		// Obnoví normální hover chování
		MouseEntered -= OnMouseEntered; // odpoj pro jistotu
		MouseExited -= OnMouseExited;
		
		MouseEntered += OnMouseEntered; // připoj znovu
		MouseExited += OnMouseExited;
		
		// Reset click state
		_wasClicked = false;
		
		// Skryj obrázek
		FadeImage(0.0f);
	}
	
	// Nové metody pro ovládání click stavu
	public void ResetClickState()
	{
		_wasClicked = false;
		
		// Pokud není myš nad buttonem, skryj image
		if (!GetRect().HasPoint(GetLocalMousePosition()))
		{
			FadeImage(0.0f);
		}
	}
	
	public void ForceVisible(float opacity = 1.0f)
	{
		_wasClicked = true;
		FadeImage(opacity);
	}
	
	public bool WasClicked => _wasClicked;
}
