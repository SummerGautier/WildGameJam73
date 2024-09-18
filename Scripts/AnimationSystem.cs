using Godot;
using System;

public partial class AnimationSystem : Node2D
{
    private AnimatedSprite2D _animations;

    private AnimationType _state;
    enum AnimationType
    {
        IDLE,
        JUMP,
        RUN
    }
    public override void _Ready()
    {
        _animations = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _state = AnimationType.IDLE;
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