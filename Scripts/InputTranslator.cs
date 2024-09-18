using Godot;
using System;


public partial class InputTranslator : Node
{
    [Signal]
    public delegate void UserJumpInputEventHandler();
    [Signal]
    public delegate void UserRunInputEventHandler(MovementSystem.Cardinal direction);
    [Signal]
    public delegate void UserIdleInputEventHandler();

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("jump"))
        {
            EmitSignal(SignalName.UserJumpInput);
            return;
        }

        MovementSystem.Cardinal cardinal = MovementSystem.Cardinal.None;

        if (Input.IsActionPressed("run_right"))
        {
            cardinal = cardinal | MovementSystem.Cardinal.Right;
        }

        if (Input.IsActionPressed("run_left"))
        {
            cardinal = cardinal | MovementSystem.Cardinal.Left;
        }

        if (Input.IsActionPressed("run_towards"))
        {
            cardinal = cardinal | MovementSystem.Cardinal.Towards;
        }

        if (Input.IsActionPressed("run_away"))
        {
            cardinal = cardinal | MovementSystem.Cardinal.Away;
        }

        if (cardinal > MovementSystem.Cardinal.None)
        {
            EmitSignal(SignalName.UserRunInput, (int)cardinal);
            return;
        }

        EmitSignal(SignalName.UserIdleInput);
    }
}