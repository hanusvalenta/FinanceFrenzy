using Godot;
using System;

public partial class CigHand : Sprite2D
{
	[Export(PropertyHint.File, "*.png,*.jpg,*.tres")]
	private Texture2D _holdingTexture;
	[Export(PropertyHint.File, "*.png,*.jpg,*.tres")]
	private Texture2D _burntTexture;

	[Export]
	private Sprite2D _targetSprite;

	private Texture2D _originalTexture;
	private float _holdTimer = 0f;
	private const float HOLD_DURATION = 3.0f;
	private bool _isBurnt = false;

	public override void _Ready()
	{
		_originalTexture = this.Texture;
	}

	public override void _Process(double delta)
	{
		if (Input.IsMouseButtonPressed(MouseButton.Left))
		{
			if (_holdingTexture != null)
			{
				this.Texture = _holdingTexture;
			}

			_holdTimer += (float)delta;

			if (!_isBurnt && _holdTimer >= HOLD_DURATION)
			{
				if (_targetSprite != null && _burntTexture != null)
				{
					_targetSprite.Texture = _burntTexture;
					_isBurnt = true;

					GetNode<DataModel>("..").V_Bool_LvlWonSwitch	= true;
				}
			}
		}
		else
		{
			this.Texture = _originalTexture;
			_holdTimer = 0f;
		}
	}
}
