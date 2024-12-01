using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 100.0f;
	public const float Acceleration = 10.0f;
	public const float Deceleration = 10.0f;

	private Vector2 character_direction = Vector2.Zero;
	private Vector2 target_velocity = Vector2.Zero;

	private AnimatedSprite2D animatedSprite;

	private bool isAttacking = false; // Cờ để kiểm tra trạng thái tấn công

	private bool isHitting = false;
	private bool isKnockedBack = false; // Cờ để kiểm tra trạng thái knockback
	private float knockbackDuration = 0.1f; // Thời gian knockback
	private float knockbackSpeed = 200.0f; // Tốc độ knockback
	private Vector2 knockbackDirection; // Hướng knockback

	private int maxHealth = 100;
	private int currentHealth;

	private TextureProgressBar healthBar;

	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		healthBar = GetNode<TextureProgressBar>("HealthBar");

		GD.Print(healthBar)

		animatedSprite.AnimationFinished += OnAnimationFinished;

		// Khởi tạo máu
		currentHealth = maxHealth;

	}

	public override void _PhysicsProcess(double delta)
	{
		if (isAttacking || isKnockedBack)
		{
			// Không di chuyển khi đang tấn công hoặc bị knockback
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

	public void OnHit(int amount)
	{
		if (!isHitting)
		{
			isHitting = true;
			GD.Print("duma Đao!");
			animatedSprite.Play("hit");

			ChangeHealth(amount);

			GD.Print("Máu hiện tại: " + currentHealth);

			// Tạo hiệu ứng knockback
			KnockBack();
		}
	}

	private void KnockBack()
	{
		if (!isKnockedBack)
		{
			isKnockedBack = true;

			// Tính toán hướng knockback (ngược lại với hướng của player)
			knockbackDirection = -character_direction.Normalized();

			// Áp dụng knockback lên velocity
			Velocity = knockbackDirection * knockbackSpeed;

			// Sau thời gian knockback, quay lại trạng thái bình thường
			var timer = new Timer();
			AddChild(timer);
			timer.WaitTime = knockbackDuration;
			timer.OneShot = true;
			timer.Timeout += () => 
			{
				isKnockedBack = false;
				timer.QueueFree();
			};
			timer.Start();
		}
	}

	private void UpdateAnimation()
	{
		if (isAttacking)
		{
			// Không thay đổi animation nếu đang tấn công
			return;
		}

		if (isHitting)
		{
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

		if (finishedAnimation == "hit")
		{
			isHitting = false;
		}
	}

	private void UpdateHealthBar()
	{
        if (healthBar != null)
		{
            healthBar.Value = currentHealth;
        }
    }

	public void ChangeHealth(int amount)
	{
        currentHealth += amount;

        if (currentHealth > maxHealth)
		{
            currentHealth = maxHealth;
        }

        if (currentHealth < 0)
		{
            currentHealth = 0;
        }

        UpdateHealthBar();

		if (currentHealth == 0)
		{
			GD.Print("Player đã chết!");
		}
    }
}
