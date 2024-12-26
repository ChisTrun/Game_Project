using Godot;
using System;

public partial class SoundChangeSlider : HSlider
{
	[Export] private Color SliderColor = new Color(0.1f, 0.1f, 0.1f); // Màu nền thanh trượt (xám tối)
	[Export] private Color GrabberColor = new Color(0.8f, 0.0f, 0.8f); // Màu nút kéo (tím neon)
	[Export] private Texture2D GrabberTexture; // Hình ảnh cho nút kéo

	private Tween _tween; // Tween animation

	public override void _Ready()
	{
		// Khởi tạo Tween
		_tween = GetTree().CreateTween();
		this.ValueChanged += OnValueChanged;

		// Đặt giá trị ban đầu và phạm vi
		this.MinValue = 0;
		this.MaxValue = 100;
		this.Step = 1;
		this.Value = 50; // Giá trị mặc định

		// Cấu hình thanh trượt và nút kéo
		SetupSlider();
		//SetupGrabber();
	}

	private void SetupSlider()
	{
		// Tạo StyleBoxFlat cho thanh trượt
		var sliderStyle = new StyleBoxFlat
		{
			BgColor = SliderColor,
			ShadowColor = new Color(0.5f, 0.0f, 0.5f, 0.5f), // Màu bóng tím
			ShadowSize = 8
		};

		// Thiết lập độ dày của thanh trượt
		sliderStyle.ContentMarginTop = 6;
		sliderStyle.ContentMarginBottom = 6;

		// Gán StyleBox cho Slider
		var theme = new Theme();
		theme.SetStylebox("slider", "HSlider", sliderStyle);
		this.Theme = theme;
	}

	public void OnValueChanged(double value)
	{
		GD.Print($"Volume changed to: {value}");

		// Điều chỉnh âm lượng (Linear Interpolation từ -80dB đến 0dB)
		AudioServer.SetBusVolumeDb(0, Mathf.Lerp(-80, 0, (float)value / 100));

		// Hiệu ứng màu mượt mà khi thay đổi giá trị
		_tween.TweenProperty(this, "modulate", new Color(1.0f, 0.5f, 0.5f), 0.2f)
			  .SetTrans(Tween.TransitionType.Sine)
			  .SetEase(Tween.EaseType.InOut);
	}
}
