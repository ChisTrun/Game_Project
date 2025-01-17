using Godot;
using System;

public partial class Bullet : Node2D
{
	public Vector2 Direction { get; set; } = Vector2.Zero; // Đảm bảo có giá trị mặc định
	[Export] public int damge = 10;	
	[Export] public float speed = 200f;
	[Export] public bool isPlayerBullet = false; // Xác định đạn của người chơi hay kẻ địch
	[Export] public float maxDistance = 500f; // Khoảng cách tối đa đạn có thể bay trước khi biến mất
	private Vector2 _spawnPosition; // Lưu vị trí xuất phát của đạn
	private AnimatedSprite2D _animatedSprite;
	private Area2D _area2D;

	public override void _Ready()
	{
		// Lưu vị trí spawn
		_spawnPosition = Position;

		// Lấy AnimatedSprite2D
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		if (_animatedSprite == null)
		{
			GD.PrintErr("AnimatedSprite2D not found in Bullet!");
		}
		else
		{
			_animatedSprite.Play("go");
		}

		// Lấy Area2D và kết nối tín hiệu body_entered
		_area2D = GetNode<Area2D>("Area2D");

		if (_area2D != null)
		{
			_area2D.BodyEntered += _On_Body_Entered;
		}
		else
		{
			GD.PrintErr("Area2D not found in Bullet!");
		}
	}

	public override void _Process(double delta)
	{
		// Di chuyển đạn
		Position += Direction * speed * (float)delta;

		// Tính khoảng cách từ điểm spawn đến vị trí hiện tại
		float distanceTravelled = _spawnPosition.DistanceTo(Position);

		// Kiểm tra nếu đạn đã bay hết khoảng cách tối đa
		if (distanceTravelled >= maxDistance)
		{
			QueueFree(); // Xóa đạn
		}
	}

	// Hàm xử lý va chạm
	private void _On_Body_Entered(Node body)
	{
		if (isPlayerBullet)
		{
			// Kiểm tra nếu đạn thuộc về người chơi, chỉ gây sát thương kẻ địch
			if (body is BaseEnemy enemy)
			{
				QueueFree();
				enemy.TakeDamage(this.damge);
			}
		}
		else
		{
			// Kiểm tra nếu đạn thuộc về kẻ địch, chỉ gây sát thương người chơi
			if (body is Player player)
			{
				QueueFree();
				player.OnHit(-this.damge); // Logic giảm máu của người chơi
			}
		}
	}
}
