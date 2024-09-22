using Godot;
using System;

public partial class AnimationSystem : Node2D
{
    private AnimatedSprite2D _animations;
    [Signal]
    public delegate void RiseAnimationDoneEventHandler();
    [Signal]
    public delegate void ShockAnimationDoneEventHandler();

    private AnimationType _state;
    enum AnimationType
    {
        IDLE,
        JUMP,
        RUN,
        HIT,
        RISE,
        ELECTROCUTE
    }
    public override void _Ready()
    {
        _animations = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _state = AnimationType.IDLE;
        _animations.AnimationFinished += OnAnimationFinished;
    }

    public void PlayIdle(MovementSystem.Cardinal direction)
    {
        _state = AnimationType.IDLE;
        _animations.Play("Idle");
        _animations.FlipH = direction.HasFlag(MovementSystem.Cardinal.Left);
    }

    public void PlayRun(MovementSystem.Cardinal direction)
    {
        _state = AnimationType.RUN;
        _animations.Play("Run");
        _animations.FlipH = direction.HasFlag(MovementSystem.Cardinal.Left);
    }

    public void PlayHit(MovementSystem.Cardinal direction)
    {
        if (_state != AnimationType.HIT)
        {
            _state = AnimationType.HIT;
            _animations.Play("Hit");
            _animations.FlipH = direction.HasFlag(MovementSystem.Cardinal.Left);
        }
    }

    public void PlayShock()
    {
        if(_state != AnimationType.ELECTROCUTE)
        {
            _state = AnimationType.ELECTROCUTE;
            _animations.Play("Shock");
        }
    }

    public void PlayRise()
    {
        _state = AnimationType.RISE;
        _animations.Play("Rise");
    }

    public void OnAnimationFinished()
    {
        if(_state == AnimationType.HIT)
        {
            PlayRise();
            return;
        }
        if(_state == AnimationType.RISE)
        {
            EmitSignal(SignalName.RiseAnimationDone);
            PlayIdle(MovementSystem.Cardinal.None);
            return;
        }
        if(_state == AnimationType.ELECTROCUTE)
        {
            EmitSignal(SignalName.ShockAnimationDone);
        }

    }

    public void PlayJump(float t, MovementSystem.Cardinal direction)
    {
        if (_state != AnimationType.JUMP)
        {
            _animations.FlipH = direction.HasFlag(MovementSystem.Cardinal.Left);
        }
        _state = AnimationType.JUMP;
        _animations.Play("Jump");
    }
}