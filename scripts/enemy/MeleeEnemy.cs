using Godot;
using System;

public partial class MeleeEnemy : BaseEnemy
{
	private float attackRange = 40f; // Phạm vi tấn công

	protected override Vector2 PerformBehavior(double delta)
	{
		if (_player == null)
			return Vector2.Zero;

		float distanceToPlayer = GlobalPosition.DistanceTo(_player.GlobalPosition);

		if (distanceToPlayer > attackRange && !IsAttacking)
		{
			// Di chuyển về phía người chơi
			return GetDirectionTowards(_player.GlobalPosition);
		}
		else if (!IsAttacking)
		{
			Attack();
			return Vector2.Zero; // Không di chuyển khi tấn công
		}

		return Vector2.Zero; // Không di chuyển nếu đang tấn công hoặc đã ở trong phạm vi
	}

	private void Attack()
	{
		IsAttacking = true;
		GD.Print("MeleeEnemy attacks!");

		// Logic gây sát thương ở đây

		// Tạo một Timer để delay tấn công
		Timer timer = new Timer();
		timer.WaitTime = 1.0f; // Delay trước khi có thể tấn công lại
		timer.OneShot = true;
		timer.Timeout += () => IsAttacking = false;
		AddChild(timer);
		timer.Start();
	}
}
