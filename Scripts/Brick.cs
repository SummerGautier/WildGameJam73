using Godot;
using System;
using System.Security.Cryptography;

public partial class Brick : Area2D
{
	public static string SCENE_PATH = "res://Scenes/Brick.tscn";

	// Brick Signals
	[Signal]
	public delegate void BrickMoveEventHandler(MovementSystem.Cardinal direction, Vector2 start_position);

	// Brick Systems
	private MovementSystem _movementSystem;
	private VisibleOnScreenNotifier2D _visibleOnScreenNotifier;
	private Area2D _floorCollisionArea2D;
	private Area2D _bodyCollisionArea2D;
	public override void _Ready()
	{
        /*
		 * Initialize Systems
		 */
        _movementSystem = GetNode<MovementSystem>("BrickMovement");
		_visibleOnScreenNotifier = GetNode<VisibleOnScreenNotifier2D>("BrickVisibleNotifier");
		_floorCollisionArea2D = GetNode<Area2D>("BrickFloorArea");
		_bodyCollisionArea2D = this;
		/*
		 * Subscribe systems to brick signals
		 */
		this.BrickMove += _movementSystem.OnEntityRun;
		/*
		 * Subscribe brick to system signals
		 */
		_movementSystem.MovePositionUpdate += this.OnUpdatePosition;
		_visibleOnScreenNotifier.ScreenExited += this.OnScreenExit;
	}

	public override void _Process(double delta)
	{
		EmitSignal(SignalName.BrickMove, (int)MovementSystem.Cardinal.Left, Position);
	}

	/*
	 * Signal Action
	 */
	public void OnUpdatePosition(Vector2 position)
	{
		this._SetPosition(position);
	}

    public void OnScreenExit()
    {
		GD.Print("Brick deleted");
        this._Cleanup();
    }

    /*
	 * Helpers & Math Methods
	 */
	public Area2D FloorCollisionArea2D()
	{
		return this._floorCollisionArea2D;
	}
	public Area2D BodyCollisionArea2D()
	{
		return this._bodyCollisionArea2D;
	}

    private void _SetPosition(Vector2 position)
	{
		Position = position;
	}

	private void _Cleanup()
	{
		QueueFree();
	}
}
