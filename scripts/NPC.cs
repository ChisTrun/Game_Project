using Godot;
using System;

public partial class NPC : Node2D
{
	private Label label;
	[Export] public string DialogueText = "Hello, Adventurer!";

	private AnimatedSprite2D _animatedSprite2D;

	public override void _Ready()
	{
		label = GetNode<Label>("Label");
		//Visible = false;
		label.Visible = false;

		_animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_animatedSprite2D.Play("idle");


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
