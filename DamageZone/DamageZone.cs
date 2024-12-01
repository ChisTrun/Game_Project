using Godot;

public partial class DamageZone : TileMap
{  [Export]
	public int DamagePerSecond = 10; // Sát thương mỗi giây

	public override void _PhysicsProcess(double delta)
	{
		// Lấy vị trí của người chơi
		var player = GetNode<Player>("../Player");

		if (player != null)
		{
			// Lấy vị trí của người chơi
			var playerPosition = player.Position;

 			Vector2 cellSize = GetUsedRect().Size / GetTileset().GetTileSize();

			playerPosition.X = Mathf.Floor(playerPosition.X / cellSize.X) * cellSize.X;
			playerPosition.Y = Mathf.Floor(playerPosition.Y / cellSize.Y) * cellSize.Y;

			// Tính toán tọa độ cell mà player đang đứng
			Vector2I cellPosition = LocalToMap(playerPosition);

			int cellId = GetCellSourceId(2,cellPosition);

			if (cellId != -1 )
			{
				player.OnHit(-4);
			}
		}
	}
}
