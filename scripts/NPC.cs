using Godot;
using System;

public partial class NPC : Node2D
{
	private Label label;
	[Export] public string DialogueText = "Hello, Adventurer!";

	public override void _Ready()
	{
		label = GetNode<Label>("Label");
		//Visible = false;
		label.Visible = false;
	}

	public void ShowNPC()
	{
		Visible = true;
		label.Visible = true;
	}

	public void HideNPC()
	{
		Visible = false;
		label.Visible = false;
	}
	
	public string GetDialogue()
	{
		return DialogueText;
	}
}
