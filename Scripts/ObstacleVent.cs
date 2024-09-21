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
    public override void _Ready()
    {
        _random = new Random();
        _obstacles = new List<Brick>();

        _spawnPositionA = new Vector2(0, 720 - Position.Y);
        _spawnPositionB = new Vector2(-150, 830 - Position.Y);

        
        _timer = GetNode<Timer>("ObstacleSpawnTimer");
        _timer.Timeout += _OnTimerTimeout;
        _timer.Start();
        
    }

    private void _OnTimerTimeout()
    {
        EmitSignal(SignalName.BrickCreated, _CreateBrick());
        if(_random.Next(2) == 0)
        {
            EmitSignal(SignalName.BrickCreated, _CreateBrick());
        }
    }

    private Brick _CreateBrick()
    {
        Brick brick = GD.Load<PackedScene>(Brick.SCENE_PATH).Instantiate<Brick>();

        brick.Position = _random.Next(10) > 5 ? _spawnPositionA : _spawnPositionB;

        this.AddChild(brick);
        this._obstacles.Add(brick);

        return brick;
    }
}
