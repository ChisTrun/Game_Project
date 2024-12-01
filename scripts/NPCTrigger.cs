using Godot;
using System;

public partial class NPCTrigger : Area2D
{
	[Export] public NodePath NPCPath; // Đường dẫn tới NPC
	private NPC npcNode;
	[Export] public NodePath DialogueUIPath; // Đường dẫn tới Dialogue UI
	private CanvasLayer dialogueUI;
	private Label dialogueLabel;
	
	public override void _Ready()
	{
		// Lấy node NPC từ đường dẫn
		npcNode = GetNode<NPC>(NPCPath);
		dialogueUI = GetNode<CanvasLayer>(DialogueUIPath);
		dialogueLabel = dialogueUI.GetNode<Label>("Label");
		dialogueUI.Visible = false;
	}

	private void _on_body_entered(Node body)
	{
		if (body is Player)
		{
			GD.Print("Player entered trigger zone!");
			dialogueUI.Visible = true;
			dialogueLabel.Text = npcNode.GetDialogue();;
		}
	}

	private void _on_body_exited(Node body)
	{
		if (body is Player)
		{
			GD.Print("Player exited trigger zone!");
			dialogueUI.Visible = false;
		}
	}

}
