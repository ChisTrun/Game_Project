using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	[Export] public float DetectionRange = 50.0f; // Khoảng cách phát hiện Player
	[Export] public float PatrolSpeed = 20.0f; // Tốc độ tuần tra
	[Export] public float PatrolPauseTime = 1.0f; // Thời gian dừng lại tại mỗi điểm tuần tra
	[Export] public Vector2 PatrolAreaSize = new Vector2(50, 50); // Kích thước khu vực tuần tra

	private Node2D player; // Tham chiếu đến Player
	private AnimatedSprite2D animatedSprite;

	private Vector2 targetPatrolPoint; // Điểm tuần tra hiện tại
	private float patrolPauseTimer = 0.0f; // Bộ đếm dừng tuần tra

	private Random random = new Random(); // Bộ sinh số ngẫu nhiên

	[Export]
	public int DamagePerSecond = 6; // Sát thương mỗi giây

	public override void _Ready()
	{
		player = GetNodeOrNull<Node2D>("../Player");
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		// Thiết lập điểm tuần tra đầu tiên
		SetRandomPatrolPoint();

		var area = GetNode<Area2D>("Area2D");
		area.BodyEntered += OnBodyEntered;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (GetTree().Paused)
			return;

		if (player == null)
			return;

		// Kiểm tra khoảng cách đến Player
		float distanceToPlayer = GlobalPosition.DistanceTo(player.GlobalPosition);

		if (distanceToPlayer <= DetectionRange)
		{
			ChasePlayer(delta);
		}
		else
		{
			Patrol(delta);
		}
	}

	private void Patrol(double delta)
	{
		// Nếu còn thời gian dừng lại, không di chuyển
		if (patrolPauseTimer > 0)
		{
			patrolPauseTimer -= (float)delta;
			animatedSprite.Play("idle");
			Velocity = Vector2.Zero;
			return;
		}

		// Di chuyển tới điểm tuần tra
		Vector2 direction = (targetPatrolPoint - GlobalPosition).Normalized();
		Velocity = direction * PatrolSpeed;
		animatedSprite.Play("run");
		MoveAndSlide();

		// Nếu đến gần điểm tuần tra, chọn điểm tuần tra mới
		float distanceToPoint = GlobalPosition.DistanceTo(targetPatrolPoint);
		if (distanceToPoint < 5.0f)
		{
			patrolPauseTimer = PatrolPauseTime; // Dừng lại một chút
			SetRandomPatrolPoint();
		}

		// Kiểm tra nếu bot bị kẹt (không di chuyển được nữa)
		if (Velocity.Length() < 1.0f) // Tốc độ gần như bằng 0
		{
			GD.Print("Bot bị kẹt, đặt lại điểm tuần tra mới");
			SetRandomPatrolPoint();
		}
	}


	private void ChasePlayer(double delta)
	{
		// Đuổi theo Player
		Vector2 direction = (player.GlobalPosition - GlobalPosition).Normalized();
		Velocity = direction * PatrolSpeed * 2; // Tăng tốc khi đuổi theo
		animatedSprite.Play("run");
		MoveAndSlide();
	}

	private void SetRandomPatrolPoint()
	{
		// Tạo một điểm tuần tra ngẫu nhiên trong khu vực
		float randomX = (float)(random.NextDouble() * PatrolAreaSize.X) - PatrolAreaSize.X / 2;
		float randomY = (float)(random.NextDouble() * PatrolAreaSize.Y) - PatrolAreaSize.Y / 2;

		targetPatrolPoint = GlobalPosition + new Vector2(randomX, randomY);
	}

	private void OnBodyEntered(Node body)
	{
		if (body is Player player)
		{
			player.OnHit(-DamagePerSecond);
		}
	}
}
