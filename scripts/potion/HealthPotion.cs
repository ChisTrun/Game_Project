using Godot;
using System;

public partial class HealthPotion : Area2D
{
	public int HealAmount = 10;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnBodyEntered(Node body)
	{
		// Kiểm tra nếu đối tượng là Player
		if (body is Player player)
		{
			// Hồi máu cho Player
			player.ChangeHealth(HealAmount);

			GD.Print($"Player đã hồi {HealAmount} máu!");

			// Xóa bình máu sau khi nhặt
			QueueFree();
		}
	}
}
