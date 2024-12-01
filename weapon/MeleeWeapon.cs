using System;
using Godot;

public partial class MeleeWeapon : Weapon
{
	private AnimationPlayer _animationPlayer;
	   // Biến lưu giá trị Rotation ban đầu

	public override void _Ready()
	{
		// Gán tham chiếu tới AnimationPlayer
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		if (_animationPlayer == null)
		{
			GD.PrintErr("AnimationPlayer not found on MeleeWeapon!");
		} else {
	 	   _animationPlayer.AnimationFinished += OnAnimationFinished;
		}
	}

	private void OnAnimationFinished(StringName animName)
	{

		if (animName == "attack")
		{
			_animationPlayer.Play("RESET");
		}
	}


	public override void _Process(double delta)
	{
		// Lấy vị trí của con trỏ chuột trong thế giới
		Vector2 mousePosition = GetGlobalMousePosition();

		// Tính góc giữa vị trí vũ khí và vị trí con trỏ
		Vector2 direction = mousePosition - GlobalPosition;
		Rotation = direction.Angle();
	}

	public override void Use(Vector2 targetPosition)
	{
		if (_animationPlayer != null)
		{
			// Chơi animation tấn công
			_animationPlayer.Play("attack");
		}
		else
		{
			GD.PrintErr("Cannot play animation: AnimationPlayer is null!");
		}
	}

}
