using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class ObstacleVent : Node2D
{
    [Signal]
    public delegate void BrickCreatedEventHandler(Brick brick);

    private List<Brick> _obstacles;
    private Timer _timer;
    private Vector2 _spawnPositionA;
    private Vector2 _spawnPositionB;
    private Random _random;
    private AnimatedSprite2D _animation;

    private float time = 0.0f;
    private float speed = 10;
    private float distance_from_center = 20;

    public override void _Ready()
    {
        _random = new Random();
        _obstacles = new List<Brick>();
        _animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _animation.Play("default");
        _spawnPositionA = new Vector2(110, 100);
        _spawnPositionB = new Vector2(165, 100);

        _timer = GetNode<Timer>("ObstacleSpawnTimer");
        _timer.Timeout += _OnTimerTimeout;
        _timer.Start();

        this._animation.AnimationFinished += this.OnAnimationFinished;
    }

    public override void _Process(double delta)
    {
        _CircularMove(delta);
    }

    private void _OnTimerTimeout()
    {
        EmitSignal(SignalName.BrickCreated, _CreateBrick());
        if(_random.Next(2) == 0)
        {
            EmitSignal(SignalName.BrickCreated, _CreateBrick());
        }
    }

    private void _CircularMove(double delta)
    {

        time += (float)delta;

        var angle = speed * time;

        var rotation = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        _animation.Position = rotation * distance_from_center;
    }
    private void OnAnimationFinished()
    {
        _animation.Play("default");
    }
    private Brick _CreateBrick()
    {
        Brick brick = GD.Load<PackedScene>(Brick.SCENE_PATH).Instantiate<Brick>();
        brick.Position = _random.Next(10) > 5 ? _spawnPositionA : _spawnPositionB;
        this.AddChild(brick);
        this._obstacles.Add(brick);
        brick._spawnPoint = brick.Position == _spawnPositionA ? new Vector2(150, 720) : new Vector2(200, 830);
        _animation.Play("Warmup");
        return brick;
    }

}
