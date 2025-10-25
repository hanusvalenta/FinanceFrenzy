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

	private readonly RandomNumberGenerator _rng = new();
	private List<Node2D> _spawnLocations;
	private int _rhythmStepCount	= 0;

	private bool V_Bool_CanSpawn	= true;

    public override void _Ready()
	{
		GetNode<DataModel>("..").Event_RhythmStep += OnRhythmStep;

        _spawnLocations = new List<Node2D> { Node2D_1, Node2D_2, Node2D_3, Node2D_4, Node2D_5 }
            .Where(node => node != null)
            .ToList();
    }

	private void OnRhythmStep()
	{
		_rhythmStepCount++;

		if (_rhythmStepCount % 4 != 0)
		{
			return;
		}

		if (MoneyPrefab == null || _spawnLocations.Count == 0)
		{
			return;
		}

		int randomIndex = _rng.RandiRange(0, _spawnLocations.Count - 1);
		Node2D randomSpawnPoint = _spawnLocations[randomIndex];

		Node2D moneyInstance = MoneyPrefab.Instantiate<Node2D>();

		AddChild(moneyInstance);
		moneyInstance.GlobalPosition = randomSpawnPoint.GlobalPosition;
	}

	public override void _ExitTree()
	{
		var dataModel = GetNode<DataModel>("..");
		if (dataModel != null)
		{
			dataModel.Event_RhythmStep -= OnRhythmStep;
		}
	}

	public void F_Loose_RNil()
	{
		V_Bool_CanSpawn	= false;
		GetNode<DataModel>("..").Event_RhythmStep -= OnRhythmStep;

		GetNode<DataModel>("..").F_SanityChange_RNil(-10);
		GetNode<DataModel>("..").Event_RhythmStep	+= () =>
		{
			GetNode<DataModel>("..").F_ChangeLevel_RNil("res://Scenes/Intermission.tscn");
		};
	}
}
