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
	List<int> V_IntList_KeyBLVL		= new List<int>(){6, 8, 9};
	private string V_Str_GameScnPath= "res://Scenes/GameScenes/";

	Sprite2D V_Sprite2_SanityState;
	Sprite2D V_Sprite2_InputHint;

	public delegate void D_RhythmStep_RNil();
	public event D_RhythmStep_RNil Event_RhythmStep;

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

			V_Random_				= new Random();

			int V_Int_Max			= 9;
			int V_Int_NextLVL;

			List<int> V_IntList_Pld	= ((Godot.Collections.Array<int>)GetTree().Root.GetMeta("Played")).ToList<int>();

			// Debug information for win condition
			GD.Print("Played levels count: " + V_IntList_Pld.Count);
			GD.Print("Max levels: " + V_Int_Max);
			GD.Print("Played levels: " + string.Join(", ", V_IntList_Pld));

			if(V_Int_Max			== V_IntList_Pld.Count)
			{
				GD.Print("Win condition met! All levels completed. Loading win scene...");
				F_ChangeLevel_RNil("res://Scenes/Winner.tscn");
				return;
			}

			do
			{
				V_Int_NextLVL		= V_Random_.Next(1, V_Int_Max+1);

			}while(V_IntList_Pld.IndexOf(V_Int_NextLVL) != -1);

		
			V_IntList_Pld.Add(V_Int_NextLVL);

			GetTree().Root.SetMeta("Played", V_IntList_Pld.ToArray<int>());

			V_Str_GameScnPath		= V_Str_GameScnPath+V_Int_NextLVL+".tscn";

			GD.Print("Next level to load: " + V_Str_GameScnPath);

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
				Event_RhythmStep?.Invoke();

				if((bool)GetMeta("V_IsInterm") == true)
				{
					V_Sprite2_SanityState.RotationDegrees	= V_Sprite2_SanityState.RotationDegrees == -10 ? 0 : -10;
					V_Sprite2_InputHint.RotationDegrees		= V_Sprite2_InputHint.RotationDegrees == 0 ? 15 : 0;
				}

				if(V_Bool_LvlWonSwitch	== true)
				{
					GD.Print("Level won! Adding points and moving to intermission.");
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
					GD.Print("Time up! Losing sanity.");
					F_SanityChange_RNil(-20);
				}

				string nextScene = (bool)GetMeta("V_IsInterm") == true ? V_Str_GameScnPath : "res://Scenes/Intermission.tscn";
				GD.Print("Time up! Loading next scene: " + nextScene);
				F_ChangeLevel_RNil(nextScene);
			}
		}
	}

	public void F_SanityChange_RNil(int PAR_Sanity)
	{
		GD.Print("Sanity change: " + PAR_Sanity + " (Current: " + (int)GetTree().Root.GetMeta("Sanity") + ")");
		
		if(PAR_Sanity	> 0)
		{
			GetTree().Root.GetNode<AudioStreamPlayer>("AudioStreamPlayer/ASP_SayWin").Play();
		}
		else
		{
			GetTree().Root.GetNode<AudioStreamPlayer>("AudioStreamPlayer/ASP_SayLose").Play();
		}

		int currentSanity = (int)GetTree().Root.GetMeta("Sanity");
		int newSanity = currentSanity + PAR_Sanity;

		if(!(newSanity > 100))
		{
			GetTree().Root.SetMeta("Sanity", newSanity);
			GD.Print("New sanity level: " + newSanity);
		}
		else
		{
			GD.Print("Sanity would exceed 100, keeping at: " + currentSanity);
		}

		if((int)GetTree().Root.GetMeta("Sanity") < 1)
		{
			GD.Print("Sanity depleted! Loading end scene.");
			F_ChangeLevel_RNil("res://Scenes/End.tscn");
		}
	}

	public void F_ChangeLevel_RNil(string PAR_ScenePath_Str	= "")
	{
		GD.Print("Attempting to load scene: " + PAR_ScenePath_Str);
		
		if(string.IsNullOrEmpty(PAR_ScenePath_Str))
		{
			GD.PrintErr("Scene path is null or empty!");
			return;
		}

		try 
		{
			// First check if the resource exists
			if(!ResourceLoader.Exists(PAR_ScenePath_Str))
			{
				GD.PrintErr("Scene file does not exist: " + PAR_ScenePath_Str);
				return;
			}

			// Try to load the scene
			var scene = ResourceLoader.Load<PackedScene>(PAR_ScenePath_Str);
			if (scene == null)
			{
				GD.PrintErr("Failed to load scene resource: " + PAR_ScenePath_Str);
				return;
			}
			
			GD.Print("Scene loaded successfully, changing scene...");
			var error = GetTree().ChangeSceneToPacked(scene);
			
			if(error != Error.Ok)
			{
				GD.PrintErr("Failed to change scene. Error code: " + error);
				
				// Try alternative method
				GD.Print("Trying alternative scene loading method...");
				error = GetTree().ChangeSceneToFile(PAR_ScenePath_Str);
				
				if(error != Error.Ok)
				{
					GD.PrintErr("Alternative method also failed. Error code: " + error);
				}
			}
		}
		catch (Exception e)
		{
			GD.PrintErr("Exception occurred while loading scene: " + e.Message);
			GD.PrintErr("Stack trace: " + e.StackTrace);
		}
	}

	public float F_BeatSpeed_RNil(float PAR_DecreaseSPD_Float = 0.0f)
	{
		if(PAR_DecreaseSPD_Float != 0.0f)
		{
			float currentSpeed = (float)GetTree().Root.GetMeta("Speed");
			float newSpeed = currentSpeed - PAR_DecreaseSPD_Float;
			GetTree().Root.SetMeta("Speed", newSpeed);
			GD.Print("Speed changed from " + currentSpeed + " to " + newSpeed);
		}

		return (float)GetTree().Root.GetMeta("Speed");
	}

	public void F_AlterReality_RNil(Node PAR__Node)
	{
		int V_Int_SanityAlter	= new Random().Next((int)GetTree().Root.GetMeta("Sanity"), 100);

		if(V_Int_SanityAlter	< 95)
		{
			GD.Print("Altering reality for node: " + PAR__Node.Name);
			((CanvasItem)PAR__Node).UseParentMaterial	= false;

			((CanvasItem)PAR__Node).QueueRedraw();
			((CanvasItem)PAR__Node)._Draw();

			PAR__Node.Reparent(PAR__Node.GetParent());
		}
	}
}
