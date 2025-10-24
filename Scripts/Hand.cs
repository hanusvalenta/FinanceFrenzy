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
	[Export]
	private Texture2D _holdingTexture;

	private Texture2D _idleTexture;
	private Node2D _heldObject = null;

	public override void _Ready()
	{
		_idleTexture = Texture;
		Input.MouseMode = Input.MouseModeEnum.Hidden;
	}

	public override void _Process(double delta)
	{
		Vector2 mousePosition = GetGlobalMousePosition();
		GlobalPosition = mousePosition;

		if (_heldObject != null)
		{
			_heldObject.GlobalPosition = mousePosition;
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButtonEvent && mouseButtonEvent.ButtonIndex == MouseButton.Left)
		{
			if (mouseButtonEvent.Pressed)
			{
				Vector2 mouseGlobalPos = GetGlobalMousePosition();
				if (_heldObject == null && _snappableObject1 != null && IsMouseOverNode(_snappableObject1, mouseGlobalPos))
				{
					_heldObject = (Node2D)_snappableObject1;
				}
				else if (_heldObject == null && _snappableObject2 != null && IsMouseOverNode(_snappableObject2, mouseGlobalPos))
				{
					_heldObject = (Node2D)_snappableObject2;
				}

				if (_heldObject != null)
				{
					Texture = _holdingTexture;
					GetViewport().SetInputAsHandled();
				}
			}
			else
			{
				if (_heldObject == null) return;
				
				_heldObject = null;
				Texture = _idleTexture;
				GetViewport().SetInputAsHandled();
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
