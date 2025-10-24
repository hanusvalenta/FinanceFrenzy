using Godot;

public partial class Scrollingbackground : Sprite2D
{
	[Export] public float ScrollSpeedX { get; set; } = 50.0f; // Pixely za sekundu doprava
	[Export] public float ScrollSpeedY { get; set; } = 30.0f; // Pixely za sekundu nahoru
	[Export] public bool EnableScrolling { get; set; } = true;
	
	// Tiling options
	[Export] public bool AutoTile { get; set; } = true;
	[Export] public Vector2 TileSize { get; set; } = new Vector2(1000, 1000); // Velikost pro auto-tiling
	
	// Visual effects
	[Export] public float OpacityMultiplier { get; set; } = 1.0f;
	[Export] public bool EnableWaveEffect { get; set; } = false;
	[Export] public float WaveAmplitude { get; set; } = 10.0f;
	[Export] public float WaveSpeed { get; set; } = 2.0f;
	
	private Vector2 _currentOffset = Vector2.Zero;
	private Vector2 _originalScale;
	private float _waveTime = 0.0f;
	
	public override void _Ready()
	{
		_originalScale = Scale;
		
		// Setup auto-tiling
		if (AutoTile && Texture != null)
		{
			SetupTiling();
		}
		
		// Set initial modulate
		Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, OpacityMultiplier);
	}
	
	public override void _Process(double delta)
	{
		if (!EnableScrolling)
			return;
			
		float deltaFloat = (float)delta;
		
		// Update scroll offset
		_currentOffset.X += ScrollSpeedX * deltaFloat;
		_currentOffset.Y -= ScrollSpeedY * deltaFloat; // Mínus pro pohyb nahoru
		
		// Apply texture offset for scrolling
		if (Texture != null)
		{
			// Calculate UV offset based on texture size
			Vector2 textureSize = Texture.GetSize();
			Vector2 uvOffset = new Vector2(
				_currentOffset.X / textureSize.X,
				_currentOffset.Y / textureSize.Y
			);
			
			// Loop the offset (0.0 to 1.0)
			uvOffset.X = uvOffset.X - Mathf.Floor(uvOffset.X);
			uvOffset.Y = uvOffset.Y - Mathf.Floor(uvOffset.Y);
			
			// Apply offset using material
			ApplyScrollOffset(uvOffset);
		}
		
		// Wave effect
		if (EnableWaveEffect)
		{
			_waveTime += deltaFloat * WaveSpeed;
			float wave = Mathf.Sin(_waveTime) * WaveAmplitude;
			Position = Position with { X = Position.X + wave - (wave * 0.5f) }; // Center the wave
		}
		
		// Update opacity
		var currentModulate = Modulate;
		Modulate = new Color(currentModulate.R, currentModulate.G, currentModulate.B, OpacityMultiplier);
	}
	
	private void SetupTiling()
	{
		if (Texture == null)
			return;
		
		Vector2 textureSize = Texture.GetSize();
		Vector2 scaleNeeded = TileSize / textureSize;
		
		// Set scale to tile the texture
		Scale = scaleNeeded;
		
		GD.Print($"ScrollingBackground: Tiled texture from {textureSize} to {TileSize} with scale {scaleNeeded}");
	}
	
	private void ApplyScrollOffset(Vector2 uvOffset)
	{
		// Create or update shader material for UV offset
		if (Material == null)
		{
			var shaderMaterial = new ShaderMaterial();
			var shader = GD.Load<Shader>("res://shaders/simple_scroll.gdshader");
			
			if (shader != null)
			{
				shaderMaterial.Shader = shader;
				Material = shaderMaterial;
			}
			else
			{
				// Fallback: use position-based scrolling
				Position = new Vector2(-uvOffset.X * 100, uvOffset.Y * 100);
			}
		}
		
		if (Material is ShaderMaterial shaderMat)
		{
			shaderMat.SetShaderParameter("scroll_offset", uvOffset);
		}
	}
	
	// Public methods for control
	public void StartScrolling()
	{
		EnableScrolling = true;
	}
	
	public void StopScrolling()
	{
		EnableScrolling = false;
	}
	
	public void ResetOffset()
	{
		_currentOffset = Vector2.Zero;
	}
	
	public void SetScrollSpeed(float speedX, float speedY)
	{
		ScrollSpeedX = speedX;
		ScrollSpeedY = speedY;
	}
	
	public void SetOpacity(float opacity)
	{
		OpacityMultiplier = Mathf.Clamp(opacity, 0.0f, 1.0f);
	}
}
