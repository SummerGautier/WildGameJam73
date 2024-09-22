using Godot;
using System;

public partial class GameOver : Node2D
{
    [Signal]
    public delegate void TryAgainPressedEventHandler();

    [Export]
    Button restartButton;
    private bool restartClickable = false;
    public override void _Ready()
    {
        restartButton.Pressed += OnTryAgainPressed;
    }

    public void OnTryAgainPressed()
    {
        EmitSignal(SignalName.TryAgainPressed);
    }
}
