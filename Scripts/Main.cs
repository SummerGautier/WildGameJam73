using Godot;
using System;

public partial class Main : Node2D
{
    private Player _player;
    private ObstacleVent _obstacleVent;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this._InitPlayer();
        this._InitObstacleVent();
    }

    /*
     * Signal Action
     */

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

}
