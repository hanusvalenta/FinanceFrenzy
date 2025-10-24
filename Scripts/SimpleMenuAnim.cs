using Godot;

public partial class SimpleMenuAnim : Node
{
	[Export] public string AnimationName { get; set; } = "default"; // Název animace v AnimatedSprite2D
	[Export] public bool PlayInBackground { get; set; } = false; // loop automaticky na pozadí
	[Export] public bool ReverseAnimation { get; set; } = false; // přehrávání pozpátku
	[Export] public NodePath TargetSpritePath { get; set; } // Cesta k AnimatedSprite2D kde se má animace přehrát
	
	[ExportGroup("ButtonScale Integration")]
	[Export] public bool ControlButtonScale { get; set; } = false; // zapne komunikaci s ButtonScale
	[Export] public NodePath ButtonScalePath { get; set; } // Cesta k ButtonScale komponentu
	[Export] public float PermanentOpacity { get; set; } = 1.0f; // opacity pro trvalé zobrazení
	[Export] public bool ShowOnAnimationStart { get; set; } = true; // zobrazí při spuštění animace
	[Export] public bool ShowOnAnimationEnd { get; set; } = false; // zobrazí po skončení animace
	
	private bool _isHovered = false;
	private bool _isPlayingFullAnimation = false;
	private AnimatedSprite2D _animatedSprite;
	private BaseButton _button;
	private Node _buttonScale; // Reference na ButtonScale komponent
	private bool _wasPlayingOriginally = false; // Zda byla animace původně spuštěná
	
	public override void _Ready()
	{
		// Najdi AnimatedSprite2D - buď z cesty nebo parent
		if (TargetSpritePath != null && !TargetSpritePath.IsEmpty)
		{
			_animatedSprite = GetNode<AnimatedSprite2D>(TargetSpritePath);
		}
		else
		{
			_animatedSprite = GetParent() as AnimatedSprite2D; // default: parent node
		}
		
		if (_animatedSprite == null)
		{
			GD.PrintErr("SimpleMenuAnim: AnimatedSprite2D not found!");
			return;
		}
		
		// Uložení původního stavu animace
		_wasPlayingOriginally = _animatedSprite.IsPlaying();
		
		// Najdi ButtonScale komponent
		if (ControlButtonScale && ButtonScalePath != null && !ButtonScalePath.IsEmpty)
		{
			_buttonScale = GetNode(ButtonScalePath);
		}
		
		// Najdi button pro klikání a hover
		_button = GetParent() as BaseButton;
		if (_button == null)
		{
			// Zkus najít button v parent's children
			var parent = GetParent();
			foreach (Node child in parent.GetChildren())
			{
				if (child is BaseButton btn)
				{
					_button = btn;
					break;
				}
			}
		}
		
		// Připoj signály pro hover a klik
		if (_button != null)
		{
			_button.Pressed += OnButtonPressed;
			_button.MouseEntered += OnButtonHoverEntered;
			_button.MouseExited += OnButtonHoverExited;
		}
		
		// Připoj signál pro konec animace
		if (_animatedSprite != null)
		{
			_animatedSprite.AnimationFinished += OnAnimationFinished;
		}
		
		// Nastav animaci a první snímek
		SetupAnimation();
		
		// Auto spuštění na pozadí
		if (PlayInBackground)
		{
			Play();
		}
	}
	
	private void SetupAnimation()
	{
		if (_animatedSprite == null || string.IsNullOrEmpty(AnimationName))
			return;
			
		// Nastav animaci
		_animatedSprite.Animation = AnimationName;
		
		// Zastav animaci a nastav na první snímek podle směru
		_animatedSprite.Stop();
		if (ReverseAnimation)
		{
			_animatedSprite.Frame = _animatedSprite.SpriteFrames.GetFrameCount(AnimationName) - 1;
		}
		else
		{
			_animatedSprite.Frame = 0;
		}
	}
	
	private void OnButtonPressed()
	{
		// Klik spustí celou animaci
		Play();
	}
	
	private void OnButtonHoverEntered()
	{
		_isHovered = true;
		
		// Pokud se nehraje celá animace, zobraz první snímek
		if (!_isPlayingFullAnimation && _animatedSprite != null)
		{
			_animatedSprite.Stop();
			if (ReverseAnimation)
			{
				_animatedSprite.Frame = _animatedSprite.SpriteFrames.GetFrameCount(AnimationName) - 1;
			}
			else
			{
				_animatedSprite.Frame = 0;
			}
		}
	}
	
	private void OnButtonHoverExited()
	{
		_isHovered = false;
		
		// Pokud se nehraje celá animace, vrať původní stav
		if (!_isPlayingFullAnimation && _animatedSprite != null)
		{
			RestoreOriginalState();
		}
	}
	
	public void Play()
	{
		if (_animatedSprite == null || string.IsNullOrEmpty(AnimationName))
			return;
			
		_isPlayingFullAnimation = true;
		_animatedSprite.Animation = AnimationName;
		
		// Nastav směr a spusť animaci
		if (ReverseAnimation)
		{
			_animatedSprite.PlayBackwards(AnimationName);
		}
		else
		{
			_animatedSprite.Play(AnimationName);
		}
		
		OnAnimationStarted();
	}
	
	public void Stop()
	{
		if (_animatedSprite == null)
			return;
			
		_isPlayingFullAnimation = false;
		_animatedSprite.Stop();
		
		// Pokud není hover, vrať původní stav, jinak zobraz první snímek
		if (_isHovered)
		{
			if (ReverseAnimation)
			{
				_animatedSprite.Frame = _animatedSprite.SpriteFrames.GetFrameCount(AnimationName) - 1;
			}
			else
			{
				_animatedSprite.Frame = 0;
			}
		}
		else
		{
			RestoreOriginalState();
		}
	}
	
	private void RestoreOriginalState()
	{
		if (_animatedSprite == null)
			return;
		
		// Vrať původní stav animace
		if (_wasPlayingOriginally && PlayInBackground)
		{
			_animatedSprite.Play(AnimationName);
		}
		else
		{
			_animatedSprite.Stop();
			// Nastav na začátek podle směru
			if (ReverseAnimation)
			{
				_animatedSprite.Frame = _animatedSprite.SpriteFrames.GetFrameCount(AnimationName) - 1;
			}
			else
			{
				_animatedSprite.Frame = 0;
			}
		}
	}
	
	private void OnAnimationStarted()
	{
		if (ControlButtonScale && ShowOnAnimationStart && _buttonScale != null)
		{
			// Zavolej metodu na ButtonScale pro trvalé zobrazení
			_buttonScale.Call("SetHoverImagePermanentlyVisible", PermanentOpacity);
		}
	}
	
	private void OnAnimationFinished()
	{
		_isPlayingFullAnimation = false;
		
		if (ControlButtonScale && ShowOnAnimationEnd && _buttonScale != null)
		{
			// Zavolej metodu na ButtonScale pro trvalé zobrazení
			_buttonScale.Call("SetHoverImagePermanentlyVisible", PermanentOpacity);
		}
		
		// Po skončení animace zkontroluj hover stav
		if (_isHovered)
		{
			// Pokud je stále hover, zobraz první snímek
			if (ReverseAnimation)
			{
				_animatedSprite.Frame = _animatedSprite.SpriteFrames.GetFrameCount(AnimationName) - 1;
			}
			else
			{
				_animatedSprite.Frame = 0;
			}
		}
		else
		{
			// Jinak vrať původní stav
			RestoreOriginalState();
		}
	}
	
	// Veřejné metody pro ruční ovládání ButtonScale
	public void MakeButtonScaleImageVisible()
	{
		if (_buttonScale != null)
		{
			_buttonScale.Call("SetHoverImagePermanentlyVisible", PermanentOpacity);
		}
	}
	
	public void RestoreButtonScaleHover()
	{
		if (_buttonScale != null)
		{
			_buttonScale.Call("RestoreHoverBehavior");
		}
	}
	
	// Metody pro ovládání směru animace
	public void SetReverse(bool reverse)
	{
		ReverseAnimation = reverse;
		SetupAnimation(); // Nastav správný snímek podle nového směru
	}
	
	public void ToggleDirection()
	{
		ReverseAnimation = !ReverseAnimation;
		SetupAnimation(); // Nastav správný snímek podle nového směru
	}
	
	public void PlayForward()
	{
		ReverseAnimation = false;
		Play();
	}
	
	public void PlayReverse()
	{
		ReverseAnimation = true;
		Play();
	}
	
	public override void _ExitTree()
	{
		// Odpoj signály
		if (_button != null)
		{
			_button.Pressed -= OnButtonPressed;
			_button.MouseEntered -= OnButtonHoverEntered;
			_button.MouseExited -= OnButtonHoverExited;
		}
		
		if (_animatedSprite != null)
		{
			_animatedSprite.AnimationFinished -= OnAnimationFinished;
		}
	}
}
