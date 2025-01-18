using Godot;

public partial class RangedWeapon : Weapon
{

	
	[Export]
	public PackedScene ProjectileScene { get; set; } // Scene của đạn

	[Export]
	public float ProjectileSpeed { get; set; } = 300f; // Vận tốc đạn, mặc định là 300

	[Export]
	public Vector2 BulletSpawnOffset { get; set; } = Vector2.Zero; // Offset khi tạo đạn

	private bool _canShoot = true;

	public override void _Process(double delta)
	{
		Vector2 mousePosition = GetGlobalMousePosition();

		// Tính toán hướng từ đối tượng đến chuột
		Vector2 direction = mousePosition - GlobalPosition;

		// Xoay đối tượng theo góc của hướng
		int flip = 1;
		Rotation = direction.Angle();
		if (mousePosition.X < GlobalPosition.X)
		{
			flip = -1;
		}

		Scale = new Vector2(Scale.X, flip * Mathf.Abs(Scale.Y));
	}

	public override void Use(Vector2 targetPosition)
	{
		if (!_canShoot)
		{
			GD.Print("Cannot shoot: Weapon is cooling down.");
			return;
		}

		if (ProjectileScene == null)
		{
			GD.PrintErr("Cannot shoot: ProjectileScene is null!");
			return;
		}

		_canShoot = false;

		// Lấy vị trí chuột toàn cục
		Vector2 mousePosition = GetGlobalMousePosition();
		GD.Print("Mouse Position: " + mousePosition);

		// Tính toán vị trí spawn của viên đạn (sử dụng Position thay vì GlobalPosition)
		Vector2 spawnPosition = Position + BulletSpawnOffset;

		// Kiểm tra tọa độ spawn trước khi tạo viên đạn
		GD.Print("Spawn Position: " + spawnPosition);

		// Tạo viên đạn
		Bullet projectile = (Bullet)ProjectileScene.Instantiate();

		projectile.damge = this.Damage / 2;

		// Đặt vị trí viên đạn
		projectile.Position = spawnPosition;
		GetParent().AddChild(projectile);

		if (AttackSound != null)
		{	
			var audioPlayer = new AudioStreamPlayer2D
			{
				Stream = AttackSound,
				Position = spawnPosition 
			};
			GetParent().AddChild(audioPlayer);
			audioPlayer.Play();
		}

		// Gán hướng cho viên đạn
		if (projectile is Bullet bulletScript)
		{
			bulletScript.Direction = (mousePosition - GlobalPosition).Normalized(); // Dùng vị trí chuột
			bulletScript.speed = ProjectileSpeed;
		}

		// Hẹn thời gian hồi chiêu
		Timer timer = new Timer();
		timer.WaitTime = Cooldown;
		timer.OneShot = true;
		timer.Timeout += () => _canShoot = true;
		AddChild(timer);
		timer.Start();

		GD.Print("Fired a ranged weapon!");
	}

}
