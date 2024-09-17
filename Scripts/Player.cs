using Godot;
using System;

public partial class Player : Area2D
{
	// Called when the node enters the scene tree for the first time.
	private InputTranslator _inputTranslator;
	private MovementSystem _movementSystem;

	[Signal]
	public delegate void PlayerRunEventHandler(MovementSystem.Cardinal direction, Vector2 startPosition);
	[Signal]
	public delegate void PlayerJumpEventHandler(Vector2 startPosition);
	[Signal]
	public delegate void PlayerJumpLandedEventHandler();

	[Export]
	public float PLAYER_FEET_HEIGHT = 30f;
	[Export]
	public float PLAYER_WIDTH = 256;
	[Export]
	public float PLAYER_HEIGHT = 384;
	[Export]
	private AssemblyLine _assemblyLine;
	private Rect2 _runBounds;
	public override void _Ready()
	{
		//initialize systems
		_inputTranslator = GetNode<InputTranslator>("PlayerInput");
		_movementSystem = GetNode<MovementSystem>("PlayerMovement");
        _runBounds = _assemblyLine.GetBoundary();

        //subscribe systems to player signals
        this.PlayerRun += _movementSystem.OnEntityRun;
		this.PlayerJump += _movementSystem.OnEntityJump;
		this.PlayerJumpLanded += _movementSystem.OnEntityJumpLanded;

		//subscribe player to systems signals
		_movementSystem.JumpPositionUpdate += this.UpdateJumpPosition;
		_movementSystem.MovePositionUpdate += this.UpdatePosition;
		_inputTranslator.UserRunInput += this._OnRunInput;
		_inputTranslator.UserJumpInput += this._OnJumpInput;

		//render animations
        GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) { }


	// Screen Boundary Positions Adjusted To Player Size
    public float GetMinimumPlayerX() {  return PLAYER_WIDTH / 2; }
    public float GetMinimumPlayerY() { return PLAYER_HEIGHT / 2; }
    public float GetMaximumPlayerX() { return DisplayServer.WindowGetSize().X - (PLAYER_WIDTH / 2); }
    public float GetMaximumPlayerY() { return DisplayServer.WindowGetSize().Y - (PLAYER_HEIGHT / 2); }

    public float GetMinimumGroundX() { return _runBounds.Position.X; }
    public float GetMinimumGroundY() { return _runBounds.Position.Y - (PLAYER_HEIGHT / 2) - PLAYER_FEET_HEIGHT*2; } //feet poke up halfway
    public float GetMaximumGroundX() { return _runBounds.End.X; }
    public float GetMaximumGroundY() { return _runBounds.End.Y - (PLAYER_HEIGHT * (float)0.75) + PLAYER_FEET_HEIGHT; }

    public void UpdatePosition(Vector2 position, bool clampToScreen = true, bool clampToGround = true)
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
	}

	public void UpdateJumpPosition(Vector2 position, Vector2 end, float jump_path_delta)
	{
		//update position
		UpdatePosition(position, clampToGround: false);
        //send signal to animation system

        if (Position.Y >= end.Y) //player has landed
        {
            EmitSignal(SignalName.PlayerJumpLanded);
        }
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
