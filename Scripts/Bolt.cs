using Godot;
using System;

public partial class Bolt : Area2D
{
	private AnimatedSprite2D _animation;
    // Called when the node enters the scene tree for the first time.
    public static string SCENE_PATH = "res://Scenes/Bolt.tscn";
    private float time = 0.0f;
    private float speed = 10;
    private float distance_from_center = 5;
    public static float TOTAL_COLLECTED = 0;
    private int value = 3;
    public override void _Ready()
	{
        _animation = GetNode<AnimatedSprite2D>("sprite1");
        _animation.Play("default");

        Random random = new Random();
        if(random.Next(3) == 1)
        {
            value--;
            _animation.Hide();
        }
        _animation = GetNode<AnimatedSprite2D>("sprite2");
        _animation.Play("default");

        if (random.Next(3) == 2)
        {
            value--;

            _animation.Hide();
        }

        _animation = GetNode<AnimatedSprite2D>("sprite3");
        _animation.Play("default");

        if (random.Next(3) == 1)
        {

            value--;
            _animation.Hide();
        }

        this.AreaEntered += OnAreaEntered;
        this.Position = new Vector2(0, -150);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        _CircularMove(delta);
	}

    public void OnAreaEntered(Area2D area)
    {
        if (area.IsInGroup("PlayerFoot"))
        {
            this.Hide();
            TOTAL_COLLECTED+=value;
            var audio = this.GetNode<AudioStreamPlayer2D>("collect");
            if (!audio.Playing)
            {
                audio.Play();
            }
        }
    }

    private void _CircularMove(double delta)
    {

        time += (float)delta;

        var angle = speed * time;

        var rotation = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

       Position = rotation * distance_from_center;
        Position = new Vector2(Position.X, Position.Y - 150);
    }

    public void _cleanup()
    {
        this.QueueFree();
    }
}
