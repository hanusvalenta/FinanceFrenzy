using Godot;

public partial class ScaleOnVisible : Node2D
{
	[Export] private Node2D _nodeToScale; // Node that will be scaled
	[Export] private CanvasItem _nodeToWatch; // Node whose visibility we watch (CanvasItem has Visible property)
	[Export] private float _delayBeforeScale = 2.0f; // Delay in seconds before starting scale animation
	[Export] private float _scaleDuration = 0.8f; // Duration in seconds
	[Export] private Vector2 _targetScale = Vector2.One; // Target scale (1,1 = normal size)
	[Export] private Vector2 _startScale = Vector2.Zero; // Starting scale (0,0 = invisible)
	[Export] private Tween.TransitionType _transitionType = Tween.TransitionType.Back;
	[Export] private Tween.EaseType _easeType = Tween.EaseType.Out;
	[Export] private bool _resetScaleOnHide = true; // Reset scale when visibility becomes false
	[Export] private bool _debugMode = true; // Enable debug prints
	[Export] private bool _scaleOnlyOnce = false; // Scale only once, don't repeat
	
	private Tween _scaleTween;
	private bool _wasVisible = false; // Track previous visibility state
	private bool _hasScaledOnce = false; // Track if we've already scaled once
	
	public override void _Ready()
	{
		// Validate required nodes
		if (_nodeToScale == null)
		{
			GD.PrintErr($"FlexibleScaleOnVisible on {Name}: NodeToScale is not assigned!");
			return;
		}
		
		if (_nodeToWatch == null)
		{
			GD.PrintErr($"FlexibleScaleOnVisible on {Name}: NodeToWatch is not assigned!");
			return;
		}
		
		// Set initial scale
		_nodeToScale.Scale = _startScale;
		
		// Get initial visibility state
		_wasVisible = _nodeToWatch.Visible;
		
		if (_debugMode)
		{
			GD.Print($"🎯 FlexibleScaleOnVisible ready:");
			GD.Print($"  - Watching: {_nodeToWatch.Name}");
			GD.Print($"  - Scaling: {_nodeToScale.Name}");
			GD.Print($"  - Initial visibility: {_wasVisible}");
			GD.Print($"  - Delay before scale: {_delayBeforeScale}s");
		}
		
		// If watched node is already visible, scale with delay
		if (_wasVisible)
		{
			ScaleUpWithDelay();
		}
	}
	
	public override void _Process(double delta)
	{
		// Check if we have valid nodes
		if (_nodeToScale == null || _nodeToWatch == null)
			return;
			
		// Get current visibility directly from CanvasItem
		bool currentVisible = _nodeToWatch.Visible;
		
		// Check if visibility has changed
		if (currentVisible != _wasVisible)
		{
			OnVisibilityChanged(currentVisible);
			_wasVisible = currentVisible;
		}
	}
	
	private void OnVisibilityChanged(bool isVisible)
	{
		if (_debugMode)
			GD.Print($"👁️ {_nodeToWatch.Name} visibility changed to: {isVisible}");
			
		if (isVisible)
		{
			// Check if we should scale only once
			if (_scaleOnlyOnce && _hasScaledOnce)
			{
				if (_debugMode)
					GD.Print("⏭️ Skipping scale - already scaled once and scaleOnlyOnce is true");
				return;
			}
			
			// Start scaling up animation with delay
			ScaleUpWithDelay();
		}
		else if (_resetScaleOnHide)
		{
			// Reset scale when hidden (no delay for hiding)
			ScaleDown();
		}
	}
	
	public void ScaleUp()
	{
		if (_nodeToScale == null) return;
		
		if (_debugMode)
			GD.Print($"📈 Scaling up {_nodeToScale.Name}");
			
		// Stop any existing tween
		if (_scaleTween != null)
		{
			_scaleTween.Kill();
		}
		
		// Create new tween
		_scaleTween = CreateTween();
		
		// Set starting scale
		_nodeToScale.Scale = _startScale;
		
		// Animate to target scale
		_scaleTween.TweenProperty(_nodeToScale, "scale", _targetScale, _scaleDuration);
		_scaleTween.SetTrans(_transitionType);
		_scaleTween.SetEase(_easeType);
		
		// Mark that we've scaled once
		_hasScaledOnce = true;
		
		// Optional: Add callback when animation finishes
		_scaleTween.TweenCallback(Callable.From(OnScaleUpComplete));
	}
	
	// Scale up with delay
	public async void ScaleUpWithDelay()
	{
		if (_delayBeforeScale > 0)
		{
			if (_debugMode)
				GD.Print($"⏰ Waiting {_delayBeforeScale} seconds before scaling {_nodeToScale?.Name}...");
				
			await ToSignal(GetTree().CreateTimer(_delayBeforeScale), SceneTreeTimer.SignalName.Timeout);
			
			if (_debugMode)
				GD.Print($"✅ Delay finished, starting scale animation for {_nodeToScale?.Name}");
		}
		
		ScaleUp();
	}
	
	public void ScaleDown()
	{
		if (_nodeToScale == null) return;
		
		if (_debugMode)
			GD.Print($"📉 Scaling down {_nodeToScale.Name}");
			
		// Stop any existing tween
		if (_scaleTween != null)
		{
			_scaleTween.Kill();
		}
		
		// Create new tween
		_scaleTween = CreateTween();
		
		// Animate to start scale (usually zero)
		_scaleTween.TweenProperty(_nodeToScale, "scale", _startScale, _scaleDuration * 0.5f); // Faster scale down
		_scaleTween.SetTrans(Tween.TransitionType.Quint);
		_scaleTween.SetEase(Tween.EaseType.In);
		
		// Optional: Add callback when animation finishes
		_scaleTween.TweenCallback(Callable.From(OnScaleDownComplete));
	}
	
	// Manual methods for external control
	public void ForceScaleUp()
	{
		if (_debugMode)
			GD.Print($"🚀 Force scaling up {_nodeToScale?.Name}");
		ScaleUp();
	}
	
	public void ForceScaleDown()
	{
		if (_debugMode)
			GD.Print($"⬇️ Force scaling down {_nodeToScale?.Name}");
		ScaleDown();
	}
	
	public void ForceScaleUpWithDelay()
	{
		if (_debugMode)
			GD.Print($"🕐 Force scaling up with delay {_nodeToScale?.Name}");
		ScaleUpWithDelay();
	}
	
	public void StopScaleAnimation()
	{
		if (_scaleTween != null)
		{
			_scaleTween.Kill();
		}
		if (_debugMode)
			GD.Print($"⏹️ Scale animation stopped for {_nodeToScale?.Name}");
	}
	
	public void ResetScale()
	{
		StopScaleAnimation();
		if (_nodeToScale != null)
		{
			_nodeToScale.Scale = _startScale;
		}
		_hasScaledOnce = false;
		if (_debugMode)
			GD.Print($"🔄 Scale reset for {_nodeToScale?.Name}");
	}
	
	public void SetToTargetScale()
	{
		StopScaleAnimation();
		if (_nodeToScale != null)
		{
			_nodeToScale.Scale = _targetScale;
		}
		_hasScaledOnce = true;
		if (_debugMode)
			GD.Print($"🎯 Scale set to target for {_nodeToScale?.Name}");
	}
	
	// Callback methods
	private void OnScaleUpComplete()
	{
		if (_debugMode)
			GD.Print($"✨ Scale up animation completed for {_nodeToScale?.Name}");
	}
	
	private void OnScaleDownComplete()
	{
		if (_debugMode)
			GD.Print($"⬇️ Scale down animation completed for {_nodeToScale?.Name}");
	}
	
	// Runtime configuration methods
	public void SetNodesToControl(Node2D nodeToScale, CanvasItem nodeToWatch)
	{
		_nodeToScale = nodeToScale;
		_nodeToWatch = nodeToWatch;
		
		if (_nodeToScale != null)
		{
			_nodeToScale.Scale = _startScale;
		}
		
		if (nodeToWatch != null)
		{
			_wasVisible = nodeToWatch.Visible;
		}
		
		_hasScaledOnce = false;
		
		if (_debugMode)
		{
			GD.Print($"🔧 Nodes updated - Watching: {_nodeToWatch?.Name}, Scaling: {_nodeToScale?.Name}");
		}
	}
	
	public void SetScaleParameters(float duration, Vector2 startScale, Vector2 targetScale)
	{
		_scaleDuration = duration;
		_startScale = startScale;
		_targetScale = targetScale;
		
		if (_debugMode)
			GD.Print($"⚙️ Scale parameters updated - Duration: {duration}, Start: {startScale}, Target: {targetScale}");
	}
	
	public void SetDelayBeforeScale(float delay)
	{
		_delayBeforeScale = delay;
		
		if (_debugMode)
			GD.Print($"⏰ Delay before scale updated to: {delay} seconds");
	}
	
	public void SetScaleParametersWithDelay(float delay, float duration, Vector2 startScale, Vector2 targetScale)
	{
		_delayBeforeScale = delay;
		_scaleDuration = duration;
		_startScale = startScale;
		_targetScale = targetScale;
		
		if (_debugMode)
			GD.Print($"🛠️ All scale parameters updated - Delay: {delay}, Duration: {duration}, Start: {startScale}, Target: {targetScale}");
	}
	
	// Utility methods
	public bool IsScaling()
	{
		return _scaleTween != null && _scaleTween.IsValid();
	}
	
	public bool HasScaledOnce()
	{
		return _hasScaledOnce;
	}
	
	public Node2D GetNodeToScale()
	{
		return _nodeToScale;
	}
	
	public CanvasItem GetNodeToWatch()
	{
		return _nodeToWatch;
	}
	
	public float GetDelayBeforeScale()
	{
		return _delayBeforeScale;
	}
}
