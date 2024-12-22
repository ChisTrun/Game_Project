using Godot;
using System;

public partial class Bullet : Node2D
{
    public Vector2 Direction { get; set; } = Vector2.Zero; // Đảm bảo có giá trị mặc định
    [Export] public float speed = 200f;
    private AnimatedSprite2D _animatedSprite;
    private Area2D _area2D;

    public override void _Ready()
    {
        // Lấy AnimatedSprite2D
        _animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        if (_animatedSprite == null)
        {
            GD.PrintErr("AnimatedSprite2D not found in Bullet!");
        }
        else
        {
            _animatedSprite.Play("go");
        }

        // Lấy Area2D và kết nối tín hiệu body_entered
        _area2D = GetNode<Area2D>("Area2D");

        if (_area2D != null)
        {
            _area2D.BodyEntered += _On_Body_Entered;
        }
        else
        {
            GD.PrintErr("Area2D not found in Bullet!");
        }
    }

    public override void _Process(double delta)
    {
        // Di chuyển đạn
        Position += Direction * speed * (float)delta;

        // Xoá đạn nếu ra khỏi màn hình
        if (!GetViewportRect().HasPoint(GlobalPosition))
        {
            QueueFree();
        }
    }

    // Hàm xử lý va chạm với Player
    private void _On_Body_Entered(Node body)
    {
        if (body is Player player)
        {
            GD.Print("Player hit by bullet!");
            QueueFree();

            // Logic giảm máu người chơi
            player.OnHit(-20); // Ví dụ, nếu Player có hàm TakeDamage
        }
    }
}
