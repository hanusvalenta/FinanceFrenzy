using Godot;
using System;

public partial class DataModel : Node2D
{
	Random V_Random_;

	public override void _Ready()
	{
		GD.Print(GetTree().Root.GetMeta("Sanity"));

		V_Random_	= new Random();		
	}

	public void F_SanityChange_RNil(int PAR_Sanity)
	{
		if(!(PAR_Sanity+(int)GetTree().Root.GetMeta("Sanity") > 100))
		{
			GetTree().Root.SetMeta("Sanity", (int)GetTree().Root.GetMeta("Sanity")+PAR_Sanity);
		}
	}

	public void F_ChangeLevel_RNil(string PAR_ScenePath_Str	= "")
	{
		if(string.IsNullOrEmpty(PAR_ScenePath_Str))
		{
			PAR_ScenePath_Str	= "res://Scenes/GameScenes";

			int V_Int_NextLVL	= V_Random_.Next(0, 2);
			PAR_ScenePath_Str	= PAR_ScenePath_Str+V_Int_NextLVL+".tscn";
		}


		Node V_Node_CurScene= GetTree().CurrentScene;
		GetTree().Root.AddChild(ResourceLoader.Load<PackedScene>(PAR_ScenePath_Str).Instantiate());
		GetTree().Root.RemoveChild(V_Node_CurScene);
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
