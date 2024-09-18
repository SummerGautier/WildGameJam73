using Godot;
using System;

public partial class Main : Node2D
{
	private Player _player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_player = GetNode<Player>("Player");
		// when the player jumps, the change in y value should no longer affect draw order.
		// but when the player lands and runs again, y value should re-affect draw order.
		_player.PlayerJump += DisableSortOrder;
		_player.PlayerJumpLanded += EnableSortOrder;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void DisableSortOrder(Vector2 unused)
	{
		this.YSortEnabled = false;
	}

	public void EnableSortOrder() {
		this.YSortEnabled = true; 
	}
}
