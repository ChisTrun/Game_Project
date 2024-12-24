using Godot;
using System.Collections.Generic;

public partial class MenuTitle : Control
{
	private List<Label> _letters = new List<Label>(); // Danh sách các Label cho từng chữ
	private string _text = "PRIMORDIAL GUARDIANS"; // Nội dung tiêu đề
	private Font _customFont;
	[Export] private Label Subtitle;
	public override void _Ready()
	{
		_customFont = GD.Load<Font>("res://Font/MarcellusSC-Regular.ttf");
		float xOffset = 291; // Vị trí X bắt đầu
		float yOffset = 33;  // Vị trí Y cố định
		// Tạo từng Label cho từng chữ
		foreach (char character in _text)
		{
			var letter = new Label
			{
				Text = character.ToString(), // Gán ký tự
				Modulate = new Color("c19c4e"), // Màu chữ
				Position = new Vector2(xOffset, yOffset) // Vị trí ban đầu
			};

			// Áp dụng font tuỳ chỉnh
			letter.AddThemeFontOverride("font", _customFont);
			letter.AddThemeFontSizeOverride("font_size", 48);

			AddChild(letter); // Thêm vào scene
			_letters.Add(letter); // Lưu vào danh sách

			// Cập nhật vị trí X cho ký tự tiếp theo dựa trên độ rộng ký tự hiện tại
			var charSize = _customFont.GetStringSize(character.ToString())*3;
			xOffset += charSize.X; // Dịch vị trí X theo chiều rộng ký tự
		}


		// Bắt đầu hiệu ứng sáng từng chữ
		AnimateLetters();
		AnimateSubtitle(Subtitle);
	}

private void AnimateLetters()
{
	var baseColor = new Color("c19c4e"); // Màu gốc
	var brightColor = new Color("ffda79"); // Màu sáng
	var fadeDuration = 0.4f; // Thời gian mỗi chữ sáng hoặc tắt
	var delay = 0.2f; // Độ trễ giữa các chữ
	var totalDuration = (_letters.Count * delay) + (2 * fadeDuration); // Tổng thời gian cho tất cả các chữ

	// Lặp vô hạn bằng cách gọi lại chính hàm này
	var tweener = GetTree().CreateTween();
	tweener.TweenCallback(Callable.From(AnimateLetters)).SetDelay(totalDuration);

	for (int i = 0; i < _letters.Count; i++)
	{
		var letter = _letters[i];

		// Tạo Tweener cho từng chữ
		var letterTweener = GetTree().CreateTween();

		// Thêm độ trễ trước khi bắt đầu fade in
		letterTweener.TweenInterval(i * delay);

		// Giai đoạn 1: Fade in (sáng lên)
		letterTweener.TweenProperty(
			letter,
			"modulate",
			brightColor, // Sáng lên
			fadeDuration // Thời gian sáng
		)
		.SetTrans(Tween.TransitionType.Sine)
		.SetEase(Tween.EaseType.InOut);

		// Giai đoạn 2: Fade out (tắt đi)
		letterTweener.TweenProperty(
			letter,
			"modulate",
			baseColor, // Trở về màu gốc
			fadeDuration // Thời gian tắt
		)
		.SetTrans(Tween.TransitionType.Sine)
		.SetEase(Tween.EaseType.InOut);
	}
}




	
	private void AnimateSubtitle(Label subtitle)
	{
		var fade_time = 0.8f; // Thời gian fade in hoặc fade out

		// Đặt giá trị ban đầu cho modulate (ẩn hoàn toàn)
		subtitle.Modulate = new Color(1, 1, 1, 0);

		var tweener = GetTree().CreateTween(); // Tạo Tweener cho subtitle

		// Giai đoạn 1: Fade in (hiện)
		tweener.TweenProperty(
			subtitle,
			"modulate",
			new Color(1, 1, 1, 1), // Hiện hoàn toàn
			fade_time // Thời gian fade in
		)
		.SetTrans(Tween.TransitionType.Sine)
		.SetEase(Tween.EaseType.InOut);

		// Giai đoạn 2: Fade out (ẩn)
		tweener.TweenProperty(
			subtitle,
			"modulate",
			new Color(1, 1, 1, 0), // Ẩn hoàn toàn
			fade_time // Thời gian fade out
		)
		.SetDelay(fade_time/2) // Đặt độ trễ để bắt đầu fade out sau fade in
		.SetTrans(Tween.TransitionType.Sine)
		.SetEase(Tween.EaseType.InOut);

		// Lặp lại hiệu ứng
		tweener.SetLoops(); // Tự động lặp lại vô hạn
	}

}
