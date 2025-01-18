using Godot;
using System;

public partial class SetSound : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		float actualValue = Mathf.Lerp(40, 140, (float)Global.VolumeValue / 100);

		// Áp dụng giá trị thực tế vào âm lượng
		AudioServer.SetBusVolumeDb(0, Mathf.Lerp(-80, 0, actualValue / 120));
		GD.Print(AudioServer.GetBusVolumeDb(0));
	}

}
