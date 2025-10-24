using Godot;
using System;

public partial class Hand : Sprite2D
{
	[Export]
	private CanvasItem _snappableObject1;
	[Export]
	private CanvasItem _snappableObject2;
	[Export]
	private Node2D _snapTargetPosition;

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Hidden;
	}

	public override void _Process(double delta)
	{
		GlobalPosition = GetGlobalMousePosition();
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButtonEvent && mouseButtonEvent.ButtonIndex == MouseButton.Left && mouseButtonEvent.Pressed)
		{
			Vector2 mouseGlobalPos = GetGlobalMousePosition();

			if (_snappableObject1 != null && _snapTargetPosition != null)
			{
				if (IsMouseOverNode(_snappableObject1, mouseGlobalPos))
				{
					((Node2D)_snappableObject1).GlobalPosition = _snapTargetPosition.GlobalPosition;
					GetViewport().SetInputAsHandled();
					return;
				}
			}
			
			if (_snappableObject2 != null && _snapTargetPosition != null)
			{
				if (IsMouseOverNode(_snappableObject2, mouseGlobalPos))
				{
					((Node2D)_snappableObject2).GlobalPosition = _snapTargetPosition.GlobalPosition;
					GetViewport().SetInputAsHandled();
				}
			}
		}
	}

	private bool IsMouseOverNode(CanvasItem node, Vector2 mouseGlobalPos)
	{
		if (node is Control control)
		{
			if (node is Node2D node2D)
			{
				return control.GetRect().HasPoint(node2D.ToLocal(mouseGlobalPos));
			}
		}
		else if (node is Sprite2D sprite)
		{
			return sprite.GetRect().HasPoint(sprite.ToLocal(mouseGlobalPos));
		}

		return false;
	}

	public override void _ExitTree()
	{
		Input.MouseMode = Input.MouseModeEnum.Visible;
	}
}
