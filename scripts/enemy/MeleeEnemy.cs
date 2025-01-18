using Godot;
using System;

public partial class MeleeEnemy : BaseEnemy
{
	[Export] public float AttackRange = 40f; // Phạm vi tấn công
	private bool _canAttack = true;

	protected override Vector2 PerformBehavior(double delta)
	{
		if (_player == null)
			return Vector2.Zero;

		float distanceToPlayer = Position.DistanceTo(_player.Position);

		if (distanceToPlayer > VisionRange)
		{

			//GD.Print("Player is out of vision range.");

			/*GD.Print("Player is out of vision range.");*/

			return Vector2.Zero;
		}

		if (distanceToPlayer > AttackRange && !IsAttacking)
		{
			return GetDirectionTowards(_player.Position);
		}
		else if (!IsAttacking && _canAttack)
		{
			Attack();
			return Vector2.Zero;
		}

		return Vector2.Zero;
	}

	private void Attack()
	{
		if (!IsAttacking && _canAttack)
		{
			IsAttacking = true;
			_animatedSprite2D.Play("attack");
			_canAttack = false;
			CreateSound(AttackSound);
			if (Position.DistanceTo(_player.Position) <= AttackRange)
			{
				Player player = _player as Player;
				player.OnHit(-1 * Damage);
				GD.Print("Damage dealt to player!");
			}

			_animatedSprite2D.AnimationFinished += OnAttackAnimationFinished;
		}
	}

	private void OnAttackAnimationFinished()
	{
		GD.Print("Attack animation finished.");
		if (_animatedSprite2D.Animation == "attack")
		{
			GD.Print("Attack animation finished.");
			_animatedSprite2D.AnimationFinished -= OnAttackAnimationFinished;
			IsAttacking = false;
			// Reset timer để cho phép tấn công lại
			Timer timer = new Timer();
			timer.WaitTime = AttackCooldown;
			timer.OneShot = true;
			timer.Timeout += () => _canAttack = true;
			AddChild(timer);
			timer.Start();

		}
	}
}
