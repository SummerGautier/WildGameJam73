using Godot;
using System;

public partial class Main : Node2D
{
    private Game game;
    private GameOver over;
    private TitleScreen titleScreen;
    private Winner winner;
    private HowToPlay instructions;

	//PrivateTitleScreen title;
	public override void _Ready()
	{
		TitleScreen();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    private void TitleScreen()
    {
        titleScreen = GD.Load<PackedScene>("res://Scenes/TitleScreen.tscn").Instantiate<TitleScreen>();
        this.AddChild(titleScreen);
        this.RemoveChild(over);
        this.RemoveChild(instructions);
        this.RemoveChild(winner);
		titleScreen.StartPressed += StartGame;
        titleScreen.InstructionsPressed += HowToPlay;
    }

    private void StartGame()
	{
        Bolt.TOTAL_COLLECTED = 0;
        game = GD.Load<PackedScene>("res://Scenes/Game.tscn").Instantiate<Game>();
		this.AddChild(game);
        this.RemoveChild(titleScreen);
        game.RepairFailed += GameOver;
        game.Winner += Winner;
    }
    private void HowToPlay()
    {
        instructions = GD.Load<PackedScene>("res://Scenes/HowToPlay.tscn").Instantiate<HowToPlay>();
        this.AddChild(instructions);
        this.RemoveChild(titleScreen);
        instructions.ReturnPressed += TitleScreen;
    }
    private void Winner()
    {
        winner = GD.Load<PackedScene>("res://Scenes/Winner.tscn").Instantiate<Winner>();
        this.AddChild(winner);
        this.RemoveChild(game);
        winner.ExitPressed += TitleScreen;
    }
    private void GameOver()
	{
        over = GD.Load<PackedScene>("res://Scenes/GameOver.tscn").Instantiate<GameOver>();
        over.TryAgainPressed += TitleScreen;
        this.AddChild(over);
		this.RemoveChild(game);
    }
}
