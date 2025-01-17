using Godot;
using System;
using System.ComponentModel.DataAnnotations.Schema;

public partial class RangedEnemy : BaseEnemy
{
	[Export] public float ShootDistance = 100f; // Khoảng cách tối đa để bắn
	[Export] public float RetreatDistance = 50f; // Khoảng cách tối thiểu để giữ an toàn
	[Export] public PackedScene BulletScene { get; set; } // Scene đạn được truyền từ editor
	[Export] public Vector2 BulletSpawnOffset = Vector2.Zero; // Offset để đạn xuất phát đúng từ enemy

	private float fadeInTime = 1.5f;
	private float elapsedTime = 0f;
	private bool isFading = false;

	private bool _canShoot = true;

	public override void _Ready()
	{
		base._Ready();
		// Hủy đăng ký tín hiệu nếu cần
		_animatedSprite2D.AnimationFinished -= base.OnDeathAnimationFinished;
		
		// Đăng ký lại với phương thức mới từ lớp con
		_animatedSprite2D.AnimationFinished += OnDeathAnimationFinished;
		// Kiểm tra xem BulletScene đã được gán hay chưa
		if (BulletScene == null)
		{
			GD.PrintErr("BulletScene is not assigned!");
		}
	}

	protected override Vector2 PerformBehavior(double delta)
	{
		if (_player == null)
			return Vector2.Zero;

		float distanceToPlayer = Position.DistanceTo(_player.Position);

		if (distanceToPlayer < RetreatDistance)
		{
			return GetDirectionAwayFrom(_player.Position);
		}
		else if (distanceToPlayer < ShootDistance)
		{
			if (_canShoot)
			{
				Shoot();
			}
			return GetDirectionAwayFrom(_player.Position) * 0.5f;
		}
		else if (distanceToPlayer < VisionRange)
		{
			// Lại gần player
			return GetDirectionTowards(_player.Position);
		}

		return Vector2.Zero; // Không làm gì nếu player ngoài tầm nhìn
	}

	private void Shoot()
	{
		if (BulletScene == null)
		{
			GD.PrintErr("Cannot shoot: BulletScene is null!");
			return;
		}

		_canShoot = false;

		Bullet bullet = (Bullet)BulletScene.Instantiate();

		bullet.damge = this.Damage;
		bullet.Position = Position + BulletSpawnOffset;
		GetParent().AddChild(bullet);


		CreateSound(AttackSound);

		if (bullet is Bullet bulletScript)
		{
			bulletScript.Direction = (_player.Position - Position).Normalized();
		}
		
		Timer timer = new Timer();
		timer.WaitTime = AttackCooldown;
		timer.OneShot = true;
		timer.Timeout += () => _canShoot = true;
		AddChild(timer);
		timer.Start();
	}
	
	
	
	protected override void Die()
	{
		IsDead = true;
		GD.Print($"{Name} died.");
		_animatedSprite2D.Play("death");
		
		CanvasLayer gameover = GetNode<CanvasLayer>("../../NoticeUI");
		Label winLabel = gameover.GetNode<Label>("WinLabel");
		Label gameOverLabel = gameover.GetNode<Label>("GameOverLabel");

		// Ẩn nhãn Win và đặt alpha cho GameOverLabel về 0 (trong suốt)
		winLabel.Visible = true;
		gameover.Visible = true;
		gameOverLabel.Visible = false;
		
		var audioPlayer = gameover.GetNodeOrNull<AudioStreamPlayer>("WinMusic");
		if (audioPlayer == null)
		{
			audioPlayer = new AudioStreamPlayer();
			gameover.AddChild(audioPlayer);
			audioPlayer.Stream = ResourceLoader.Load<AudioStream>("res://OutAssets/sound/winRound.wav"); // Đường dẫn đến nhạc
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
				// Để việc xóa đối tượng được xử lý sau khi animation death hoàn tất

	}
		protected override void OnDeathAnimationFinished()
	{
		//GD.Print("Death animation finished in SubEnemy, removing the enemy.");
		//QueueFree(); // Xóa node khỏi scene trong lớp con
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
}
