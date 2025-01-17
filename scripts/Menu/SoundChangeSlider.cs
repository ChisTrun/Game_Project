using Godot;
using System;

public partial class SoundChangeSlider : HSlider
{
	[Export] private Color SliderColor = new Color(0.1f, 0.1f, 0.1f); // Màu nền thanh trượt (xám tối)
	[Export] private Color GrabberColor = new Color(0.8f, 0.0f, 0.8f); // Màu nút kéo (tím neon)
	[Export] private Texture2D GrabberTexture; // Hình ảnh cho nút kéo
	private int volumeScale = 40;

	private Color _currentModulateColor = Colors.White; // Màu hiện tại
	private Color _targetModulateColor = Colors.White; // Màu mục tiêu
	private float _lerpSpeed = 5.0f; // Tốc độ chuyển đổi màu

	public override void _Ready()
	{
		this.ValueChanged += OnValueChanged;

		// Đặt giá trị ban đầu và phạm vi
		this.MinValue = 0;
		this.MaxValue = 100;
		this.Step = 1;
		this.Value = Global.VolumeValue; // Giá trị mặc định

		// Cấu hình thanh trượt
		SetupSlider();
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
		Global.VolumeValue = value;
		// Điều chỉnh âm lượng (Linear Interpolation từ -80dB đến 0dB)
		// Chuyển giá trị từ phạm vi 0-100 sang 20-120
		float actualValue = Mathf.Lerp(40, 140, (float)value / 100);

		// Áp dụng giá trị thực tế vào âm lượng
		AudioServer.SetBusVolumeDb(0, Mathf.Lerp(-80, 0, actualValue / 120));

		// Cập nhật màu mục tiêu
		_targetModulateColor = new Color(1.0f, 0.5f, 0.5f);
	}

	public override void _Process(double delta)
	{
		// Chuyển màu modulate dần dần
		_currentModulateColor = new Color(
			Mathf.Lerp(_currentModulateColor.R, _targetModulateColor.R, (float)delta * _lerpSpeed),
			Mathf.Lerp(_currentModulateColor.G, _targetModulateColor.G, (float)delta * _lerpSpeed),
			Mathf.Lerp(_currentModulateColor.B, _targetModulateColor.B, (float)delta * _lerpSpeed),
			Mathf.Lerp(_currentModulateColor.A, _targetModulateColor.A, (float)delta * _lerpSpeed)
		);

		this.Modulate = _currentModulateColor;

		// Kiểm tra nếu màu hiện tại gần màu mục tiêu, đặt lại màu trắng
		if (IsColorClose(_currentModulateColor, _targetModulateColor, 0.01f))
		{
			_targetModulateColor = Colors.White;
		}
	}

	private bool IsColorClose(Color colorA, Color colorB, float threshold)
	{
		// So sánh từng thành phần màu (RGB) để kiểm tra khoảng cách
		return Mathf.Abs(colorA.R - colorB.R) < threshold &&
			   Mathf.Abs(colorA.G - colorB.G) < threshold &&
			   Mathf.Abs(colorA.B - colorB.B) < threshold &&
			   Mathf.Abs(colorA.A - colorB.A) < threshold;
	}
}
