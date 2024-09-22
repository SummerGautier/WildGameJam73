using Godot;
using System;

public partial class ProgressBar : Godot.ProgressBar
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("default");

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		this.SetValueNoSignal(Bolt.TOTAL_COLLECTED*10);
	}
}
