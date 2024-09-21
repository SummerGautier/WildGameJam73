using Godot;
using System;

public partial class MovementSystem : Node
{
    [Signal]
    public delegate void JumpPositionUpdateEventHandler(Vector2 position, Vector2 end, float delta);
    [Signal]
    public delegate void MovePositionUpdateEventHandler(Vector2 position);
    [Signal]
    public delegate void IdleEventHandler();

    [ExportGroup("Run Properties")]
    [Export]
    private float _runSpeed = 500f;

    [ExportGroup("Jump Properties")]
    [Export]
    private float _jumpCurveWidth = 100f;
    [Export]
    private float _jumpCurveHeight = 200f;
    [Export]
    private Vector2 _controlPoint0Offset = new Vector2(10f, -100f);
    [Export]
    private Vector2 _controlPoint1Offset = new Vector2(100f, 0);

    [Export(PropertyHint.Range, "1,5,0.1")]
    private float _jumpSpeedMultiplier
    {
        get
        {
            return _jumpSpeed;
        }
        set
        {
            _jumpSpeed = Mathf.Clamp(value / 100, 0, 1);
        }
    }

    [Export(PropertyHint.Range, "0,100,5")]
    public float _allowedInputErrorPercentage
    {
        get
        {
            return _inputMarginOfError;
        }
        set
        {
            _inputMarginOfError = Mathf.Clamp(value / 100, 0, 1);
        }
    }

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

    private Vector2 _runStartPosition;
    private Cardinal _direction;
    private float _inputMarginOfError;


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
                if (_previousMovement != MovementType.IDLE)
                {
                    EmitSignal(SignalName.Idle);
                }
                _previousMovement = MovementType.IDLE;
                break;
            case MovementType.JUMP:
                // check if jump just started
                if (_previousMovement != MovementType.JUMP)
                {
                    
                    GD.Print("JUMP");
                }
                _DrawJumpPath();
                // move towards jump target
                _FollowJumpCurve();
                _previousMovement = MovementType.JUMP;
                break;
            case MovementType.RUN:
                if (_previousMovement != MovementType.RUN)
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

    public void OnEntityRun(MovementSystem.Cardinal target_direction, Vector2 startPosition)
    {
        //only run if we're not jumping;
        if (_currentMovement != MovementType.JUMP)
        {
            _currentMovement = MovementType.RUN;
        }
        else if (_jumpPathDelta > (1 - _inputMarginOfError))
        {
            _nextMovement = MovementType.RUN;
        }

        if (_currentMovement == MovementType.JUMP)
        {
            if (_jumpEndPosition.X > _jumpStartPosition.X && target_direction == Cardinal.Left)
            {
                _jumpEndPosition.X -= 10;
            } else if (_jumpEndPosition.X < _jumpStartPosition.X && target_direction == Cardinal.Right)
            {
                _jumpEndPosition.X += 10;
            }
        }

        _runStartPosition = startPosition;
        _direction = target_direction;
    }

    public void OnEntityJump(Vector2 start_position)
    {
        // only jump if we aren't already
        if (_currentMovement != MovementType.JUMP)
        {
            _ResetJump(start_position);
            _currentMovement = MovementType.JUMP;
        }
        else if (_jumpPathDelta > (1 - _inputMarginOfError)) // if jump is 80% done, allow registration of next jump
        {
            _nextMovement = MovementType.JUMP;
        }
    }

    public void OnEntityJumpLanded()
    {
        if (_currentMovement == MovementType.JUMP)
        {
            _ResetJump(_jumpEndPosition);
            _currentMovement = _nextMovement;
            _nextMovement = MovementType.IDLE;

        }
    }

    private void _OnEntityIdle()
    {
        if (_currentMovement != MovementType.JUMP)
        {
            _currentMovement = MovementType.IDLE;
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
        if (_direction.HasFlag(Cardinal.Towards)) { velocity.Y += 0.5f; }
        if (_direction.HasFlag(Cardinal.Away)) { velocity.Y -= 0.5f; }

        velocity = velocity.Normalized() * _runSpeed;
        Vector2 target_position = _runStartPosition + (velocity * (float)delta);
        EmitSignal(SignalName.MovePositionUpdate, target_position);
    }

    private void _FollowJumpCurve()
    {
        _jumpPathDelta += Mathf.Clamp(_jumpSpeed, 0, 1); //fraction of how far along jump curve we are so far
        Vector2 target_position = _GetPointOnJumpCurve(_jumpCurveControl0, _jumpCurveControl1, (float)_jumpPathDelta);
        EmitSignal(SignalName.JumpPositionUpdate, target_position, _jumpEndPosition, _jumpPathDelta);

    }

    private void _ResetJump(Vector2 start_position)
    {
        int offset = (_direction.HasFlag(Cardinal.Left) ? -1 : 1);

        // start and end position of jump
        _jumpStartPosition = start_position; //global position
        _jumpEndPosition = new Vector2(_jumpStartPosition.X + _jumpCurveWidth * offset, _jumpStartPosition.Y);//global position

        // left control point on bezier curve
        _jumpCurveControl0 = new Vector2(
            x: _jumpStartPosition.X - _controlPoint0Offset.X * offset,
            y: _jumpStartPosition.Y - _jumpCurveHeight + _controlPoint0Offset.Y
        );
        // right control point on bezier curve
        _jumpCurveControl1 = new Vector2(
            x: _jumpStartPosition.X + _controlPoint1Offset.X * offset,
            y: _jumpStartPosition.Y - _jumpCurveHeight + _controlPoint1Offset.Y);

        // reset jump path progres to 0%
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

    public Cardinal GetDirection()
    {
        return _direction;
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