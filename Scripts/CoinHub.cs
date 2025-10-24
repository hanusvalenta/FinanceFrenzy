using Godot;
using System;

public partial class CoinHub : Node2D
{
    [Export]
    private string CoinName;
    [Export]
    private CoinHub Hub1;
    [Export]
    private CoinHub Hub2;
    [Export]
    private CoinHub Hub3;

    private bool _isCoinHubActive = false;
    
    private void _OnHubEntered(Area2D body)
    {
        if (body.GetParent().Name == CoinName)
        {
            _isCoinHubActive = true;

            if (Hub1 != null && Hub2 != null && Hub3 != null)
            {
                if (Hub1._isCoinHubActive && Hub2._isCoinHubActive && Hub3._isCoinHubActive)
                {
                    GetNode<DataModel>("..").V_Bool_LvlWonSwitch	= true;
                }
            }
        }
	}
}
