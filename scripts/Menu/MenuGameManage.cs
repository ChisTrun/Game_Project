using Godot;
using System.IO;
using System;

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
	GD.Print("Quit button pressed, saving game state and exiting...");

	// Lấy trạng thái game
	var gameState = GetGameState();

	// Lưu trạng thái game
	SaveToJsonFile("user/save_user.json", gameState);
	SaveToTxtFile("user/save_user.txt", gameState);

	GetTree().Quit();
}

private Godot.Collections.Dictionary GetGameState()
{
	// Khởi tạo dictionary lưu trạng thái
	Godot.Collections.Dictionary gameState = new Godot.Collections.Dictionary
	{
		{ "Gold", Global.Gold },
		{ "HealHealth", Global.HealHealth },
		{ "PlayerSpeed", Global.PlayerSpeed },
		{ "PlayerAcceleration", Global.PlayerAcceleration },
		{ "PlayerDeceleration", Global.PlayerDeceleration },
		{ "PlayerBaseDamage", Global.PlayerBaseDamage },
		{ "PlayerSkillCD", Global.PlayerSkillCD },
		{ "PlayerPosition", Global.PlayerInstance?.GlobalPosition ?? Vector2.Zero },
		{ "CurrentMap ", Global.CurrentMap },
		{ "CurrentHealth", Global.CurrentHealth },
		{ "VolumeValue", Global.VolumeValue },
		{ "PlayerMaxHealth", Global.PlayerMaxHealth }
	};

	// Lưu trạng thái kỹ năng dưới dạng 2 mảng
	var skillNames = new Godot.Collections.Array();
	var skillStates = new Godot.Collections.Array();

	foreach (var skill in Global.Skills)
	{
		skillNames.Add(skill.Key);
		skillStates.Add(skill.Value.IsActive);
	}

	gameState["SkillNames"] = skillNames;
	gameState["SkillStates"] = skillStates;

	return gameState;
}

private void SaveToJsonFile(string filePath, Godot.Collections.Dictionary gameState)
{
	try
	{
		// Serialize game state
		string jsonString = Json.Stringify(gameState);

		// Save to file
		File.WriteAllText(filePath, jsonString);
		GD.Print($"Game state saved to {filePath}");
	}
	catch (Exception ex)
	{
		GD.PrintErr($"Failed to save game state to JSON: {ex.Message}");
	}
}

private void SaveToTxtFile(string filePath, Godot.Collections.Dictionary gameState)
{
	try
	{
		// Tạo nội dung dạng text
		var lines = new System.Text.StringBuilder();

		foreach (var key in gameState.Keys)
		{
			// Ép kiểu tường minh cho giá trị
			if (gameState[key] is Godot.Variant value && value.Obj is Godot.Collections.Array array)
			{
				lines.AppendLine($"{key}: [{string.Join(", ", array)}]");
			}
			else
			{
				lines.AppendLine($"{key}: {gameState[key]}");
			}
		}

		// Save to file
		File.WriteAllText(filePath, lines.ToString());
		GD.Print($"Game state saved to {filePath}");
	}
	catch (Exception ex)
	{
		GD.PrintErr($"Failed to save game state to TXT: {ex.Message}");
	}
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
