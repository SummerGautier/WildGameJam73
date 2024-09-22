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
	private AnimatedSprite2D _animation;
	private STATE _state;
	public Vector2 _spawnPoint;
	private Bolt _bolt;
	private enum STATE
	{
		FALLING, 
		MOVING,
		BREAKING
	}

	private ulong start;
	public override void _Ready()
	{
		/*
		 * Initialize Systems
		 */
		_animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _movementSystem = GetNode<MovementSystem>("BrickMovement");
		_visibleOnScreenNotifier = GetNode<VisibleOnScreenNotifier2D>("BrickVisibleNotifier");
		_floorCollisionArea2D = GetNode<Area2D>("BrickFloorArea");
		_bodyCollisionArea2D = this;
		_animation.Play("Mold");
		_state = STATE.FALLING;
		/*
		 * Subscribe systems to brick signals
		 */
		this.BrickMove += _movementSystem.OnEntityRun;
		this._spawnPoint = new Vector2(1000, 1000);
		/*
		 * Subscribe brick to system signals
		 */
		_movementSystem.MovePositionUpdate += this.OnUpdatePosition;
		_visibleOnScreenNotifier.ScreenExited += this.OnScreenExit;
		_animation.AnimationFinished += this._OnAnimationFinished;

		start = Time.GetTicksMsec();
	
	}

	public override void _Process(double delta)
	{
		if(Time.GetTicksMsec() - start < 1000)
		{
			this._movementSystem._runSpeed = 100;
		}else
		{
			this._movementSystem._runSpeed = 1050;
		}
		if (_state == STATE.FALLING)
		{
            EmitSignal(SignalName.BrickMove, (int)MovementSystem.Cardinal.Towards, Position);
			if(GlobalPosition.Y > _spawnPoint.Y)
			{
				this._state = STATE.MOVING;
				this._movementSystem._runSpeed /= 3;
			}
        }
		if (_state == STATE.MOVING)
		{
			EmitSignal(SignalName.BrickMove, (int)MovementSystem.Cardinal.Left, Position);
		}
		
		if (this.IsInGroup("BrickBroken") || _floorCollisionArea2D.IsInGroup("BrickBroken"))
		{
			this._OnBrickBroken();
		}
	}

	/*
	 * Signal Action
	 */
	public void OnUpdatePosition(Vector2 position)
	{
		if (_state == STATE.MOVING || _state == STATE.FALLING)
		{
			this._SetPosition(position);
		}
	}

    public void OnScreenExit()
    {;
        this._Cleanup();
    }

	private void _OnBrickBroken()
	{
		_bolt.Hide();
		this._state = STATE.BREAKING;
		this._animation.Play("Broken");
        var audio = this.GetNode<AudioStreamPlayer2D>("Break");
        if (!audio.Playing)
        {
            audio.Play();
        }

    }

	private void _OnAnimationFinished()
	{
		if(this._state == STATE.BREAKING)
		{
			this._Cleanup();
			return;
		}

		if(this._state == STATE.FALLING)
		{
			_CreateBolt();
			this._animation.Play("Idle");
		}
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

	private void _CreateBolt()
	{
        Bolt bolt = GD.Load<PackedScene>(Bolt.SCENE_PATH).Instantiate<Bolt>();
        this.AddChild(bolt);
        _bolt = bolt;
    }
}
