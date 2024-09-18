using Godot;
using System;

public partial class AssemblyLine : Area2D
{
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
