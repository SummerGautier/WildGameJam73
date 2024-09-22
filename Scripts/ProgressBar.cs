using Godot;
using System;

public partial class ProgressBar : Godot.ProgressBar
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("default");

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		float new_value = Bolt.TOTAL_COLLECTED * 10;
		if (this.Value <= new_value)
		{
            var audio = this.GetNode<AudioStreamPlayer2D>("PowerUp");
            if (!audio.Playing)
            {
                audio.Play();
            }
        }
		else if (this.Value > new_value)
		{
            var audio = this.GetNode<AudioStreamPlayer2D>("PowerDown");
            if (!audio.Playing)
            {
                audio.Play();
            }
        }
		this.SetValueNoSignal(Bolt.TOTAL_COLLECTED*10);
	}
}
