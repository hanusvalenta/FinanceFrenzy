using Godot;
using System;

public partial class MenuManager : Node
{
	[Export]
	private PackedScene _menuScene;

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_cancel"))
		{
			if (_menuScene != null)
			{
				GetViewport().SetInputAsHandled();
				GetTree().ChangeSceneToPacked(_menuScene);
			}
		}
	}
}
