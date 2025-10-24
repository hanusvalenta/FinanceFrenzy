using Godot;
using System;

public partial class CardHand : Sprite2D
{
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Hidden;
	}

	public override void _Process(double delta)
	{
		Vector2 mousePosition = GetGlobalMousePosition();
		float newX = Mathf.Min(mousePosition.X, 662.0f);
		GlobalPosition = new Vector2(newX, mousePosition.Y);
	}

	public override void _ExitTree()
	{
		Input.MouseMode = Input.MouseModeEnum.Visible;
	}
}
