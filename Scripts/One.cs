using Godot;
using System;
using System.Collections.Generic;

public partial class One : Node2D
{
	[Export]
	private Node2D[] _targetObjects = new Node2D[5];

	private Timer _drawTimer;
	private int _connectionsDrawn = 0;
	private readonly Color _upwardColor = Colors.Green;
	private readonly Color _downwardColor = Colors.Red;
    private const float LINE_DRAW_INTERVAL = 0.5f;
    private int goodClicks = 0;

	public override void _Ready()
	{
		_drawTimer = new Timer();
		AddChild(_drawTimer);
		_drawTimer.WaitTime = LINE_DRAW_INTERVAL;
		_drawTimer.OneShot = false;
		_drawTimer.Timeout += OnDrawTimerTimeout;
		_drawTimer.Start();
	}

	private void OnDrawTimerTimeout()
	{
		if (_connectionsDrawn < _targetObjects.Length - 1)
		{
			_connectionsDrawn++;
			QueueRedraw();
		}
		else
		{
			_drawTimer.Stop();
		}
	}
	
	public override void _Draw()
	{
		for (int i = 0; i < _connectionsDrawn; i++)
		{
			Node2D startNode = _targetObjects[i];
			Node2D endNode = _targetObjects[i + 1];
	
			if (startNode != null && endNode != null)
			{
				bool isMovingUp = endNode.GlobalPosition.Y < startNode.GlobalPosition.Y;
				Color lineColor = isMovingUp ? _upwardColor : _downwardColor;

				Vector2 localStart = ToLocal(startNode.GlobalPosition);
				Vector2 localEnd = ToLocal(endNode.GlobalPosition);
				DrawLine(localStart, localEnd, lineColor, 2.0f);
			}
		}
	}

	private void F_FinishLevel_RNil(int PAR_SanityInc_Int)
	{
		GetNode<DataModel>("..").F_SanityChange_RNil(PAR_SanityInc_Int);
		GetNode<DataModel>("..").V_Bool_LvlWonSwitch	= true;
	}

	private void Good()
	{
		goodClicks++;
		if (goodClicks >= 5)
		{
			F_FinishLevel_RNil(0);
		}
	}
	
	private void Bad()
	{
		F_FinishLevel_RNil(-30);
	}
}
