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
	private float knockbackSpeed = 200.0f; // Tốc độ knockback\

	private Weapon weapon;
	private Vector2 knockbackDirection; // Hướng knockback

	[Export]
	public int maxHealth = 82;
	[Export]
	public int currentHealth;

	private TextureProgressBar healthBar;

	private Weapon GetWeaponFromChildren()
	{
		// Duyệt qua tất cả các node con và kiểm tra xem node đó có phải là lớp con của Weapon không
		foreach (var child in GetChildren())
		{
			if (child is Weapon weapon)
			{
				return weapon;
			}
		}
		return null; // Trả về null nếu không tìm thấy node Weapon nào
	}
	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		healthBar = GetNode<TextureProgressBar>("HealthBar");

		GD.Print(healthBar.Value);

		animatedSprite.AnimationFinished += OnAnimationFinished;

		// Khởi tạo máu
		currentHealth = maxHealth;

		weapon = GetWeaponFromChildren();

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
		UpdateDirectionToMouse();
	}

	private void UpdateDirectionToMouse()
	{
		// Lấy vị trí chuột và vị trí player
		Vector2 mousePosition = GetGlobalMousePosition();
		Vector2 playerPosition = GlobalPosition;

		// Kiểm tra hướng của chuột so với player
		if (mousePosition.X < playerPosition.X)
		{
			animatedSprite.FlipH = true; // Lật player theo chiều ngang
		}
		else
		{
			animatedSprite.FlipH = false; // Không lật
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			if (mouseEvent.ButtonIndex == MouseButton.Left)
			{
				// Chơi animation tấn công
				PlayerAttack();
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

			GD.Print("Máu hiện tại: " + currentHealth + "/" + maxHealth);

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
		}
	}

	private void PlayerAttack()
	{
		if (!isAttacking)
		{
			isAttacking = true;
			if (weapon != null) {
				weapon.Use(new Vector2());
				isAttacking = false;
			} else {
				animatedSprite.Play("attack");
			}

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

	public void ChangeHealth(int amount)
	{
		currentHealth += amount;

		if (currentHealth > maxHealth)
		{
			currentHealth = maxHealth;
		}

		if (currentHealth < 13)
		{
			currentHealth = 13;
		}

		healthBar.Value = currentHealth;

		if (currentHealth == 13)
		{
			GD.Print("Player đã chết!");
		}
	}
}
