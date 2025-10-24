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

		if (!_hasWon && delta > 0)
		{
			float velocityY = (mousePosition.Y - _previousMousePosition.Y) / (float)delta;
			if (velocityY > SWIPE_VELOCITY_THRESHOLD)
			{
				_hasWon = true;

				GetNode<DataModel>("..").F_SanityChange_RNil(10);
                GetNode<DataModel>("..").F_ChangeLevel_RNil("res://Scenes/Intermission.tscn");
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

	public override void _ExitTree()
	{
		Input.MouseMode = Input.MouseModeEnum.Visible;
	}
}
