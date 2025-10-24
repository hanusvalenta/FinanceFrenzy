using Godot;
using System;
public partial class Hotgun : Node
{
	[Export]
	private CollisionShape2D _hole1;
	[Export]
	private CollisionShape2D _hole2;
	[Export]
	private CollisionShape2D _ammo1;
	[Export]
	private CollisionShape2D _ammo2;
}
