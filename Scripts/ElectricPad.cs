using Godot;
using System;

public partial class ElectricPad : Area2D
{
	private AnimatedSprite2D _animation;
	private STATE _state;
	private ulong time;

	[Signal]
	public delegate void ElectrocuteAreaEventHandler(Area2D area);
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 0);
		this.Hide();
        this.AreaEntered += this.OnAreaEntered;
		this._state = STATE.INACTIVE;
		this._animation.AnimationFinished += OnAnimationFinished;
	}

	private enum STATE
	{
		ACTIVE,
		ELECTROCUTE,
		INACTIVE
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(_state == STATE.ACTIVE)
		{
            Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, (Modulate.A + ((float)delta*0.5f)));
        } 

		if(_state == STATE.INACTIVE)
		{
            Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, (Modulate.A - ((float)delta)));
        }

		if (_state == STATE.ELECTROCUTE)
		{
			EmitSignal(SignalName.ElectrocuteArea, this);
		}
		
		if(Modulate.A >= 1)
		{
			Electrocute();
		}
	}

	private void OnAnimationFinished()
	{
		this.Hide();
        this._state = STATE.INACTIVE;
    }

	private void OnAreaEntered(Area2D area)
	{
		if (area.IsInGroup("Player") && _state == STATE.INACTIVE)
		{
			this._state = STATE.ACTIVE;
            this.Show();
            this._animation.Play("Idle");
			Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 0);
        }
	}

	private void Electrocute()
	{
		if (this.Visible && this._state != STATE.ELECTROCUTE)
		{
			GD.Print("ELECTROCUTE");
			this._state = STATE.ELECTROCUTE;
			this._animation.Play("Electrocute");
			time = Time.GetTicksMsec();
		}
	}
}
