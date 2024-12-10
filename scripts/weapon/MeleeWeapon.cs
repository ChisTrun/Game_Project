using System;
using Godot;

public partial class MeleeWeapon : Weapon
{
	private AnimationPlayer _animationPlayer;
	private Area2D _collisionArea; // Vùng phát hiện va chạm của vũ khí
	private AnimatedSprite2D _animatedSprite2D; // Vùng phát hiện va chạm của vũ khí

	public override void _Ready()
	{
		// Gán tham chiếu tới AnimationPlayer
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_collisionArea = GetNode<Area2D>("./Sprite2D/Area2D");
		_animatedSprite2D = GetNode<AnimatedSprite2D>("./Sprite2D/AnimatedSprite2D");

		if (_animationPlayer == null)
		{
			GD.PrintErr("AnimationPlayer not found on MeleeWeapon!");
		}
		else
		{
			_animationPlayer.AnimationFinished += OnAnimationFinished;
		}

		if (_collisionArea == null)
		{
			GD.PrintErr("CollisionArea not found on MeleeWeapon!");
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

		// Xử lý logic gây sát thương
		AttackEnemiesInRange();
	}

	private void AttackEnemiesInRange()
	{
		if (_collisionArea == null)
		{
			GD.PrintErr("Cannot detect collisions: CollisionArea is null!");
			return;
		}

		// Lấy danh sách các đối tượng trong vùng va chạm
		var overlappingBodies = _collisionArea.GetOverlappingBodies();

		foreach (var body in overlappingBodies)
		{
			if (body is BaseEnemy enemy)
			{
				_animatedSprite2D.Play("go");
				enemy.TakeDamage(10); // Gọi hàm gây sát thương, ví dụ 10 đơn vị sát thương
				GD.Print($"Damaged enemy: {enemy.Name}");
			}
		}
	}
}
