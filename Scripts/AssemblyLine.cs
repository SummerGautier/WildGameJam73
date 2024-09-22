using Godot;
using System;

public partial class AssemblyLine : Area2D
{
    private MovementSystem _movementSystem;
    private VisibleOnScreenNotifier2D _notifier;

    public override void _Ready()
    {
        _movementSystem = GetNode<MovementSystem>("AssemblyMovement");
        _movementSystem.MovePositionUpdate += this.UpdateMovePosition;
        _notifier = GetNode<VisibleOnScreenNotifier2D>("Notifier");
        _notifier.ScreenExited += OnScreenExit;
    }

    public override void _Process(double delta)
    {
       // this._movementSystem.OnEntityRun(MovementSystem.Cardinal.Left, Position);
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

    private void OnScreenExit()
    {
        this.Position = new Vector2(1900, Position.Y);
    }

    public void UpdateMovePosition(Vector2 position)
    {
        this.Position = position;
    }
}
