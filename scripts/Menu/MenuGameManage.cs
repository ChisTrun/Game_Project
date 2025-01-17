using Godot;

public partial class MenuGameManage : CanvasLayer
{
	// Biến tham chiếu đến các nút
	[Export] private Button _quitButton;
	[Export] private Button _newGameButton;
	[Export] private Control _settingBox;
	[Export] private Control _buttonBox;
	[Export] private Button _closeButton;
	[Export] private Button _settingButton;
	[Export] private Button _continueButton;
	// Tên scene để chuyển khi chọn New Game

	public override void _Ready()
	{
		// Kết nối tín hiệu cho các nút
		_quitButton.Pressed += OnQuitPressed;
		_newGameButton.Pressed += OnNewGamePressed;
		_settingButton.Pressed += OnSettingButtonPressed;
		_closeButton.Pressed += OnCloseButtonPressed;
		_continueButton.Pressed += OnContinuePressed;
		_settingBox.Visible = false;
	}

	private void OnQuitPressed()
	{
		GD.Print("Quit button pressed, exiting game.");
		GetTree().Quit(); // Thoát ứng dụng
	}

	private void OnNewGamePressed()
	{
		GD.Print("New Game button pressed, restarting current scene.");
	
		// Lấy scene hiện tại và restart lại
		var currentScene = GetTree().CurrentScene;
		if (currentScene != null)
		{
			GetTree().ReloadCurrentScene(); // Restart lại scene hiện tại
		}
		else
		{
			GD.PrintErr("No current scene is loaded. Unable to restart.");
		}
	}
	
	private void OnContinuePressed(){
		this.Visible = false;
	}
	
	private void OnSettingButtonPressed()
	{
		// Hiện hộp cài đặt
		_buttonBox.Visible = false; // Ẩn Control gốc (ButtonBox)
		_settingBox.Visible = true; // Hiện SettingBox
	}

	private void OnCloseButtonPressed()
	{
		// Ẩn hộp cài đặt
		_settingBox.Visible = false;
		_buttonBox.Visible = true;
	}
}
