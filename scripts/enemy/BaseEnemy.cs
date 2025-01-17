using Godot;
using System;

public abstract partial class BaseEnemy : CharacterBody2D
{
	protected Node2D _player;

	[Export] public AudioStream AttackSound { get; set; }
	[Export] public AudioStream HitSound { get; set; }
	[Export] public AudioStream DeadSound { get; set; }
	[Export] public PackedScene MainScene2;


	[Export] public int Damage = 10;
	[Export] public float Speed = 100f;
	[Export] public float VisionRange = 100f;
	[Export] public int MaxHealth = 100;
	[Export] public float AttackCooldown = 2.0f;
	[Export] public PackedScene CoinScene;
	[Export] public PackedScene HealthPotionScene;
	[Export] public float RunSoundCooldown = 0.5f; // Cooldown for run sound

	private TextureProgressBar _healthBar;
	private int _currentHealth;
	protected bool IsAttacking = false;
	protected bool IsDead = false;
	protected bool isTakeDamage = false;
	protected AnimatedSprite2D _animatedSprite2D;

	private double _lastRunSoundTime = 0.0; // Last time the run sound was played

	public void CreateSound(AudioStream sound, float cooldown = 0f,  double lastSoundTime = 0)
	{
		double currentTime = Time.GetTicksMsec() / 1000.0;

		if (sound != null && (currentTime - lastSoundTime >= cooldown))
		{
			AudioStreamPlayer2D audioStreamPlayer2D = new AudioStreamPlayer2D();
			audioStreamPlayer2D.Stream = sound;
			GetParent().AddChild(audioStreamPlayer2D);
			audioStreamPlayer2D.Play();
			lastSoundTime = currentTime;
		}
	}

	public void DropLoot()
	{
		Random random = new Random();

		for (int i = 0; i < 5; i++)
		{
			PackedScene lootScene = random.Next(0, 2) == 0 ? CoinScene : HealthPotionScene;
			if (lootScene == null) continue;

			Node2D loot = (Node2D)lootScene.Instantiate();
			loot.Position = Position + new Vector2(random.Next(-20, 20), random.Next(-20, 20)); // Rải vật phẩm gần vị trí enemy
			GetParent().AddChild(loot);
		}
	}

	public override void _Ready()
	{
		_player = GetParent().GetNode<CharacterBody2D>("Player");
		_animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_currentHealth = MaxHealth;

		if (_animatedSprite2D != null)
		{
			_animatedSprite2D.AnimationFinished += OnDeathAnimationFinished;
		}

		_healthBar = GetNode<TextureProgressBar>("EnemyHealthBar");
		if (_healthBar != null)
		{
			_healthBar.MaxValue = MaxHealth;
			_healthBar.Value = _currentHealth;
		}
	}

	protected virtual void OnDeathAnimationFinished()
	{
		if (_animatedSprite2D.Animation == "death")
		{
			QueueFree();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_player == null || IsDead) return;

		FacePlayer();

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
		MoveAndSlide();
	}

	protected abstract Vector2 PerformBehavior(double delta);

	public virtual void TakeDamage(int damage)
	{
		if (IsDead) return;
		isTakeDamage = true;
		_currentHealth -= damage;

		CreateSound(HitSound);

		if (_healthBar != null)
		{
			_healthBar.Value = _currentHealth;
		}

		if (_currentHealth <= 0)
		{
			Die();
			DropLoot();
		}
	}

	protected virtual void Die()
	{
		IsDead = true;
		CreateSound(DeadSound);
		_animatedSprite2D.Play("death");
		Timer timer = new Timer();
		timer.WaitTime = 5; // 5 giây
		timer.OneShot = true;
		timer.Autostart = true;
		AddChild(timer);
		GD.Print("Timer Started!");

		timer.Timeout += () =>
		{
			GD.Print("Chuyển 1");
			if (MainScene2 != null)
			{
				GD.Print("Chuyển");
				GetTree().ChangeSceneToPacked(MainScene2); // Chuyển đến Scene Main
				
			}
			else
			{
				GD.PrintErr("MainScene is not set! Please assign it in the inspector.");
			}
		};
	}

	protected void FacePlayer()
	{
		if (_player == null || _animatedSprite2D == null) return;
		_animatedSprite2D.FlipH = _player.Position.X < Position.X;
	}

	protected Vector2 GetDirectionTowards(Vector2 targetPosition)
	{
		return (targetPosition - Position).Normalized() * Speed;
	}

	protected Vector2 GetDirectionAwayFrom(Vector2 targetPosition)
	{
		float distance = Position.DistanceTo(targetPosition);
		if (distance < 5.0f)
		{
			return Vector2.Zero;
		}
		return (Position - targetPosition).Normalized() * Speed;
	}
}
