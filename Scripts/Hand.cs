using Godot;
using System;

public partial class Hand : Sprite2D
{
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Hidden;
	}

	public override void _Process(double delta)
	{
		GlobalPosition = GetGlobalMousePosition();
	}

	public override void _ExitTree()
	{
		Input.MouseMode = Input.MouseModeEnum.Visible;
	}
}
