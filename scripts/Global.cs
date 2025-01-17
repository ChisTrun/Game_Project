using Godot;
using System;

public partial class Global : Node
{
	public static int Gold = 0;

	public static int HealHealth = 10;

	public static float PlayerSpeed = 100.0f;

	public static float PlayerAcceleration = 10.0f;

	public static float PlayerDeceleration = 10.0f;

	public static float PlayerBaseDamage = 10.0f;

	public static float PlayerSkillCD = 1.0f;
	
	public static double VolumeValue { get; set; } = 50f;
}
