using Godot;
using System;


public partial class InputTranslator : Node
{
    [Signal]
    public delegate void UserJumpInputEventHandler();

    [Signal]
    public delegate void UserRunInputEventHandler(MovementSystem.CardinalDirection direction);
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("jump"))
        {
            EmitSignal(SignalName.UserJumpInput);
        }

        MovementSystem.CardinalDirection cardinal = MovementSystem.CardinalDirection.None;

        if (Input.IsActionPressed("run_right"))
        {
            cardinal = cardinal & MovementSystem.CardinalDirection.Right;
        }

        if (Input.IsActionPressed("run_left"))
        {
            cardinal = cardinal & MovementSystem.CardinalDirection.Left;
        }

        if (Input.IsActionPressed("run_towards"))
        {
            cardinal = cardinal & MovementSystem.CardinalDirection.Towards;
        }

        if (Input.IsActionPressed("run_away"))
        {
            cardinal = cardinal & MovementSystem.CardinalDirection.Away;
        }

        if (cardinal > MovementSystem.CardinalDirection.None)
        {
            EmitSignal(SignalName.UserRunInput, (int)cardinal);
        }
    }

}