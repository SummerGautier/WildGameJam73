using Godot;
using System;

public partial class Game : Node2D
{
    private Player _player;
    private ObstacleVent _obstacleVent;
    [Export]
    private ProgressBar _progress;
    [Signal]
    public delegate void WinnerEventHandler();
    private enum STATE
    {
        FADEIN,
        PLAY,
        FADEOUT,
    }
    private STATE _state;

    [Signal]
    public delegate void RepairFailedEventHandler();
    public override void _Ready()
    {
        this._state = STATE.FADEIN;
        this.Modulate = new Color(0, 0, 0,0);
    }

    public override void _Process(double delta)
    {
        if (_state == STATE.FADEIN)
        {
            
            var tween = CreateTween();
            tween.TweenProperty(this, "modulate", new Color(1,1,1,1),0.5).From(new Color(0,0,0,0.1f));
            tween.SetSpeedScale(2f);
            tween.SetEase(Tween.EaseType.Out);
            tween.Finished += _startLevel;
        }
        if(_progress.Ratio >= 1)
        {
            EmitSignal(SignalName.Winner);
        }
    }

    private void _startLevel()
    {
        this._state = STATE.PLAY;
        this._InitPlayer();
        this._InitObstacleVent();
    }
    /*
     * Helpers & Math
     */
    private void _DisableSortOrder(Vector2 unused)
    {
        this.YSortEnabled = false;
    }

    private void _EnableSortOrder()
    {
        this.YSortEnabled = true;
    }

    private void _InitPlayer()
    {
        _player = GetNode<Player>("Player");
        _player.PlayerJump += _DisableSortOrder;
        _player.PlayerJumpLanded += _EnableSortOrder;
    }

    private void _InitObstacleVent()
    {
        _obstacleVent = GetNode<ObstacleVent>("ObstacleVent");
    }

    public void OnLevelTimerTimeout()
    {
        if (_progress.Ratio < 1)
        {
            EmitSignal(SignalName.RepairFailed);
        }
    }
}
