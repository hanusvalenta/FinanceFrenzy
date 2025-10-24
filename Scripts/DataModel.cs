using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class DataModel : Node2D
{
	Random V_Random_;
	public static int V_Int_Level			= 1;
	public static double V_Double_Nextlvl	= 3;
	public static bool V_Bool_IsInterim		= true;

	public override void _Ready()
	{
		V_Random_			= new Random();
	}

	public override void _Process(double delta)
	{
		if((int)GetTree().Root.GetMeta("Sanity") != 101)
		{
			V_Double_Nextlvl	-= delta;
			
			if(V_Double_Nextlvl	< 0)
			{
				F_ChangeLevel_RNil();
				V_Double_Nextlvl= 5;
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
	{
		if(string.IsNullOrEmpty(PAR_ScenePath_Str) && V_Bool_IsInterim	== false)
		{
			PAR_ScenePath_Str		= "res://Scenes/Intermission.tscn";
			V_Bool_IsInterim		= true;
		}
		else if(string.IsNullOrEmpty(PAR_ScenePath_Str))
		{
			int V_Int_Max;
			int V_Int_NextLVL;

			if(true)//(int)GetTree().Root.GetMeta("Sanity") > 35)
			{
				V_Int_Max			= 3;
				PAR_ScenePath_Str	= "res://Scenes/GameScenes/";
			}
			else
			{
				V_Int_Max			= 2;
				PAR_ScenePath_Str	= "res://Scenes/CursedScenes/";
			}

			
			V_Int_NextLVL			= V_Random_.Next(1, V_Int_Max+1);

			PAR_ScenePath_Str		= PAR_ScenePath_Str+V_Int_NextLVL+".tscn";
			V_Bool_IsInterim		= false;
		}

		NextLvL:

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
