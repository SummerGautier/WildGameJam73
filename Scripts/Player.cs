using Godot;
using System;
using System.Runtime.CompilerServices;
using static MovementSystem;

public partial class Player : Area2D
{
    // Collection of Component Systems
    private InputTranslator _inputTranslator;
    private MovementSystem _movementSystem;
    private AnimationSystem _animationSystem;
    private PlayerFoot _foot;

    private Rect2 _runBounds;
    private bool _movementEnabled = true;
    private Sprite2D _shadow;
    private bool shock = false;

    [Signal]
    public delegate void PlayerRunEventHandler(MovementSystem.Cardinal direction, Vector2 start_position);
    [Signal]
    public delegate void PlayerJumpEventHandler(Vector2 start_position);
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
    private float _playerFeetHeight = 30f;
    [Export]
    private float _playerFeetWidth = 100f;
    [Export]
    private float _playerBodyHeight = 256;
    [Export]
    private float _playerBodyWidth = 384;
    [Export]
    private AssemblyLine _assemblyLine;

    public override void _Ready()
    {
        /*
		 * Initialize Systems
		 */
        _inputTranslator = GetNode<InputTranslator>("PlayerInput");
        _movementSystem = GetNode<MovementSystem>("PlayerMovement");
        _animationSystem = GetNode<AnimationSystem>("PlayerAnimation");
        _foot = GetNode<PlayerFoot>("PlayerFootArea");
        _runBounds = _assemblyLine.GetBoundary();

        //shadow asset
        this._InitShadow();

        /* 
         * Subscribe systems to player signals
         */

        // send movement events
        this.PlayerRun += _movementSystem.OnEntityRun;
        this.PlayerJump += _movementSystem.OnEntityJump;
        this.PlayerJump += _foot.OnPlayerJump;
        this.PlayerJumpLanded += _movementSystem.OnEntityJumpLanded;
        this.PlayerJumpLanded += _foot.OnPlayerJumpLanded;

        // send animation events
        this.PlayerJumpAnimation += _animationSystem.PlayJump;
        this.PlayerIdleAnimation += _animationSystem.PlayIdle;
        this.PlayerRunAnimation += _animationSystem.PlayRun;

        /*
		 * Subscribe player to system signals
		 */
        // read movement signals
        _movementSystem.JumpPositionUpdate += this.UpdateJumpPosition;
        _movementSystem.JumpPositionUpdate += this._UpdateJumpShadow;
        _movementSystem.MovePositionUpdate += this.UpdateMovePosition;
        _movementSystem.Idle += this.UpdateIdlePosition;

        // read input signals
        _inputTranslator.UserRunInput += this.OnRunInput;
        _inputTranslator.UserJumpInput += this.OnJumpInput;
        _inputTranslator.UserIdleInput += this.OnIdleInput;

        // read collision events
        _foot.PlayerCollidedWithBrick += this.OnBrickCollision;
        _foot.PlayerCollidedWithAssembler+= this.OnAssemblerCollision;

        // read animation events
        _animationSystem.RiseAnimationDone += this.OnRiseAnimationDone;
        _animationSystem.ShockAnimationDone += this.OnShockAnimationDone;
    }

    private void OnElectrocute(Area2D area)
    {
        if (this.OverlapsArea(area))
        {
            if(shock == false)
            {
                Bolt.TOTAL_COLLECTED *= 0.5f;
                var audio = this.GetNode<AudioStreamPlayer2D>("Shock");
                if (!audio.Playing)
                {
                    audio.Play();
                }
            }
            shock = true;
            _movementEnabled = false;
            this._animationSystem.PlayShock();
        }
    }

    /**
	 * Movement Signal Actions
	 */
    public void UpdateIdlePosition()
    {
        if (_movementEnabled)
        {
            EmitSignal(SignalName.PlayerIdleAnimation, (int)_movementSystem.GetDirection());
        }
    }
    public void UpdateMovePosition(Vector2 position)
    {
        if (_movementEnabled)
        {
            _SetPosition(position, clamp_on_screen: true, clamp_on_ground: true);
            EmitSignal(SignalName.PlayerRunAnimation, (int)_movementSystem.GetDirection());
        }

    }
    public void UpdateJumpPosition(Vector2 position, Vector2 end, float jump_path_delta)
    {
        if (_movementEnabled)
        {
            _SetPosition(position, clamp_on_screen: true, clamp_on_ground: false);
            EmitSignal(SignalName.PlayerJumpAnimation, jump_path_delta, (int)_movementSystem.GetDirection());

            if (jump_path_delta > 1)
            {
                EmitSignal(SignalName.PlayerJumpLanded);
                return;
            }
        }
    }

    /**
	 * Input Signals Actions
	 */
    public void OnIdleInput()
    {
        if (_movementEnabled)
        {
            EmitSignal(SignalName.PlayerIdle);
        }
        
    }
    public void OnRunInput(Cardinal cardinal)
    {
        if (_movementEnabled)
        {
            EmitSignal(SignalName.PlayerRun, (int)cardinal, Position);
        }
    }
    public void OnJumpInput()
    {
        if (_movementEnabled)
        {
            EmitSignal(SignalName.PlayerJump, GlobalPosition);
        }
    }

    /**
     * Collision Signals Actions
     */
    public void OnBrickCollision()
    {
        if (_movementEnabled)
        {
            Bolt.TOTAL_COLLECTED *= 0.8f;
        }
        this._movementEnabled = false;
        GD.Print("brick collision");
        _animationSystem.PlayHit(_movementSystem.GetDirection());
        var audio = this.GetNode<AudioStreamPlayer2D>("Grunt");
        if (!audio.Playing)
        {
            audio.Play();
        }
    }
    public void OnAssemblerCollision()
    {
        if (shock)
        {
            return;
        }
        float speed = (_movementEnabled) ? 5f : 2f;
        _SetPosition(new Vector2(Position.X - speed, Position.Y));
        
    }

    /**
     *  Animation Signal Actions
     */
    public void OnRiseAnimationDone()
    {
        this._movementEnabled = true;
        EmitSignal(SignalName.PlayerIdle);
    }
    public void OnShockAnimationDone()
    {
        shock = false;
       this.OnBrickCollision();
    }


    /**
	 * Helpers
	 */

    private void _InitShadow()
    {
        this._shadow = new Sprite2D();
        this.AddChild(this._shadow);
        _shadow.Texture = GD.Load<Texture2D>("res://Art/player_shadow.png");
        _shadow.Position = new Vector2(0, 0);
        _shadow.ZIndex = 5;
        _shadow.Scale = new Vector2(0.5f, 0.5f);
        _shadow.Hide();
    }
    private void _UpdateJumpShadow(Vector2 position, Vector2 end, float jump_path_delta)
    {
        _shadow.Show();
        _shadow.GlobalPosition = new Vector2(position.X, end.Y);
        float shadow_scale = Mathf.Abs(5 * Mathf.Sin(1 * jump_path_delta * 2f * (float)Math.PI) / (2 * (float)Math.PI));
        _shadow.Scale = new Vector2(shadow_scale, shadow_scale);
    }
    public PlayerFoot Foot()
    {
        return this._foot;
    }

    // Screen Boundary Positions Adjusted To Player Size
    private float _GetMinimumPlayerX() { return _playerFeetWidth / 2; }
    private float _GetMinimumPlayerY() { return _playerFeetHeight / 2; }
    private float _GetMaximumPlayerX() { return DisplayServer.WindowGetSize().X - (_playerFeetWidth / 2); }
    private float _GetMaximumPlayerY() { return DisplayServer.WindowGetSize().Y - (_playerFeetHeight / 2); }
    private float _GetMinimumGroundX() { return _runBounds.Position.X; }
    private float _GetMinimumGroundY() { return _runBounds.Position.Y; }
    private float _GetMaximumGroundX() { return _runBounds.End.X - (_playerFeetWidth / 2); }
    private float _GetMaximumGroundY() { return _runBounds.End.Y - (_playerFeetHeight / 2); }

    private void _SetPosition(Vector2 position, bool clamp_on_screen = true, bool clamp_on_ground = true)
    {
        Position = position;
        if (clamp_on_screen)
        {
            //clamp to the screen
            Position = new Vector2(
                x: Mathf.Clamp(Position.X, _GetMinimumPlayerX(), _GetMaximumPlayerX()),
                y: Mathf.Clamp(Position.Y, _GetMinimumPlayerY(), _GetMaximumPlayerY())
            );
        }
        if (clamp_on_ground)
        {
            //clamp to the run area
            Rect2 run_bounds = _assemblyLine.GetBoundary();
            Position = new Vector2(
                x: Mathf.Clamp(Position.X, _GetMinimumGroundX(), _GetMaximumGroundX()),
                y: Mathf.Clamp(Position.Y, _GetMinimumGroundY(), _GetMaximumGroundY())
            );
        }
    }
}
