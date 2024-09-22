using Godot;
using System;

public partial class HowToPlay : Node2D
{
    [Signal]
    public delegate void ReturnPressedEventHandler();

    [Export]
    Button ReturnButton;
    public override void _Ready()
    {
        ReturnButton.Pressed += OnReturnPressed;
    }

    public void OnReturnPressed()
    {
        EmitSignal(SignalName.ReturnPressed);
    }
}
