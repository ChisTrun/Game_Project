using Godot;
using System;

public partial class FloatingTextManager : Node
{
	private PackedScene floatingTextScene;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		floatingTextScene = (PackedScene)ResourceLoader.Load("res://scenes/floatingText/FloatingText.tscn");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void ShowFloatingText(Vector2 position, string text, Color color)
	{
		if (floatingTextScene != null)
		{
			var floatingText = floatingTextScene.Instantiate<Label>();
			GetTree().Root.AddChild(floatingText);
			floatingText.GlobalPosition = position;
			/*floatingText.SetUp(text, color);*/
			floatingText.Text = text;
			floatingText.Modulate = color;
		}
	}
}
