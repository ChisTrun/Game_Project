using Godot;
using System;

public abstract partial class BaseEnemy : CharacterBody2D
{
	protected Node2D _player;
	[Export] public int Damage = 10;
	[Export] public float Speed = 100f; // Tốc độ di chuyển cơ bản
	[Export] public float VisionRange = 100f; // Tốc độ di chuyển cơ bản
	[Export] public int MaxHealth = 100; // Máu tối đa
	[Export] public float AttackCooldown = 2.0f; // Thời gian hồi giữa các lần bắn
	[Export] public PackedScene CoinScene; // Scene vàng
	[Export] public PackedScene HealthPotionScene; // Scene bình máu
	private TextureProgressBar _healthBar;

	private int _currentHealth;
	protected bool IsAttacking = false;
	protected bool IsDead = false;

	protected bool isTakeDamage = false;
	protected AnimatedSprite2D _animatedSprite2D;

	public override void _Ready()
	{
		_player = GetParent().GetNode<CharacterBody2D>("Player");
		_animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_currentHealth = MaxHealth; // Khởi tạo máu hiện tại bằng máu tối đa

		if (_animatedSprite2D != null)
		{
			// Đăng ký signal AnimationFinished để biết khi animation chết đã hoàn tất
			_animatedSprite2D.AnimationFinished += OnDeathAnimationFinished;
		}
		
		_healthBar = GetNode<TextureProgressBar>("EnemyHealthBar");
		if (_healthBar != null)
		{
			_healthBar.MaxValue = MaxHealth;
			_healthBar.Value = _currentHealth;
		}
	}

	private void OnDeathAnimationFinished()
	{
		if (_animatedSprite2D.Animation == "death")
		{
			GD.Print("Death animation finished, removing the enemy.");
			QueueFree(); // Xóa node khỏi scene sau khi animation death kết thúc
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_player == null) return;
		if (IsDead) return;
		FacePlayer(); // Lật hướng về phía người chơi

		Vector2 velocity = PerformBehavior(delta);
		if (velocity != Vector2.Zero && !IsAttacking)
		{
			_animatedSprite2D.Play("run");
		}
		else if (velocity == Vector2.Zero && !IsAttacking)
		{
			_animatedSprite2D.Play("idle");
		}

		Velocity = velocity;

		var collision = GetLastSlideCollision();
		if (collision != null)
		{
			// GD.Print($"Collided with: {collision.GetCollider()}");
		}
		MoveAndSlide();
	}

	protected abstract Vector2 PerformBehavior(double delta);

	// Nhận sát thương
	public virtual void TakeDamage(int damage)
	{
		if (IsDead) return;
		isTakeDamage = true;
		_currentHealth -= damage;
		GD.Print($"{Name} took {damage} damage. Current health: {_currentHealth}");

		// Cập nhật thanh máu
		if (_healthBar != null)
		{
			_healthBar.Value = _currentHealth;
		}

		if (_currentHealth <= 0)
		{
			Die();
		}

	}


	// Hành động khi kẻ địch chết
	protected virtual void Die()
	{
		IsDead = true;
		GD.Print($"{Name} died.");
		_animatedSprite2D.Play("death");
		// Để việc xóa đối tượng được xử lý sau khi animation death hoàn tất
	}


	// Lật hướng về phía người chơi
	protected void FacePlayer()
	{
		if (_player == null || _animatedSprite2D == null) return;

		// Kiểm tra vị trí người chơi so với kẻ địch và lật hình
		_animatedSprite2D.FlipH = _player.Position.X < Position.X;
	}

	// Phương thức chung để di chuyển về hướng mục tiêu
	protected Vector2 GetDirectionTowards(Vector2 targetPosition)
	{
		return (targetPosition - Position).Normalized() * Speed;
	}

	// Phương thức chung để lùi khỏi mục tiêu
	protected Vector2 GetDirectionAwayFrom(Vector2 targetPosition)
	{
		float distance = Position.DistanceTo(targetPosition);
		// GD.Print($"Distance to player: {distance}");

		if (distance < 5.0f) // 5.0f là ngưỡng tối thiểu
		{
			return Vector2.Zero; // Không di chuyển
		}

		// GD.Print((Position - targetPosition).Normalized() * Speed);

		return (Position - targetPosition).Normalized() * Speed;
	}
}
