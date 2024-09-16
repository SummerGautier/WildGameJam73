using Godot;
using System;

public partial class Player : Area2D
{
	// Called when the node enters the scene tree for the first time.
	private InputTranslator _inputTranslator;
	private MovementSystem _movementSystem;

	public override void _Ready()
	{
		_inputTranslator = new InputTranslator();
		_movementSystem = new MovementSystem();

		GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
