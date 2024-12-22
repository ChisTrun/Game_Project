using Godot;
using System;

public partial class HealthBar : TextureProgressBar
{
	private Player player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetTree().Root.GetNodeOrNull<Player>("world/Player");

		if (player == null)
		{
			GD.PrintErr("Player not found");
			return;
		}

		this._Update();
	}

	public void _Update()
	{
		this.Value = player.currentHealth * 100 / player.maxHealth;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
