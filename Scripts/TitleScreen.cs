using Godot;
using System;

public partial class TitleScreen : Node2D
{
	[Signal]
	public delegate void StartPressedEventHandler();

	[Export]
	Area2D playButton;
	private bool playClickable = false;
	public override void _Ready()
	{
		playButton.MouseEntered += OnPlayEntered;
		playButton.MouseExited += OnPlayExited;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (playClickable)
		{
            if (Input.IsMouseButtonPressed(MouseButton.Left))
            {
                EmitSignal(SignalName.StartPressed);
                return;
            }
        }
	}

	public void OnPlayEntered()
	{
		playClickable = true;
	}

    public void OnPlayExited()
    {
        playClickable = false;
    }
    
	public void OnStartPressed()
	{
		EmitSignal(SignalName.StartPressed);
	}
}
