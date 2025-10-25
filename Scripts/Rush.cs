using Godot;
using System;

public partial class Rush : Sprite2D
{
	[Export]
	public Sprite2D TargetSprite { get; set; }
	[Export]
	public Node2D TargetNode { get; set; }

	[ExportGroup("Game Mechanics")]
	[Export] public float StretchPerPress { get; set; } = 0.15f;
	[Export] public float StretchDecayRate { get; set; } = 1.0f;
	[Export] public float WinScaleY { get; set; } = 1.6f;
	[Export] public float FlingDuration { get; set; } = 0.4f;

	private Vector2 _initialScale;
	private bool _isFlinging = false;

	public override void _Ready()
	{
		_initialScale = this.Scale;
	}

	public override void _Process(double delta)
	{
		if (_isFlinging)
		{
			return;
		}

		if (Input.IsActionJustPressed("ui_accept"))
		{
			Scale = new Vector2(Scale.X  + StretchPerPress, Scale.Y);
		}

		if (Scale.X > _initialScale.X)
		{
			float decay = StretchDecayRate * (float)delta;
			Scale = new Vector2(Scale.X  - decay, Mathf.Max(_initialScale.Y, Scale.Y));
		}

		if (Scale.X >= WinScaleY)
		{
			FlingObject();
		}
	}

	private async void FlingObject()
	{
		_isFlinging = true;

		if (TargetSprite != null && TargetNode != null)
		{
			var tween = CreateTween();
			tween.TweenProperty(TargetSprite, "global_position", TargetNode.GlobalPosition, FlingDuration).SetTrans(Tween.TransitionType.Quint).SetEase(Tween.EaseType.Out);
			
			await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);
			GetNode<DataModel>("..").V_Bool_LvlWonSwitch = true;
		}
	}
}
