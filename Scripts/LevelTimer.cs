using Godot;
using System;

public partial class LevelTimer : Timer
{
	[Export]
	private RichTextLabel _label;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.Start();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		int seconds = (int)this.TimeLeft;
		_label.Text = $"T-{seconds}s";
	}
}
