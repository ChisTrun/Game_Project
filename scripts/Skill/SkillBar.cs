using Godot;
using System;
using System.Collections.Generic;

public partial class SkillBar : CanvasLayer
{
	[Export]
	public int TotalSlots = 10;
	/*private List<TextureRect> skillSlotNodes = new List<TextureRect>();*/
	private List<PanelContainer> skillSlotContainers = new List<PanelContainer>();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var container = GetNode<HBoxContainer>("Control/HBoxContainer");

		// Lấy tất cả PanelContainer bên trong HBoxContainer
		foreach (Node child in container.GetChildren())
		{
			if (child is PanelContainer panel)
			{
				skillSlotContainers.Add(panel);

				// Tạo viền mặc định là màu đen
				var style = new StyleBoxFlat
				{
					BorderColor = new Color(0, 0, 0), // Màu đen
					BorderWidthTop = 2,
					BorderWidthBottom = 2,
					BorderWidthLeft = 2,
					BorderWidthRight = 2
				};
				panel.AddThemeStyleboxOverride("panel", style);


				// Lấy TextureRect bên trong PanelContainer để hiển thị hình ảnh kỹ năng
				var icon = panel.GetNode<TextureRect>("Icon");
				icon.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
				icon.Texture = null; // Bắt đầu không có kỹ năng
			}
		}

		UpdateSkillBar();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		UpdateSkillBar();
	}

	public void UpdateSkillBar()
	{

		// Lấy danh sách các kỹ năng đã kích hoạt
		var activeSkills = Global.GetActiveSkills();

		for (int i = 0; i < skillSlotContainers.Count; i++)
		{
			var panel = skillSlotContainers[i];
			var icon = panel.GetNode<TextureRect>("Icon"); // Lấy TextureRect bên trong PanelContainer

			if (i < activeSkills.Count)
			{
				// Lấy thông tin kỹ năng
				var skillName = activeSkills[i];
				var skill = Global.Skills[skillName];

				// Đặt hình ảnh kỹ năng
				icon.Texture = GD.Load<Texture2D>(skill.IconPath);
			}
			else
			{
				// Bỏ trống các ô không có kỹ năng
				icon.Texture = null;
			}
		}
	}

	public void HighlightSkill(string skillName)
	{
		/*for (int i = 0; i < skillSlotNodes.Count; i++)
		{
			// Kiểm tra kỹ năng trong slot
			if (i < Global.GetActiveSkills().Count && Global.GetActiveSkills()[i] == skillName)
			{
				// Làm nổi bật ô kỹ năng
				skillSlotNodes[i].Modulate = new Color(1, 0, 0); // Hiệu ứng đỏ

				// Tắt hiệu ứng sau 0.5 giây
				GetTree().CreateTimer(0.5f).Timeout += () =>
				{
					skillSlotNodes[i].Modulate = new Color(1, 1, 1); // Trở lại bình thường
				};
				break;
			}
		}*/

		for (int i = 0; i < skillSlotContainers.Count; i++)
		{
			// Kiểm tra kỹ năng trong slot
			if (i < Global.GetActiveSkills().Count && Global.GetActiveSkills()[i] == skillName)
			{
				var panel = skillSlotContainers[i];

				// Đổi màu viền sang xanh lá cây
				var style = new StyleBoxFlat
				{
					BorderWidthTop = 2,
					BorderWidthBottom = 2,
					BorderWidthLeft = 2,
					BorderWidthRight = 2,
					BorderColor = new Color(0, 1, 0) // Xanh lá cây
				};
				panel.AddThemeStyleboxOverride("panel", style);

				// Tắt hiệu ứng sau 0.5 giây
				GetTree().CreateTimer(0.5f).Timeout += () =>
				{
					style.BorderColor = new Color(0, 0, 0); // Trở lại màu đen
					panel.AddThemeStyleboxOverride("panel", style);
				};

				break;
			}
		}
	}

}
