using Godot;
using System;

public partial class Player : Area2D
{
	// Called when the node enters the scene tree for the first time.
	private InputTranslator _inputTranslator;
	private MovementSystem _movementSystem;
	private AnimationSystem _animationSystem;

	[Signal]
	public delegate void PlayerRunEventHandler(MovementSystem.Cardinal direction, Vector2 startPosition);
	[Signal]
	public delegate void PlayerJumpEventHandler(Vector2 startPosition);
	[Signal]
	public delegate void PlayerJumpLandedEventHandler();
    [Signal]
    public delegate void PlayerIdleEventHandler();

    [Signal]
	public delegate void PlayerJumpAnimationEventHandler(float delta, MovementSystem.Cardinal direction);
    [Signal]
    public delegate void PlayerRunAnimationEventHandler(MovementSystem.Cardinal direction);
    [Signal]
    public delegate void PlayerIdleAnimationEventHandler(MovementSystem.Cardinal direction);

    [Export]
	public float PLAYER_FEET_HEIGHT = 30f;
	[Export]
	public float PLAYER_FEET_WIDTH = 100f;
	[Export]
	public float PLAYER_WIDTH = 256;
	[Export]
	public float PLAYER_HEIGHT = 384;
	[Export]
	private AssemblyLine _assemblyLine;
	private Rect2 _runBounds;
	public override void _Ready()
	{
		/*
		 * Initialize Systems
		 */
		_inputTranslator = GetNode<InputTranslator>("PlayerInput");
		_movementSystem = GetNode<MovementSystem>("PlayerMovement");
		_animationSystem = GetNode<AnimationSystem>("PlayerAnimation");
        _runBounds = _assemblyLine.GetBoundary();

        /* 
         * Subscribe systems to player signals
         */

		// send movement events
        this.PlayerRun += _movementSystem.OnEntityRun;
		this.PlayerJump += _movementSystem.OnEntityJump;
		this.PlayerJumpLanded += _movementSystem.OnEntityJumpLanded;

		// send animation events
		this.PlayerJumpAnimation += _animationSystem.PlayJump;
		this.PlayerIdleAnimation += _animationSystem.PlayIdle;
		this.PlayerRunAnimation += _animationSystem.PlayRun;

		/*
		 * Subscribe player to system signals
		 */
		// read movement signals
		_movementSystem.JumpPositionUpdate += this.UpdateJumpPosition;
		_movementSystem.MovePositionUpdate += this.UpdateMovePosition;
		_movementSystem.Idle += this.UpdateIdlePosition;

		// read input signals
		_inputTranslator.UserRunInput += this._OnRunInput;
		_inputTranslator.UserJumpInput += this._OnJumpInput;
		_inputTranslator.UserIdleInput += this._OnIdleInput;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) { }


	// Screen Boundary Positions Adjusted To Player Size
    public float GetMinimumPlayerX() { return PLAYER_FEET_WIDTH/2; }
    public float GetMinimumPlayerY() { return PLAYER_FEET_HEIGHT / 2; }
    public float GetMaximumPlayerX() { return DisplayServer.WindowGetSize().X - (PLAYER_FEET_WIDTH / 2); }
    public float GetMaximumPlayerY() { return DisplayServer.WindowGetSize().Y - (PLAYER_FEET_HEIGHT / 2); }

    public float GetMinimumGroundX() { return _runBounds.Position.X; }
    public float GetMinimumGroundY() { return _runBounds.Position.Y; }
    public float GetMaximumGroundX() { return _runBounds.End.X - (PLAYER_FEET_WIDTH / 2); }
    public float GetMaximumGroundY() { return _runBounds.End.Y - (PLAYER_FEET_HEIGHT / 2); }

    public void SetPosition(Vector2 position, bool clampToScreen = true, bool clampToGround = true)
	{
        Position = position;
		if (clampToScreen)
		{
            //clamp to the screen
            Position = new Vector2(
                x: Mathf.Clamp(Position.X, GetMinimumPlayerX(), GetMaximumPlayerX()),
                y: Mathf.Clamp(Position.Y, GetMinimumPlayerY(), GetMaximumPlayerY())
            );
        }
		if (clampToGround)
		{
			//clamp to the run area
			Rect2 run_bounds = _assemblyLine.GetBoundary();
			Position = new Vector2(
				x: Mathf.Clamp(Position.X, GetMinimumGroundX(), GetMaximumGroundX()),
				y: Mathf.Clamp(Position.Y, GetMinimumGroundY(), GetMaximumGroundY())
			);
		}

		//TODO: Update animation with _movementSystem.GetDirection() to determine which one to do
	}

	public void UpdateMovePosition(Vector2 position)
	{
		EmitSignal(SignalName.PlayerRunAnimation, (int)_movementSystem.GetDirection());
		SetPosition(position, clampToScreen: true, clampToGround:true);
	}

	public void UpdateJumpPosition(Vector2 position, Vector2 end, float jump_path_delta)
	{
        EmitSignal(SignalName.PlayerJumpAnimation, jump_path_delta, (int)_movementSystem.GetDirection());
        SetPosition(position, clampToScreen: true, clampToGround: false);

        if (jump_path_delta > 1)
		{
            EmitSignal(SignalName.PlayerJumpLanded);
			return;
        }
    }

	public void UpdateIdlePosition()
	{
        EmitSignal(SignalName.PlayerIdleAnimation, (int)_movementSystem.GetDirection());
    }

    private void _OnIdleInput()
    {
		EmitSignal(SignalName.PlayerIdle);
    }

    private void _OnRunInput(MovementSystem.Cardinal cardinal)
	{
		EmitSignal(SignalName.PlayerRun, (int)cardinal, Position);
	}

    private void _OnJumpInput()
    {
        EmitSignal(SignalName.PlayerJump, GlobalPosition);
    }

}
