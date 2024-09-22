using Godot;
using System;

public partial class Winner : Node2D
{
        [Signal]
        public delegate void ExitPressedEventHandler();

        [Export]
        Button ExitButton;
        public override void _Ready()
        {
            ExitButton.Pressed += OnExitPressed;
        }

        public void OnExitPressed()
        {
            EmitSignal(SignalName.ExitPressed);
        }
}
