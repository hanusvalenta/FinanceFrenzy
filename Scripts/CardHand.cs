using Godot;
using System;

public partial class CardHand : Sprite2D
{
	private bool _isXLocked = false;
	private const float LOCK_X_POSITION = 661.0f;

	private Vector2 _previousMousePosition;
	private const float SWIPE_VELOCITY_THRESHOLD = 2500.0f;
	private bool _hasWon = false;

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Hidden;
		_previousMousePosition = GetGlobalMousePosition();
	}

	public override void _Process(double delta)
	{
		Vector2 mousePosition = GetGlobalMousePosition();
		float newX;

		if (!_hasWon && delta > 0 && _isXLocked)
		{
			float velocityY = (mousePosition.Y - _previousMousePosition.Y) / (float)delta;
			if (velocityY > SWIPE_VELOCITY_THRESHOLD)
			{
				_hasWon = true;
				WinSequence();
			}
		}
		_previousMousePosition = mousePosition;

		if (mousePosition.X >= LOCK_X_POSITION)
		{
			_isXLocked = true;
		}

		newX = _isXLocked ? LOCK_X_POSITION : mousePosition.X;

		GlobalPosition = new Vector2(newX, mousePosition.Y);
	}

	private async void WinSequence()
	{
		await ToSignal(GetTree().CreateTimer(2.0), SceneTreeTimer.SignalName.Timeout);
		GetNode<DataModel>("..").V_Bool_LvlWonSwitch	= true;
	}

	public override void _ExitTree()
	{
		Input.MouseMode = Input.MouseModeEnum.Visible;
	}
}
