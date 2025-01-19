using Godot;
using System;
using System.Collections.Generic;
using NewGameProject.scripts.Skills;

public partial class Global : Node
{
	public static int Gold = 100;
	
	public static int HealHealth = 10;

	public static Player PlayerInstance { get; set; }

	public static float PlayerSpeed = 100.0f;

	public static float PlayerAcceleration = 10.0f;

	public static float PlayerDeceleration = 10.0f;

	public static float PlayerBaseDamage = 12.0f;

	public static float PlayerSkillCD = 1.0f;

	public static int PlayerMaxHealth = 100;

	public static float CurrentHealth = PlayerMaxHealth;

	public static Dictionary<string, Skill> Skills = new();

	public static Vector2 PlayerPosition = new Vector2(530.0f, 1373.0f);
	
	public static int CurrentMap = 1; 

	public static double VolumeValue { get; set; } = 50f;


	public static void InitializeSkills()
	{
		if (Global.Skills.ContainsKey("Dash")){
			GD.Print("Has");
		}
		else
		{ 
			Skills["Dash"] = new Skill(
			name: "Dash",
			cooldown: 1.0f,
			isActive: Skills.ContainsKey("Dash") ? Skills["Dash"].IsActive : false,
			iconPath: "res://addons/duelyst_animated_sprites/assets/skills/Dash.png",
			onUse: () =>
			{
				if (Global.PlayerInstance != null) // Đảm bảo tham chiếu Player tồn tại
				{
					DashAction(Global.PlayerInstance);
				}
				else
				{
					GD.Print("Player instance not set in Global.");
				}
			}
			);
		
		}
	}

	public static void SyncToPlayer(Player player)
	{
		if (player == null)
		{
			GD.PrintErr("Player is null during sync!");
			return;
		}

		// Đồng bộ các chỉ số cơ bản từ Global
		player.maxHealth = PlayerMaxHealth;
		player.currentHealth = CurrentHealth;
		player.BaseDamage = PlayerBaseDamage;
		player.Speed = PlayerSpeed;
		player.Acceleration = PlayerAcceleration;
		player.Deceleration = PlayerDeceleration;

		GD.Print("Global data synced to Player.");
	}



	public static void SyncFromPlayer(Player player)
	{
		if (player == null)
		{
			GD.PrintErr("Player is null during sync!");
			return;
		}

		// Đồng bộ các chỉ số cơ bản từ Player
		PlayerMaxHealth = player.maxHealth;
		CurrentHealth = player.currentHealth;
		PlayerBaseDamage = player.BaseDamage;
		PlayerSpeed = player.Speed;
		PlayerAcceleration = player.Acceleration;
		PlayerDeceleration = player.Deceleration;

		GD.Print("Player data synced to Global.");
	}



	public static void ActivateSkill(string skillName)
	{
		if (Skills.ContainsKey(skillName))
		{
			Skills[skillName].IsActive = true;
		}
		else
		{
			GD.Print($"Skill {skillName} not found.");
		}
	}

	public static List<string> GetActiveSkills()
	{
		var activeSkills = new List<string>();

		foreach (var skill in Skills)
		{
			if (skill.Value.IsActive)
			{
				activeSkills.Add(skill.Key);
			}
		}

		return activeSkills;
	}


	public static void DashAction(Player player)
	{
		if (player == null)
		{
			GD.Print("Player node not found!");
			return;
		}

		if (player.isDashing)
		{
			GD.Print("Dash is already in progress!");
			return;
		}

		player.isDashing = true;
		player.dashTimer = player.dashCooldown + player.dashDuration;

		Vector2 dashDirection;

		// Kiểm tra hướng Dash
		if (player.character_direction != Vector2.Zero)
		{
			dashDirection = player.character_direction.Normalized(); // Dash theo hướng di chuyển
		}
		else
		{
			Vector2 mousePosition = player.GetGlobalMousePosition();
			dashDirection = (mousePosition - player.GlobalPosition).Normalized(); // Dash theo hướng chuột
		}

		player.Velocity = dashDirection * player.dashSpeed;

		GD.Print("Dash activated!");

		// Hiển thị hiệu ứng Dash phía sau CollisionShape2D
		if (player.dashEffect != null)
		{
			CollisionShape2D collisionShape = player.GetNode<CollisionShape2D>("CollisionShape2D");
			Vector2 collisionCenter = collisionShape.GlobalPosition;

			Vector2 effectPosition = collisionCenter - (dashDirection * 40);
			player.dashEffect.GlobalPosition = effectPosition;

			float angle = Mathf.Atan2(-dashDirection.Y, -dashDirection.X);
			player.dashEffect.Rotation = angle;

			player.dashEffect.Visible = true;
			player.dashEffect.Play("chainlightningblue");
		}

		// Tạo Timer để dừng Dash
		var dashEndTimer = new Timer();
		player.AddChild(dashEndTimer);
		dashEndTimer.WaitTime = player.dashDuration;
		dashEndTimer.OneShot = true;
		dashEndTimer.Timeout += () =>
		{
			player.isDashing = false;
			GD.Print("Dash ended.");

			if (player.dashEffect != null)
			{
				player.dashEffect.Visible = false;
				player.dashEffect.Stop();
			}

			dashEndTimer.QueueFree();
		};
		dashEndTimer.Start();
	}

}
