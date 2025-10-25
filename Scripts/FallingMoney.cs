using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class FallingMoney : Sprite2D
{
	[Export]
	public float TargetScaleValue { get; set; } = 0.187f;

	[Export]
	public float AnimationDuration { get; set; } = 2.0f;

	[Export]
	private Texture2D _texture1;
	[Export]
	private Texture2D _texture2;
	[Export]
	private Texture2D _texture3;

    private readonly RandomNumberGenerator _rng = new();
    
	private Area2D _catchArea;

	[Export]
	private Area2D _moneyArea;

    public override async void _Ready()
    {
		var textures = new List<Texture2D> { _texture1, _texture2, _texture3 }
			.Where(t => t != null)
			.ToList();

		if (textures.Count > 0)
		{
			int randomIndex = _rng.RandiRange(0, textures.Count - 1);
			this.Texture = textures[randomIndex];
		}

        var tween = CreateTween();

        tween.TweenProperty(
            this,
            "scale",
            new Vector2(TargetScaleValue, TargetScaleValue),
            AnimationDuration
        ).SetTrans(Tween.TransitionType.Quint)
         .SetEase(Tween.EaseType.Out);

        await ToSignal(tween, Tween.SignalName.Finished);

        CheckCollisionAndFinalize(_catchArea);
    }
    
    private void _SetCatchArea(Area2D catchArea)
    {
        _catchArea = catchArea;
    }

	private void CheckCollisionAndFinalize(Area2D _catchArea)
	{
		if (_catchArea != null && _catchArea.Name == "HandCatchArea" && _moneyArea != null && _moneyArea.OverlapsArea(_catchArea))
		{
			Vector2 originalGlobalScale = this.GlobalScale;

			var oldParent = GetParent();
            oldParent.RemoveChild(this);
            
			Node newParent = _catchArea.GetParent();
			if (newParent != null)
			{
				newParent.AddChild(this);
			}

			this.GlobalScale = originalGlobalScale;

            this.TopLevel = false;

            this.GetChildren().OfType<Area2D>().ToList().ForEach(area => area.QueueFree());

            float offsetX = _rng.RandfRange(-8.0f, 8.0f);
            float offsetY = _rng.RandfRange(-8.0f, 8.0f);

            this.Offset = new Vector2(0, 0);
            this.Position = _catchArea.Position + new Vector2(offsetX, offsetY);
		}
		else
		{
            QueueFree();

			GetNode<Seven>("..").F_Loose_RNil();
		}
	}
}
