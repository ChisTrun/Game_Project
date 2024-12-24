using Godot;
using System;

public partial class HealthBar : TextureProgressBar
{
	private Player player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetNode<Player>("../../../../Player");

		if (player == null)
		{
			GD.PrintErr("Player not found");
		}

		this._Update();
	}

	public void _Update()
	{
		if (player != null)
		{
			this.Value = player.currentHealth * 100 / player.maxHealth;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
