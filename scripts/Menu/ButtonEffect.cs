using Godot;

public partial class ButtonEffect : Button
{
	public override void _Ready()
	{
		// Tạo StyleBoxFlat cho trạng thái bình thường
		var normalStyle = new StyleBoxFlat();
		normalStyle.BgColor = new Color("#E8D977", 0.65f); // Màu nền chính

		// Tạo StyleBoxFlat cho trạng thái hover
		var hoverStyle = new StyleBoxFlat();
		hoverStyle.BgColor = new Color("#E8D977"); // Màu nền với opacity 65%

		// Gán StyleBoxFlat vào các trạng thái nút
		AddThemeStyleboxOverride("normal", normalStyle); // Trạng thái bình thường
		AddThemeStyleboxOverride("hover", hoverStyle);  // Trạng thái hover
	}
}
