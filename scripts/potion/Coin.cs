using Godot;
using System;

public partial class Coin : Area2D
{
	public int Value = 1;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node body)
	{
		if (body is Player player)
		{
			player.ChangeCoin(Value);
			QueueFree();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
