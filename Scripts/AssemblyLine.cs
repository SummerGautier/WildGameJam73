using Godot;
using System;

public partial class AssemblyLine : Area2D
{

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    public override void _Draw()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public Rect2 GetBoundary()
    {
        Vector2 size = GetNode<CollisionShape2D>("GroundCollider").Shape.GetRect().Size;
        Vector2 position = new Vector2(
            x: Position.X,
            y: Position.Y - (size.Y / 2)
        );
        return new Rect2(position, size);
    }
}
