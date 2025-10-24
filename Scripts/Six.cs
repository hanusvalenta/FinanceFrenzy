using Godot;
using System;

public partial class Six : Node
{
	[Export]
	private PackedScene Prefab1 { get; set; }

	[Export]
	private PackedScene Prefab2 { get; set; }

	[Export]
	private Node2D Node2D_1 { get; set; }

	[Export]
	private Node2D Node2D_2 { get; set; }

	[Export]
	private float PrefabScale { get; set; } = 1.0f;

	private Timer _spawnTimer;
	private readonly RandomNumberGenerator _rng = new();

	public override void _Ready()
	{
		_spawnTimer = new Timer();
		AddChild(_spawnTimer);
		_spawnTimer.Timeout += OnSpawnTimerTimeout;

		ScheduleNextSpawn();
	}

	private void OnSpawnTimerTimeout()
	{
		if (_rng.RandiRange(0, 1) == 0)
		{
			SpawnPrefab(Prefab1, Node2D_1);
		}
		else
		{
			SpawnPrefab(Prefab2, Node2D_2);
		}

		ScheduleNextSpawn();
	}

	private void ScheduleNextSpawn()
	{
		_spawnTimer.WaitTime = _rng.RandfRange(0.5f, 2.0f);
		_spawnTimer.Start();
	}

	private void SpawnPrefab(PackedScene prefab, Node2D spawnLocation)
	{
		if (prefab != null && spawnLocation != null)
		{
			Node2D instance = prefab.Instantiate<Node2D>();
			AddChild(instance);
			instance.GlobalPosition = spawnLocation.GlobalPosition;
			instance.Scale = new Vector2(PrefabScale, PrefabScale);
		}
	}
}
