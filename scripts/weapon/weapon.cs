using Godot;

public enum WeaponType { Melee, Ranged }

public abstract partial class Weapon : Node2D
{

	[Export]
	public AudioStream AttackSound { get; set; } // Âm thanh khi bắn

	[Export]
	public float Damage { get; set; } = Global.PlayerBaseDamage;

	[Export]
	public float Range { get; set; } = 0f;

	[Export]
	public float Cooldown { get; set; } = 1.0f;

	[Export]
	public Texture WeaponSprite { get; set; }

	public abstract void Use(Vector2 targetPosition);
}
