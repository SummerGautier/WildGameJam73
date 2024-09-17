using Godot;
using System;

public partial class AssemblyLine : Area2D
{

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public Rect2 GetBoundary()
	{
		Rect2 colliderRect = GetNode<CollisionShape2D>("GroundCollider").Shape.GetRect();
		return new Rect2(GlobalPosition, colliderRect.Size);
	}
}
