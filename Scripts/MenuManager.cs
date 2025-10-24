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
			GetViewport().SetInputAsHandled();

			if (_menuScene != null)
			{
				GetTree().ChangeSceneToPacked(_menuScene);
			}
			else
			{
				GetTree().Quit();
			}
		}
	}
}
