using Godot;
using System;
using System.Collections.Generic;

public partial class GameController : Control
{
	Button V_Btn_Play;
	Button V_Btn_Score;
	Button V_Btn_Exit;

	List<PackedScene> V_SceneList_Games;
	PackedScene	V_Scene_Interm;
	PackedScene V_Scene_Death;

	public override void _Ready()
	{
		V_Scene_Interm	= ResourceLoader.Load<PackedScene>("res://Scenes/Intermission.tscn");
		V_Scene_Death	= ResourceLoader.Load<PackedScene>("res://Scenes/End.tscn");

		V_Btn_Play	= GetNode<Button>("./Start");
		//v_Btn_Score	= GetNode<Button>("./Start");
		V_Btn_Exit	= GetNode<Button>("./Exit");
	}
}
