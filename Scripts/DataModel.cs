using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class DataModel : Node2D
{
	Random V_Random_;
	public double V_Double_Nextlvl;
	private bool V_Bool_QuitService	= false;

	public override void _Ready()
	{
		V_Double_Nextlvl	= (double)GetMeta("V_NextLevelIn");

		V_Random_			= new Random();
	}

	public override void _Process(double delta)
	{
		if((int)GetTree().Root.GetMeta("Sanity") != 101 && V_Bool_QuitService == false)
		{
			V_Double_Nextlvl	-= delta;

			if(V_Double_Nextlvl	< 0)
			{
				V_Bool_QuitService	= true;

				if((bool)GetMeta("V_IsInterm") == false)
				{
					GetNode<DataModel>("..").F_SanityChange_RNil(-10);
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
	}

	public void F_ChangeLevel_RNil(string PAR_ScenePath_Str	= "")
	{GD.Print(V_Double_Nextlvl+" | "+PAR_ScenePath_Str);
		if(string.IsNullOrEmpty(PAR_ScenePath_Str))
		{
			int V_Int_Max;
			int V_Int_NextLVL;

			if(true)//(int)GetTree().Root.GetMeta("Sanity") > 35)
			{
				V_Int_Max			= 4;
				PAR_ScenePath_Str	= "res://Scenes/GameScenes/";
			}
			else
			{
				V_Int_Max			= 2;
				PAR_ScenePath_Str	= "res://Scenes/CursedScenes/";
			}

			do
			{
				V_Int_NextLVL			= V_Random_.Next(1, V_Int_Max+1);
			}while(false);//(int[])GetTree().Root.GetMeta("Played").IndexOf(V_Int_NextLVL) != -1);

			PAR_ScenePath_Str		= PAR_ScenePath_Str+V_Int_NextLVL+".tscn";
		}

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
