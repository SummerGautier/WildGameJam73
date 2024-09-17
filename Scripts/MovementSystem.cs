using Godot;
using System;

public partial class MovementSystem : Node
{
    // read-only variables
    [Export]
    private Player _player;
    [Export]
    private float _runSpeed = 500f;

    [ExportGroup("Jump Properties")]

    private float _inputMarginOfError;
    [Export(PropertyHint.Range, "0,100,5")]
    public float AllowedInputErrorPercentage
    {
        get { return _inputMarginOfError; }
        set
        {
            _inputMarginOfError = Mathf.Clamp(value / 100, 0, 1);
        }
    }

    
    [Export(PropertyHint.Range, "1,5,0.1")]
    public float JumpSpeedMultiplier
    {
        get { return _jumpSpeed; }
        set
        {
            _jumpSpeed = Mathf.Clamp(value / 100, 0, 1);
        }
    }

    [Export]
    float _jumpCurveWidth = 100f;
    [Export]
    float _jumpCurveHeight = 200f;

    [ExportGroup("Debug Settings")]
    [Export]
    private Graph _graph;

    // internal use-only
    private Vector2 _jumpStartPosition;
    private Vector2 _jumpEndPosition;
    private float _jumpSpeed;
    private double _jumpPathDelta;
    private Vector2 _jumpCurveControl0;
    private Vector2 _jumpCurveControl1;
    private Cardinal _direction;

    // class state management
    private MovementType _currentMovement;
    private MovementType _previousMovement;
    private MovementType _nextMovement;

    // enums, flags, statics
    private enum MovementType
    {
        IDLE,
        JUMP,
        RUN
    }

    [Flags]
    public enum Cardinal
    {
        None = 0,
        Left,
        Right,
        Towards,
        Away,
    }

    public override void _Ready()
    {
        _jumpStartPosition = Vector2.Zero;
        _jumpPathDelta = 0;
        _currentMovement = MovementType.IDLE;
        _graph = new Graph();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        switch (_currentMovement)
        {
            case MovementType.IDLE:
                if(_previousMovement != MovementType.IDLE)
                {
                    GD.Print("IDLE");
                }
                _previousMovement = MovementType.IDLE;
                break;
            case MovementType.JUMP:
                // check if jump just started
                if(_previousMovement != MovementType.JUMP)
                {
                    _DrawJumpPath();
                    GD.Print("JUMP");
                }
                // move towards jump target
                _FollowJumpCurve();
                _previousMovement = MovementType.JUMP;
                break;
            case MovementType.RUN:
                if(_previousMovement != MovementType.RUN)
                {
                    GD.Print("RUN");
                }
                _FollowRunTarget(delta);
                _previousMovement = MovementType.RUN;
                _currentMovement = MovementType.IDLE;
                break;

        }
    }

    /*
     * SIGNAL ACTION METHODS
     */

    private void _OnRunInput(MovementSystem.Cardinal  target_direction)
    {
        //only run if we're not jumping;
        if(_currentMovement != MovementType.JUMP)
        {
            _direction = target_direction;
            _currentMovement = MovementType.RUN;
        }else
        {
            _nextMovement = MovementType.RUN;
            _direction = target_direction;
        }
    }

    private void _OnJumpInput()
    {
        // only jump if we aren't already
        if (_currentMovement != MovementType.JUMP)
        {
            _ResetJump();
            _currentMovement = MovementType.JUMP;
        } else if(_jumpPathDelta > (1-_inputMarginOfError)) // if jump is 80% done, allow registration of next jump
        {
            _nextMovement = MovementType.JUMP;
        }
    }

    private void _OnIdleInput()
    {
        if(_currentMovement != MovementType.JUMP)
        {
            _currentMovement = MovementType.IDLE;
        }
    }

    public void OnJumpLand(Area2D area)
    {
        if (_currentMovement == MovementType.JUMP)
        {
            _currentMovement = _nextMovement;
            _nextMovement = MovementType.IDLE;
            _player.GlobalPosition = new Vector2(_jumpEndPosition.X, _jumpEndPosition.Y);
            _ResetJump();
        }
    }

    /*
     * HELPER METHODS & CLASSES
     */

    private void _FollowRunTarget(double delta)
    {
        
        Vector2 velocity = Vector2.Zero;
        if (_direction.HasFlag(Cardinal.Right)) { velocity.X += 1; }
        if (_direction.HasFlag(Cardinal.Left)) { velocity.X -= 1; }
        if (_direction.HasFlag(Cardinal.Towards)) { velocity.Y += 1; }
        if (_direction.HasFlag(Cardinal.Away)) { velocity.Y -= 1; }

        velocity = velocity.Normalized() * _runSpeed;
        _player.UpdatePosition(_player.Position + (velocity * (float)delta));
    }

    private void _FollowJumpCurve()
    {
        _jumpPathDelta +=  Mathf.Clamp(_jumpSpeed, 0, 1); //fraction of how far along jump curve we are so far
        Vector2 target_position = _GetPointOnJumpCurve(_jumpCurveControl0, _jumpCurveControl1, (float)_jumpPathDelta);
        _player.UpdatePosition(target_position, clampToGround: false);
        if(_player.Position.Y >= _jumpEndPosition.Y)
        {
            this.OnJumpLand(null);
        }

    }

    private void _ResetJump()
    {
        _jumpStartPosition = new Vector2(_player.GlobalPosition.X, _player.GlobalPosition.Y); //global position
        _jumpEndPosition = new Vector2(_jumpStartPosition.X + _jumpCurveWidth, _jumpStartPosition.Y);//global position

        _jumpCurveControl0 = new Vector2(_jumpStartPosition.X - 10, _jumpStartPosition.Y - _jumpCurveHeight); //relative_position;
        _jumpCurveControl1 = new Vector2(_jumpStartPosition.X + 10, _jumpStartPosition.Y - _jumpCurveHeight); //relative position;
        _jumpPathDelta = 0;
    }

    private Vector2 _GetPointOnJumpCurve(Vector2 control_point_0, Vector2 control_point_1, float t)
    {
        t = Mathf.Clamp(t, 0, 1);
        Vector2 q0 = _jumpStartPosition.Lerp(control_point_0, t);
        Vector2 q1 = control_point_0.Lerp(control_point_1, t);
        Vector2 q2 = control_point_1.Lerp(_jumpEndPosition, t);

        Vector2 r0 = q0.Lerp(q1, t);
        Vector2 r1 = q1.Lerp(q2, t);

        Vector2 s = r0.Lerp(r1, t);
        return s;
    }

    /*
     * DEBUG HELPERS
     */
    private void _DrawJumpPath()
    {
        int number_points = (int)(1 / _jumpCurveWidth) * 100;
        // generate array of points
        Vector2[] pointsOnJumpCurve = new Vector2[100];
        float point_spacing = 0.1f;
        for (int i = 0; i < pointsOnJumpCurve.Length; i++)
        {
            Vector2 vector = _GetPointOnJumpCurve(_jumpCurveControl0, _jumpCurveControl1, i * point_spacing);
            pointsOnJumpCurve[i] = new Vector2(vector.X, vector.Y);
        }

        _graph.PopulateCurve2D(pointsOnJumpCurve);
    }

}