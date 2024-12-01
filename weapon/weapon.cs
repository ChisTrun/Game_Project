using Godot;

public enum WeaponType { Melee, Ranged } 

public abstract partial class Weapon : Node2D
{
	[Export]
	public int Damage { get; set; } = 10;

	[Export]
	public float Range { get; set; } = 0f;

	[Export]
	public float Cooldown { get; set; } = 1.0f;

	[Export]
	public WeaponType Type { get; set; } = WeaponType.Melee;

	[Export]
	public Texture WeaponSprite { get; set; }

	public abstract void Use(Vector2 targetPosition);
}
