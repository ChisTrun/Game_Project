using Godot;
using System;
using System.Linq.Expressions;

public partial class Player : CharacterBody2D
{
	public const float Speed = 100.0f;
	public const float Acceleration = 10.0f;
	public const float Deceleration = 10.0f;

	private Vector2 character_direction = Vector2.Zero;
	private Vector2 target_velocity = Vector2.Zero;

	private AnimatedSprite2D animatedSprite;

	private bool isAttacking = false; // Cờ để kiểm tra trạng thái tấn công

	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		animatedSprite.AnimationFinished += OnAnimationFinished;
	}

	

	public override void _PhysicsProcess(double delta)
	{
		if (isAttacking)
		{
			return;
		}

		// Lấy đầu vào từ bàn phím
		character_direction.Y = Input.GetAxis("move_up", "move_down");
		character_direction.X = Input.GetAxis("move_left", "move_right");

		character_direction = character_direction.Normalized();
		target_velocity = character_direction * Speed;

		Velocity = Velocity.Lerp(target_velocity, (float)(Acceleration * delta));

		if (character_direction == Vector2.Zero)
		{
			Velocity = Velocity.Lerp(Vector2.Zero, (float)(Deceleration * delta));
		}

		MoveAndSlide();

		UpdateAnimation();
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			if (mouseEvent.ButtonIndex == MouseButton.Left)
			{
				// Chơi animation tấn công
				PlayAttackAnimation();
			}
		}
	}

	private void UpdateAnimation()
	{
	

		if (isAttacking)
		{
			// Không thay đổi animation nếu đang tấn công
			return;
		}

		if (character_direction == Vector2.Zero)
		{
			animatedSprite.Play("idle");
		}
		else
		{
			animatedSprite.Play("run");
			animatedSprite.FlipH = character_direction.X < 0;
		}
	}

	private void PlayAttackAnimation()
	{
		if (!isAttacking)
		{
			isAttacking = true;
			animatedSprite.Play("attack");
		}
	}


	private void OnAnimationFinished()
	{
		// Lấy tên animation vừa kết thúc
		string finishedAnimation = animatedSprite.Animation;

		if (finishedAnimation == "attack")
		{
			isAttacking = false;
		}
	}

}
