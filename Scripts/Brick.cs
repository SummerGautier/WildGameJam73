using Godot;
using System;

public partial class Brick : Area2D
{
	[Signal]
	public delegate void BrickMoveEventHandler(MovementSystem.Cardinal direction, Vector2 start_position);
	[Signal]
	public delegate void BrickJumpLandedEventHandler();

	private MovementSystem _movementSystem;
	private VisibleOnScreenNotifier2D _visibleOnScreenNotifier;
	public override void _Ready()
	{
        /*
		 * Initialize Systems
		 */
        _movementSystem = GetNode<MovementSystem>("BrickMovement");
		_visibleOnScreenNotifier = GetNode<VisibleOnScreenNotifier2D>("BrickVisibleNotifier");

		/*
		 * Subscribe systems to brick signals
		 */
		this.BrickMove += _movementSystem.OnEntityRun;
		this.BrickJumpLanded += _movementSystem.OnEntityJumpLanded;
		/*
		 * Subscribe brick to system signals
		 */
		_movementSystem.MovePositionUpdate += this.OnUpdatePosition;
		_movementSystem.JumpPositionUpdate += this.OnUpdateJumpPosition;
		_visibleOnScreenNotifier.ScreenExited += this.OnScreenExit;
	}

	public override void _Process(double delta)
	{
		EmitSignal(SignalName.BrickMove, (int)MovementSystem.Cardinal.Left, Position);
		if (Time.GetTicksMsec()%15 == 0 & Time.GetTicksMsec() > 5000)
		{
			_movementSystem.OnEntityJump(Position);
		}
	}

	/*
	 * Signal Action
	 */
	public void OnUpdatePosition(Vector2 position)
	{
		this._SetPosition(position);
	}
	public void OnUpdateJumpPosition(Vector2 position, Vector2 end, float jump_path_delta)
	{
		this._SetPosition(position);
        if (jump_path_delta > 0.8)
        {
            EmitSignal(SignalName.BrickJumpLanded);
            return;
        }
    }
    public void OnScreenExit()
    {
		GD.Print("Brick deleted");
        this._Cleanup();
    }

    /*
	 * Helpers & Math Methods
	 */
    private void _SetPosition(Vector2 position)
	{
		Position = position;
	}
	private void _Cleanup()
	{
		QueueFree();
	}
}
