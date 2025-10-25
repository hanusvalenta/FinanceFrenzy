using Godot;
using System;

public partial class Mario : CharacterBody2D
{
	[Export]
	public float Speed { get; set; } = 300.0f;

	[Export]
	public float JumpVelocity { get; set; } = -400.0f;
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	
	private bool _jumpRequested = false;

	public override void _Input(InputEvent @event)
	{
		if (Input.IsKeyPressed(Key.W))
		{
			_jumpRequested = true;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		if (!IsOnFloor())
			velocity.Y += gravity * (float)delta;

		if (_jumpRequested && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		float moveDirection = 0.0f;

		if (Input.IsKeyPressed(Key.D))
		{
			moveDirection = 1.0f;
		}
		else if (Input.IsKeyPressed(Key.A))
		{
			moveDirection = -1.0f;
		}

		if (moveDirection != 0.0f)
		{
			velocity.X = moveDirection * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();

		_jumpRequested = false;

		if (GlobalPosition.Y > 664.0f)
		{
			GD.Print("Loose");
		}
	}
	
	private void OnBodyEntered(Area2D body)
	{
		GD.Print("Win");
	}
}
