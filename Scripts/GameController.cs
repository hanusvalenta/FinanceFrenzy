using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;

public partial class GameController : Control
{
	Button V_Btn_Sets;

	Node V_Scene_Interm;
	Node V_Scene_Death;

	[Export]
	int V_Int_Delay_ms;

	public override void _Ready()
	{
		GetTree().Root.SetMeta("Sanity", 101);
		GetTree().Root.SetMeta("Speed", 0.464f);

		V_Scene_Interm		= ResourceLoader.Load<PackedScene>("res://Scenes/Intermission.tscn").Instantiate();
		V_Scene_Death		= ResourceLoader.Load<PackedScene>("res://Scenes/End.tscn").Instantiate();

		GetNode<Button>("./Start").Pressed	+= () => {F_LoadLVL_RNil(false);};
		GetNode<Button>("./Options").Pressed+= () => {F_Settings_RNil(true);};
		GetNode<Button>("./Exit").Pressed	+= () => {F_LoadLVL_RNil(true);};
	}

	public void F_Settings_RNil(bool PAR_Open_Bool)
	{
		
	}
	
	public void F_Switch() {
		
	}

	public async void F_LoadLVL_RNil(bool PAR_Exit_Bool)
	{
		if(V_Int_Delay_ms	> 0)
		{
			await Task.Delay(V_Int_Delay_ms);
		}

		if(PAR_Exit_Bool	== true)
		{
			GetTree().Quit();
		}

		GetTree().Root.SetMeta("Sanity", 100);

		AudioStreamPlayer V_Audio_	= new AudioStreamPlayer
		{
			Stream			= new AudioStreamWav
			{
				ResourcePath= "res://Sounds/mus_sparkle_act1.wav",
				LoopMode	= AudioStreamWav.LoopModeEnum.Forward
			},
		};

		

		GetTree().Root.AddChild(V_Audio_);
		V_Audio_.Play();

		Node V_Node_CurScene= GetTree().CurrentScene;
		GetTree().Root.AddChild(V_Scene_Interm);
		GetTree().Root.RemoveChild(V_Node_CurScene);
	}
}
