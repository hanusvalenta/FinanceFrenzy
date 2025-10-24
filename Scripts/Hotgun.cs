using Godot;
using System;

public partial class Hotgun : Sprite2D
{
	[Export]
	private Sprite2D _sprite;

	[Export(PropertyHint.File, "*.png,*.jpg,*.tres")]
	private Texture2D _hole1Texture;

	[Export(PropertyHint.File, "*.png,*.jpg,*.tres")]
	private Texture2D _hole2Texture;

	[Export(PropertyHint.File, "*.png,*.jpg,*.tres")]
	private Texture2D _hole3Texture;

	private bool _holeFilled = false;

	private void _OnHole1BodyEntered(Node2D body)
	{
		if (_holeFilled)
		{
			_sprite.Texture = _hole3Texture;
			body.GetParent().QueueFree();
		}
		else
		{
			_sprite.Texture = _hole1Texture;
			_holeFilled = true;
			body.GetParent().QueueFree();
		}

		F_ForceDrop_RNil();
	}

	private void _OnHole2BodyEntered(Node2D body)
	{
		if (_holeFilled)
		{
			_sprite.Texture = _hole3Texture;
			body.GetParent().QueueFree();
		}
		else
		{
			_sprite.Texture = _hole2Texture;
			_holeFilled = true;
			body.GetParent().QueueFree();
		}

		F_ForceDrop_RNil();
	}

	private void F_ForceDrop_RNil()
    {
        GetNode<Hand>("../Hand").F_DropObject_RNil();
    }
}
