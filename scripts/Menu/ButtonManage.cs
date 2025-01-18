using Godot;
using System;
using System.IO;

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
 	GD.Print("Continue button pressed, loading game state...");

 	string filePath = "user/save_user.json"; // Thay đổi thành đường dẫn tương thích với Godot

 	if (File.Exists(filePath))
 	{
 		try
 		{
 			// Đọc nội dung file
 			string fileContent = File.ReadAllText(filePath);
 			Json jsonLoader = new Json();
 			// Parse JSON
 			Error err = jsonLoader.Parse(fileContent);
 			if (err != Error.Ok)
 			{
 				GD.Print(err);
 				return;
 			}

 			// Đảm bảo dữ liệu JSON được chuyển đổi chính xác
 			Godot.Collections.Dictionary gameState = (Godot.Collections.Dictionary)jsonLoader.Data;
 			if (gameState == null)
 			{
 				GD.PrintErr("Failed to parse game state.");
 				return;
 			}

 			// Phục hồi trạng thái trò chơi
 			Global.Gold = Convert.ToInt32(gameState["Gold"]);
 			Global.HealHealth = Convert.ToInt32(gameState["HealHealth"]);
 			Global.PlayerSpeed = Convert.ToSingle(gameState["PlayerSpeed"]);
 			Global.PlayerAcceleration = Convert.ToSingle(gameState["PlayerAcceleration"]);
 			Global.PlayerDeceleration = Convert.ToSingle(gameState["PlayerDeceleration"]);
 			Global.PlayerBaseDamage = Convert.ToSingle(gameState["PlayerBaseDamage"]);
 			Global.PlayerSkillCD = Convert.ToSingle(gameState["PlayerSkillCD"]);
			GD.Print(Global.PlayerSkillCD);

 			Godot.Collections.Array skillNamesVariant = (Godot.Collections.Array)gameState["SkillNames"];
			Godot.Collections.Array skillStatesVariant = (Godot.Collections.Array)gameState["SkillStates"];
			if (skillNamesVariant is Godot.Collections.Array skillNames &&
					skillStatesVariant is Godot.Collections.Array skillStates &&
					skillNames.Count == skillStates.Count)
				{
					for (int i = 0; i < skillNames.Count; i++)
					{
						string skillName = (string)skillNames[i];
						bool skillState = Convert.ToBoolean(skillStates[i]);

						if (!string.IsNullOrEmpty(skillName))
						{
							Global.Skills[skillName].IsActive = skillState;
						}
					}
				}
				else
				{
					GD.PrintErr("Failed to restore skills: SkillNames or SkillStates are invalid.");
				}
			GD.Print(Global.Skills);
			string playerPositionString = (string)gameState["PlayerPosition"];

			if (!string.IsNullOrEmpty(playerPositionString))
			{
				// Loại bỏ dấu ngoặc và phân tích chuỗi
				playerPositionString = playerPositionString.Trim('(', ')');
				string[] positionParts = playerPositionString.Split(',');

				if (positionParts.Length == 2 &&
					float.TryParse(positionParts[0], out float x) &&
					float.TryParse(positionParts[1], out float y))
				{
					Vector2 position = new Vector2(x, y);

					if (Global.PlayerInstance != null)
					{
						Global.PlayerInstance.GlobalPosition = position;
						GD.Print($"Player position restored to: {position}");
					}
					else
					{
						GD.PrintErr("Player instance not set in Global. Unable to restore position.");
					}
				}
				else
				{
					GD.PrintErr("Failed to parse PlayerPosition from string.");
				}
			}
			GD.Print(Global.PlayerInstance.GlobalPosition);
 			var currentScene = GetTree().CurrentScene;
 			if (currentScene != null)
 			{
 				GetTree().ReloadCurrentScene();
 			}
 			else
 			{
 				GD.PrintErr("No current scene is loaded. Unable to restart.");
 			}
 		}
 		catch (Exception ex)
 		{
 			GD.PrintErr($"Failed to load game state: {ex.Message}");
 		}
 	}
 	else
 	{
 		GD.PrintErr($"Save file not found at {filePath}");
 	}
	
	if (MainScene != null)
		{
			GetTree().ChangeSceneToPacked(MainScene); // Chuyển đến Scene Main
		}
		else
		{
			GD.PrintErr("MainScene is not set! Please assign it in the inspector.");
		}
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
