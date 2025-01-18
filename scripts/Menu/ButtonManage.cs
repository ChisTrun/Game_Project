using Godot;
using System;
using System.IO;
using System.Collections.Generic;
using Godot.Collections;

public partial class ButtonManage : Control
{
	// Biến tham chiếu đến các nút
	[Export] private Button _quitButton;
	[Export] private Button _newGameButton;
	[Export] private Control _settingBox;
	[Export] private Button _closeButton;
	[Export] private Button _settingButton;
	[Export] private Button _continueButton;

	// Tên scene để chuyển khi chọn New Game
	[Export] public PackedScene MainScene;

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
	
	
 private void OnContinuePressed()
{
	
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
}
