using Godot;
using System;

public partial class FloatingText : Label
{
	public float Speed = 30f;

	public float Lifetime = 1f;

	private Vector2 direction = new Vector2(0, -0.5f);
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var timer = new Timer();
		timer.WaitTime = Lifetime;
		timer.OneShot = true;
		timer.Timeout += () => QueueFree();
		AddChild(timer);
		timer.Start();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position += direction * Speed * (float)delta;
	}

	public void SetUp(string text, Color color)
	{
		Text = text;
		Modulate = color;
	}
}
