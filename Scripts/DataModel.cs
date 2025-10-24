using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class DataModel : Node2D
{
	Random V_Random_;
	public double V_Double_Nextlvl;
	public bool V_Bool_LvlWonSwitch	= false;
	private double V_Double_Rhythm;
	private bool V_Bool_QuitService	= false;

	public override void _Ready()
	{
		V_Double_Rhythm		= (double)GetTree().Root.GetMeta("Speed");
		V_Double_Nextlvl	= (double)GetMeta("V_NextLevelIn")*V_Double_Rhythm;
		

		V_Random_			= new Random();
	}

	public override void _Process(double delta)
	{
		if((int)GetTree().Root.GetMeta("Sanity") != 101 && V_Bool_QuitService == false)
		{
			V_Double_Nextlvl		-= delta;
			V_Double_Rhythm			-= delta;

			if(V_Double_Rhythm		< 0)
			{
				if(V_Bool_LvlWonSwitch	== true)
				{GD.Print("Point add");
					V_Bool_QuitService	= true;

					F_SanityChange_RNil(10);
					F_ChangeLevel_RNil("res://Scenes/Intermission.tscn");

					return;
				}

				V_Double_Rhythm		= (double)GetTree().Root.GetMeta("Speed");
			}

			if(V_Double_Nextlvl		< 0)
			{
				V_Bool_QuitService	= true;

				if((bool)GetMeta("V_IsInterm") == false)
				{
					F_SanityChange_RNil(-20);
				}

				F_ChangeLevel_RNil((bool)GetMeta("V_IsInterm") == true ? "" : "res://Scenes/Intermission.tscn");
			}
		}
	}

	public void F_SanityChange_RNil(int PAR_Sanity)
	{
		if(!(PAR_Sanity+(int)GetTree().Root.GetMeta("Sanity") > 100))
		{
			GetTree().Root.SetMeta("Sanity", (int)GetTree().Root.GetMeta("Sanity")+PAR_Sanity);
		}

		if((int)GetTree().Root.GetMeta("Sanity") < 1)
		{
			F_ChangeLevel_RNil("res://Scenes/End.tscn");
		}
	}

	public void F_ChangeLevel_RNil(string PAR_ScenePath_Str	= "")
	{
		if(string.IsNullOrEmpty(PAR_ScenePath_Str))
		{
			int V_Int_Max			= 6;
			int V_Int_NextLVL;
			PAR_ScenePath_Str		= "res://Scenes/GameScenes/";

			List<int> V_IntList_Pld	= ((Godot.Collections.Array<int>)GetTree().Root.GetMeta("Played")).ToList<int>();

			if(V_Int_Max			== V_IntList_Pld.Count)
			{GD.Print("Out");
				GetTree().ChangeSceneToPacked(ResourceLoader.Load<PackedScene>("res://Scenes/Win.tscn"));
				return;
			}

			do
			{
				V_Int_NextLVL		= V_Random_.Next(1, V_Int_Max+1);

			}while(V_IntList_Pld.IndexOf(V_Int_NextLVL) != -1);

			V_IntList_Pld.Add(V_Int_NextLVL);

			GetTree().Root.SetMeta("Played", V_IntList_Pld.ToArray<int>());

			PAR_ScenePath_Str		= PAR_ScenePath_Str+V_Int_NextLVL+".tscn";
		}
GD.Print(PAR_ScenePath_Str);
		GetTree().ChangeSceneToPacked(ResourceLoader.Load<PackedScene>(PAR_ScenePath_Str));
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
