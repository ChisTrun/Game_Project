using Godot;
using NewGameProject.scripts.Skills;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody2D
{
	public float Speed = Global.PlayerSpeed;
	public float Acceleration = Global.PlayerAcceleration;
	public float Deceleration = Global.PlayerDeceleration;

	public float BaseDamage = Global.PlayerBaseDamage;

	public Vector2 character_direction = Vector2.Zero;
	private Vector2 target_velocity = Vector2.Zero;

	private AnimatedSprite2D animatedSprite;

	private List<Weapon> weaponList;

	private bool isAttacking = false; // Cờ để kiểm tra trạng thái tấn công

	private bool isHitting = false;
	private bool isKnockedBack = false; // Cờ để kiểm tra trạng thái knockback
	private float knockbackDuration = 0.1f; // Thời gian knockback
	private float knockbackSpeed = 200.0f; // Tốc độ knockback\

	private Weapon weapon;
	private Vector2 knockbackDirection; // Hướng knockback

	[Export]
	public int maxHealth = Global.PlayerMaxHealth;
	[Export]
	public float currentHealth;

	private TextureProgressBar healthBar;

	private bool isDead = false;

	private Label goldLabel;

	[Signal]
	public delegate void OnHealthChangedEventHandler(int amount);

	[Signal]
	public delegate void OnCoinChangedEventHandler(int amount);
	
	private float fadeInTime = 1.5f;
	private float elapsedTime = 0f;
	private bool isFading = false;

	public bool isDashing = false;
	public float dashCooldown = Global.PlayerSkillCD;
	public float dashSpeed = 280.0f;
	public float dashDuration = 0.2f;
	public float dashTimer = 0.0f;
	public AnimatedSprite2D dashEffect;

	private void GetWeaponFromChildren()
	{
		weaponList = new List<Weapon>();

		foreach (var child in GetChildren())
		{
			if (child is Weapon weapon)
			{
				weaponList.Add(weapon);
				weapon.Visible = false;
				weapon.SetPhysicsProcess(false);
				weapon.SetProcess(false);
			}
		}

		if (weaponList.Count > 0)
		{
			weapon = weaponList[0];
			weapon.Visible = true;
			weapon.SetPhysicsProcess(true);
			weapon.SetProcess(true);
		}
	}

	public override void _Ready()
	{
		Global.PlayerInstance = this;
		Global.SyncToPlayer(this);
		Global.InitializeSkills();

		GD.Print(Global.Skills["Dash"].IsActive);

		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		healthBar = GetNode<TextureProgressBar>("HealthBar/CanvasLayer/HealthBar2");
		animatedSprite.AnimationFinished += OnAnimationFinished;
		goldLabel = GetNode<Label>("GoldLabel");
		goldLabel.ZIndex = 10;
		dashEffect = GetNode<AnimatedSprite2D>("DashEffect"); // Đường dẫn đến node hiệu ứng
		dashEffect.Visible = false;

		// Khởi tạo máu
		currentHealth = Global.CurrentHealth;
		maxHealth = Global.PlayerMaxHealth;
		healthBar.MaxValue = maxHealth;
		healthBar.Value = currentHealth;

		GetWeaponFromChildren();
		if (weaponList.Count > 0)
		{
			weapon = weaponList[0];
		}

		UpdateGoldLabel();
		// Kết nối signal với FloatingTextManager
		var manager = GetNode<FloatingTextManager>("/root/FloatingTextManager");

		Connect(nameof(OnHealthChanged), Callable.From<int>((amount) =>
		{
			manager.ShowFloatingText(GlobalPosition, (amount > 0 ? "+" : "") + amount, amount > 0 ? Colors.Green : Colors.Red);
		}));

		Connect(nameof(OnCoinChanged), Callable.From<int>((amount) =>
		{
			manager.ShowFloatingText(GlobalPosition, "+" + amount, Colors.Yellow);
		}));

		Speed = Global.PlayerSpeed;
		Acceleration = Global.PlayerAcceleration;
		Deceleration = Global.PlayerDeceleration;
		BaseDamage = Global.PlayerBaseDamage;
		dashCooldown = Global.PlayerSkillCD;
		GlobalPosition = Global.PlayerPosition;
	}

	public override void _ExitTree()
	{
		Global.SyncFromPlayer(this);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (isDead)
		{
			return;
		}

		if (isAttacking || isKnockedBack)
		{
			// Không di chuyển khi đang tấn công hoặc bị knockback
			return;
		}

		if (isDashing)
		{
			MoveAndSlide();
			return; // Không thực hiện các hành vi khác trong khi Dash
		}

		if (dashTimer > 0.0f)
		{
			dashTimer -= (float)delta;
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
		if (@event is InputEventKey keyEvent && keyEvent.Pressed)
		{
			if (keyEvent.Keycode == Key.E)
			{
				// Chuyển vũ khí khi nhấn phím E
				SwitchWeapon();
			}
			if (keyEvent.Keycode == Key.P)
			{
				var shop = GetNode<Shop>("../../Shop");
				if (shop != null)
				{
					if (shop.Visible)
					{
						shop.Visible = false;
						/*GetTree().Paused = false;*/
					}
					else
					{
						shop.Visible = true;
						/*GetTree().Paused = true;*/
					}
				}
				else
				{
					GD.Print("Shop node not found!");
				}
			}
			if (keyEvent.Keycode == Key.Space && Global.Skills.ContainsKey("Dash"))
			{
				var dashSkill = Global.Skills["Dash"];

				if (dashSkill.IsActive && !isDashing && dashTimer <= 0.0f)
				{
					dashSkill.Use();

					// Tìm SkillBar và gọi HighlightSkill
					var skillBar = GetNode<SkillBar>("../../SkillBar");
					if (skillBar != null)
					{
						skillBar.HighlightSkill("Dash");
					}
					else
					{
						GD.Print("SkillBar node not found!");
					}
				}
				else
				{
					GD.Print("Cannot use Dash: Skill not active, cooldown, or already dashing.");
				}
			}

		}

	}

	public void OnHit(float amount)
	{
		if (!isHitting)
		{
			isHitting = true;
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
		if (!isAttacking && !isDead)
		{
			isAttacking = true;
			if (weapon != null)
			{
				Vector2 mousePosition = GetGlobalMousePosition();
				weapon.Use(mousePosition);
				isAttacking = false;
			}
			else
			{
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

	public void ChangeHealth(float amount)
	{
		if (isDead)
		{
			return;
		}
		currentHealth += amount;
		Global.CurrentHealth = currentHealth;
		GD.Print("Máu sau khi thay đổi: " + currentHealth + "/" + maxHealth);

		if (currentHealth > maxHealth)
		{
			currentHealth = maxHealth;
		}

		if (currentHealth < 0)
		{
			currentHealth = 0;
		}

		healthBar.Value = currentHealth;

		EmitSignal(SignalName.OnHealthChanged, amount);

		if (currentHealth == 0)
		{
			GD.Print("Player đã chết!");
			Die();
		}
	}

private void Die()
{
	isDead = true; // Đánh dấu Player đã chết

	// Chuyển animation sang "death"
	animatedSprite.Play("death");

	GD.Print("Player đã chết!");

	// Đợi animation "death" kết thúc rồi xử lý tiếp
	animatedSprite.AnimationFinished += OnDeathAnimationFinished;

	CanvasLayer gameover = GetNode<CanvasLayer>("../../NoticeUI");
	Label winLabel = gameover.GetNode<Label>("WinLabel");
	Label gameOverLabel = gameover.GetNode<Label>("GameOverLabel");

	// Ẩn nhãn Win và đặt alpha cho GameOverLabel về 0 (trong suốt)
	winLabel.Visible = false;
	gameover.Visible = true;
	gameOverLabel.Visible = true;
	
	var audioPlayer = gameover.GetNodeOrNull<AudioStreamPlayer>("GameOverMusic");
	if (audioPlayer == null)
	{
		audioPlayer = new AudioStreamPlayer();
		gameover.AddChild(audioPlayer);
		audioPlayer.Stream = ResourceLoader.Load<AudioStream>("res://OutAssets/sound/playerDie.wav"); // Đường dẫn đến nhạc
	}
	var audioPlayerTheme = GetNode<AudioStreamPlayer2D>("../../AudioStreamPlayer2D");
	audioPlayerTheme.Stop();
	audioPlayer.Play();

	// Bắt đầu hiệu ứng fade-in
	isFading = true;
	elapsedTime = 0; // Reset thời gian

	

	// Tạo Timer để hiển thị menu sau 5 giây
	Timer timer = new Timer();
	timer.WaitTime = 5; // 5 giây
	timer.OneShot = true;
	timer.Autostart = true;
	AddChild(timer);

	timer.Timeout += () =>
	{
		CanvasLayer menu = GetNode<CanvasLayer>("../../Menu");
		menu.GetNode<Label>("./GameOverLabel").Visible = true;
		menu.GetNode<Button>("./Button/Continue").Disabled = true;
		menu.GetNode<Button>("./Button/Setting").Disabled = true;
		menu.Visible = true;
		GD.Print("Menu displayed!");
	};
}

public override void _Process(double delta)
{
	if (isFading)
	{
		elapsedTime += (float)delta;
		float alpha = Mathf.Clamp(elapsedTime / fadeInTime, 0, 1);

		// Lấy tất cả các con trong CanvasLayer và áp dụng fade-in
		CanvasLayer gameover = GetNode<CanvasLayer>("../../NoticeUI");
		foreach (Node child in gameover.GetChildren())
		{
			if (child is Control control)
			{
				control.Modulate = new Color(1, 1, 1, alpha);
			}
		}

		// Kết thúc fade-in
		if (elapsedTime >= fadeInTime)
		{
			isFading = false;
		}
	}
}


	private void OnDeathAnimationFinished()
	{
		if (animatedSprite.Animation == "death")
		{
			// Hiển thị thông báo kết thúc game
			GD.Print("Game Over!");
			
			// Ví dụ: Kết thúc game hoặc chuyển scene
			GetTree().Paused = true;
			// GetTree().ChangeScene("res://Scenes/GameOver.tscn"); // Chuyển đến màn hình Game Over
		}
	}

	public void ChangeCoin(int amount)
	{
		Global.Gold += amount;
		UpdateGoldLabel();
		EmitSignal(SignalName.OnCoinChanged, amount);
	}

	private void UpdateGoldLabel()
	{
		if (goldLabel != null)
		{
			goldLabel.Text = $"Gold: {Global.Gold}";
		}
	}

	private void SwitchWeapon()
	{
		if (weaponList.Count == 0)
			return;

		int currentIndex = weaponList.IndexOf(weapon);

		int nextIndex = (currentIndex + 1) % weaponList.Count;

		if (weapon != null)
		{
			weapon.Visible = false;
			weapon.SetPhysicsProcess(false);
			weapon.SetProcess(false);
		}

		weapon = weaponList[nextIndex];
		weapon.Visible = true;
		weapon.SetPhysicsProcess(true);
		weapon.SetProcess(true);

		GD.Print($"Switched to weapon: {weapon.Name}");
	}
}
