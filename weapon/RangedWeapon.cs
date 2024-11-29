using Godot;

public partial class RangedWeapon : Weapon
{
    [Export]
    public PackedScene ProjectileScene { get; set; } // Scene của đạn

    public override void Use(Vector2 targetPosition)
    {
        GD.Print("Firing ranged weapon!");

        if (ProjectileScene != null)
        {
            // Tạo instance cho đạn
            CharacterBody2D projectile = (CharacterBody2D)ProjectileScene.Instantiate();

            // Lấy parent để thêm đạn vào cây scene
            Node2D parent = GetParent<Node2D>();
            if (parent != null)
            {
                parent.AddChild(projectile);

                // Đặt vị trí ban đầu của đạn
                projectile.Position = GlobalPosition;

                // Thêm logic bắn
                Vector2 direction = (targetPosition - GlobalPosition).Normalized();
                projectile.Velocity = direction * 300; // Giả sử có thuộc tính Velocity
            }
            else
            {
                GD.PrintErr("Parent is null. Cannot add projectile.");
            }
        }
        else
        {
            GD.PrintErr("ProjectileScene is not assigned!");
        }
    }
}
