using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;

public partial class GameController : Control
{
	Button V_Btn_Play;
	Button V_Btn_Sets;
	Button V_Btn_Exit;

	Node V_Scene_Interm;
	Node V_Scene_Death;

	public override void _Ready()
	{
		GetTree().Root.SetMeta("Sanity", 100);
		GetTree().Root.SetMeta("Speed", 0.5f);

		V_Scene_Interm		= ResourceLoader.Load<PackedScene>("res://Scenes/Intermission.tscn").Instantiate();
		V_Scene_Death		= ResourceLoader.Load<PackedScene>("res://Scenes/End.tscn").Instantiate();

		V_Btn_Play	= GetNode<Button>("./Start");
		V_Btn_Sets	= GetNode<Button>("./Options");
		V_Btn_Exit	= GetNode<Button>("./Exit");
	}

	public void F_LoadLVL_RNil(bool PAR_Exit_Bool)
	{
		if(PAR_Exit_Bool	== true)
		{
			GetTree().Quit();
		}

		Node V_Node_CurScene= GetTree().CurrentScene;
		GetTree().Root.AddChild(V_Scene_Interm);
		GetTree().Root.RemoveChild(V_Node_CurScene);
	}
}
