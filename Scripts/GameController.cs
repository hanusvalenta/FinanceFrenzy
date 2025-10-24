using Godot;
using System;
using System.Collections.Generic;

public partial class GameController : Control
{
	Button V_Btn_Play;
	Button V_Btn_Sets;
	Button V_Btn_Exit;

	List<PackedScene> V_SceneList_Games;
	PackedScene	V_Scene_Interm;
	PackedScene V_Scene_Death;

	public override void _Ready()
	{
		GetTree().Root.SetMeta("Sanity", 100);

		V_Scene_Interm	= ResourceLoader.Load<PackedScene>("res://Scenes/Intermission.tscn");
		V_Scene_Death	= ResourceLoader.Load<PackedScene>("res://Scenes/End.tscn");

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

		GetTree().CurrentScene.RemoveChild(GetTree().CurrentScene);
		GetTree().CurrentScene.AddChild(V_Scene_Interm.Instantiate());
	}
}
