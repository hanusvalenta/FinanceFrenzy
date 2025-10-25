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
	List<int> V_IntList_KeyBLVL		= new List<int>(){6};
	private string V_Str_GameScnPath= "res://Scenes/GameScenes/";

	Sprite2D V_Sprite2_SanityState;
	Sprite2D V_Sprite2_InputHint;

	public override void _Ready()
	{
		V_Double_Rhythm		= (double)GetTree().Root.GetMeta("Speed");
		V_Double_Nextlvl	= (double)GetMeta("V_NextLevelIn")*V_Double_Rhythm;
		
		if((bool)GetMeta("V_IsInterm")	== true)
		{
			V_Sprite2_SanityState			= GetNode<Sprite2D>("./Reaction/Sane");

			if((int)GetTree().Root.GetMeta("Sanity") < 75 && (int)GetTree().Root.GetMeta("Sanity") > 45) 
			{
				GetNode<Sprite2D>("./Reaction/Sane").Visible	= false;

				V_Sprite2_SanityState			= GetNode<Sprite2D>("./Reaction/NotSane");
				V_Sprite2_SanityState.Visible	= true;
			}
			else if((int)GetTree().Root.GetMeta("Sanity") <= 45)
			{
				GetNode<Sprite2D>("./Reaction/Sane").Visible	= false;

				V_Sprite2_SanityState			= GetNode<Sprite2D>("./Reaction/InSane");
				V_Sprite2_SanityState.Visible	= true;
			}

			V_Random_			= new Random();

			int V_Int_Max			= 7;
			int V_Int_NextLVL;

			List<int> V_IntList_Pld	= ((Godot.Collections.Array<int>)GetTree().Root.GetMeta("Played")).ToList<int>();

			if(V_Int_Max			== V_IntList_Pld.Count)
			{
				F_ChangeLevel_RNil("res://Scenes/Win.tscn");
				return;
			}

			do
			{
				V_Int_NextLVL		= V_Random_.Next(1, V_Int_Max+1);

			}while(V_IntList_Pld.IndexOf(V_Int_NextLVL) != -1);

		
			V_IntList_Pld.Add(V_Int_NextLVL);

			GetTree().Root.SetMeta("Played", V_IntList_Pld.ToArray<int>());

			V_Str_GameScnPath		= V_Str_GameScnPath+V_Int_NextLVL+".tscn";

			V_Sprite2_InputHint			= GetNode<Sprite2D>("./NextInputType/Mouse");

			if(V_IntList_KeyBLVL.Contains(V_Int_NextLVL))
			{
				GetNode<Sprite2D>("./NextInputType/Mouse").Visible		= false;

				V_Sprite2_InputHint			= GetNode<Sprite2D>("./NextInputType/Keyboard");
				V_Sprite2_InputHint.Visible	= true;
			}
		}
	}

	public override void _Process(double delta)
	{
		if((int)GetTree().Root.GetMeta("Sanity") != 101 && V_Bool_QuitService == false)
		{
			V_Double_Nextlvl		-= delta;
			V_Double_Rhythm			-= delta;

			if(V_Double_Rhythm		< 0)
			{
				if((bool)GetMeta("V_IsInterm") == true)
				{
					V_Sprite2_SanityState.RotationDegrees	= V_Sprite2_SanityState.RotationDegrees == -10 ? 0 : -10;
					V_Sprite2_InputHint.RotationDegrees		= V_Sprite2_InputHint.RotationDegrees == 0 ? 15 : 0;
				}

				if(V_Bool_LvlWonSwitch	== true)
				{GD.Print("Add point");
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

				F_ChangeLevel_RNil((bool)GetMeta("V_IsInterm") == true ? V_Str_GameScnPath : "res://Scenes/Intermission.tscn");
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
	{GD.Print(PAR_ScenePath_Str);
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
