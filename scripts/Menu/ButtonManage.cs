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
	public PackedScene MainScene;

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
	GD.Print("Continue button pressed, loading game state from TXT...");

	string filePath = "user/save_user.txt";

	if (File.Exists(filePath))
	{
		try
		{
			// Đọc giá trị từ file TXT
			var gameState = ReadFromTxtFile(filePath);
			GD.Print(gameState);
			// Cập nhật giá trị vào Global
			Global.InitializeSkills();
			if (gameState.ContainsKey("Gold"))
				Global.Gold = Convert.ToInt32(gameState["Gold"]);

			if (gameState.ContainsKey("HealHealth"))
				Global.HealHealth = Convert.ToInt32(gameState["HealHealth"]);

			if (gameState.ContainsKey("PlayerSpeed"))
				Global.PlayerSpeed = Convert.ToSingle(gameState["PlayerSpeed"]);

			if (gameState.ContainsKey("PlayerAcceleration"))
				Global.PlayerAcceleration = Convert.ToSingle(gameState["PlayerAcceleration"]);

			if (gameState.ContainsKey("PlayerDeceleration"))
				Global.PlayerDeceleration = Convert.ToSingle(gameState["PlayerDeceleration"]);

			if (gameState.ContainsKey("PlayerBaseDamage"))
				Global.PlayerBaseDamage = Convert.ToSingle(gameState["PlayerBaseDamage"]);

			if (gameState.ContainsKey("PlayerSkillCD"))
				Global.PlayerSkillCD = Convert.ToSingle(gameState["PlayerSkillCD"]);

			if (gameState.ContainsKey("CurrentMap"))
				Global.CurrentMap = Convert.ToInt32(gameState["CurrentMap"]);

			if (gameState.ContainsKey("CurrentHealth"))
				Global.CurrentHealth = Convert.ToSingle(gameState["CurrentHealth"]);

			if (gameState.ContainsKey("VolumeValue"))
				Global.VolumeValue = Convert.ToDouble(gameState["VolumeValue"]);

			if (gameState.ContainsKey("PlayerMaxHealth"))
				Global.PlayerMaxHealth = Convert.ToInt32(gameState["PlayerMaxHealth"]);
			

			if (gameState.ContainsKey("PlayerPosition"))
			{
				string positionString = gameState["PlayerPosition"].ToString();
				string[] positionParts = positionString.Trim('(', ')').Split(',');

				// Cập nhật vị trí vào Global.playerPosition
				Global.PlayerPosition = new Vector2(
					Convert.ToSingle(positionParts[0]),
					Convert.ToSingle(positionParts[1])
				);
			}

			if (gameState.ContainsKey("SkillNames") && gameState.ContainsKey("SkillStates"))
			{
				string[] skillNames = gameState["SkillNames"].ToString().Trim('[', ']').Split(',');
				string[] skillStates = gameState["SkillStates"].ToString().Trim('[', ']').Split(',');

				for (int i = 0; i < skillNames.Length; i++)
				{
					string skillName = skillNames[i].Trim();
					bool isActive = Convert.ToBoolean(skillStates[i].Trim());
					//Global.Skills[skillName].IsActive = isActive;
					if (Global.Skills.ContainsKey(skillName))
					{
						Global.Skills[skillName].IsActive = isActive;
					}
					else
					{
						GD.Print($"Skill {skillName} not found in Global.");
					}
				}
			}
			if (Global.CurrentMap == 1)
			{
				MainScene = (PackedScene)ResourceLoader.Load("res://scenes/main.tscn");
			} else if (Global.CurrentMap == 2)
			{
				MainScene = (PackedScene)ResourceLoader.Load("res://scenes/map/map2.tscn");
			}
			else if (Global.CurrentMap == 3)
			{
				MainScene = (PackedScene)ResourceLoader.Load("res://scenes/map/map3.tscn");
			}
			if (MainScene != null)
			{
				GetTree().ChangeSceneToPacked(MainScene); // Chuyển đến Scene Main
			}
			else
			{
				GD.PrintErr("MainScene is not set! Please assign it in the inspector.");
			}
			GD.Print("Game state loaded successfully from TXT.");
		}
		catch (Exception ex)
		{
			GD.PrintErr($"Failed to load game state from TXT: {ex.Message}");
		}
	}
	else
	{
		GD.PrintErr("Save file not found at path: " + filePath);
	}
}

private System.Collections.Generic.Dictionary<string, string> ReadFromTxtFile(string filePath)
{
	var gameState = new System.Collections.Generic.Dictionary<string, string>();

	try
	{
		// Đọc tất cả các dòng từ file
		var lines = File.ReadAllLines(filePath);

		foreach (var line in lines)
		{
			// Tách key và value bằng dấu ':'
			var parts = line.Split(new[] { ':' }, 2);
			if (parts.Length == 2)
			{
				string key = parts[0].Trim();
				string value = parts[1].Trim();
				gameState[key] = value;
			}
		}
	}
	catch (Exception ex)
	{
		GD.PrintErr($"Failed to read from TXT file: {ex.Message}");
	}

	return gameState;
}




	private void OnNewGamePressed()
	{
		GD.Print("New Game button pressed, switching to main screen.");
		MainScene = (PackedScene)ResourceLoader.Load("res://scenes/main.tscn");
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
