using Godot;
using System;

public partial class TitleScreen : Node2D
{
	[Signal]
	public delegate void StartPressedEventHandler();

	[Export]
	Area2D playButton;
	private bool playClickable = false;
	[Export]
	Sprite2D playActive;

	[Export]
	Sprite2D playInactive;


    [Signal]
    public delegate void  InstructionsPressedEventHandler();

    [Export]
    Area2D instructionsButton;
    private bool instructionsClickable = false;
    [Export]
    Sprite2D instructionsActive;

    [Export]
    Sprite2D instructionsInactive;
    public override void _Ready()
	{
		playButton.MouseEntered += OnPlayEntered;
		playButton.MouseExited += OnPlayExited;
        instructionsButton.MouseEntered += OnInstructionsEntered;
        instructionsButton.MouseExited += OnInstructionsExited;
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
        if (instructionsClickable)
        {
            if (Input.IsMouseButtonPressed(MouseButton.Left))
            {
                EmitSignal(SignalName.InstructionsPressed);
                return;
            }
        }
    }

	public void OnPlayEntered()
	{
		playActive.Show();
		playInactive.Hide();
		playClickable = true;
	}

    public void OnPlayExited()
    {
		playInactive.Show();
		playActive.Hide();
        playClickable = false;
    }
    
	public void OnStartPressed()
	{
		EmitSignal(SignalName.StartPressed);
	}


    public void OnInstructionsEntered()
    {
        instructionsActive.Show();
        instructionsInactive.Hide();
        instructionsClickable = true;
    }

    public void OnInstructionsExited()
    {
        playInactive.Show();
        playActive.Hide();
        playClickable = false;
    }

    public void OnInstructionsPressed()
    {
        EmitSignal(SignalName.InstructionsPressed);
    }
}
