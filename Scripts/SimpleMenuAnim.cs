using Godot;

public partial class SimpleMenuAnim : Node
{
	[Export] public Texture2D[] Frames { get; set; } = new Texture2D[0];
	[Export] public float Speed { get; set; } = 12.0f; // FPS
	[Export] public bool PlayInBackground { get; set; } = false; // loop automaticky na pozadí
	[Export] public bool PlayOnClick { get; set; } = true;
	[Export] public bool ReverseAnimation { get; set; } = false; // přehrávání pozpátku
	[Export] public NodePath TargetSpritePath { get; set; } // Cesta k Sprite2D kde se má animace přehrát
	
	[ExportGroup("ButtonScale Integration")]
	[Export] public bool ControlButtonScale { get; set; } = false; // zapne komunikaci s ButtonScale
	[Export] public NodePath ButtonScalePath { get; set; } // Cesta k ButtonScale komponentu
	[Export] public float PermanentOpacity { get; set; } = 1.0f; // opacity pro trvalé zobrazení
	[Export] public bool ShowOnAnimationStart { get; set; } = true; // zobrazí při spuštění animace
	[Export] public bool ShowOnAnimationEnd { get; set; } = false; // zobrazí po skončení animace
	
	private int _frame = 0;
	private float _timer = 0.0f;
	private bool _playing = false;
	private Node _targetSprite;
	private BaseButton _button;
	private Node _buttonScale; // Reference na ButtonScale komponent
	
	public override void _Ready()
	{
		// Najdi target sprite - buď z cesty nebo parent
		if (TargetSpritePath != null && !TargetSpritePath.IsEmpty)
		{
			_targetSprite = GetNode(TargetSpritePath);
		}
		else
		{
			_targetSprite = GetParent(); // default: parent node
		}
		
		// Najdi ButtonScale komponent
		if (ControlButtonScale && ButtonScalePath != null && !ButtonScalePath.IsEmpty)
		{
			_buttonScale = GetNode(ButtonScalePath);
		}
		
		// Najdi button pro klikání
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
		
		// Připoj klik
		if (_button != null && PlayOnClick)
		{
			_button.Pressed += () => Play();
		}
		
		// Nastav první snímek podle směru
		if (Frames.Length > 0)
		{
			int startFrame = ReverseAnimation ? Frames.Length - 1 : 0;
			ShowFrame(startFrame);
		}
		
		// Auto spuštění na pozadí
		if (PlayInBackground)
		{
			Play();
		}
	}
	
	public override void _Process(double delta)
	{
		if (!_playing || Frames.Length == 0)
			return;
			
		_timer += (float)delta;
		
		if (_timer >= 1.0f / Speed)
		{
			_timer = 0.0f;
			
			// Pohyb snímků podle směru
			if (ReverseAnimation)
			{
				_frame--;
				
				if (_frame < 0)
				{
					if (PlayInBackground)
					{
						_frame = Frames.Length - 1; // loop zpět na konec
					}
					else
					{
						_frame = 0; // zastav na prvním snímku
						_playing = false;
						OnAnimationFinished();
						return;
					}
				}
			}
			else
			{
				_frame++;
				
				if (_frame >= Frames.Length)
				{
					if (PlayInBackground)
					{
						_frame = 0; // loop zpět na začátek
					}
					else
					{
						_frame = Frames.Length - 1; // zastav na posledním snímku
						_playing = false;
						OnAnimationFinished();
						return;
					}
				}
			}
			
			ShowFrame(_frame);
		}
	}
	
	public void Play()
	{
		// Nastav startovací snímek podle směru
		_frame = ReverseAnimation ? Frames.Length - 1 : 0;
		_timer = 0.0f;
		_playing = true;
		ShowFrame(_frame);
		OnAnimationStarted();
	}
	
	public void Stop()
	{
		_playing = false;
		// Resetuj na začátek podle směru
		_frame = ReverseAnimation ? Frames.Length - 1 : 0;
		ShowFrame(_frame);
	}
	
	private void ShowFrame(int index)
	{
		if (index >= Frames.Length || _targetSprite == null) 
			return;
		
		// Podporuje různé typy node
		if (_targetSprite is Sprite2D sprite2D)
			sprite2D.Texture = Frames[index];
		else if (_targetSprite is TextureRect rect)
			rect.Texture = Frames[index];
		else if (_targetSprite is TextureButton btn)
			btn.TextureNormal = Frames[index];
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
		if (ControlButtonScale && ShowOnAnimationEnd && _buttonScale != null)
		{
			// Zavolej metodu na ButtonScale pro trvalé zobrazení
			_buttonScale.Call("SetHoverImagePermanentlyVisible", PermanentOpacity);
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
	}
	
	public void ToggleDirection()
	{
		ReverseAnimation = !ReverseAnimation;
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
			_button.Pressed -= () => Play();
		}
	}
}
