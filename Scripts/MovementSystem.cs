using Godot;
using System;

public partial class MovementSystem : Node
{
    // read-only variables
    [Export]
    private Node2D _traveler;

    [Export]
    private float _runSpeed = 0f;
    [Export]
    private float _jumpSpeed = 500f;

    [ExportGroup("Jump Properties")]
    [Export]
    float _jumpCurveWidth = 0.005f;
    [Export]
    float _jumpCurveHeight = 0.1f;

    [ExportGroup("Debug Settings")]
    [Export]
    private Graph _graph;

    // internal use-only
    private Vector2 _jumpStartPosition;
    private double _jumpPathDelta;

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
    public enum CardinalDirection
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
                _previousMovement = MovementType.IDLE;
                break;
            case MovementType.JUMP:
                // check if jump just started
                if(_previousMovement != MovementType.JUMP)
                {
                    _DrawJumpPath();
                    // reset jump position
                   // _jumpStartPosition = new Vector2(_traveler.GlobalPosition.X, _traveler.GlobalPosition.Y);
                    // reset jump vector
                    _jumpPathDelta = 0.0f;
                }
                // move towards jump target
                _FollowJumpCurve(delta);
                _previousMovement = MovementType.JUMP;
                break;
            case MovementType.RUN:
                // move towards target followRunTarget(delta)
                _previousMovement = MovementType.RUN;
                break;

        }
    }

    private void _FollowJumpCurve(double delta)
    {
        /*
         * Assuming the origin of the graph is the node's jump start position,
         * calculate the relative target position along the parabola,
         * add relative target vector ontop of starting vector to get target position
         * move player to target position
         */

        _jumpPathDelta += delta * _jumpSpeed; //fraction of how far along jump curve we are so far
        Vector2 pointOnJumpCurve = _GetPointOnJumpCurve((float)_jumpPathDelta);
        Vector2 targetPosition = _jumpStartPosition + pointOnJumpCurve;

        _traveler.Position = targetPosition;

    }

    private void _DrawJumpPath()
    {
        int number_points = (int)(1 / _jumpCurveWidth) * 100;
        // generate array of points
        Vector2[] pointsOnJumpCurve = new Vector2[number_points];
        float point_spacing = 0.1f;
        for (int i = 0; i < pointsOnJumpCurve.Length; i++)
        {
            Vector2 vector = _GetPointOnJumpCurve(i * point_spacing);
            pointsOnJumpCurve[i] = new Vector2(vector.X, vector.Y);
        }

        _graph.GlobalPosition = _traveler.GlobalPosition;

        _graph.PopulateCurve2D(pointsOnJumpCurve);
    }
    private Vector2 _GetPointOnJumpCurve(float relative_delta) //x value where origin is jump start position
    {
        /** For more complex curves: 
         * //https://docs.godotengine.org/en/stable/tutorials/math/beziers_and_curves.html
         */
        float jump_curve_offset = (1 / _jumpCurveWidth);
        float delta = relative_delta - jump_curve_offset; // should start at the first x-intercept;

        Vector2 pointOnJumpCurve = Vector2.Zero;
        pointOnJumpCurve.Y = (_jumpCurveWidth * delta * delta) + (_jumpCurveHeight * delta) - jump_curve_offset;
        pointOnJumpCurve.X = relative_delta;

        return pointOnJumpCurve;
    }

    private void _OnJumpInput()
    {
        // Only jump if we aren't already
        if (_currentMovement != MovementType.JUMP)
        {
            _currentMovement = MovementType.JUMP;
           _jumpStartPosition = _traveler.Position;
            _jumpPathDelta = 0;
        }
        GD.Print("Jump Input Recieved by Movement System;");
    }

    private void _OnRunInput(MovementSystem.CardinalDirection cardinalDirection)
    {
        //if state is jump
        //set nextState to Run

        //else
        //set state to Run if not already
        //update velocity based on input
        GD.Print("Run Input Received by Movement System" + cardinalDirection);

    }

}