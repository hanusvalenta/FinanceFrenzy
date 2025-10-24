using Godot;

public partial class SimpleTileBackground : Node2D
{
	[Export] public Texture2D TileTexture { get; set; } // Textura malého sprite
	[Export] public float ScrollSpeedX { get; set; } = 50.0f; // Rychlost doprava
	[Export] public float ScrollSpeedY { get; set; } = 30.0f; // Rychlost nahoru
	[Export] public Vector2 TileSize { get; set; } = new Vector2(64, 64); // Velikost jednoho tile
	
	[ExportGroup("Opacity Control")]
	[Export] public float DollarOpacity { get; set; } = 1.0f; // Opacity pro dolary (0.0 - 1.0)
	[Export] public bool AnimateOpacityChanges { get; set; } = true; // Animované změny
	[Export] public float OpacityAnimationSpeed { get; set; } = 2.0f; // Rychlost animace
	[Export] public bool PulseEffect { get; set; } = false; // Pulzující efekt
	[Export] public float PulseSpeed { get; set; } = 3.0f; // Rychlost pulzování
	[Export] public float PulseRange { get; set; } = 0.3f; // Rozsah pulzování (0.0-1.0)
	
	private Node2D _container;
	private Vector2 _offset = Vector2.Zero;
	private int _tilesX = 35; // Dostatek tiles pro 1920px + extra
	private int _tilesY = 20; // Dostatek tiles pro 1080px + extra
	private float _currentOpacity = 1.0f;
	private float _pulseTime = 0.0f;
	
	public override void _Ready()
	{
		// Vytvoř kontejner
		_container = new Node2D();
		AddChild(_container);
		
		// Nastav počáteční opacity
		_currentOpacity = DollarOpacity;
		
		// Vytvoř mřížku tiles
		CreateTileGrid();
	}
	
	private void CreateTileGrid()
	{
		if (TileTexture == null)
		{
			GD.Print("SimpleTileBackground: No texture set!");
			return;
		}
		
		// Vytvoř mřížku sprites
		for (int x = 0; x < _tilesX; x++)
		{
			for (int y = 0; y < _tilesY; y++)
			{
				var sprite = new Sprite2D();
				sprite.Texture = TileTexture;
				sprite.Position = new Vector2(x * TileSize.X, y * TileSize.Y);
				
				// Přizpůsob velikost
				if (TileTexture != null)
				{
					Vector2 scale = TileSize / TileTexture.GetSize();
					sprite.Scale = scale;
				}
				
				// Nastav počáteční opacity
				sprite.Modulate = new Color(1, 1, 1, _currentOpacity);
				
				_container.AddChild(sprite);
			}
		}
		
		// Vycentruj
		_container.Position = -new Vector2(_tilesX * TileSize.X, _tilesY * TileSize.Y) * 0.5f;
	}
	
	public override void _Process(double delta)
	{
		float deltaFloat = (float)delta;
		
		// Scrolluj
		_offset.X += ScrollSpeedX * deltaFloat;
		_offset.Y -= ScrollSpeedY * deltaFloat;
		
		// Loop když se dostane moc daleko
		if (_offset.X >= TileSize.X) _offset.X -= TileSize.X;
		if (_offset.X < 0) _offset.X += TileSize.X;
		if (_offset.Y >= TileSize.Y) _offset.Y -= TileSize.Y;
		if (_offset.Y < 0) _offset.Y += TileSize.Y;
		
		// Aplikuj offset
		Vector2 basePos = -new Vector2(_tilesX * TileSize.X, _tilesY * TileSize.Y) * 0.5f;
		_container.Position = basePos + _offset;
		
		// Update opacity
		UpdateOpacity(deltaFloat);
	}
	
	private void UpdateOpacity(float delta)
	{
		float targetOpacity = DollarOpacity;
		
		// Pulse effect
		if (PulseEffect)
		{
			_pulseTime += delta * PulseSpeed;
			float pulseOffset = Mathf.Sin(_pulseTime) * PulseRange;
			targetOpacity = Mathf.Clamp(DollarOpacity + pulseOffset, 0.0f, 1.0f);
		}
		
		// Animate opacity changes
		if (AnimateOpacityChanges)
		{
			_currentOpacity = Mathf.Lerp(_currentOpacity, targetOpacity, delta * OpacityAnimationSpeed);
		}
		else
		{
			_currentOpacity = targetOpacity;
		}
		
		// Apply to all tiles
		foreach (Node child in _container.GetChildren())
		{
			if (child is Sprite2D sprite)
			{
				var currentColor = sprite.Modulate;
				sprite.Modulate = new Color(currentColor.R, currentColor.G, currentColor.B, _currentOpacity);
			}
		}
	}
	
	// Public methods for external control
	public void SetDollarOpacity(float opacity)
	{
		DollarOpacity = Mathf.Clamp(opacity, 0.0f, 1.0f);
	}
	
	public void FadeIn(float duration = 1.0f)
	{
		var tween = CreateTween();
		float startOpacity = DollarOpacity;
		tween.TweenMethod(Callable.From<float>(SetDollarOpacity), 0.0f, startOpacity, duration);
	}
	
	public void FadeOut(float duration = 1.0f)
	{
		var tween = CreateTween();
		float startOpacity = DollarOpacity;
		tween.TweenMethod(Callable.From<float>(SetDollarOpacity), startOpacity, 0.0f, duration);
	}
	
	public void SetPulse(bool enable, float speed = 3.0f, float range = 0.3f)
	{
		PulseEffect = enable;
		PulseSpeed = speed;
		PulseRange = range;
	}
	
	// Properties
	public float CurrentOpacity => _currentOpacity;
}
