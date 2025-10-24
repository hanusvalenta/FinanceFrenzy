using Godot;
using System;

public partial class DataModel : Node2D
{
	public override void _Ready()
	{
		GD.Print(GetTree().Root.GetMeta("Sanity"));
	}

	public void F_SanityChange_RNil(int PAR_Sanity)
	{
		if(!(PAR_Sanity+(int)GetTree().Root.GetMeta("Sanity") > 100))
		{
			GetTree().Root.SetMeta("Sanity", (int)GetTree().Root.GetMeta("Sanity")+PAR_Sanity);
		}
	}

	public void F_ChangeLevel_RNil()
	{

	}
}
