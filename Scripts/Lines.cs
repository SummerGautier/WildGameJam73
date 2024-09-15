using Godot;
using System;

public partial class Lines : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

    public override void _Draw()
    {
        DrawLine(new Vector2(0, 500), new Vector2(1920, 500), new Color(0, 0, 0), 2);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
