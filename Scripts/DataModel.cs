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

	public float F_BeatSpeed_RNil(float PAR_DecreaseSPD_Float = 0.0f)
	{
		if(PAR_DecreaseSPD_Float != 0.0f)
		{
			GetTree().Root.SetMeta("Speed", (float)GetTree().Root.GetMeta("Speed")-PAR_DecreaseSPD_Float);
		}

		return (float)GetTree().Root.GetMeta("Speed");
	}
}
