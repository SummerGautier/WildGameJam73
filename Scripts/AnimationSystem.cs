using Godot;
using System;

public partial class AnimationSystem : Node2D
{
    private AnimatedSprite2D _animations;

    private AnimationType _currentAnimation;
    enum AnimationType
    {
        IDLE,
        JUMP,
        RUN
    }
    public override void _Ready()
    {
        _animations = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _currentAnimation = AnimationType.IDLE;
    }

    public override void _Process(double delta)
    {
    }

    public void PlayIdle(MovementSystem.Cardinal direction)
    {
        _currentAnimation = AnimationType.IDLE;
        _animations.Play("Idle");
    }

    public void PlayRun(MovementSystem.Cardinal direction)
    {
        _currentAnimation = AnimationType.RUN;
        _animations.Play("Run");
    }

    public void PlayJump(float t, MovementSystem.Cardinal direction)
    {
        _currentAnimation = AnimationType.JUMP;
        _animations.Play("Jump");
        //_jump.Show();
        //if (!_jump.IsPlaying())
        //{
        //    _jump.Play();
        //}
        //int frame_count = _jump.SpriteFrames.GetFrameCount("default");
        //int frame = (int)Math.Ceiling( frame_count * t);/*
        //Texture2D texture = _jump.SpriteFrames.GetFrameTexture("default",frame);
        //_jump.SpriteFrames.SetFrame("default",frame, texture);*/
        //_jump.Frame = frame;
    }

}