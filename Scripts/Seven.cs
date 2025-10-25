using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
public partial class Seven : Node2D
{
	[Export]
	private Node2D Node2D_1 { get; set; }

	[Export]
	private Node2D Node2D_2 { get; set; }

	[Export]
	private Node2D Node2D_3 { get; set; }

	[Export]
	private Node2D Node2D_4 { get; set; }

	[Export]
	private Node2D Node2D_5 { get; set; }

	[Export]
	private PackedScene MoneyPrefab { get; set; }

	private Timer _spawnTimer;
	private readonly RandomNumberGenerator _rng = new();
	private List<Node2D> _spawnLocations;

    public override void _Ready()
    {
        _spawnLocations = new List<Node2D> { Node2D_1, Node2D_2, Node2D_3, Node2D_4, Node2D_5 }
            .Where(node => node != null)
            .ToList();

        _spawnTimer = new Timer();
        AddChild(_spawnTimer);
        _spawnTimer.WaitTime = 3.0f;
        _spawnTimer.Timeout += OnSpawnTimerTimeout;
        _spawnTimer.Start();
    }

	private void OnSpawnTimerTimeout()
	{
		if (MoneyPrefab == null || _spawnLocations.Count == 0)
		{
			_spawnTimer.Stop();
			return;
		}

		int randomIndex = _rng.RandiRange(0, _spawnLocations.Count - 1);
		Node2D randomSpawnPoint = _spawnLocations[randomIndex];

		Node2D moneyInstance = MoneyPrefab.Instantiate<Node2D>();

		AddChild(moneyInstance);
		moneyInstance.GlobalPosition = randomSpawnPoint.GlobalPosition;
	}
}
