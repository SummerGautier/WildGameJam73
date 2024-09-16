﻿using Godot;
using System;


public partial class InputTranslator : Node
{
    [Signal]
    public delegate void UserJumpInputEventHandler();

    [Signal]
    public delegate void UserRunInputEventHandler(MovementSystem.Cardinal direction);
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
        }
    }

}