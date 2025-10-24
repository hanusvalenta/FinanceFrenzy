using Godot;
using System;

public partial class Plank : RigidBody2D
{
	[Export]
	public float RotationStrength { get; set; } = 8000.0f;

	public override void _PhysicsProcess(double delta)
	{
		LinearVelocity = new Vector2(0, Mathf.Max(LinearVelocity.Y, 0));

		float rotationDirection = 0.0f;

		if (Input.IsKeyPressed(Key.A))
		{
			rotationDirection = -1.0f;
		}
		else if (Input.IsKeyPressed(Key.D))
		{
			rotationDirection = 1.0f;
		}

		ApplyTorque(rotationDirection * RotationStrength);
	}
}
