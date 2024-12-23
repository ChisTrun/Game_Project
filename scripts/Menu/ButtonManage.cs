using Godot;

public partial class ButtonManage : Control
{
	// Biến tham chiếu đến các nút
	[Export] private Button _quitButton;
	[Export] private Button _newGameButton;
	[Export] private Control _settingBox;
	[Export] private HSlider _volumeSlider;
	[Export] private Button _closeButton;
	[Export] private Button _settingButton;
	// Tên scene để chuyển khi chọn New Game
	[Export] public PackedScene MainScene;

	public override void _Ready()
	{
		// Kết nối tín hiệu cho các nút
		_quitButton.Pressed += OnQuitPressed;
		_newGameButton.Pressed += OnNewGamePressed;
		_volumeSlider.ValueChanged += OnVolumeChanged;
		_settingButton.Pressed += OnSettingButtonPressed;
		_closeButton.Pressed += OnCloseButtonPressed;
		_settingBox.Visible = false;
		_volumeSlider.Value = 100;

		// Tạo một Theme mới
		var theme = new Theme();
		var styleBox = new StyleBoxFlat();
		
		// Cài đặt màu nền và viền
		styleBox.BgColor = new Color("#2E3440");  // Màu nền
		styleBox.BorderColor = new Color("#4C566A"); // Màu viền

		// Áp dụng StyleBoxFlat vào Theme
		theme.SetStylebox("panel", "Panel", styleBox);

		// Áp dụng Theme vào _settingBox
		_settingBox.Theme = theme;
	}

	private void OnQuitPressed()
	{
		GD.Print("Quit button pressed, exiting game.");
		GetTree().Quit(); // Thoát ứng dụng
	}

	private void OnNewGamePressed()
	{
		GD.Print("New Game button pressed, switching to main screen.");
		
		// Kiểm tra nếu scene đã được gán
		if (MainScene != null)
		{
			GetTree().ChangeSceneToPacked(MainScene); // Chuyển đến Scene Main
		}
		else
		{
			GD.PrintErr("MainScene is not set! Please assign it in the inspector.");
		}
	}
	private void OnSettingButtonPressed()
	{
		// Hiện hộp cài đặt
		this.Visible = false; // Ẩn Control gốc (ButtonBox)
		_settingBox.Visible = true; // Hiện SettingBox
	}

	private void OnCloseButtonPressed()
	{
		// Ẩn hộp cài đặt
		_settingBox.Visible = false;
		this.Visible = true;
	}

	private void OnVolumeChanged(double value)
	{
		// Cập nhật âm lượng (0 - 100) dựa trên thanh trượt
		GD.Print($"Volume changed to: {value}");
		AudioServer.SetBusVolumeDb(0, Mathf.Lerp(-80, 0, (float)value / 100));
	}
}
