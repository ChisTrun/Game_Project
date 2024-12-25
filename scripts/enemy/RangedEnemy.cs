using Godot;
using System;
using System.ComponentModel.DataAnnotations.Schema;

public partial class RangedEnemy : BaseEnemy
{
	[Export] public float ShootDistance = 100f; // Khoảng cách tối đa để bắn
	[Export] public float RetreatDistance = 50f; // Khoảng cách tối thiểu để giữ an toàn
	[Export] public PackedScene BulletScene { get; set; } // Scene đạn được truyền từ editor
	[Export] public Vector2 BulletSpawnOffset = Vector2.Zero; // Offset để đạn xuất phát đúng từ enemy

	private bool _canShoot = true;

	public override void _Ready()
	{
		base._Ready();

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

		// Tạo viên đạn
		Node2D bullet = (Node2D)BulletScene.Instantiate();

		// Đặt vị trí viên đạn (cộng thêm offset nếu cần)
		bullet.Position = Position + BulletSpawnOffset;
		GetParent().AddChild(bullet);

		// Gán hướng cho viên đạn
		if (bullet is Bullet bulletScript)
		{
			// Tính toán hướng từ enemy tới player
			bulletScript.Direction = (_player.Position - Position).Normalized();
		}

		// Hẹn thời gian hồi chiêu
		Timer timer = new Timer();
		timer.WaitTime = AttackCooldown;
		timer.OneShot = true;
		timer.Timeout += () => _canShoot = true;
		AddChild(timer);
		timer.Start();
	}
}
