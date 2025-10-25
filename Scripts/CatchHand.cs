using Godot;
using System;

public partial class CatchHand : Sprite2D
{
	private int _initialChildCount;
	private bool _hasWon = false;

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Hidden;
		_initialChildCount = GetChildCount();
	}

	public override void _Process(double delta)
	{
		GlobalPosition = GetGlobalMousePosition();

		if (!_hasWon)
		{
			if (GetChildCount() >= _initialChildCount + 5)
			{
				_hasWon = true;
				GetNode<DataModel>("..").V_Bool_LvlWonSwitch	= true;
			}
		}
	}

	public override void _ExitTree()
	{
		Input.MouseMode = Input.MouseModeEnum.Visible;
	}
}
